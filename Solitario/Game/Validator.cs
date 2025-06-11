using Solitario.Game.Types;

namespace Solitario.Game;
internal static class Validator {
  internal static bool ValidateCardMove(Card sourceCard, List<Card> targetPile, Areas targetArea) {
    if (targetArea == Areas.Tableau) {
      // Se è un re
      if (targetPile.Count == 0) {
        if (sourceCard.NumericValue == 13) return true;
        return false;
      }
      if (targetPile[^1].Color == sourceCard.Color) return false; // Stesso colore, non valido

      if (sourceCard.NumericValue == targetPile[^1].NumericValue - 1) return true;
      return false;
    }
    else if (targetArea == Areas.Foundation) {
      // Caso dell'asso
      if (targetPile.Count == 0) {
        if (sourceCard.NumericValue == 1) return true;
        return false;
      }
      if (sourceCard.Seed != targetPile[0].Seed) return false; // Seme diverso, non valido

      if (sourceCard.NumericValue == targetPile[^1].NumericValue + 1) return true;
      return false;
    }

    else return false; // Non supportato
  }

}
