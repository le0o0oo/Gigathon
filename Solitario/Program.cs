
using Solitario.Activities;

namespace Solitario;
internal class Program {
  // Utilizzato per evitare race condition tra il thread di resize e il draw() dopo l'interrupt nel loop
  private static readonly object ConsoleLock = new();

  static void Main(string[] args) {
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    Console.CursorVisible = false;
    Console.Title = "Solitario";
    Console.Clear();

    var activityManager = new ActivityManager();

    // Carica il menu principale
    activityManager.Launch(new Activities.Screens.ModeSelector(activityManager));

    #region Resize thread
    var resizeThread = new Thread(() => {
      int lastWidth = Console.WindowWidth;
      int lastHeight = Console.WindowHeight;
      bool adjust = false;

      while (true) {
        int currentWidth = Console.WindowWidth;
        int currentHeight = Console.WindowHeight;

        if (currentWidth != lastWidth || currentHeight != lastHeight) {
          lock (ConsoleLock) {
            Console.Clear();
            activityManager.Draw();
          }

          lastWidth = currentWidth;
          lastHeight = currentHeight;
        }

        Thread.Sleep(250);
      }
    });

    resizeThread.IsBackground = true;
    resizeThread.Start();

    #endregion

    // Main application loop
    while (activityManager.IsRunning) {
      // Blocca fino a prossimo tasto per non far esplodere la cpu
      ConsoleKeyInfo keyInfo = Console.ReadKey(true);

      lock (ConsoleLock) {
        activityManager.HandleInput(keyInfo);
      }

      FlushInputBuffer();
    }

    Console.Clear();
    Console.WriteLine("Grazie per aver giocato!");
    Console.CursorVisible = true;
  }

  private static void FlushInputBuffer() {
    while (Console.KeyAvailable) {
      Console.ReadKey(intercept: true);
    }
  }
}
