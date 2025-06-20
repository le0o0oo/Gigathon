namespace Solitario.Activities;
internal interface IActivity {
  /// <summary>
  /// Called when the activity becomes the active one.
  /// Use this to perform initial setup or drawing.
  /// </summary>
  void OnEnter();

  /// <summary>
  /// Draws the current state of the activity to the console.
  /// </summary>
  void Draw();

  (int, int) GetMinSize();

  /// <summary>
  /// Handles user input for the activity.
  /// </summary>
  internal void HandleInput(ConsoleKeyInfo keyInfo);
}
