using Solitario.Game.Managers;

namespace Solitario.Game;

internal class Game {
  internal const byte cardWidth = 15; // (+ offset)
  internal const byte cardHeight = 9;

  private bool autoplay = false;

  Deck deck;
  Tableau tableau;
  Foundation foundation;
  Cursor cursor;
  Legend legend;
  Selection selection;

  public Game() {
    deck = new Deck(); // Create a new deck of cards
    tableau = new Tableau(deck); // Create a new tableau with the deck
    legend = new Legend(); // Initialize the legend for the game
    foundation = new Foundation(); // Create a new foundation
    selection = new Selection(tableau, deck, foundation);
    cursor = new Cursor(tableau, legend, selection, foundation, deck); // Initialize the cursor for card selection

    Draw();

    // Gestisce ridimensionamento spawnando un nuovo thread in background
    Thread resizeThread = new Thread(() => {
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

  internal Deck GetDeck() {
    return deck;
  }
  internal Tableau GetTableau() {
    return tableau;
  }
  internal Foundation GetFoundation() {
    return foundation;
  }

  /// <summary>
  /// "Disegna" il gioco nella console da zero.
  /// </summary>
  internal void Draw() {
    Console.Clear();

    Utils.SetCurrentGame(this); // Set the current game for utility functions

    Utils.PrintDeck();
    Utils.PrintFoundations();
    Utils.PrintTableau();
    cursor.DrawSelection(true);

    cursor.Draw();
    legend.Draw();
  }

  private void Input() {
    while (true) {
      ConsoleKeyInfo keyInfo = Console.ReadKey(true);

      switch (keyInfo.Key) {
        case ConsoleKey.Escape:
          return; // Exit the game

        case ConsoleKey.UpArrow:
          cursor.MoveUp(); // Move the cursor up
          break;
        case ConsoleKey.DownArrow:
          cursor.MoveDown(); // Move the cursor down
          break;
        case ConsoleKey.LeftArrow:
          cursor.MoveLeft(); // Move the cursor left
          break;
        case ConsoleKey.RightArrow:
          cursor.MoveRight(); // Move the cursor right
          break;

        case ConsoleKey.R:
          if (selection.active) break;
          deck.PickCard();
          Utils.PrintDeck();
          break;

        case ConsoleKey.E:
          if (selection.active || deck.GetWaste().Count == 0) break;
          selection.SetSelection(Selection.Areas.Waste, 0, [deck.GetWasteCardAt(-1)]);
          cursor.DrawSelection();
          legend.SetSelected(true);
          break;

        case ConsoleKey.Spacebar:
          cursor.Select();
          break;
        case ConsoleKey.X:
          if (!selection.active) break;
          selection.ClearSelection();
          legend.SetSelected(false);

          if (selection.sourceArea == Selection.Areas.Tableau) Utils.PrintTableau();
          else if (selection.sourceArea == Selection.Areas.Foundation) Utils.PrintFoundations();
          else if (selection.sourceArea == Selection.Areas.Waste) Utils.PrintDeck();

          cursor.Draw();
          break;
      }
    }
  }
}

