using Solitario.Activities.Components;
using Solitario.Activities.Models;
using Solitario.Activities.Rendering;
using Solitario.Utils;

namespace Solitario.Activities.Screens;
internal class MenuActivity : IActivity {
  private readonly ActivityManager _activityManager;
  private readonly List<Button> _buttons;
  private int _selectedIndex = 0;

  private const int startYTitle = 5;
  private const int startYBtns = 10;
  private const int btnsOffset = 3;

  private string titleArt = @"███████╗ ██████╗ ██╗     ██╗████████╗ █████╗ ██████╗ ██╗ ██████╗ 
██╔════╝██╔═══██╗██║     ██║╚══██╔══╝██╔══██╗██╔══██╗██║██╔═══██╗
███████╗██║   ██║██║     ██║   ██║   ███████║██████╔╝██║██║   ██║
╚════██║██║   ██║██║     ██║   ██║   ██╔══██║██╔══██╗██║██║   ██║
███████║╚██████╔╝███████╗██║   ██║   ██║  ██║██║  ██║██║╚██████╔╝
╚══════╝ ╚═════╝ ╚══════╝╚═╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝ ╚═════╝ ";

  public MenuActivity(ActivityManager activityManager) {
    _activityManager = activityManager;

    _buttons = [
      new("New Game", () => _activityManager.Launch(new GameActivity(activityManager))),
      new("Settings", () => _activityManager.Launch(new SettingsActivity(activityManager))),
      new("Restore game", () => Console.Write("restore settings")),

      new("Exit", () => _activityManager.Stop())
    ];
  }

  public void OnEnter() {
    //Draw();
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    switch (keyInfo.Key) {
      case ConsoleKey.Escape:
        Tuple<string, Action>[] btns = [
         new("No", () => {
           _activityManager.CloseModal();
         }),
         new("Sì", () => {
           _activityManager.Stop();
         })
        ];

        var modal = new Modal("Esci", "Sei sicuro di voler uscire?", btns);
        _activityManager.ShowModal(modal);
        break;

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

      case ConsoleKey.Spacebar:
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

  public (int, int) GetMinSize() {
    int height = startYBtns + (_buttons.Count * 3) + 3;
    return (30, height);
  }

  public void DrawTitle() {
    const string titleArtShort = "Solitario";
    bool canDrawTitle = Console.WindowWidth >= titleArt.Split('\n')[0].Length;
    Pencil.DrawCentered(canDrawTitle ? titleArt : titleArtShort, startYTitle);
  }

  public void DrawButtons() {

    for (int i = 0; i < _buttons.Count; i++) {
      bool selected = _selectedIndex == i;
      Pencil.DrawCentered(ComponentRenderer.GetButtonArt(_buttons[i], selected), startYBtns + (btnsOffset * (i + 1)));
    }
  }
}
