using TerminalMatrix.TerminalColor;

namespace TestApplication;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        terminalMatrixControl1.SetStartPosition(5, 5);
        terminalMatrixControl1.UpdateBitmap();
    }

    private void terminalMatrixControl1_TypedLine(object sender, TerminalMatrix.Events.TypedLineEventArgs e)
    {
        Text = $@"{DateTime.Now.ToShortTimeString()} ({e.InputStart.X}, {e.InputStart.Y})-({e.InputEnd.X}, {e.InputEnd.Y})  ""{e.InputValue}""";

        if (e.InputValue == "IA")
        {
            e.CancelNewLine = true;
            terminalMatrixControl1.HorizontalLine(40, ColorName.Cyan);
            terminalMatrixControl1.HorizontalLine(10, 41, 630, ColorName.DarkGrey);
            terminalMatrixControl1.VerticalLine(40, ColorName.Green);
            terminalMatrixControl1.VerticalLine(41, 10, 190, ColorName.White);
            terminalMatrixControl1.Box(ColorName.LightBlue, 43, 43, 63, 63);
            terminalMatrixControl1.PrintAt(ColorName.LightGreen, 20, 10, "ABAB BA");
            terminalMatrixControl1.SetStartPosition(20, 10);
            terminalMatrixControl1.UpdateBitmap();
        }
    }
}