using Solitario.Activities.Models;

namespace Solitario.Activities.Components;
internal class Checkbox : ActionComponent {
  internal bool Checked = false;

  internal Checkbox(string text, Action onClick) : base(text, onClick) { }

  /// <summary>
  /// Inverti lo stato di Checked
  /// </summary>
  /// <returns>the new state</returns>
  public bool Toggle() {
    Checked = !Checked;
    return Checked;
  }
}
