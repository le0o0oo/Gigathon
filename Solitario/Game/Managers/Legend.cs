namespace Solitario.Game.Managers;
internal class Legend {
  // Config
  internal static readonly string[] selectTexts = { "Seleziona carta/e", "Posiziona" };
  internal static readonly string pickCardText = "Pesca una carta dal mazzo";
  internal static readonly string pickWasteText = "Seleziona carta di riserva";
  internal static readonly string deselectText = "Annulla selezione";
  internal static readonly string undoText = "Annulla azione";
  internal static readonly string[] hintText = { "Suggerimento", "Applica suggerimento" };
  internal static readonly string menuText = "Menu";

  // Variabili di stato
  internal int selectTextIndex { get; private set; } = 0; // Indice per il testo di selezione corrente
  internal bool CanUndo { get; private set; } = false;

  internal Legend() {

  }

  internal void SetSelected(bool selected) {
    // Cambia il testo di selezione in base allo stato
    selectTextIndex = selected ? 1 : 0;
  }

  internal void SetCanUndo(bool newState) {
    CanUndo = newState;
  }
}
