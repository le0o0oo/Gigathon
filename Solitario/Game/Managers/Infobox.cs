namespace Solitario.Game.Managers;

internal class Infobox {
  internal string Text { get; private set; } = "";

  private readonly Stats scoreManager;

  internal Infobox(Stats scoreManager) {
    this.scoreManager = scoreManager;

    UpdateText();
  }

  internal void UpdateText() {
    AddLine($"Score: {scoreManager.Value}");
  }

  private void AddLine(string line) {
    Text += line + '\n';
  }
}
