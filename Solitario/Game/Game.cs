using Solitario.Game.Managers;

namespace Solitario.Game;

internal class Game {
	private byte cardWidth = 14;
	private byte cardHeight = 9;

	Deck deck;
	Tableau tableau;

	public Game() {
		deck = new Deck(); // Create a new deck of cards
		tableau = new Tableau(deck); // Create a new tableau with the deck

		printTableau();


		/*int cardWidth = 14; // 11 + borders/padding
		int cardHeight = 9;

		for (int i = 0; i < 5; i++) {
			var card = deck.GetCardAt(i); // Ottieni la "arte" (come la chiamo? 😭) della carta
			string[] lines = card.GetCardArt().Split('\n'); // Dividi l'arte della carta in linee

			Console.ForegroundColor = card.GetColor(); // Imposta il colore della console in base al seme della carta

			// Stampa l'arte della carta linea per linea
			for (int line = 0; line < lines.Length; line++) {
				Console.SetCursorPosition(i * cardWidth, line); // (left, top), dove left è il punto di partenza (colonna) e top è la riga corrente
				Console.Write(lines[line]); // Scrive la linea corrente dell'arte della carta
			}

			Console.ResetColor();
		}

		Console.SetCursorPosition(0, cardHeight + 1); */
	}

	void printTableau() {
		// Itera per ogni colonna
		for (int i = 0; i < 7; i++) {
			byte j = 0;
			foreach (Card card in tableau.GetTableau()[i]) {

				Console.ForegroundColor = card.GetColor();
				if (tableau.GetTableau()[i].IndexOf(card) != tableau.GetTableau()[i].Count - 1) {
					Console.SetCursorPosition(i * cardWidth, j);
					Console.WriteLine(card.GetCardArtShort());
				}
				else {
					string[] lines = card.GetCardArt().Split('\n');
					byte offset = 0;
					for (int line = 0; line < lines.Length; line++) {
						Console.SetCursorPosition(i * cardWidth, j + offset);
						Console.Write(lines[line]);
						offset++;
					}
				}

				j++;
			}
		}

		Console.ResetColor();
	}
}

