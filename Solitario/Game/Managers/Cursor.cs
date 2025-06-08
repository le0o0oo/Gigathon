namespace Solitario.Game.Managers;
internal class Cursor {
  internal enum CursorArea {
    Foundation,
    Tableau
  }
  const ConsoleColor color = ConsoleColor.DarkGreen;
  const char cursorChar = '❮';
  readonly Dictionary<CursorArea, byte> areaMax = new Dictionary<CursorArea, byte> {
      { CursorArea.Foundation, 4 }, // Numero di fondazioni
      { CursorArea.Tableau, 7 } // Numero di pile nel tableau
    };

  private readonly Tableau tableau;
  private readonly Legend legend;
  private readonly Selection selection;
  private readonly Foundation foundation;
  private readonly Deck deck; // Necessario per disegnare la selezione dalla riserva

  CursorArea currentArea = CursorArea.Tableau;
  /*Indica l'elemento corrente della selezione in base all'area, e vale solo per tableau e foundation.
   * Per esempio, se l'area corrente è Tableau, currentItemIndex indica l'indice della pila di carte selezionata.
   * Se invece è foundation, currentItemIndex indica l'indice della pila di fondazione selezionata.
  */
  internal int currentItemIndex { get; private set; } = 0;
  internal int currentCardPileIndex { get; private set; } = 0; // Indice della carta nella pla corrente (solo per tableau). Parte della carta più in basso della pila corrente.


  private int[] position = { Game.cardWidth - 2, Game.cardHeight + 2 }; // Posizione iniziale del cursore (colonna, riga)

  /*
   * 0 - currentItemIndex
   * 1 - currentCardPileIndex
  */
  private int[] selectionPosition = { 0, 0 }; // Posizione del cursore di selezione (colonna, riga), usato per disegnare la selezione

  internal Cursor(Tableau tableau, Legend legend, Selection selection, Foundation foundation, Deck deck) {
    this.tableau = tableau;
    this.legend = legend;
    this.selection = selection;
    this.foundation = foundation;
    this.deck = deck;

    Draw();
  }

  /// <summary>
  /// Renderizza il cursore date le coordinate attuali.
  /// </summary>
  internal void Draw() {
    (int prevLeft, int prevTop) = Console.GetCursorPosition();

    Console.SetCursorPosition(position[0], position[1]);
    Console.ForegroundColor = color;
    Console.Write(cursorChar);

    Console.ResetColor();
    Console.SetCursorPosition(prevLeft, prevTop);
  }

  /// <summary>
  /// Imposta la posizione del cursore nella console.
  /// </summary>
  /// <param name="top">Numero di righe a partire dall'inizio</param>
  /// <param name="left">Numero di colonne a partire da sinistra</param>
  internal void SetPosition(int left, int top) {
    (int prevLeft, int prevTop) = Console.GetCursorPosition();

    // Rimuove il cursore dalla posizione precedente
    Console.SetCursorPosition(position[0], position[1]);
    Console.Write(' '); // Sovrascrive con uno spazio vuoto

    // Imposta la nuova posizione del cursore
    position[0] = left;
    position[1] = top;
    Console.SetCursorPosition(position[0], position[1]);
    Console.ForegroundColor = color;
    Console.Write(cursorChar); // Disegna il cursore nella nuova posizione

    Console.ResetColor();
    Console.SetCursorPosition(prevLeft, prevTop);

  }

  private void DrawPosition() {
    if (currentArea == CursorArea.Foundation) {
      SetPosition(Game.cardWidth * (4 + currentItemIndex) - 2, 1);
    }
    else { // Nel tableau
      if (currentCardPileIndex < 0) currentCardPileIndex = 0;
      SetPosition(Game.cardWidth * (currentItemIndex + 1) - 2, Game.cardHeight + 2 + currentCardPileIndex);
    }

  }

  internal void MoveUp() {
    if (currentArea == CursorArea.Foundation) return;
    bool canGoUp = currentCardPileIndex == 0 || !tableau.GetCard(currentItemIndex, currentCardPileIndex - 1).revealed;


    // Vai su
    if (canGoUp) {
      currentItemIndex = currentItemIndex - 3 < 0 ? 0 : currentItemIndex - 3;
      currentArea = CursorArea.Foundation;
      DrawPosition();
      return;
    }

    // Seleziona carta tra la pila
    currentCardPileIndex--;
    DrawPosition();
  }

  internal void MoveDown() {
    // Vai giù
    if (currentArea == CursorArea.Foundation) {
      currentItemIndex = currentItemIndex + 3;
      currentCardPileIndex = tableau.GetPile(currentItemIndex).Count - 1;
      currentArea = CursorArea.Tableau;
      DrawPosition();
      return;
    }

    if (tableau.GetPile(currentItemIndex).Count - 1 == currentCardPileIndex) return;
    currentCardPileIndex++;
    DrawPosition();
  }

