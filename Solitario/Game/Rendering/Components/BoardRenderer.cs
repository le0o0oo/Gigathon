using Solitario.Game.Managers;
using Solitario.Game.Models;
using Solitario.Game.Rendering.Helpers;
using Solitario.Utils;

namespace Solitario.Game.Rendering.Components;
internal class BoardRenderer {
  private readonly Deck deck;
  private readonly Tableau tableau;
  private readonly Foundation foundation;

  private readonly int tableauStartLine = CardArt.cardHeight + 2;

  internal BoardRenderer(Deck deck, Tableau tableau, Foundation foundation) {
    this.deck = deck;
    this.tableau = tableau;
    this.foundation = foundation;
  }

  /// <summary>
  /// Disegna la parte di <see cref="Areas.Deck"/>
  /// </summary>
  internal void DrawDeck() {
    Console.ResetColor();
    Pencil.ClearRectangle(0, 0, CardArt.cardWidth * 2, CardArt.cardHeight + 2);

    string art = CardArt.GetFlippedArt();

    if (deck.GetCards().Count == 0) {
      art = CardArt.GetEmptyArt();
    }

    Console.SetCursorPosition(0, 0);
    Console.Write($"Mazzo ({deck.GetCards().Count})");
    Console.SetCursorPosition(CardArt.cardWidth, 0);
    Console.Write($"Scarti ({deck.GetWaste().Count})");

    Console.ForegroundColor = ConsoleColor.DarkGray;
    string[] lines = art.Split([Environment.NewLine], StringSplitOptions.None);

    for (int i = 0; i < lines.Length; i++) {
      Console.SetCursorPosition(0, i + 1);
      Console.Write(lines[i]);
    }

    // Waste

    var wasteCard = deck.GetTopWaste();
    if (wasteCard != null) {
      art = CardArt.GetCardArt(wasteCard);
    }
    else {
      art = CardArt.GetEmptyArt();
    }
    string[] wasteLines = art.Split([Environment.NewLine], StringSplitOptions.None);
    Console.ForegroundColor = wasteCard == null ? ConsoleColor.DarkGray : CardArt.GetColor(wasteCard, true);

    for (int i = 0; i < wasteLines.Length; i++) {
      Console.SetCursorPosition(CardArt.cardWidth, i + 1);
      Console.Write(wasteLines[i]);
    }

    Console.ResetColor();
  }

  /// <summary>
  /// Disegna una pila del tableau specificata.
  /// </summary>
  /// <param name="index">Indice della pila</param>
  /// <param name="clearRectangle">se cancellare l'area della pila</param>
  /// <exception cref="ArgumentOutOfRangeException">Se l'indice della pila è fuori dai limiti</exception>
  internal void DrawTableauPile(int index, bool clearRectangle = false) {
    if (index < 0 || index >= tableau.Piles.Count) {
      throw new ArgumentOutOfRangeException(nameof(index), "Indice della pila del tableau fuori dai limiti.");
    }

    if (clearRectangle) Pencil.ClearRectangle(index * CardArt.cardWidth, tableauStartLine, CardArt.cardWidth, Renderer.tableauHeight);

    byte j = 0;

    if (tableau.GetPile(index).Count == 0) {
      string[] lines = CardArt.GetEmptyArt().Split([Environment.NewLine], StringSplitOptions.None);
      Console.ForegroundColor = ConsoleColor.DarkGray;
      for (int k = 0; k < lines.Length; k++) {
        Console.SetCursorPosition(index * CardArt.cardWidth, j + tableauStartLine);
        Console.Write(lines[k]);
        j++;
      }
      return;
    }

    var rawTableau = tableau.Piles;

    foreach (Card card in rawTableau[index]) {

      Console.ForegroundColor = CardArt.GetColor(card);
      if (rawTableau[index].IndexOf(card) != rawTableau[index].Count - 1) {
        Console.SetCursorPosition(index * CardArt.cardWidth, j + tableauStartLine);
        Console.WriteLine(CardArt.GetShortArt(card));
      }
      else {
        Console.SetCursorPosition(index * CardArt.cardWidth, j + tableauStartLine);
        //DrawArt(card.GetCardArt());
        string[] lines = CardArt.GetCardArt(card).Split([Environment.NewLine], StringSplitOptions.None);
        byte offset = 0;
        for (int line = 0; line < lines.Length; line++) {
          Console.SetCursorPosition(index * CardArt.cardWidth, j + offset + tableauStartLine);
          Console.Write(lines[line]);
          offset++;
        }
      }

      j++;
    }
  }

  /// <summary>
  /// Disegna la parte di <see cref="Areas.Tableau"/>
  /// </summary>
  internal void DrawTableau() {
    Console.ResetColor();
    Pencil.ClearRectangle(0, tableauStartLine, CardArt.cardWidth * 7, Renderer.tableauHeight);
    // Itera per ogni colonna
    for (int i = 0; i < 7; i++) {
      DrawTableauPile(i);
    }

    Console.ResetColor();
  }

  /// <summary>
  /// Disegna la parte di <see cref="Areas.Foundation"/>
  /// </summary>
  internal void DrawFoundations() {
    Console.ResetColor();
    int startXPos = CardArt.cardWidth * 3;
    Pencil.ClearRectangle(startXPos, 0, CardArt.cardWidth * 4, CardArt.cardHeight + 2);

    Console.SetCursorPosition(startXPos, 0);
    Console.Write("Fondazioni");


    for (int i = 0; i < 4; i++) {
      string foundationArt = foundation.GetPile(i).Count == 0 ? CardArt.GetFoundationArt(i) : CardArt.GetCardArt(foundation.GetPile(i)[^1]);
      string[] lines = foundationArt.Split([Environment.NewLine], StringSplitOptions.None);
      ConsoleColor foregroundColor;
      if (foundation.GetPile(i).Count == 0) foregroundColor = ConsoleColor.DarkGray;
      else foregroundColor = CardArt.GetColor(foundation.GetPile(i)[^1]);
      Console.ForegroundColor = foregroundColor;

      for (int j = 0; j < lines.Length; j++) {
        Console.SetCursorPosition(startXPos, j + 1);
        Console.Write(lines[j]);
      }

      startXPos += CardArt.cardWidth;
    }

    Console.ResetColor();
  }
}
