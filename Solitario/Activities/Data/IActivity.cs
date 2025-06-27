namespace Solitario.Activities;
internal interface IActivity {
  /// <summary>
  /// Chiamato quando l'attività diventa quella corrente
  /// </summary>
  void OnEnter();

  /// <summary>
  /// Disegna lo stato attuale della attività alla console
  /// </summary>
  void Draw();

  /// <summary>
  /// Ottiene la dimensione minima per disegnare l'attività
  /// </summary>
  /// <returns></returns>
  (int, int) GetMinSize();

  /// <summary>
  /// Gestisce input utente per l'attività
  /// </summary>
  internal void HandleInput(ConsoleKeyInfo keyInfo);
}
