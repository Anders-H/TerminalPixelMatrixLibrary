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
        var startPosition = GetStartPosition(enterPressedAt);
        startPosition = MoveToContent(startPosition, enterPressedAt);
        var endPosition = GetEndPosition(enterPressedAt);
        endPosition = MoveToEndContent(endPosition, enterPressedAt);

#if DEBUG
        System.Diagnostics.Debug.WriteLine($@"Enter pressed at {enterPressedAt.X}, {enterPressedAt.Y} and input is started at {startPosition.X}, {startPosition.Y}.");
#endif

        inputStart = Coordinate.Home();
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
}