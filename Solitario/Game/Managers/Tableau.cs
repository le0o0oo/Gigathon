namespace Solitario.Game.Managers;

internal class Tableau {
  private static readonly Random rng = new();

  internal List<List<Card>> tableau { get; private set; } = [];

  public Tableau(Deck deck) {
    for (byte i = 0; i < 7; i++) { // i = indice della colonna
      tableau.Add([]); // Aggiungi una nuova colonna al tableau
      for (byte j = 0; j <= i; j++) { // j = indice della carta nella colonna
        var card = deck.TakeCardAt(0);
        tableau[i].Add(card);
        if (j == i) card.Revealed = true;
      }
    }
  }

  internal Card GetCard(int column, int index = -1) {
    if (index == -1) {
      index = tableau[column].Count - 1; // Prendi l'ultima carta della colonna
    }
    if (column < 0 || column >= tableau.Count) {
      throw new ArgumentOutOfRangeException(nameof(column), "Colonna fuori dai limiti del tableau.");
    }
    if (index < 0 || index >= tableau[column].Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della colonna.");
    }
    return tableau[column][index];
  }

  internal List<Card> GetPile(int index) {
    return tableau[index];
  }

  internal List<Card> TakeCards(int column, int startIndex) {
    if (column < 0 || column >= tableau.Count) {
      throw new ArgumentOutOfRangeException(nameof(column), "Colonna fuori dai limiti del tableau.");
    }
    if (startIndex < 0 || startIndex >= tableau[column].Count) {
      throw new ArgumentOutOfRangeException(nameof(startIndex), "Indice di partenza fuori dai limiti della colonna.");
    }
    var cards = tableau[column].GetRange(startIndex, tableau[column].Count - startIndex);
    tableau[column].RemoveRange(startIndex, tableau[column].Count - startIndex);
    return cards;
  }

  internal Card TakeCardAt(int column, int index = -1) {
    if (column < 0 || column >= tableau.Count) {
      throw new ArgumentOutOfRangeException(nameof(column), "Colonna fuori dai limiti del tableau.");
    }
    if (index == -1) {
      index = tableau[column].Count - 1; // Prendi l'ultima carta della colonna
    }
    if (index < 0 || index >= tableau[column].Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della colonna.");
    }
    Card card = tableau[column][index];
    tableau[column].RemoveAt(index);
    return card;
  }

  internal Card GetCardAt(int column, int index = -1) {
    if (column < 0 || column >= tableau.Count) {
      throw new ArgumentOutOfRangeException(nameof(column), "Colonna fuori dai limiti del tableau.");
    }
    if (index == -1) {
      index = tableau[column].Count - 1; // Prendi l'ultima carta della colonna
    }
    if (index < 0 || index >= tableau[column].Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della colonna.");
    }
    return tableau[column][index];
  }
}
