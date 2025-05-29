namespace Solitario.Game.Managers;

internal class Tableau {
	private static readonly Random rng = new();

	List<List<Card>> tableau = [];

	public Tableau(Deck deck) {
		for (byte i = 0; i < 7; i++) { // i = indice della colonna
			tableau.Add([]); // Aggiungi una nuova colonna al tableau
			for (byte j = 0; j <= i; j++) { // j = indice della carta nella colonna
				tableau[i].Add(deck.TakeCardAt(0));
			}
		}
	}

	internal Card GetCard(byte column, byte index) {
		if (column < 0 || column >= tableau.Count) {
			throw new ArgumentOutOfRangeException(nameof(column), "Colonna fuori dai limiti del tableau.");
		}
		if (index < 0 || index >= tableau[column].Count) {
			throw new ArgumentOutOfRangeException(nameof(index), "Indice fuori dai limiti della colonna.");
		}
		return tableau[column][index];
	}

	internal List<List<Card>> GetTableau() {
		return tableau;
	}
}
