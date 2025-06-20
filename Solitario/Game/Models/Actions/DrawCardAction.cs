using Solitario.Game.Managers;

namespace Solitario.Game.Models.Actions;
internal class DrawCardAction : IAction {
  private readonly Deck deck;

  // Usato solo se ShuffleEmptyDeck è true
  private List<Card>? wasteCards;

  internal DrawCardAction(Deck deck) {
    this.deck = deck;
  }

  public void Execute() {
    // Salva lo stato del mazzo
    if (deck.GetCards().Count == 0 && CurrentSettings.ShuffleEmptyDeck) {
      wasteCards = [.. deck.GetWaste()];
    }

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

      if (wasteCards == null) waste.AddRange(deck.GetCards().AsEnumerable());
      else waste.AddRange(wasteCards.AsEnumerable());
      cards.Clear();
    }
  }
}
