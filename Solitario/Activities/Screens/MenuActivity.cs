using Solitario.Activities.Models;
using Solitario.Activities.Rendering;
using Solitario.Utils;

namespace Solitario.Activities.Screens;
internal class MenuActivity : IActivity {
  private readonly ActivityManager _activityManager;
  private readonly List<Button> _buttons;
  private int _selectedIndex = 0;

  private string titleArt = @"███████╗ ██████╗ ██╗     ██╗████████╗ █████╗ ██████╗ ██╗ ██████╗ 
██╔════╝██╔═══██╗██║     ██║╚══██╔══╝██╔══██╗██╔══██╗██║██╔═══██╗
███████╗██║   ██║██║     ██║   ██║   ███████║██████╔╝██║██║   ██║
╚════██║██║   ██║██║     ██║   ██║   ██╔══██║██╔══██╗██║██║   ██║
███████║╚██████╔╝███████╗██║   ██║   ██║  ██║██║  ██║██║╚██████╔╝
╚══════╝ ╚═════╝ ╚══════╝╚═╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝ ╚═════╝ ";

  public MenuActivity(ActivityManager activityManager) {
    _activityManager = activityManager;

    _buttons = [
      new("New Game", () => _activityManager.Launch(new GameActivity())),
      new("Settings", () => _activityManager.Launch(new SettingsActivity(activityManager))),
      new("Restore game", () => Console.Write("restore settings")),

      new("Exit", () => _activityManager.Stop())
    ];
  }

  public void OnEnter() {
    Draw();
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    switch (keyInfo.Key) {
      case ConsoleKey.UpArrow:
        if (_selectedIndex <= 0) break;
        _selectedIndex--;
        DrawButtons();
        break;
      case ConsoleKey.DownArrow:
        if (_selectedIndex >= _buttons.Count - 1) break;
        _selectedIndex++;
        DrawButtons();
        break;
      case ConsoleKey.Enter:
        _buttons[_selectedIndex].OnClick();
        break;
    }
  }


  public void Draw() {
    Console.Clear();
    DrawTitle();
    DrawButtons();
  }

  public void DrawTitle() {
    const int startY = 5;
    const string titleArtShort = "Solitario";
    bool canDrawTitle = Console.WindowWidth >= titleArt.Split('\n')[0].Length;
    Pencil.DrawCentered(canDrawTitle ? titleArt : titleArtShort, startY);
  }

  public void DrawButtons() {
    const int startY = 10;
    const int offset = 3;

    for (int i = 0; i < _buttons.Count; i++) {
      bool selected = _selectedIndex == i;
      Pencil.DrawCentered(ComponentRenderer.GetButtonArt(_buttons[i], selected), startY + (offset * (i + 1)));
    }
  }
}
