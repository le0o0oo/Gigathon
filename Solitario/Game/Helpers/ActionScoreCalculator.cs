using Solitario.Game.Data;
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
  /// <exception cref="ArgumentException">Quando una mossa non è valida</exception>
  internal static int Calculate(IAction action, Tableau tableau) {
    if (action is not MoveCardsAction moveAction) {
      return 0;
    }

    return (moveAction.sourceArea, moveAction.destArea) switch
    {
      (Areas.Tableau, Areas.Tableau) => CalculateTableauToTableauScore(tableau, moveAction),
      (_, Areas.Foundation) => ActionScores.MoveToFoundation,
      (Areas.Foundation, Areas.Tableau) => ActionScores.MoveFromFoundationToTableau,
      (Areas.Deck, Areas.Tableau) => ActionScores.MoveFromWasteToTableau,
      _ => 0 // Nessun punteggio per altre mosse
    };
  }


  #region Direct helpers
  /// <summary>
  /// Restituisce il punteggio di una mossa Tableau-Tableau
  /// </summary>
  /// <param name="actions"></param>
  /// <returns></returns>
  internal static int CalculateTableauMoveHint(Tableau tableau, MoveCardsAction action) {
    /*
      * Ordine:
      * 1. Mossa che rivela una carta nascosta nel Tableau - 75
      * 2. Muovi un re in uno spazio vuoto - 60
      * 3. Qualsiasi altra mossa - 10 + valore carta
    */
    int score = 0;

    if (IsRevealingNewCard(tableau, action)) {
      score += ActionScores.HintRevealTableauCard;
    }

    if (IsMovingKingToEmptySpace(tableau, action)) {
      score += ActionScores.HintMoveKingToEmptySpace;
    }

    if (score == 0) score += ActionScores.HintBaseTableauToTableauMove + action.CardsSelection[0].NumericValue;

    return score;
  }

  #endregion

  #region Private helpers
  /// <summary>
  /// Calcola il punteggio per una mossa da Tableau a Tableau.
  /// </summary>
  private static int CalculateTableauToTableauScore(Tableau tableau, MoveCardsAction moveAction) {
    int score = 0;

    if (IsRevealingNewCard(tableau, moveAction)) {
      score += ActionScores.RevealTableauCard;
    }

    if (IsMovingKingToEmptySpace(tableau, moveAction)) {
      score += ActionScores.MoveKingToEmptySpace;
    }

    if (score == 0) score += ActionScores.MoveFromTableauToTableau;

    return score;
  }

  /// <summary>
  /// Controlla se l'azione di movimento sposterà un Re (la prima carta della selezione) in una colonna vuota del tableau.
  /// </summary>
  private static bool IsMovingKingToEmptySpace(Tableau tableau, MoveCardsAction moveAction) {
    // La pila di destinazione deve essere vuota e la carta spostata un Re.
    return tableau.GetPile(moveAction.destIndex).Count == 0 &&
           moveAction.CardsSelection.Count > 0 &&
           moveAction.CardsSelection[0].NumericValue == 13;
  }

  /// <summary>
  /// Controlla se l'azione di movimento rivelerà una carta precedentemente coperta nella pila di origine.
  /// </summary>
  /// <remarks>
  /// Questo metodo deve essere chiamato PRIMA che l'azione venga eseguita.
  /// </remarks>
  private static bool IsRevealingNewCard(Tableau tableau, MoveCardsAction moveAction) {
    var sourcePile = tableau.GetPile(moveAction.sourceIndex);
    int cardsToMoveCount = moveAction.CardsSelection.Count;

    // C'è una carta sotto a quelle che vengono spostate?
    if (sourcePile.Count > cardsToMoveCount) {
      // Indice della carta che diventerà la nuova cima della pila di origine
      int newTopCardIndex = sourcePile.Count - cardsToMoveCount - 1;

      // La carta era nascosta?
      return !sourcePile[newTopCardIndex].Revealed;
    }

    // Non ci sono carte sotto, quindi nessuna carta verrà rivelata.
    return false;
  }

  #endregion
}
