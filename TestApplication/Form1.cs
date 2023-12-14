using TerminalMatrix;
using TerminalMatrix.TerminalColor;

namespace TestApplication;

public partial class Form1 : Form
{
    private Random _rnd = new Random();

    public Form1()
    {
        InitializeComponent();
    }

    private void terminalMatrixControl1_TypedLine(object sender, TerminalMatrix.Events.TypedLineEventArgs e)
    {
        Text = $@"{DateTime.Now.ToShortTimeString()} ""{e.InputValue}""";

        if (e.InputValue == "IA")
        {
            for (int i = 0; i < 300; i++)
                terminalMatrixControl1.SetPixel(_rnd.Next(640), _rnd.Next(200), ColorName.Red);

            terminalMatrixControl1.SetLayerOrder(LayerOrder.GraphicsOverText);
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
        else if (e.InputValue == "INLINE INPUT")
        {
            var response = terminalMatrixControl1.Input("What now? ");
            terminalMatrixControl1.WriteLine($"You say: {response}");
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
        else if (e.InputValue == "RED")
        {
            terminalMatrixControl1.CurrentCursorColor = (int)ColorName.Red;
        }
        else if (e.InputValue == "LIMIT")
        {
            terminalMatrixControl1.SetTextRenderLimit(23);
        }
        else if (e.InputValue == "GIF")
        {
            var gif = terminalMatrixControl1.LoadPictureFromGif(@"..\..\..\..\testgif.gif");
            terminalMatrixControl1.SetPixels(0, 0, gif);
            terminalMatrixControl1.UpdateBitmap();
        }
    }

    private void terminalMatrixControl1_InputCompleted(object sender, TerminalMatrix.Events.TypedLineEventArgs e)
    {
        Text = $@"INPUT COMPLETED {DateTime.Now.ToShortTimeString()} ""{e.InputValue}""";
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        terminalMatrixControl1.WriteLine($"{new string(' ', 28)}*** A BASIC LANGUAGE ***");
    }
}