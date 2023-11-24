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
        inputStart = enterPressedAt.Copy();
        do
        {
            if (_inputStart.HitTest(inputStart))
                break;

        } while (inputStart.MovePrevious());

        if (inputStart.IsSame(enterPressedAt))
            return "";

        var result = new StringBuilder();
        var counter = inputStart.Copy();

        do
        {
            result.Append((char)_chars[counter.X, counter.Y]);
            counter.MoveNext();
        } while (counter.MoveNext());

        return result.ToString();
    }
}