using PixelmapLibrary;

namespace TerminalMatrix.StatementManagement;

public class StatementLocationList : List<StatementLocation>
{
    public bool LastInputWasBack { get; set; }
    public bool LastInputWasDelete { get; set; }
    public bool LastInputWasTab { get; set; }
    public bool LastInputWasEnter { get; set; }
    public bool LastInputWasRegularCharacter { get; set; }

    public StatementLocationList()
    {
        LastInputWasBack = false;
        LastInputWasDelete = false;
        LastInputWasTab = false;
        LastInputWasEnter = false;
        LastInputWasRegularCharacter = false;
    }

    public void Draw(Pixelmap pixelmap, int borderWidth, int borderHeight)
    {
        foreach (var s in this)
            s.Draw(pixelmap, borderWidth, borderHeight);
    }

    public StatementLocation GetStatementLocationFromPosition(int x, int y) =>
        this.FirstOrDefault(s => s.HitTest(x, y)) ?? new StatementLocation(x, y, x, y);
}