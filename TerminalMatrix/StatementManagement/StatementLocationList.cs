﻿using PixelmapLibrary;

namespace TerminalMatrix.StatementManagement;

public class StatementLocationList : List<StatementLocation>
{
    public bool LastInputWasBack { get; set; }
    public bool LastInputWasDelete { get; set; }
    public bool LastInputWasTab { get; set; }
    public bool LastInputWasEnter { get; set; }
    public bool LastInputWasCursorMovement { get; set; }
    public bool LastInputWasRegularCharacter { get; set; }
    public PreviousInputCategory Previous { get; set; }

    public StatementLocationList()
    {
        LastInputWasBack = false;
        LastInputWasDelete = false;
        LastInputWasTab = false;
        LastInputWasEnter = false;
        LastInputWasCursorMovement = false;
        LastInputWasRegularCharacter = false;
        Previous = PreviousInputCategory.RegularCharacter;
    }

#if DEBUG
    public new void Add(StatementLocation s)
    {
        if (DataCorrupted())
            throw new SystemException("Override!!!!");

        base.Add(s);
    }

    private bool DataCorrupted()
    {
        foreach (var a in this)
        {
            foreach (var b in this)
            {
                if (a == b)
                    continue;

                if (a.HitTest(b))
                    return true;
            }
        }

        return false;
    }
#endif

    public void Draw(Pixelmap pixelmap, int borderWidth, int borderHeight, StatementLocation? current)
    {
        foreach (var s in this)
            s.Draw(pixelmap, borderWidth, borderHeight, current == s);
    }

    public StatementLocation GetStatementLocationFromPosition(int x, int y)
    {
        var result = this.FirstOrDefault(s => s.HitTest(x, y));

        if (result != null)
            return result;

        result = new StatementLocation(x, y, x, y);
        Add(result);
        return result;
    }

    public StatementLocation FindStatementLocationFromPosition(int x, int y)
    {
        if (x == 0 && y == 0)
            return GetStatementLocationFromPosition(0, 0);

        var result = this.FirstOrDefault(s => s.HitTest(x, y));

        if (result != null)
            return result;

        result = this.FirstOrDefault(s => s.InputEndY == y);

        if (result == null)
            return GetStatementLocationFromPosition(x, y);

        if (result.InputEndX < x && x > 0)
        {
            result.InputEndX = x - 1;
            return result;
        }

        return GetStatementLocationFromPosition(x, y);
    }

    public void Scroll()
    {
        foreach (var sl in this)
            sl.Scroll();

        bool repeat;

        do
        {
            repeat = false;

            foreach (var sl in this.Where(sl => sl.InputStartY < 0))
            {
                Remove(sl);
                repeat = true;
                break;
            }
        } while (repeat);
    }
}