using Solitario.Game.Managers;
using Solitario.Game.Models;
using Solitario.Game.Models.Actions;
using Solitario.Game.Rendering;

namespace Solitario.Game;

internal class Game {
  private bool autoplay = false;

  private readonly Deck deck;
  private readonly Tableau tableau;
  private readonly Foundation foundation;
  private readonly Cursor cursor;
  private readonly Legend legend;
  private readonly Selection selection;
  private readonly Managers.Hint hintManager;

  private readonly ConsoleRenderer renderer;
  private readonly Actions actionsManager;

  internal record GameManagers(Deck Deck, Tableau Tableau, Foundation Foundation, Selection Selection, Cursor cursor);

  internal Action? OnWin;

  public Game() {
    deck = new Deck(); // Create a new deck of cards
    tableau = new Tableau(deck); // Create a new tableau with the deck
    legend = new Legend(); // Initialize the legend for the game
    foundation = new Foundation(); // Create a new foundation
    selection = new Selection();
    cursor = new Cursor(tableau); // Initialize the cursor for card selection
    hintManager = new Managers.Hint();

    renderer = new ConsoleRenderer(deck, tableau, foundation, cursor, legend, selection, hintManager);

    actionsManager = new Actions();

  }

  /// <summary>
  /// "Disegna" il gioco nella console da zero.
  /// </summary>
  internal void Draw() {
    Console.Clear();
    if (!ConsoleRenderer.CanDraw()) {
      Console.SetCursorPosition(0, 0);
      Console.WriteLine($"Please resize your console window to at least {ConsoleRenderer.minWidth}x{ConsoleRenderer.minHeight}");
      Console.WriteLine($"Current size: {Console.WindowWidth}x{Console.WindowHeight}");
      return;
    }

    legend.SetCanUndo(actionsManager.CanUndo());

    renderer.DrawDeck();
    renderer.DrawFoundations();
    renderer.DrawTableau();
    renderer.DrawSelection(true);

    renderer.DrawCursor();
    renderer.DrawLegend();
  }

  // Ottieni pila da ogni area
  private List<Card> GetPile(Areas area, int index) {
    return area switch
    {
      Areas.Tableau => tableau.GetPile(index),
      Areas.Foundation => foundation.GetPile(index),
      _ => deck.GetWaste()
    };
  }

  #region Input handlers
  private void HandleKeyPress(ConsoleKey key) {
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
    var wasActive = selection.active;
    var sourceAreaBeforeMove = selection.sourceArea;

    HandleSelectAction();

    // If a move was just completed (selection is now inactive)
    if (wasActive && !selection.active) {
      // Redraw the source and destination areas
      var destArea = cursor.CurrentArea;
      renderer.DrawBasedOnArea(sourceAreaBeforeMove);
      renderer.DrawBasedOnArea(destArea);
    }
    // If a selection was just made
    else if (!wasActive && selection.active) {
      renderer.DrawSelection();
    }

    legend.SetSelected(selection.active);
    renderer.DrawCursor();
  }
  #endregion

