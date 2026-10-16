using System.Reflection;
using System.Runtime.ExceptionServices;
using TerminalMatrix;
using TerminalMatrix.TerminalColor;

namespace TerminalMatrixTests;

[TestClass]
public class TerminalRenderingTests
{
    private static void OnSta(Action test)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try { test(); }
            catch (Exception ex) { failure = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.IsTrue(thread.Join(TimeSpan.FromSeconds(30)), "Rendering check timed out.");
        if (failure != null)
            ExceptionDispatchInfo.Capture(failure).Throw();
    }

    [TestMethod]
    public void ControlsKeepIndependentDimensionsAndRenderLimits() => OnSta(() =>
    {
        using var wide = new TerminalMatrixControl();
        using var small = new TerminalMatrixControl();
        wide.SetResolution(Resolution.Pixels640x200Characters80x25);
        wide.SetTextRenderLimit(5);
        small.SetResolution(Resolution.Pixels160x200Characters20x25);
        small.SetTextRenderLimit(2);
        wide.SetStartPosition(79, 24);
        Assert.IsTrue(wide.CursorPosition.AtEnd());
        Assert.IsTrue(wide.CursorPosition.Copy().AtEnd());
        Assert.AreEqual(80, wide.CharacterWidth);
        Assert.AreEqual(20, small.CharacterWidth);
        Assert.AreEqual(5, wide.TextRenderLimit);
        Assert.AreEqual(2, small.TextRenderLimit);
        wide.UpdateBitmap();
        small.UpdateBitmap();
        small.SetResolution(Resolution.LogPixels640x80Characters80x10);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => small.SetTextRenderLimit(9));
        wide.SetTextRenderLimit(23);
        wide.SetResolution(Resolution.LogPixels640x80Characters80x10);
        Assert.AreEqual(8, wide.TextRenderLimit);
        wide.UpdateBitmap();
    });

    [TestMethod]
    public void SetPixelsPreservesBorderOffsetAndUsesActualResolution() => OnSta(() =>
    {
        using var terminal = new TerminalMatrixControl();
        terminal.BorderWidth = 4;
        terminal.BorderHeight = 2;
        terminal.SetResolution(Resolution.Pixels640x200Characters80x25);
        var pixels = new byte[2, 2];
        for (var y = 0; y < 2; y++)
            for (var x = 0; x < 2; x++)
                pixels[x, y] = (byte)ColorName.White;
        terminal.UpdateBitmap(null, map =>
        {
            terminal.SetPixels(500, 10, pixels);
            Assert.AreEqual(255, (int)map.GetPixel(504, 12).R);
            Assert.AreEqual(255, (int)map.GetPixel(504, 13).R);
            Assert.AreEqual(0, (int)map.GetPixel(500, 13).R);
        });
        terminal.SetResolution(Resolution.Pixels160x200Characters20x25);
        terminal.UpdateBitmap(null, map =>
        {
            terminal.SetPixels(159, 10, pixels);
            Assert.AreEqual(255, (int)map.GetPixel(163, 12).R);
            Assert.AreEqual(0, (int)map.GetPixel(164, 12).R);
            terminal.SetPixels(-1, 10, pixels);
            Assert.AreEqual(0, (int)map.GetPixel(3, 12).R);
            Assert.AreEqual(255, (int)map.GetPixel(4, 12).R);
        });
    });

    [TestMethod]
    public void BitmapIsUnlockedAfterEitherCallbackThrows() => OnSta(() =>
    {
        using var terminal = new TerminalMatrixControl();
        terminal.SetResolution(Resolution.Pixels320x200Characters40x25);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            terminal.UpdateBitmap(_ => throw new InvalidOperationException()));
        terminal.UpdateBitmap();
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            terminal.UpdateBitmap(null, _ => throw new InvalidOperationException()));
        terminal.UpdateBitmap();
    });

    [TestMethod]
    public void DisposeStopsTimerAndReleasesBitmap() => OnSta(() =>
    {
        using var terminal = new TerminalMatrixControl();
        terminal.SetResolution(Resolution.Pixels320x200Characters40x25);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var timer = (System.Windows.Forms.Timer)typeof(TerminalMatrixControl)
            .GetField("_timer", flags)!.GetValue(terminal)!;
        timer.Start();
        terminal.Dispose();
        Assert.IsTrue(terminal.QuitFlag);
        Assert.IsFalse(timer.Enabled);
        Assert.IsNull(typeof(TerminalMatrixControl).GetField("_fastBitmap", flags)!.GetValue(terminal));
        terminal.UpdateBitmap();
    });
}