using Solitario.Activities.Components;
using Solitario.Activities.Models;
using Solitario.Activities.Rendering;
using Solitario.Game.Helpers;
using Solitario.Utils;
using System.Text;

namespace Solitario.Activities.Screens;

internal class SavedGames : IActivity {
  private readonly ActivityManager _activityManager;
  private readonly List<Button> _buttons = new(); // Corrected initialization of the list

  const int maxButtonsPerPage = 10;
  const int maxFileLength = 23;
  const int buttonsStartY = 5;
  private int PagesCount => (int)((_buttons.Count) / maxButtonsPerPage) + 1;
  /// <summary>
  /// Indice del primo pulsante della pagina corrente.
  /// </summary>
  private int FirstBtnIndex => _currentPage * maxButtonsPerPage + 1; // +1 per il pulsante "Indietro"

  private int _selectedIndex = 0; // Indice della selezione attuale
  private int _currentPage = 0; // Indice della pagina corrente

  internal SavedGames(ActivityManager activityManager) {
    this._activityManager = activityManager;

    _buttons.Add(new Button("◄ Indietro (Esc)", () => {
      _activityManager.Back();
    }));
  }

  private void LoadGame(string fileName) {
    try {
      var deserializedGame = Serializer.LoadFromFile(Path.Combine(Config.SavesDirectory, fileName));
      _activityManager.Back();
      _activityManager.Launch(new GameActivity(_activityManager, deserializedGame));
    }
    catch (Exception) {
      var errorModal = new Modal("Errore", "Impossibile caricare la partita.\nIl file di salvataggio potrebbe essere corrotto.", [new("OK", () => _activityManager.CloseModal())]);
      _activityManager.ShowModal(errorModal);
    }
  }

  public void OnEnter() {
    if (!Directory.Exists(Config.SavesDirectory)) {
      Directory.CreateDirectory(Config.SavesDirectory);
    }

    string[] files = Directory.GetFiles(Config.SavesDirectory, "*.json").Select(Path.GetFileName).ToArray()!;

    foreach (string file in files) {
      StringBuilder sb = new(file);
      // +5 per .json
      if (sb.Length > maxFileLength + 5) {
        sb.Remove(maxFileLength - 3, sb.Length - maxFileLength + 3);
        sb.Append("...");
      }
      sb.Replace("-", ":").Replace(".json", "");
      _buttons.Add(new Button(sb.ToString(), () => LoadGame(file)));
    }
  }

  public void Draw() {
    Console.Clear();
    DrawUI();
  }

  private void DrawUI() {
    int endIndex = Math.Min(FirstBtnIndex + maxButtonsPerPage, _buttons.Count);

    Pencil.DrawArt(ComponentRenderer.GetButtonArt(_buttons[0], _selectedIndex == 0), 1, 0);
    Pencil.DrawCentered($"Pagina {_currentPage + 1}/{PagesCount}", 0);
    Pencil.DrawCentered($"Salvataggi totali: {_buttons.Count - 1}", 1);
    Pencil.DrawCentered("Usa le freccie per cambiare selezione/pagina", 2);

    int iteration = 0;
    for (int i = FirstBtnIndex; i < endIndex; i++) {
      if (i + 1 > _buttons.Count) break; // Previene l'errore di indice fuori limite

      var el = _buttons[i];
      Pencil.DrawCentered(ComponentRenderer.GetButtonArt(el, _selectedIndex == i), buttonsStartY + (iteration * 3));

      iteration++;
    }

    if (iteration == 0) {
      // Se non ci sono bottoni da mostrare, mostra un messaggio
      Pencil.DrawCentered("Nessun salvataggio disponibile.", buttonsStartY);
    }

    /*for (int i = 1; i < _buttons.Count; i++) {
      var el = _buttons[i];
      if (el != null) {
        Pencil.DrawCentered(ComponentRenderer.GetButtonArt(el, _selectedIndex == i), startY + (i * spacing));
      }
    } */
  }

  public (int, int) GetMinSize() {
    int width = 90;
    // 3 è l'altezza di un bottone
    int height = buttonsStartY + 3 * maxButtonsPerPage;
    return (width, height);
  }

  public void HandleInput(ConsoleKeyInfo keyInfo) {
    switch (keyInfo.Key) {
      case ConsoleKey.Escape:
        _activityManager.Back();
        return;

      case ConsoleKey.Spacebar:
      case ConsoleKey.Enter:
        _buttons[_selectedIndex].OnClick.Invoke();
        return;

      case ConsoleKey.UpArrow:
        if (_selectedIndex <= 0) break;

        if (_selectedIndex <= FirstBtnIndex) {
          // Primo bottone della pagina
          _selectedIndex = 0; // Imposta il tasto "Indietro" come selezionato
        }
        else _selectedIndex--; // Caso generale, decrementa l'indice

        break;
      case ConsoleKey.DownArrow:
        if (_selectedIndex >= _buttons.Count - 1) break;

        // Se il bottone selezionato è il tasto "Indietro"
        if (_selectedIndex == 0) {
          _selectedIndex = FirstBtnIndex;

          break;
        }

        if (_selectedIndex >= FirstBtnIndex + maxButtonsPerPage - 1) {
          // Ultimo bottone della pagina
          if (_currentPage < PagesCount - 1) {
            _currentPage++;
            Console.Clear();
          }
        }
        else _selectedIndex++;

        break;

      case ConsoleKey.RightArrow:
        if (_currentPage >= PagesCount - 1) break;
        _currentPage++;
        Console.Clear();

        break;
      case ConsoleKey.LeftArrow:
        if (_currentPage <= 0) break;
        _currentPage--;
        Console.Clear();

        break;
    }

    // In caso la selezione sia fuori dai limiti della pagina corrente, reimpostala al primo bottone della pagina
    if (_selectedIndex < FirstBtnIndex || _selectedIndex >= (FirstBtnIndex + maxButtonsPerPage)) {
      if (_selectedIndex != 0) _selectedIndex = FirstBtnIndex;
    }
    DrawUI();
  }
}
