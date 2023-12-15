using TerminalMatrix;
using TerminalMatrix.TerminalColor;

namespace AdventureGameExample;

public partial class MainWindow : Form
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainWindow_Shown(object sender, EventArgs e)
    {
        terminalMatrixControl1.SetTextRenderLimit(13);
        var gif = terminalMatrixControl1.LoadPictureFromGif(@"..\..\..\..\testgif.gif");
        terminalMatrixControl1.SetPixels(0, 0, gif);
        Refresh();

        var quitFlag = false;

        do
        {
            terminalMatrixControl1.CurrentCursorColor = (int)ColorName.Yellow;
            terminalMatrixControl1.WriteLine("You are in a forest.");
            terminalMatrixControl1.CurrentCursorColor = (int)ColorName.Cyan;
            var input = terminalMatrixControl1.Input(">");
        } while (!quitFlag);
    }
}