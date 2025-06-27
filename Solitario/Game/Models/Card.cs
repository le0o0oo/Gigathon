using Solitario.Game.Data;

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
  public string Rank { get; init; }
  /// <summary>
  /// Vero valore numerico della carta (1, 2, ..., 10, 11, 12, 13)
  /// </summary>
  public byte NumericValue { get; init; }
  /// <summary>
  /// Seme della carta (spades, hearts, diamonds, clubs)
  /// </summary>
  public CardSeed Seed { get; init; }
  public CardColor Color { get; init; }
  public bool Revealed { get; set; }


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
