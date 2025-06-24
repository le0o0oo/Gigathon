using Solitario.Activities.Models;
using Solitario.Activities.Rendering;
using Solitario.Utils;

namespace Solitario.Activities.Screens;
internal class ModeSelector : IActivity {
  private readonly ActivityManager _activityManager;

  private static readonly Button[] components = {
    new("Normale", () => {
      CurrentSettings.UseAnsi = true;
    }),
    new("No ANSI", () => {
      CurrentSettings.UseAnsi = false;
    })
  };
  private static readonly string[] modesDescription = {
    "Abilita i colori per un'esperienza visiva più ricca. Consigliata per la maggior parte dei terminali.",
    "Disabilita i colori. Scegli questa modalità se visualizzi caratteri strani o illeggibili."
  };
  int currentSelection = 0;

  internal ModeSelector(ActivityManager activityManager) {
    this._activityManager = activityManager;
  }

  public void OnEnter() {
    //Draw();
  }

  public void Draw() {
    Console.Clear();

    Pencil.DrawCentered("Seleziona modalità di visualizzazione", 9);
    Pencil.DrawCentered("Usa le freccie per selezionare", 10);

    DrawComponents();
  }

  public (int, int) GetMinSize() {
    return (10, 23);
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    switch (keyInfo.Key) {
      case ConsoleKey.UpArrow:
        if (currentSelection == 0) break;
        currentSelection--;

        DrawComponents();
        break;

      case ConsoleKey.DownArrow:
        if (currentSelection >= components.Length - 1) break;
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
    DrawInfos();
  }

  private void DrawInfos() {
    string blankLine = new string(' ', currentSelection == 0 ? modesDescription[1].Length : modesDescription[0].Length);
    Pencil.DrawCentered(blankLine, 20);

    Pencil.DrawCentered(modesDescription[currentSelection], 20);
  }
}
