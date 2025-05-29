/*
 * Spades - Picche
 * Hearts - Cuori
 * Quadri - Diamonds
 * Clubs - Fiori
*/

namespace Solitario.Classes.Cards {
	abstract class Card {
		private string value; // Valore della carta
		private Int16 actualValue; // Vero valore numerico della carta
		private string type;

		Card(string type, string value) {
			this.type = type;
			this.value = value;
		}

		string GetColor() // Ottieni il colore della carta
		{
			if (type == "spades" || type == "clubs") return "red";
			else return "black";
		}

		public char GetCharacter() {
			if (value == "1") return 'A';

			return value[0].ToString().ToUpper().ToCharArray()[0];
		}

		public abstract string GetPrintText();
	}
}
