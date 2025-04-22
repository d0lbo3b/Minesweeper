using System.Numerics;
using Minesweeper.Utilities;

namespace Minesweeper;

internal static class Program {
    private static readonly string _directory = Path.Combine(Environment.CurrentDirectory, "data");
    private static readonly string _filePath =  Path.Combine(_directory, "prefs.json");


    private static void Main() {
        var input = new ConsoleKeyInfo();
        var shouldRestart = false;

        do {
            Console.Clear();
            shouldRestart = false;
            
            var answer = ConsoleKey.Q;
            var fieldData = new FieldData();

            if (JsonUtility.ReadSafe<FieldData?>(_filePath, ref fieldData)) {
                Console.WriteLine("I've found your old preferences.\nWould you like to keep them? [Q - refuse] or [E - accept]\n");
                fieldData?.Display();

                answer = Console.ReadKey().Key;
                Console.Clear();
            }

            if (answer == ConsoleKey.Q) {
                Console.WriteLine("Welcome to Minesweeper!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();

                Console.Write("Enter Field width: ");
                fieldData!.Width = Convert.ToInt32(Console.ReadLine());
                Console.Clear();

                Console.Write("Enter Field height: ");
                fieldData.Height = Convert.ToInt32(Console.ReadLine());
                Console.Clear();

                Console.Write("Enter Mines Covering in %: ");
                fieldData.MinesPercentage = Convert.ToInt32(Console.ReadLine());
                Console.Clear();

                GenerateJson(fieldData);
            }
            Console.CursorVisible = false;
        
            var field = new Field(fieldData!, new Vector2(1, 1), out var cursor);
            var inputHandler = new InputHandler(field, cursor);
            var bombCaught = false;
            
            inputHandler.OnFirstOpen += () => { field.Randomize(fieldData!.MinesPercentage, cursor.GetPosition()); };
            inputHandler.OnBombCaught += () => { bombCaught = true;};
            inputHandler.OnRestartPressed += () => { shouldRestart = true; };
            
            do {
                field.Draw();
                inputHandler.HandleInput();

                if (bombCaught
                    || shouldRestart) {
                    break;
                }
            } while (true);
            field.Draw();

            if (field.Solved) {
                field.DrawVictoryMessage();
            } else if (bombCaught) {
                field.DrawDefeatMessage();
            } else if (shouldRestart) {
                continue;
            }
            field.OpenAllMines();
            
            Console.WriteLine("\nPress R to restart");
            Console.WriteLine("Press any other key to exit");
            input = Console.ReadKey(true);
        } while (input.Key == ConsoleKey.R || shouldRestart);
    }

    private static void GenerateJson(FieldData fieldData) {
        if (!File.Exists(_filePath)) {
            Directory.CreateDirectory(_directory);
        }

        JsonUtility.WriteSafe(_filePath, fieldData);
    }
}

//Sometimes(most of time) this code is being shit
//No AI shit used