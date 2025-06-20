using Solitario.Activities.Components;

namespace Solitario.Activities;

internal class ActivityManager {
  private IActivity? _currentActivity;
  public bool IsRunning { get; private set; } = true;

  private List<IActivity> _activities = [];
  private Modal? currentModal;

  public void Launch(IActivity newActivity) {
    _currentActivity = newActivity;
    _activities.Add(newActivity);
    _currentActivity.OnEnter();
  }

  public void ShowModal(Modal modal) {
    if (currentModal != null) throw new Exception("Another modal is currently active");

    this.currentModal = modal;
    if (currentModal.OnClose == null) currentModal.OnClose = () => CloseModal();

    currentModal.Draw();
  }

  public void CloseModal() {
    if (currentModal == null) return;

    this.currentModal = null;

    _currentActivity?.Draw();
  }

  public void Back() {
    if (_activities.Count < 2) return;

    _activities.RemoveAt(_activities.Count - 1);
    _currentActivity = _activities[^1];
    _currentActivity.OnEnter();
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    if (!CanDraw()) return;

    if (currentModal != null) {
      currentModal.HandleInput(keyInfo);
      return;
    }

    _currentActivity?.HandleInput(keyInfo);
  }

  private bool CanDraw() {
    if (_currentActivity == null) return false;

    (int minW, int minH) = _currentActivity.GetMinSize();
    bool canDraw = Console.WindowWidth >= minW && Console.WindowHeight >= minH;

    return canDraw;
  }

  public void Draw() {
    if (_currentActivity == null) return;

    (int minW, int minH) = _currentActivity.GetMinSize();

    if (!CanDraw()) {
      Console.SetCursorPosition(0, 0);
      Console.WriteLine($"Please resize your console window to at least {minW}x{minH}");
      Console.WriteLine($"Current size: {Console.WindowWidth}x{Console.WindowHeight}");
      return;
    }

    _currentActivity?.Draw();

    if (currentModal != null) currentModal.Draw();
  }

  public void Stop() {
    IsRunning = false;
  }
}