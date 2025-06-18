namespace Solitario.Game.Managers;

using Solitario.Game.Models;

internal class Selection {
  // Variabili di stato della selezione

  // 0 = carta più in alto della pila di selezione
  internal List<Card> selectedCards { get; private set; } = new List<Card>();
  internal int sourceIndex { get; private set; } = 0; // Indice della selezione corrente. Valido per tableau e foundation, e si riferisce alla pila della selezione.

  internal Areas sourceArea { get; private set; } = Areas.Waste;

  internal bool active { get; private set; } = false;

  internal Selection() {

  }

  /// <summary>
  /// Sets the current selection
  /// </summary>
  /// <param name="area">The current area of the selection</param>
  /// <param name="pileIndex">The pile index of the selection</param>
  /// <param name="cards">An array of the currently selected cards in the current pile</param>
  public void SetSelection(Areas area, int pileIndex, List<Card> cards) {
    sourceArea = area;
    sourceIndex = pileIndex;
    selectedCards = cards;
    active = true; // Attiva la selezione

    // Evidenzia la selezione

  }
  public void ClearSelection() {
    selectedCards.Clear();
    active = false; // Disattiva la selezione
  }

}