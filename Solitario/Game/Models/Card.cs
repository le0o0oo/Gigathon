/*
 * Spades - Picche
 * Hearts - Cuori
 * Quadri - Diamonds
 * Clubs - Fiori
*/

namespace Solitario.Game.Models;

/// <summary>
/// Rappresenta una carta
/// </summary>
class Card {
  /// <summary>
  /// Valore della carta (A, 2, ..., 10, J, Q, K)
  /// </summary>
  internal string Rank { get; }
  /// <summary>
  /// Vero valore numerico della carta (1, 2, ..., 10, 11, 12, 13)
  /// </summary>
  internal byte NumericValue { get; }
  /// <summary>
  /// Seme della carta (spades, hearts, diamonds, clubs)
  /// </summary>
  internal CardSeed Seed { get; }
  internal CardColor Color { get; }
  internal bool Revealed { get; set; }


  public Card(CardSeed seed, byte numericValue) {
    this.Seed = seed;
    this.NumericValue = numericValue;
    this.Revealed = false;

    if (numericValue < 1 || numericValue > 13) {
      throw new ArgumentOutOfRangeException(nameof(numericValue), "Il valore numerico deve essere compreso tra 1 e 13.");
    }

    Rank = numericValue switch
    {
      1 => "A",// Asso
      11 => "J",// Jack
      12 => "Q",// Regina
      13 => "K",// Re
      _ => numericValue.ToString(),// Numeri da 2 a 10
    };

    this.Color = seed switch
    {
      CardSeed.Clubs => CardColor.Black,
      CardSeed.Spades => CardColor.Black,

      _ => CardColor.Red,
    };
  }
}
