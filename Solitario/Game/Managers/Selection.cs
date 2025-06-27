namespace Solitario.Game.Managers;
using Solitario.Game.Data;

using Solitario.Game.Models;

internal class Selection {
  // Variabili di stato della selezione

  // 0 = carta più in alto della pila di selezione
  internal List<Card> SelectedCards { get; private set; } = []; // Lista che rappresenta la selezione attuale
  internal int SourceIndex { get; private set; } = 0; // Indice della selezione corrente. Valido per tableau e foundation, e si riferisce alla pila della selezione.
  internal Areas SourceArea { get; private set; } = Areas.Deck;
  internal bool Active { get; private set; } = false;

  internal Selection() {

  }

  /// <summary>
  /// Imposta la selezione attuale
  /// </summary>
  /// <param name="area">L'area della selezione</param>
  /// <param name="pileIndex">L'indice della pila della selezione</param>
  /// <param name="cards">Una lista contenente tutte le carte della pila</param>
  public void SetSelection(Areas area, int pileIndex, List<Card> cards) {
    SourceArea = area;
    SourceIndex = pileIndex;
    SelectedCards = cards;
    Active = true; // Attiva la selezione
  }

  /// <summary>
  /// Annulla la selezione
  /// </summary>
  public void ClearSelection() {
    SelectedCards.Clear();
    Active = false; // Disattiva la selezione
  }

}