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
}

