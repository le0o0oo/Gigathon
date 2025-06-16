using Solitario.Game.Models.Actions;

namespace Solitario.Game.Managers;

internal class Actions {
  private readonly Stack<IAction> _history = [];

  internal void Execute(IAction action) {
    action.Execute();
    _history.Push(action);
  }

  internal IAction Undo() {
    var action = _history.Pop();
    action.Undo();

    return action;
  }

  internal bool CanUndo() {
    return _history.Count > 0;
  }
}