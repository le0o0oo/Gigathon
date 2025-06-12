/*
 * Spades - Picche
 * Hearts - Cuori
 * Quadri - Diamonds
 * Clubs - Fiori
*/

using Solitario.Game.Types;

namespace Solitario.Game.Models;

class Card {
  internal readonly string Rank; // Valore della carta (A, 2, ..., 10, J, Q, K)
  internal readonly byte NumericValue; // Vero valore numerico della carta (1, 2, ..., 10, 11, 12, 13)
  internal readonly CardSeed Seed; // Seme della carta (spades, hearts, diamonds, clubs)
  internal readonly CardColor Color;
  internal bool Revealed;

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
