using P = TerminalMatrix.Definitions.CharacterPosition;

namespace TerminalMatrixTests;

[TestClass]
public class CharacterPosition
{
    [TestMethod]
    public void LargerThan()
    {
        Assert.IsTrue(new P(10, 10) > new P(9, 10));
        Assert.IsFalse(new P(9, 10) > new P(9, 10));
        Assert.IsTrue(new P(10, 11) > new P(10, 10));
    }
}