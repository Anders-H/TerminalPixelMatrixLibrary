using PixelmapLibrary.SpriteManagement;
using TerminalMatrix;
using TerminalMatrix.TerminalColor;

namespace AdventureGameExample;

public partial class MainWindow : Form
{
    private readonly StillImageSprite _image;

    public MainWindow()
    {
        _image = new StillImageSprite(@"..\..\..\..\testgif.gif");
        InitializeComponent();
    }

    private void MainWindow_Load(object sender, EventArgs e)
    {
        terminalMatrixControl1.SetResolution(Resolution.Pixels640x200Characters80x25);
        terminalMatrixControl1.SetPixelsToBackground(_image, 0, 0);
        terminalMatrixControl1.UseBackground24Bit = true;
    }

    private void MainWindow_Shown(object sender, EventArgs e)
    {
        terminalMatrixControl1.SetTextRenderLimit(13);
        Refresh();
        var quitFlag = false;

        do
        {
            terminalMatrixControl1.CurrentCursorColor = (int)ColorName.Yellow;
            terminalMatrixControl1.WriteText("You are in a forest.");
            terminalMatrixControl1.CurrentCursorColor = (int)ColorName.Cyan;
            var input = terminalMatrixControl1.InputString(">");

            if (input == "QUIT" || terminalMatrixControl1.QuitFlag)
                quitFlag = true;

        } while (!quitFlag);

        Close();
    }

    private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
    {
        terminalMatrixControl1.Quit();
    }
}