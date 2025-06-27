using Solitario.Game.Models;
using Solitario.Game.Rendering.Helpers;

namespace Solitario.Game.Managers;

internal class Cursor {
  readonly static Dictionary<Areas, byte> areaMax = new()
  {
    { Areas.Foundation, 4 }, // Numero di fondazioni
    { Areas.Tableau, 7 } // Numero di pile nel tableau
  };

  private readonly Tableau tableau;
  private const int FoundationToTableauRowOffset = 3;

  internal Areas CurrentArea { get; private set; } = Areas.Tableau;

  /// <summary>
  /// Indica l'elemento corrente della selezione in base all'area, e vale solo per tableau e foundation.
  /// Per esempio, se l'area corrente è Tableau, currentItemIndex indica l'indice della pila di carte selezionata.
  /// Se invece è foundation, currentItemIndex indica l'indice della pila di fondazione selezionata.
  /// </summary>
  internal int CurrentItemIndex { get; private set; } = 0;

  /// <summary>
  /// Indice della carta nella pla corrente (solo per tableau). Parte della carta più in basso della pila corrente.
  /// </summary>
  internal int CurrentCardIndex { get; private set; } = 0;

  internal ConsolePoint Position { get; private set; } = new(CardArt.cardWidth - 2, CardArt.cardHeight + 2); // Posizione iniziale del cursore (colonna, riga)
  internal ConsolePoint PrevPosition { get; private set; } = new(CardArt.cardWidth - 2, CardArt.cardHeight + 2); // Posizione iniziale del cursore (colonna, riga)

  /*
   * 0 - currentItemIndex
   * 1 - currentCardPileIndex
  */
  internal int[] SelectionPosition { get; private set; } = { 0, 0 }; // Posizione del cursore di selezione (colonna, riga), usato per disegnare la selezione

  internal Cursor(Tableau tableau) {
    this.tableau = tableau;
  }

  /// <summary>
  /// Imposta le coordinate della posizione del cursore
  /// </summary>
  /// <param name="top">Numero di righe a partire dall'inizio</param>
  /// <param name="left">Numero di colonne a partire da sinistra</param>
  private void SetPosition(int left, int top) {
    // Salva la posizione precedente per il disegno della selezione
    PrevPosition = new(Position.X, Position.Y);

    // Aggiorna la posizione corrente
    Position = new(left, top);
  }

  /// <summary>
  /// Calcola e aggiorna le coordinate della posizione del cursore nella console in base allo stato attuale
  /// </summary>
  private void UpdatePosition() {
    if (CurrentArea == Areas.Foundation) {
      SetPosition(CardArt.cardWidth * (4 + CurrentItemIndex) - 2, 1);
    }
    else { // Nel tableau
      if (CurrentCardIndex < 0) CurrentCardIndex = 0;
      SetPosition(CardArt.cardWidth * (CurrentItemIndex + 1) - 2, CardArt.cardHeight + 2 + CurrentCardIndex);
    }

  }

  internal void MoveUp() {
    if (CurrentArea == Areas.Foundation) return;
    if (CurrentCardIndex > tableau.GetPile(CurrentItemIndex).Count - 1) {
      CurrentCardIndex = tableau.GetPile(CurrentItemIndex).Count - 1 > 0 ? tableau.GetPile(CurrentItemIndex).Count - 1 : 0;
      //UpdatePosition();
      //return;
    }
    bool canGoUp = CurrentCardIndex == 0 || !tableau.GetCard(CurrentItemIndex, CurrentCardIndex - 1).Revealed;


    // Vai su
    if (canGoUp) {
      CurrentItemIndex = CurrentItemIndex - FoundationToTableauRowOffset < 0 ? 0 : CurrentItemIndex - FoundationToTableauRowOffset;
      CurrentArea = Areas.Foundation;
      UpdatePosition();
      return;
    }

    // Seleziona carta tra la pila
    CurrentCardIndex--;
    UpdatePosition();
  }

  internal void MoveDown() {
    // Vai giù
    if (CurrentArea == Areas.Foundation) {
      CurrentItemIndex = CurrentItemIndex + FoundationToTableauRowOffset;
      CurrentCardIndex = tableau.GetPile(CurrentItemIndex).Count - 1;
      CurrentArea = Areas.Tableau;
      UpdatePosition();
      return;
    }

    if (tableau.GetPile(CurrentItemIndex).Count - 1 <= CurrentCardIndex ||
      tableau.GetPile(CurrentItemIndex).Count == 0) return;
    CurrentCardIndex++;
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
      CurrentCardIndex = tableau.GetPile(CurrentItemIndex).Count - 1;
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
      CurrentCardIndex = tableau.GetPile(CurrentItemIndex).Count - 1;
      UpdatePosition();
    }
  }

  /// <summary>
  /// Decrementa l'indice della carta corrente nella pila corrente del tableau.
  /// </summary>
  internal void DecCurrentCardIndex() {
    if (CurrentCardIndex <= 0) return;
    bool canGoUp = CurrentCardIndex == 0 || !tableau.GetCard(CurrentItemIndex, CurrentCardIndex - 1).Revealed;

    if (canGoUp) return;

    CurrentCardIndex--;
    UpdatePosition();
  }
}
