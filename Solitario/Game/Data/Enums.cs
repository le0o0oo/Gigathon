namespace Solitario.Game.Data;

/// <summary>
/// Rappresenta le possibili aree del gioco
/// </summary>
internal enum Areas {
  /// <summary>
  /// Rappresenta sia le carte da pescare che quelle di scarto
  /// </summary>
  Deck,
  /// <summary>
  /// Fondazione
  /// </summary>
  Foundation,
  /// <summary>
  /// Rappresenta il Tableau
  /// </summary>
  Tableau
}

/// <summary>
/// Rappresenta il possibile colore di una carta
/// </summary>
public enum CardColor {
  Red,
  Black
}

/// <summary>
/// Rappresenta i possibili semi che una carta può avere
/// </summary>
public enum CardSeed {
  Spades,
  Hearts,
  Diamonds,
  Clubs
}
