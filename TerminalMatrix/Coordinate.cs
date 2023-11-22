namespace TerminalMatrix;

public class Coordinate
{
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool IsSame(int x, int y) =>
        X == x && Y == y;

    public bool IsSame(Coordinate c) =>
        X == c.X && Y == c.Y;

    public Coordinate Set(int x, int y)
    {
        X = x;
        Y = y;
        return this;
    }

    public Coordinate AlsoSet(Coordinate c)
    {
        X = c.X;
        Y = c.Y;
        return this;
    }

    public void NextRow()
    {
        X = 0;
        Y++;
    }

    public static bool operator >(Coordinate? a, Coordinate? b)
    {
        if (a == null || b == null)
            return false;

        if (a.Y > b.Y)
            return true;

        if (a.Y < b.Y)
            return false;

        return a.X > b.X;
    }

    public static bool operator <(Coordinate? a, Coordinate? b)
    {
        if (a == null || b == null)
            return false;

        if (a.Y < b.Y)
            return true;

        if (a.Y > b.Y)
            return false;

        return a.X < b.X;
    }
}