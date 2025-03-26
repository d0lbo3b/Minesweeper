using System.Numerics;

namespace Minesweeper;

public enum CellType {
	Empty = ' ',
	Mine = '@'
}

public enum CellState {
	Opened = ' ',
	Closed = '.'
}

public enum FlagState {
	None,
	Flagged = '!'
}

public class Cell {
	public CellType Type { get; private set; } = CellType.Empty;
	public CellState State { get; private set; } = CellState.Closed;
	public FlagState Flagged { get; private set; } = FlagState.None;
	
	private int NeighboursCount { get; set; }
	

	public void Draw() {
		Console.Write(GetDrawOutput());
	}

	public string GetDrawOutput() {
		if (Flagged == FlagState.Flagged) {
			return ((char)Flagged).ToString();
		}
		if (State == CellState.Opened) {
			if (NeighboursCount > 0 && Type == CellType.Empty) {
				return $"{NeighboursCount}";
			}
			return ((char)Type).ToString();
		}
	
		return ((char)State).ToString();
	}

	public void CalculateNeighbours(Field field, Vector2 position) {
		if (Type == CellType.Mine) return;
		
		for (var dx = -1; dx <= 1; dx++) {
			for (var dy = -1; dy <= 1; dy++) {
				if (field.TryGetCell(new Vector2(position.X - dx, position.Y - dy), out var cell)
					&& cell != this
					&& cell.Type == CellType.Mine) {
					NeighboursCount++;
				}
			}
		}
	}

	public void SetOpen() {
		State = CellState.Opened;
	}
	
	public void SetMine() {
		Type = CellType.Mine;
	}

	public void SetFlagged() {
		Flagged = Flagged == FlagState.Flagged ? FlagState.None : FlagState.Flagged;
	}

	public static bool IsNeighbourOf(Vector2 cellPosition, Vector2 nbourPosition) {
		for (var dx = -1; dx < 1; dx++) {
			for (var dy = -1; dy < 1; dy++) {
				if (cellPosition + new Vector2(dx, dy) == nbourPosition) {
					return true;
				}
			}
		}
		
		return false;
	}
}