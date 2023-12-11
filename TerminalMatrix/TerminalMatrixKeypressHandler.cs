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
        if (TerminalCodePage.Characters.IndexOf(e.KeyChar) >= 0)
            typeCharacter(e.KeyChar);
    }

    internal void HandleKeyDown(KeyEventArgs e, bool inputMode, int inputStartX, Action<char> typeCharacter, Coordinate cursorPosition, Action showKeyboardActivity, Action scroll)
    {
        if (e.KeyCode == Keys.Enter)
        {
            _owner.HandleEnter((e.Modifiers & Keys.Shift) != 0);
            return;
        }

        switch (e.KeyData)
        {
            case Keys.Left:
                if (inputMode)
                {
                    if (cursorPosition.X > inputStartX)
                    {
                        cursorPosition.X--;
                        showKeyboardActivity();
                    }
                }
                else
                {
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
                }
                break;
            case Keys.Up:
                if (inputMode)
                    return;

                if (cursorPosition.Y > 0)
                {
                    cursorPosition.Y--;
                    showKeyboardActivity();
                }
                break;
            case Keys.Right:
                if (inputMode)
                {
                    if (cursorPosition.X < CharacterMatrixDefinition.Width - 1)
                    {
                        cursorPosition.X++;
                        showKeyboardActivity();
                    }
                }
                else
                {
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
                }
                break;
            case Keys.Down:
                if (inputMode)
                    return;

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