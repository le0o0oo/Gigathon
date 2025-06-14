namespace Solitario.Activities.Screens;
internal class GameActivity : IActivity {
  private readonly Game.Game game;

  internal GameActivity() {
    game = new Game.Game();
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
}
