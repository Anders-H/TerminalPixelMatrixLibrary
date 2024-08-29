using PixelmapLibrary;

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

    public StatementLocation? GetStatementLocationFromPositionIfExists(int x, int y) =>
        this.FirstOrDefault(s => s.HitTest(x, y));

    public StatementLocation GetStatementLocationFromPosition(int x, int y)
    {
        var result = this.FirstOrDefault(s => s.HitTest(x, y));

        if (result != null)
            return result;

        result = new StatementLocation(x, y, x, y);
        Add(result);
        return result;
    }

    public StatementLocation FindStatementLocationFromPosition(int x, int y, int columns, int rows, byte[,] characterMap)
    {
        if (x == 0 && y == 0)
            return GetStatementLocationFromPosition(0, 0);

        var result = this.FirstOrDefault(s => s.HitTest(x, y));

        if (result != null)
            return result;

        result = this.FirstOrDefault(s => s.InputEndY == y);

        if (result == null)
            return CreateStatementLocationOnRow(x, y, columns, rows, characterMap);

        if (result.InputEndX < x && x > 0)
        {
            result.InputEndX = x - 1;
            return result;
        }

        StatementLocation.BackOne(ref x, ref y, columns, rows);
        return GetStatementLocationFromPosition(x, y);
    }

    private StatementLocation CreateStatementLocationOnRow(int x, int y, int columns, int rows, byte[,] characterMap)
    {
        if (x == 0 && y == 0)
            return GetStatementLocationFromPosition(0, 0);

        if (Count == 0)
            return GetStatementLocationFromPosition(x, y);

        if (x == 0)
        {
            StatementLocation.BackOne(ref x, ref y, columns, rows);
            return GetStatementLocationFromPosition(x, y);
        }

        var statementLocationsOnSameRow = this.Where(s => s.HitTest(x, y)).ToList();

        if (statementLocationsOnSameRow.Count == 0)
            return GetStatementLocationFromPosition(x, y);

        foreach (var statementLocation in statementLocationsOnSameRow)
            Remove(statementLocation);

        var result = statementLocationsOnSameRow.First();

        if (statementLocationsOnSameRow.Count > 1)
        {
            for (var i = 1; i < statementLocationsOnSameRow.Count; i++)
                result.Merge(statementLocationsOnSameRow[i]);
        }

        if (y == result.InputStartY)
        {
            var startX = GetDataStart(characterMap, y);

            if (startX < result.InputStartX)
                result.InputStartX = startX;
        }
        else if (y == result.InputEndY)
        {
            var startX = GetDataEnd(characterMap, y);

            if (startX > result.InputEndX)
                result.InputEndX = startX;
        }

        return result;
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

    private int GetDataStart(byte[,] c, int row)
    {
        var cols = c.GetLength(0);

        for (var x = 0; x < cols; x++)
        {
            if (c[x, row] <= 0 || c[x, row] == 32)
                continue;

            return x;
        }

        return cols - 1;
    }

    private int GetDataEnd(byte[,] c, int row)
    {
        var cols = c.GetLength(0);

        for (var x = cols - 1; x >= 0; x--)
        {
            if (c[x, row] <= 0 || c[x, row] == 32)
                continue;

            return x;
        }

        return 0;
    }
}