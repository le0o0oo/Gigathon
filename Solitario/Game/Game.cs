using Solitario.Game.Controllers;
using Solitario.Game.Managers;
using Solitario.Game.Models.Actions;
using Solitario.Game.Rendering;

namespace Solitario.Game;

internal class Game {
  internal readonly Deck deck;
  internal readonly Tableau tableau;
  internal readonly Foundation foundation;
  private readonly Cursor cursor;
  private readonly Legend legend;
  private readonly Selection selection;
  private readonly Managers.Hint hintManager;
  private readonly InputHandler inputHandler;
  internal readonly Stats statsManager;

  private readonly Renderer renderer;
  private readonly Actions actionsManager;

  private int prevMovesCount = 0;

  internal record GameManagers(
    Deck Deck,
    Tableau Tableau,
    Foundation Foundation,
    Actions ActionsManager,
    Cursor Cursor,
    Legend Legend,
    Selection Selection,
    Managers.Hint HintManager,
    Stats StatsManager
  );
  internal GameManagers Managers => new(deck, tableau, foundation, actionsManager, cursor, legend, selection, hintManager, statsManager);

  internal Action? OnWin;
  internal Action? OnEsc;

  internal Game(Deck? deck = null, Tableau? tableau = null, Foundation? foundation = null, Stats? statsManager = null) {
    deck ??= new Deck();
    tableau ??= new Tableau(deck);
    foundation ??= new Foundation();
    statsManager ??= new Stats(tableau);

    this.deck = deck;
    this.tableau = tableau;
    this.foundation = foundation;
    this.statsManager = statsManager;

    legend = new Legend();
    selection = new Selection();
    cursor = new Cursor(tableau);
    hintManager = new Managers.Hint();
    actionsManager = new Actions();

    renderer = new Renderer(Managers);
    inputHandler = new InputHandler(this, renderer, Managers);

    prevMovesCount = statsManager.MovesCount;
  }

  #region Public methods
  /// <summary>
  /// Gestisce input utente
  /// </summary>
  /// <param name="keyInfo"></param>
  internal void Input(ConsoleKeyInfo keyInfo) {
    inputHandler.ProcessInput(keyInfo);

    if (statsManager.MovesCount != prevMovesCount) {
      if (actionsManager.LastAction != null) DrawDirtyAreas(actionsManager.LastAction);
      // Area sconosciuta
      else Draw();
      prevMovesCount = statsManager.MovesCount;
    }

    if (HasWon()) {
      statsManager.CalculateFinalScore();
      OnWin?.Invoke();
    }
  }

  /// <summary>
  /// "Disegna" il gioco nella console da zero.
  /// </summary>
  internal void Draw() {
    Console.Clear();

    legend.SetCanUndo(actionsManager.CanUndo());

    renderer.DrawDeck();
    renderer.DrawFoundations();
    renderer.DrawTableau();
    renderer.DrawSelection(true);
    if (hintManager.ShowingHint && hintManager.LastAction != null) renderer.DrawAction(hintManager.LastAction, false);

    renderer.DrawCursor();
    renderer.DrawLegend();
    renderer.DrawStats();
  }

  /// <summary>
  /// Ridisegna le aree modificate dopo una azione.
  /// </summary>
  /// <param name="action"></param>
  internal void DrawDirtyAreas(IAction action) {
    // Pescata carta dal mazzo
    if (action is DrawCardAction) {
      renderer.DrawDeck();
      return;
    }
    else if (action is MoveCardsAction movAction) {
      renderer.DrawBasedOnArea(movAction.sourceArea);
      renderer.DrawBasedOnArea(movAction.destArea);
    }

    renderer.DrawLegend();
    renderer.DrawStats();
    renderer.DrawCursor();
  }
  #endregion

  #region Private methods

  private bool HasWon() {
    return foundation.GetPile(0).Count == 13 &&
       foundation.GetPile(1).Count == 13 &&
       foundation.GetPile(2).Count == 13 &&
       foundation.GetPile(3).Count == 13;
  }

  #endregion
}

