using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TerminalMatrix;

public partial class TerminalMatrixControl : UserControl
{
    private readonly int[,] _characterColorMap;
    private readonly int[,] _characterMap;
    private readonly int[,] _pixelMap;
    private readonly int[] _bitmap;
    private bool _cursorVisibleBlink;
    private readonly System.Windows.Forms.Timer _timer = new();
    // ReSharper disable once CollectionNeverUpdated.Local
    private readonly TerminalFont _font = new();
    private readonly TerminalCodePage _codePage;
    private readonly Palette _palette;
    public const int PixelsWidth = 640;
    public const int PixelsHeight = 200;
    public const int CharactersWidth = 80;
    public const int CharactersHeight = 25;
    public Bitmap? Bitmap { get; private set; }
    public int CursorX { get; set; }
    public int CursorY { get; set; }
    public int CurrentCursorColor { get; set; }

    public TerminalMatrixControl()
    {
        _characterColorMap = new int[CharactersWidth, CharactersHeight];
        _characterMap = new int[CharactersWidth, CharactersHeight];
        _pixelMap = new int[PixelsWidth, PixelsHeight];
        _bitmap = new int[PixelsWidth * PixelsHeight];
        _cursorVisibleBlink = false;
        _codePage = new TerminalCodePage();
        _palette = new Palette();
        CurrentCursorColor = (int)ColorName.Green;
        _timer.Interval = 1000;
        InitializeComponent();
    }

    private void UserControl1_Load(object sender, EventArgs e)
    {
        DoubleBuffered = true;
        _timer.Tick += Blink;
        _timer.Enabled = true;
        Clear();
    }

