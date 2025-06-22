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

  internal InputHandler(Game game, Cursor cursor, Renderer renderer, Selection selection, Legend legend, Deck deck, Tableau tableau, Foundation foundation, Actions actions, Managers.Hint hintsManager) {
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
  }

  internal void ProcessInput(ConsoleKeyInfo keyInfo) {
    bool changedHintState = false;

    switch (keyInfo.Key) {
      case ConsoleKey.Escape:
        game.OnEsc?.Invoke();
        break;

      case ConsoleKey.UpArrow:
      case ConsoleKey.DownArrow:
      case ConsoleKey.LeftArrow:
      case ConsoleKey.RightArrow:
        HandleCursorMovement(keyInfo.Key);
        break;

      case ConsoleKey.R:
        if (selection.Active) break;
        actionsManager.Execute(new DrawCardAction(deck));

        legend.SetCanUndo(actionsManager.CanUndo());
        renderer.DrawDeck();
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
        break;

      case ConsoleKey.X:
        if (!selection.Active) break;
        selection.ClearSelection();
        legend.SetSelected(false);

        renderer.DrawBasedOnArea(selection.SourceArea);
        renderer.DrawCursor();
        break;

      case ConsoleKey.Z:
        if (selection.Active) break;
        if (!actionsManager.CanUndo()) break;

        var action = actionsManager.Undo();
        legend.SetCanUndo(actionsManager.CanUndo());

        if (action is DrawCardAction) {
          renderer.DrawDeck();
        }
        else game.Draw();

        renderer.DrawLegend();
        break;

      case ConsoleKey.H:
        if (selection.Active || !CurrentSettings.UseHints) break;
        var managers = new GameManagers(deck, tableau, foundation, selection, cursor);

        var hint = Hints.FindHint(managers);
        if (hint == null) break;

        if (!hintManager.ShowingHint) {
          hintManager.SetLastAction(hint);
          renderer.DrawAction(hint);
          hintManager.ShowingHint = true;
          changedHintState = true;
        }
        else if (hintManager.LastAction != null) {
          // Non ricacolare hint
          actionsManager.Execute(hintManager.LastAction);
        }
        break;
    }

    if (hintManager.ShowingHint && !changedHintState) {
      hintManager.ShowingHint = false;

      if (hintManager.LastAction is MoveCardsAction action) {
        renderer.DrawBasedOnArea(action.sourceArea);
        renderer.DrawBasedOnArea(action.destArea);
      }
      else if (hintManager.LastAction is DrawCardAction) {
        renderer.DrawDeck();
      }

      renderer.DrawCursor();
      if (selection.Active) renderer.DrawSelection();
    }

  }

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
    }
    // If a selection was just made
    else if (!wasActive && selection.Active) {
      renderer.DrawSelection();
    }

    legend.SetSelected(selection.Active);
    renderer.DrawCursor();
  }

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

    actionsManager.Execute(new MoveCardsAction(managers, sourceArea, sourceIndex, destArea, destIndex));
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
