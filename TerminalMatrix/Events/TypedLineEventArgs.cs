namespace TerminalMatrix.Events;

public class TypedLineEventArgs : EventArgs
{
    public string InputValue { get; }
    public bool CancelNewLine { get; set; }

    public TypedLineEventArgs(string inputValue)
    {
        InputValue = inputValue;
        CancelNewLine = false;
    }
}