using System.Numerics;
using System.Text;

namespace Minesweeper;

enum Border {
	Left = '[',
	Right = ']'
}

public class Cursor {
	private Vector2 _cursorPosition;
	private Field _field;
	
	
	#region Constructors

	public Cursor() {
		_cursorPosition = new Vector2(0, 0);
	}
	
	public Cursor(Field field, Vector2 cursorPosition) {
		_field = field;
		MoveTo(cursorPosition);
	}
	
	#endregion

	public void MoveTo(Vector2 position) {
		if (_field.TryGetCell(position, out _)) {
			_cursorPosition = position;
		}
	}

	public void MoveBy(Vector2 delta) {
		MoveTo(_cursorPosition + delta);
	}
	
	public string GetCursor() {
		var padding = _field.GetPadding();

		var cursor = new StringBuilder();
		
		cursor.Append((char)Border.Left);
		cursor.Append(_field.GetCell(_cursorPosition).GetDrawOutput());
		cursor.Append((char)Border.Right);

		return cursor.ToString();
	}

	public Vector2 GetPosition() {
		return _cursorPosition;
	}
	
	public void SetField(Field field) {
		_field = field;
	}
}