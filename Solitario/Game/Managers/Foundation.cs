using Solitario.Game.Models;

namespace Solitario.Game.Managers;

/*
 * Clubs - Fiori - 0
 * Hearts - Cuori - 1
 * Spades - Picche - 2
 * Quadri - Diamonds - 3
*/
internal class Foundation {
  private readonly List<Card>[] piles = { new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>() };
  static internal readonly Dictionary<CardSeed, int> seedIndexMap = new Dictionary<CardSeed, int> {
    { CardSeed.Clubs, 0 },
    { CardSeed.Hearts, 1 },
    { CardSeed.Spades, 2 },
    { CardSeed.Diamonds, 3 }
  };

  public Foundation() {

  }

  internal void AddCard(Card card) {
    var pile = piles[seedIndexMap[card.Seed]];

    piles[seedIndexMap[card.Seed]].Add(card);
  }

  internal List<Card> GetCards(int index) {
    if (index < 0 || index >= piles.Length) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della fondazione.");
    }
    return piles[index];
  }
  internal Card GetCardAt(int pileIndex, int index = -1) {
    if (index == -1) {
      index = piles[pileIndex].Count - 1; // Prendi l'ultima carta della pila
    }
    if (pileIndex < 0 || pileIndex >= piles.Length) {
      throw new ArgumentOutOfRangeException(nameof(pileIndex), "Indice della pila fuori dai limiti della fondazione.");
    }
    if (index < 0 || index >= piles[pileIndex].Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice della carta fuori dai limiti della pila.");
    }
    return piles[pileIndex][index];
  }

  internal Card TakeCardAt(int pileIndex, int index = -1) {
    if (pileIndex < 0 || pileIndex >= piles.Length) {
      throw new ArgumentOutOfRangeException(nameof(pileIndex), "Indice della pila fuori dai limiti della fondazione.");
    }
    if (piles[pileIndex].Count == 0) {
      throw new InvalidOperationException("Non ci sono carte nella pila specificata.");
    }
    if (index == -1) {
      index = piles[pileIndex].Count - 1; // Prendi l'ultima carta della pila
    }
    if (index < 0 || index >= piles[pileIndex].Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice della carta fuori dai limiti della pila.");
    }
    Card card = piles[pileIndex][index];
    piles[pileIndex].RemoveAt(index);
    return card;
  }

  internal List<Card> GetPile(int index) {
    if (index < 0 || index >= piles.Length) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice della pila fuori dai limiti della fondazione.");
    }
    return piles[index];
  }
}

