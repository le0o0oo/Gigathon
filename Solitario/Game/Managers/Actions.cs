using Solitario.Game.Models.Actions;

namespace Solitario.Game.Managers;

/// <summary>
/// Manager del command pattern
/// </summary>
internal class Actions {
  /// <summary>
  /// Restituisce l'ultima azione dello stack. Se una azione è stata annullata, questa non viene considerata.
  /// </summary>
  internal IAction? TopAction => _history.Count > 0 ? _history.Peek() : null;
  /// <summary>
  /// L'ultima azione eseguita, includendo le azioni annullate
  /// </summary>
  internal IAction? LastAction { get; private set; } = null;

  private readonly Stack<IAction> _history = [];

  /// <summary>
  /// Esegue una azione dato un suo oggetto e lo salva in memoria
  /// </summary>
  /// <param name="action"></param>
  internal void Execute(IAction action) {
    action.Execute();
    _history.Push(action);
    LastAction = action;
  }

  /// <summary>
  /// Annulla l'ultima azione
  /// </summary>
  /// <returns></returns>
  internal IAction Undo() {
    var action = _history.Pop();
    action.Undo();
    LastAction = action;

    return action;
  }

  /// <summary>
  /// Restituisce True/False in base a se sono esistenti azioni precedenti
  /// </summary>
  /// <returns></returns>
  internal bool CanUndo() {
    return _history.Count > 0;
  }
}