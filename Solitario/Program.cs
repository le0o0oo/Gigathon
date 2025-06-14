
using Solitario.Activities;

namespace Solitario;
internal class Program {

  static void Main(string[] args) {
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    Console.CursorVisible = false;
    Console.Title = "Solitario";

    //var settings = Settings.Settings.Load();

    var activityManager = new ActivityManager();

    // Carica il menu principale
    activityManager.SwitchTo(new Activities.Screens.MenuActivity(activityManager));

    var resizeThread = new Thread(() => {
      int lastWidth = Console.WindowWidth;
      int lastHeight = Console.WindowHeight;

      while (true) {
        int currentWidth = Console.WindowWidth;
        int currentHeight = Console.WindowHeight;

        if (currentWidth != lastWidth || currentHeight != lastHeight) {
          Console.Clear();
          activityManager.Draw();

          lastWidth = currentWidth;
          lastHeight = currentHeight;
        }

        Thread.Sleep(500);
      }
    });

    resizeThread.IsBackground = true;
    resizeThread.Start();

    // Main application loop
    while (activityManager.IsRunning) {
      ConsoleKeyInfo keyInfo = Console.ReadKey(true);
      activityManager.HandleInput(keyInfo);
      /*if (Console.KeyAvailable) {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        activityManager.HandleInput(keyInfo);
      } */

      // ~ 60 volte al secondo
      //Thread.Sleep(16);
    }

    Console.Clear();
    Console.WriteLine("Grazie per aver giocato!");
    Console.CursorVisible = true;
  }
}
