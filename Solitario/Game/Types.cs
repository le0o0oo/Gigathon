// Types only for the game

using System.Text.Json.Serialization;

namespace Solitario.Game;

/// <summary>
/// Rappresenta le possibili aree del gioco
/// </summary>
internal enum Areas {
  /// <summary>
  /// Rappresenta sia le carte da pescare che quelle di scarto
  /// </summary>
  Deck,
  /// <summary>
  /// Fondazione
  /// </summary>
  Foundation,
  /// <summary>
  /// Rappresenta il Tableau
  /// </summary>
  Tableau
}

/// <summary>
/// Rappresenta il possibile colore di una carta
/// </summary>
internal enum CardColor {
  Red,
  Black
}

/// <summary>
/// Rappresenta i possibili semi che una carta può avere
/// </summary>
internal enum CardSeed {
  Spades,
  Hearts,
  Diamonds,
  Clubs
}

#region Serialization
/*
 * Struttua:

{
  "deck": {
    "hiddenCards": [],
    "revealedCards": []
  },
  "foundation": [[], [], [], []],
  "tableau": [[], [], [], [], [], [], []]
}

*/
public struct CardStruct {
  public string Seed;
  public byte NumericValue;
  public bool Revealed;

  internal CardStruct(CardSeed seed, byte Value, bool revealed) {
    this.Seed = seed switch
    {
      CardSeed.Spades => "spades",
      CardSeed.Hearts => "hearts",
      CardSeed.Diamonds => "diamonds",
      CardSeed.Clubs => "clubs",
      _ => throw new ArgumentException("Invalid seed parameter"),
    };

    this.NumericValue = Value;
    this.Revealed = revealed;
  }
}

public struct SerializedData {
  public struct DeckData {
    [JsonPropertyName("hiddenCards")]
    public List<CardStruct> HiddenCards { get; set; }

    [JsonPropertyName("waste")]
    public List<CardStruct> Waste { get; set; }
  }

  [JsonPropertyName("deck")]
  public DeckData Deck;

  [JsonPropertyName("foundation")]
  public List<CardStruct>[] Foundation;

  [JsonPropertyName("tableau")]
  public List<List<CardStruct>> Tableau;

  public SerializedData() {
    Foundation = [[], [], [], []];
    Tableau = new();
  }
}

#endregion