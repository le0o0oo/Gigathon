namespace Solitario.Activities.Models;

/// <summary>
/// Una classe che rappresenta qualsiasi componente utilizzato che può triggerare una azione 
/// </summary>
internal abstract class ActionComponent {
  internal string Text { get; }
  internal Action OnClick { get; set; } // An action to perform when the button is "clicked"

  internal ActionComponent(string text, Action onClick) {
    Text = text;
    OnClick = onClick;
  }
}
