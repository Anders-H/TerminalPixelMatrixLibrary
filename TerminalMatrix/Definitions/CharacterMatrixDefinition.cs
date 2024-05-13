namespace TerminalMatrix.Definitions;

public static class CharacterMatrixDefinition
{
    public static int Width;
    public const int Height = 25;
    public const int CharacterEmpty = 32;
    public static int TextRenderLimit { get; set; }

    static CharacterMatrixDefinition()
    {
        Width = 40;
        TextRenderLimit = 0;
    }

    public static byte[,] Create(Resolution resolution)
    {
        switch (resolution)
        {
            case Resolution.Pixels640x200Characters80x25:
                Width = 80;
                break;
            case Resolution.Pixels480x200Characters60x25:
                Width = 60;
                break;
            case Resolution.Pixels320x200Characters40x25:
                Width = 40;
                break;
            case Resolution.Pixels160x200Characters20x25:
                Width = 20;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
        }

        return new byte[Width, Height];
    }
}