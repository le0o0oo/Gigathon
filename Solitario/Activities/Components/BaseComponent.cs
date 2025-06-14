namespace Solitario.Activities.Components;
internal abstract class BaseComponent {
  internal string Text { get; }
  internal Action OnClick { get; set; } // An action to perform when the button is "clicked"

  internal BaseComponent(string text, Action onClick) {
    Text = text;
    OnClick = onClick;
  }
}
