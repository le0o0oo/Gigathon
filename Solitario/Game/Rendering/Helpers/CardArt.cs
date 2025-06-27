using Solitario.Game.Models;

namespace Solitario.Game.Rendering.Helpers;
internal static class CardArt {
  internal static readonly byte cardWidth = 15; // (+ offset)
  internal static readonly byte cardHeight = 9;

  /// <summary>
  /// Restituisce il colore della carta dato il suo stato <see cref="Card.Revealed"/>.
  /// Il colore effettivo può essere forzato impostando force = true
  /// </summary>
  /// <param name="card">Oggetto della carta</param>
  /// <param name="force">Se ignorare lo stato di <see cref="Card.Revealed"/> e restituire il suo vero colore</param>
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

  /// <summary>
  /// Restituisce il rank della carta
  /// </summary>
  /// <param name="card"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Restituisce una immagine della carta non complet in base a <see cref="Card.Revealed"/>
  /// </summary>
  /// <param name="card"></param>
  /// <returns></returns>
  internal static string GetShortArt(Card card) {
    char icon = GetSeedIcon(card.Seed);

    string artExposed = $@"╔ {GetCharacter(card)}{icon} {(GetCharacter(card).Length > 1 ? "" : "═")}══════╗";
    string artHidden = "╔═══════════╗";

    return card.Revealed ? artExposed : artHidden;
  }

  /// <summary>
  /// Restituisce una immagine di una carta non definita
  /// </summary>
  /// <returns></returns>
  internal static string GetEmptyArt() {
    return
@"╔ ═ ═ ═ ═ ═ ╗
             
║           ║
             
║           ║
             
║           ║
             
╚ ═ ═ ═ ═ ═ ╝";
  }

  /// <summary>
  /// Restituisce l'immagine di una carta generica ribaltata
  /// </summary>
  /// <returns></returns>
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

  /// <summary>
  /// Restituisce una immagine di una fondazione vuota
  /// </summary>
  /// <param name="index">Indice della fondazione</param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  internal static string GetFoundationArt(int index) {
    string cardIcon = index switch
    {
      0 => "♣", // Clubs
      1 => "♥", // Hearts
      2 => "♠", // Spades
      3 => "♦", // Diamonds
      _ => throw new ArgumentOutOfRangeException(nameof(index), "Indice della fondazione non valido.")
    };

    string art =
$@"╔ ═ ═ ═ ═ ═ ╗
             
║           ║
             
║     {cardIcon}     ║
             
║           ║
             
╚ ═ ═ ═ ═ ═ ╝";


    return art;
  }
}
