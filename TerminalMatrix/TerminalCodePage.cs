namespace TerminalMatrix;

public class TerminalCodePage
{
    public Dictionary<int, char> Chr { get; }
    public Dictionary<char, int> Asc { get; }

    public TerminalCodePage()
    {
        Chr = new Dictionary<int, char>
        {
            { 0, ' ' },
            { 1, 'A' },
            { 2, 'B' },
            { 3, 'C' }
        };

        Asc = new Dictionary<char, int>
        {
            { ' ', 0 },
            { 'A', 1 },
            { 'B', 2 },
            { 'C', 3 }
        };
    }
}