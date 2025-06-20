using Solitario.Activities.Components;

namespace Solitario.Activities.Models;
internal class Button : InputComponent {
  internal bool Disabled = false;

  internal Button(string text, Action onClick) : base(text, onClick) { }

}
