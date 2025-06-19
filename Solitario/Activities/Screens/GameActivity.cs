using Solitario.Activities.Components;

namespace Solitario.Activities.Screens;
internal class GameActivity : IActivity {
  private readonly Game.Game game;
  private readonly ActivityManager _activityManager;

  internal GameActivity(ActivityManager activityManager) {
    _activityManager = activityManager;

    game = new Game.Game();

    game.OnWin = () => HandleWin();
  }

  public void OnEnter() {
    Draw();
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    game.Input(keyInfo);
  }

  public void Draw() {
    game.Draw();
  }

  private void HandleWin() {
    var modal = new Modal("Congratulazioni", "Hai vinto!");
    modal.OnClose = () => {
      _activityManager.HideModal();
      _activityManager.Back();
    };

    _activityManager.ShowModal(modal);
  }
}
