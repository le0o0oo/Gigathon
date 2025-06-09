namespace Solitario.Game.Managers;
internal class Legend {
  // Config
  private static readonly int legendWidth = 61;
  private static readonly int legendStartHeight = Game.cardHeight * 3;
  private static readonly string[] selectTexts = { "Seleziona carta/e", "Posiziona" };
  private static string pickCardText = "Pesca una carta dal mazzo";
  private static string pickWasteText = "Seleziona carta di riserva";
  private static string deselectText = "Annulla selezione";

  // Variabili di stato
  private int selectTextIndex = 0; // Indice per il testo di selezione corrente

  internal Legend() {

  }

  /// <summary>
  /// Disegna la legenda in base allo stato attuale
  /// </summary>
  internal void Draw() {
    //legendWidth = Console.WindowWidth / 2;
    Console.SetCursorPosition(0, legendStartHeight);
    Console.Write(
        $"\u001b[1;34m╔{new string('═', legendWidth - 2)}╗\n" +
        $"\u001b[1;34m║\u001b[0m  \u001b[1;32mUsa le freccie per muovere il cursore\u001b[0m{new string(' ', legendWidth - 2 - 39)}\u001b[1;34m║\n" +
        $"\u001b[1;34m║\u001b[0m  \u001b[1;33m(R)\u001b[0m \u001b[1;3{(selectTextIndex == 0 ? '6' : '0')}m{pickCardText}\u001b[0m{new string(' ', legendWidth - 2 - 6 - pickCardText.Length)}\u001b[1;34m║\n" +
        $"\u001b[1;34m║\u001b[0m  \u001b[1;33m(E)\u001b[0m \u001b[1;3{(selectTextIndex == 0 ? '6' : '0')}m{pickWasteText}\u001b[0m{new string(' ', legendWidth - 2 - 6 - pickWasteText.Length)}\u001b[1;34m║\n" +
        $"\u001b[1;34m║\u001b[0m  \u001b[1;33m(Spazio)\u001b[0m \u001b[1;36m{selectTexts[selectTextIndex]}\u001b[0m{new string(' ', legendWidth - 2 - 11 - selectTexts[selectTextIndex].Length)}\u001b[1;34m║\n" +
        $"\u001b[1;34m║\u001b[0m  \u001b[1;33m(X)\u001b[0m \u001b[1;3{(selectTextIndex == 0 ? '0' : '6')}m{deselectText}\u001b[0m{new string(' ', legendWidth - 2 - 6 - deselectText.Length)}\u001b[1;34m║\n" +
        //$"\u001b[1;34m║\u001b[0m  \u001b[1;37mIn attesa di un input...\u001b[0m{new string(' ', legendWidth - 2 - 26)}\u001b[1;34m║\n" +
        $"\u001b[1;34m╚{new string('═', legendWidth - 2)}╝\u001b[0m\n"
    );
  }

  internal void SetSelected(bool selected) {
    // Cambia il testo di selezione in base allo stato
    selectTextIndex = selected ? 1 : 0;
    Draw(); // Ridisegna la legenda con il nuovo stato
  }
}
