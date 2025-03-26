using System.Numerics;
using Minesweeper.Utilities;

namespace Minesweeper;

internal class Program {
	private static readonly string _directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\k0nch\minesweeperData\";
	private static readonly string _filePath = $"{_directory}preferences.json";


	private static void Main() {
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
			fieldData!.Height = Convert.ToInt32(Console.ReadLine());
			Console.Clear();
		
			Console.Write("Enter Mines Covering in %: ");
			fieldData!.MinesPercentage = Convert.ToInt32(Console.ReadLine());
			Console.Clear();
			
			GenerateJson(fieldData);
		}
		
		ConsoleKeyInfo input;
		do {
			var field = new Field(fieldData!, new Vector2(1, 1), out var cursor);
			var inputHandler = new InputHandler(field, cursor);
		
			inputHandler.OnFirstOpen += () => { field.Randomize(fieldData!.MinesPercentage, cursor.GetPosition()); };
			do {
				field.Draw();
				inputHandler.HandleInput(Console.ReadKey(), out var hasCaughtBomb);
			
				var stopConditions = hasCaughtBomb || field.IsAllClear();
				if (stopConditions) break;
			} while (true);

			if (field.IsAllClear()) {
				field.DrawVictoryMessage();
			}
			else {
				field.OpenAllMines();
				field.DrawDefeatMessage();
			}
		
			Console.WriteLine("Press R to restart");
			Console.WriteLine("Press any other key to exit");
			input = Console.ReadKey();
		} while (input.Key == ConsoleKey.R);
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