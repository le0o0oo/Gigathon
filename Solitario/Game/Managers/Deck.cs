namespace Solitario.Game.Managers;

internal class Deck {
	private List<Card> cards = new();
	private static readonly Random rng = new();

	private void GenerateCards() {
		string[] seeds = { "spades", "hearts", "diamonds", "clubs" };

		foreach (string seed in seeds) {
			for (byte i = 1; i <= 13; i++) {
				cards.Add(new Card(seed, i));
			}
		}
	}

	public Deck() {
		GenerateCards();
		Shuffle();
	}

	public void Shuffle() {
		for (int i = cards.Count - 1; i > 0; i--) {
			int j = rng.Next(i + 1);
			(cards[i], cards[j]) = (cards[j], cards[i]);
		}
	}

	internal Card GetCardAt(int index) {
		if (index < 0 || index >= cards.Count) {
			throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della pila di carte.");
		}
		return cards[index];
	}

	internal Card TakeCardAt(int index) {
		if (index < 0 || index >= cards.Count) {
			throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della pila di carte.");
		}

		Card card = cards[index];
		cards.RemoveAt(index);
		return card;
	}

	internal List<Card> GetCards() {
		return cards;
	}
}
