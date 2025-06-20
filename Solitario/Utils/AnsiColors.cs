namespace Solitario.Utils;
internal static class AnsiColors {
  private static bool enabled = CurrentSettings.UseAnsi;

  internal static string Reset => enabled ? "\u001b[0m" : "";

  internal static class Foreground {
    // Regular
    internal static string Black => enabled ? "\u001b[0;30m" : "";
    internal static string Red => enabled ? "\u001b[0;31m" : "";
    internal static string Green => enabled ? "\u001b[0;32m" : "";
    internal static string Yellow => enabled ? "\u001b[0;33m" : "";
    internal static string Blue => enabled ? "\u001b[0;34m" : "";
    internal static string Magenta => enabled ? "\u001b[0;35m" : "";
    internal static string Cyan => enabled ? "\u001b[0;36m" : "";
    internal static string White => enabled ? "\u001b[0;37m" : "";
    internal static string Lime => enabled ? "\u001b[38;5;15m" : "";

    // Bold
    internal static string BoldBlue => enabled ? "\u001b[1;34m" : "";
    internal static string BoldGreen => enabled ? "\u001b[1;32m" : "";
    internal static string BoldYellow => enabled ? "\u001b[1;33m" : "";
    internal static string BoldCyan => enabled ? "\u001b[1;36m" : "";

    internal static string DarkGray => enabled ? "\u001b[1;30m" : "";
  }

  internal static class Background {
    internal static string White => enabled ? "\u001b[48;5;255m" : "";
    internal static string DarkGreen => enabled ? "\u001b[48;5;22m" : "";
  }

  internal static void UpdateSettings() {
    enabled = CurrentSettings.UseAnsi;
  }
}