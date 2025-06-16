using System.Text.RegularExpressions;

namespace Solitario.Utils;
internal static class Pencil {
  internal static readonly Regex AnsiRegex = new(@"\u001b\[[;\d]*m", RegexOptions.Compiled);

  internal static void DrawCentered(string text, int top) {
    string[] textLines = text.Split([Environment.NewLine], StringSplitOptions.None);

    for (int i = 0; i < textLines.Length; i++) {
      var noAnsiLine = AnsiRegex.Replace(textLines[i], "");
      var startPos = Math.Abs((Console.WindowWidth - noAnsiLine.Length) / 2);
      Console.SetCursorPosition(startPos, i + top);
      Console.Write(textLines[i]);
    }
  }

  internal static void DrawArt(string art, int left, int top) {
    string[] artLines = art.Split(Environment.NewLine);

    for (int i = 0; i < artLines.Length; i++) {
      Console.SetCursorPosition(left, i + top);
      Console.Write(artLines[i]);
    }
  }
}
