namespace Solitario.Game.Models;

/// <summary>
/// Rappresenta un punto nella console
/// </summary>
/// <param name="x">Posizione X (distanza a partire da sinistra)</param>
/// <param name="y">Posizione Y (disyanza a partire da destra)</param>
internal struct ConsolePoint(int x, int y) {
  internal int X = x;
  internal int Y = y;
}
