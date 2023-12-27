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
    public event UserBreakDelegate? UserBreak;
    public event RequestToggleFullscreenDelegate? RequestToggleFullscreen;
    public event FunctionKeyPressedDelegate? FunctionKeyPressed;

    private byte[,] _characterColorMap;
    private byte[,] _characterMap;
    private byte[,] _pixelMap;
    private int[] _bitmap;
    private bool _cursorVisibleBlink;
    private readonly System.Windows.Forms.Timer _timer = new();
    private string _lastInput;
    private readonly TerminalFont _font = new();
    private readonly TerminalCodePage _codePage;
    private readonly Palette _palette;
    private TerminalState TerminalState { get; }
    private readonly TerminalMatrixKeypressHandler _keypressHandler;
    public Resolution Resolution { get; private set; }
    public bool QuitFlag { get; private set; }
    public Bitmap? Bitmap { get; private set; }
    public Coordinate CursorPosition { get; private set; }
    public byte CurrentCursorColor { get; set; }
    public ProgramLineDictionary ProgramLines { get; } = new();
    /// <summary>
    /// The width of the border in physical pixels.
    /// </summary>
    public int BorderWidth { get; set; }
    /// <summary>
    /// The height of the border in physical pixels.
    /// </summary>
    public int BorderHeight { get; set; }

#pragma warning disable CS8618
    public TerminalMatrixControl()
