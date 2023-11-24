using TerminalMatrix;

namespace TerminalMatrixTests;

[TestClass]
public class CoordinateListTests
{
    [TestMethod]
    public void HitTest()
    {
        var cList = new CoordinateList
        {
            { 10, 10 },
            { 20, 20 }
        };

        Assert.IsTrue(cList.HitTest(new Coordinate(20, 20)));
        Assert.IsTrue(cList.HitTest(new Coordinate(10, 10)));
        Assert.IsFalse(cList.HitTest(new Coordinate(11, 10)));
        Assert.IsFalse(cList.HitTest(new Coordinate(20, 21)));
        Assert.IsFalse(cList.HitTest(new Coordinate(19, 20)));
    }
}