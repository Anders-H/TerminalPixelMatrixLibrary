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
    private readonly TerminalFont _font;
    private readonly TerminalCodePage _codePage;
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
        _font = new TerminalFont();
        _codePage = new TerminalCodePage();
        _palette = new Palette();
        InitializeComponent();
    }

    private void UserControl1_Load(object sender, EventArgs e)
    {
        DoubleBuffered = true;
        Clear();
    }

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

    public void HorizontalLine(int x1, int y, int x2, ColorName color)
    {
        var c = (int)color;

        for (var x = x1; x <= x2; x++)
            _pixelMap[x, y] = c;
    }

    public void VerticalLine(int x, ColorName color)
    {
        var c = (int)color;

        for (var y = 0; y < PixelsHeight; y++)
            _pixelMap[x, y] = c;
    }

    public void VerticalLine(int x, int y1, int y2, ColorName color)
    {
        var c = (int)color;

        for (var y = y1; y <= y2; y++)
            _pixelMap[x, y] = c;
    }

    public void Box(ColorName color, int x1, int y1, int x2, int y2)
    {
        var c = (int)color;

        for (var x = x1; x <= x2; x++)
        {
            _pixelMap[x, y1] = c;
            _pixelMap[x, y2] = c;
        }

        for (var y = y1 + 1; y < y2; y++)
        {
            _pixelMap[x1, y] = c;
            _pixelMap[x2, y] = c;
        }
    }

    public void PrintAt(ColorName color, int x, int y, string text)
    {
        var c = (int)color;

        for (var i = 0; i < text.Length; i++)
        {
            _characterColorMap[x + i, y] = c;
            _characterMap[x + i, y] = _codePage.Asc[text[i]];
        }
    }

    public void UpdateBitmap()
    {
        var bitsHandle = GCHandle.Alloc(_bitmap, GCHandleType.Pinned);
        Bitmap = new Bitmap(PixelsWidth, PixelsHeight, PixelsWidth * 4, PixelFormat.Format32bppArgb, bitsHandle.AddrOfPinnedObject());

        unsafe
        {
            var data = Bitmap.LockBits(new Rectangle(0, 0, PixelsWidth, PixelsHeight), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            for (var y = 0; y < PixelsHeight; y++)
            {
                for (var x = 0; x < PixelsWidth; x++)
                {
                    var index = x + y * PixelsWidth;
                    _bitmap[index] = _palette.GetColor(_pixelMap[x, y]).ToArgb();
                }
            }

            for (var y = 0; y < CharactersHeight; y++)
            {
                for (var x = 0; x < CharactersWidth; x++)
                {
                    if (_characterMap[x, y] > 0)
                    {
                        var characterFont = _font[_characterMap[x, y]];
                        var c = _palette.GetColor(_characterColorMap[x, y]).ToArgb();
                        var pixelXStart = x * 8;
                        var pixelYStart = y * 8;
                        var sourceX = 0;
                        var sourceY = 0;
                        for (var pixelY = pixelYStart; pixelY < pixelYStart + 8; pixelY++)
                        {
                            for (var pixelX = pixelXStart; pixelX < pixelXStart + 8; pixelX++)
                            {
                                var index = pixelX + pixelY * PixelsWidth;
                                if (characterFont.Pixels[sourceX, sourceY])
                                    _bitmap[index] = c;

                                sourceX++;
                            }

                            sourceX = 0;
                            sourceY++;
                        }
                    }
                }
            }

            Bitmap.UnlockBits(data);
            bitsHandle.Free();
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