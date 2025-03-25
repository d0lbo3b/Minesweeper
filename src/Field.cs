using System.Numerics;

namespace Minesweeper;

public class Field {
	private Cell[,] _field;
	private Cursor _cursor;
	private Vector2 _fieldSize;
	private Vector2 _padding;

	private int _bombsCount;
	
	
	#region Constructors

	public Field() {}

	public Field(int x, int y, Vector2 padding, out Cursor cursor) {
		Resize(new Vector2(x, y));
		
		_padding = padding;
		cursor = new Cursor(this, new Vector2(0, 0));
		_cursor = cursor;
	}

	public Field(Vector2 fieldSize, Vector2 padding, out Cursor cursor) {
		Resize(fieldSize);
		
		_padding = padding;
		cursor = new Cursor(this, new Vector2(0, 0));
		_cursor = cursor;
	}
	
	#endregion


	public void SetPadding(Vector2 padding) {
		_padding = padding;	
	}

	public Vector2 GetPadding() {
		return _padding;
	}
	
	public void Resize(Vector2 newSize) {
		_fieldSize = newSize;

		_field = new Cell[(int)_fieldSize.X, (int)_fieldSize.Y];
		Clear();
	}

	public void Draw() {
		Console.Clear();
		for (var x = 0; x < _fieldSize.X; x++) {
			for (var y = 0; y < _fieldSize.Y; y++) {
				if (_cursor.GetPosition() == new Vector2(x, y)) {
					Console.Write(_cursor.GetCursor());
				}
				else {
					DrawPadding((int)_padding.X);
					_field[x, y].Draw();
					DrawPadding((int)_padding.Y);
				}
			}
			Console.WriteLine();
		}

		DrawGuide();
	}

	public void Randomize(int bombsPercentage, Vector2 cursorPosition) {
		bombsPercentage = Math.Clamp(bombsPercentage, 0, 100);
		_bombsCount = (int)(bombsPercentage*_fieldSize.X*_fieldSize.Y)/100;

		Clear();
		
		for (var x = 0; x < _fieldSize.X; x++) {
			for (var y = 0; y < _fieldSize.Y; y++) {
				if (_bombsCount <= 0) return;
				
				var random = new Random();
				var randomNumber = random.Next(0, 100);

				if (randomNumber <= bombsPercentage && new Vector2(x, y) != cursorPosition) {
					_field[x, y].SetMine();
				}
			}
		}

		for (var x = 0; x < _fieldSize.X; x++) {
			for (var y = 0; y < _fieldSize.Y; y++) {
				_field[x,y].CalculateNeighbours(this, new Vector2(x, y));
			}
		}
	}

	public bool TryGetCell(Vector2 position, out Cell cell) {
		if (position.X < _fieldSize.X && position.Y < _fieldSize.Y && position is { X: >= 0, Y: >= 0 }) {
			cell = GetCell(position);
			return true;
		}
		
		cell = null!;
		return false;
	}

	public void DrawGuide() {
		Console.WriteLine();
		Console.Write("WASD - cursor movement\nSPACE - open\nF - flag");
	}
	
	public Cell GetCell(Vector2 position) {
		return _field[(int)position.X, (int)position.Y];
	}

	public void OpenCell(Vector2 position, out bool isBomb) {
		var cell = GetCell(position);
		if (cell.Flagged != FlagState.Flagged)
			cell.SetOpen();
		isBomb = cell.Type == CellType.Mine;
	}

	public bool TryOpenCell(Vector2 position, out bool isBomb) {
		if (TryGetCell(position, out var cell)) {
			if (cell.Flagged != FlagState.Flagged) {
				OpenCell(position, out isBomb);
				return true;
			}
		}

		isBomb = false;
		return false;
	}

	public void FlagCell(Vector2 position) {
		GetCell(position).SetFlagged();
	}

	public bool TryFlagCell(Vector2 position) {
		if (TryGetCell(position, out var cell)) {
			cell.SetFlagged();
			return true;
		}

		return false;
	}
	
	public void DrawPadding(int padding) {
		for (var i = 0; i < padding; i++) {
			Console.Write(' ');
		}
	}

	public void DrawDefeatMessage() {
		Console.WriteLine("Oops!");
	}

	public void OpenAllMines() {
		for (var x = 0; x < _fieldSize.X; x++) {
			for (var y = 0; y < _fieldSize.Y; y++) {
				var cell = GetCell(new Vector2(x, y));
				if (cell.Type == CellType.Mine) {
					cell.SetOpen();
				}
			}
		}
		Draw();
	}

	public void DrawVictoryMessage() {
		Console.WriteLine("You Win!");
	}

	public bool IsAllClear() {
		var clearedCellsCounter = (int)_fieldSize.X * (int)_fieldSize.Y;
		
		for (var x = 0; x < _fieldSize.X; x++) {
			for (var y = 0; y < _fieldSize.Y; y++) {
				if (_field[x, y].Type == CellType.Empty && _field[x, y].State == CellState.Opened) {
					clearedCellsCounter--;
				}
			}
		}

		return clearedCellsCounter == _bombsCount;
	}
	
	private void Clear() {
		for (var x = 0; x < _fieldSize.X; x++) {
			for (var y = 0; y < _fieldSize.Y; y++) {
				_field[x, y] = new Cell();	
			}
		}
	}
}