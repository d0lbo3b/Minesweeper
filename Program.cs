using System.Numerics;

namespace Minesweeper;

internal class Program {
	private static void Main() {
		Console.WriteLine("Welcome to Minesweeper!");
		Console.WriteLine("Press any key to continue...");
		Console.ReadKey();
		Console.Clear();
		
		Console.Write("Enter Field width: ");
		var width = Convert.ToInt32(Console.ReadLine());
		Console.Clear();
		
		Console.Write("Enter Field height: ");
		var height = Convert.ToInt32(Console.ReadLine());
		Console.Clear();
		
		Console.Write("Enter Mines Covering in %: ");
		var minesPercentage = Convert.ToInt32(Console.ReadLine());
		Console.Clear();
		
		var field = new Field(new Vector2(height, width), new Vector2(1, 1), out var cursor);
		var inputHandler = new InputHandler(field, cursor);

		inputHandler.OnFirstOpen += () => { field.Randomize(minesPercentage, cursor.GetPosition()); };
		
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
		
		Console.WriteLine("Press any key to exit...");
		Console.ReadKey();
	}
}
//Sometimes(most of time) this code is being shit