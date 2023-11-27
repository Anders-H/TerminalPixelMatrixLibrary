namespace TerminalMatrix.Events;

public class TypedLineEventArgs : EventArgs
{
    public Coordinate InputStart { get; }
    public Coordinate InputEnd { get; }
    public string InputValue { get; }
    public bool CancelNewLine { get; set; }

    public TypedLineEventArgs(Coordinate inputStart, Coordinate inputEnd, string inputValue)
    {
        InputStart = inputStart;
        InputEnd = inputEnd;
        InputValue = inputValue;
        CancelNewLine = false;
    }
}