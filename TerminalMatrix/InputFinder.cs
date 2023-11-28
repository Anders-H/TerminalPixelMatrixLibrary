using System.Text;
using TerminalMatrix.Definitions;

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
        var result = new StringBuilder();
        var lastLine = enterPressedAt.Y >= CharacterMatrixDefinition.Height - 1;

        Coordinate? nextEnterPress = null;

        if (!lastLine)
            nextEnterPress = GetNextFrom(enterPressedAt);

        inputStart = enterPressedAt.Copy();

        do
        {
            if (_inputStart.HitTest(inputStart))
                break;

        } while (inputStart.MovePrevious());

        if (inputStart.IsSame(enterPressedAt))
            return "";

        var counter = inputStart.Copy();

        do
        {
            result.Append((char)_chars[counter.X, counter.Y]);
            counter.MoveNext();
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
        } while (counter < enterPressedAt);

        if (lastLine || nextEnterPress != null)
        {
            nextEnterPress = inputStart.Copy();
            bool repeat;
            do
            {
                repeat = nextEnterPress.MoveNext();
                result.Append((char)_chars[nextEnterPress.X, nextEnterPress.Y]);

            } while (repeat);
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