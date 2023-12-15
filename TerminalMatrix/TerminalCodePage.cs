namespace TerminalMatrix;

public class TerminalCodePage
{
    public const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ,.;:!?\"@#$%&/\\*-+'()<>=";
    public Dictionary<byte, char> Chr { get; }
    public Dictionary<char, byte> Asc { get; }

    public TerminalCodePage()
    {
        Chr = new Dictionary<byte, char>();

        foreach (var character in Characters)
            Chr.Add((byte)character, character);

        Asc = new Dictionary<char, byte>();

        foreach (var c in Chr)
            Asc.Add(c.Value, c.Key);
    }
}