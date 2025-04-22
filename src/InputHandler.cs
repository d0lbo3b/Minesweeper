using System.Numerics;

namespace Minesweeper;
public delegate void Handler();

public class InputHandler {

    private Cursor _cursor = null!;
    private Field _field = null!;

    private bool _hasEverOpened;
    public event Handler? OnFirstOpen;
    public event Handler? OnBombCaught;
    public event Handler? OnRestartPressed;

    
    public void HandleInput() {
        var keyInfo = Console.ReadKey(true);
        
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

                if (!_field.TryOpenCell(_cursor.GetPosition(), out var isBomb)) return;

                if (isBomb) {
                    OnBombCaught?.Invoke();
                }

                return;
            }

            case ConsoleKey.R: {
                OnRestartPressed?.Invoke();
                break;
            }
            
            #endregion
        }
    }

    public void SetField(Field field) {
        _field = field;
    }

    public void SetCursor(Cursor cursor) {
        _cursor = cursor;
    }


    #region Constructors

    public InputHandler() {}

    public InputHandler(Field field, Cursor cursor) {
        _field = field;
        _cursor = cursor;
    }

    #endregion
}