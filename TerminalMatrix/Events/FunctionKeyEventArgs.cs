namespace TerminalMatrix.Events;

public class FunctionKeyEventArgs : EventArgs
{
    public FunctionKey Key { get; }

    public FunctionKeyEventArgs(FunctionKey key)
    {
        Key = key;
    }
}