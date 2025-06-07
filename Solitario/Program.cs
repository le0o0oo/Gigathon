namespace Solitario {
  internal class Program {
    static void Main(string[] args) {
      Console.OutputEncoding = System.Text.Encoding.UTF8;
      Console.CursorVisible = false;

      Game.Game game = new Game.Game();
    }
  }
}
