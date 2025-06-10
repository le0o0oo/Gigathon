using Solitario.Game.Managers;

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
  internal static readonly int tableauHeight = Game.cardHeight + 6;
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

  internal void DrawAll() {
    DrawDeck();
    DrawFoundations();
    DrawTableau();
    DrawCursor();
  }

  internal void DrawCursor(int left, int top) {
    SaveCursorPosition();



    RestoreCursorPosition();
  }

  internal void DrawDeck() {
    SaveCursorPosition();
    ClearRectangle(0, 0, Game.cardWidth * 2, Game.cardHeight + 2);

    string art = CardArt.GetFlippedArt();

    if (deck.GetCards().Count == 0) {
      art = CardArt.GetEmptyArt();
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
      Console.SetCursorPosition(Game.cardWidth, i + 1);
      Console.Write(wasteLines[i]);
    }

    Console.ResetColor();
    RestoreCursorPosition();
  }

  internal void DrawTableau() {

    int startLine = (int)(Game.cardHeight + 2);

    ClearRectangle(0, startLine, Game.cardWidth * 7, tableauHeight);
    // Itera per ogni colonna
    for (int i = 0; i < 7; i++) {
      byte j = 0;
      if (tableau.GetPile(i).Count == 0) {
        string[] lines = CardArt.GetEmptyArt().Split('\n');
        Console.ForegroundColor = ConsoleColor.DarkGray;
        for (int k = 0; k < lines.Length; k++) {
          Console.SetCursorPosition(i * Game.cardWidth, j + startLine);
          Console.Write(lines[k]);
          j++;
        }
        continue;
      }

      var rawTableau = tableau.GetRawTableau();

      foreach (Card card in rawTableau[i]) {

        Console.ForegroundColor = CardArt.GetColor(card);
        if (rawTableau[i].IndexOf(card) != rawTableau[i].Count - 1) {
          Console.SetCursorPosition(i * Game.cardWidth, j + startLine);
          Console.WriteLine(CardArt.GetShortArt(card));
        }
        else {
          Console.SetCursorPosition(i * Game.cardWidth, j + startLine);
          //DrawArt(card.GetCardArt());
          string[] lines = CardArt.GetCardArt(card).Split('\n');
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

  internal void DrawFoundations() {
    int startXPos = Game.cardWidth * 3;
    ClearRectangle(startXPos, 0, Game.cardWidth * 4, Game.cardHeight + 2);

    Console.SetCursorPosition(startXPos, 0);
    Console.Write("Fondazioni");

    for (int i = 0; i < 4; i++) {
      string[] lines = foundation.GetFoundationArt(i).Split('\n');
      Console.ForegroundColor = foundation.GetFoundationColor(i);

      for (int j = 0; j < lines.Length; j++) {
        Console.SetCursorPosition(startXPos, j + 1);
        Console.Write(lines[j]);
      }

      startXPos += Game.cardWidth;
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

    if (selection.sourceArea == Selection.Areas.Tableau) {
      List<Card> cards = selection.selectedCards;
      for (int i = 0; i < cards.Count; i++) {
        Card card = cards[i];
        string cardArt = i == cards.Count - 1 ? CardArt.GetCardArt(card) : CardArt.GetShortArt(card);
        string[] artLines = cardArt.Split('\n');

        Console.ForegroundColor = CardArt.GetColor(card) == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;
        Console.BackgroundColor = ConsoleColor.Gray;

        int j = 0;
        foreach (string line in artLines) {
          Console.SetCursorPosition(Game.cardWidth * selectionItemIndex, Game.cardHeight + 2 + j + i + selectionCardPileIndex);
          Console.WriteLine(line);
          j++;
        }
      }
    }
    else if (selection.sourceArea == Selection.Areas.Foundation) {
      Card card = selection.selectedCards[0];
      string cardArt = CardArt.GetCardArt(card);
      string[] artLines = cardArt.Split('\n');
      Console.ForegroundColor = CardArt.GetColor(card) == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;
      Console.BackgroundColor = ConsoleColor.Gray;
      int j = 0;
      foreach (string line in artLines) {
        Console.SetCursorPosition(Game.cardWidth * (3 + selectionItemIndex), 1 + j);
        Console.WriteLine(line);
        j++;
      }
    }
    else if (selection.sourceArea == Selection.Areas.Waste) {
      Card card = selection.selectedCards[0];
      string cardArt = CardArt.GetCardArt(card);
      string[] artLines = cardArt.Split('\n');
      Console.ForegroundColor = CardArt.GetColor(card, true) == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;
      Console.BackgroundColor = ConsoleColor.Gray;
      int j = 0;
      foreach (string line in artLines) {
        Console.SetCursorPosition(Game.cardWidth, 1 + j);
        Console.WriteLine(line);
        j++;
      }
    }

    RestoreCursorPosition();
  }
}
