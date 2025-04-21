using System.Numerics;

namespace Minesweeper;

public class Field {
    private readonly Cursor _cursor = null!;

    private int _bombsCount;

    private Cell[,] _field = null!;
    private Vector2 _fieldSize;
    private Vector2 _padding;
    public bool Solved => IsAllClear();

    
    private void Resize(Vector2 newSize) {
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
                } else {
                    DrawPadding((int)_padding.X);
                    GetCell(new Vector2(x, y)).Draw();
                    DrawPadding((int)_padding.Y);
                }
            }
            Console.WriteLine();
        }

        if (!Solved) {
            DrawGuide();
        }
    }

    public void Randomize(int bombsPercentage, Vector2 cursorPosition) {
        bombsPercentage = Math.Clamp(bombsPercentage, 0, 100);

        Clear();

        for (var x = 0; x < _fieldSize.X; x++) {
            for (var y = 0; y < _fieldSize.Y; y++) {
                var random = new Random();
                var randomNumber = random.Next(0, 100);
                var cell = GetCell(new Vector2(x, y));
                
                if (randomNumber > bombsPercentage
                    || new Vector2(x, y) == cursorPosition
                    || cell.IsNeighbourOf(cursorPosition)) continue;

                _bombsCount++;
                cell.SetMine();
            }
        }

        for (var x = 0; x < _fieldSize.X; x++) {
            for (var y = 0; y < _fieldSize.Y; y++) {
                GetCell(new Vector2(x, y)).CalculateMinesNearby(this);
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

    private void DrawGuide() {
        Console.WriteLine();
        Console.Write("WASD - cursor movement\nSPACE - open\nF - flag");
    }

    private void RevealEmptyArea(Vector2 initialPos) {
        var checkedCells = new HashSet<int>();
        var cellsToCheck = new Queue<(Vector2, bool)>();
        cellsToCheck.Enqueue((initialPos, false));
    
        while (cellsToCheck.Count > 0) {
            var pos = cellsToCheck.Dequeue();
            checkedCells.Add(pos.Item1.GetHashCode());
            GetCell(pos.Item1).SetOpen();
            
            for (var dx = -1; dx <= 1; dx++) {
                for (var dy = -1; dy <= 1; dy++) {
                    var newPos = pos.Item1 + new Vector2(dx, dy);
                    if (checkedCells.Contains(newPos.GetHashCode())
                        || !TryGetCell(newPos, out var cell)
                        || !CanBeOpened(cell)
                        || pos.Item2)
                        continue;
                    var hasMinesNearby = cell.MinesNearby > 0;
                    cellsToCheck.Enqueue((newPos, hasMinesNearby));
                }
            }
        }
    }
    
    public Cell GetCell(Vector2 position) {
        return _field[(int)position.X, (int)position.Y];
    }

    public void OpenCell(Vector2 position, out bool isBomb) {
        var cell = GetCell(position);
        if (cell.Flagged != FlagState.Flagged) {
            cell.SetOpen();

            if (cell.CalculateMinesNearby(this) == 0) {
                RevealEmptyArea(position);
            }
        }
        isBomb = cell.Type == CellType.Mine;
    }

    public void OpenCell(Cell cell, out bool isBomb) {
        if (cell.Flagged != FlagState.Flagged) {
            cell.SetOpen();
            
            if (cell.CalculateMinesNearby(this) == 0) {
                RevealEmptyArea(cell.Position);
            }
        }
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

    public bool TryOpenCell(Cell cell, out bool isBomb) {
        if (cell.Flagged != FlagState.Flagged) {
            OpenCell(cell, out isBomb);
            return true;
        }

        isBomb = false;
        return false;
    }

    public bool CanBeOpened(Cell cell) {
        return cell.Flagged != FlagState.Flagged;
    }
    
    public void FlagCell(Vector2 position) {
        GetCell(position).SetFlagged();
    }

    public bool TryFlagCell(Vector2 position) {
        if (!TryGetCell(position, out var cell)) return false;

        cell.SetFlagged();
        return true;
    }

    private void DrawPadding(int padding) {
        for (var i = 0; i < padding; i++) {
            Console.Write(' ');
        }
    }

    public void DrawDefeatMessage() {
        Console.WriteLine("\nOops!");
    }

    public void OpenAllMines() {
        for (var x = 0; x < _fieldSize.X; x++) {
            for (var y = 0; y < _fieldSize.Y; y++) {
                var cell = GetCell(new Vector2(x, y));
                if (cell.Type == CellType.Mine) {
                    OpenCell(cell, out _);
                }
            }
        }
        Draw();
    }

    public void DrawVictoryMessage() {
        Console.WriteLine("\nYou Win!");
    }

    private bool IsAllClear() {
        var clearedCellsCounter = (int)_fieldSize.X * (int)_fieldSize.Y;

        for (var x = 0; x < _fieldSize.X; x++) {
            for (var y = 0; y < _fieldSize.Y; y++) {
                if (GetCell(new Vector2(x, y)).Type == CellType.Empty
                    && GetCell(new Vector2(x, y)).State == CellState.Opened) {
                    clearedCellsCounter--;
                }
            }
        }

        return clearedCellsCounter == _bombsCount;
    }

    private void Clear() {
        for (var x = 0; x < _fieldSize.X; x++) {
            for (var y = 0; y < _fieldSize.Y; y++) {
                _field[x, y] = new Cell(new Vector2(x, y));
            }
        }
    }


    #region Constructors

    public Field() {}

    public Field(int x, int y, Vector2 padding, out Cursor cursor) {
        Resize(new Vector2(x, y));

        _padding = padding;
        cursor = new Cursor(this, new Vector2(0, 0));
        _cursor = cursor;
    }

    public Field(FieldData data, Vector2 padding, out Cursor cursor) {
        Resize(new Vector2(data.Width, data.Height));

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
}