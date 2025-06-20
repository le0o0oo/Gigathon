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
    if (currentModal != null) {
      currentModal.HandleInput(keyInfo);
      return;
    }

    _currentActivity?.HandleInput(keyInfo);
  }

  public void Draw() {
    _currentActivity?.Draw();

    if (currentModal != null) currentModal.Draw();
  }

  public void Stop() {
    IsRunning = false;
  }
}