using Solitario.Activities.Components;
using Solitario.Activities.Models;
using Solitario.Utils;

namespace Solitario.Activities.Rendering;
internal static class ComponentRenderer {
  internal static string GetButtonArt(Button button, bool selected) {
    string art;
    if (!selected) {
      art = $@"┌{new string('─', button.Text.Length + 2)}┐
│ {button.Text} │
└{new string('─', button.Text.Length + 2)}┘";
    }
    else {
      art = $@"{AnsiColors.Foreground.BoldGreen}┌{new string('─', button.Text.Length + 2)}┐
│ {AnsiColors.Background.DarkGreen}{AnsiColors.Foreground.Lime}{button.Text}{AnsiColors.Reset} {AnsiColors.Foreground.BoldGreen}│
└{new string('─', button.Text.Length + 2)}┘{AnsiColors.Reset}";
    }

    return art;
  }

  internal static string GetCheckboxArt(Checkbox checkbox, bool selected) {
    string icon = checkbox.Checked ? "🗹" : "☐";

    if (!selected)
      return $"{checkbox.Text} {icon} ";

    return $"{AnsiColors.Background.DarkGreen}{checkbox.Text} {icon} {AnsiColors.Reset}";
  }
}
