using TerminalMatrix;

namespace TerminalMatrixTests;

[TestClass]
public class CoordinateTests
{
    [TestMethod]
    public void GreaterThan()
    {
        var c = new Coordinate(10, 10);

        Assert.IsTrue(c > new Coordinate(9, 10));
        Assert.IsTrue(c > new Coordinate(10, 9));
        Assert.IsFalse(c > new Coordinate(10, 10));
        Assert.IsFalse(c > new Coordinate(11, 10));
        Assert.IsFalse(c > new Coordinate(10, 11));
    }

    [TestMethod]
    public void LessThan()
    {
        var c = new Coordinate(10, 10);

        Assert.IsFalse(c < new Coordinate(9, 10));
        Assert.IsFalse(c < new Coordinate(10, 9));
        Assert.IsFalse(c < new Coordinate(10, 10));
        Assert.IsTrue(c < new Coordinate(11, 10));
        Assert.IsTrue(c < new Coordinate(10, 11));
    }

    [TestMethod]
    public void MoveNext()
    {
        var steps = 0;
        var c = new Coordinate(50, 18);

        while (c.MoveNext())
        {
            steps++;
            System.Diagnostics.Debug.WriteLine($"{steps} {c.X} {c.Y}");
        }

        Assert.AreEqual(509, steps);
        Assert.AreEqual(79, c.X);
        Assert.AreEqual(24, c.Y);
    }

    [TestMethod]
    public void MovePrevious()
    {
        var steps = 0;
        var c = new Coordinate(50, 18);

        while (c.MovePrevious())
        {
            steps++;
            System.Diagnostics.Debug.WriteLine($"{steps} {c.X} {c.Y}");
        }

        Assert.AreEqual(1490, steps);
        Assert.AreEqual(0, c.X);
        Assert.AreEqual(0, c.Y);
    }
}