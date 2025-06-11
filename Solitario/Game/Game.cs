using Solitario.Game.Managers;
using Solitario.Game.Rendering;
using Solitario.Game.Types;

namespace Solitario.Game;

internal class Game {
  private bool autoplay = false;

  private readonly Deck deck;
  private readonly Tableau tableau;
  private readonly Foundation foundation;
  private readonly Cursor cursor;
  private readonly Legend legend;
  private readonly Selection selection;

  private readonly ConsoleRenderer renderer;

  private readonly Thread resizeThread;

  public Game() {
    deck = new Deck(); // Create a new deck of cards
    tableau = new Tableau(deck); // Create a new tableau with the deck
    legend = new Legend(); // Initialize the legend for the game
    foundation = new Foundation(); // Create a new foundation
    selection = new Selection();
    cursor = new Cursor(tableau); // Initialize the cursor for card selection

    renderer = new ConsoleRenderer(deck, tableau, foundation, cursor, legend, selection);

    Draw();

    // Gestisce ridimensionamento spawnando un nuovo thread in background
    resizeThread = new Thread(() => {
      int lastWidth = Console.WindowWidth;
      int lastHeight = Console.WindowHeight;

      while (true) {
        int currentWidth = Console.WindowWidth;
        int currentHeight = Console.WindowHeight;

        if (currentWidth != lastWidth || currentHeight != lastHeight) {
          Draw();

          lastWidth = currentWidth;
          lastHeight = currentHeight;
        }

        Thread.Sleep(200);
      }
    });

    resizeThread.IsBackground = true;
    resizeThread.Start();

    Input();
  }

  /// <summary>
  /// "Disegna" il gioco nella console da zero.
  /// </summary>
  private void Draw() {
    Console.Clear();

    renderer.DrawDeck();
    renderer.DrawFoundations();
    renderer.DrawTableau();
    renderer.DrawSelection(true);

    renderer.DrawCursor();
    renderer.DrawLegend();
  }

  private void Input() {
    while (true) {
      ConsoleKeyInfo keyInfo = Console.ReadKey(true);

      switch (keyInfo.Key) {
        case ConsoleKey.Escape:
          return; // Exit the game

        case ConsoleKey.UpArrow:
          cursor.MoveUp();
          renderer.DrawCursor();
          break;
        case ConsoleKey.DownArrow:
          cursor.MoveDown();
          renderer.DrawCursor();
          break;
        case ConsoleKey.LeftArrow:
          cursor.MoveLeft();
          renderer.DrawCursor();
          break;
        case ConsoleKey.RightArrow:
          cursor.MoveRight();
          renderer.DrawCursor();
          break;

        case ConsoleKey.R:
          if (selection.active) break;
          deck.PickCard();
          renderer.DrawDeck();
          break;

        case ConsoleKey.E:
          if (selection.active || deck.GetWaste().Count == 0) break;
          selection.SetSelection(Areas.Waste, 0, [deck.GetWasteCardAt(-1)]);
          renderer.DrawSelection();
          legend.SetSelected(true);
          break;

        case ConsoleKey.Spacebar:
          // If the selection state changed
          HandleSelectAction();

          legend.SetSelected(selection.active);

          renderer.DrawAll();
          if (selection.active) renderer.DrawSelection();

          /*if (selection.active) {
            renderer.DrawBasedOnArea(selection.sourceArea);
            renderer.DrawSelection();
          }
          else {
            renderer.DrawBasedOnArea(selection.sourceArea);
            renderer.DrawBasedOnArea(selection.TargetArea);
          } */

          renderer.DrawCursor();
          renderer.DrawLegend();


          break;
        case ConsoleKey.X:
          if (!selection.active) break;
          selection.ClearSelection();
          legend.SetSelected(false);
          renderer.DrawLegend();

          if (selection.sourceArea == Areas.Tableau) renderer.DrawTableau();
          else if (selection.sourceArea == Areas.Foundation) renderer.DrawFoundations();
          else if (selection.sourceArea == Areas.Waste) renderer.DrawDeck();

          renderer.DrawCursor();
          break;
      }
    }
  }


