namespace Solitario.Utils;
internal static class AnsiColors {
  internal const string Reset = "\u001b[0m";

  internal static class Foreground {
    // Regular
    internal const string Black = "\u001b[0;30m";
    internal const string Red = "\u001b[0;31m";
    internal const string Green = "\u001b[0;32m";
    internal const string Yellow = "\u001b[0;33m";
    internal const string Blue = "\u001b[0;34m";
    internal const string Magenta = "\u001b[0;35m";
    internal const string Cyan = "\u001b[0;36m";
    internal const string White = "\u001b[0;37m";
    internal const string Lime = "\u001b[38;5;15m";

    // Bold
    internal const string BoldBlue = "\u001b[1;34m";
    internal const string BoldGreen = "\u001b[1;32m";
    internal const string BoldYellow = "\u001b[1;33m";
    internal const string BoldCyan = "\u001b[1;36m";

    internal const string DarkGray = "\u001b[1;30m";
  }

  internal static class Background {
    internal const string DarkGreen = "\u001b[48;5;22m";
  }
}