using Solitario.Game.Models;

namespace Solitario.Game.Managers;

internal class Tableau {
  private static readonly Random rng = new();

  /// <summary>
  /// Le 7 pile del tableau
  /// </summary>
  internal List<List<Card>> Piles { get; private set; } = [];

  public Tableau(List<List<Card>> tableau) {
    this.Piles = tableau;
  }

  public Tableau(Deck deck) {
    for (byte i = 0; i < 7; i++) { // i = indice della colonna
      Piles.Add([]); // Aggiungi una nuova colonna al tableau
      for (byte j = 0; j <= i; j++) { // j = indice della carta nella colonna
        var card = deck.TakeCardAt(0);
        Piles[i].Add(card);
        if (j == i) card.Revealed = true;
      }
    }
  }

  /// <summary>
  /// Restituisce una carta dato l'indice della colonna e l'indice della carta nella pila
  /// </summary>
  /// <param name="column">Indice della pila del tableau</param>
  /// <param name="index">Indice della carta nella pila. Se è -1 restiutisce l'ultima carta della colonna (quella più fuori)</param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException">Se uno degli indici è fuori dai criteri di ricerca</exception>
  internal Card GetCard(int column, int index = -1) {
    if (index == -1) {
      index = Piles[column].Count - 1; // Prendi l'ultima carta della colonna
    }
    if (column < 0 || column >= Piles.Count) {
      throw new ArgumentOutOfRangeException(nameof(column), "Colonna fuori dai limiti del tableau.");
    }
    if (index < 0 || index >= Piles[column].Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della colonna.");
    }
    return Piles[column][index];
  }

  /// <summary>
  /// Restituisce una lista di carte corrispondente alla pila dato il suo indice
  /// </summary>
  /// <param name="index"></param>
  /// <returns></returns>
  internal List<Card> GetPile(int index) {
    return Piles[index];
  }

  /// <summary>
  /// Rimuove carte dato l'indice della pila e l'indice di inizio.
  /// Le carte vengono rimosso verso il basso (fino a quella più fuori)
  /// </summary>
  /// <param name="column">Indice della pila</param>
  /// <param name="startIndex">Indice della carta nella pila</param>
  /// <returns></returns>
  /// <remarks>Muta la pila del tableau</remarks>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  internal List<Card> TakeCards(int column, int startIndex) {
    if (column < 0 || column >= Piles.Count) {
      throw new ArgumentOutOfRangeException(nameof(column), "Colonna fuori dai limiti del tableau.");
    }
    if (startIndex < 0 || startIndex >= Piles[column].Count) {
      throw new ArgumentOutOfRangeException(nameof(startIndex), "Indice di partenza fuori dai limiti della colonna.");
    }
    var cards = Piles[column].GetRange(startIndex, Piles[column].Count - startIndex);
    Piles[column].RemoveRange(startIndex, Piles[column].Count - startIndex);
    return cards;
  }

  /// <summary>
  /// Rimuove e restituisce una carta dato l'indice della pila e l'indice della carta nella pila
  /// </summary>
  /// <param name="column">Indice della pila</param>
  /// <param name="index">Indice della carta nella pila</param>
  /// <returns></returns>
  /// <remarks>Muta la pila del tableau</remarks>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  internal Card TakeCardAt(int column, int index = -1) {
    if (column < 0 || column >= Piles.Count) {
      throw new ArgumentOutOfRangeException(nameof(column), "Colonna fuori dai limiti del tableau.");
    }
    if (index == -1) {
      index = Piles[column].Count - 1; // Prendi l'ultima carta della colonna
    }
    if (index < 0 || index >= Piles[column].Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della colonna.");
    }
    Card card = Piles[column][index];
    Piles[column].RemoveAt(index);
    return card;
  }
}
