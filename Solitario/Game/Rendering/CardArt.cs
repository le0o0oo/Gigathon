using Solitario.Game.Models;

namespace Solitario.Game.Rendering;
internal static class CardArt {
  internal static readonly byte cardWidth = 15; // (+ offset)
  internal static readonly byte cardHeight = 9;

  /// <summary>
  /// Get the color of the card based on its seed and revealed state.
  /// </summary>
  /// <returns></returns>
  internal static ConsoleColor GetColor(Card card, bool force = false) {
    if (!card.Revealed && !force) return ConsoleColor.DarkGray;

    return card.Color == CardColor.Red ? ConsoleColor.Red : ConsoleColor.White;
  }

  private static char GetSeedIcon(CardSeed seed) {
    return seed switch
    {
      CardSeed.Spades => '♠',
      CardSeed.Hearts => '♥',
      CardSeed.Diamonds => '♦',
      CardSeed.Clubs => '♣',
      _ => throw new InvalidOperationException("Seme non riconosciuto.")
    };
  }

  internal static string GetCharacter(Card card) {
    if (card.Rank == "10") return "10";

    return card.Rank[0].ToString().ToUpper();
  }

  /// <summary>
  /// Restituisce una "immagine" della carta in formato stringa.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException">Triggerata quando il seme non viene riconosciuto</exception>
  internal static string GetCardArt(Card card) {
    char icon = GetSeedIcon(card.Seed);

    string art =
$@"╔═══════════╗
║ {GetCharacter(card),-2}        ║
║ {icon}         ║
║           ║
║     {card.NumericValue.ToString(),-2}    ║
║           ║
║        {GetCharacter(card),2} ║
║         {icon} ║
╚═══════════╝";

    return art;
  }

  internal static string GetShortArt(Card card) {
    char icon = GetSeedIcon(card.Seed);

    string artExposed = $@"╔ {GetCharacter(card)}{icon} {(GetCharacter(card).Length > 1 ? "" : "═")}══════╗";
    string artHidden = "╔═══════════╗";

    return card.Revealed ? artExposed : artHidden;
  }

  internal static string GetEmptyArt() {
    return
@"╔ ═ ═ ═ ═ ═ ╗
             
║           ║
             
║           ║
             
║           ║
             
╚ ═ ═ ═ ═ ═ ╝";
  }

  internal static string GetFlippedArt() {
    return @"╔═══════════╗
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
╚═══════════╝";
  }

  internal static string GetFoundationArt(Managers.Foundation foundation, int index) {
    string art;
    string cardIcon = index switch
    {
      0 => "♣", // Clubs
      1 => "♥", // Hearts
      2 => "♠", // Spades
      3 => "♦", // Diamonds
      _ => throw new ArgumentOutOfRangeException(nameof(index), "Indice della fondazione non valido.")
    };

    if (foundation.GetPile(index).Count == 0) {
      art =
$@"╔ ═ ═ ═ ═ ═ ╗
             
║           ║
             
║     {cardIcon}     ║
             
║           ║
             
╚ ═ ═ ═ ═ ═ ╝";
    }
    else {
      art = CardArt.GetCardArt(foundation.GetPile(index)[^1]);
    }

    return art;
  }
}
