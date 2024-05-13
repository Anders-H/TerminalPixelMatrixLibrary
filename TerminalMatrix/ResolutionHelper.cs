namespace TerminalMatrix;

public static class ResolutionHelper
{
    public static Size GetPixelSize(Resolution resolution) =>
        resolution switch
        {
            Resolution.Pixels640x200Characters80x25 => new Size(640, 200),
            Resolution.Pixels480x200Characters60x25 => new Size(480, 200),
            Resolution.Pixels320x200Characters40x25 => new Size(320, 200),
            Resolution.Pixels160x200Characters20x25 => new Size(160, 200),
            _ => throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null)
        };

    public static Size GetCharacterMapSize(Resolution resolution) =>
        resolution switch
        {
            Resolution.Pixels640x200Characters80x25 => new Size(80, 25),
            Resolution.Pixels480x200Characters60x25 => new Size(60, 25),
            Resolution.Pixels320x200Characters40x25 => new Size(40, 25),
            Resolution.Pixels160x200Characters20x25 => new Size(20, 25),
            _ => throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null)
        };

    public static Bitmap GetBitmap(Resolution resolution)
    {
        var size = GetPixelSize(resolution);
        return new Bitmap(size.Width, size.Height);
    }
}