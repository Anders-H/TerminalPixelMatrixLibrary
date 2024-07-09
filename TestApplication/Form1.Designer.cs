namespace TestApplication
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            terminalMatrixControl1 = new TerminalMatrix.TerminalMatrixControl();
            SuspendLayout();
            // 
            // terminalMatrixControl1
            // 
            terminalMatrixControl1.AutoProgramManagement = false;
            terminalMatrixControl1.BorderHeight = 20;
            terminalMatrixControl1.BorderWidth = 20;
            terminalMatrixControl1.ControlOverlayPainter = null;
            terminalMatrixControl1.CurrentCursorColor = 5;
            terminalMatrixControl1.Dock = DockStyle.Fill;
            terminalMatrixControl1.Location = new Point(0, 0);
            terminalMatrixControl1.Name = "terminalMatrixControl1";
            terminalMatrixControl1.RenderingMode = TerminalMatrix.RenderingMode.HighSpeed;
            terminalMatrixControl1.Size = new Size(1054, 691);
            terminalMatrixControl1.TabIndex = 0;
            terminalMatrixControl1.UnlimitedInput = true;
            terminalMatrixControl1.Use32BitForeground = false;
            terminalMatrixControl1.UseBackground24Bit = false;
            terminalMatrixControl1.TypedLine += terminalMatrixControl1_TypedLine;
            terminalMatrixControl1.InputCompleted += terminalMatrixControl1_InputCompleted;
            terminalMatrixControl1.UserBreak += terminalMatrixControl1_UserBreak;
            terminalMatrixControl1.RequestToggleFullscreen += terminalMatrixControl1_RequestToggleFullscreen;
            terminalMatrixControl1.FunctionKeyPressed += terminalMatrixControl1_FunctionKeyPressed;
            terminalMatrixControl1.Tick += terminalMatrixControl1_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1054, 691);
            Controls.Add(terminalMatrixControl1);
            DoubleBuffered = true;
            Name = "Form1";
            Text = "Form1";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private TerminalMatrix.TerminalMatrixControl terminalMatrixControl1;
    }
}
