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

    public string GetInput(Coordinate enterPressedAt, out Coordinate inputStart, out Coordinate inputEnd, out bool foundTerminator)
    {
        var startPosition = GetStartPosition(enterPressedAt);
        startPosition = MoveToContent(startPosition, enterPressedAt);
        var endPosition = GetEndPosition(enterPressedAt);
        endPosition = MoveToEndContent(endPosition, enterPressedAt);

        inputStart = startPosition;
        inputEnd = endPosition;
        foundTerminator = false;
        return "";
    }

    private Coordinate GetStartPosition(Coordinate enterPressedAt)
    {
        if (_inputStart.HitTest(enterPressedAt))
            return enterPressedAt;

        var p = enterPressedAt.Copy();

        do
        {
            if (!p.MovePrevious())
                return p;

            if (_inputStart.HitTest(p))
                return p;

        } while (true);
    }

    private Coordinate GetEndPosition(Coordinate enterPressedAt)
    {
        if (_inputStart.HitTest(enterPressedAt))
            return enterPressedAt;

        var p = enterPressedAt.Copy();

        do
        {
            if (!p.MoveNext())
                return p;

            if (_inputStart.HitTest(p))
                return p;

        } while (true);
    }

    private Coordinate MoveToContent(Coordinate currentPosition, Coordinate enterPressedAt)
    {
        while (currentPosition < enterPressedAt && _chars[currentPosition.X, currentPosition.Y] == ' ')
        {
            if (!currentPosition.MoveNext())
                break;
        }

        return currentPosition;
    }

    private Coordinate MoveToEndContent(Coordinate currentPosition, Coordinate enterPressedAt)
    {
        while (currentPosition >= enterPressedAt && _chars[currentPosition.X, currentPosition.Y] == ' ')
        {
            if (!currentPosition.MovePrevious())
                break;
        }

        return currentPosition;
    }
}