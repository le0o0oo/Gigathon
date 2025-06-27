using Solitario.Game.Data;
using Solitario.Game.Models;

namespace Solitario.Game.Managers;

/*
 * Clubs - Fiori - 0
 * Hearts - Cuori - 1
 * Spades - Picche - 2
 * Quadri - Diamonds - 3
*/
internal class Foundation {
  private readonly List<Card>[] piles = { [], [], [], [] };
  static internal readonly Dictionary<CardSeed, int> seedIndexMap = new() // Mappa che rappresenta il corrispettivo indice della pila di una fondazione
  {
    { CardSeed.Clubs, 0 },
    { CardSeed.Hearts, 1 },
    { CardSeed.Spades, 2 },
    { CardSeed.Diamonds, 3 }
  };

  public Foundation(List<Card>[] piles) {
    this.piles = piles;
  }

  public Foundation() {

  }

  /// <summary>
  /// Aggiunge una carta alle fondazioni. Non è necessario specificare l'indice perché viene trovato automaticamente
  /// </summary>
  /// <param name="card">Carta da aggiungere</param>
  internal void AddCard(Card card) {
    piles[seedIndexMap[card.Seed]].Add(card);
  }

  /// <summary>
  /// Restituisce la lista delle carte di una pila dato un indice partendo da 0
  /// </summary>
  /// <param name="index">Indice</param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException">Se l'indice è fuori dai criteri di ricerca</exception>
  internal List<Card> GetCards(int index) {
    if (index < 0 || index >= piles.Length) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della fondazione.");
    }
    return piles[index];
  }

  /// <summary>
  /// Restituisce una carta dato l'indice della pila di ricerca e l'indice della carta all'interno della pila
  /// </summary>
  /// <param name="pileIndex">Indice della pila</param>
  /// <param name="index">Indice della carta all'interno della pila</param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException">Se uno degli indici è fuori dai criteri di ricerca</exception>
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

  /// <summary>
  /// Restituisce e rimuove una carta dato l'indice della pila di ricerca e l'indice della carta all'interno della pila
  /// </summary>
  /// <param name="pileIndex">Indice della pila</param>
  /// <param name="index">Indice della carta all'interno della pila</param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException">Se uno degli indici non soddisfa i criteri di ricerca</exception>
  /// <exception cref="InvalidOperationException">Se non ci sono carte nella pila specificata</exception>
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

  /// <summary>
  /// Restituisce una pila dato l'indice
  /// </summary>
  /// <param name="index"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException">Se l'indice non soddisfa i criteri di ricerca</exception>
  internal List<Card> GetPile(int index) {
    if (index < 0 || index >= piles.Length) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice della pila fuori dai limiti della fondazione.");
    }
    return piles[index];
  }
}

