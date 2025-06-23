namespace Solitario.Utils;

internal static class ConsoleBuffer {
  private static ConsoleBufferChar[,] matrix = new ConsoleBufferChar[0, 0];

  internal static void UpdateMatrixSize() {
    matrix = new ConsoleBufferChar[Console.WindowHeight, Console.WindowWidth];
  }
}
