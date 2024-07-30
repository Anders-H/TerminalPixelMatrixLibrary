using TerminalMatrix.StatementManagement;

namespace TerminalMatrixTests;

[TestClass]
public class StatementLocationTests
{
    [TestMethod]
    public void GetPoints()
    {
        var s = new StatementLocation(5, 2, 7, 3);
        var points = s.GetPoints(10);
        Assert.IsTrue(points.Length == 13);
        Assert.IsTrue(points.First().X == 5 && points.First().Y == 2);
        Assert.IsTrue(points.Last().X == 7 && points.Last().Y == 3);
    }

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
    public void HitTest2()
    {
        var s = new StatementLocation(5, 2, 7, 3);
        Assert.IsFalse(s.HitTest(new StatementLocation(7, 3, 8, 3)));
        Assert.IsFalse(s.HitTest(new StatementLocation(8, 3, 9, 3)));
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

    [TestMethod]
    public void Grow()
    {
        var s = new StatementLocation(5, 2, 7, 3);
        s.Grow(40, 25);
        Assert.IsTrue(s.Is(6, 2, 7, 3));
    }

    [TestMethod]
    public void GrabRowFromCoordinate()
    {
        var statementLocations = new StatementLocationList
        {
            new(5, 2, 7, 3)
        };

        Assert.AreEqual(1, statementLocations.Count);
        var x1 = statementLocations.FindStatementLocationFromPosition(10, 10);
        Assert.IsTrue(x1.Is(10, 10, 10, 10));
        Assert.AreEqual(2, statementLocations.Count);
        Assert.IsTrue(statementLocations[0].Is(5, 2, 7, 3));
        Assert.IsTrue(statementLocations[1].Is(10, 10, 10, 10));
        var x2 = statementLocations.FindStatementLocationFromPosition(10, 3);
        Assert.IsTrue(x2.Is(5, 2, 9, 3));
        Assert.AreEqual(2, statementLocations.Count);
        Assert.IsTrue(statementLocations[0].Is(5, 2, 9, 3));
        Assert.IsTrue(statementLocations[1].Is(10, 10, 10, 10));
    }
}