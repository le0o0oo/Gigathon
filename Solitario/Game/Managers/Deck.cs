using Solitario.Game.Models;
using Solitario.Game.Types;

namespace Solitario.Game.Managers;

internal class Deck {
  private readonly List<Card> cards = [];
  private readonly List<Card> waste = [];
  private static readonly Random rng = new();

  private void GenerateCards() {
    CardSeed[] seeds = { CardSeed.Spades, CardSeed.Hearts, CardSeed.Diamonds, CardSeed.Clubs };

    foreach (var seed in seeds) {
      for (byte i = 1; i <= 13; i++) {
        cards.Add(new Card(seed, i));
      }
    }
  }

  public Deck() {
    GenerateCards();
    Shuffle();
  }

  internal void Shuffle() {
    for (int i = cards.Count - 1; i > 0; i--) {
      int j = rng.Next(i + 1);
      (cards[i], cards[j]) = (cards[j], cards[i]);
    }
  }

  internal Card GetCardAt(int index) {
    if (index < 0 || index >= cards.Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della pila di carte.");
    }
    return cards[index];
  }

  internal Card TakeCardAt(int index) {
    if (index < 0 || index >= cards.Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della pila di carte.");
    }

    Card card = cards[index];
    cards.RemoveAt(index);
    return card;
  }

  internal List<Card> GetCards() {
    return cards;
  }
  internal List<Card> GetWaste() {
    return waste;
  }

  /// <summary>
  /// Removes the top card from the deck and adds it to the waste pile.
  /// </summary>
  /// <remarks>This method modifies the state of the deck and waste pile. The top card is removed from the deck
  /// and added to the waste pile.</remarks>
  internal void PickCard() {
    if (cards.Count == 0) {
      cards.AddRange(waste);
      waste.Clear();
      //Shuffle();
      return;
    }
    Card card = cards[0];
    cards.RemoveAt(0);
    waste.Add(card);
  }

  internal Card? GetTopWaste() {
    if (waste.Count == 0) return null;

    return waste[^1];
  }

  internal Card GetWasteCardAt(int index = -1) {
    if (index == -1) {
      index = waste.Count - 1; // Prendi l'ultima carta della pila di scarti
    }
    if (index < 0 || index >= waste.Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della pila di scarti.");
    }
    return waste[index];
  }

  internal Card TakeWasteCardAt(int index = -1) {
    if (index == -1) {
      index = waste.Count - 1; // Prendi l'ultima carta della pila di scarti
    }
    if (index < 0 || index >= waste.Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della pila di scarti.");
    }
    Card card = waste[index];
    waste.RemoveAt(index);
    return card;
  }
}
