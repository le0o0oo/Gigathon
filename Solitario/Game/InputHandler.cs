using Solitario.Game.Helpers;
using Solitario.Game.Managers;
using Solitario.Game.Models;
using Solitario.Game.Models.Actions;
using Solitario.Game.Rendering;
using static Solitario.Game.Game;

namespace Solitario.Game;
internal class InputHandler {
  private readonly Game game;
  private readonly Cursor cursor;
  private readonly Renderer renderer;
  private readonly Selection selection;
  private readonly Legend legend;
  private readonly Deck deck;
  private readonly Tableau tableau;
  private readonly Foundation foundation;
  private readonly Actions actionsManager;
  private readonly Managers.Hint hintManager;
  private readonly Stats statsManager;

  internal InputHandler(Game game, Cursor cursor, Renderer renderer, Selection selection, Legend legend, Deck deck, Tableau tableau, Foundation foundation, Actions actions, Managers.Hint hintsManager, Stats statsManager) {
    this.game = game;
    this.cursor = cursor;
    this.renderer = renderer;
    this.selection = selection;
    this.legend = legend;
    this.deck = deck;
    this.tableau = tableau;
    this.foundation = foundation;
    this.actionsManager = actions;
    this.hintManager = hintsManager;
    this.statsManager = statsManager;
  }

  internal void ProcessInput(ConsoleKeyInfo keyInfo) {
    bool changedHintState = false;

    switch (keyInfo.Key) {
      case ConsoleKey.Escape:
        game.OnEsc?.Invoke();
        return;

      case ConsoleKey.UpArrow:
      case ConsoleKey.DownArrow:
      case ConsoleKey.LeftArrow:
      case ConsoleKey.RightArrow:
        HandleCursorMovement(keyInfo.Key);
        break;

      case ConsoleKey.R:
        if (selection.Active) break;
        var drawAction = new DrawCardAction(deck);
        statsManager.ApplyActionScore(drawAction);
        actionsManager.Execute(drawAction);
        statsManager.IncMovesCount();

        legend.SetCanUndo(actionsManager.CanUndo());
        renderer.DrawDeck();
        renderer.DrawStats();
        break;

      case ConsoleKey.E:
        if (selection.Active || deck.GetWaste().Count == 0) break;
        selection.SetSelection(Areas.Deck, 0, [deck.GetWasteCardAt(-1)]);
        legend.SetSelected(true);

        renderer.DrawSelection();
        break;

      case ConsoleKey.Enter:
      case ConsoleKey.Spacebar:
        HandleSelection();
        renderer.DrawStats();
        break;

      case ConsoleKey.X:
        if (!selection.Active) break;
        selection.ClearSelection();
        legend.SetSelected(false);

        renderer.DrawBasedOnArea(selection.SourceArea);
        renderer.DrawCursor();
        break;

      case ConsoleKey.F:
        ToFoundation();
        break;

      case ConsoleKey.Z:
        if (selection.Active) break;
        if (!actionsManager.CanUndo()) break;

        var undoAction = actionsManager.Undo();
        statsManager.ApplyUndoPenality();
        statsManager.RemoveActionScore(undoAction);
        statsManager.IncUndosCount();
        statsManager.DecMovesCount();

        legend.SetCanUndo(actionsManager.CanUndo());

        if (undoAction is DrawCardAction) {
          renderer.DrawDeck();
        }
        else game.Draw();

        renderer.DrawLegend();
        renderer.DrawStats();
        break;

      case ConsoleKey.H:
        if (selection.Active || !CurrentSettings.UseHints) break;
        var managers = new GameManagers(deck, tableau, foundation, selection, cursor);

        var hint = Hints.FindHint(managers);
        if (hint == null) break;

        if (!hintManager.ShowingHint) {
          statsManager.IncHintsCount();
          statsManager.ApplyHintPenalty();
          hintManager.SetLastAction(hint);
          renderer.DrawAction(hint);
          hintManager.ShowingHint = true;
          changedHintState = true;
        }
        else if (hintManager.LastAction != null) {
          // Non ricacolare hint
          statsManager.ApplyActionScore(hintManager.LastAction!);
          actionsManager.Execute(hintManager.LastAction);
        }

        renderer.DrawStats();
        break;
    }

    if (hintManager.ShowingHint && !changedHintState) {
      hintManager.ShowingHint = false;

      statsManager.IncMovesCount();

      if (hintManager.LastAction is MoveCardsAction action) {
        renderer.DrawBasedOnArea(action.sourceArea);
        renderer.DrawBasedOnArea(action.destArea);
      }
      else if (hintManager.LastAction is DrawCardAction) {
        renderer.DrawDeck();
      }

      renderer.DrawCursor();
      if (selection.Active) renderer.DrawSelection();
      renderer.DrawStats();
    }

    legend.CanShortCutFoundation = !(selection.Active && selection.SourceArea == Areas.Tableau);
  }

