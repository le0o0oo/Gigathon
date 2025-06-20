using Solitario.Activities.Models;
using Solitario.Activities.Rendering;
using Solitario.Utils;

namespace Solitario.Activities.Screens;
internal class ModeSelector : IActivity {
  private readonly ActivityManager _activityManager;

  Button[] components = {
    new("Normale", () => {
      CurrentSettings.UseAnsi = true;
    }),
    new("No ANSI", () => {
      CurrentSettings.UseAnsi = false;
    })
  };
  int currentSelection = 0;

  internal ModeSelector(ActivityManager activityManager) {
    this._activityManager = activityManager;
  }

  public void OnEnter() {
    Draw();
  }

  public void Draw() {
    Pencil.DrawCentered("Seleziona modalità di visualizzazione", 10);

    DrawComponents();
  }

  public (int, int) GetMinSize() {
    return (1, 2);
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    switch (keyInfo.Key) {
      case ConsoleKey.UpArrow:
        if (currentSelection == 0) break;
        currentSelection--;

        DrawComponents();
        break;

      case ConsoleKey.DownArrow:
        if (currentSelection > components.Length - 1) break;
        currentSelection++;

        DrawComponents();
        break;

      case ConsoleKey.Spacebar:
      case ConsoleKey.Enter:
        components[currentSelection].OnClick.Invoke();
        AnsiColors.UpdateSettings();
        _activityManager.Launch(new MenuActivity(_activityManager));
        break;
    }
  }

  private void DrawComponents() {
    for (int i = 0; i < components.Length; i++) {
      Pencil.DrawCentered(ComponentRenderer.GetButtonArt(components[i], currentSelection == i), 12 + (i * 3));
    }
  }
}
