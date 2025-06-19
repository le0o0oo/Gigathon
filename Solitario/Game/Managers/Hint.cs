using Solitario.Game.Models.Actions;

namespace Solitario.Game.Managers;
internal class Hint {
  public bool ShowingHint = false;
  internal IAction? LastAction { get; private set; }

  internal void SetLastAction(IAction? lastAction) {
    this.LastAction = lastAction;
  }
}
