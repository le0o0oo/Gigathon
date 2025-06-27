using Solitario.Game.Data;
using Solitario.Game.Models;

namespace Solitario.Game.Managers;

internal class Deck {
  private readonly List<Card> cards = []; // Lista che rappresenta le carte del mazzo non prese
  private readonly List<Card> waste = []; // Lista che rappresenta le carte della pila degli scarti
  private static readonly Random rng = new();

  /// <summary>
  /// Genera le carte
  /// </summary>
  private void GenerateCards() {
    CardSeed[] seeds = { CardSeed.Spades, CardSeed.Hearts, CardSeed.Diamonds, CardSeed.Clubs };

    foreach (var seed in seeds) {
      for (byte i = 1; i <= 13; i++) {
        cards.Add(new Card(seed, i));
      }
    }
  }

  public Deck(List<Card> cards, List<Card> waste) {
    this.cards = cards;
    this.waste = waste;
  }

  public Deck() {
    GenerateCards();
    Shuffle();
  }

  /// <summary>
  /// Mischia le carte del mazzo (non degli scarti)
  /// </summary>
  internal void Shuffle() {
    for (int i = cards.Count - 1; i > 0; i--) {
      int j = rng.Next(i + 1);
      (cards[i], cards[j]) = (cards[j], cards[i]);
    }
  }

  /// <summary>
  /// Restituisce una carta dato un indice specificato
  /// </summary>
  /// <param name="index"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  internal Card GetCardAt(int index) {
    if (index < 0 || index >= cards.Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della pila di carte.");
    }
    return cards[index];
  }

  /// <summary>
  /// Rimuove e restiuisce una carta dato un indice specificato dal mazzo (non dagli scarti)
  /// </summary>
  /// <param name="index">Indice della carta da rimuovere</param>
  /// <returns>L'oggetto <see cref="Card"/> che deve essere rimosso</returns>
  /// <exception cref="ArgumentOutOfRangeException">Triggerata se <paramref name="index"/> non è tra i limiti della lista</exception>
  internal Card TakeCardAt(int index) {
    if (index < 0 || index >= cards.Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della pila di carte.");
    }

    Card card = cards[index];
    cards.RemoveAt(index);
    return card;
  }

  /// <summary>
  /// Restituisce la lista del mazzo cruda
  /// </summary>
  /// <returns></returns>
  public List<Card> GetCards() {
    return cards;
  }

  /// <summary>
  /// Restituisce la lista degli scarti cruda
  /// </summary>
  /// <returns></returns>
  internal List<Card> GetWaste() {
    return waste;
  }

  /// <summary>
  /// Rimuove la carta più in alto dal mazzo e la aggiunge alla pila degli scarti
  /// </summary>
  /// <remarks>Viene modificato lo stato del mazzo e della pila degli scarti</remarks>
  internal void PickCard() {
    if (cards.Count == 0) {
      cards.AddRange(waste);
      waste.Clear();
      if (CurrentSettings.ShuffleEmptyDeck) Shuffle();
      return;
    }
    Card card = cards[0];
    cards.RemoveAt(0);
    waste.Add(card);
  }

  /// <summary>
  /// Aggiunge una carta al mazzo ad un indice specifico
  /// </summary>
  /// <param name="card">Carta da inserire</param>
  /// <param name="index">Indice</param>
  internal void AddToDeckAt(Card card, int index) {
    cards.Insert(index, card);
  }

  /// <summary>
  /// Aggiunge una carta alla pila degli scarti
  /// </summary>
  /// <param name="card">Carta da aggiungere</param>
  internal void AddToWaste(Card card) {
    waste.Add(card);
  }

  /// <summary>
  /// Restituisce la carta più in alto della pila degli scarti, se esiste.
  /// </summary>
  /// <returns></returns>
  internal Card? GetTopWaste() {
    if (waste.Count == 0) return null;

    return waste[^1];
  }

  /// <summary>
  /// Ottiene una carta dalla pila degli scarti dato un indice specificato.
  /// Se l'indice è -1, restituisce la carta più in alto
  /// </summary>
  /// <param name="index"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException">Se l'indice è fuori dai limiti della pila</exception>
  internal Card GetWasteCardAt(int index = -1) {
    if (index == -1) {
      index = waste.Count - 1; // Prendi l'ultima carta della pila di scarti
    }
    if (index < 0 || index >= waste.Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della pila di scarti.");
    }
    return waste[index];
  }

  /// <summary>
  /// Rimuove e restituisce una carta dalla pila degli scarti
  /// </summary>
  /// <param name="index"></param>
  /// <returns></returns>
  /// <remarks>Viene mutata la lista degli scarti</remarks>
  /// <exception cref="ArgumentOutOfRangeException">Se l'indice è fuori dai limiti della pila</exception>
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
