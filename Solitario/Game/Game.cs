using Solitario.Game.Managers;

namespace Solitario.Game;

internal class Game {
  internal const byte cardWidth = 15; // (+ offset)
  internal const byte cardHeight = 9;

  Deck deck;
  Tableau tableau;
  Foundation foundation;
  Cursor cursor;

  public Game() {
    deck = new Deck(); // Create a new deck of cards
    tableau = new Tableau(deck); // Create a new tableau with the deck
    foundation = new Foundation(); // Create a new foundation
    cursor = new Cursor(); // Initialize the cursor for card selection

    Utils.SetCurrentGame(this); // Set the current game for utility functions

    Utils.PrintDeck();
    Utils.PrintFoundations();
    Utils.PrintTableau();

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

  private void Input() {
    Console.SetCursorPosition(0, cardHeight * 3);
    Console.WriteLine("Premi ESC per uscire dal gioco");
    Console.WriteLine("Ascoltando per input...");
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
      }
    }
  }
}

