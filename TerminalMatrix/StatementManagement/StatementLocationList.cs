using PixelmapLibrary;

namespace TerminalMatrix.StatementManagement;

public class StatementLocationList : List<StatementLocation>
{
    public bool LastInputWasBack { get; set; }

    public StatementLocationList()
    {
        LastInputWasBack = false;
    }

    public void Draw(Pixelmap pixelmap, int borderWidth, int borderHeight)
    {
        foreach (var s in this)
            s.Draw(pixelmap, borderWidth, borderHeight);
    }

    public StatementLocation GetStatementLocationFromPosition(int x, int y) =>
        this.FirstOrDefault(s => s.HitTest(x, y)) ?? new StatementLocation(x, y, x, y);
}