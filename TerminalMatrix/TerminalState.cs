namespace TerminalMatrix;

public class TerminalState
{
    public bool DirectMode { get; set; }
    public bool InputMode { get; set; }

    public TerminalState()
    {
        DirectMode = true;
        InputMode = false;
    }
}