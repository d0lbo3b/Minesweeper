using System.Numerics;

namespace Minesweeper;

public class InputHandler {
	private Field _field;
	private Cursor _cursor;

	private bool _hasEverOpened;
	
	public delegate void FirstOpenHandler();
	public event FirstOpenHandler OnFirstOpen;
	

	#region Constructors

	public InputHandler(){}
	
	public InputHandler(Field field, Cursor cursor) {
		_field = field;
		_cursor = cursor;
	}
	
	#endregion

	public void HandleInput(ConsoleKeyInfo keyInfo, out bool hasCaughtBomb) {
		switch (keyInfo.Key) {
			#region Movement
			case ConsoleKey.W: {
				_cursor.MoveBy(new Vector2(-1, 0));
				break;
			}
			case ConsoleKey.S: {
				_cursor.MoveBy(new Vector2(1, 0));
				break;
			}
			case ConsoleKey.D: {
				_cursor.MoveBy(new Vector2(0, 1));
				break;
			}
			case ConsoleKey.A: {
				_cursor.MoveBy(new Vector2(0, -1));
				break;
			}
			#endregion
			#region Special
			case ConsoleKey.F: {
				_field.FlagCell(_cursor.GetPosition());
				break;
			}
			case ConsoleKey.Spacebar: {
				if (!_hasEverOpened) {
					OnFirstOpen?.Invoke();
					_hasEverOpened = true;
				}
				_field.OpenCell(_cursor.GetPosition(), out var isBomb);
				
				hasCaughtBomb = isBomb;
				return;
			}
			#endregion
		}

		hasCaughtBomb = false;
	}
	
	public void SetField(Field field) {
		_field = field;
	}

	public void SetCursor(Cursor cursor) {
		_cursor = cursor;
	}
}