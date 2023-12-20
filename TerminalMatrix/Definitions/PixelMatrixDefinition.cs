namespace TerminalMatrix.Definitions;

public static class PixelMatrixDefinition
{
    public static int Width;
    public const int Height = 200;

    static PixelMatrixDefinition()
    {
        Width = 320;
    }

    public static byte[,] Create(Resolution resolution)
    {
        switch (resolution)
        {
            case Resolution.Pixels640x200Characters80x25:
                Width = 640;
                break;
            case Resolution.Pixels480x200Characters60x25:
                Width = 480;
                break;
            case Resolution.Pixels320x200Characters40x25:
                Width = 320;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
        }

        return new byte[Width, Height];
    }
    
    public static int[] CreateBitmap() =>
        new int[Width * Height];
}