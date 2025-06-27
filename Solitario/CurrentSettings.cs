namespace Solitario;

internal static class CurrentSettings {
  /// <summary>
  /// Mischia il mazzo ogni volta che si svuota
  /// </summary>
  internal static bool ShuffleEmptyDeck = false;
  /// <summary>
  /// Abilita i suggerimenti
  /// </summary>
  internal static bool UseHints = true;

  /// <summary>
  /// Utilizza il formato ANSI per i colori
  /// </summary>
  internal static bool UseAnsi = false;
}