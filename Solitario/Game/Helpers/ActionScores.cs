namespace Solitario.Game.Helpers;
internal static class ActionScores {
  #region Suggerimenti delle azioni
  // Punteggi positivi
  internal const int MoveToFoundation = 15;
  internal const int RevealTableauCard = 10;
  internal const int MoveFromWasteToTableau = 5;
  internal const int MoveKingToEmptySpace = 3;
  internal const int MoveFromTableauToTableau = 0; // Utente potrebbe abusare di un loop

  // Penalità
  internal const int MoveFromFoundationToTableau = -20;
  internal const int UndoPenalty = -1; // Penalità per l'uso dell'annulla
  internal const int HintPenality = -3; // Penalità per l'uso delle hint
  internal const float MovePenalty = -.4f; // Penalità per ogni mossa (applicata alla fine)
  #endregion

  #region Suggerimenti delle hint
  internal const int HintMoveToFoundation = 100;
  internal const int HintRevealTableauCard = 75;
  internal const int HintMoveKingToEmptySpace = 60;
  internal const int HintMoveFromWasteToTableau = 50;
  internal const int HintBaseTableauToTableauMove = 10;
  internal const int HintDrawFromDeck = 1;
  internal const int HintNoMove = 0;
  #endregion
}
