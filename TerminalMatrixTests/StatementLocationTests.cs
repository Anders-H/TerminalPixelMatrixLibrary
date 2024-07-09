using TerminalMatrix.StatementManagement;

namespace TerminalMatrixTests;

[TestClass]
public class StatementLocationTests
{
    [TestMethod]
    public void HitTest()
    {
        var s = new StatementLocation(5, 2, 7, 3);
        Assert.IsFalse(s.HitTest(6, 1));
        Assert.IsFalse(s.HitTest(3, 2));
        Assert.IsFalse(s.HitTest(4, 2));
        Assert.IsTrue(s.HitTest(5, 2));
        Assert.IsTrue(s.HitTest(6, 2));
        Assert.IsTrue(s.HitTest(6, 3));
        Assert.IsTrue(s.HitTest(7, 3));
        Assert.IsFalse(s.HitTest(8, 3));
        Assert.IsFalse(s.HitTest(9, 3));
        Assert.IsFalse(s.HitTest(6, 4));
        Assert.IsFalse(s.HitTest(6, 5));
    }

    [TestMethod]
    public void GetFromCoordinate()
    {
        var x = new StatementLocationList
        {
            new(5, 2, 7, 3),
            new(0, 7, 10, 7)
        };

        Assert.IsTrue(x.GetStatementLocationFromPosition(4, 2).Is(4, 2, 4, 2));
        Assert.IsTrue(x.GetStatementLocationFromPosition(5, 2).Is(5, 2, 7, 3));
        Assert.IsTrue(x.GetStatementLocationFromPosition(7, 2).Is(5, 2, 7, 3));
        Assert.IsTrue(x.GetStatementLocationFromPosition(8, 6).Is(8, 6, 8, 6));

        Assert.IsTrue(x.GetStatementLocationFromPosition(4, 6).Is(4, 6, 4, 6));
        Assert.IsTrue(x.GetStatementLocationFromPosition(0, 7).Is(0, 7, 10, 7));
        Assert.IsTrue(x.GetStatementLocationFromPosition(10, 7).Is(0, 7, 10, 7));
        Assert.IsTrue(x.GetStatementLocationFromPosition(11, 7).Is(11, 7, 11, 7));
    }
}