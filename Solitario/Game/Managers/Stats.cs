using Solitario.Game.Helpers;
using Solitario.Game.Models.Actions;

namespace Solitario.Game.Managers;
internal class Stats {
  private readonly DateTime startTime;
  private readonly Tableau tableau;

  internal int Value { get; private set; } = 0;
  internal int TimeElapsed => (int)(DateTime.UtcNow - startTime).TotalSeconds;
  internal int MovesCount { get; private set; } = 0;
  internal int UndosCount { get; private set; } = 0;
  internal int HintsCount { get; private set; } = 0;

  internal Stats(Tableau tableau, int value, int movesCount, int undosCount, int hintsCount) {
    //this.startTime = startTime;
    this.tableau = tableau;
    Value = value;
    MovesCount = movesCount;
    UndosCount = undosCount;
    HintsCount = hintsCount;
  }

  internal Stats(Tableau tableau) {
    startTime = DateTime.UtcNow;
    this.tableau = tableau;
  }

  private static bool ValidateAction(IAction action) {
    //if (action is MoveCardsAction moveAction) {
    //if (moveAction.sourceArea == Areas.Tableau && moveAction.destArea == moveAction.sourceArea) return false;
    //}
    if (action is DrawCardAction) return false;

    return true;
  }

  /// <summary>
  /// Applica il punteggio di una azione al punteggio totale.
  /// </summary>
  /// <remarks>Non vuol dire che il valore del punteggio verrà per forza incrementato! Se il valore dell'azione è negativo, esso verrà decrementato.
  /// Inoltre, è meglio applicare la funzione PRIMA di eseguire l'azione!
  /// </remarks>
  /// <param name="action"></param>
  internal void ApplyActionScore(IAction action) {
    if (!ValidateAction(action)) return;

    Value += ActionScoreCalculator.Calculate(action, tableau);
  }

  /// <summary>
  /// Rimuove il punteggio di una azione dal punteggio totale.
  /// </summary>
  /// <remarks>Non vuol dire che il valore del punteggio verrà per forza decrementato! Se il valore dell'azione è negativo, esso verrà incrementato</remarks>
  /// <param name="action"></param>
  internal void RemoveActionScore(IAction action) {
    if (!ValidateAction(action)) return;



    Value -= ActionScoreCalculator.Calculate(action, tableau);
  }

  /// <summary>
  /// Incrementa il contatore delle mosse di 1
  /// </summary>
  internal void IncMovesCount() {
    MovesCount++;
  }

  /// <summary>
  /// Decrementa il contatore delle mosse di 1, se possibile (non può essere negativo)
  /// </summary>
  internal void DecMovesCount() {
    if (MovesCount > 0) MovesCount--;
  }

  /// <summary>
  /// Incrementa il contatore degli annullamenti di 1
  /// </summary>
  internal void IncUndosCount() {
    UndosCount++;
  }

  /// <summary>
  /// Incrementa il contatore degli indizi di 1
  /// </summary>
  internal void IncHintsCount() {
    HintsCount++;
  }
}
