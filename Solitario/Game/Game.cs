using Solitario.Game.Managers;

namespace Solitario.Game;

internal class Game {
  internal const byte cardWidth = 15; // (+ offset)
  internal const byte cardHeight = 9;

  Deck deck;
  Tableau tableau;
  Foundation foundation;

  public Game() {
    deck = new Deck(); // Create a new deck of cards
    tableau = new Tableau(deck); // Create a new tableau with the deck
    foundation = new Foundation(); // Create a new foundation

    Utils.SetCurrentGame(this); // Set the current game for utility functions

    Utils.PrintDeck();
    Utils.PrintFoundations();
    Utils.PrintTableau();
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
}

