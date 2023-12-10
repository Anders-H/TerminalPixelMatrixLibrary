using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
    public int CurrentCursorColor { get; set; }
    public ProgramLineDictionary ProgramLines { get; }

    public TerminalMatrixControl()
    {
        CursorPosition = new Coordinate(0, 0);
        _characterColorMap = CharacterMatrixDefinition.Create();
        _characterMap = CharacterMatrixDefinition.Create();
        _pixelMap = PixelMatrixDefinition.Create();
        _bitmap = PixelMatrixDefinition.CreateBitmap();
        _cursorVisibleBlink = false;
        _codePage = new TerminalCodePage();
        _palette = new Palette();
        TerminalState = new TerminalState();
        _keypressHandler = new TerminalMatrixKeypressHandler(this);
        CurrentCursorColor = (int)ColorName.White;
        _timer.Interval = 1000;
        ProgramLines = new ProgramLineDictionary();
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
                _characterColorMap[x, y] = CurrentCursorColor;
    }

    public void ClearCharacterMap()
    {
        for (var y = 0; y < CharacterMatrixDefinition.Height; y++)
            for (var x = 0; x < CharacterMatrixDefinition.Width; x++)
                _characterMap[x, y] = CharacterMatrixDefinition.CharacterEmpty;
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
                var c = _palette.GetColor(_characterColorMap[x, y]).ToArgb();
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
        // Needed for multiline support, not supported now.
        //else if (!TerminalState.InputMode && CursorPosition.Y < CharacterMatrixDefinition.Height - 1)
        //{
        //    CursorPosition.X = 0;
        //    CursorPosition.Y++;
        //    ShowKeyboardActivity();
        //}

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

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        _keypressHandler.HandleKeyPress(e, TerminalState.InputMode, TypeCharacter, CursorPosition, ShowKeyboardActivity, Show);
        base.OnKeyPress(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        switch (e.KeyData)
        {
            case Keys.PageUp:

                if (TerminalState.InputMode)
                    return;

                CursorPosition.Y = 0;
                ShowEffect();
                break;
            case Keys.PageDown:

                if (TerminalState.InputMode)
                    return;

                CursorPosition.Y = CharacterMatrixDefinition.Height - 1;
                ShowEffect();
                break;
            case Keys.Home:
                CursorPosition.X = TerminalState.InputMode ? TerminalState.InputStartX : 0;
                ShowEffect();
                break;
            case Keys.End:
                CursorPosition.X = CharacterMatrixDefinition.Width - 1;
                ShowEffect();
                break;
            case Keys.Insert:
                DoInsert(_characterMap, CharacterMatrixDefinition.CharacterEmpty);
                DoInsert(_characterColorMap, _characterColorMap[CursorPosition.X, CursorPosition.Y]);
                ShowEffect();
                break;
            case Keys.Delete:
                DoDelete(_characterMap, CharacterMatrixDefinition.CharacterEmpty);
                DoDelete(_characterColorMap, _characterColorMap[CursorPosition.X, CursorPosition.Y]);
                ShowEffect();
                break;
            default:
                _keypressHandler.HandleKeyDown(e, TerminalState.InputMode, TypeCharacter, CursorPosition, ShowKeyboardActivity, Show);
                break;
        }

        base.OnKeyDown(e);

        void ShowEffect()
        {
            _timer.Enabled = false;
            _cursorVisibleBlink = true;
            ShowKeyboardActivity();
            _timer.Enabled = true;
        }
    }

    internal void HandleEnter(bool shift)
    {
        TypedLineEventArgs? eventArgs = null;
        var fireEventTypedLine = false;
        var inputValue = new StringBuilder();

        for (var x = 0; x < CharacterMatrixDefinition.Width; x++)
            inputValue.Append(_codePage.Chr[_characterMap[x, CursorPosition.Y]]);

        var v = inputValue.ToString().Trim();
       
        if (AddProgramLine(v, shift))
        {
            NextLine();
        }
        else
        {
            eventArgs = new TypedLineEventArgs(v);

            if (TerminalState.InputMode)
            {
                TerminalState.InputMode = false;
                InputCompleted?.Invoke(this, eventArgs);
            }
            else
            {
                if (!shift)
                    fireEventTypedLine = true;
            }

            NextLine();
        }

        void NextLine()
        {
            CursorPosition.X = 0;

            if (CursorPosition.CanMoveDown())
                CursorPosition.Y++;
            else
                Scroll();
        }

        _timer.Enabled = false;
        _cursorVisibleBlink = true;
        ShowKeyboardActivity();
        _timer.Enabled = true;

        if (fireEventTypedLine)
            TypedLine?.Invoke(this, eventArgs!);
    }

    private static void DoInsert(int[,] map, int empty)
    {
        
    }

    private static void DoDelete(int[,] map, int empty)
    {

    }

    private bool AddProgramLine(string value, bool shift)
    {
        if (shift || TerminalState.InputMode)
            return false;

        var match = Regex.Match(value, @"^([0-9]+)\s(.*)$");

        if (match.Success)
        {

            if (!int.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var lineNumber))
                return false;

            var line = new ProgramLine(value, lineNumber, match.Groups[2].Value.Trim());
            ProgramLines.InsertProgramLine(line);
            return true;
        }

        match = Regex.Match(value, @"^([0-9]+)$");

        if (match.Success)
        {
            if (!int.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var lineNumber))
                return false;

            ProgramLines.RemoveProgramLine(lineNumber);
            return true;
        }

        return false;
    }

    private new void Scroll()
    {
        ScrollCharacterMap(_characterColorMap, 0);
        ScrollCharacterMap(_characterMap, ' ');
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

    public void BeginInput(string prompt)
    {
        TerminalState.InputMode = true;
    }

    public void List()
    {
        foreach (var programLine in ProgramLines)
            WriteLine(programLine.Value.RawString);
    }

    private void WriteLine(string text)
    {
        var y = CursorPosition.Y;

        for (var x = 0; x < text.Length; x++)
        {
            if (x >= CharacterMatrixDefinition.Width)
                break;
            _characterMap[x, y] = text[x];
        }

        CursorPosition.X = 0;

        if (CursorPosition.CanMoveDown())
            CursorPosition.Y++;
        else
            Scroll();

        UpdateBitmap();
        Invalidate();
    }

    public void New()
    {
        ProgramLines.Clear();
    }
}