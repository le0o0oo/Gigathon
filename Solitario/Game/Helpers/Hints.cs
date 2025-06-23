using Solitario.Game.Managers;
using Solitario.Game.Models;
using Solitario.Game.Models.Actions;

/*
 * Questa classe calcola ogni possibile mossa valida e assegna un punteggio a quella puù rilevante
 * 
 * Ordine e punteggi:
 * 1. Muovi qualsiasi carta alla Fondazione - 100
 * 2. Mossa che rivela una carta nascosta nel Tableau - 75
 * 3. Muovi un re in uno spazio vuoto - 60
 * 4. Muovi una carta dagli scarti al Tableau - 50
 * 5. Mossa Tableau-Tableau - 10 + valore della carta
 * 6. Pesca carta dal mazzo - 1
*/

namespace Solitario.Game.Helpers;
internal static class Hints {
  /*
   * Ordine:
   * 1. Muovi qualsiasi carta alla Fondazione - 100
   * 2. Muovi una carta dagli scarti al Tableau - 50
   * 3. Pesca carta dal mazzo - 1
  */
  /// <summary>
  /// Trova il suggerimento
  /// </summary>
  /// <param name="managers"></param>
  /// <returns>Un oggetto di azione</returns>
  internal static IAction? FindHint(Game.GameManagers managers) {
    Deck deck = managers.Deck;
    Foundation foundation = managers.Foundation;
    Tableau tableau = managers.Tableau;

    List<Tuple<IAction?, int>> candidates = new();

    candidates.Add(PickTableauMove(tableau, GetTableauMoves(managers)));
    candidates.Add(GetWasteMove(managers));
    candidates.Add(TableauToFoundation(managers));
    if (deck.GetCards().Count > 0 || deck.GetWaste().Count > 0) candidates.Add(new(new DrawCardAction(deck), 1));


    var bestAction = candidates
.OrderByDescending(candidate => candidate.Item2) // Ordina per punteggio (Item2)
.Select(candidate => candidate.Item1)           // Seleziona l'azione (Item1)
.FirstOrDefault();

    return bestAction;
  }

  #region Tableau pick
  /// <summary>
  /// Restituisce la mossa migliore data una lista di mosse nel Tableau
  /// </summary>
  /// <param name="actions"></param>
  /// <returns></returns>
  private static Tuple<IAction?, int> PickTableauMove(Tableau tableau, MoveCardsAction[] actions) {
    // Una lista con tutte le mosse candidate e i loro punteggi [Azione, punteggio]
    List<Tuple<MoveCardsAction, int>> candidates = [];
    int topScore = 0;

    foreach (var action in actions) {
      int score = ActionScoreCalculator.CalculateTableauMove(tableau, action);

      candidates.Add(new(action, score));
      if (score > topScore) topScore = score;
    }

    var bestAction = candidates
    .OrderByDescending(candidate => candidate.Item2) // Ordina per punteggio (Item2)
    .Select(candidate => candidate.Item1)           // Seleziona l'azione (Item1)
    .FirstOrDefault();

    return new(bestAction, topScore);
  }

  /// <summary>
  /// Calcola tutte le mosse possibili nel tableau
  /// </summary>
  /// <param name="managers"></param>
  /// <returns>Tutte le mosse possibili</returns>
  private static MoveCardsAction[] GetTableauMoves(Game.GameManagers managers) {
    Tableau tableau = managers.Tableau;

    /*
     * Una lista che contiene
     * [Azione, punteggio]
     */
    List<MoveCardsAction> potentialActions = [];

    // Itera per ogni pila del tableau
    for (int cPileIndex = 0; cPileIndex < 7; cPileIndex++) {
      var referencePile = tableau.GetPile(cPileIndex);

      // Itera per ogni carta della pila
      for (int cardIndex = 0; cardIndex < referencePile.Count; cardIndex++) {
        // Se la carta non è visibile salta iterazione corrente
        if (!tableau.GetCard(cPileIndex, cardIndex).Revealed) continue;

        // Controlla le altre pile relative alla carta
        for (int pileIndex = 0; pileIndex < 7; pileIndex++) {
          if (pileIndex == cPileIndex) continue;

          var currentCard = tableau.GetCard(cPileIndex, cardIndex);
          // Mossa valida
          if (Validator.ValidateCardMove(currentCard, tableau.GetPile(pileIndex), Areas.Tableau)) {
            List<Card> selectionCards = referencePile.GetRange(cardIndex, referencePile.Count - cardIndex);

            // Evita di entrare in un loop infinito
            if (cardIndex > 0 && referencePile.Count > 0) {
              if (Validator.ValidateCardMove(currentCard, [referencePile[cardIndex - 1]], Areas.Tableau)) {
                continue;
              }
            }
            else if (tableau.GetPile(pileIndex).Count == 0 && referencePile.Count > 0) {
              if (referencePile[0].NumericValue == 13) continue;
            }

            // 1. Crea l'oggetto di selezione
            var actionSelection = new Selection();
            actionSelection.SetSelection(Areas.Tableau, cPileIndex, selectionCards);

            // 2. Crea la mossa
            var action = new MoveCardsAction(managers, Areas.Tableau, cPileIndex, Areas.Tableau, pileIndex, actionSelection);

            // 3. Aggiungi la mossa ai candidati
            potentialActions.Add(action);
          }
        }
      }
    }

    return [.. potentialActions];
  }

