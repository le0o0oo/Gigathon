namespace Solitario.Game.Rendering;
internal static class CardArt {
  /// <summary>
  /// Get the color of the card based on its seed and revealed state.
  /// </summary>
  /// <returns></returns>
  internal static ConsoleColor GetColor(Card card, bool force = false) {
    if (!card.revealed && !force) return ConsoleColor.DarkGray;
    if (card.seed == "spades" || card.seed == "clubs") return ConsoleColor.White;
    else return ConsoleColor.Red;
  }

  internal static string GetCharacter(Card card) {
    if (card.value == "1") return "A";
    if (card.value == "10") return "10";

    return card.value[0].ToString().ToUpper();
  }

  /// <summary>
  /// Restituisce una "immagine" della carta in formato stringa.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException">Triggerata quando il seme non viene riconosciuto</exception>
  internal static string GetCardArt(Card card) {
    char icon = card.seed switch
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
║     {card.numericValue}{(card.numericValue > 9 ? "" : " ")}    ║
║           ║
║        {(GetCharacter(card).Length > 1 ? "" : " ")}{GetCharacter(card)} ║
║         {icon} ║
╚═══════════╝";

    return art;
  }

  internal static string GetShortArt(Card card) {
    char icon = card.seed switch
    {
      "spades" => '♠',
      "hearts" => '♥',
      "diamonds" => '♦',
      "clubs" => '♣',
      _ => throw new InvalidOperationException("Seme non riconosciuto.")
    };

    string artExposed = $@"╔ {GetCharacter(card)}{icon} {(GetCharacter(card).Length > 1 ? "" : "═")}══════╗";
    string artHidden = "╔═══════════╗";

    return card.revealed ? artExposed : artHidden;
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
}
