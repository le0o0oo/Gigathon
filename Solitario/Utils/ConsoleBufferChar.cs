namespace Solitario.Utils;

internal class ConsoleBufferChar {
  internal char? Character;
  internal ConsoleColor ForegroundColor;
  internal ConsoleColor BackgroundColor;

  internal ConsoleBufferChar(char? character, ConsoleColor foregroundColor, ConsoleColor backgroundColor) {
    Character = character;
    ForegroundColor = foregroundColor;
    BackgroundColor = backgroundColor;
  }
}
