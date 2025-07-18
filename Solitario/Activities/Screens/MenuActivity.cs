﻿using Solitario.Activities.Components;
using Solitario.Activities.Models;
using Solitario.Activities.Rendering;
using Solitario.Utils;

namespace Solitario.Activities.Screens;
internal class MenuActivity : IActivity {
	private readonly ActivityManager _activityManager;
	private readonly List<Button> _buttons;
	private int _selectedIndex = 0;

	private const int startYTitle = 5;
	private const int startYBtns = 10;
	private const int btnsOffset = 3;

	private string titleArt = @"███████╗ ██████╗ ██╗     ██╗████████╗ █████╗ ██████╗ ██╗ ██████╗ 
██╔════╝██╔═══██╗██║     ██║╚══██╔══╝██╔══██╗██╔══██╗██║██╔═══██╗
███████╗██║   ██║██║     ██║   ██║   ███████║██████╔╝██║██║   ██║
╚════██║██║   ██║██║     ██║   ██║   ██╔══██║██╔══██╗██║██║   ██║
███████║╚██████╔╝███████╗██║   ██║   ██║  ██║██║  ██║██║╚██████╔╝
╚══════╝ ╚═════╝ ╚══════╝╚═╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝ ╚═════╝ ";

	public MenuActivity(ActivityManager activityManager) {
		_activityManager = activityManager;

		_buttons = [
			new("Nuova partita", () => _activityManager.Launch(new GameActivity(activityManager))),
			new("Partite salvate", () => {
				_activityManager.Launch(new SavedGamesActivity(_activityManager));
			}),
			new("Impostazioni", () => _activityManager.Launch(new SettingsActivity(activityManager))),

			new("Esci", () => _activityManager.Stop())
		];
	}

	public void OnEnter() {
		//Draw();
		// _buttons[1].Disabled = !File.Exists(Config.SaveFilename);
	}

	public void HandleInput(ConsoleKeyInfo keyInfo) {
		switch (keyInfo.Key) {
			case ConsoleKey.Escape:
				Tuple<string, Action>[] btns = [
				 new("No", () => {
					 _activityManager.CloseModal();
				 }),
				 new("Sì", () => {
					 _activityManager.Stop();
				 })
				];

				var modal = new Modal("Esci", "Sei sicuro di voler uscire?", btns);
				_activityManager.ShowModal(modal);
				break;

			case ConsoleKey.UpArrow:
				if (_selectedIndex <= 0) break;
				_selectedIndex--;
				if (_buttons[_selectedIndex].Disabled) _selectedIndex--;
				DrawButtons();
				break;
			case ConsoleKey.DownArrow:
				if (_selectedIndex >= _buttons.Count - 1) break;
				_selectedIndex++;
				if (_buttons[_selectedIndex].Disabled) _selectedIndex++;
				DrawButtons();
				break;

			case ConsoleKey.Spacebar:
			case ConsoleKey.Enter:
				_buttons[_selectedIndex].OnClick();
				break;
		}
	}

	public void Draw() {
		Console.Clear();
		DrawTitle();
		DrawButtons();
	}

	public (int, int) GetMinSize() {
		int height = startYBtns + (_buttons.Count * 3) + 3;
		return (30, height);
	}

	private void DrawTitle() {
		const string titleArtShort = "Solitario";
		bool canDrawTitle = Console.WindowWidth >= titleArt.Split(["\r\n", "\n", "\r"], StringSplitOptions.None)[0].Length;
		Pencil.DrawCentered(canDrawTitle ? titleArt : titleArtShort, startYTitle);
	}

	private void DrawButtons() {
		for (int i = 0; i < _buttons.Count; i++) {
			if (_buttons[i].Disabled) Console.ForegroundColor = ConsoleColor.DarkGray;
			bool selected = _selectedIndex == i;
			Pencil.DrawCentered(ComponentRenderer.GetButtonArt(_buttons[i], selected), startYBtns + (btnsOffset * (i + 1)));
			Console.ResetColor();
		}
	}
}
