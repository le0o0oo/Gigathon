using Solitario.Game.Managers;

namespace Solitario.Game.Models.Actions;

/// <summary>
/// Rappresenta l'azione dello spostamento delle carte da e a di diverse carte
/// </summary>
internal class MoveCardsAction : IAction {
  internal readonly Areas sourceArea, destArea;
  internal readonly int sourceIndex, destIndex;
  //internal readonly int scoreValue;

  internal readonly int[] SelectionPosition;

  private readonly Tableau tableau;
  private readonly Foundation foundation;
  private readonly Deck deck;
  private readonly Selection selection;

  private readonly List<Card> _cardsSelection;
  /// <summary>
  /// Selezione attuale delle carte da spostare nella azione.
  /// </summary>
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
    this.selection = customSelection ?? managers.Selection;

    //this.scoreValue = Helpers.ActionScoreCalculator.Calculate(this, tableau);

    _cardsSelection = [.. selection.SelectedCards];

    SelectionPosition = managers.Cursor.SelectionPosition;
  }

  public void Execute() {
    // Dal tableau
    if (sourceArea == Areas.Tableau) {
      // Prende carte dal tableau
      var cardsToMove = tableau.TakeCards(sourceIndex, tableau.GetPile(sourceIndex).Count - selection.SelectedCards.Count);
      if (tableau.GetPile(sourceIndex).Count > 0) {
        _prevCardRevealed = tableau.GetPile(sourceIndex)[^1].Revealed;
        tableau.GetPile(sourceIndex)[^1].Revealed = true;
      }

      if (destArea == Areas.Tableau) {
        tableau.GetPile(destIndex).AddRange(cardsToMove);
      }
      else { // Alla fondazio e
        foundation.AddCard(cardsToMove[0]);
      }
    }
    // Dagli scarti
    else if (sourceArea == Areas.Deck) {
      var cardToMove = deck.TakeWasteCardAt(-1);
      cardToMove.Revealed = true;
      if (destArea == Areas.Tableau) {
        tableau.GetPile(destIndex).Add(cardToMove);
      }
      else { // Alla fondazione
        foundation.AddCard(cardToMove);
      }
    }
    // Dalla fondazione
    else if (sourceArea == Areas.Foundation) {
      var cardToMove = foundation.TakeCardAt(sourceIndex);
      tableau.GetPile(destIndex).Add(cardToMove);
    }
  }

  public void Undo() {
    if (sourceArea == Areas.Tableau) {
      // Ripristina lo stato Revealed dell'ultima carta
      if (tableau.GetPile(sourceIndex).Count > 0 && _prevCardRevealed != null) {
        tableau.GetPile(sourceIndex)[^1].Revealed = (bool)_prevCardRevealed;
      }

      // Rimetti le carte nel tableau
      tableau.GetPile(sourceIndex).AddRange(_cardsSelection);

      if (destArea == Areas.Tableau) {
        tableau.TakeCards(destIndex, tableau.GetPile(destIndex).Count - _cardsSelection.Count);
      }
      else { // Rimuove dalla fondazione
        foundation.GetPile(destIndex).RemoveAt(foundation.GetPile(destIndex).Count - 1);
      }
    }
    // Dagli scarti
    else if (sourceArea == Areas.Deck) {
      // Rimetti carta negli scarti
      var card = _cardsSelection[0];
      card.Revealed = false;
      deck.AddToWaste(card);

      if (destArea == Areas.Tableau) {
        tableau.GetPile(destIndex).Remove(_cardsSelection[0]);
      }
      else { // Rimuove dalla fondazione
        foundation.GetPile(destIndex).Remove(_cardsSelection[0]);
      }
    }
    // Dalla fondazione
    else if (sourceArea == Areas.Foundation) {
      foundation.AddCard(_cardsSelection[0]);
      tableau.GetPile(destIndex).Remove(_cardsSelection[0]);
    }
  }
}
