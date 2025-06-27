using Solitario.Game.Managers;
using Solitario.Game.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solitario.Game.Helpers;

/// <summary>
/// Classe utilizzata per serializzare/deserializzare lo stato del gioco in json
/// </summary>
internal class Serializer {
  private readonly Deck deck;
  private readonly Foundation foundation;
  private readonly Tableau tableau;
  private readonly Stats statsManager;

  private static readonly JsonSerializerOptions options = new()
  {
    IncludeFields = true,

    Converters = { new JsonStringEnumConverter() }
  };

  internal Serializer(Deck deck, Foundation foundation, Tableau tableau, Stats statsManager) {
    this.deck = deck;
    this.foundation = foundation;
    this.tableau = tableau;
    this.statsManager = statsManager;
  }

  /// <summary>
  /// Salva lo stato del gioco come un file
  /// </summary>
  /// <param name="fileName">Nome del file + estensione (per es. save.json)</param>
  public void SaveAsFile(string path) {
    var data = Serialize();

    string jsonData = JsonSerializer.Serialize(data, options);

    File.WriteAllText(path, jsonData);
  }

  /// <summary>
  /// Restituisce un oggetto di una partita caricata da un file
  /// </summary>
  /// <param name="fileName">Nome del file + estensione (per es. save.json)</param>
  /// <returns></returns>
  public static Game LoadFromFile(string fileName) {
    string content = File.ReadAllText(fileName);
    var data = JsonSerializer.Deserialize<SerializedData>(content, options);

    Deck deck;
    Foundation foundation;
    Tableau tableau;
    Stats statsManager;

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

    #region Load stats
    statsManager = new(tableau, data.Stats.Score, data.Stats.Moves, data.Stats.Undos, data.Stats.Hints, data.Stats.StartTime);
    #endregion

    var game = new Game(deck, tableau, foundation, statsManager);

    return game;
  }

  #region Private helpers
  /// <summary>
  /// Converte un oggetto Card alla sua relativa struttura serializzabile
  /// </summary>
  /// <param name="card"></param>
  /// <returns></returns>
  private static CardStruct CardToStruct(Card card) {
    return new CardStruct(card.Seed, card.NumericValue, card.Revealed);
  }
  /// <summary>
  /// Converte una strutta CardStruct in un oggetto Card utilizzabile
  /// </summary>
  /// <param name="serializedCard"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  private static Card CardFromStruct(CardStruct serializedCard) {
    CardSeed seed = serializedCard.Seed;

    var card = new Card(seed, serializedCard.NumericValue)
    {
      Revealed = serializedCard.Revealed
    };

    return card;
  }

  /// <summary>
  /// Restituisce la struttura serializzabile della partita
  /// </summary>
  /// <returns></returns>
  private SerializedData Serialize() {
    SerializedData data = new();

    #region Stats
    data.Stats.StartTime = statsManager.StartTime;
    data.Stats.Score = statsManager.Value;
    data.Stats.Moves = statsManager.MovesCount;
    data.Stats.Undos = statsManager.UndosCount;
    data.Stats.Hints = statsManager.HintsCount;
    #endregion

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
