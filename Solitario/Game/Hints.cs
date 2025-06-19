using Solitario.Game.Managers;
using Solitario.Game.Models;
using Solitario.Game.Models.Actions;

/*
 * Ordine:
 * 1. Move any card to the Foundation - 100
 * 2. Reveal a face-down Tableau card - 75
 * 3. Move a King to an Empty Space - 60
 * 4. Play a card from the Waste pile to the Tableau - 50
 * 5. Tableau-Tableau - 10 + card value
 * 6. Draw card from stock - 1
*/

namespace Solitario.Game;
internal static class Hints {
  // Calcola il suggerimento
  /*
   * Ordine:
   * 1. Move any card to the Foundation - 100
   * 2. Play a card from the Waste pile to the Tableau - 50
   * 3. Draw card from stock - 1
  */
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
  /// Picks the best move out of all in a list
  /// </summary>
  /// <param name="actions"></param>
  /// <returns></returns>
  private static Tuple<IAction?, int> PickTableauMove(Tableau tableau, MoveCardsAction[] actions) {
    // Una lista con tutte le mosse candidate e i loro punteggi [Azione, punteggio]
    List<Tuple<MoveCardsAction, int>> candidates = new();
    int topScore = 0;

    /*
      * Ordine:
      * 1. Reveal a face-down Tableau card - 75
      * 2. Move a King to an Empty Space - 60
      * 3. Any other move - 10 + card value
    */

    foreach (var action in actions) {
      int score = 0;
      bool otherCases = false;

      // if a king
      if (action.CardsSelection.Count > 0 && action.CardsSelection[0].NumericValue == 13) {
        // Check if king is already top card
        if (tableau.GetPile(action.sourceIndex).Count == 0) { // Prevent errors
          if (tableau.GetPile(action.sourceIndex)[0].NumericValue != 13) {
            score += 60;
            otherCases = true;
          }
        }
      }

      // reveal a card
      if (tableau.GetPile(action.sourceIndex).Count > action.CardsSelection.Count) {
        int revealedCardCount = tableau.GetPile(action.sourceIndex).Count(card => card.Revealed);

        if (revealedCardCount > 0) {
          score += 75;
          otherCases = true;
        }
      }

      if (!otherCases) score += 10 + action.CardsSelection[0].NumericValue;

      candidates.Add(new(action, score));
      if (score > topScore) topScore = score;
    }

    var bestAction = candidates
    .OrderByDescending(candidate => candidate.Item2) // Ordina per punteggio (Item2)
    .Select(candidate => candidate.Item1)           // Seleziona l'azione (Item1)
    .FirstOrDefault();

    return new(bestAction, topScore);
  }

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
        return new(new MoveCardsAction(managers, Areas.Tableau, cPileIndex, Areas.Foundation, pileIndex, selection), 100);

      }
    }

    return new(null, 0);
  }

  #endregion

  /*
    * Ordine:
    * 1. Move to Foundation - 100
    * 2. Move to tableau - 50
   */
  private static Tuple<IAction?, int> GetWasteMove(Game.GameManagers managers) {
    Deck deck = managers.Deck;
    Tableau tableau = managers.Tableau;
    Foundation foundation = managers.Foundation;

    var wasteCard = deck.GetTopWaste();
    if (wasteCard == null) return new(null, 0);

    var selection = new Selection();

    #region moves to foundations
    // Find any moves to foundations

    for (int foundationIndex = 0; foundationIndex < 4; foundationIndex++) {
      var pileData = foundation.GetPile(foundationIndex);

      if (Validator.ValidateCardMove(wasteCard, pileData, Areas.Foundation, foundationIndex)) {
        selection.SetSelection(Areas.Waste, 0, [wasteCard]);
        return new(new MoveCardsAction(managers, Areas.Waste, 0, Areas.Foundation, foundationIndex, selection), 100);
      }
    }
    #endregion

    #region moves to tableau
    // Find moves for tableau

    for (int tableauIndex = 0; tableauIndex < 7; tableauIndex++) {
      var currentPileData = tableau.GetPile(tableauIndex);
      if (Validator.ValidateCardMove(wasteCard, currentPileData, Areas.Tableau)) {
        selection.SetSelection(Areas.Waste, 0, [wasteCard]);
        return new(new MoveCardsAction(managers, Areas.Waste, 0, Areas.Tableau, tableauIndex, selection), 50);
      }
    }
    #endregion

    return new(null, 0);
  }

}