#pragma warning restore CS8618
    {
        SetResolution(Resolution.Pixels320x200Characters40x25);
        _cursorVisibleBlink = false;
        _lastInput = "";
        _codePage = new TerminalCodePage();
        _palette = new Palette();
        TerminalState = new TerminalState();
        _keypressHandler = new TerminalMatrixKeypressHandler(this);
        CurrentCursorColor = (int)ColorName.White;
        _timer.Interval = 1000;
        QuitFlag = false;
        InitializeComponent();
    }

    private void UserControl1_Load(object sender, EventArgs e)
    {
        DoubleBuffered = true;
        _timer.Tick += Blink;
        _timer.Enabled = true;
    }

    /// <summary>
    /// First visible text line.
    /// </summary>
    /// <param name="limit">0 (default, all lines are visible) to 23 (only the bottom two lines are visible)</param>
    public void SetTextRenderLimit(int limit)
    {
        if (limit < 0 || limit > 23)
            throw new ArgumentOutOfRangeException(nameof(limit));

        if (CursorPosition.Y < limit)
            CursorPosition.Y = limit;

        CharacterMatrixDefinition.TextRenderLimit = limit;
        UpdateBitmap();
        Invalidate();
    }

    public void SetStartPosition(int x, int y)
    {
        CursorPosition.Set(x, y);
    }

    public void SetResolution(Resolution resolution)
    {
        Resolution = resolution;
        CursorPosition = new Coordinate(0, 0);
        _characterColorMap = CharacterMatrixDefinition.Create(Resolution);
        _characterMap = CharacterMatrixDefinition.Create(Resolution);
        _pixelMap = PixelMatrixDefinition.Create(Resolution);
        _bitmap = PixelMatrixDefinition.CreateBitmap();
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
        var c = (byte)color;

        for (var x = 0; x < PixelMatrixDefinition.Width; x++)
            _pixelMap[x, y] = c;
    }

    public byte[,] LoadPictureFromGif(string filename)
    {
        using var gif = new Bitmap(filename);
        var result = new byte[gif.Width, gif.Height];

        for (var y = 0; y < gif.Height; y++)
            for (var x = 0; x < gif.Width; x++)
                result[x, y] = _palette.SearchColor(gif.GetPixel(x, y));

        return result;
    }

    public void SetPixel(int x, int y, ColorName color)
    {
        _pixelMap[x, y] = (byte)color;
    }

    public void SetPixels(int x, int y, byte[,] colors)
    {
        var targetX = x;
        var targetY = y;

        for (var sourceY = 0; sourceY < colors.GetLength(1); sourceY++)
        {
            for (var sourceX = 0; sourceX < colors.GetLength(0); sourceX++)
            {
                if (targetX >= 0 && targetX < PixelMatrixDefinition.Width && targetY >= 0 && targetY < PixelMatrixDefinition.Height)
                    _pixelMap[targetX, targetY] = colors[sourceX, sourceY];

                targetX++;
            }

            targetX = x;
            targetY++;
        }
    }

    public void HorizontalLine(int x1, int y, int x2, ColorName color)
    {
        var c = (byte)color;

        for (var x = x1; x <= x2; x++)
            _pixelMap[x, y] = c;
    }

    public void VerticalLine(int x, ColorName color)
    {
        var c = (byte)color;

        for (var y = 0; y < PixelMatrixDefinition.Height; y++)
            _pixelMap[x, y] = c;
    }

    public void VerticalLine(int x, int y1, int y2, ColorName color)
    {
        var c = (byte)color;

        for (var y = y1; y <= y2; y++)
            _pixelMap[x, y] = c;
    }

    public void Box(ColorName color, int x1, int y1, int x2, int y2)
    {
        var c = (byte)color;

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
        var c = (byte)color;

        for (var i = 0; i < text.Length; i++)
        {
            _characterColorMap[x + i, y] = c;
            _characterMap[x + i, y] = _codePage.Asc[text[i]];
        }
    }

    public void UpdateBitmap()
    {
        if (QuitFlag)
            return;

        Bitmap?.Dispose();
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

        for (var y = CharacterMatrixDefinition.TextRenderLimit; y < CharacterMatrixDefinition.Height; y++)
        {
            for (var x = 0; x < CharacterMatrixDefinition.Width; x++)
            {
                var characterFont = _font[_characterMap[x, y]];
                var c = _palette.GetColor(_characterColorMap[x, y]).ToArgb();
                var cursCol = _palette.GetColor(CurrentCursorColor).ToArgb();
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
                                _bitmap[index] = characterFont.Pixels[source.X, source.Y] ? 0 : cursCol;

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
                            else
                                _bitmap[index] = _palette.GetColor(_pixelMap[pixelX, pixelY]).ToArgb();

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

        var doubleWidth = BorderWidth + BorderWidth;
        var doubleHeight = BorderHeight + BorderHeight;

        if (Width < 5 + doubleWidth || Height < 5 + doubleHeight)
            return;

        e.Graphics.CompositingMode = CompositingMode.SourceCopy;
        e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.SmoothingMode = SmoothingMode.None;
        e.Graphics.Clear(Color.Black);
        e.Graphics.DrawImage(Bitmap, BorderWidth, BorderHeight, Width - doubleWidth, Height - doubleHeight);
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
        _characterMap[CursorPosition.X, CursorPosition.Y] = (byte)c;
        _characterColorMap[CursorPosition.X, CursorPosition.Y] = CurrentCursorColor;

        if (CursorPosition.X < CharacterMatrixDefinition.Width - 1)
        {
            CursorPosition.X++;
            ShowKeyboardActivity();
        }

        ShowKeyboardActivity();
    }

    public void WriteText(string text)
    {
        var rows = WordWrapper.WordWrap(text);

        foreach (var row in rows)
            WriteLine(row);
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
        if (e.KeyCode == Keys.Back)
        {
            var limit = TerminalState.InputMode ? TerminalState.InputStartX : 0;

            if (CursorPosition.X > limit)
            {
                CursorPosition.X--;
                DoDelete(_characterMap, CharacterMatrixDefinition.CharacterEmpty);
                DoDelete(_characterColorMap, _characterColorMap[CursorPosition.X, CursorPosition.Y]);
                ShowEffect();
            }

            return;
        }

        if (e is { KeyCode: Keys.C, Control: true })
        {
            UserBreak?.Invoke(this, e);
            return;
        }

        switch (e.KeyData)
        {
            case Keys.F1:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F1));
                break;
            case Keys.F2:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F2));
                break;
            case Keys.F3:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F3));
                break;
            case Keys.F4:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F4));
                break;
            case Keys.F5:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F5));
                break;
            case Keys.F6:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F6));
                break;
            case Keys.F7:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F7));
                break;
            case Keys.F8:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F8));
                break;
            case Keys.F9:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F9));
                break;
            case Keys.F10:
                FunctionKeyPressed?.Invoke(this, new FunctionKeyEventArgs(FunctionKey.F10));
                break;
            case Keys.F11:
                RequestToggleFullscreen?.Invoke(this, e);
                break;
            case Keys.PageUp:

                if (TerminalState.InputMode)
                    return;

                CursorPosition.Y = CharacterMatrixDefinition.TextRenderLimit;
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
                _keypressHandler.HandleKeyDown(e, TerminalState.InputMode, TerminalState.InputStartX, TypeCharacter, CursorPosition, ShowKeyboardActivity, Show);
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

        var start = TerminalState.InputMode ? TerminalState.InputStartX : 0;

        for (var x = start; x < CharacterMatrixDefinition.Width; x++)
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
                _lastInput = v;
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

    private void DoInsert(byte[,] map, byte empty)
    {
        if (CursorPosition.X >= CharacterMatrixDefinition.Width - 1)
        {
            map[CursorPosition.X, CursorPosition.Y] = empty;
            return;
        }

        for (var x = CharacterMatrixDefinition.Width - 2; x >= CursorPosition.X; x--)
            map[x + 1, CursorPosition.Y] = map[x, CursorPosition.Y];

        map[CursorPosition.X, CursorPosition.Y] = empty;
    }

    private void DoDelete(byte[,] map, byte empty)
    {
        if (CursorPosition.X >= CharacterMatrixDefinition.Width - 1)
        {
            map[CursorPosition.X, CursorPosition.Y] = empty;
            return;
        }

        for (var x = CursorPosition.X; x < CharacterMatrixDefinition.Width - 1; x++)
            map[x, CursorPosition.Y] = map[x + 1, CursorPosition.Y];

        map[CharacterMatrixDefinition.Width - 1, CursorPosition.Y] = empty;
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
        ScrollCharacterMap(_characterColorMap, CurrentCursorColor);
        ScrollCharacterMap(_characterMap, (byte)' ');
    }

    private void ScrollCharacterMap(byte[,] characterMap, byte blank)
    {
        for (var y = 1; y < CharacterMatrixDefinition.Height; y++)
            for (var x = 0; x < CharacterMatrixDefinition.Width; x++)
                characterMap[x, y - 1] = characterMap[x, y];

        const int last = CharacterMatrixDefinition.Height - 1;

        for (var x = 0; x < CharacterMatrixDefinition.Width; x++)
            characterMap[x, last] = blank;
    }

    public void BeginInput(string prompt) =>
        BeginInput(prompt, "", CurrentCursorColor, CurrentCursorColor);

    public void BeginInput(string prompt, string defaultValue) =>
        BeginInput(prompt, defaultValue, CurrentCursorColor, CurrentCursorColor);

    public void BeginInput(string prompt, byte promptColor, byte valueColor) =>
        BeginInput(prompt, "", CurrentCursorColor, CurrentCursorColor);

    public void BeginInput(string prompt, string defaultValue, byte promptColor, byte valueColor)
    {
        if (prompt.Length > CharacterMatrixDefinition.Width - 2)
            prompt = prompt.Substring(0, CharacterMatrixDefinition.Width - 2);

        ClearLine(_characterMap, CursorPosition.Y, CharacterMatrixDefinition.CharacterEmpty);
        ClearLine(_characterColorMap, CursorPosition.Y, CurrentCursorColor);
        CurrentCursorColor = promptColor;
        Write(prompt);
        CurrentCursorColor = valueColor;
        TerminalState.InputStartX = prompt.Length;
        Write(TerminalState.InputStartX, defaultValue);

        if (prompt.Length == 0)
        {
            CursorPosition.X = TerminalState.InputStartX;
        }
        else
        {
            CursorPosition.X = TerminalState.InputStartX + defaultValue.Length;

            if (CursorPosition.X > CharacterMatrixDefinition.Width - 1)
                CursorPosition.X = CharacterMatrixDefinition.Width - 1;
        }

        TerminalState.InputMode = true;
        UpdateBitmap();
        Invalidate();
    }

    private void ClearLine(byte[,] map, int y, byte clear)
    {
        for (var x = 0; x < CharacterMatrixDefinition.Width; x++)
            map[x, y] = clear;
    }

    public void List()
    {
        foreach (var programLine in ProgramLines)
            WriteLine(programLine.Value.RawString);
    }

    public void Write(string text) =>
        Write(0, text);

    public void Write(int xStart, string text)
    {
        var y = CursorPosition.Y;

        for (var x = 0; x < text.Length; x++)
        {
            if (x + xStart >= CharacterMatrixDefinition.Width)
                break;
            _characterMap[x + xStart, y] = (byte)text[x];
            _characterColorMap[x + xStart, y] = CurrentCursorColor;
        }

        UpdateBitmap();
        Invalidate();
    }

    public void WriteLine(string text)
    {
        var y = CursorPosition.Y;

        for (var x = 0; x < text.Length; x++)
        {
            if (x >= CharacterMatrixDefinition.Width)
                break;

            _characterMap[x, y] = (byte)text[x];
            _characterColorMap[x, y] = CurrentCursorColor;
        }

        CursorPosition.X = 0;

        if (CursorPosition.CanMoveDown())
            CursorPosition.Y++;
        else
            Scroll();

        UpdateBitmap();
        Invalidate();
    }

    public string Input(string prompt) =>
        Input(prompt, "", CurrentCursorColor, CurrentCursorColor);

    public string Input(string prompt, string defaultValue) =>
        Input(prompt, defaultValue, CurrentCursorColor, CurrentCursorColor);

    public string Input(string prompt, byte promptColor, byte valueColor) =>
        Input(prompt, "", CurrentCursorColor, CurrentCursorColor);

    public string Input(string prompt, string defaultValue, byte promptColor, byte valueColor)
    {
        BeginInput(prompt, defaultValue, promptColor, valueColor);

        do
        {
            if (QuitFlag)
                return "";

            Thread.Yield();
            Thread.Sleep(2);
            Application.DoEvents();
        } while (TerminalState.InputMode && !QuitFlag);

        return _lastInput;
    }

    public void New()
    {
        ProgramLines.Clear();
    }

    public void Quit()
    {
        QuitFlag = true;
    }
}