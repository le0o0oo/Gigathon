namespace Solitario.Activities;

internal class ActivityManager {
  private IActivity? _currentActivity;
  public bool IsRunning { get; private set; } = true;

  public void SwitchTo(IActivity newActivity) {
    _currentActivity = newActivity;
    _currentActivity.OnEnter();
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    _currentActivity?.HandleInput(keyInfo);
  }

  public void Draw() {
    _currentActivity?.Draw();
  }

  public void Stop() {
    IsRunning = false;
  }
}