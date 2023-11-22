namespace TerminalMatrix;

public class CoordinateList : List<Coordinate>
{
    public CoordinateList()
    {
    }

    public CoordinateList(int startX, int startY)
    {
        Add(new Coordinate(startX, startY));
    }

    public void Add(int x, int y) =>
        Add(new Coordinate(x, y));
}