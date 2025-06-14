using Solitario.Activities.Components;
using Solitario.Activities.Models;
using Solitario.Activities.Rendering;
using Solitario.Utils;

namespace Solitario.Activities.Screens;
internal class SettingsActivity : IActivity {
  private BaseComponent[] _elements = new BaseComponent[2];
  private int _selectedIndex = 1;

  internal SettingsActivity() {
    var btn = new Button("◄ Indietro", () => { });
    var checkbox = new Checkbox("test", null);

    checkbox.OnClick = () => {
      checkbox.Toggle();
      DrawUI();
    };

    _elements[0] = btn;
    _elements[1] = checkbox;
  }

  public void OnEnter() {
    Draw();
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    switch (keyInfo.Key) {
      case ConsoleKey.UpArrow:
        if (_selectedIndex <= 0) break;
        _selectedIndex--;
        DrawUI();
        break;
      case ConsoleKey.DownArrow:
        if (_selectedIndex >= _elements.Length - 1) break;
        _selectedIndex++;
        DrawUI();
        break;
      case ConsoleKey.Enter:
        _elements[_selectedIndex].OnClick();
        break;
    }
  }

  public void Draw() {
    Console.Clear();


    DrawUI();

  }

  private void DrawUI() {
    const int startY = 5;
    const int spacing = 1;

    Pencil.DrawArt(ComponentRenderer.GetButtonArt((Button)_elements[0], _selectedIndex == 0), 1, 0);

    for (int i = 1; i < _elements.Length; i++) {
      var el = _elements[i];
      Pencil.DrawCentered(ComponentRenderer.GetCheckboxArt((Checkbox)el, _selectedIndex == i), startY + (i * spacing));
    }
  }
}
