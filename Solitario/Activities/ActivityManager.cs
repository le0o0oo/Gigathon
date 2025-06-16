namespace Solitario.Activities;

internal class ActivityManager {
  private IActivity? _currentActivity;
  public bool IsRunning { get; private set; } = true;

  private List<IActivity> _activities = [];

  public void Launch(IActivity newActivity) {
    _currentActivity = newActivity;
    _activities.Add(newActivity);
    _currentActivity.OnEnter();
  }

  public void Back() {
    if (_activities.Count < 2) return;

    _activities.RemoveAt(_activities.Count - 1);
    _currentActivity = _activities[^1];
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