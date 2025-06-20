using Solitario.Utils;

namespace Solitario.Activities.Components;
internal class Modal {
  private readonly string title;
  private readonly string content;
  private readonly string[] splittedContent;
  private readonly int width;
  private readonly Tuple<string, Action>[]? _buttons;
  private int startTop;

  private int startTopCounter = 0;
  private int currentButtonIndex = 0;
  private int buttonStartTopIndex = 0;

  internal Action? OnClose;

  internal Modal(string title, string content, Tuple<string, Action>[]? buttons = null) {
    this.title = title;
    this.content = content;
    this.splittedContent = content.Split('\n');
    this._buttons = buttons;

    int contentWidth = Pencil.AnsiRegex.Replace(content, "").Split('\n')
      .OrderByDescending(line => line.Length).FirstOrDefault()?.Length ?? 0;
    int titleWidth = title.Length + 6;

    this.width = Math.Max(contentWidth + 4, titleWidth + 4);
  }

  internal void Draw() {
    startTopCounter = 0;
    int modalHeight = 1 + splittedContent.Length + 2; // Top + content lines + bottom + shadow
    if (_buttons != null) modalHeight++;
    startTop = (Console.WindowHeight - modalHeight) / 2;

    DrawTop();
    for (int i = 0; i < splittedContent.Length; i++) {
      DrawLine(splittedContent[i]);
    }
    if (_buttons != null) {
      DrawLine("");
      buttonStartTopIndex = startTopCounter;
      DrawButtons();
    }
    DrawBottom();
  }

  internal void HandleInput(ConsoleKeyInfo keyInfo) {

    switch (keyInfo.Key) {
      case ConsoleKey.Escape:
        OnClose?.Invoke();
        break;

      case ConsoleKey.RightArrow:
        if (currentButtonIndex + 1 > _buttons?.Length - 1) break;
        currentButtonIndex++;
        DrawButtons();
        break;

      case ConsoleKey.LeftArrow:
        if (currentButtonIndex == 0) break;
        currentButtonIndex--;
        DrawButtons();
        break;

      case ConsoleKey.Spacebar:
      case ConsoleKey.Enter:
        if (_buttons == null) break;

        _buttons[currentButtonIndex].Item2.Invoke();
        break;
    }
  }

  private void DrawTop() {
    string titleSection = $"╔═ {title} ";
    int remainingWidth = width - titleSection.Length - 1; // -1 per '╗'
    Pencil.DrawCentered($" {titleSection}{new string('═', remainingWidth)}╗ ", startTop);

    startTopCounter++;
  }

  private void DrawLine(string text) {
    int padding = width -
                      4 - // per '║' e gli spazi
                      Pencil.AnsiRegex.Replace(text, "").Length;

    Pencil.DrawCentered($" ║ {text}{new string(' ', padding)} ║█", startTop + startTopCounter);

    startTopCounter++;
  }

  private void DrawButtons() {
    if (_buttons == null) return;
    string buttonsArt = "";

    for (int i = 0; i < _buttons.Length; i++) {
      if (i > 0) buttonsArt += " | ";
      var btn = _buttons[i];
      buttonsArt += currentButtonIndex == i ?
        $"{AnsiColors.Foreground.Black + AnsiColors.Background.White}<{btn.Item1}>{AnsiColors.Reset}"
        :
        $"{btn.Item1}";
    }

    // Strip ANSI codes to get visual width
    int visualLength = Pencil.AnsiRegex.Replace(buttonsArt, "").Length;
    int paddingTotal = width - 2 - visualLength; // 4 for borders and spaces
    int paddingLeft = paddingTotal / 2;
    int paddingRight = paddingTotal - paddingLeft;


    string paddedLine = $" ║{new string(' ', paddingLeft)}{buttonsArt}{new string(' ', paddingRight)}║█";
    Pencil.DrawCentered(paddedLine, startTop + buttonStartTopIndex);

    startTopCounter++;
  }

  private void DrawBottom() {
    var extraChar = Console.WindowWidth % 2 == 0 ? 1 : 2;
    Pencil.DrawCentered($" ╚{new string('═', width - 2)}╝█", startTop + startTopCounter);
    Pencil.DrawCentered($"  {new string('▀', width - extraChar)}▀", startTop + startTopCounter + 1);
  }
}