  /// <summary>
  /// Determina se è possibile spostare una carta dal Tableau alla Fondazione, restutendo un oggetto azione con il suo punteggio in caso positivo.
  /// </summary>
  /// <param name="managers"></param>
  /// <returns></returns>
  private static Tuple<IAction?, int> TableauToFoundation(Game.GameManagers managers) {
    Tableau tableau = managers.Tableau;
    Foundation foundation = managers.Foundation;

    // Itera per ogni pila
    for (int cPileIndex = 0; cPileIndex < 7; cPileIndex++) {
      int cardIndex = tableau.GetPile(cPileIndex).Count - 1;
      if (cardIndex < 0) continue;

      // Itera per ogni carta
      var currentCard = tableau.GetCard(cPileIndex, cardIndex);

      // Se nascosta, skippa
      if (!currentCard.Revealed) continue;

      int pileIndex = Foundation.seedIndexMap[currentCard.Seed];
      if (Validator.ValidateCardMove(currentCard, foundation.GetPile(pileIndex), Areas.Foundation, pileIndex)) {
        Selection selection = new();
        selection.SetSelection(Areas.Tableau, cPileIndex, [currentCard]);
        return new(new MoveCardsAction(managers, Areas.Tableau, cPileIndex, Areas.Foundation, pileIndex, selection), ActionScores.MoveToFoundation);
      }
    }

    return new(null, ActionScores.NoMove);
  }

  #endregion

  /*
    * Ordine:
    * 1. Muovi a fondazione - 100
    * 2. Muovi a tableau - 50
   */
  /// <summary>
  /// Determina se è possibile spostare una carta dagli scarti al Tableau o alla Fondazione, restituendo un oggetto azione con il suo punteggio in caso positivo.
  /// </summary>
  /// <param name="managers"></param>
  /// <returns></returns>
  private static Tuple<IAction?, int> GetWasteMove(Game.GameManagers managers) {
    Deck deck = managers.Deck;
    Tableau tableau = managers.Tableau;
    Foundation foundation = managers.Foundation;

    var wasteCard = deck.GetTopWaste();
    if (wasteCard == null) return new(null, ActionScores.NoMove);

    var selection = new Selection();

    #region moves to foundations
    // Find any moves to foundations

    for (int foundationIndex = 0; foundationIndex < 4; foundationIndex++) {
      var pileData = foundation.GetPile(foundationIndex);

      if (Validator.ValidateCardMove(wasteCard, pileData, Areas.Foundation, foundationIndex)) {
        selection.SetSelection(Areas.Deck, 0, [wasteCard]);
        return new(new MoveCardsAction(managers, Areas.Deck, 0, Areas.Foundation, foundationIndex, selection), ActionScores.MoveToFoundation);
      }
    }
    #endregion

    #region moves to tableau
    // Find moves for tableau

    for (int tableauIndex = 0; tableauIndex < 7; tableauIndex++) {
      var currentPileData = tableau.GetPile(tableauIndex);
      if (Validator.ValidateCardMove(wasteCard, currentPileData, Areas.Tableau)) {
        selection.SetSelection(Areas.Deck, 0, [wasteCard]);
        return new(new MoveCardsAction(managers, Areas.Deck, 0, Areas.Tableau, tableauIndex, selection), ActionScores.MoveFromWasteToTableau);
      }
    }
    #endregion

    return new(null, ActionScores.NoMove);
  }

}
