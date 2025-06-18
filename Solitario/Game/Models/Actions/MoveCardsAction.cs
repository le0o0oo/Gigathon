using Solitario.Game.Managers;

namespace Solitario.Game.Models.Actions;

/// <summary>
/// Gestisce il movimento delle carte.
/// </summary>
internal class MoveCardsAction : IAction {
  internal readonly Areas sourceArea, destArea;
  internal readonly int sourceIndex, destIndex;

  private readonly Tableau tableau;
  private readonly Foundation foundation;
  private readonly Deck deck;
  private readonly Selection selection;

  private readonly List<Card> _cardsSelection;
  internal IReadOnlyList<Card> CardsSelection => _cardsSelection;

  private bool? _prevCardRevealed;

  internal MoveCardsAction(Game.GameManagers managers, Areas sourceArea, int sourceIndex, Areas destArea, int destIndex, Selection? customSelection = null) {
    this.sourceArea = sourceArea;
    this.sourceIndex = sourceIndex;
    this.destArea = destArea;
    this.destIndex = destIndex;

    this.tableau = managers.Tableau;
    this.foundation = managers.Foundation;
    this.deck = managers.Deck;
    this.selection = customSelection == null ? managers.Selection : customSelection;

    _cardsSelection = [.. selection.selectedCards];
  }

  public void Execute() {
    // From tableau
    if (sourceArea == Areas.Tableau) {
      // Take cards from tableau
      var cardsToMove = tableau.TakeCards(sourceIndex, tableau.GetPile(sourceIndex).Count - selection.selectedCards.Count);
      if (tableau.GetPile(sourceIndex).Count > 0) {
        _prevCardRevealed = tableau.GetPile(sourceIndex)[^1].Revealed;
        tableau.GetPile(sourceIndex)[^1].Revealed = true;
      }

      if (destArea == Areas.Tableau) {
        tableau.GetPile(destIndex).AddRange(cardsToMove);
      }
      else { // To Foundation
        foundation.AddCard(cardsToMove[0]);
      }
    }
    // From waste
    else if (sourceArea == Areas.Waste) {
      var cardToMove = deck.TakeWasteCardAt(-1);
      cardToMove.Revealed = true;
      if (destArea == Areas.Tableau) {
        tableau.GetPile(destIndex).Add(cardToMove);
      }
      else { // To Foundation
        foundation.AddCard(cardToMove);
      }
    }
    // From foundation
    else if (sourceArea == Areas.Foundation) {
      var cardToMove = foundation.TakeCardAt(sourceIndex);
      tableau.GetPile(destIndex).Add(cardToMove);
    }
  }

  public void Undo() {
    if (sourceArea == Areas.Tableau) {
      // Hide the last card
      if (tableau.GetPile(sourceIndex).Count > 0 && _prevCardRevealed != null) {
        tableau.GetPile(sourceIndex)[^1].Revealed = (bool)_prevCardRevealed;
      }

      // Put cards back into tableau
      tableau.GetPile(sourceIndex).AddRange(_cardsSelection);

      if (destArea == Areas.Tableau) {
        tableau.TakeCards(destIndex, tableau.GetPile(destIndex).Count - _cardsSelection.Count);
      }
      else { // Remove from foundation
        foundation.GetPile(destIndex).RemoveAt(foundation.GetPile(destIndex).Count - 1);
      }
    }
    // From waste
    else if (sourceArea == Areas.Waste) {
      // Put back card into waste
      var card = _cardsSelection[0];
      card.Revealed = false;
      deck.AddToWaste(card);

      if (destArea == Areas.Tableau) {
        tableau.GetPile(destIndex).Remove(_cardsSelection[0]);
      }
      else { // Remove from Foundation
        foundation.GetPile(destIndex).Remove(_cardsSelection[0]);
      }
    }
    // From foundation
    else if (sourceArea == Areas.Foundation) {
      foundation.AddCard(_cardsSelection[0]);
      tableau.GetPile(destIndex).Remove(_cardsSelection[0]);
    }
  }
}
