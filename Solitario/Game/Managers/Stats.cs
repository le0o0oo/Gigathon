using Solitario.Game.Helpers;
using Solitario.Game.Models.Actions;

namespace Solitario.Game.Managers;
internal class Stats {
  private readonly DateTime startTime;
  private readonly Tableau tableau;

  internal long StartTime => ((DateTimeOffset)startTime).ToUnixTimeSeconds(); // Unix timestamp in secondi
  internal int Value { get; private set; } = 0;
  internal TimeSpan TimeElapsed => (DateTime.UtcNow - startTime);
  internal int MovesCount { get; private set; } = 0;
  internal int UndosCount { get; private set; } = 0;
  internal int HintsCount { get; private set; } = 0;
  internal int TimeBonus { get; private set; } = 0;
  internal int MovesPenality => (int)(ActionScores.MovePenalty * MovesCount);

  internal Stats(Tableau tableau, int value, int movesCount, int undosCount, int hintsCount, long startTime) {
    //this.startTime = startTime;
    this.tableau = tableau;
    Value = value;
    MovesCount = movesCount;
    UndosCount = undosCount;
    HintsCount = hintsCount;
    this.startTime = DateTimeOffset.FromUnixTimeSeconds(startTime).UtcDateTime;
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

    if (Value < 0) Value = 0;
  }

  /// <summary>
  /// Rimuove il punteggio di una azione dal punteggio totale.
  /// </summary>
  /// <remarks>Non vuol dire che il valore del punteggio verrà per forza decrementato! Se il valore dell'azione è negativo, esso verrà incrementato</remarks>
  /// <param name="action"></param>
  internal void RemoveActionScore(IAction action) {
    if (!ValidateAction(action)) return;

    Value -= ActionScoreCalculator.Calculate(action, tableau);

    if (Value < 0) Value = 0;
  }

  internal void ApplyUndoPenality() {
    Value += ActionScores.UndoPenalty;
    if (Value < 0) Value = 0;

  }

  internal void ApplyHintPenalty() {
    Value += ActionScores.HintPenality;
    if (Value < 0) Value = 0;

  }

  /// <summary>
  /// Calcola il punteggio finale includendo il bonus tempo.
  /// Chiamare solo in caso di vittoria.
  /// </summary>
  /// <returns>Una tupla con (bonus tempo, penalità mosse)</returns>
  internal void CalculateFinalScore() {
    if (TimeElapsed.TotalSeconds > 30) { // Nessun bonus se ci metti meno di 30 secondi
      // Formula di esempio: 700,000 / secondi totali.
      // Più veloce sei, più alto il bonus.
      TimeBonus = 700000 / (int)TimeElapsed.TotalSeconds;
      Value += TimeBonus;
    }

    Value += MovesPenality; // Penalità per le mosse
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
