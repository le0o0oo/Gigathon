using System.Text.Json.Serialization;

namespace Solitario.Game.Data;

public struct CardStruct {
  public CardSeed Seed { get; set; }
  public byte NumericValue { get; set; }
  public bool Revealed { get; set; }

  internal CardStruct(CardSeed seed, byte Value, bool revealed) {
    this.Seed = seed;
    this.NumericValue = Value;
    this.Revealed = revealed;
  }
}
public struct DeckData {
  [JsonPropertyName("hiddenCards")]
  public List<CardStruct> HiddenCards { get; set; }

  [JsonPropertyName("waste")]
  public List<CardStruct> Waste { get; set; }
}

public struct StatsData {
  [JsonPropertyName("startTime")]
  public long StartTime { get; set; }

  [JsonPropertyName("score")]
  public int Score { get; set; }

  [JsonPropertyName("moves")]
  public int Moves { get; set; }

  [JsonPropertyName("undos")]
  public int Undos { get; set; }

  [JsonPropertyName("hints")]
  public int Hints { get; set; }
}

public struct SerializedData {
  [JsonPropertyName("version")]
  public int Version { get; set; } = 1;

  [JsonPropertyName("stats")]
  public StatsData Stats;

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