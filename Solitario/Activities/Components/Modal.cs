using Solitario.Utils;

namespace Solitario.Activities.Components;
internal class Modal {
  private readonly string title;
  private readonly string content;
  private readonly string[] splittedContent;
  private readonly int width;
  private int startTop;

  private int startTopCounter = 0;

  internal Action? OnClose;

  internal Modal(string title, string content) {
    this.title = title;
    this.content = content;
    this.splittedContent = content.Split('\n');

    int contentWidth = splittedContent.OrderByDescending(line => line.Length).FirstOrDefault()?.Length ?? 0;
    int titleWidth = title.Length + 6;

    this.width = Math.Max(contentWidth + 4, titleWidth + 4);
  }

  internal void Draw() {
    startTopCounter = 0;
    int modalHeight = 1 + splittedContent.Length + 2; // Top + content lines + bottom + shadow
    startTop = (Console.WindowHeight - modalHeight) / 2;

    DrawTop();
    for (int i = 0; i < splittedContent.Length; i++) {
      DrawLine(splittedContent[i]);
    }
    DrawBottom();
  }

  internal void HandleInput(ConsoleKeyInfo keyInfo) {
    OnClose?.Invoke();
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
                      text.Length;

    Pencil.DrawCentered($" ║ {text}{new string(' ', padding)} ║█", startTop + startTopCounter);

    startTopCounter++;
  }

  private void DrawBottom() {
    Pencil.DrawCentered($" ╚{new string('═', width - 2)}╝█", startTop + startTopCounter);
    Pencil.DrawCentered($"  {new string('▀', width - 2)}▀", startTop + startTopCounter + 1);
  }
}
