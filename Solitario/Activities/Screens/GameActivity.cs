using Solitario.Activities.Components;
using Solitario.Game.Rendering;
using Solitario.Utils;

namespace Solitario.Activities.Screens;
internal class GameActivity : IActivity {
  private Game.Game game;
  private readonly ActivityManager _activityManager;

  internal GameActivity(ActivityManager activityManager) {
    _activityManager = activityManager;

    game = new Game.Game();
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
    return (ConsoleRenderer.minWidth, ConsoleRenderer.minHeight);
  }

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
      new("Salva", () => {}),
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
