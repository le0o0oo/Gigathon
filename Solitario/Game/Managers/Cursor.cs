namespace Solitario.Game.Managers {
  internal class Cursor {
    private enum CursorArea {
      Foundation,
      Tableau
    }
    const ConsoleColor color = ConsoleColor.DarkGreen;
    const char cursorChar = '❮';
    readonly Dictionary<CursorArea, byte> areaMax = new Dictionary<CursorArea, byte> {
      { CursorArea.Foundation, 4 }, // Numero di fondazioni
      { CursorArea.Tableau, 7 } // Numero di pile nel tableau
    };

    CursorArea currentArea = CursorArea.Tableau;
    /*Indica l'elemento corrente della selezione in base all'area, e vale solo per tableau e foundation.
     * Per esempio, se l'area corrente è Tableau, currentItemIndex indica l'indice della pila di carte selezionata.
     * Se invece è foundation, currentItemIndex indica l'indice della pila di fondazione selezionata.
    */
    int currentItemIndex = 0;

    private int[] position = { Game.cardWidth - 2, Game.cardHeight + 2 }; // Posizione iniziale del cursore (colonna, riga)

    internal Cursor() {
      Draw();
    }

    /// <summary>
    /// Renderizza il cursore date le coordinate attuali.
    /// </summary>
    internal void Draw() {
      (int prevLeft, int prevTop) = Console.GetCursorPosition();

      Console.SetCursorPosition(position[0], position[1]);
      Console.ForegroundColor = color;
      Console.Write(cursorChar);

      Console.ResetColor();
      Console.SetCursorPosition(prevLeft, prevTop);
    }

    /// <summary>
    /// Imposta la posizione del cursore nella console.
    /// </summary>
    /// <param name="top">Numero di righe a partire dall'inizio</param>
    /// <param name="left">Numero di colonne a partire da sinistra</param>
    internal void SetPosition(int left, int top) {
      (int prevLeft, int prevTop) = Console.GetCursorPosition();

      // Rimuove il cursore dalla posizione precedente
      Console.SetCursorPosition(position[0], position[1]);
      Console.Write(' '); // Sovrascrive con uno spazio vuoto

      // Imposta la nuova posizione del cursore
      position[0] = left;
      position[1] = top;
      Console.SetCursorPosition(position[0], position[1]);
      Console.ForegroundColor = color;
      Console.Write(cursorChar); // Disegna il cursore nella nuova posizione

      Console.ResetColor();
      Console.SetCursorPosition(prevLeft, prevTop);

    }

    internal void MoveUp() {
      if (currentArea == CursorArea.Foundation) return;

      currentItemIndex = 0;
      SetPosition(Game.cardWidth * 4 - 2, 1);
      currentArea = CursorArea.Foundation;

    }

    internal void MoveDown() {
      if (currentArea == CursorArea.Tableau) return;

      currentItemIndex = 0;
      SetPosition(Game.cardWidth - 2, Game.cardHeight + 2);
      currentArea = CursorArea.Tableau;
    }

    internal void MoveLeft() {
      if (currentItemIndex <= 0) return;
      if (currentArea == CursorArea.Foundation) {
        currentItemIndex--;

        SetPosition(Game.cardWidth * (4 + currentItemIndex) - 2, 1);
      }
      else {
        currentItemIndex--;
        SetPosition(Game.cardWidth * (currentItemIndex + 1) - 2, Game.cardHeight + 2);
      }
    }

    internal void MoveRight() {
      if (currentItemIndex >= areaMax[currentArea] - 1) return;

      if (currentArea == CursorArea.Foundation) {
        currentItemIndex++;

        SetPosition(Game.cardWidth * (4 + currentItemIndex) - 2, 1);
      }
      else {
        currentItemIndex++;
        SetPosition(Game.cardWidth * (currentItemIndex + 1) - 2, Game.cardHeight + 2);
      }
    }
  }
}
