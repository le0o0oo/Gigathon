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

    return card.Color == Types.CardColor.Red ? ConsoleColor.Red : ConsoleColor.White;
  }

  internal static string GetCharacter(Card card) {
    if (card.Value == "1") return "A";
    if (card.Value == "10") return "10";

    return card.Value[0].ToString().ToUpper();
  }

  /// <summary>
  /// Restituisce una "immagine" della carta in formato stringa.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException">Triggerata quando il seme non viene riconosciuto</exception>
  internal static string GetCardArt(Card card) {
    char icon = card.Seed switch
    {
      "spades" => '♠',
      "hearts" => '♥',
      "diamonds" => '♦',
      "clubs" => '♣',
      _ => throw new InvalidOperationException("Seme non riconosciuto.")
    };

    string art =
$@"╔═══════════╗
║ {GetCharacter(card)}        {(GetCharacter(card).Length > 1 ? "" : " ")}║
║ {icon}         ║
║           ║
║     {card.NumericValue}{(card.NumericValue > 9 ? "" : " ")}    ║
║           ║
║        {(GetCharacter(card).Length > 1 ? "" : " ")}{GetCharacter(card)} ║
║         {icon} ║
╚═══════════╝";

    return art;
  }

  internal static string GetShortArt(Card card) {
    char icon = card.Seed switch
    {
      "spades" => '♠',
      "hearts" => '♥',
      "diamonds" => '♦',
      "clubs" => '♣',
      _ => throw new InvalidOperationException("Seme non riconosciuto.")
    };

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

  internal static ConsoleColor GetFoundationColor(Managers.Foundation foundation, int index) {
    if (foundation.GetPile(index).Count == 0) return ConsoleColor.DarkGray;
    return CardArt.GetColor(foundation.GetPile(index)[^1]);
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
