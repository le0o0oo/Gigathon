using Solitario.Activities.Components;
using Solitario.Game.Helpers;
using Solitario.Game.Rendering;
using Solitario.Utils;

namespace Solitario.Activities.Screens;

/// <summary>
/// Una specie di "wrapper" del gioco che gestisce i modal e inoltra gli input
/// </summary>
internal class GameActivity : IActivity {
  private Game.Game game;
  private readonly ActivityManager _activityManager;

  internal GameActivity(ActivityManager activityManager, Game.Game? initialGame = null) {
    _activityManager = activityManager;

    if (initialGame == null) {
      game = new Game.Game();
    }
    else {
      game = initialGame;
    }
    AttachActions();
  }

  public void OnEnter() {
    //Draw();
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    game.Input(keyInfo);
  }

  public void Draw() {
    game.Draw();
  }

  public (int, int) GetMinSize() {
    return (Renderer.minWidth, Renderer.minHeight);
  }

  /// <summary>
  /// Collega i gestori di eventi alle azioni del gioco per eventi specifici.
  /// </summary>
  private void AttachActions() {
    game.OnWin = () => HandleWin();
    game.OnEsc = () => EscMenu();
  }

  private void HandleWin() {
    Tuple<string, Action>[] btns = [
      new("OK", () => {
        _activityManager.CloseModal();
        _activityManager.Back();
      })
    ];
    var modal = new Modal("Congratulazioni", "Hai vinto!", btns);

    modal.OnClose = () => {
      _activityManager.CloseModal();
      _activityManager.Back();
    };

    _activityManager.ShowModal(modal);
  }

  private void EscMenu() {
    Tuple<string, Action>[] btns = [
      new("Chiudi", () => {
        _activityManager.CloseModal();
      }),
      new("Salva partita", () => {
        var serializer = new Serializer(game.deck, game.foundation, game.tableau);
        serializer.SaveAsFile(Config.SaveFilename);

        var feedbackModal = new Modal("Salva", "Partita salvata con successo", [new("OK", () => _activityManager.CloseModal())]);

        _activityManager.CloseModal();
        _activityManager.ShowModal(feedbackModal);
      }),
      new("Rigioca", () => {
        game = new Game.Game();
        AttachActions();
        Draw();

        //GC.Collect();
        _activityManager.CloseModal();
      }),
      new("Menu principale", () => {
        _activityManager.CloseModal();
        _activityManager.Back();
      })
    ];

    var modal = new Modal("Menu", $"Scegli una azione... o fai una pausa\nPremi {AnsiColors.Foreground.BoldYellow}(Esc){AnsiColors.Reset} per chiudere il menu", btns);

    _activityManager.ShowModal(modal);
  }
}
