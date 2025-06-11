/*
 * Spades - Picche
 * Hearts - Cuori
 * Quadri - Diamonds
 * Clubs - Fiori
*/

using Solitario.Game.Types;

class Card {
  internal readonly string Value; // Valore della carta (A, 2, ..., 10, J, Q, K)
  internal readonly byte NumericValue; // Vero valore numerico della carta (1, 2, ..., 10, 11, 12, 13)
  internal readonly string Seed; // Seme della carta (spades, hearts, diamonds, clubs)
  internal readonly CardColor Color;
  internal bool Revealed;

  public Card(string seed, byte numericValue) {
    this.Seed = seed;
    this.NumericValue = numericValue;
    this.Revealed = false;

    if (numericValue < 1 || numericValue > 13) {
      throw new ArgumentOutOfRangeException(nameof(numericValue), "Il valore numerico deve essere compreso tra 1 e 13.");
    }
    if (string.IsNullOrEmpty(seed) || seed != "spades" && seed != "hearts" && seed != "diamonds" && seed != "clubs") {
      throw new ArgumentException("Il seme deve essere uno tra: spades, hearts, diamonds, clubs.", nameof(seed));
    }

    Value = numericValue switch
    {
      1 => "1",// Asso
      11 => "J",// Jack
      12 => "Q",// Regina
      13 => "K",// Re
      _ => numericValue.ToString(),// Numeri da 2 a 10
    };

    this.Color = seed switch
    {
      "clubs" => CardColor.Black,
      "spades" => CardColor.Black,

      _ => CardColor.Red,
    };
  }
}
