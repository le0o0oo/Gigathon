using Solitario.Game.Managers;

namespace Solitario.Game;
internal static class Utils {
  private static Game? currentGame;

  public static void SetCurrentGame(Game game) {
    if (game == null) {
      throw new ArgumentNullException(nameof(game), "Il gioco non può essere null.");
    }
    currentGame = game;
  }

  public static string GetEmptyCardArt() {
    return
@"╔ ═ ═ ═ ═ ═ ╗
           
║           ║
           
║           ║
           
║           ║
           
╚ ═ ═ ═ ═ ═ ╝";
  }

  public static void PrintDeck() {
    if (currentGame == null) {
      throw new InvalidOperationException("Il gioco non è stato inizializzato.");
    }

    Deck deck = currentGame.GetDeck();

    ClearRectangle(0, 0, Game.cardWidth * 2, Game.cardHeight + 2);

    string art =
@"╔═══════════╗
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
║░░░░░░░░░░░║
╚═══════════╝";

    if (deck.GetCards().Count == 0) {
      art = GetEmptyCardArt();
    }

    Console.SetCursorPosition(0, 0);
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
      string[] lines = currentGame!.GetFoundation().GetFoundationArt(i).Split('\n');
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

    ClearRectangle(0, startLine, Game.cardWidth * 7, Game.cardHeight + 6);
    // Itera per ogni colonna
    for (int i = 0; i < 7; i++) {
      byte j = 0;
      if (currentGame!.GetTableau().GetPile(i).Count == 0) {
        string[] lines = GetEmptyCardArt().Split('\n');
        Console.ForegroundColor = ConsoleColor.DarkGray;
        for (int k = 0; k < lines.Length; k++) {
          Console.SetCursorPosition(i * Game.cardWidth, j + startLine);
          Console.Write(lines[k]);
          j++;
        }
        continue;
      }

      foreach (Card card in currentGame!.GetTableau().GetRawTableau()[i]) {

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

  public static void ClearRectangle(int left, int top, int width, int height) {
    string blankLine = new string(' ', width);
    for (int y = top; y < top + height; y++) {
      Console.SetCursorPosition(left, y);
      Console.Write(blankLine);
    }
  }
}