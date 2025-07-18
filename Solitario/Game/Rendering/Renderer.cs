﻿using Solitario.Game.Data;
using Solitario.Game.Managers;
using Solitario.Game.Models.Actions;
using Solitario.Game.Rendering.Components;
using Solitario.Game.Rendering.Helpers;

namespace Solitario.Game.Rendering;
internal class Renderer {
  #region Classi di stato
  private readonly Deck deck;
  private readonly Tableau tableau;
  private readonly Foundation foundation;
  private readonly Cursor cursor;
  private readonly Legend legend;
  private readonly Selection selection;
  private readonly Managers.Hint hintManager;
  private readonly Stats statsManager;

  private readonly BoardRenderer boardRenderer;
  private readonly UIRenderer uiRenderer;
  private readonly ActionRenderer actionRenderer;
  #endregion

  #region Costanti
  internal static readonly int tableauHeight = CardArt.cardHeight + 19;

  internal static readonly int legendWidth = 50;
  internal static readonly int legenStartX = 0;
  internal static readonly int legendStartY = CardArt.cardHeight + 3 + (tableauHeight - 6);

  internal static readonly int statsBoxWidth = 25;
  internal static readonly int statsBoxStartX = CardArt.cardWidth * 7;
  internal static readonly int statsBoxStartY = 1;

  internal static readonly ConsoleColor color = ConsoleColor.DarkGreen;
  internal static readonly char cursorChar = CurrentSettings.UseAnsi ? '❮' : '<';

  internal static readonly int minWidth = statsBoxStartX + statsBoxWidth;
  internal static readonly int minHeight = 1 + CardArt.cardHeight + 1 + tableauHeight + 7;
  #endregion

  #region Variabili di stato
  private static int prevLeft, prevTop = 0;  // Posizione precedente del cursore
  #endregion

  #region Private helpers
  /// <summary>
  /// Salva la posizione del cursore della console
  /// </summary>
  protected internal static void SaveCursorPosition() {
    prevLeft = Console.CursorLeft;
    prevTop = Console.CursorTop;
  }
  /// <summary>
  /// Ripristina la posizione del cursore della console
  /// </summary>
  protected internal static void RestoreCursorPosition() {
    Console.ResetColor();
    Console.SetCursorPosition(prevLeft, prevTop);
  }
  #endregion

  #region Public helpers
  /// <summary>
  /// Disegna una specifica area
  /// </summary>
  /// <param name="area">Area da idisegnare</param>
  internal void DrawBasedOnArea(Areas area) {
    switch (area) {
      case Areas.Tableau:
        DrawTableau();
        break;
      case Areas.Foundation:
        DrawFoundations();
        break;
      case Areas.Deck:
        DrawDeck();
        break;
    }
  }

  /// <summary>
  /// Ridisegna le aree modificate dopo una azione.
  /// </summary>
  /// <param name="action"></param>
  internal void DrawDirtyAreas(IAction action) {
    // Pescata carta dal mazzo
    if (action is DrawCardAction) {
      DrawDeck();
      return;
    }
    else if (action is MoveCardsAction movAction) {
      // Ottimizza il ridisegno delle aree (solo per tableau)
      if (movAction.sourceArea == Areas.Tableau) DrawTableauPile(movAction.sourceIndex, clearRectangle: true);
      else DrawBasedOnArea(movAction.sourceArea);

      if (movAction.destArea == Areas.Tableau) DrawTableauPile(movAction.destIndex, true);
      else DrawBasedOnArea(movAction.destArea);
    }

    DrawLegend();
    DrawStats();
    DrawCursor();
  }
  #endregion

  internal Renderer(Game.GameManagers managers) {
    this.deck = managers.Deck;
    this.tableau = managers.Tableau;
    this.foundation = managers.Foundation;
    this.cursor = managers.Cursor;
    this.legend = managers.Legend;
    this.selection = managers.Selection;
    this.hintManager = managers.HintManager;
    this.statsManager = managers.StatsManager;

    // Create the specialized renderers, giving them only what they need.
    this.boardRenderer = new BoardRenderer(deck, tableau, foundation);
    this.uiRenderer = new UIRenderer(cursor, selection, legend, hintManager, statsManager);
    this.actionRenderer = new ActionRenderer(deck, tableau, foundation);
  }

  #region Methods
  internal void DrawDeck() => boardRenderer.DrawDeck();
  internal void DrawTableau() => boardRenderer.DrawTableau();
  internal void DrawFoundations() => boardRenderer.DrawFoundations();

  internal void DrawCursor() => uiRenderer.DrawCursor();
  internal void DrawSelection(bool redraw = false) => uiRenderer.DrawSelection(redraw);
  internal void DrawLegend() => uiRenderer.DrawLegend();
  internal void DrawAction(IAction action, bool animate = true) => actionRenderer.DrawAction(action, animate);
  internal void DrawStats() => uiRenderer.DrawStats();

  #endregion

  #region Specific methods
  internal void DrawTableauPile(int index, bool clearRectangle = false) => boardRenderer.DrawTableauPile(index, clearRectangle);
  #endregion
}
