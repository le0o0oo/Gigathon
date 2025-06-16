using Solitario.Game.Managers;

namespace Solitario.Game.Models.Actions;
internal class DrawCardAction : IAction {
  Deck deck;

  internal DrawCardAction(Deck deck) {
    this.deck = deck;
  }

  public void Execute() {
    deck.PickCard();
  }

  public void Undo() {
    if (deck.GetWaste().Count > 0) {
      var lastWaste = deck.TakeWasteCardAt(-1);
      deck.AddToDeckAt(lastWaste, 0);
    }
    // Caso del mazzo scarto vuoto
    else {
      var waste = deck.GetWaste();
      var cards = deck.GetCards();

      waste.AddRange(deck.GetCards().AsEnumerable());
      cards.Clear();
    }
  }
}
