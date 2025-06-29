using Solitario.Game.Data;
using Solitario.Game.Managers;
using Solitario.Game.Models.Actions;
using Solitario.Game.Rendering.Helpers;

namespace Solitario.Game.Rendering.Components;
internal class ActionRenderer {
  Deck deck;
  Tableau tableau;
  Foundation foundation;

  internal ActionRenderer(Deck deck, Tableau tableau, Foundation foundation) {
    this.deck = deck;
    this.tableau = tableau;
    this.foundation = foundation;
  }

  /// <summary>
  /// Disegna una azione
  /// </summary>
  /// <param name="managers"></param>
  /// <param name="action"></param>
  internal void DrawAction(IAction action, bool animate) {
    const ConsoleColor sourceColor = ConsoleColor.Yellow;
    const ConsoleColor destColor = ConsoleColor.DarkGreen;

    if (action is DrawCardAction) {
      Console.ForegroundColor = ConsoleColor.Black;
      Console.BackgroundColor = sourceColor;
      var artLines = CardArt.GetFlippedArt().Split('\n');

      for (int i = 0; i < artLines.Length; i++) {
        Console.SetCursorPosition(0, i + 1);
        Console.Write(artLines[i]);
      }
    }
    else if (action is MoveCardsAction movAction) {
      switch (movAction.sourceArea) {
        case Areas.Tableau:
          int cardPileIndex = movAction.sourceIndex;
          int startDrawIndex = tableau.GetPile(cardPileIndex).IndexOf(movAction.CardsSelection[0]);
          var cards = movAction.CardsSelection;

          for (int i = 0; i < cards.Count; i++) {
            var card = cards[i];
            string art = i == cards.Count - 1 ? CardArt.GetCardArt(card) : CardArt.GetShortArt(card);
            string[] lines = art.Split('\n');

            Console.ForegroundColor = CardArt.GetColor(card) == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.Red;
            Console.BackgroundColor = sourceColor;

            for (int j = 0; j < lines.Length; j++) {
              Console.SetCursorPosition(CardArt.cardWidth * cardPileIndex, CardArt.cardHeight + 2 + j + i + startDrawIndex);
              Console.WriteLine(lines[j]);
            }
          }
          break;

        case Areas.Deck:
          Console.BackgroundColor = sourceColor;
          UIRenderer.DrawCard(deck.GetTopWaste()!, CardArt.cardWidth, 1, true);
          break;
      }

      if (animate) Thread.Sleep(360);

      Console.BackgroundColor = destColor;
      switch (movAction.destArea) {
        case Areas.Tableau:
          int cardPileIndex = movAction.destIndex;
          int startDrawIndex = 0;
          var cards = tableau.GetPile(cardPileIndex);

          for (int i = 0; i < cards.Count; i++) {
            var card = cards[i];
            if (!card.Revealed) continue;
            string art = i == cards.Count - 1 ? CardArt.GetCardArt(card) : CardArt.GetShortArt(card);
            string[] lines = art.Split('\n');

            Console.ForegroundColor = CardArt.GetColor(card) == ConsoleColor.White ? ConsoleColor.White : ConsoleColor.DarkRed;

            for (int j = 0; j < lines.Length; j++) {
              Console.SetCursorPosition(CardArt.cardWidth * cardPileIndex, CardArt.cardHeight + 2 + j + i + startDrawIndex);
              Console.WriteLine(lines[j]);
            }
          }

          if (cards.Count == 0) {
            var lines = CardArt.GetEmptyArt().Split('\n');

            for (int j = 0; j < lines.Length; j++) {
              Console.SetCursorPosition(CardArt.cardWidth * cardPileIndex, CardArt.cardHeight + 2 + j + startDrawIndex);
              Console.WriteLine(lines[j]);

            }
          }
          break;

        case Areas.Foundation:
          string[] foundationLines;
          string foundationArt = foundation.GetPile(movAction.destIndex).Count == 0 ? CardArt.GetFoundationArt(movAction.destIndex) : CardArt.GetCardArt(foundation.GetPile(movAction.destIndex)[^1]);
          if (foundation.GetPile(movAction.destIndex).Count == 0)
            foundationLines = foundationArt.Split('\n');
          else
            foundationLines = CardArt.GetCardArt(foundation.GetCardAt(movAction.destIndex)).Split('\n');


          int startXPos = CardArt.cardWidth * (3 + movAction.destIndex);

          for (int j = 0; j < foundationLines.Length; j++) {
            Console.SetCursorPosition(startXPos, j + 1);
            Console.Write(foundationLines[j]);
          }
          break;
      }
    }

    Console.ResetColor();
  }
}
