namespace SlideshowExample;

public partial class MainWindow : Form
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainWindow_Shown(object sender, EventArgs e)
    {
        terminalMatrixControl1.SetTextRenderLimit(23);
        terminalMatrixControl1.WriteLine("Loading...");
        Refresh();
        var images = new List<byte[,]>();

        for (var i = 1; i < 4; i++)
            images.Add(terminalMatrixControl1.LoadPictureFromGif($@"..\..\..\slideshow{i}.gif"));

        var quitFlag = false;
        
        do
        {
            for (var i = 1; i <= images.Count; i++)
            {
                var gif = images[i - 1];
                terminalMatrixControl1.SetPixels(0, 0, gif);
                terminalMatrixControl1.WriteLine($"Image {i} of {images.Count}. Press Enter or type QUIT.");
                var response = terminalMatrixControl1.Input(">").ToUpper().Trim();

                if (response == "QUIT")
                {
                    quitFlag = true;
                    Close();
                    break;
                }
            }
        } while (!quitFlag && !terminalMatrixControl1.QuitFlag);
    }

    private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
    {
        terminalMatrixControl1.Quit();
    }
}