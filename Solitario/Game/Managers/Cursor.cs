namespace Solitario.Game.Managers;
internal class Cursor {
  internal enum CursorArea {
    Foundation,
    Tableau
  }
  internal static readonly ConsoleColor color = ConsoleColor.DarkGreen;
  internal static readonly char cursorChar = '❮';
  readonly static Dictionary<CursorArea, byte> areaMax = new()
  {
    { CursorArea.Foundation, 4 }, // Numero di fondazioni
    { CursorArea.Tableau, 7 } // Numero di pile nel tableau
  };

  private readonly Tableau tableau;
  private readonly Legend legend;
  private readonly Selection selection;
  private readonly Foundation foundation;

  internal CursorArea CurrentArea { get; private set; } = CursorArea.Tableau;
  /*Indica l'elemento corrente della selezione in base all'area, e vale solo per tableau e foundation.
   * Per esempio, se l'area corrente è Tableau, currentItemIndex indica l'indice della pila di carte selezionata.
   * Se invece è foundation, currentItemIndex indica l'indice della pila di fondazione selezionata.
  */
  internal int CurrentItemIndex { get; private set; } = 0;
  internal int CurrentCardPileIndex { get; private set; } = 0; // Indice della carta nella pla corrente (solo per tableau). Parte della carta più in basso della pila corrente.


  internal int[] Position { get; private set; } = { Game.cardWidth - 2, Game.cardHeight + 2 }; // Posizione iniziale del cursore (colonna, riga)
  internal int[] PrevPosition { get; private set; } = { Game.cardWidth - 2, Game.cardHeight + 2 }; // Posizione iniziale del cursore (colonna, riga)

  /*
   * 0 - currentItemIndex
   * 1 - currentCardPileIndex
  */
  internal int[] SelectionPosition { get; private set; } = { 0, 0 }; // Posizione del cursore di selezione (colonna, riga), usato per disegnare la selezione

  internal Cursor(Tableau tableau, Legend legend, Selection selection, Foundation foundation) {
    this.tableau = tableau;
    this.legend = legend;
    this.selection = selection;
    this.foundation = foundation;
  }

  /// <summary>
  /// Imposta la posizione del cursore nella console.
  /// </summary>
  /// <param name="top">Numero di righe a partire dall'inizio</param>
  /// <param name="left">Numero di colonne a partire da sinistra</param>
  private void SetPosition(int left, int top) {
    // Salva la posizione precedente per il disegno della selezione
    PrevPosition[0] = Position[0];
    PrevPosition[1] = Position[1];

    // Aggiorna la posizione corrente
    Position[0] = left;
    Position[1] = top;
  }

  private void UpdatePosition() {
    if (CurrentArea == CursorArea.Foundation) {
      SetPosition(Game.cardWidth * (4 + CurrentItemIndex) - 2, 1);
    }
    else { // Nel tableau
      if (CurrentCardPileIndex < 0) CurrentCardPileIndex = 0;
      SetPosition(Game.cardWidth * (CurrentItemIndex + 1) - 2, Game.cardHeight + 2 + CurrentCardPileIndex);
    }

  }

  internal void MoveUp() {
    if (CurrentArea == CursorArea.Foundation) return;
    bool canGoUp = CurrentCardPileIndex == 0 || !tableau.GetCard(CurrentItemIndex, CurrentCardPileIndex - 1).revealed;


    // Vai su
    if (canGoUp) {
      CurrentItemIndex = CurrentItemIndex - 3 < 0 ? 0 : CurrentItemIndex - 3;
      CurrentArea = CursorArea.Foundation;
      UpdatePosition();
      return;
    }

    // Seleziona carta tra la pila
    CurrentCardPileIndex--;
    UpdatePosition();
  }

  internal void MoveDown() {
    // Vai giù
    if (CurrentArea == CursorArea.Foundation) {
      CurrentItemIndex = CurrentItemIndex + 3;
      CurrentCardPileIndex = tableau.GetPile(CurrentItemIndex).Count - 1;
      CurrentArea = CursorArea.Tableau;
      UpdatePosition();
      return;
    }

    if (tableau.GetPile(CurrentItemIndex).Count - 1 == CurrentCardPileIndex) return;
    CurrentCardPileIndex++;
    UpdatePosition();
  }

  internal void MoveLeft() {
    if (CurrentItemIndex <= 0) return;
    if (CurrentArea == CursorArea.Foundation) {
      CurrentItemIndex--;
      UpdatePosition();
    }
    else {
      CurrentItemIndex--;
      CurrentCardPileIndex = tableau.GetPile(CurrentItemIndex).Count - 1;
      UpdatePosition();
    }
  }

  internal void MoveRight() {
    if (CurrentItemIndex >= areaMax[CurrentArea] - 1) return;

    if (CurrentArea == CursorArea.Foundation) {
      CurrentItemIndex++;

      UpdatePosition();
    }
    else {
      CurrentItemIndex++;
      CurrentCardPileIndex = tableau.GetPile(CurrentItemIndex).Count - 1;
      UpdatePosition();
    }
  }
  internal bool Select() {
    if (CurrentArea == CursorArea.Foundation) {
      // Da tableau/riserva alla fondazione
      if (selection.active) {
        if (selection.selectedCards.Count != 1) return false; // Per la fondazione è possibile selezionare una sola carta alla volta
        if (!Validator.ValidateCardMove(selection.selectedCards[0], foundation.GetPile(CurrentItemIndex), Selection.Areas.Foundation)) return false;
        if (Foundation.seedIndexMap[selection.selectedCards[0].seed] != CurrentItemIndex) return false;

        selection.AddToTarget(Selection.Areas.Foundation, CurrentItemIndex);
        legend.SetSelected(false);

        return true;
      }
      // Selezione non attiva, seleziona carta dalla fondazione
      else if (!selection.active) {
        if (foundation.GetPile(CurrentItemIndex).Count == 0) return false; // Non ci sono carte nella fondazione
        var cards = new List<Card>([foundation.GetCardAt(CurrentItemIndex, -1)]);
        selection.SetSelection(Selection.Areas.Foundation, CurrentItemIndex, cards);

        SelectionPosition[0] = CurrentItemIndex;
        SelectionPosition[1] = CurrentCardPileIndex;

        legend.SetSelected(true);

        return true;
      }
    }
    // Nel tableau. Potevo anche toglere else if ma è più chiaro così
    else if (CurrentArea == CursorArea.Tableau) {
      // Selezione non attiva: seleziona carte dalla pila corrente del tableau
      if (!selection.active) {
        List<Card> cards = new List<Card>();

        for (int i = CurrentCardPileIndex; i < tableau.GetPile(CurrentItemIndex).Count; i++) {
          cards.Add(tableau.GetCard(CurrentItemIndex, i));
        }
        if (cards.Count == 0) return false;

        selection.SetSelection(Selection.Areas.Tableau, CurrentItemIndex, cards);
        legend.SetSelected(true);

        SelectionPosition[0] = CurrentItemIndex;
        SelectionPosition[1] = CurrentCardPileIndex;

        return true;
      }
      // Selezione attiva, muovi carte
      else {
        if (!Validator.ValidateCardMove(selection.selectedCards[0], tableau.GetPile(CurrentItemIndex), Selection.Areas.Tableau)) return false;
        selection.AddToTarget(Selection.Areas.Tableau, CurrentItemIndex);
        legend.SetSelected(false);

        return true;
      }
    }

    return false;
  }
}
