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
    public event InputCompletedDelegate? InputCompleted;

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
    private TerminalState TerminalState { get; }
    private readonly TerminalMatrixKeypressHandler _keypressHandler;
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
        TerminalState = new TerminalState();
        _keypressHandler = new TerminalMatrixKeypressHandler(this);
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

#if DEBUG
        foreach (var c in InputStart)
        {
            _bitmap[c.X * 8 + c.Y * 8 * PixelMatrixDefinition.Width] = _cursorVisibleBlink ? Color.Purple.ToArgb() : Color.White.ToArgb();
        }
#endif
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
        else if (!TerminalState.InputMode && CursorPosition.Y < CharacterMatrixDefinition.Height - 1)
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
        _keypressHandler.HandleKeyDown(e, TerminalState.InputMode, TypeCharacter, CursorPosition, ShowKeyboardActivity, Show);
        base.OnKeyDown(e);
    }

    internal void HandleEnter()
    {
        var inputValue = new InputFinder(_characterMap, InputStart)
            .GetInput(CursorPosition, out var inputStart, out var foundTerminator);

        var eventArgs = new TypedLineEventArgs(inputStart, CursorPosition, inputValue);
        
        if (TerminalState.InputMode)
        {
            InputCompleted?.Invoke(this, eventArgs);
        }
        else
        {
            TypedLine?.Invoke(this, eventArgs);
        }


        if (!eventArgs.CancelNewLine)
        {
            CursorPosition.X = 0;

            if (CursorPosition.CanMoveDown())
                CursorPosition.Y++;
            else
                Scroll();
        }

        if (!foundTerminator)
            InputStart.Add(CursorPosition.Copy());

        _timer.Enabled = false;
        _cursorVisibleBlink = true;
        ShowKeyboardActivity();
        _timer.Enabled = true;
    }

    private new void Scroll()
    {
        ScrollCharacterMap(_characterColorMap, 0);
        ScrollCharacterMap(_characterMap, ' ');

        foreach (var c in InputStart)
        {
            c.Scroll();
        }

        var again = false;

        do
        {
            foreach (var c in InputStart.Where(c => c.Y < 0))
            {
                InputStart.Remove(c);
                again = true;
                break;
            }
        } while (again);
    }

    private void ScrollCharacterMap(int[,] characterMap, int blank)
    {
        for (var y = 1; y < CharacterMatrixDefinition.Height; y++)
            for (var x = 0; x < CharacterMatrixDefinition.Width; x++)
                characterMap[x, y - 1] = characterMap[x, y];

        const int last = CharacterMatrixDefinition.Height - 1;

        for (var x = 0; x < CharacterMatrixDefinition.Width; x++)
            characterMap[x, last] = blank;
    }
}