  /// <summary>
  /// Porta automaticamente la carta selezionata in fondazione, se possibile.
  /// </summary>
  private void ToFoundation() {
    Card card;
    Selection movSelection = new();
    Areas area;
    if (selection.Active && selection.SourceArea == Areas.Deck) {
      card = selection.SelectedCards[0]; // Prende la prima carta della selezione
      area = Areas.Deck;
      movSelection.SetSelection(Areas.Deck, 0, [card]);
    }
    else if (cursor.CurrentArea == Areas.Tableau && !selection.Active) {
      if (cursor.CurrentCardIndex != tableau.GetPile(cursor.CurrentItemIndex).Count - 1) return; // Non puoi piazzare in fondazione se non hai selezionato l'ultima carta
      card = tableau.GetCard(cursor.CurrentItemIndex);
      area = Areas.Tableau;
      movSelection.SetSelection(Areas.Tableau, cursor.CurrentItemIndex, [card]);
    }
    else return;

    var foundationPileIndex = Foundation.seedIndexMap[card.Seed];
    if (!Validator.ValidateCardMove(card, foundation.GetPile(foundationPileIndex), Areas.Foundation, foundationPileIndex)) return; // Non puoi piazzare in fondazione se non è valida
    selection.ClearSelection();
    legend.SetSelected(false);

    var managers = new GameManagers(deck, tableau, foundation, selection, cursor);
    var action = new MoveCardsAction(managers, area, cursor.CurrentItemIndex, Areas.Foundation, foundationPileIndex, movSelection);

    statsManager.IncMovesCount();
    statsManager.ApplyActionScore(action);
    actionsManager.Execute(action);
    cursor.DecCurrentCardIndex();

    renderer.DrawBasedOnArea(area);
    renderer.DrawFoundations();
    renderer.DrawStats();
    renderer.DrawCursor();
  }

  /// <summary>
  /// Gestisce il movimento del cursore in base ai tasti premuti.
  /// </summary>
  /// <param name="key"></param>
  private void HandleCursorMovement(ConsoleKey key) {
    switch (key) {
      case ConsoleKey.UpArrow:
        cursor.MoveUp();
        break;
      case ConsoleKey.DownArrow:
        cursor.MoveDown();
        break;
      case ConsoleKey.LeftArrow:
        cursor.MoveLeft();
        break;
      case ConsoleKey.RightArrow:
        cursor.MoveRight();
        break;
    }
    renderer.DrawCursor();
  }

  /// <summary>
  /// Gestisce la selezione attuale delle carte, ridisegnado le aree interessate.
  /// </summary>
  private void HandleSelection() {
    var wasActive = selection.Active;
    var sourceAreaBeforeMove = selection.SourceArea;

    HandleSelectAction();

    // If a move was just completed (selection is now inactive)
    if (wasActive && !selection.Active) {
      // Redraw the source and destination areas
      var destArea = cursor.CurrentArea;
      renderer.DrawBasedOnArea(sourceAreaBeforeMove);
      renderer.DrawBasedOnArea(destArea);
      statsManager.IncMovesCount();
    }
    // If a selection was just made
    else if (!wasActive && selection.Active) {
      renderer.DrawSelection();
    }

    legend.SetSelected(selection.Active);
    renderer.DrawCursor();
  }

  /// <summary>
  /// Gestisce l'azione di selezione delle carte, sia per piazzarle che per prenderle.
  /// </summary>
  private void HandleSelectAction() {
    if (selection.Active) {
      // Piazza le carte
      var targetArea = cursor.CurrentArea; // Tableau o fondazioni
      var targetPileIndex = cursor.CurrentItemIndex;

      // 1. Controlla se mossa valida
      if (!Validator.ValidateCardMove(selection.SelectedCards[0], GetPile(targetArea, targetPileIndex), targetArea, targetPileIndex)) {
        return; // Invalid move, do nothing.
      }

      // 2. Piazza le carte
      PlaceSelectedCards(selection.SourceArea, selection.SourceIndex, targetArea, targetPileIndex);

      // 3. Cleanup
      selection.ClearSelection();
      legend.SetSelected(false);
    }
    else {
      // Prendi le carte da tableau
      if (cursor.CurrentArea == Areas.Tableau) {
        var pile = tableau.GetPile(cursor.CurrentItemIndex);
        if (pile.Count == 0 || cursor.CurrentCardIndex >= pile.Count) return;

        var cardsToSelect = new List<Card>();
        for (int i = cursor.CurrentCardIndex; i < pile.Count; i++) {
          cardsToSelect.Add(pile[i]);
        }
        if (!cardsToSelect[0].Revealed) return; // Carte non rivelate, non prenderle

        cursor.SelectionPosition[0] = cursor.CurrentItemIndex;
        cursor.SelectionPosition[1] = cursor.CurrentCardIndex;

        selection.SetSelection(Areas.Tableau, cursor.CurrentItemIndex, cardsToSelect);
        legend.SetSelected(true);
      }
      else { // Cursore nella fondazione, prende da essa.
        var pile = foundation.GetPile(cursor.CurrentItemIndex);
        if (pile.Count == 0) return;

        cursor.SelectionPosition[0] = cursor.CurrentItemIndex;
        cursor.SelectionPosition[1] = cursor.CurrentCardIndex;

        selection.SetSelection(Areas.Foundation, cursor.CurrentItemIndex, [pile[^1]]);
        legend.SetSelected(true);
      }
    }

    legend.SetCanUndo(actionsManager.CanUndo());

  }

  // Piazza le carte della selezione.
  private void PlaceSelectedCards(Areas sourceArea, int sourceIndex, Areas destArea, int destIndex) {
    var managers = new GameManagers(deck, tableau, foundation, selection, cursor);
    var action = new MoveCardsAction(managers, sourceArea, sourceIndex, destArea, destIndex);

    statsManager.ApplyActionScore(action);
    actionsManager.Execute(action);
  }

  /// <summary>
  /// Ottieni pila da ogni area
  /// </summary>
  /// <param name="area"></param>
  /// <param name="index"></param>
  /// <returns></returns>
  private List<Card> GetPile(Areas area, int index) {
    return area switch
    {
      Areas.Tableau => tableau.GetPile(index),
      Areas.Foundation => foundation.GetPile(index),
      _ => deck.GetWaste()
    };
  }
}
