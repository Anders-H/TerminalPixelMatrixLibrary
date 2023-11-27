using TerminalMatrix.Definitions;
using TerminalMatrix.Events;

namespace TerminalMatrix;

internal class TerminalMatrixKeypressHandler
{
    private readonly TerminalMatrixControl _owner;
    private readonly int[,] _characterMap;

    internal TerminalMatrixKeypressHandler(TerminalMatrixControl owner, int[,] characterMap)
    {
        _owner = owner;
        _characterMap = characterMap;
    }

    internal void HandleKeyDown(KeyEventArgs e, bool inputMode, Action<char> typeCharacter)
    {
        switch (e.KeyCode)
        {
            case Keys.Back: // Backspace
                break;
            case Keys.Return: // ...and Enter
                _owner.HandleEnter();
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
                typeCharacter(' ');
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
                    Scroll();
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
                typeCharacter('A');
                break;
            case Keys.B:
                typeCharacter('B');
                break;
            case Keys.C:
                typeCharacter('C');
                break;
            case Keys.D:
                typeCharacter('D');
                break;
            case Keys.E:
                typeCharacter('E');
                break;
            case Keys.F:
                typeCharacter('F');
                break;
            case Keys.G:
                typeCharacter('G');
                break;
            case Keys.H:
                typeCharacter('H');
                break;
            case Keys.I:
                typeCharacter('I');
                break;
            case Keys.J:
                typeCharacter('J');
                break;
            case Keys.K:
                typeCharacter('K');
                break;
            case Keys.L:
                typeCharacter('L');
                break;
            case Keys.M:
                typeCharacter('M');
                break;
            case Keys.N:
                typeCharacter('N');
                break;
            case Keys.O:
                typeCharacter('O');
                break;
            case Keys.P:
                typeCharacter('P');
                break;
            case Keys.Q:
                typeCharacter('Q');
                break;
            case Keys.R:
                typeCharacter('R');
                break;
            case Keys.S:
                typeCharacter('S');
                break;
            case Keys.T:
                typeCharacter('T');
                break;
            case Keys.U:
                typeCharacter('U');
                break;
            case Keys.V:
                typeCharacter('V');
                break;
            case Keys.W:
                typeCharacter('W');
                break;
            case Keys.X:
                typeCharacter('X');
                break;
            case Keys.Y:
                typeCharacter('Y');
                break;
            case Keys.Z:
                typeCharacter('Z');
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

    }
}