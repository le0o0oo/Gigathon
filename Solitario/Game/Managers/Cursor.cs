using Solitario.Game.Rendering;
using Solitario.Game.Types;

namespace Solitario.Game.Managers;

internal class Cursor {
  readonly static Dictionary<Areas, byte> areaMax = new()
  {
    { Areas.Foundation, 4 }, // Numero di fondazioni
    { Areas.Tableau, 7 } // Numero di pile nel tableau
  };

  private readonly Tableau tableau;

  internal Areas CurrentArea { get; private set; } = Areas.Tableau;
  /*Indica l'elemento corrente della selezione in base all'area, e vale solo per tableau e foundation.
   * Per esempio, se l'area corrente è Tableau, currentItemIndex indica l'indice della pila di carte selezionata.
   * Se invece è foundation, currentItemIndex indica l'indice della pila di fondazione selezionata.
  */
  internal int CurrentItemIndex { get; private set; } = 0;
  internal int CurrentCardPileIndex { get; private set; } = 0; // Indice della carta nella pla corrente (solo per tableau). Parte della carta più in basso della pila corrente.


  internal int[] Position { get; private set; } = { CardArt.cardWidth - 2, CardArt.cardHeight + 2 }; // Posizione iniziale del cursore (colonna, riga)
  internal int[] PrevPosition { get; private set; } = { CardArt.cardWidth - 2, CardArt.cardHeight + 2 }; // Posizione iniziale del cursore (colonna, riga)

  /*
   * 0 - currentItemIndex
   * 1 - currentCardPileIndex
  */
  internal int[] SelectionPosition { get; private set; } = { 0, 0 }; // Posizione del cursore di selezione (colonna, riga), usato per disegnare la selezione

  internal Cursor(Tableau tableau) {
    this.tableau = tableau;
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
    if (CurrentArea == Areas.Foundation) {
      SetPosition(CardArt.cardWidth * (4 + CurrentItemIndex) - 2, 1);
    }
    else { // Nel tableau
      if (CurrentCardPileIndex < 0) CurrentCardPileIndex = 0;
      SetPosition(CardArt.cardWidth * (CurrentItemIndex + 1) - 2, CardArt.cardHeight + 2 + CurrentCardPileIndex);
    }

  }

  internal void MoveUp() {
    if (CurrentArea == Areas.Foundation) return;
    bool canGoUp = CurrentCardPileIndex == 0 || !tableau.GetCard(CurrentItemIndex, CurrentCardPileIndex - 1).Revealed;


    // Vai su
    if (canGoUp) {
      CurrentItemIndex = CurrentItemIndex - 3 < 0 ? 0 : CurrentItemIndex - 3;
      CurrentArea = Areas.Foundation;
      UpdatePosition();
      return;
    }

    // Seleziona carta tra la pila
    CurrentCardPileIndex--;
    UpdatePosition();
  }

  internal void MoveDown() {
    // Vai giù
    if (CurrentArea == Areas.Foundation) {
      CurrentItemIndex = CurrentItemIndex + 3;
      CurrentCardPileIndex = tableau.GetPile(CurrentItemIndex).Count - 1;
      CurrentArea = Areas.Tableau;
      UpdatePosition();
      return;
    }

    if (tableau.GetPile(CurrentItemIndex).Count - 1 == CurrentCardPileIndex) return;
    CurrentCardPileIndex++;
    UpdatePosition();
  }

  internal void MoveLeft() {
    if (CurrentItemIndex <= 0) return;
    if (CurrentArea == Areas.Foundation) {
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

    if (CurrentArea == Areas.Foundation) {
      CurrentItemIndex++;

      UpdatePosition();
    }
    else {
      CurrentItemIndex++;
      CurrentCardPileIndex = tableau.GetPile(CurrentItemIndex).Count - 1;
      UpdatePosition();
    }
  }
}