    private void Blink(object? sender, EventArgs e)
    {
        _cursorVisibleBlink = !_cursorVisibleBlink;
        UpdateBitmap();
        Invalidate();
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
                var characterFont = _font[_characterMap[x, y]];
                var c = _palette.GetColor(ColorName.Green).ToArgb();
                var pixelXStart = x * 8;
                var pixelYStart = y * 8;
                var sourceX = 0;
                var sourceY = 0;

                if (x == CursorX && y == CursorY)
                {
                    for (var pixelY = pixelYStart; pixelY < pixelYStart + 8; pixelY++)
                    {
                        for (var pixelX = pixelXStart; pixelX < pixelXStart + 8; pixelX++)
                        {
                            var index = pixelX + pixelY * PixelsWidth;
                            if (_cursorVisibleBlink)
                                _bitmap[index] = characterFont.Pixels[sourceX, sourceY] ? c : 0;
                            else
                                _bitmap[index] = characterFont.Pixels[sourceX, sourceY] ? 0 : c;

                            sourceX++;
                        }

                        sourceX = 0;
                        sourceY++;
                    }
                }
                else if (_characterMap[x, y] > 0)
                {
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

    private void ShowKeyboardActivity()
    {
        _timer.Enabled = false;
        _cursorVisibleBlink = false;
        _timer.Enabled = true;
        UpdateBitmap();
        Invalidate();
    }

    private void TypeCharacter(char c)
    {
        
    }

    protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Tab:
            case Keys.Up:
            case Keys.Right:
            case Keys.Down:
            case Keys.Left:
                e.IsInputKey = true;
                break;
        }

        base.OnPreviewKeyDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Back: // Backspace
                break;
            case Keys.Return: // ...and Enter
                break;
            case Keys.Escape:
                break;
            case Keys.IMEConvert:
                break;
            case Keys.IMENonconvert:
                break;
            case Keys.IMEAccept:
                break;
            case Keys.IMEModeChange:
                break;
            case Keys.Space:
                TypeCharacter(' ');
                break;
            case Keys.Prior:
                break;
            case Keys.Next:
                break;
            case Keys.End:
                break;
            case Keys.Home:
                break;
            case Keys.Left:
                if (CursorX > 0)
                {
                    CursorX--;
                    ShowKeyboardActivity();
                }
                else if (CursorY > 0)
                {
                    CursorX = CharactersWidth - 1;
                    CursorY--;
                    ShowKeyboardActivity();
                }
                break;
            case Keys.Up:
                if (CursorY > 0)
                {
                    CursorY--;
                    ShowKeyboardActivity();
                }
                break;
            case Keys.Right:
                if (CursorX < CharactersWidth - 1)
                {
                    CursorX++;
                    ShowKeyboardActivity();
                }
                else if (CursorY < CharactersHeight - 1)
                {
                    CursorX = 0;
                    CursorY++;
                    ShowKeyboardActivity();
                }
                break;
            case Keys.Down:
                if (CursorY < CharactersHeight - 1)
                {
                    CursorY++;
                    ShowKeyboardActivity();
                }
                else
                {
                    ScrollCharactersUp();
                    ShowKeyboardActivity();
                }
                break;
            case Keys.Insert:
                break;
            case Keys.Delete:
                break;
            case Keys.Help:
                break;
            case Keys.D0:
                break;
            case Keys.D1:
                break;
            case Keys.D2:
                break;
            case Keys.D3:
                break;
            case Keys.D4:
                break;
            case Keys.D5:
                break;
            case Keys.D6:
                break;
            case Keys.D7:
                break;
            case Keys.D8:
                break;
            case Keys.D9:
                break;
            case Keys.A:
                TypeCharacter('A');
                break;
            case Keys.B:
                TypeCharacter('B');
                break;
            case Keys.C:
                TypeCharacter('C');
                break;
            case Keys.D:
                break;
            case Keys.E:
                break;
            case Keys.F:
                break;
            case Keys.G:
                break;
            case Keys.H:
                break;
            case Keys.I:
                break;
            case Keys.J:
                break;
            case Keys.K:
                break;
            case Keys.L:
                break;
            case Keys.M:
                break;
            case Keys.N:
                break;
            case Keys.O:
                break;
            case Keys.P:
                break;
            case Keys.Q:
                break;
            case Keys.R:
                break;
            case Keys.S:
                break;
            case Keys.T:
                break;
            case Keys.U:
                break;
            case Keys.V:
                break;
            case Keys.W:
                break;
            case Keys.X:
                break;
            case Keys.Y:
                break;
            case Keys.Z:
                break;
            case Keys.NumPad0:
                break;
            case Keys.NumPad1:
                break;
            case Keys.NumPad2:
                break;
            case Keys.NumPad3:
                break;
            case Keys.NumPad4:
                break;
            case Keys.NumPad5:
                break;
            case Keys.NumPad6:
                break;
            case Keys.NumPad7:
                break;
            case Keys.NumPad8:
                break;
            case Keys.NumPad9:
                break;
            case Keys.Multiply:
                break;
            case Keys.Add:
                break;
            case Keys.Separator:
                break;
            case Keys.Subtract:
                break;
            case Keys.Decimal:
                break;
            case Keys.Divide:
                break;
            case Keys.F1:
                break;
            case Keys.F2:
                break;
            case Keys.F3:
                break;
            case Keys.F4:
                break;
            case Keys.F5:
                break;
            case Keys.F6:
                break;
            case Keys.F7:
                break;
            case Keys.F8:
                break;
            case Keys.F9:
                break;
            case Keys.F10:
                break;
            case Keys.F11:
                break;
            case Keys.F12:
                break;
            case Keys.F13:
                break;
            case Keys.F14:
                break;
            case Keys.F15:
                break;
            case Keys.F16:
                break;
            case Keys.F17:
                break;
            case Keys.F18:
                break;
            case Keys.F19:
                break;
            case Keys.F20:
                break;
            case Keys.F21:
                break;
            case Keys.F22:
                break;
            case Keys.F23:
                break;
            case Keys.F24:
                break;
            case Keys.NumLock:
                break;
            case Keys.Scroll:
                break;
            case Keys.LShiftKey:
                break;
            case Keys.RShiftKey:
                break;
            case Keys.LControlKey:
                break;
            case Keys.RControlKey:
                break;
            case Keys.LMenu:
                break;
            case Keys.RMenu:
                break;
            case Keys.OemSemicolon:
                break;
            case Keys.Oemplus:
                break;
            case Keys.Oemcomma:
                break;
            case Keys.OemMinus:
                break;
            case Keys.OemPeriod:
                break;
            case Keys.OemQuestion:
                break;
            case Keys.Oemtilde:
                break;
            case Keys.OemOpenBrackets:
                break;
            case Keys.OemPipe:
                break;
            case Keys.OemCloseBrackets:
                break;
            case Keys.OemQuotes:
                break;
            case Keys.Oem8:
                break;
            case Keys.OemBackslash:
                break;
        }
        base.OnKeyDown(e);
    }

    private void ScrollCharactersUp()
    {

    }
}