﻿using Solitario.Game.Managers;
using Solitario.Game.Models.Actions;
using Solitario.Game.Rendering;
using static Solitario.Game.Game;

namespace Solitario.Game.Controllers;
internal class InputHandler {
  private readonly Game game;
  private readonly Cursor cursor;
  private readonly Renderer renderer;
  private readonly Selection selection;
  private readonly Legend legend;
  private readonly Hint hintManager;
  private readonly Stats statsManager;

  private readonly GameController controller;

  internal InputHandler(Game game, Renderer renderer, GameManagers managers) {
    controller = new GameController(managers);

    this.game = game;
    cursor = managers.Cursor;
    this.renderer = renderer;
    selection = managers.Selection;
    legend = managers.Legend;
    hintManager = managers.HintManager;
    statsManager = managers.StatsManager;
  }

  internal void ProcessInput(ConsoleKeyInfo keyInfo) {
    bool changedHintState = false;

    switch (keyInfo.Key) {
      case ConsoleKey.Escape:
        game.OnEsc?.Invoke();
        return;

      case ConsoleKey.UpArrow:
      case ConsoleKey.DownArrow:
      case ConsoleKey.LeftArrow:
      case ConsoleKey.RightArrow:
        HandleCursorMovement(keyInfo.Key);
        break;

      case ConsoleKey.R:
        controller.AttempDrawCard();

        //renderer.DrawDeck();
        //renderer.DrawStats();
        break;

      case ConsoleKey.E:
        controller.AttemptWasteSelection();
        renderer.DrawSelection();
        renderer.DrawLegend();
        break;

      case ConsoleKey.Enter:
      case ConsoleKey.Spacebar:
        controller.HandleSelection();

        legend.UpdateFoundationShortcut(selection);
        renderer.DrawLegend();
        /*renderer.DrawBasedOnArea(selection.SourceArea);
        renderer.DrawCursor();*/
        break;

      case ConsoleKey.X:
        if (controller.ClearSelection()) {

          renderer.DrawBasedOnArea(selection.SourceArea);
          renderer.DrawCursor();
          renderer.DrawLegend();
        }
        break;

      case ConsoleKey.F:
        controller.AttemptCardToFoundation();
        break;

      case ConsoleKey.Z:
        controller.UndoLastAction();
        break;

      case ConsoleKey.H:
        changedHintState = controller.RequestHint();
        renderer.DrawLegend();
        break;
    }

    if (hintManager.ShowingHint && !changedHintState) {
      hintManager.ShowingHint = false;

      if (hintManager.LastAction is MoveCardsAction action) {
        renderer.DrawBasedOnArea(action.sourceArea);
        renderer.DrawBasedOnArea(action.destArea);
      }
      else if (hintManager.LastAction is DrawCardAction) {
        renderer.DrawDeck();
      }

      renderer.DrawCursor();
      if (selection.Active) renderer.DrawSelection();
      renderer.DrawStats();
      renderer.DrawLegend();
    }

    //legend.CanShortCutFoundation = selection.Active || selection.Active && selection.SourceArea == Areas.Deck;
    //if (selection.Active && selection.SourceArea == Areas.Deck) legend.CanShortCutFoundation = true;
    //else if (cursor.CurrentArea == Areas.Tableau && !selection.Active) legend.CanShortCutFoundation = true;
    //else legend.CanShortCutFoundation = false;
    //renderer.DrawLegend();
  }

  /// <summary>
  /// Gestisce il movimento del cursore in base ai tasti premuti.
  /// </summary>
  /// <param name="key"></param>
  private void HandleCursorMovement(ConsoleKey key) {
    switch (key) {
      case ConsoleKey.UpArrow:
        cursor.MoveUp();
        break;
      case ConsoleKey.DownArrow:
        cursor.MoveDown();
        break;
      case ConsoleKey.LeftArrow:
        cursor.MoveLeft();
        break;
      case ConsoleKey.RightArrow:
        cursor.MoveRight();
        break;
    }
    renderer.DrawCursor();
  }
}
