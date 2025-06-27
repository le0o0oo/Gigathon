using Solitario.Game.Models.Actions;

namespace Solitario.Game.Managers;

/// <summary>
/// Manager del command pattern
/// </summary>
internal class Actions {
  /// <summary>
  /// Restituisce l'ultima azione eseguita, se esiste
  /// </summary>
  internal IAction? LastAction => _history.Count > 0 ? _history.Peek() : null;

  private readonly Stack<IAction> _history = [];

  /// <summary>
  /// Esegue una azione dato un suo oggetto e lo salva in memoria
  /// </summary>
  /// <param name="action"></param>
  internal void Execute(IAction action) {
    action.Execute();
    _history.Push(action);
  }

  /// <summary>
  /// Annulla l'ultima azione
  /// </summary>
  /// <returns></returns>
  internal IAction Undo() {
    var action = _history.Pop();
    action.Undo();

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