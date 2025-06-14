using Solitario.Activities.Components;

namespace Solitario.Activities.Models;
internal class Button : BaseComponent {

  internal Button(string text, Action onClick) : base(text, onClick) { }

}
