using Solitario.Game.Managers;

namespace Solitario.Game;
internal static class Utils {
  private static Game currentGame;

  public static void SetCurrentGame(Game game) {
    if (game == null) {
      throw new ArgumentNullException(nameof(game), "Il gioco non può essere null.");
    }
    currentGame = game;
  }

  public static void PrintDeck() {
    if (currentGame == null) {
      throw new InvalidOperationException("Il gioco non è stato inizializzato.");
    }

    Deck deck = currentGame.GetDeck();


    string art =
$@"╔═══════════╗
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
╚═══════════╝";

    Console.Write($"Mazzo ({deck.GetCards().Count})");
    Console.SetCursorPosition(Game.cardWidth, 0);
    Console.Write($"Scarti ({deck.GetWaste().Count})");

    Console.ForegroundColor = ConsoleColor.DarkGray;
    string[] lines = art.Split('\n');

    for (int i = 0; i < lines.Length; i++) {
      Console.SetCursorPosition(0, i + 1);
      Console.Write(lines[i]);
    }

    string[] wasteLines = deck.GetWasteArt().Split('\n');
    Console.ForegroundColor = deck.GetWasteColor();

    for (int i = 0; i < wasteLines.Length; i++) {
      Console.SetCursorPosition(Game.cardWidth, i + 1);
      Console.Write(wasteLines[i]);
    }

    Console.ResetColor();
  }

  public static void PrintFoundations() {
    int startXPos = Game.cardWidth * 3;

    Console.SetCursorPosition(startXPos, 0);
    Console.Write("Fondazioni");

    for (int i = 0; i < 4; i++) {
      string[] lines = currentGame.GetFoundation().GetFoundationArt(i).Split('\n');
      Console.ForegroundColor = currentGame.GetFoundation().GetFoundationColor(i);

      for (int j = 0; j < lines.Length; j++) {
        Console.SetCursorPosition(startXPos, j + 1);
        Console.Write(lines[j]);
      }

      startXPos += Game.cardWidth;
    }

    Console.ResetColor();
  }

  public static void PrintTableau() {
    int startLine = (int)(Game.cardHeight + 2);
    // Itera per ogni colonna
    for (int i = 0; i < 7; i++) {
      byte j = 0;
      foreach (Card card in currentGame.GetTableau().GetRawTableau()[i]) {

        Console.ForegroundColor = card.GetColor();
        if (currentGame.GetTableau().GetRawTableau()[i].IndexOf(card) != currentGame.GetTableau().GetRawTableau()[i].Count - 1) {
          Console.SetCursorPosition(i * Game.cardWidth, j + startLine);
          Console.WriteLine(card.GetCardArtShort());
        }
        else {
          string[] lines = card.GetCardArt().Split('\n');
          byte offset = 0;
          for (int line = 0; line < lines.Length; line++) {
            Console.SetCursorPosition(i * Game.cardWidth, j + offset + startLine);
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