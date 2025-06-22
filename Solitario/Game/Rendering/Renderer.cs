using Solitario.Game.Managers;
using Solitario.Game.Models.Actions;

namespace Solitario.Game.Rendering;
internal class Renderer {
  #region Classi di stato
  private readonly Deck deck;
  private readonly Tableau tableau;
  private readonly Foundation foundation;
  private readonly Cursor cursor;
  private readonly Legend legend;
  private readonly Selection selection;
  private readonly Managers.Hint hintManager;

  private readonly BoardRenderer boardRenderer;
  private readonly UIRenderer uiRenderer;
  private readonly ActionRenderer actionRenderer;
  #endregion

  #region Costanti
  protected internal static readonly int tableauHeight = CardArt.cardHeight + 19;

  protected internal static readonly int legendWidth = 50;
  protected internal static readonly int legenStartX = 0;
  protected internal static readonly int legendStartY = CardArt.cardHeight + 3 + (tableauHeight - 6);

  internal static readonly ConsoleColor color = ConsoleColor.DarkGreen;
  internal static readonly char cursorChar = CurrentSettings.UseAnsi ? '❮' : '<';

  internal static readonly int minWidth = CardArt.cardWidth * 7;
  internal static readonly int minHeight = 1 + CardArt.cardHeight + 1 + tableauHeight + 7;
  #endregion

  #region Variabili di stato
  private static int prevLeft, prevTop = 0;  // Posizione precedente del cursore
  #endregion

  #region Private helpers
  /// <summary>
  /// Salva la posizione del cursore della console
  /// </summary>
  protected internal static void SaveCursorPosition() {
    prevLeft = Console.CursorLeft;
    prevTop = Console.CursorTop;
  }
  /// <summary>
  /// Ripristina la posizione del cursore della console
  /// </summary>
  protected internal static void RestoreCursorPosition() {
    Console.ResetColor();
    Console.SetCursorPosition(prevLeft, prevTop);
  }
  #endregion

  #region Public helpers
  /// <summary>
  /// Disegna tutte le aree
  /// </summary>
  internal void DrawAll() {
    DrawDeck();
    DrawFoundations();
    DrawTableau();
    DrawCursor();
  }

  /// <summary>
  /// Disegna una specifica area
  /// </summary>
  /// <param name="area">Area da idisegnare</param>
  internal void DrawBasedOnArea(Areas area) {
    switch (area) {
      case Areas.Tableau:
        DrawTableau();
        break;
      case Areas.Foundation:
        DrawFoundations();
        break;
      case Areas.Deck:
        DrawDeck();
        break;
    }
  }
  #endregion

  internal Renderer(Deck deck, Tableau tableau, Foundation foundation, Cursor cursor, Legend legend, Selection selection, Hint hintManager) {
    this.deck = deck;
    this.tableau = tableau;
    this.foundation = foundation;
    this.cursor = cursor;
    this.legend = legend;
    this.selection = selection;
    this.hintManager = hintManager;

    // Create the specialized renderers, giving them only what they need.
    this.boardRenderer = new BoardRenderer(deck, tableau, foundation);
    this.uiRenderer = new UIRenderer(cursor, selection, legend, hintManager);
    this.actionRenderer = new ActionRenderer(deck, tableau, foundation);
  }

  #region Methods
  internal void DrawDeck() => boardRenderer.DrawDeck();
  internal void DrawTableau() => boardRenderer.DrawTableau();
  internal void DrawFoundations() => boardRenderer.DrawFoundations();

  internal void DrawCursor() => uiRenderer.DrawCursor();
  internal void DrawSelection(bool useInitialPosition = false) => uiRenderer.DrawSelection(useInitialPosition);
  internal void DrawLegend() => uiRenderer.DrawLegend();
  internal void DrawAction(IAction action) => actionRenderer.DrawAction(action);

  #endregion
}
