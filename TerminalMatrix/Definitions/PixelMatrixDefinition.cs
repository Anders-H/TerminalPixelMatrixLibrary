namespace TerminalMatrix.Definitions;

public static class PixelMatrixDefinition
{
    public const int Width = 640;
    public const int Height = 200;

    public static int[,] Create() =>
        new int[Width, Height];

    public static int[] CreateBitmap() =>
        new int[Width * Height];
}