using Solitario.Game.Managers;
using Solitario.Game.Rendering;

namespace Solitario.Game;
internal static class Validator {
  internal static bool ValidateCardMove(Card sourceCard, List<Card> targetPile, Selection.Areas targetArea) {
    if (targetArea == Selection.Areas.Tableau) {
      // Se è un re
      if (targetPile.Count == 0) {
        if (sourceCard.numericValue == 13) return true;
        return false;
      }
      if (CardArt.GetColor(targetPile[^1]) == CardArt.GetColor(sourceCard)) return false; // Stesso colore, non valido

      if (sourceCard.numericValue == targetPile[^1].numericValue - 1) return true;
      return false;
    }
    else if (targetArea == Selection.Areas.Foundation) {
      // Caso dell'asso
      if (targetPile.Count == 0) {
        if (sourceCard.numericValue == 1) return true;
        return false;
      }
      if (sourceCard.seed != targetPile[0].seed) return false; // Seme diverso, non valido

      if (sourceCard.numericValue == targetPile[^1].numericValue + 1) return true;
      return false;
    }

    else return false; // Non supportato
  }

}
