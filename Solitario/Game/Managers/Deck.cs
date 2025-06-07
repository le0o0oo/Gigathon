namespace Solitario.Game.Managers;

internal class Deck {
  private List<Card> cards = new();
  private List<Card> waste = new();
  private static readonly Random rng = new();

  private void GenerateCards() {
    string[] seeds = { "spades", "hearts", "diamonds", "clubs" };

    foreach (string seed in seeds) {
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
      cards.AddRange(waste.AsEnumerable().Reverse());
      waste.Clear();
      //Shuffle();
      return;
    }
    Card card = cards[0];
    cards.RemoveAt(0);
    waste.Insert(0, card);
  }

  internal string GetWasteArt() {
    string art;

    if (waste.Count == 0) {
      art = Utils.GetEmptyCardArt();
    }
    else {
      art = waste[0].GetCardArt();
    }

    return art;
  }

  internal ConsoleColor GetWasteColor() {
    if (waste.Count == 0) return ConsoleColor.DarkGray;
    return waste[0].GetColor(true);
  }
}
