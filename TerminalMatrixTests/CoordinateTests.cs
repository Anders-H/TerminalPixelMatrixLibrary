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
}