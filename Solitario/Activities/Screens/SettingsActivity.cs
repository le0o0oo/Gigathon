using Solitario.Activities.Components;
using Solitario.Activities.Models;
using Solitario.Activities.Rendering;
using Solitario.Utils;

namespace Solitario.Activities.Screens;
internal class SettingsActivity : IActivity {
  private static readonly InputComponent[] baseComponents = new InputComponent[3];
  private readonly InputComponent[] _elements = baseComponents;
  private readonly ActivityManager _activityManager;

  private int _selectedIndex = 1;

  private readonly Checkbox autoplay_cb, useHints_cb;

  internal SettingsActivity(ActivityManager activityManager) {
    this._activityManager = activityManager;

    var backBtn = new Button("◄ Indietro (Esc)", () => {
      activityManager.Back();
    });
    autoplay_cb = new Checkbox("Autoplay", () => {
      CurrentSettings.Autoplay = autoplay_cb!.Checked;
      DrawUI();
    });
    useHints_cb = new Checkbox("Abilita suggerimenti", () => {
      CurrentSettings.UseHints = useHints_cb!.Checked;
      DrawUI();
    });

    _elements[0] = backBtn;
    _elements[1] = autoplay_cb;
    _elements[2] = useHints_cb;
  }

  public void OnEnter() {
    autoplay_cb.Checked = CurrentSettings.Autoplay;
    useHints_cb.Checked = CurrentSettings.UseHints;

    //Draw();
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    switch (keyInfo.Key) {
      case ConsoleKey.Escape:
        _activityManager.Back();
        break;

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

      case ConsoleKey.Spacebar:
      case ConsoleKey.Enter:
        if (_elements[_selectedIndex] is Checkbox checkbox) {
          checkbox.Toggle();
        }
        _elements[_selectedIndex]?.OnClick();
        break;
    }
  }

  public void Draw() {
    Console.Clear();
    DrawUI();
  }

  public (int, int) GetMinSize() {
    return (2, 3);
  }

  private void DrawUI() {
    const int startY = 5;
    const int spacing = 2;

    Pencil.DrawArt(ComponentRenderer.GetButtonArt((Button)_elements[0], _selectedIndex == 0), 1, 0);

    for (int i = 1; i < _elements.Length; i++) {
      var el = _elements[i];
      if (el != null) {
        Pencil.DrawCentered(ComponentRenderer.GetCheckboxArt((Checkbox)el, _selectedIndex == i), startY + (i * spacing));
      }
    }
  }
}
