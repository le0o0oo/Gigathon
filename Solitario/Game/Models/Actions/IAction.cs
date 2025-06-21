namespace Solitario.Game.Models.Actions;

internal interface IAction {
  /// <summary>
  /// Esegue l'azione
  /// </summary>
  void Execute();

  /// <summary>
  /// Annulla l'azione
  /// </summary>
  void Undo();
}
