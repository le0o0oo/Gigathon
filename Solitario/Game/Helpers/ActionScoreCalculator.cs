using Solitario.Game.Managers;
using Solitario.Game.Models.Actions;

namespace Solitario.Game.Helpers;

/*
 * Ordine e punteggi:
 * 1. Muovi qualsiasi carta alla Fondazione - 100
 * 2. Mossa che rivela una carta nascosta nel Tableau - 75
 * 3. Muovi un re in uno spazio vuoto - 60
 * 4. Muovi una carta dagli scarti al Tableau - 50
 * 5. Mossa Tableau-Tableau - 10 + valore della carta
 * 6. Pesca carta dal mazzo - 1
*/

internal static class ActionScoreCalculator {
  /// <summary>
  /// Calcola il punteggio di un'azione
  /// </summary>
  /// <param name="action"></param>
  /// <param name="managers"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  internal static int Calculate(IAction action, Tableau tableau) {

    if (action is MoveCardsAction moveAction) {
      // Tableau-Tableau
      if (moveAction.sourceArea == Areas.Tableau && moveAction.destArea == Areas.Tableau) return CalculateTableauMove(tableau, moveAction);
      // [QUALSIASI]-Fondazione
      else if (moveAction.destArea == Areas.Foundation) return ActionScores.MoveToFoundation;
      // Fondazione-Tableau
      else if (moveAction.sourceArea == Areas.Foundation && moveAction.destArea == Areas.Tableau) return ActionScores.MoveToFoundation * -1;
      // Scarti-Tableau
      else if (moveAction.sourceArea == Areas.Deck && moveAction.destArea == Areas.Tableau) return ActionScores.MoveFromWasteToTableau;
      else throw new ArgumentException("Azione non valida/supportata.");
    }
    else if (action is DrawCardAction) return ActionScores.DrawFromDeck;
    else throw new ArgumentException("Azione non valida/supportata.");
  }


  #region Direct helpers
  /// <summary>
  /// Restituisce il punteggio di una mossa Tableau-Tableau
  /// </summary>
  /// <param name="actions"></param>
  /// <returns></returns>
  internal static int CalculateTableauMove(Tableau tableau, MoveCardsAction action) {
    /*
      * Ordine:
      * 1. Mossa che rivela una carta nascosta nel Tableau - 75
      * 2. Muovi un re in uno spazio vuoto - 60
      * 3. Qualsiasi altra mossa - 10 + valore carta
    */
    int score = 0;
    bool otherCases = false;

    // se è un re
    if (action.CardsSelection.Count > 0 && action.CardsSelection[0].NumericValue == 13) {
      // controlla se il re è non è la prima carta della pila
      if (action.CardsSelection[0].NumericValue == 13 && tableau.GetPile(action.destIndex).Count == 0) {
        score += ActionScores.MoveKingToEmptySpace;
        otherCases = true;
      }
    }

    // rivela una carta
    if (tableau.GetPile(action.sourceIndex).Count > action.CardsSelection.Count) {
      // Indice della carta che diventerà la nuova cima della pila di origine
      int newTopCardIndex = tableau.GetPile(action.sourceIndex).Count - action.CardsSelection.Count - 1;

      // Controlla se quella carta era nascosta
      if (!tableau.GetPile(action.sourceIndex)[newTopCardIndex].Revealed) {
        score += ActionScores.RevealTableauCard;
        otherCases = true;
      }
    }

    if (!otherCases) score += ActionScores.BaseTableauToTableauMove + action.CardsSelection[0].NumericValue;

    return score;
  }

  #endregion
}
