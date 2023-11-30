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

    public string GetInput(Coordinate enterPressedAt, out Coordinate inputStart, out bool foundTerminator)
    {
        foundTerminator = false;
        var result = new StringBuilder();
        inputStart = enterPressedAt.Copy();

        while (!_inputStart.HitTest(inputStart))
        {
            if (!inputStart.MovePrevious())
                break;
        }

        var counter = inputStart.Copy();
        result.Append((char)_chars[counter.X, counter.Y]);

        while (counter.MoveNext())
        {
            if (_inputStart.HitTest(counter))
            {
                foundTerminator = true;
                break;
            }

            result.Append((char)_chars[counter.X, counter.Y]);
        }

        return result.ToString().Trim();
    }

    private Coordinate? GetNextFrom(Coordinate start)
    {
        var p = start.Copy();

        while (p.MoveNext())
        {
            if (_inputStart.HitTest(p))
                return p;
        }

        return p.AtEnd(_inputStart) ? null : p;
    }
}