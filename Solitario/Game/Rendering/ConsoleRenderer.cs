using Solitario.Game.Managers;
using Solitario.Game.Types;

namespace Solitario.Game.Rendering;
internal class ConsoleRenderer {
  #region Classi di stato
  private readonly Deck deck;
  private readonly Tableau tableau;
  private readonly Foundation foundation;
  private readonly Cursor cursor;
  private readonly Legend legend;
  private readonly Selection selection;
  #endregion

  #region Costanti
  private static readonly int tableauHeight = CardArt.cardHeight + 6;

  private static readonly int legendWidth = 61;
  private static readonly int legendStartHeight = CardArt.cardHeight * 3;
  #endregion

  #region Variabili di stato
  int prevLeft, prevTop = 0;  // Posizione precedente del cursore
  #endregion

  internal ConsoleRenderer(Deck deck, Tableau tableau, Foundation foundation, Cursor cursor, Legend legend, Selection selection) {
    this.deck = deck;
    this.tableau = tableau;
    this.foundation = foundation;
    this.cursor = cursor;
    this.legend = legend;
    this.selection = selection;
  }

  #region Private helpers
  private void SaveCursorPosition() {
    prevLeft = Console.CursorLeft;
    prevTop = Console.CursorTop;
  }
  private void RestoreCursorPosition() {
    Console.ResetColor();
    Console.SetCursorPosition(prevLeft, prevTop);
  }
  private void ClearRectangle(int left, int top, int width, int height) {
    string blankLine = new string(' ', width);
    for (int y = top; y < top + height; y++) {
      Console.SetCursorPosition(left, y);
      Console.Write(blankLine);
    }
  }
  private void DrawArt(string art) {
    var startLeft = Console.CursorLeft;
    var lines = art.Split('\n');

    for (int i = 0; i < lines.Length; i++) {
      Console.SetCursorPosition(startLeft, Console.CursorTop + i);
      Console.Write(lines[i]);
    }
  }
  #endregion

  #region Public helpers
  internal void DrawAll() {
    DrawDeck();
    DrawFoundations();
    DrawTableau();
    DrawCursor();
  }

  internal void DrawBasedOnArea(Areas area) {
    switch (area) {
      case Areas.Tableau:
        DrawTableau();
        break;
      case Areas.Foundation:
        DrawFoundations();
        break;
      case Areas.Waste:
        DrawDeck();
        break;
    }
  }
  #endregion

  internal void DrawDeck() {
    SaveCursorPosition();
    ClearRectangle(0, 0, CardArt.cardWidth * 2, CardArt.cardHeight + 2);

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
    RestoreCursorPosition();
  }

  internal void DrawTableau() {

    int startLine = (int)(CardArt.cardHeight + 2);

    ClearRectangle(0, startLine, CardArt.cardWidth * 7, tableauHeight);
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

      var rawTableau = tableau.GetRawTableau();

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

  internal void DrawFoundations() {
    int startXPos = CardArt.cardWidth * 3;
    ClearRectangle(startXPos, 0, CardArt.cardWidth * 4, CardArt.cardHeight + 2);

    Console.SetCursorPosition(startXPos, 0);
    Console.Write("Fondazioni");

    for (int i = 0; i < 4; i++) {
      string[] lines = CardArt.GetFoundationArt(foundation, i).Split('\n');
      Console.ForegroundColor = CardArt.GetFoundationColor(foundation, i);

      for (int j = 0; j < lines.Length; j++) {
        Console.SetCursorPosition(startXPos, j + 1);
        Console.Write(lines[j]);
      }

      startXPos += CardArt.cardWidth;
    }

    Console.ResetColor();
  }

  internal void DrawCursor() {
    (int prevLeft, int prevTop) = Console.GetCursorPosition();

    // Rimuove il cursore dalla posizione precedente
    Console.SetCursorPosition(cursor.PrevPosition[0], cursor.PrevPosition[1]);
    Console.Write(' '); // Sovrascrive con uno spazio vuoto

    // Imposta la nuova posizione del cursore
    Console.SetCursorPosition(cursor.Position[0], cursor.Position[1]);
    Console.ForegroundColor = Cursor.color;
    Console.Write(Cursor.cursorChar); // Disegna il cursore nella nuova posizione

    Console.ResetColor();
    Console.SetCursorPosition(prevLeft, prevTop);
  }

  internal void DrawSelection(bool redraw = false) {
    if (!selection.active) return;
    SaveCursorPosition();

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

    RestoreCursorPosition();
  }

  /// <summary>
  /// Disegna la legenda in base allo stato attuale
  /// </summary>
  internal void DrawLegend() {

    Console.SetCursorPosition(0, legendStartHeight);
    Console.Write(
        $"\u001b[1;34m╔{new string('═', legendWidth - 2)}╗\n" +
        $"\u001b[1;34m║\u001b[0m  \u001b[1;32mUsa le freccie per muovere il cursore\u001b[0m{new string(' ', legendWidth - 2 - 39)}\u001b[1;34m║\n" +
        $"\u001b[1;34m║\u001b[0m  \u001b[1;33m(R)\u001b[0m \u001b[1;3{(legend.selectTextIndex == 0 ? '6' : '0')}m{Legend.pickCardText}\u001b[0m{new string(' ', legendWidth - 2 - 6 - Legend.pickCardText.Length)}\u001b[1;34m║\n" +
        $"\u001b[1;34m║\u001b[0m  \u001b[1;33m(E)\u001b[0m \u001b[1;3{(legend.selectTextIndex == 0 ? '6' : '0')}m{Legend.pickWasteText}\u001b[0m{new string(' ', legendWidth - 2 - 6 - Legend.pickWasteText.Length)}\u001b[1;34m║\n" +
        $"\u001b[1;34m║\u001b[0m  \u001b[1;33m(Spazio)\u001b[0m \u001b[1;36m{Legend.selectTexts[legend.selectTextIndex]}\u001b[0m{new string(' ', legendWidth - 2 - 11 - Legend.selectTexts[legend.selectTextIndex].Length)}\u001b[1;34m║\n" +
        $"\u001b[1;34m║\u001b[0m  \u001b[1;33m(X)\u001b[0m \u001b[1;3{(legend.selectTextIndex == 0 ? '0' : '6')}m{Legend.deselectText} \u001b[0m {new string(' ', legendWidth - 2 - 8 - Legend.deselectText.Length)}\u001b[1;34m║\n" +
        //$"\u001b[1;34m║\u001b[0m  \u001b[1;37mIn attesa di un input...\u001b[0m{new string(' ', legendWidth - 2 - 26)}\u001b[1;34m║\n" +
        $"\u001b[1;34m╚{new string('═', legendWidth - 2)}╝\u001b[0m\n"
    );
  }
}
