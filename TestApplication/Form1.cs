using TerminalMatrix;

namespace TestApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            terminalMatrixControl1.UpdateBitmap();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            terminalMatrixControl1.HorizontalLine(40, ColorName.Cyan);
            terminalMatrixControl1.UpdateBitmap();
        }
    }
}
