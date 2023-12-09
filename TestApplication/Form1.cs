using TerminalMatrix.TerminalColor;

namespace TestApplication;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void terminalMatrixControl1_TypedLine(object sender, TerminalMatrix.Events.TypedLineEventArgs e)
    {
        Text = $@"{DateTime.Now.ToShortTimeString()} ""{e.InputValue}""";

        if (e.InputValue == "IA")
        {
            terminalMatrixControl1.HorizontalLine(40, ColorName.Cyan);
            terminalMatrixControl1.HorizontalLine(10, 41, 630, ColorName.DarkGrey);
            terminalMatrixControl1.VerticalLine(40, ColorName.Green);
            terminalMatrixControl1.VerticalLine(41, 10, 190, ColorName.White);
            terminalMatrixControl1.Box(ColorName.LightBlue, 43, 43, 63, 63);
            terminalMatrixControl1.PrintAt(ColorName.LightGreen, 20, 10, "ABAB BA abc");
            terminalMatrixControl1.SetStartPosition(20, 10);
            terminalMatrixControl1.UpdateBitmap();
        }
        else if (e.InputValue == "INPUT")
        {
            System.Diagnostics.Debug.WriteLine("Starting input.");
            terminalMatrixControl1.BeginInput("Hello? ");
        }
        else if (e.InputValue == "LIST")
        {
            System.Diagnostics.Debug.WriteLine("Call list.");
            terminalMatrixControl1.List();
        }
        else if (e.InputValue == "NEW")
        {
            System.Diagnostics.Debug.WriteLine("Call new.");
            terminalMatrixControl1.New();
        }
    }

    private void terminalMatrixControl1_InputCompleted(object sender, TerminalMatrix.Events.TypedLineEventArgs e)
    {
        Text = $@"INPUT COMPLETED {DateTime.Now.ToShortTimeString()} ""{e.InputValue}""";
    }
}