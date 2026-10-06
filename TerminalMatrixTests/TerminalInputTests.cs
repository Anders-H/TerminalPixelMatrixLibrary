using System.Runtime.ExceptionServices;
using TerminalMatrix;

namespace TerminalMatrixTests;

[TestClass]
[DoNotParallelize]
public class TerminalInputTests
{
    private sealed class TestTerminal : TerminalMatrixControl
    {
        public void Type(string text)
        {
            foreach (var c in text)
                OnKeyPress(new KeyPressEventArgs(c));
        }

        public void Press(Keys key) => OnKeyDown(new KeyEventArgs(key));
    }

    private static void WithTerminal(Action<TestTerminal> test)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var terminal = new TestTerminal();
                terminal.SetResolution(Resolution.Pixels320x200Characters40x25);
                test(terminal);
                terminal.Quit();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure != null)
            ExceptionDispatchInfo.Capture(failure).Throw();
    }

    [DataTestMethod]
    [DataRow(39, 0)]
    [DataRow(40, 0)]
    [DataRow(41, 0)]
    [DataRow(80, 0)]
    [DataRow(161, 0)]
    [DataRow(40, 24)]
    [DataRow(161, 24)]
    public void EnterReturnsEntireWrappedLine(int length, int row)
    {
        WithTerminal(terminal =>
        {
            var expected = new string('A', length);
            var submitted = new List<string>();
            terminal.TypedLine += (_, e) => submitted.Add(e.InputValue);
            terminal.SetStartPosition(0, row);
            terminal.Type(expected);
            terminal.Press(Keys.Enter);
            terminal.Press(Keys.Enter);
            CollectionAssert.AreEqual(new[] { expected, "" }, submitted);
        });
    }

    [TestMethod]
    public void EmptyLineDoesNotSubmitOutputBelowIt()
    {
        WithTerminal(terminal =>
        {
            terminal.SetStartPosition(0, 2);
            terminal.WriteLine("OLD OUTPUT");
            terminal.SetStartPosition(0, 1);
            string? submitted = null;
            terminal.TypedLine += (_, e) => submitted = e.InputValue;
            terminal.Press(Keys.Enter);
            Assert.AreEqual("", submitted);
        });
    }

    [TestMethod]
    public void SpacesAtWrapBoundariesArePreserved()
    {
        WithTerminal(terminal =>
        {
            var expected = new string('A', 38) + "    B";
            string? submitted = null;
            terminal.TypedLine += (_, e) => submitted = e.InputValue;
            terminal.Type(expected);
            terminal.Press(Keys.Enter);
            Assert.AreEqual(expected, submitted);
        });
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(24)]
    public void WrappedInputExcludesPrompt(int row)
    {
        WithTerminal(terminal =>
        {
            var expected = new string('C', 81);
            string? submitted = null;
            terminal.InputCompleted += (_, e) => submitted = e.InputValue;
            terminal.SetStartPosition(0, row);
            terminal.BeginInput("? ");
            terminal.Type(expected);
            terminal.Press(Keys.Enter);
            Assert.AreEqual(expected, submitted);
        });
    }

    [TestMethod]
    public void LongDefaultValueIsNotTruncated()
    {
        WithTerminal(terminal =>
        {
            var expected = new string('D', 81);
            string? submitted = null;
            terminal.InputCompleted += (_, e) => submitted = e.InputValue;
            terminal.BeginInput("? ", expected);
            terminal.Press(Keys.Enter);
            Assert.AreEqual(expected, submitted);
        });
    }

    [TestMethod]
    public void ScrollMovesLineBoundariesWithTheirText()
    {
        WithTerminal(terminal =>
        {
            terminal.Type(new string('A', 41));
            terminal.Press(Keys.Enter);
            terminal.SetStartPosition(0, 24);
            terminal.WriteLine("OUTPUT");
            Assert.IsTrue(terminal.GetTerminator(0));
            Assert.IsTrue(terminal.GetTerminator(24));

            terminal.Clear();
            terminal.SetStartPosition(0, 23);
            terminal.Type(new string('B', 41));
            terminal.Press(Keys.Down);
            Assert.IsFalse(terminal.GetTerminator(22));
            Assert.IsTrue(terminal.GetTerminator(23));
            Assert.IsTrue(terminal.GetTerminator(24));
        });
    }

    [TestMethod]
    public void EnterFromFirstRowReadsAllContinuationRowsOnce()
    {
        WithTerminal(terminal =>
        {
            var expected = new string('A', 40) + new string('B', 40)
                + new string('C', 40) + new string('D', 40) + "E";
            string? submitted = null;
            terminal.TypedLine += (_, e) => submitted = e.InputValue;
            terminal.Type(expected);
            terminal.SetStartPosition(0, 0);
            terminal.Press(Keys.Enter);
            Assert.AreEqual(expected, submitted);
            Assert.AreEqual(5, terminal.CursorPosition.Y);
        });
    }

    [TestMethod]
    public void CompletionCanStartAnotherPrompt()
    {
        WithTerminal(terminal =>
        {
            var submitted = new List<string>();
            terminal.InputCompleted += (_, e) =>
            {
                submitted.Add(e.InputValue);
                if (submitted.Count == 1)
                    terminal.BeginInput("Next? ");
            };
            terminal.BeginInput("First? ");
            terminal.Type("ONE");
            terminal.Press(Keys.Enter);
            terminal.Type("TWO");
            terminal.Press(Keys.Enter);
            CollectionAssert.AreEqual(new[] { "ONE", "TWO" }, submitted);
        });
    }

    [TestMethod]
    public void PromptNavigationUsesTheStartRowAsWellAsColumn()
    {
        WithTerminal(terminal =>
        {
            terminal.BeginInput("? ");
            terminal.Type(new string('A', 39));
            terminal.Press(Keys.Left);
            Assert.AreEqual(0, terminal.CursorPosition.X);
            terminal.Press(Keys.Left);
            Assert.AreEqual(0, terminal.CursorPosition.Y);
            Assert.AreEqual(39, terminal.CursorPosition.X);
            terminal.Press(Keys.Right);
            Assert.AreEqual(1, terminal.CursorPosition.Y);
            terminal.Press(Keys.Home);
            Assert.AreEqual(0, terminal.CursorPosition.X);
        });
    }

    [TestMethod]
    public void BreakCancelsEventBasedInputWithoutCompletingIt()
    {
        WithTerminal(terminal =>
        {
            var breaks = 0;
            var completed = 0;
            var typed = new List<string>();
            terminal.UserBreak += (_, _) => breaks++;
            terminal.InputCompleted += (_, _) => completed++;
            terminal.TypedLine += (_, e) => typed.Add(e.InputValue);
            terminal.BeginInput("? ");
            terminal.Type("PARTIAL");
            terminal.Press(Keys.Control | Keys.C);
            terminal.BeginInput("Next? ");
            terminal.Type("OK");
            terminal.Press(Keys.Enter);
            terminal.Type("COMMAND");
            terminal.Press(Keys.Enter);
            Assert.AreEqual(1, breaks);
            Assert.AreEqual(1, completed);
            CollectionAssert.AreEqual(new[] { "COMMAND" }, typed);
        });
    }

    [DataTestMethod]
    [DataRow("string")]
    [DataRow("integer")]
    [DataRow("double")]
    public void BreakReturnsFromSynchronousInputWithoutEnter(string kind)
    {
        WithTerminal(terminal =>
        {
            var breaks = 0;
            var completed = 0;
            var returned = false;
            terminal.UserBreak += (_, _) =>
            {
                Assert.IsFalse(returned);
                breaks++;
            };
            terminal.InputCompleted += (_, _) => completed++;
            // Queue the key through the UI message pump used by InputString.
            _ = terminal.Handle;
            terminal.BeginInvoke(new Action(() =>
            {
                terminal.Type("123");
                terminal.Press(Keys.Control | Keys.C);
            }));
            var timedOut = false;
            using var watchdog = new System.Windows.Forms.Timer { Interval = 2000 };
            watchdog.Tick += (_, _) => { timedOut = true; terminal.Quit(); };
            watchdog.Start();
            switch (kind)
            {
                case "string": Assert.AreEqual("", terminal.InputString("? ")); break;
                case "integer": Assert.AreEqual(0, terminal.InputInteger("? ")); break;
                case "double": Assert.AreEqual(0d, terminal.InputDouble("? ")); break;
            }
            returned = true;
            watchdog.Stop();
            Assert.IsFalse(timedOut, "Input waited after Ctrl+C.");
            Assert.IsFalse(terminal.QuitFlag);
            Assert.AreEqual(1, breaks);
            Assert.AreEqual(0, completed);

            terminal.BeginInvoke(new Action(() =>
            {
                terminal.Type("42");
                terminal.Press(Keys.Enter);
            }));
            watchdog.Start();
            Assert.AreEqual("42", terminal.InputString("Next? "));
            watchdog.Stop();
            Assert.IsFalse(timedOut);
        });
    }

    [TestMethod]
    public void ProgramLinesAndShiftEnterKeepTheirBehavior()
    {
        WithTerminal(terminal =>
        {
            terminal.AutoProgramManagement = true;
            var submissions = 0;
            terminal.TypedLine += (_, _) => submissions++;
            terminal.Type("10 PRINT 1");
            terminal.Press(Keys.Enter);
            Assert.AreEqual(1, terminal.ProgramLines.Count);
            terminal.Type("IGNORED");
            terminal.Press(Keys.Shift | Keys.Enter);
            Assert.AreEqual(0, submissions);
        });
    }
}
