namespace Solitario.Activities.Models;
internal class Button : ActionComponent {
  internal bool Disabled = false;

  internal Button(string text, Action onClick) : base(text, onClick) { }

}