  internal void Input(ConsoleKeyInfo keyInfo) {
    bool changedHintState = false;

    switch (keyInfo.Key) {
      case ConsoleKey.UpArrow:
      case ConsoleKey.DownArrow:
      case ConsoleKey.LeftArrow:
      case ConsoleKey.RightArrow:
        HandleKeyPress(keyInfo.Key);
        break;

      case ConsoleKey.R:
        if (selection.active) break;
        actionsManager.Execute(new DrawCardAction(deck));

        legend.SetCanUndo(actionsManager.CanUndo());
        renderer.DrawDeck();
        break;

      case ConsoleKey.E:
        if (selection.active || deck.GetWaste().Count == 0) break;
        selection.SetSelection(Areas.Waste, 0, [deck.GetWasteCardAt(-1)]);
        legend.SetSelected(true);

        renderer.DrawSelection();
        break;

      case ConsoleKey.Spacebar:
        HandleSelection();
        break;

      case ConsoleKey.X:
        if (!selection.active) break;
        selection.ClearSelection();
        legend.SetSelected(false);

        renderer.DrawBasedOnArea(selection.sourceArea);
        renderer.DrawCursor();
        break;

      case ConsoleKey.Z:
        if (selection.active) break;
        if (!actionsManager.CanUndo()) break;

        var action = actionsManager.Undo();
        legend.SetCanUndo(actionsManager.CanUndo());

        if (action is DrawCardAction) {
          renderer.DrawDeck();
        }
        else Draw();

        renderer.DrawLegend();
        break;

      case ConsoleKey.H:
        if (selection.active) break;
        var managers = new GameManagers(deck, tableau, foundation, selection, cursor);

        var hint = Hints.FindHint(managers);
        if (hint == null) break;

        if (!hintManager.ShowingHint) {
          hintManager.SetLastAction(hint);
          renderer.DrawAction(managers, hint);
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
      if (selection.active) renderer.DrawSelection();
    }

    renderer.DrawLegend();

    if (HasWon()) {
      OnWin?.Invoke();
    }
  }

  private bool HasWon() {
    return foundation.GetPile(0).Count == 13 &&
       foundation.GetPile(1).Count == 13 &&
       foundation.GetPile(2).Count == 13 &&
       foundation.GetPile(3).Count == 13;
  }

  private void HandleSelectAction() {
    if (selection.active) {
      // Piazza le carte
      var targetArea = cursor.CurrentArea; // Tableau o fondazioni
      var targetPileIndex = cursor.CurrentItemIndex;

      // Convert the cursor area to a selection area (they are logically the same)
      var selectionTargetArea = targetArea;

      // 1. Controlla se mossa valida
      if (!Validator.ValidateCardMove(selection.selectedCards[0], GetPile(selectionTargetArea, targetPileIndex), selectionTargetArea, targetPileIndex)) {
        return; // Invalid move, do nothing.
      }

      // 2. Piazza le carte
      PlaceSelectedCards(selection.sourceArea, selection.sourceIndex, selectionTargetArea, targetPileIndex);

      // 3. Cleanup
      selection.ClearSelection();
      legend.SetSelected(false);
    }
    else {
      // Prendi le carte da tableau
      if (cursor.CurrentArea == Areas.Tableau) {
        var pile = tableau.GetPile(cursor.CurrentItemIndex);
        if (pile.Count == 0 || cursor.CurrentCardPileIndex >= pile.Count) return;

        var cardsToSelect = new List<Card>();
        for (int i = cursor.CurrentCardPileIndex; i < pile.Count; i++) {
          cardsToSelect.Add(pile[i]);
        }
        if (!cardsToSelect[0].Revealed) return; // Carte non rivelate, non prenderle

        cursor.SelectionPosition[0] = cursor.CurrentItemIndex;
        cursor.SelectionPosition[1] = cursor.CurrentCardPileIndex;

        selection.SetSelection(Areas.Tableau, cursor.CurrentItemIndex, cardsToSelect);
        legend.SetSelected(true);
      }
      else { // Cursore nella fondazione, prende da essa.
        var pile = foundation.GetPile(cursor.CurrentItemIndex);
        if (pile.Count == 0) return;

        cursor.SelectionPosition[0] = cursor.CurrentItemIndex;
        cursor.SelectionPosition[1] = cursor.CurrentCardPileIndex;

        selection.SetSelection(Areas.Foundation, cursor.CurrentItemIndex, [pile[^1]]);
        legend.SetSelected(true);
      }
    }

    legend.SetCanUndo(actionsManager.CanUndo());

  }

  // This method now receives all information it needs as parameters.
  private void PlaceSelectedCards(Areas sourceArea, int sourceIndex, Areas destArea, int destIndex) {
    var managers = new GameManagers(deck, tableau, foundation, selection, cursor);

    actionsManager.Execute(new MoveCardsAction(managers, sourceArea, sourceIndex, destArea, destIndex));
  }
}

