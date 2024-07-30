using PixelmapLibrary;

namespace TerminalMatrix.StatementManagement;

public class StatementLocation
{
    public int InputStartX { get; set; }
    public int InputStartY { get; set; }
    public int InputEndX { get; set; }
    public int InputEndY { get; set; }

    public StatementLocation() : this(0, 0, 0, 0)
    {
    }

    public StatementLocation(int inputStartX, int inputStartY, int inputEndX, int inputEndY)
    {
        InputStartX = inputStartX;
        InputStartY = inputStartY;
        InputEndX = inputEndX;
        InputEndY = inputEndY;
    }

    public void Draw(Pixelmap pixelmap, int borderWidth, int borderHeight, bool current)
    {
        var inputStartX = InputStartX * 8 + borderWidth;
        var inputStartY = InputStartY * 8 + borderHeight;
        var inputEndX = InputEndX * 8 + borderWidth;
        var inputEndY = InputEndY * 8 + borderHeight;

        if (current)
        {
            pixelmap.AddColor(inputStartX, inputStartY, 80, 80, 110);

            pixelmap.AddColor(inputStartX + 1, inputStartY, 70, 70, 100);
            pixelmap.AddColor(inputStartX + 2, inputStartY, 60, 60, 90);
            pixelmap.AddColor(inputStartX + 3, inputStartY, 50, 50, 80);
            pixelmap.AddColor(inputStartX + 4, inputStartY, 40, 40, 70);
            pixelmap.AddColor(inputStartX + 5, inputStartY, 30, 30, 60);

            pixelmap.AddColor(inputStartX, inputStartY + 1, 70, 70, 100);
            pixelmap.AddColor(inputStartX, inputStartY + 2, 60, 60, 90);
            pixelmap.AddColor(inputStartX, inputStartY + 3, 50, 50, 80);
            pixelmap.AddColor(inputStartX, inputStartY + 4, 40, 40, 70);
            pixelmap.AddColor(inputStartX, inputStartY + 5, 30, 30, 60);

            pixelmap.AddColor(inputEndX + 7, inputEndY + 7, 80, 80, 110);

            pixelmap.AddColor(inputEndX + 6, inputEndY + 7, 70, 70, 100);
            pixelmap.AddColor(inputEndX + 5, inputEndY + 7, 60, 60, 90);
            pixelmap.AddColor(inputEndX + 4, inputEndY + 7, 50, 50, 80);
            pixelmap.AddColor(inputEndX + 3, inputEndY + 7, 40, 40, 70);
            pixelmap.AddColor(inputEndX + 2, inputEndY + 7, 30, 30, 60);

            pixelmap.AddColor(inputEndX + 7, inputEndY + 6, 70, 70, 100);
            pixelmap.AddColor(inputEndX + 7, inputEndY + 5, 60, 60, 90);
            pixelmap.AddColor(inputEndX + 7, inputEndY + 4, 50, 50, 80);
            pixelmap.AddColor(inputEndX + 7, inputEndY + 3, 40, 40, 70);
            pixelmap.AddColor(inputEndX + 7, inputEndY + 2, 30, 30, 60);
            return;
        }

        pixelmap.AddColor(inputStartX, inputStartY, 80, 80, 80);
        
        pixelmap.AddColor(inputStartX + 1, inputStartY, 70, 70, 70);
        pixelmap.AddColor(inputStartX + 2, inputStartY, 60, 60, 60);
        pixelmap.AddColor(inputStartX + 3, inputStartY, 50, 50, 50);
        pixelmap.AddColor(inputStartX + 4, inputStartY, 40, 40, 40);
        
        pixelmap.AddColor(inputStartX, inputStartY + 1, 70, 70, 70);
        pixelmap.AddColor(inputStartX, inputStartY + 2, 60, 60, 60);
        pixelmap.AddColor(inputStartX, inputStartY + 3, 50, 50, 50);
        pixelmap.AddColor(inputStartX, inputStartY + 4, 40, 40, 40);

        pixelmap.AddColor(inputEndX + 7, inputEndY + 7, 80, 80, 80);

        pixelmap.AddColor(inputEndX + 6, inputEndY + 7, 70, 70, 70);
        pixelmap.AddColor(inputEndX + 5, inputEndY + 7, 60, 60, 60);
        pixelmap.AddColor(inputEndX + 4, inputEndY + 7, 50, 50, 50);
        pixelmap.AddColor(inputEndX + 3, inputEndY + 7, 40, 40, 40);
        
        pixelmap.AddColor(inputEndX + 7, inputEndY + 6, 70, 70, 70);
        pixelmap.AddColor(inputEndX + 7, inputEndY + 5, 60, 60, 60);
        pixelmap.AddColor(inputEndX + 7, inputEndY + 4, 50, 50, 50);
        pixelmap.AddColor(inputEndX + 7, inputEndY + 3, 40, 40, 40);
    }

    public bool HitTest(int x, int y)
    {
        if (y < InputStartY || y > InputEndY)
            return false;

        if (y > InputStartY && y < InputEndY)
            return true;

        if (InputStartY == InputEndY)
        {
            if (x >= InputStartX && x <= InputEndX)
                return true;
        }
        else if (InputStartY < InputEndY)
        {
            if (y == InputStartY)
                return x >= InputStartX;

            if (y == InputEndY)
                return x <= InputEndX;
        }

        return false;
    }

    public void HitTest(StatementLocation s)
    {

    }

    public Point[] GetPoints(int width)
    {

    }

    public bool Is(int inputStartX, int inputStartY, int inputEndX, int inputEndY) =>
        InputStartX == inputStartX
        && InputStartY == inputStartY
        && InputEndX == inputEndX
        && InputEndY == inputEndY;

    public bool Is(StatementLocation s) =>
        InputStartX == s.InputStartX
        && InputStartY == s.InputStartY
        && InputEndX == s.InputEndX
        && InputEndY == s.InputEndY;

    public void Scroll()
    {
        InputStartY--;
        InputEndY--;
    }

    public void Grow(int width, int height)
    {
        if (InputEndX < width - 1)
        {
            InputEndX++;
            return;
        }

        InputEndX = 0;
        InputEndY++;
    }

    public override string ToString() =>
        $"({InputStartX}, {InputStartY})-({InputEndX}, {InputEndY})";
}