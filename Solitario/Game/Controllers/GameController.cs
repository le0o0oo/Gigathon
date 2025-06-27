using Solitario.Game.Data;
using Solitario.Game.Helpers;
using Solitario.Game.Managers;
using Solitario.Game.Models;
using Solitario.Game.Models.Actions;
using Solitario.Game.Rendering;

namespace Solitario.Game.Controllers;

internal class GameController {
  private Game.GameManagers Managers { get; init; }
  private readonly Selection selection;
  private readonly Stats statsManager;
  private readonly Actions actionsManager;
  private readonly Deck deck;
  private readonly Legend legend;
  private readonly Renderer renderer;
  private readonly Tableau tableau;
  private readonly Foundation foundation;
  private readonly Cursor cursor;
  private readonly Game game;
  private readonly Hint hintManager;

  internal GameController(Game.GameManagers managers, Game game) {
    Managers = managers;
    selection = managers.Selection;
    statsManager = managers.StatsManager;
    actionsManager = managers.ActionsManager;
    deck = managers.Deck;
    legend = managers.Legend;
    renderer = new Renderer(managers);
    tableau = managers.Tableau;
    foundation = managers.Foundation;
    cursor = managers.Cursor;
    hintManager = managers.HintManager;
    this.game = game;
  }

  /// <summary>
  /// Pesca una carta dal mazzo e la mette negli scarti
  /// </summary>
  internal void AttempDrawCard() {
    if (selection.Active) return;
    var drawAction = new DrawCardAction(deck);
    statsManager.ApplyActionScore(drawAction);
    actionsManager.Execute(drawAction);
    statsManager.IncMovesCount();

    legend.SetCanUndo(actionsManager.CanUndo());
  }

  /// <summary>
  /// Seleziona la carta degli scarti
  /// </summary>
  internal void AttemptWasteSelection() {
    if (selection.Active || deck.GetWaste().Count == 0) return;
    selection.SetSelection(Areas.Deck, 0, [deck.GetWasteCardAt(-1)]);
    legend.SetSelected(true);
  }

  /// <summary>
  /// Deseleziona l'elemento/i attualmente selezionato/i
  /// </summary>
  /// <returns>Se l'azione è stata eseguita o meno</returns>
  internal bool ClearSelection() {
    if (!selection.Active) return false;
    selection.ClearSelection();
    legend.SetSelected(false);

    return true;
  }

  /// <summary>
  /// Porta automaticamente la carta selezionata in fondazione, se possibile.
  /// </summary>
  internal void AttemptCardToFoundation() {
    Card card;
    Selection movSelection = new();
    Areas area;
    if (selection.Active && selection.SourceArea == Areas.Deck) {
      card = selection.SelectedCards[0]; // Prende la prima carta della selezione
      area = Areas.Deck;
      movSelection.SetSelection(Areas.Deck, 0, [card]);
    }
    else if (cursor.CurrentArea == Areas.Tableau && !selection.Active) {
      if (cursor.CurrentCardIndex != tableau.GetPile(cursor.CurrentItemIndex).Count - 1) return; // Non puoi piazzare in fondazione se non hai selezionato l'ultima carta
      card = tableau.GetCard(cursor.CurrentItemIndex);
      area = Areas.Tableau;
      movSelection.SetSelection(Areas.Tableau, cursor.CurrentItemIndex, [card]);
    }
    else return;

    var foundationPileIndex = Foundation.seedIndexMap[card.Seed];
    if (!Validator.ValidateCardMove(card, foundation.GetPile(foundationPileIndex), Areas.Foundation, foundationPileIndex)) return; // Non puoi piazzare in fondazione se non è valida
    selection.ClearSelection();
    legend.SetSelected(false);

    var action = new MoveCardsAction(Managers, area, cursor.CurrentItemIndex, Areas.Foundation, foundationPileIndex, movSelection);

    statsManager.IncMovesCount();
    statsManager.ApplyActionScore(action);
    actionsManager.Execute(action);
    cursor.DecCurrentCardIndex();
  }

  /// <summary>
  /// Annulla l'ultima azione eseguita, se possibile.
  /// </summary>
  internal void UndoLastAction() {
    if (selection.Active) return;
    if (!actionsManager.CanUndo()) return;

    var undoAction = actionsManager.Undo();
    statsManager.ApplyUndoPenality();
    statsManager.RemoveActionScore(undoAction);
    statsManager.IncUndosCount();
    statsManager.DecMovesCount();

    legend.SetCanUndo(actionsManager.CanUndo());
  }

