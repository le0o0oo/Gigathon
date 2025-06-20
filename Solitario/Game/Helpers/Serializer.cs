using Solitario.Game.Managers;
using Solitario.Game.Models;
using System.Text.Json;

namespace Solitario.Game.Helpers;
internal class Serializer {
  private readonly Deck deck;
  private readonly Foundation foundation;
  private readonly Tableau tableau;

  private static readonly JsonSerializerOptions options = new()
  {
    IncludeFields = true
  };

  internal Serializer(Deck deck, Foundation foundation, Tableau tableau) {
    this.deck = deck;
    this.foundation = foundation;
    this.tableau = tableau;
  }

  public void SaveAsFile(string fileName) {
    var data = Serialize();

    string jsonData = JsonSerializer.Serialize(data, options);

    File.WriteAllText(fileName, jsonData);
  }

  public static Game LoadFromFile(string fileName) {
    string content = File.ReadAllText(fileName);
    var data = JsonSerializer.Deserialize<SerializedData>(content, options);

    Deck deck;
    Foundation foundation;
    Tableau tableau;

    #region Load deck
    List<Card> deckCards = [.. data.Deck.HiddenCards.Select(CardFromStruct)];
    List<Card> wasteCards = [.. data.Deck.Waste.Select(CardFromStruct)];

    deck = new(deckCards, wasteCards);
    #endregion

    #region Load foundation
    List<Card>[] foundationData = [[], [], [], []];

    for (int i = 0; i < 4; i++) {
      foundationData[i] = [.. data.Foundation[i].Select(CardFromStruct)];
    }

    foundation = new(foundationData);
    #endregion

    #region Load tableau
    List<List<Card>> newTableau = [];
    for (int i = 0; i < 7; i++) {
      newTableau.Add([.. data.Tableau[i].Select(CardFromStruct)]);
    }

    tableau = new(newTableau);
    #endregion

    var game = new Game(deck, tableau, foundation);

    return game;
  }

  #region Private helpers

  private static CardStruct CardToStruct(Card card) {
    return new CardStruct(card.Seed, card.NumericValue, card.Revealed);
  }
  private static Card CardFromStruct(CardStruct serializedCard) {
    CardSeed seed = serializedCard.Seed switch
    {
      "spades" => CardSeed.Spades,
      "hearts" => CardSeed.Hearts,
      "diamonds" => CardSeed.Diamonds,
      "clubs" => CardSeed.Clubs,
      _ => throw new ArgumentException("Invalid card seed")
    };

    var card = new Card(seed, serializedCard.NumericValue)
    {
      Revealed = serializedCard.Revealed
    };

    return card;
  }

  private SerializedData Serialize() {
    SerializedData data = new();

    #region Deck
    data.Deck.HiddenCards = [.. deck.GetCards().Select(CardToStruct)];
    data.Deck.Waste = [.. deck.GetWaste().Select(CardToStruct)];
    #endregion

    #region Foundations
    for (int i = 0; i < 4; i++) {
      List<CardStruct> cards = [.. foundation.GetPile(i).Select(CardToStruct)];
      data.Foundation[i] = cards;
    }
    #endregion

    #region Tableau
    for (int i = 0; i < 7; i++) {
      List<CardStruct> cards = [.. tableau.GetPile(i).Select(CardToStruct)];
      data.Tableau.Add([]);
      data.Tableau[i] = cards;
    }
    #endregion

    return data;
  }

  #endregion
}
