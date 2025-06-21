using Solitario.Game.Managers;
using Solitario.Game.Models;
using Solitario.Utils;

namespace Solitario.Game.Rendering;
internal class BoardRenderer {
  private readonly Deck deck;
  private readonly Tableau tableau;
  private readonly Foundation foundation;

  internal BoardRenderer(Deck deck, Tableau tableau, Foundation foundation) {
    this.deck = deck;
    this.tableau = tableau;
    this.foundation = foundation;
  }

  /// <summary>
  /// Disegna la parte di <see cref="Areas.Deck"/>
  /// </summary>
  internal void DrawDeck() {
    Pencil.ClearRectangle(0, 0, CardArt.cardWidth * 2, CardArt.cardHeight + 2);

    string art = CardArt.GetFlippedArt();

    if (deck.GetCards().Count == 0) {
      art = CardArt.GetEmptyArt();
    }

    Console.SetCursorPosition(0, 0);
    Console.Write($"Mazzo ({deck.GetCards().Count})");
    Console.SetCursorPosition(CardArt.cardWidth, 0);
    Console.Write($"Scarti ({deck.GetWaste().Count})");

    Console.ForegroundColor = ConsoleColor.DarkGray;
    string[] lines = art.Split('\n');

    for (int i = 0; i < lines.Length; i++) {
      Console.SetCursorPosition(0, i + 1);
      Console.Write(lines[i]);
    }

    // Waste

    var wasteCard = deck.GetTopWaste();
    if (wasteCard != null) {
      art = CardArt.GetCardArt(wasteCard);
    }
    else {
      art = CardArt.GetEmptyArt();
    }
    string[] wasteLines = art.Split('\n');
    Console.ForegroundColor = wasteCard == null ? ConsoleColor.DarkGray : CardArt.GetColor(wasteCard, true);

    for (int i = 0; i < wasteLines.Length; i++) {
      Console.SetCursorPosition(CardArt.cardWidth, i + 1);
      Console.Write(wasteLines[i]);
    }

    Console.ResetColor();
  }

  /// <summary>
  /// Disegna la parte di <see cref="Areas.Tableau"/>
  /// </summary>
  internal void DrawTableau() {

    int startLine = (int)(CardArt.cardHeight + 2);

    Pencil.ClearRectangle(0, startLine, CardArt.cardWidth * 7, ConsoleRenderer.tableauHeight);
    // Itera per ogni colonna
    for (int i = 0; i < 7; i++) {
      byte j = 0;
      if (tableau.GetPile(i).Count == 0) {
        string[] lines = CardArt.GetEmptyArt().Split('\n');
        Console.ForegroundColor = ConsoleColor.DarkGray;
        for (int k = 0; k < lines.Length; k++) {
          Console.SetCursorPosition(i * CardArt.cardWidth, j + startLine);
          Console.Write(lines[k]);
          j++;
        }
        continue;
      }

      var rawTableau = tableau.Piles;

      foreach (Card card in rawTableau[i]) {

        Console.ForegroundColor = CardArt.GetColor(card);
        if (rawTableau[i].IndexOf(card) != rawTableau[i].Count - 1) {
          Console.SetCursorPosition(i * CardArt.cardWidth, j + startLine);
          Console.WriteLine(CardArt.GetShortArt(card));
        }
        else {
          Console.SetCursorPosition(i * CardArt.cardWidth, j + startLine);
          //DrawArt(card.GetCardArt());
          string[] lines = CardArt.GetCardArt(card).Split('\n');
          byte offset = 0;
          for (int line = 0; line < lines.Length; line++) {
            Console.SetCursorPosition(i * CardArt.cardWidth, j + offset + startLine);
            Console.Write(lines[line]);
            offset++;
          }
        }

        j++;
      }
    }

    Console.ResetColor();
  }

  /// <summary>
  /// Disegna la parte di <see cref="Areas.Foundation"/>
  /// </summary>
  internal void DrawFoundations() {
    int startXPos = CardArt.cardWidth * 3;
    Pencil.ClearRectangle(startXPos, 0, CardArt.cardWidth * 4, CardArt.cardHeight + 2);

    Console.SetCursorPosition(startXPos, 0);
    Console.Write("Fondazioni");

    for (int i = 0; i < 4; i++) {
      string[] lines = CardArt.GetFoundationArt(foundation, i).Split('\n');
      ConsoleColor foregroundColor;
      if (foundation.GetPile(i).Count == 0) foregroundColor = ConsoleColor.DarkGray;
      else foregroundColor = CardArt.GetColor(foundation.GetPile(i)[^1]);
      Console.ForegroundColor = foregroundColor;

      for (int j = 0; j < lines.Length; j++) {
        Console.SetCursorPosition(startXPos, j + 1);
        Console.Write(lines[j]);
      }

      startXPos += CardArt.cardWidth;
    }

    Console.ResetColor();
  }
}
