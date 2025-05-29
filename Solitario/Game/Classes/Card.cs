/*
 * Spades - Picche
 * Hearts - Cuori
 * Quadri - Diamonds
 * Clubs - Fiori
*/


class Card {
	protected readonly string value; // Valore della carta (A, 2, ..., 10, J, Q, K)
	protected readonly byte numericValue; // Vero valore numerico della carta (1, 2, ..., 10, 11, 12, 13)
	internal readonly string seed; // Seme della carta (spades, hearts, diamonds, clubs)

	public Card(string seed, byte numericValue) {
		this.seed = seed;
		this.numericValue = numericValue;

		if (numericValue < 1 || numericValue > 13) {
			throw new ArgumentOutOfRangeException(nameof(numericValue), "Il valore numerico deve essere compreso tra 1 e 13.");
		}
		if (string.IsNullOrEmpty(seed) || seed != "spades" && seed != "hearts" && seed != "diamonds" && seed != "clubs") {
			throw new ArgumentException("Il seme deve essere uno tra: spades, hearts, diamonds, clubs.", nameof(seed));
		}

		value = numericValue switch {
			1 => "1",// Asso
			11 => "J",// Jack
			12 => "Q",// Regina
			13 => "K",// Re
			_ => numericValue.ToString(),// Numeri da 2 a 10
		};
	}

	public ConsoleColor GetColor() // Ottieni il colore della carta
	{
		if (seed == "spades" || seed == "clubs") return ConsoleColor.White;
		else return ConsoleColor.Red;
	}

	public string GetCharacter() {
		if (value == "1") return "A";
		if (value == "10") return "10";

		return value[0].ToString().ToUpper();
	}

	/**
	 * * Restituisce una "immagine" della carta in formato stringa.
	*/
	internal string GetCardArt() {
		char icon = seed switch {
			"spades" => '♠',
			"hearts" => '♥',
			"diamonds" => '♦',
			"clubs" => '♣',
			_ => throw new InvalidOperationException("Seme non riconosciuto.")
		};

		string art =
$@"╔═══════════╗
║ {GetCharacter()}        {(GetCharacter().Length > 1 ? "" : " ")}║
║ {icon}         ║
║           ║
║     {numericValue}{(numericValue > 9 ? "" : " ")}    ║
║           ║
║        {(GetCharacter().Length > 1 ? "" : " ")}{GetCharacter()} ║
║         {icon} ║
╚═══════════╝";

		return art;
	}

	internal string GetCardArtShort() {
		char icon = seed switch {
			"spades" => '♠',
			"hearts" => '♥',
			"diamonds" => '♦',
			"clubs" => '♣',
			_ => throw new InvalidOperationException("Seme non riconosciuto.")
		};

		string art = $@"╔ {GetCharacter()}{icon} {(GetCharacter().Length > 1 ? "" : "═")}══════╗";

		return art;
	}
}
