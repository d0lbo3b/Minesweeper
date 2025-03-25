using System.Numerics;

namespace Minesweeper;

internal class Program {
	private static void Main() {
		var field = new Field(new Vector2(10, 10), new Vector2(1, 1), out var cursor);
		var inputHandler = new InputHandler(field, cursor);
		
		field.Randomize(20, cursor.GetPosition());
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