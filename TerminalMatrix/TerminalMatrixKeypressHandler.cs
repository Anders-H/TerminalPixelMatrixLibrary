using TerminalMatrix.Definitions;

namespace TerminalMatrix;

internal class TerminalMatrixKeypressHandler
{
    private readonly TerminalMatrixControl _owner;

    internal TerminalMatrixKeypressHandler(TerminalMatrixControl owner)
    {
        _owner = owner;
    }

    internal void HandleKeyPress(KeyPressEventArgs e, bool inputMode, Action<char> typeCharacter, Coordinate cursorPosition, Action showKeyboardActivity, Action scroll)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($@"HandleKeyPress: {e.KeyChar}");
#endif
        switch (e.KeyChar)
        {
            case ' ':
            case 'A':
            case 'B':
            case 'C':
                typeCharacter(e.KeyChar);
                break;
        }
    }

    internal void HandleKeyDown(KeyEventArgs e, bool inputMode, Action<char> typeCharacter, Coordinate cursorPosition, Action showKeyboardActivity, Action scroll)
    {
#if DEBUG
        System.Diagnostics.Debug.WriteLine($@"HandleKeyDown: {e.KeyData} ({e.KeyValue})");
#endif
        switch (e.KeyData)
        {
            case Keys.Return: // ...and Enter
            _owner.HandleEnter();
            break;
            case Keys.Left:
                if (cursorPosition.X > 0)
                {
                    cursorPosition.X--;
                    showKeyboardActivity();
                }
                else if (cursorPosition.Y > 0)
                {
                    cursorPosition.X = CharacterMatrixDefinition.Width - 1;
                    cursorPosition.Y--;
                    showKeyboardActivity();
                }
                break;
            case Keys.Up:
                if (cursorPosition.Y > 0)
                {
                    cursorPosition.Y--;
                    showKeyboardActivity();
                }
                break;
            case Keys.Right:
                if (cursorPosition.X < CharacterMatrixDefinition.Width - 1)
                {
                    cursorPosition.X++;
                    showKeyboardActivity();
                }
                else if (cursorPosition.Y < CharacterMatrixDefinition.Height - 1)
                {
                    cursorPosition.X = 0;
                    cursorPosition.Y++;
                    showKeyboardActivity();
                }
                break;
            case Keys.Down:
                if (cursorPosition.Y < CharacterMatrixDefinition.Height - 1)
                {
                    cursorPosition.Y++;
                    showKeyboardActivity();
                }
                else
                {
                    scroll();
                    showKeyboardActivity();
                }
                break;
        }
    }
}