using Solitario.Activities.Components;
using Solitario.Activities.Models;
using Solitario.Activities.Rendering;
using Solitario.Game.Helpers;
using Solitario.Game.Managers;
using Solitario.Game.Rendering.Helpers;
using Solitario.Utils;

namespace Solitario.Activities.Screens;
internal class MenuActivity : IActivity {
  private readonly ActivityManager _activityManager;
  private readonly List<Button> _buttons;
  private int _selectedIndex = 0;
  private readonly Random rand = new Random();
  private Deck? deck; // Utilizzato per DrawBackground()

  private const int startYTitle = 5;
  private const int startYBtns = 10;
  private const int btnsOffset = 3;

  private string titleArt = @"███████╗ ██████╗ ██╗     ██╗████████╗ █████╗ ██████╗ ██╗ ██████╗ 
██╔════╝██╔═══██╗██║     ██║╚══██╔══╝██╔══██╗██╔══██╗██║██╔═══██╗
███████╗██║   ██║██║     ██║   ██║   ███████║██████╔╝██║██║   ██║
╚════██║██║   ██║██║     ██║   ██║   ██╔══██║██╔══██╗██║██║   ██║
███████║╚██████╔╝███████╗██║   ██║   ██║  ██║██║  ██║██║╚██████╔╝
╚══════╝ ╚═════╝ ╚══════╝╚═╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝ ╚═════╝ ";

  public MenuActivity(ActivityManager activityManager) {
    _activityManager = activityManager;

    _buttons = [
      new("Nuova partita", () => _activityManager.Launch(new GameActivity(activityManager))),
      new("Opzioni", () => _activityManager.Launch(new SettingsActivity(activityManager))),
      new("Ripristina sessione", () => {
        try {
          var deserializedGame = Serializer.LoadFromFile(Config.SaveFilename);
          _activityManager.Launch(new GameActivity(activityManager, deserializedGame));
        } catch (Exception) {
          var errorModal = new Modal("Errore", "Impossibile caricare la partita.\nIl file di salvataggio potrebbe essere corrotto.", [new("OK", () => _activityManager.CloseModal())]);
          _activityManager.ShowModal(errorModal);
        }
      }),

      new("Esci", () => _activityManager.Stop())
    ];
  }

  public void OnEnter() {
    //Draw();
    _buttons[2].Disabled = !File.Exists(Config.SaveFilename);
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    switch (keyInfo.Key) {
      case ConsoleKey.Escape:
        Tuple<string, Action>[] btns = [
         new("No", () => {
           _activityManager.CloseModal();
         }),
         new("Sì", () => {
           _activityManager.Stop();
         })
        ];

        var modal = new Modal("Esci", "Sei sicuro di voler uscire?", btns);
        _activityManager.ShowModal(modal);
        break;

      case ConsoleKey.UpArrow:
        if (_selectedIndex <= 0) break;
        _selectedIndex--;
        if (_buttons[_selectedIndex].Disabled) _selectedIndex--;
        DrawButtons();
        break;
      case ConsoleKey.DownArrow:
        if (_selectedIndex >= _buttons.Count - 1) break;
        _selectedIndex++;
        if (_buttons[_selectedIndex].Disabled) _selectedIndex++;
        DrawButtons();
        break;

      case ConsoleKey.Spacebar:
      case ConsoleKey.Enter:
        _buttons[_selectedIndex].OnClick();
        break;
    }
  }

  public void Draw() {
    Console.Clear();
    //DrawBackground();
    DrawTitle();
    DrawButtons();
  }

  /// <summary>
  /// Disegna lo sfondo
  /// </summary>
  /// <remarks>È inefficiente e buggata</remarks>
  [Obsolete]
  private void DrawBackground() {
    if (deck == null) deck = new();
    (int startX, int length) = Pencil.GetCenteredStartingPoint(titleArt, startYTitle);

    int Xsubdivs = Console.WindowWidth / CardArt.cardWidth;
    int Ysubdivs = Console.WindowHeight / CardArt.cardHeight;

    for (int i = 0; i < Xsubdivs; i++) {
      int lastStartY = 1;
      for (int j = 0; j < Ysubdivs; j++) {
        int startY = rand.Next(1, 4) + lastStartY;
        var cardsLen = deck.GetCards().Count;
        var selectedCard = deck.GetCards()[rand.Next(0, cardsLen)];
        Console.ForegroundColor = CardArt.GetColor(selectedCard, true);
        lastStartY = startY + CardArt.cardHeight;
        bool fulldraw = Pencil.DrawArt(CardArt.GetCardArt(selectedCard), (CardArt.cardWidth * i), startY);
        if (!fulldraw) break;
      }
    }

    Console.ResetColor();

    const int padding = 4;
    Pencil.ClearRectangle(startX - padding, startYTitle - padding, length + padding, btnsOffset * (_buttons.Count + 2) + 3 + padding);
  }

  public (int, int) GetMinSize() {
    int height = startYBtns + (_buttons.Count * 3) + 3;
    return (30, height);
  }

  private void DrawTitle() {
    const string titleArtShort = "Solitario";
    bool canDrawTitle = Console.WindowWidth >= titleArt.Split('\n')[0].Length;
    Pencil.DrawCentered(canDrawTitle ? titleArt : titleArtShort, startYTitle);
  }

  private void DrawButtons() {
    for (int i = 0; i < _buttons.Count; i++) {
      if (_buttons[i].Disabled) Console.ForegroundColor = ConsoleColor.DarkGray;
      bool selected = _selectedIndex == i;
      Pencil.DrawCentered(ComponentRenderer.GetButtonArt(_buttons[i], selected), startYBtns + (btnsOffset * (i + 1)));
      Console.ResetColor();
    }
  }
}
