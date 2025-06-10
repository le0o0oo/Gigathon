using Solitario.Game.Managers;
using Solitario.Game.Rendering;

namespace Solitario.Game;

internal class Game {
  internal const byte cardWidth = 15; // (+ offset)
  internal const byte cardHeight = 9;

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
    selection = new Selection(tableau, deck, foundation);
    cursor = new Cursor(tableau, legend, selection, foundation); // Initialize the cursor for card selection

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

    renderer.DrawDeck();
    renderer.DrawFoundations();
    renderer.DrawTableau();
    renderer.DrawSelection(true);

    renderer.DrawCursor();
    legend.Draw();
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
          selection.SetSelection(Selection.Areas.Waste, 0, [deck.GetWasteCardAt(-1)]);
          renderer.DrawSelection();
          legend.SetSelected(true);
          break;

        case ConsoleKey.Spacebar:
          if (cursor.Select()) {
            renderer.DrawAll();
            renderer.DrawSelection();
          }

          break;
        case ConsoleKey.X:
          if (!selection.active) break;
          selection.ClearSelection();
          legend.SetSelected(false);

          if (selection.sourceArea == Selection.Areas.Tableau) renderer.DrawTableau();
          else if (selection.sourceArea == Selection.Areas.Foundation) renderer.DrawFoundations();
          else if (selection.sourceArea == Selection.Areas.Waste) renderer.DrawDeck();

          renderer.DrawCursor();
          break;
      }
    }
  }
}

