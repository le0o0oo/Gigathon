namespace Solitario.Game.Managers;
internal class Selection {
  internal enum Areas {
    Waste,
    Foundation,
    Tableau
  }
  private readonly Tableau tableau;
  private readonly Deck deck;
  private readonly Foundation foundation;

  // Variabili di stato della selezione
  internal List<Card> selectedCards { get; private set; } = new List<Card>();
  int sourceIndex = 0; // Indice della selezione corrente. Valido per tableau e foundation, e si riferisce alla pila della selezione.
  internal Areas sourceArea { get; private set; } = Areas.Waste;

  internal bool active { get; private set; } = false;

  internal Selection(Tableau tableau, Deck deck, Foundation foundation) {
    this.tableau = tableau;
    this.deck = deck;
    this.foundation = foundation;
  }

  public void SetSelection(Areas area, int pileIndex, List<Card> cards) {
    sourceArea = area;
    sourceIndex = pileIndex;
    selectedCards = cards;
    active = true; // Attiva la selezione

    // Evidenzia la selezione

  }
  public void ClearSelection() {
    selectedCards.Clear();
    active = false; // Disattiva la selezione
  }

  public void AddToTarget(Areas destArea, int destIndex) {
    if (!active) throw new Exception("Selection mode not active");

    // Da tableau a fondazione
    if (sourceArea == Areas.Tableau && destArea == Areas.Foundation) {
      if (selectedCards.Count != 1) throw new ArgumentException("Per la fondazione è possibile selezionare una sola carta alla volta.", nameof(selectedCards));

      int pileVal; // Valore della pila di destinazione
      if (foundation.GetCards(destIndex).Count == 0) pileVal = 0; // Se la pila è vuota, il valore è 0
      else pileVal = foundation.GetCardAt(destIndex, -1).numericValue;

      foundation.AddCard(selectedCards[0]);

      tableau.TakeCardAt(sourceIndex, -1);
      if (tableau.GetPile(sourceIndex).Count > 0) tableau.GetCardAt(sourceIndex, -1).revealed = true;

    }
    // Muovi carte del tableau
    else if (sourceArea == Areas.Tableau && destArea == Areas.Tableau) {
      tableau.GetPile(destIndex).AddRange(selectedCards);

      tableau.TakeCards(sourceIndex, tableau.GetPile(sourceIndex).Count - selectedCards.Count);
      if (tableau.GetPile(sourceIndex).Count > 0) tableau.GetCardAt(sourceIndex, -1).revealed = true;

    }
    // Dalla fondazione al tableau
    else if (sourceArea == Areas.Foundation && destArea == Areas.Tableau) {
      foundation.TakeCardAt(sourceIndex, -1); // Rimuovi la carta dalla riserva
      tableau.GetPile(destIndex).Add(selectedCards[0]); // Aggiungi la carta al tableau
    }
    // Dalla riserva al tableau
    else if (sourceArea == Areas.Waste && destArea == Areas.Tableau) {
      var card = deck.TakeWasteCardAt(-1); // Prendi l'ultima carta dalla riserva
      card.revealed = true;

      tableau.GetPile(destIndex).Add(card);
    }
    // Dalla riserva alla fondazione
    else if (sourceArea == Areas.Waste && destArea == Areas.Foundation) {
      if (selectedCards.Count != 1) throw new ArgumentException("Per la fondazione è possibile selezionare una sola carta alla volta.", nameof(selectedCards));

      var card = deck.TakeWasteCardAt(-1); // Prendi l'ultima carta dalla riserva
      card.revealed = true;
      foundation.AddCard(card);
    }
    else {
      //throw new ArgumentException("Combinazione di aree non valida.", nameof(destArea));
    }

    selectedCards.Clear();
    active = false;
  }
}