  internal void MoveLeft() {
    if (currentItemIndex <= 0) return;
    if (currentArea == CursorArea.Foundation) {
      currentItemIndex--;
      DrawPosition();
    }
    else {
      currentItemIndex--;
      currentCardPileIndex = tableau.GetPile(currentItemIndex).Count - 1;
      DrawPosition();
    }
  }

  internal void MoveRight() {
    if (currentItemIndex >= areaMax[currentArea] - 1) return;

    if (currentArea == CursorArea.Foundation) {
      currentItemIndex++;

      DrawPosition();
    }
    else {
      currentItemIndex++;
      currentCardPileIndex = tableau.GetPile(currentItemIndex).Count - 1;
      DrawPosition();
    }
  }

  internal void DrawSelection(bool redraw = false) {
    if (!selection.active) return;
    (int prevLeft, int prevTop) = Console.GetCursorPosition();
    int prevItemIndex = selectionPosition[0];
    int prevCardPileIndex = selectionPosition[1];

    if (redraw) {
      currentItemIndex = selectionPosition[0];
      currentCardPileIndex = selectionPosition[1];
    }
    else {
      selectionPosition[0] = currentItemIndex;
      selectionPosition[1] = currentCardPileIndex;
    }

    if (selection.sourceArea == Selection.Areas.Tableau) {
      List<Card> cards = selection.selectedCards;
      for (int i = 0; i < cards.Count; i++) {
        Card card = cards[i];
        string cardArt = i == cards.Count - 1 ? card.GetCardArt() : card.GetCardArtShort();
        string[] artLines = cardArt.Split('\n');

        Console.ForegroundColor = card.GetColor() == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;
        Console.BackgroundColor = ConsoleColor.Gray;

        int j = 0;
        foreach (string line in artLines) {
          Console.SetCursorPosition(Game.cardWidth * currentItemIndex, Game.cardHeight + 2 + j + i + currentCardPileIndex);
          Console.WriteLine(line);
          j++;
        }
      }
    }
    else if (selection.sourceArea == Selection.Areas.Foundation) {
      Card card = selection.selectedCards[0];
      string cardArt = card.GetCardArt();
      string[] artLines = cardArt.Split('\n');
      Console.ForegroundColor = card.GetColor() == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;
      Console.BackgroundColor = ConsoleColor.Gray;
      int j = 0;
      foreach (string line in artLines) {
        Console.SetCursorPosition(Game.cardWidth * (3 + currentItemIndex), 1 + j);
        Console.WriteLine(line);
        j++;
      }
    }
    else if (selection.sourceArea == Selection.Areas.Waste) {
      Card card = selection.selectedCards[0];
      string cardArt = card.GetCardArt();
      string[] artLines = cardArt.Split('\n');
      Console.ForegroundColor = card.GetColor(true) == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;
      Console.BackgroundColor = ConsoleColor.Gray;
      int j = 0;
      foreach (string line in artLines) {
        Console.SetCursorPosition(Game.cardWidth, 1 + j);
        Console.WriteLine(line);
        j++;
      }
    }


    if (redraw) {
      currentItemIndex = prevItemIndex;
      currentCardPileIndex = prevCardPileIndex;
    }

    Console.ResetColor();
    Console.SetCursorPosition(prevLeft, prevTop);
  }

  internal void Select() {
    if (currentArea == CursorArea.Foundation) {
      // Da tableau/riserva alla fondazione
      if (selection.active) {
        selection.AddToTarget(Selection.Areas.Foundation, currentItemIndex);
        Utils.PrintTableau();
        Utils.PrintFoundations();
        Utils.PrintDeck();
        legend.SetSelected(false);
        Draw();

      }
      // Selezione non attiva, seleziona carta dalla fondazione
      else if (!selection.active) {
        if (foundation.GetPile(currentItemIndex).Count == 0) return; // Non ci sono carte nella fondazione
        var cards = new List<Card>([foundation.GetCardAt(currentItemIndex, -1)]);
        selection.SetSelection(Selection.Areas.Foundation, currentItemIndex, cards);

        DrawSelection();
        legend.SetSelected(true);
        Draw();
      }
    }
    // Nel tableau. Potevo anche toglere else if ma è più chiaro così
    else if (currentArea == CursorArea.Tableau) {
      // Selezione non attiva: seleziona carte dalla pila corrente del tableau
      if (!selection.active) {
        List<Card> cards = new List<Card>();

        for (int i = currentCardPileIndex; i < tableau.GetPile(currentItemIndex).Count; i++) {
          cards.Add(tableau.GetCard(currentItemIndex, i));
        }
        if (cards.Count == 0) return;

        selection.SetSelection(Selection.Areas.Tableau, currentItemIndex, cards);
        legend.SetSelected(true);

        DrawSelection();
      }
      // Selezione attiva, muovi carte
      else {
        selection.AddToTarget(Selection.Areas.Tableau, currentItemIndex);
        Utils.PrintTableau();
        Utils.PrintFoundations();
        Utils.PrintDeck();
        Draw();
        legend.SetSelected(false);
      }
    }
  }
}
