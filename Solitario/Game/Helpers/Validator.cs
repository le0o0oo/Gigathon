using Solitario.Game.Models;

namespace Solitario.Game.Helpers;
internal static class Validator {
  /// <summary>
  /// Valida una mossa
  /// </summary>
  /// <param name="sourceCard">La carta sorgente</param>
  /// <param name="targetPile">Una lista di carte che corrisponde alle carte della pila di destinazione</param>
  /// <param name="targetArea">L'area di destinazione</param>
  /// <param name="targetIndex">Utilizzato solo se targetArea è Foundation e deve essere l'indice della pila della fondazione</param>
  /// <returns></returns>
  internal static bool ValidateCardMove(Card sourceCard, List<Card> targetPile, Areas targetArea, int targetIndex = -1) {
    if (targetArea == Areas.Tableau) {
      // Se è un re
      if (targetPile.Count == 0) {
        return sourceCard.NumericValue == 13; // Just return the result of the comparison!
      }
      if (targetPile[^1].Color == sourceCard.Color) return false; // Stesso colore, non valido

      if (sourceCard.NumericValue == targetPile[^1].NumericValue - 1) return true;
      return false;
    }
    else if (targetArea == Areas.Foundation) {
      // Caso dell'asso
      if (Managers.Foundation.seedIndexMap[sourceCard.Seed] != targetIndex) return false;

      if (targetPile.Count == 0) {
        return sourceCard.NumericValue == 1;
      }
      if (sourceCard.Seed != targetPile[0].Seed) return false; // Seme diverso, non valido

      if (sourceCard.NumericValue == targetPile[^1].NumericValue + 1) return true;


      return false;
    }

    else return false; // Non supportato
  }

}
