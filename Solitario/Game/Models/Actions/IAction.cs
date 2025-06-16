namespace Solitario.Game.Models.Actions;

internal interface IAction {
  void Execute();

  void Undo();
}
