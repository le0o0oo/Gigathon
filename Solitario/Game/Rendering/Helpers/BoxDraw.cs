using Solitario.Utils;

namespace Solitario.Game.Rendering.Helpers;

internal static class BoxDraw {
  internal static int startX = 0;

  /// <summary>
  /// Disegna il bordo superiore di un box
  /// </summary>
  internal static void DrawBoxTop(int width, string? ansiColor = null) {
    startX = Console.CursorLeft;
    ansiColor ??= AnsiColors.Foreground.White;
    Console.WriteLine($"{ansiColor}╔{new string('═', width - 2)}╗{AnsiColors.Reset}");
  }

  /// <summary>
  /// Disegna il bordo inferiore di un box
  /// </summary>
  internal static void DrawBoxBottom(int width, string? ansiColor = null) {
    ansiColor ??= AnsiColors.Foreground.White;

    Console.SetCursorPosition(startX, Console.CursorTop);
    Console.Write($"{ansiColor}╚{new string('═', width - 2)}╝{AnsiColors.Reset}");
  }

  /// <summary>
  /// Disegna una linea di un box
  /// </summary>
  /// <param name="formattedContent">Il testo da visualizzare, può essere anche ANSI</param>
  internal static void DrawBoxLine(string formattedContent, int width, string? ansiColor = null) {
    string plainText = Pencil.AnsiRegex.Replace(formattedContent, string.Empty);
    ansiColor ??= AnsiColors.Foreground.White;

    // Calcola il padding richiesto
    int padding = width - 2 // per '║'
                  - 2 // per gli spazi dal margine sinistor
                  - plainText.Length;

    if (padding < 0) {
      padding = 0;
    }

    Console.SetCursorPosition(startX, Console.CursorTop);
    Console.WriteLine(
        $"{ansiColor}║{AnsiColors.Reset}  {formattedContent}{AnsiColors.Reset}{new string(' ', padding)}{ansiColor}║{AnsiColors.Reset}"
    );
  }
}
