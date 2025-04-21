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
    public Vector2 Position { get; }
    public int MinesNearby { get; set; }
    

    public Cell(Vector2 pos) {
        Position = pos;
    }
    
    public void Draw() {
        Console.Write(GetDrawOutput());
    }

    public string GetDrawOutput() {
        if (Flagged == FlagState.Flagged) {
            return ((char)Flagged).ToString();
        }
        if (State != CellState.Opened) return ((char)State).ToString();

        if (MinesNearby > 0 && Type == CellType.Empty) {
            return $"{MinesNearby}";
        }
        return ((char)Type).ToString();

    }

    public int CalculateMinesNearby(Field field) {
        if (Type == CellType.Mine) return -1;

        MinesNearby = 0;
        for (var dx = -1; dx <= 1; dx++) {
            for (var dy = -1; dy <= 1; dy++) {
                if (field.TryGetCell(new Vector2(Position.X - dx, Position.Y - dy), out var cell)
                    && cell != this
                    && cell.Type == CellType.Mine) {
                    MinesNearby++;
                }
            }
        }

        return MinesNearby;
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

    public bool IsNeighbourOf(Vector2 nbourPosition) {
        for (var dx = -1; dx <= 1; dx++) {
            for (var dy = -1; dy <= 1; dy++) {
                if (Position + new Vector2(dx, dy) == nbourPosition) {
                    return true;
                }
            }
        }

        return false;
    }
}