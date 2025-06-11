namespace Solitario.Game.Managers;
using Solitario.Game.Types;


internal class Selection {
  // Variabili di stato della selezione
  internal List<Card> selectedCards { get; private set; } = new List<Card>();
  internal int sourceIndex { get; private set; } = 0; // Indice della selezione corrente. Valido per tableau e foundation, e si riferisce alla pila della selezione.

  internal Areas sourceArea { get; private set; } = Areas.Waste;

  internal bool active { get; private set; } = false;

  internal Selection() {

  }

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