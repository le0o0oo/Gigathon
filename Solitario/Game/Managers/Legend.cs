using Solitario.Game.Data;

namespace Solitario.Game.Managers;
internal class Legend {
  // Config
  internal static readonly string[] selectTexts = { "Seleziona carta/e", "Posiziona" };
  internal static readonly string pickCardText = "Pesca una carta dal mazzo";
  internal static readonly string pickWasteText = "Seleziona carta di scarto";
  internal static readonly string deselectText = "Annulla selezione";
  internal static readonly string undoText = "Annulla azione";
  internal static readonly string toFoundationText = "Porta a fondazione (se possibile)";
  internal static readonly string[] hintText = { "Suggerimento", "Applica suggerimento" };
  internal static readonly string menuText = "Menu";

  // Variabili di stato
  internal int SelectTextIndex { get; private set; } = 0; // Indice per il testo di selezione corrente
  internal bool CanUndo { get; private set; } = false; // Se è possibile annullare l'azione
  internal bool CanShortCutFoundation { get; set; } = true; // Se è possibile portare una carta in fondazione con un tasto di scelta rapida

  internal Legend() {

  }

  internal void SetSelected(bool selected) {
    // Cambia il testo di selezione in base allo stato
    SelectTextIndex = selected ? 1 : 0;
  }

  internal void SetCanUndo(bool newState) {
    CanUndo = newState;
  }

  /// <summary>
  /// Aggiorna lo stato di CanShortCutFoundation in base alla selezione.
  /// </summary>
  /// <param name="cursor"></param>
  /// <param name="selection"></param>
  internal void UpdateFoundationShortcut(Selection selection) {
    if (selection.SourceArea == Areas.Tableau) {
      CanShortCutFoundation = !selection.Active;
    }
    else if (selection.Active && selection.SourceArea == Areas.Deck) CanShortCutFoundation = true;
    else CanShortCutFoundation = true;
  }
}
