namespace TerminalMatrix.Definitions;

public static class CharacterMatrixDefinition
{
    public const int Width = 80;
    public const int Height = 25;
    public const int CharacterEmpty = 32;
    public static int TextRenderLimit { get; set; }

    static CharacterMatrixDefinition()
    {
        TextRenderLimit = 0;
    }

    public static byte[,] Create() =>
        new byte[Width, Height];
}