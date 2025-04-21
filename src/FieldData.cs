namespace Minesweeper;

public class FieldData {
    public int Width { get; set; }
    public int Height { get; set; }
    public int MinesPercentage { get; set; }

    public void Display() {
        Console.WriteLine($"Width: {Width}\nHeight: {Height}\nMinesPercentage: {MinesPercentage}");
    }
}