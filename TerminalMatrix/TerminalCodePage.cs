namespace TerminalMatrix;

public class TerminalCodePage
{
    public Dictionary<int, char> Chr { get; }
    public Dictionary<char, int> Asc { get; }

    public TerminalCodePage()
    {
        Chr = new Dictionary<int, char>
        {
            { 32, ' ' },
            { 65, 'A' },
            { 66, 'B' },
            { 67, 'C' }
        };

        Asc = new Dictionary<char, int>
        {
            { ' ', 32 },
            { 'A', 65 },
            { 'B', 66 },
            { 'C', 67 }
        };
    }
}