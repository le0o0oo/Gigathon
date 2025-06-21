using Solitario.Activities.Components;
using Solitario.Activities.Models;
using Solitario.Utils;

namespace Solitario.Activities.Rendering;
internal static class ComponentRenderer {
  /// <summary>
  /// Restituisce una stringa contenente la grafica di un bottone
  /// </summary>
  /// <param name="button">Bottone</param>
  /// <param name="selected">Se deve essere restituita una versione in cui il bottone viene selezionato o no</param>
  /// <returns></returns>
  internal static string GetButtonArt(Button button, bool selected) {
    string art;
    if (!selected) {
      var btnText = $" {button.Text} ";
      art = $@"┌{new string('─', btnText.Length + 2)}┐
│ {btnText} │
└{new string('─', btnText.Length + 2)}┘";
    }
    else {
      var selectedBtnText = $"<{button.Text}>";
      art = $@"{AnsiColors.Foreground.BoldGreen}╔{new string('═', selectedBtnText.Length + 2)}╗
║ {AnsiColors.Background.DarkGreen}{AnsiColors.Foreground.Lime}{selectedBtnText}{AnsiColors.Reset} {AnsiColors.Foreground.BoldGreen}║
╚{new string('═', selectedBtnText.Length + 2)}╝{AnsiColors.Reset}";
    }

    return art;
  }

  /// <summary>
  /// Restituisce una stringa contenente la grafica della checkbox
  /// </summary>
  /// <param name="checkbox">L'oggetto della checkbox</param>
  /// <param name="selected">Se deve essere restituita una versione in cui la checkbox viene selezionato o no</param>
  /// <returns></returns>
  internal static string GetCheckboxArt(Checkbox checkbox, bool selected) {
    string icon = checkbox.Checked ? "🗹" : "☐";

    if (!selected)
      return $"  {checkbox.Text} {icon} ";

    return $"{AnsiColors.Background.DarkGreen}> {checkbox.Text} {icon} {AnsiColors.Reset}";
  }
}
