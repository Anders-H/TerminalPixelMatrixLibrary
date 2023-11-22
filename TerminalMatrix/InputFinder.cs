using System.Text;

namespace TerminalMatrix;

public class InputFinder
{
    private readonly int[,] _chars;
    private readonly CoordinateList _inputStart;

    public InputFinder(int[,] chars, CoordinateList inputStart)
    {
        _chars = chars;
        _inputStart = inputStart;
    }

    public string GetInput(Coordinate enterPressedAt, out Coordinate inputStart)
    {
        inputStart = null;

        var result = new StringBuilder();

        return result.ToString();
    }
}