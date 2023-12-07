namespace TerminalMatrix;

public class TerminalCodePage
{
    public const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ,.;:!\"@#$%&/\\*-+'()";
    public Dictionary<int, char> Chr { get; }
    public Dictionary<char, int> Asc { get; }

    public TerminalCodePage()
    {
        Chr = new Dictionary<int, char>();

        foreach (var character in Characters)
            Chr.Add(character, character);

        Asc = new Dictionary<char, int>();

        foreach (var c in Chr)
            Asc.Add(c.Value, c.Key);
    }
}