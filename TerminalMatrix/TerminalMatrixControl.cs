using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TerminalMatrix.Definitions;
using TerminalMatrix.Events;
using TerminalMatrix.TerminalColor;

namespace TerminalMatrix;

public partial class TerminalMatrixControl : UserControl
{
    public event TypedLineDelegate? TypedLine;

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
    private bool _isInInputState;
    public Bitmap? Bitmap { get; private set; }
    public Coordinate CursorPosition { get; }
    public CoordinateList InputStart { get; }
    public int CurrentCursorColor { get; set; }

    public TerminalMatrixControl()
    {
        CursorPosition = new Coordinate(0, 0);
        InputStart = new CoordinateList(0, 0);
        _characterColorMap = CharacterMatrixDefinition.Create();
        _characterMap = CharacterMatrixDefinition.Create();
        _pixelMap = PixelMatrixDefinition.Create();
        _bitmap = PixelMatrixDefinition.CreateBitmap();
        _cursorVisibleBlink = false;
        _codePage = new TerminalCodePage();
        _palette = new Palette();
        _isInInputState = false;
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

    public void SetStartPosition(int x, int y)
    {
        CursorPosition.Set(x, y);
        InputStart.Clear();
        InputStart.Add(x, y);
    }
    
    private void Blink(object? sender, EventArgs e)
    {
        _cursorVisibleBlink = !_cursorVisibleBlink;
        UpdateBitmap();
        Invalidate();
    }

    public void ClearColorMap()
    {
        for (var y = 0; y < CharacterMatrixDefinition.Height; y++)
            for (var x = 0; x < CharacterMatrixDefinition.Width; x++)
                _characterColorMap[x, y] = 1;
    }

    public void ClearCharacterMap()
    {
        for (var y = 0; y < CharacterMatrixDefinition.Height; y++)
            for (var x = 0; x < CharacterMatrixDefinition.Width; x++)
                _characterMap[x, y] = 32;
    }

    public void ClearPixelMap()
    {
        for (var y = 0; y < PixelMatrixDefinition.Height; y++)
            for (var x = 0; x < PixelMatrixDefinition.Width; x++)
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

        for (var x = 0; x < PixelMatrixDefinition.Width; x++)
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

        for (var y = 0; y < PixelMatrixDefinition.Height; y++)
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
        Bitmap = new Bitmap(PixelMatrixDefinition.Width, PixelMatrixDefinition.Height, PixelMatrixDefinition.Width * 4, PixelFormat.Format32bppArgb, bitsHandle.AddrOfPinnedObject());

        var data = Bitmap.LockBits(new Rectangle(0, 0, PixelMatrixDefinition.Width, PixelMatrixDefinition.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

        for (var y = 0; y < PixelMatrixDefinition.Height; y++)
        {
            for (var x = 0; x < PixelMatrixDefinition.Width; x++)
            {
                var index = x + y * PixelMatrixDefinition.Width;
                _bitmap[index] = _palette.GetColor(_pixelMap[x, y]).ToArgb();
            }
        }

        for (var y = 0; y < CharacterMatrixDefinition.Height; y++)
        {
            for (var x = 0; x < CharacterMatrixDefinition.Width; x++)
            {
                var characterFont = _font[_characterMap[x, y]];
                var c = _palette.GetColor(ColorName.Green).ToArgb();
                var pixelStart = new Coordinate(x * 8, y * 8);
                var source = new Coordinate(0, 0);

                if (CursorPosition.IsSame(x, y))
                {
                    for (var pixelY = pixelStart.Y; pixelY < pixelStart.Y + 8; pixelY++)
                    {
                        for (var pixelX = pixelStart.X; pixelX < pixelStart.X + 8; pixelX++)
                        {
                         
                            var index = pixelX + pixelY * PixelMatrixDefinition.Width;
                            
                            if (_cursorVisibleBlink)
                                _bitmap[index] = characterFont.Pixels[source.X, source.Y] ? c : 0;
                            else
                                _bitmap[index] = characterFont.Pixels[source.X, source.Y] ? 0 : c;

                            source.X++;
                        }

                        source.NextRow();
                    }
                }
                else if (_characterMap[x, y] > 0)
                {
                    for (var pixelY = pixelStart.Y; pixelY < pixelStart.Y + 8; pixelY++)
                    {
                        for (var pixelX = pixelStart.X; pixelX < pixelStart.X + 8; pixelX++)
                        {
                            var index = pixelX + pixelY * PixelMatrixDefinition.Width;
                            
                            if (characterFont.Pixels[source.X, source.Y])
                                _bitmap[index] = c;

                            source.X++;
                        }

                        source.NextRow();
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
        e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
        e.Graphics.InterpolationMode = InterpolationMode.Low; // This should be an option
        e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
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
        _characterMap[CursorPosition.X, CursorPosition.Y] = c;
        _characterColorMap[CursorPosition.X, CursorPosition.Y] = CurrentCursorColor;

        if (CursorPosition.X < CharacterMatrixDefinition.Width - 1)
        {
            CursorPosition.X++;
            ShowKeyboardActivity();
        }
        else if (_isInInputState && CursorPosition.Y < CharacterMatrixDefinition.Height - 1)
        {
            CursorPosition.X = 0;
            CursorPosition.Y++;
            ShowKeyboardActivity();
        }

        ShowKeyboardActivity();
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
#if DEBUG
        System.Diagnostics.Debug.WriteLine($@"Keypress: {e.KeyCode} ({e.KeyValue})");
#endif
        switch (e.KeyCode)
        {
            case Keys.Back: // Backspace
                break;
            case Keys.Return: // ...and Enter


                if (_isInInputState)
                {

                }
                else
                {
                    var inputValue = new InputFinder(_characterMap, InputStart)
                        .GetInput(CursorPosition, out var inputStart);

                    if (CursorPosition > inputStart && !string.IsNullOrEmpty(inputValue))
                        TypedLine?.Invoke(this, new TypedLineEventArgs(inputStart, CursorPosition, inputValue));
                }

                InputStart.Add(CursorPosition.Copy());
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
                if (CursorPosition.X > 0)
                {
                    CursorPosition.X--;
                    ShowKeyboardActivity();
                }
                else if (CursorPosition.Y > 0)
                {
                    CursorPosition.X = CharacterMatrixDefinition.Width - 1;
                    CursorPosition.Y--;
                    ShowKeyboardActivity();
                }
                break;
            case Keys.Up:
                if (CursorPosition.Y > 0)
                {
                    CursorPosition.Y--;
                    ShowKeyboardActivity();
                }
                break;
            case Keys.Right:
                if (CursorPosition.X < CharacterMatrixDefinition.Width - 1)
                {
                    CursorPosition.X++;
                    ShowKeyboardActivity();
                }
                else if (CursorPosition.Y < CharacterMatrixDefinition.Height - 1)
                {
                    CursorPosition.X = 0;
                    CursorPosition.Y++;
                    ShowKeyboardActivity();
                }
                break;
            case Keys.Down:
                if (CursorPosition.Y < CharacterMatrixDefinition.Height - 1)
                {
                    CursorPosition.Y++;
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
                TypeCharacter('D');
                break;
            case Keys.E:
                TypeCharacter('E');
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
            case Keys.OemMinus:
                TypeCharacter('-');
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