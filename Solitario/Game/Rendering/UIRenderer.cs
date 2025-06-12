using Solitario.Game.Managers;
using Solitario.Game.Models;
using Solitario.Game.Rendering.Utils;
using Solitario.Game.Types;
using System.Text.RegularExpressions;

namespace Solitario.Game.Rendering;
internal class UIRenderer {
  private readonly Cursor cursor;
  private readonly Selection selection;
  private readonly Legend legend;

  // Rimuove codici 
  private static readonly Regex AnsiRegex = new(@"\u001b\[[;\d]*m", RegexOptions.Compiled);

  internal UIRenderer(Cursor cursor, Selection selection, Legend legend) {
    this.cursor = cursor;
    this.selection = selection;
    this.legend = legend;
  }

  internal void DrawCursor() {
    (int prevLeft, int prevTop) = Console.GetCursorPosition();

    // Rimuove il cursore dalla posizione precedente
    Console.SetCursorPosition(cursor.PrevPosition[0], cursor.PrevPosition[1]);
    Console.Write(' '); // Sovrascrive con uno spazio vuoto

    // Imposta la nuova posizione del cursore
    Console.SetCursorPosition(cursor.Position[0], cursor.Position[1]);
    Console.ForegroundColor = ConsoleRenderer.color;
    Console.Write(ConsoleRenderer.cursorChar); // Disegna il cursore nella nuova posizione

    Console.ResetColor();
    Console.SetCursorPosition(prevLeft, prevTop);
  }

  internal void DrawSelection(bool redraw = false) {
    if (!selection.active) return;
    ConsoleRenderer.SaveCursorPosition();

    int selectionItemIndex, selectionCardPileIndex;

    if (redraw) {
      selectionItemIndex = cursor.SelectionPosition[0];
      selectionCardPileIndex = cursor.SelectionPosition[1];
    }
    else {
      selectionItemIndex = cursor.CurrentItemIndex;
      selectionCardPileIndex = cursor.CurrentCardPileIndex;
    }

    if (selection.sourceArea == Areas.Tableau) {
      List<Card> cards = selection.selectedCards;
      for (int i = 0; i < cards.Count; i++) {
        Card card = cards[i];
        string cardArt = i == cards.Count - 1 ? CardArt.GetCardArt(card) : CardArt.GetShortArt(card);
        string[] artLines = cardArt.Split('\n');

        Console.ForegroundColor = CardArt.GetColor(card) == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;
        Console.BackgroundColor = ConsoleColor.Gray;

        int j = 0;
        foreach (string line in artLines) {
          Console.SetCursorPosition(CardArt.cardWidth * selectionItemIndex, CardArt.cardHeight + 2 + j + i + selectionCardPileIndex);
          Console.WriteLine(line);
          j++;
        }
      }
    }
    else if (selection.sourceArea == Areas.Foundation) {
      Card card = selection.selectedCards[0];
      string cardArt = CardArt.GetCardArt(card);
      string[] artLines = cardArt.Split('\n');
      Console.ForegroundColor = CardArt.GetColor(card) == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;
      Console.BackgroundColor = ConsoleColor.Gray;
      int j = 0;
      foreach (string line in artLines) {
        Console.SetCursorPosition(CardArt.cardWidth * (3 + selectionItemIndex), 1 + j);
        Console.WriteLine(line);
        j++;
      }
    }
    else if (selection.sourceArea == Areas.Waste) {
      Card card = selection.selectedCards[0];
      string cardArt = CardArt.GetCardArt(card);
      string[] artLines = cardArt.Split('\n');
      Console.ForegroundColor = CardArt.GetColor(card, true) == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;
      Console.BackgroundColor = ConsoleColor.Gray;
      int j = 0;
      foreach (string line in artLines) {
        Console.SetCursorPosition(CardArt.cardWidth, 1 + j);
        Console.WriteLine(line);
        j++;
      }
    }

    ConsoleRenderer.RestoreCursorPosition();
  }

  /// <summary>
  /// Disegna la legenda in base allo stato attuale
  /// </summary>
  internal void DrawLegend() {
    Console.SetCursorPosition(0, ConsoleRenderer.legendStartHeight);

    // Determine dynamic colors and text before drawing.
    string pickActionColor = legend.selectTextIndex == 0 ? AnsiColors.BoldCyan : AnsiColors.DarkGray;
    string deselectActionColor = legend.selectTextIndex == 0 ? AnsiColors.DarkGray : AnsiColors.BoldCyan;
    string dynamicSelectText = Legend.selectTexts[legend.selectTextIndex];

    // This is much easier to read and modify.
    string line1 = $"{AnsiColors.BoldGreen}Usa le freccie per muovere il cursore";
    string line2 = $"{AnsiColors.BoldYellow}(R){AnsiColors.Reset} {pickActionColor}{Legend.pickCardText}";
    string line3 = $"{AnsiColors.BoldYellow}(E){AnsiColors.Reset} {pickActionColor}{Legend.pickWasteText}";
    string line4 = $"{AnsiColors.BoldYellow}(Spazio){AnsiColors.Reset} {AnsiColors.BoldCyan}{dynamicSelectText}";
    string line5 = $"{AnsiColors.BoldYellow}(X){AnsiColors.Reset} {deselectActionColor}{Legend.deselectText}";

    DrawBoxTop();
    DrawBoxLine(line1);
    DrawBoxLine(line2);
    DrawBoxLine(line3);
    DrawBoxLine(line4);
    DrawBoxLine(line5);
    DrawBoxBottom();
  }

  /// <summary>
  /// Draws the top border of the box.
  /// </summary>
  private void DrawBoxTop() {
    Console.WriteLine($"{AnsiColors.BoldBlue}╔{new string('═', ConsoleRenderer.legendWidth - 2)}╗{AnsiColors.Reset}");
  }

  /// <summary>
  /// Draws the bottom border of the box.
  /// </summary>
  private void DrawBoxBottom() {
    // Use Write instead of WriteLine to prevent an extra newline at the end.
    Console.Write($"{AnsiColors.BoldBlue}╚{new string('═', ConsoleRenderer.legendWidth - 2)}╝{AnsiColors.Reset}");
  }

  /// <summary>
  /// Draws a line of text inside the box, automatically handling borders and padding.
  /// </summary>
  /// <param name="formattedContent">The text to display, which can include ANSI color codes.</param>
  private void DrawBoxLine(string formattedContent) {
    string plainText = AnsiRegex.Replace(formattedContent, string.Empty);

    // Calcola il padding richiesto
    int padding = ConsoleRenderer.legendWidth - 2 // per '║'
                  - 2 // per gli spazi dal margine sinistor
                  - plainText.Length;

    if (padding < 0) {
      padding = 0;
    }

    Console.WriteLine(
        $"{AnsiColors.BoldBlue}║{AnsiColors.Reset}  {formattedContent}{AnsiColors.Reset}{new string(' ', padding)}{AnsiColors.BoldBlue}║{AnsiColors.Reset}"
    );
  }

}
