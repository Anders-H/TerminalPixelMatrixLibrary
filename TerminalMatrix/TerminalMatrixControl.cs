using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TerminalMatrix;

public partial class TerminalMatrixControl : UserControl
{
    private int[,] _characterColorMap;
    private int[,] _characterMap;
    private int[,] _pixelMap;
    private int[] _bitmap;
    private readonly Palette _palette;
    public const int PixelsWidth = 640;
    public const int PixelsHeight = 200;
    public const int CharactersWidth = 80;
    public const int CharactersHeight = 25;
    public Bitmap? Bitmap { get; private set; }

    public TerminalMatrixControl()
    {
        _characterColorMap = new int[CharactersWidth, CharactersHeight];
        _characterMap = new int[CharactersWidth, CharactersHeight];
        _pixelMap = new int[PixelsWidth, PixelsHeight];
        _bitmap = new int[PixelsWidth * PixelsHeight];
        _palette = new Palette();
        InitializeComponent();
    }

    private void UserControl1_Load(object sender, EventArgs e) =>
        Clear();

    public void ClearColorMap()
    {
        for (var y = 0; y < CharactersHeight; y++)
            for (var x = 0; x < CharactersWidth; x++)
                _characterColorMap[x, y] = 1;
    }

    public void ClearCharacterMap()
    {
        for (var y = 0; y < CharactersHeight; y++)
            for (var x = 0; x < CharactersWidth; x++)
                _characterMap[x, y] = 0;
    }

    public void ClearPixelMap()
    {
        for (var y = 0; y < PixelsHeight; y++)
            for (var x = 0; x < PixelsWidth; x++)
                _pixelMap[x, y] = 0;
    }

    public void Clear()
    {
        ClearColorMap();
        ClearCharacterMap();
        ClearPixelMap();
    }

    public void HorizontalLine(int y, ColorName color)
    {
        var c = (int)color;

        for (var x = 0; x < PixelsWidth; x++)
            _pixelMap[x, y] = c;
    }

    public void UpdateBitmap()
    {
        var bitsHandle = GCHandle.Alloc(_bitmap, GCHandleType.Pinned);
        Bitmap = new Bitmap(PixelsWidth, PixelsHeight, PixelsWidth * 4, PixelFormat.Format32bppArgb, bitsHandle.AddrOfPinnedObject());
        var data = Bitmap.LockBits(new Rectangle(0, 0, PixelsWidth, PixelsHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

        unsafe
        {
            var bytes = (byte*)data.Scan0;

            for (var y = 0; y < PixelsHeight; y++)
            {
                for (var x = 0; x < PixelsWidth; x++)
                {
                    var index = x + y * PixelsWidth;
                    _bitmap[index] = _palette.GetColor(_pixelMap[x, y]).ToArgb();
                }

                bytes++;
            }

            Bitmap.UnlockBits(data);
            bitsHandle.Free();

            // TODO: Characters
        }

        Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
        Invalidate();
        base.OnResize(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (Bitmap == null)
            return;

        e.Graphics.CompositingMode = CompositingMode.SourceCopy;
        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.SmoothingMode = SmoothingMode.None;
        e.Graphics.DrawImage(Bitmap, 0, 0, Width, Height);
        base.OnPaint(e);
    }
}