using Solitario.Game.Managers;
using Solitario.Game.Models;
using Solitario.Utils;
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

  #region Private helpers
  private static void DrawCard(Card card, int x, int y, bool highlightWhiteAsBlack = false) {
    string cardArt = CardArt.GetCardArt(card);
    string[] artLines = cardArt.Split('\n');

    var cardConsoleColor = CardArt.GetColor(card, highlightWhiteAsBlack);
    Console.ForegroundColor = (cardConsoleColor == ConsoleColor.White) ? ConsoleColor.Black : ConsoleColor.Red;

    Console.BackgroundColor = ConsoleColor.Gray;

    for (int j = 0; j < artLines.Length; j++) {
      Console.SetCursorPosition(x, y + j);
      Console.WriteLine(artLines[j]);
    }
  }
  #endregion

  internal void DrawCursor() {
    (int prevLeft, int prevTop) = Console.GetCursorPosition();

    // Rimuove il cursore dalla posizione precedente
    Console.SetCursorPosition(cursor.PrevPosition.X, cursor.PrevPosition.Y);
    Console.Write(' '); // Sovrascrive con uno spazio vuoto

    // Imposta la nuova posizione del cursore
    Console.SetCursorPosition(cursor.Position.X, cursor.Position.Y);
    Console.ForegroundColor = ConsoleRenderer.color;
    Console.Write(ConsoleRenderer.cursorChar); // Disegna il cursore nella nuova posizione

    Console.ResetColor();
    Console.SetCursorPosition(prevLeft, prevTop);
  }

  internal void DrawSelection(bool redraw = false) {
    if (!selection.active) return;
    ConsoleRenderer.SaveCursorPosition();

    int selectionItemIndex = redraw ? cursor.SelectionPosition[0] : cursor.CurrentItemIndex;
    int selectionCardPileIndex = redraw ? cursor.SelectionPosition[1] : cursor.CurrentCardPileIndex;

    switch (selection.sourceArea) {
      case Areas.Tableau:
        var cards = selection.selectedCards;
        for (int i = 0; i < cards.Count; i++) {
          var card = cards[i];
          string art = i == cards.Count - 1 ? CardArt.GetCardArt(card) : CardArt.GetShortArt(card);
          string[] lines = art.Split('\n');

          Console.ForegroundColor = CardArt.GetColor(card) == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;
          Console.BackgroundColor = ConsoleColor.Gray;

          for (int j = 0; j < lines.Length; j++) {
            Console.SetCursorPosition(CardArt.cardWidth * selectionItemIndex, CardArt.cardHeight + 2 + j + i + selectionCardPileIndex);
            Console.WriteLine(lines[j]);
          }
        }
        break;

      case Areas.Foundation:
        DrawCard(selection.selectedCards[0], CardArt.cardWidth * (3 + selectionItemIndex), 1);
        break;

      case Areas.Waste:
        DrawCard(selection.selectedCards[0], CardArt.cardWidth, 1, highlightWhiteAsBlack: true);
        break;
    }

    ConsoleRenderer.RestoreCursorPosition();
  }

  /// <summary>
  /// Disegna la legenda in base allo stato attuale
  /// </summary>
  internal void DrawLegend() {
    Console.SetCursorPosition(0, ConsoleRenderer.legendStartY);

    // Determine dynamic colors and text before drawing.
    string pickActionColor = legend.selectTextIndex == 0 ? AnsiColors.Foreground.BoldCyan : AnsiColors.Foreground.DarkGray;
    string undoActionColor = legend.CanUndo && legend.selectTextIndex == 0 ? AnsiColors.Foreground.BoldCyan : AnsiColors.Foreground.DarkGray;
    string deselectActionColor = legend.selectTextIndex == 0 ? AnsiColors.Foreground.DarkGray : AnsiColors.Foreground.BoldCyan;
    string dynamicSelectText = Legend.selectTexts[legend.selectTextIndex];

    string[] lines =
    {
      $"{AnsiColors.Foreground.BoldGreen}Usa le freccie per muovere il cursore",
      $"{AnsiColors.Foreground.BoldYellow}(R){AnsiColors.Reset} {pickActionColor}{Legend.pickCardText}",
      $"{AnsiColors.Foreground.BoldYellow}(E){AnsiColors.Reset} {pickActionColor}{Legend.pickWasteText}",
      $"{AnsiColors.Foreground.BoldYellow}(Spazio){AnsiColors.Reset} {AnsiColors.Foreground.BoldCyan}{dynamicSelectText}",
      $"{AnsiColors.Foreground.BoldYellow}(X){AnsiColors.Reset} {deselectActionColor}{Legend.deselectText}",
      $"{AnsiColors.Foreground.BoldYellow}(Z){AnsiColors.Reset} {undoActionColor}{Legend.undoText}",
      $"{AnsiColors.Foreground.BoldYellow}(H){AnsiColors.Reset} {undoActionColor}{Legend.hintText}",
    };

    DrawBoxTop();
    foreach (var line in lines) {
      DrawBoxLine(line);
    }
    DrawBoxBottom();
  }

  /// <summary>
  /// Draws the top border of the box.
  /// </summary>
  private void DrawBoxTop() {
    Console.WriteLine($"{AnsiColors.Foreground.BoldBlue}╔{new string('═', ConsoleRenderer.legendWidth - 2)}╗{AnsiColors.Reset}");
  }

  /// <summary>
  /// Draws the bottom border of the box.
  /// </summary>
  private void DrawBoxBottom() {
    // Use Write instead of WriteLine to prevent an extra newline at the end.
    Console.Write($"{AnsiColors.Foreground.BoldBlue}╚{new string('═', ConsoleRenderer.legendWidth - 2)}╝{AnsiColors.Reset}");
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
        $"{AnsiColors.Foreground.BoldBlue}║{AnsiColors.Reset}  {formattedContent}{AnsiColors.Reset}{new string(' ', padding)}{AnsiColors.Foreground.BoldBlue}║{AnsiColors.Reset}"
    );
  }

}
