/*
 * Spades - Picche
 * Hearts - Cuori
 * Quadri - Diamonds
 * Clubs - Fiori
*/


class Card {
  internal readonly string value; // Valore della carta (A, 2, ..., 10, J, Q, K)
  internal readonly byte numericValue; // Vero valore numerico della carta (1, 2, ..., 10, 11, 12, 13)
  internal readonly string seed; // Seme della carta (spades, hearts, diamonds, clubs)
  internal bool revealed;

  public Card(string seed, byte numericValue) {
    this.seed = seed;
    this.numericValue = numericValue;
    this.revealed = false;

    if (numericValue < 1 || numericValue > 13) {
      throw new ArgumentOutOfRangeException(nameof(numericValue), "Il valore numerico deve essere compreso tra 1 e 13.");
    }
    if (string.IsNullOrEmpty(seed) || seed != "spades" && seed != "hearts" && seed != "diamonds" && seed != "clubs") {
      throw new ArgumentException("Il seme deve essere uno tra: spades, hearts, diamonds, clubs.", nameof(seed));
    }

    value = numericValue switch
    {
      1 => "1",// Asso
      11 => "J",// Jack
      12 => "Q",// Regina
      13 => "K",// Re
      _ => numericValue.ToString(),// Numeri da 2 a 10
    };
  }
}