  /// <summary>
  /// Mostra un suggerimento per la prossima mossa, se disponibile.
  /// </summary>
  /// <returns>Se lo stato delle hint è stato mutato</returns>
  internal bool RequestHint() {
    if (selection.Active || !CurrentSettings.UseHints) return false;

    var hint = Hints.FindHint(Managers);
    if (hint == null) return false;
    bool changedHintState = false;

    if (!hintManager.ShowingHint) {
      statsManager.IncHintsCount();
      statsManager.ApplyHintPenalty();
      hintManager.SetLastAction(hint);
      renderer.DrawAction(hint);
      hintManager.ShowingHint = true;
      changedHintState = true;
    }
    else if (hintManager.LastAction != null) {
      // Non ricacolare hint
      statsManager.ApplyActionScore(hintManager.LastAction!);
      actionsManager.Execute(hintManager.LastAction);
    }

    renderer.DrawStats();

    return changedHintState;
  }

  #region Gestione delle selezioni
  /// <summary>
  /// Gestisce la selezione attuale delle carte, ridisegnado le aree interessate.
  /// </summary>
  internal void HandleSelection() {
    var wasActive = selection.Active;

    HandleSelectAction();

    // If a move was just completed (selection is now inactive)
    if (wasActive && !selection.Active) {
      statsManager.IncMovesCount();
    }
    // If a selection was just made
    else if (!wasActive && selection.Active) {
      renderer.DrawSelection();
    }

    legend.SetSelected(selection.Active);
  }

  /// <summary>
  /// Gestisce l'azione di selezione delle carte, sia per piazzarle che per prenderle.
  /// </summary>
  private void HandleSelectAction() {
    if (selection.Active) {
      // Piazza le carte
      var targetArea = cursor.CurrentArea; // Tableau o fondazioni
      var targetPileIndex = cursor.CurrentItemIndex;

      // 1. Controlla se mossa valida
      if (!Validator.ValidateCardMove(selection.SelectedCards[0], GetPile(targetArea, targetPileIndex), targetArea, targetPileIndex)) {
        return; // Invalid move, do nothing.
      }

      // 2. Piazza le carte
      PlaceSelectedCards(selection.SourceArea, selection.SourceIndex, targetArea, targetPileIndex);

      // 3. Cleanup
      selection.ClearSelection();
      legend.SetSelected(false);
    }
    else {
      // Prendi le carte da tableau
      if (cursor.CurrentArea == Areas.Tableau) {
        var pile = tableau.GetPile(cursor.CurrentItemIndex);
        if (pile.Count == 0 || cursor.CurrentCardIndex >= pile.Count) return;

        var cardsToSelect = new List<Card>();
        for (int i = cursor.CurrentCardIndex; i < pile.Count; i++) {
          cardsToSelect.Add(pile[i]);
        }
        if (!cardsToSelect[0].Revealed) return; // Carte non rivelate, non prenderle

        cursor.SelectionPosition[0] = cursor.CurrentItemIndex;
        cursor.SelectionPosition[1] = cursor.CurrentCardIndex;

        selection.SetSelection(Areas.Tableau, cursor.CurrentItemIndex, cardsToSelect);
        legend.SetSelected(true);
      }
      else { // Cursore nella fondazione, prende da essa.
        var pile = foundation.GetPile(cursor.CurrentItemIndex);
        if (pile.Count == 0) return;

        cursor.SelectionPosition[0] = cursor.CurrentItemIndex;
        cursor.SelectionPosition[1] = cursor.CurrentCardIndex;

        selection.SetSelection(Areas.Foundation, cursor.CurrentItemIndex, [pile[^1]]);
        legend.SetSelected(true);
      }
    }

    legend.SetCanUndo(actionsManager.CanUndo());

  }

  // Piazza le carte della selezione.
  private void PlaceSelectedCards(Areas sourceArea, int sourceIndex, Areas destArea, int destIndex) {
    var action = new MoveCardsAction(Managers, sourceArea, sourceIndex, destArea, destIndex);

    statsManager.ApplyActionScore(action);
    actionsManager.Execute(action);
  }

  /// <summary>
  /// Ottieni pila da ogni area
  /// </summary>
  /// <param name="area"></param>
  /// <param name="index"></param>
  /// <returns></returns>
  private List<Card> GetPile(Areas area, int index) {
    return area switch
    {
      Areas.Tableau => tableau.GetPile(index),
      Areas.Foundation => foundation.GetPile(index),
      _ => deck.GetWaste()
    };
  }
  #endregion
}
