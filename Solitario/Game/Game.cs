using Solitario.Game.Managers;
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

  internal record GameManagers(Deck Deck, Tableau Tableau, Foundation Foundation, Selection Selection, Cursor Cursor);

  internal Action? OnWin;
  internal Action? OnEsc;

  internal Game(Deck? deck = null, Tableau? tableau = null, Foundation? foundation = null, Stats? scoreManager = null) {
    deck ??= new Deck();
    tableau ??= new Tableau(deck);
    foundation ??= new Foundation();
    scoreManager ??= new Stats(tableau);

    this.deck = deck;
    this.tableau = tableau;
    this.foundation = foundation;
    this.statsManager = scoreManager;

    legend = new Legend();
    selection = new Selection();
    cursor = new Cursor(tableau);
    hintManager = new Managers.Hint();
    actionsManager = new Actions();

    renderer = new Renderer(deck, tableau, foundation, cursor, legend, selection, hintManager, scoreManager);
    inputHandler = new InputHandler(this, cursor, renderer, selection, legend, deck, tableau, foundation, actionsManager, hintManager, scoreManager);
  }

  #region Public methods
  /// <summary>
  /// Gestisce input utente
  /// </summary>
  /// <param name="keyInfo"></param>
  internal void Input(ConsoleKeyInfo keyInfo) {
    inputHandler.ProcessInput(keyInfo);

    renderer.DrawLegend();

    if (HasWon()) {
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

    renderer.DrawCursor();
    renderer.DrawLegend();
    renderer.DrawStats();
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

