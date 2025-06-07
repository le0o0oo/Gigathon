namespace Solitario.Game.Managers;

/*
 * Clubs - Fiori - 0
 * Hearts - Cuori - 1
 * Spades - Picche - 2
 * Quadri - Diamonds - 3
*/
internal class Foundation {
  private readonly List<Card>[] piles = { new List<Card>(), new List<Card>(), new List<Card>(), new List<Card>() };
  readonly Dictionary<string, int> seedIndexMap = new Dictionary<string, int> {
    { "clubs", 0 },
    { "hearts", 1 },
    { "spades", 2 },
    { "diamonds", 3 }
  };

  public Foundation() {

  }

  internal void AddCard(Card card) {
    if (!ValidateCard(card, seedIndexMap[card.seed])) {
      throw new ArgumentException("Carta non valida per questa fondazione.", nameof(card));
    }

    piles[seedIndexMap[card.seed]].Add(card);
  }

  private bool ValidateCard(Card card, int pileIndex) {
    int lastCardVal = piles[pileIndex].Count > 0 ? piles[pileIndex][^1].numericValue : 0;

    bool result = false;
    if (card.numericValue == lastCardVal + 1) result = true;

    return result;
  }

  internal string GetFoundationArt(int index) {
    string art;
    string cardIcon = index switch
    {
      0 => "♣", // Clubs
      1 => "♥", // Hearts
      2 => "♠", // Spades
      3 => "♦", // Diamonds
      _ => throw new ArgumentOutOfRangeException(nameof(index), "Indice della fondazione non valido.")
    };

    if (piles[index].Count == 0) {
      art =
$@"╔ ═ ═ ═ ═ ═ ╗
           
║           ║
           
║     {cardIcon}     ║
           
║           ║
           
╚ ═ ═ ═ ═ ═ ╝";
    }
    else {
      art = piles[index][^1].GetCardArt();
    }

    return art;
  }
  internal ConsoleColor GetFoundationColor(int index) {
    if (piles[index].Count == 0) return ConsoleColor.DarkGray;
    return piles[index][^1].GetColor();
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

  /// <summary>
  /// Takes the cards from a pile from a starting index. For example, if pile is of length 7, and we give it index 3, it will return all cards starting from that index all the way to 6
  /// </summary>
  /// <param name="index"></param>
  /// <returns></returns>
  /// <remarks>Mutates the pile, removing the cards from that pile</remarks>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  internal List<Card> TakeCards(int index) {
    if (index < 0 || index >= piles.Length) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della fondazione.");
    }

    if (piles[index].Count == 0) return new List<Card>(); // Se la pila è vuota, ritorna una lista vuota

    var cards = new List<Card>();
    for (int i = index; i < piles[index].Count; i++) {
      cards.Add(piles[index][i]);
    }

    return cards;
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
}

