using Solitario.Game.Managers;
using Solitario.Game.Models;
using Solitario.Utils;

namespace Solitario.Game.Rendering;
internal class UIRenderer {
  private readonly Cursor cursor;
  private readonly Selection selection;
  private readonly Legend legend;
  private readonly Managers.Hint hintManager;

  internal UIRenderer(Cursor cursor, Selection selection, Legend legend, Managers.Hint hintManager) {
    this.cursor = cursor;
    this.selection = selection;
    this.legend = legend;
    this.hintManager = hintManager;
  }

  #region Private helpers
  /// <summary>
  /// Disegna una carta
  /// </summary>
  /// <param name="card">Oggetto della carta</param>
  /// <param name="x">Posizione X iniziale</param>
  /// <param name="y">Posizione Y iniziale</param>
  /// <param name="highlightWhiteAsBlack">Inverte il bianco con il nero</param>
  internal static void DrawCard(Card card, int x, int y, bool highlightWhiteAsBlack = false) {
    string cardArt = CardArt.GetCardArt(card);
    string[] artLines = cardArt.Split('\n');

    var cardConsoleColor = CardArt.GetColor(card, highlightWhiteAsBlack);
    Console.ForegroundColor = (cardConsoleColor == ConsoleColor.White) ? ConsoleColor.Black : ConsoleColor.Red;

    //Console.BackgroundColor = ConsoleColor.Gray;

    for (int j = 0; j < artLines.Length; j++) {
      Console.SetCursorPosition(x, y + j);
      Console.WriteLine(artLines[j]);
    }
  }
  #endregion

  /// <summary>
  /// Disegna il cursore nella sua posizione attuale
  /// </summary>
  internal void DrawCursor() {
    (int prevLeft, int prevTop) = Console.GetCursorPosition();

    // Rimuove il cursore dalla posizione precedente
    Console.SetCursorPosition(cursor.PrevPosition.X, cursor.PrevPosition.Y);
    Console.Write(' '); // Sovrascrive con uno spazio vuoto

    // Imposta la nuova posizione del cursore
    Console.SetCursorPosition(cursor.Position.X, cursor.Position.Y);
    Console.ForegroundColor = Renderer.color;
    Console.Write(Renderer.cursorChar); // Disegna il cursore nella nuova posizione

    Console.ResetColor();
    Console.SetCursorPosition(prevLeft, prevTop);
  }

  /// <summary>
  /// Disegna la selezione attuale
  /// </summary>
  /// <param name="redraw"></param>
  internal void DrawSelection(bool redraw = false) {
    if (!selection.Active) return;
    Renderer.SaveCursorPosition();

    int selectionItemIndex = redraw ? cursor.SelectionPosition[0] : cursor.CurrentItemIndex;
    int selectionCardPileIndex = redraw ? cursor.SelectionPosition[1] : cursor.CurrentCardIndex;

    Console.BackgroundColor = ConsoleColor.White;

    switch (selection.SourceArea) {
      case Areas.Tableau:
        var cards = selection.SelectedCards;
        for (int i = 0; i < cards.Count; i++) {
          var card = cards[i];
          string art = i == cards.Count - 1 ? CardArt.GetCardArt(card) : CardArt.GetShortArt(card);
          string[] lines = art.Split('\n');

          Console.ForegroundColor = CardArt.GetColor(card) == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;

          for (int j = 0; j < lines.Length; j++) {
            Console.SetCursorPosition(CardArt.cardWidth * selectionItemIndex, CardArt.cardHeight + 2 + j + i + selectionCardPileIndex);
            Console.WriteLine(lines[j]);
          }
        }
        break;

      case Areas.Foundation:
        DrawCard(selection.SelectedCards[0], CardArt.cardWidth * (3 + selectionItemIndex), 1);
        break;

      case Areas.Deck:
        DrawCard(selection.SelectedCards[0], CardArt.cardWidth, 1, highlightWhiteAsBlack: true);
        break;
    }

    Renderer.RestoreCursorPosition();
  }

  /// <summary>
  /// Disegna la legenda in base allo stato attuale
  /// </summary>
  internal void DrawLegend() {
    Console.SetCursorPosition(0, Renderer.legendStartY);

    // Determina colori e testo prima di disegnare
    string pickActionColor = legend.selectTextIndex == 0 ? AnsiColors.Foreground.BoldCyan : AnsiColors.Foreground.DarkGray;
    string undoActionColor = legend.CanUndo && legend.selectTextIndex == 0 ? AnsiColors.Foreground.BoldCyan : AnsiColors.Foreground.DarkGray;
    string deselectActionColor = legend.selectTextIndex == 0 ? AnsiColors.Foreground.DarkGray : AnsiColors.Foreground.BoldCyan;
    string dynamicSelectText = Legend.selectTexts[legend.selectTextIndex];

    int hintTextIndex = hintManager.ShowingHint ? 1 : 0;

    List<string> lines =
    [
      $"{AnsiColors.Foreground.BoldGreen}Usa le freccie per muovere il cursore",
      $"{AnsiColors.Foreground.BoldYellow}(R){AnsiColors.Reset} {pickActionColor}{Legend.pickCardText}",
      $"{AnsiColors.Foreground.BoldYellow}(E){AnsiColors.Reset} {pickActionColor}{Legend.pickWasteText}",
      $"{AnsiColors.Foreground.BoldYellow}(Spazio){AnsiColors.Reset} {AnsiColors.Foreground.BoldCyan}{dynamicSelectText}",
      $"{AnsiColors.Foreground.BoldYellow}(X){AnsiColors.Reset} {deselectActionColor}{Legend.deselectText}",
      $"{AnsiColors.Foreground.BoldYellow}(Z){AnsiColors.Reset} {undoActionColor}{Legend.undoText}",
      $"{AnsiColors.Foreground.BoldYellow}(Esc){AnsiColors.Reset} {AnsiColors.Foreground.BoldCyan}{Legend.menuText}",
    ];
    if (CurrentSettings.UseHints) {
      lines.Insert(6, $"{AnsiColors.Foreground.BoldYellow}(H){AnsiColors.Reset} {pickActionColor}{Legend.hintText[hintTextIndex]}");
    }
    DrawBoxTop();
    foreach (var line in lines) {
      DrawBoxLine(line);
    }
    DrawBoxBottom();
  }

  /// <summary>
  /// Disegna il bordo superiore del box della legenda
  /// </summary>
  private static void DrawBoxTop() {
    Console.WriteLine($"{AnsiColors.Foreground.BoldBlue}╔{new string('═', Renderer.legendWidth - 2)}╗{AnsiColors.Reset}");
  }

  /// <summary>
  /// Disegna il bordo inferiore del box della legenda
  /// </summary>
  private static void DrawBoxBottom() {
    // Use Write instead of WriteLine to prevent an extra newline at the end.
    Console.Write($"{AnsiColors.Foreground.BoldBlue}╚{new string('═', Renderer.legendWidth - 2)}╝{AnsiColors.Reset}");
  }

  /// <summary>
  /// Disegna una linea della legenda
  /// </summary>
  /// <param name="formattedContent">Il testo da visualizzare, può essere anche ANSI</param>
  private void DrawBoxLine(string formattedContent) {
    string plainText = Pencil.AnsiRegex.Replace(formattedContent, string.Empty);

    // Calcola il padding richiesto
    int padding = Renderer.legendWidth - 2 // per '║'
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