  private void HandleSelectAction() {
    if (selection.active) {
      // --- ATTEMPT TO PLACE CARDS ---
      var targetArea = cursor.CurrentArea; // Tableau or Foundation
      var targetPileIndex = cursor.CurrentItemIndex;

      // Convert the cursor area to a selection area (they are logically the same)
      var selectionTargetArea = targetArea == CursorArea.Tableau ? Areas.Tableau : Areas.Foundation;

      // 1. VALIDATE
      if (!Validator.ValidateCardMove(selection.selectedCards[0], GetPile(selectionTargetArea, targetPileIndex), selectionTargetArea)) {
        return; // Invalid move, do nothing.
      }

      // 2. EXECUTE
      // The move is valid, so we execute it by passing parameters directly.
      PlaceSelectedCards(selection.sourceArea, selection.sourceIndex, selectionTargetArea, targetPileIndex);

      // 3. CLEANUP
      selection.ClearSelection();
      legend.SetSelected(false);


    }
    else {
      // --- ATTEMPT TO PICK UP CARDS ---
      if (cursor.CurrentArea == CursorArea.Tableau) {
        var pile = tableau.GetPile(cursor.CurrentItemIndex);
        if (pile.Count == 0 || cursor.CurrentCardPileIndex >= pile.Count) return;

        var cardsToSelect = new List<Card>();
        for (int i = cursor.CurrentCardPileIndex; i < pile.Count; i++) {
          cardsToSelect.Add(pile[i]);
        }
        if (!cardsToSelect[0].Revealed) return; // Cannot pick up un-revealed cards

        selection.SetSelection(Areas.Tableau, cursor.CurrentItemIndex, cardsToSelect);
        legend.SetSelected(true);
      }
      else { // Cursor is on Foundation
        var pile = foundation.GetPile(cursor.CurrentItemIndex);
        if (pile.Count == 0) return;

        selection.SetSelection(Areas.Foundation, cursor.CurrentItemIndex, [pile[^1]]);
        legend.SetSelected(true);
      }
    }
  }

  // A helper to get a pile from any area
  private List<Card> GetPile(Areas area, int index) {
    return area switch
    {
      Areas.Tableau => tableau.GetPile(index),
      Areas.Foundation => foundation.GetPile(index),
      _ => deck.GetWaste() // Note: you might need a method on Deck for this
    };
  }


  // This method now receives all information it needs as parameters.
  // It is a pure "action" method.
  private void PlaceSelectedCards(Areas sourceArea, int sourceIndex, Areas destArea, int destIndex) {
    // FROM TABLEAU
    if (sourceArea == Areas.Tableau) {
      var cardsToMove = tableau.TakeCards(sourceIndex, tableau.GetPile(sourceIndex).Count - selection.selectedCards.Count);
      if (tableau.GetPile(sourceIndex).Count > 0) {
        tableau.GetPile(sourceIndex)[^1].Revealed = true;
      }

      if (destArea == Areas.Tableau) {
        tableau.GetPile(destIndex).AddRange(cardsToMove);
      }
      else { // To Foundation
        foundation.AddCard(cardsToMove[0]);
      }
    }
    // FROM WASTE
    else if (sourceArea == Areas.Waste) {
      var cardToMove = deck.TakeWasteCardAt(-1);
      if (destArea == Areas.Tableau) {
        tableau.GetPile(destIndex).Add(cardToMove);
      }
      else { // To Foundation
        foundation.AddCard(cardToMove);
      }
    }
    // FROM FOUNDATION
    else if (sourceArea == Areas.Foundation) {
      var cardToMove = foundation.TakeCardAt(sourceIndex);
      tableau.GetPile(destIndex).Add(cardToMove);
    }
  }
}

