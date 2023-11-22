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
            { 45, '-' },
            { 65, 'A' },
            { 66, 'B' },
            { 67, 'C' },
            { 68, 'D' },
            { 69, 'E' }
        };

        Asc = new Dictionary<char, int>
        {
            { ' ', 32 },
            { '-', 45 },
            { 'A', 65 },
            { 'B', 66 },
            { 'C', 67 },
            { 'D', 68 },
            { 'E', 69 }
        };
    }
}