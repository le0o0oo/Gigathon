using Solitario.Activities.Components;

namespace Solitario.Activities;

internal class ActivityManager {
  private IActivity? _currentActivity; // Attività attuale
  public bool IsRunning { get; private set; } = true;

  private readonly List<IActivity> _activities = []; // Lista delle attività
  private Modal? currentModal; // Finestra modale attualmente aperta (se presente)

  /// <summary>
  /// Avvia una attività
  /// </summary>
  /// <param name="newActivity">Attività da avviare</param>
  public void Launch(IActivity newActivity) {
    _currentActivity = newActivity;
    _activities.Add(newActivity);
    _currentActivity.OnEnter();

    Console.Clear();
    Draw();
  }

  /// <summary>
  /// Mostra una finestra modale
  /// </summary>
  /// <param name="modal">L'oggetto modale</param>
  /// <exception cref="Exception">Triggerata se un'altra finestra modale è aperta</exception>
  public void ShowModal(Modal modal) {
    if (currentModal != null) throw new Exception("Another modal is currently active");

    this.currentModal = modal;
    currentModal.OnClose ??= () => CloseModal();

    currentModal.Draw();
  }

  /// <summary>
  /// Chiude la finestra modale attuale
  /// </summary>
  public void CloseModal() {
    if (currentModal == null) return;

    this.currentModal = null;

    _currentActivity?.Draw();
  }

  /// <summary>
  /// Torna alla attività precedente
  /// </summary>
  public void Back() {
    if (_activities.Count < 2) return;

    _activities.RemoveAt(_activities.Count - 1);
    _currentActivity = _activities[^1];
    _currentActivity.OnEnter();

    Draw();
  }

  /// <summary>
  /// Inoltra le informazioni sul tasto premuto alla attività corrente
  /// </summary>
  /// <param name="keyInfo"></param>
  public void HandleInput(ConsoleKeyInfo keyInfo) {
    if (!CanDraw()) return;

    if (currentModal != null) {
      currentModal.HandleInput(keyInfo);
      return;
    }

    _currentActivity?.HandleInput(keyInfo);
  }

  /// <summary>
  /// Helper private per determinare se la attività attuale può essere disegnata o no
  /// </summary>
  /// <returns></returns>
  private bool CanDraw() {
    if (_currentActivity == null) return false;

    (int minW, int minH) = _currentActivity.GetMinSize();
    bool canDraw = Console.WindowWidth >= minW && Console.WindowHeight >= minH;

    return canDraw;
  }

  /// <summary>
  /// Disegna la attività attuale.
  /// Se il valore restituito da CanDraw() è false, verrà mostrato un messaggio di fallback
  /// </summary>
  public void Draw() {
    if (_currentActivity == null) return;

    (int minW, int minH) = _currentActivity.GetMinSize();

    if (!CanDraw()) {
      Console.Clear();
      Console.SetCursorPosition(0, 0);
      Console.WriteLine($"Per favore imposta la dimensione della tua console a {minW}x{minH}");
      Console.WriteLine($"Dimensione attuale: {Console.WindowWidth}x{Console.WindowHeight}");
      return;
    }

    _currentActivity?.Draw();

    if (currentModal != null) currentModal.Draw();
  }

  public void Stop() {
    IsRunning = false;
  }
}