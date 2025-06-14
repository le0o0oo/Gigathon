namespace Solitario.Activities.Components;
internal class Checkbox : BaseComponent {
  internal bool Checked = false;

  internal Checkbox(string text, Action onClick) : base(text, onClick) { }

  /// <summary>
  /// Toggles between Checked and Unchecked state
  /// </summary>
  /// <returns>the new state</returns>
  public bool Toggle() {
    Checked = !Checked;
    return Checked;
  }
}
