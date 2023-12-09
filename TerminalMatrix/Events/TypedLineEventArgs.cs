namespace TerminalMatrix.Events;

public class TypedLineEventArgs : EventArgs
{
    public string InputValue { get; }

    public TypedLineEventArgs(string inputValue)
    {
        InputValue = inputValue;
    }
}