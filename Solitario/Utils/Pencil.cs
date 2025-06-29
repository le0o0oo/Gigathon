using System.Text.RegularExpressions;

namespace Solitario.Utils;
internal static class Pencil {
  internal static readonly Regex AnsiRegex = new(@"\u001b\[[;\d]*m", RegexOptions.Compiled);
  internal static readonly string[] LineEndings = ["\r\n", "\n", "\r"];

  /// <summary>
  /// Scrive una linea centrata orizzontalmente nella console
  /// </summary>
  /// <param name="text"></param>
  /// <param name="top"></param>
  internal static void DrawCentered(string text, int top) {
    string[] textLines = text.Split(LineEndings, StringSplitOptions.None);

    for (int i = 0; i < textLines.Length; i++) {
      var noAnsiLine = AnsiRegex.Replace(textLines[i], "");
      var startPos = Math.Abs((Console.WindowWidth - noAnsiLine.Length) / 2);
      Console.SetCursorPosition(startPos, i + top);
      Console.Write(textLines[i]);
    }
  }

  /// <summary>
  /// Restituisce una tupla con primo parametro la posizione X iniziale del testo e come secondo la lunghezza
  /// </summary>
  /// <param name="text">Input text</param>
  /// <param name="top">Longest line length</param>
  /// <returns></returns>
  internal static (int, int) GetCenteredStartingPoint(string text, int top) {
    string[] textLines = text.Split(Environment.NewLine, StringSplitOptions.None);
    string noAnsiLine = textLines
          .Select(line => AnsiRegex.Replace(line, ""))
          .OrderByDescending(cleanLine => cleanLine.Length)
          .FirstOrDefault() ?? "";

    return (Math.Abs((Console.WindowWidth - noAnsiLine.Length) / 2), noAnsiLine.Length);
  }

  /// <summary>
  /// Disegna qualsiasi tipo di "arte"
  /// </summary>
  /// <param name="art"></param>
  /// <param name="left"></param>
  /// <param name="top"></param>
  /// <returns>True se il disegno è stato completato, False se non è andato a buon fine</returns>
  internal static bool DrawArt(string art, int left, int top) {
    string[] artLines = art.Split(LineEndings, StringSplitOptions.None);

    for (int i = 0; i < artLines.Length; i++) {
      if (i + top >= Console.WindowHeight) return false;
      Console.SetCursorPosition(left, i + top);
      Console.Write(artLines[i]);
    }

    return true;
  }

  /// <summary>
  /// Cancella un'area specificata
  /// </summary>
  /// <param name="left">Posizione X iniziale</param>
  /// <param name="top">Posizione Y iniziale</param>
  /// <param name="width">Lunghezza del rettangolo</param>
  /// <param name="height">Altezza del rettangolo</param>
  /// <param name="debugChar">Specificare un carattere che può essere utilizzato per debug</param>
  internal static void ClearRectangle(int left, int top, int width, int height, char debugChar = ' ') {
    string blankLine = new(debugChar, width);
    for (int y = top; y < top + height; y++) {
      Console.SetCursorPosition(left, y);
      Console.Write(blankLine);
    }
  }
}
