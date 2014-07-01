using System.Windows.Forms;
namespace VikingGameServer {
    partial class FormServerMain {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.TextBoxConsole = new System.Windows.Forms.TextBox();
            this.ButtonStartStop = new System.Windows.Forms.Button();
            this.StatusBar = new System.Windows.Forms.StatusBar();
            this.LabelFps = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TextBoxConsole
            // 
            this.TextBoxConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxConsole.Location = new System.Drawing.Point(12, 12);
            this.TextBoxConsole.Multiline = true;
            this.TextBoxConsole.Name = "TextBoxConsole";
            this.TextBoxConsole.ReadOnly = true;
            this.TextBoxConsole.Size = new System.Drawing.Size(260, 209);
            this.TextBoxConsole.TabIndex = 0;
            // 
            // ButtonStartStop
            // 
            this.ButtonStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ButtonStartStop.Location = new System.Drawing.Point(12, 227);
            this.ButtonStartStop.Name = "ButtonStartStop";
            this.ButtonStartStop.Size = new System.Drawing.Size(75, 23);
            this.ButtonStartStop.TabIndex = 1;
            this.ButtonStartStop.Text = "Start";
            this.ButtonStartStop.UseVisualStyleBackColor = true;
            this.ButtonStartStop.Click += new System.EventHandler(this.ButtonStartStop_Click);
            // 
            // StatusBar
            // 
            this.StatusBar.Location = new System.Drawing.Point(0, 0);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(100, 22);
            this.StatusBar.TabIndex = 0;
            // 
            // LabelFps
            // 
            this.LabelFps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelFps.AutoSize = true;
            this.LabelFps.Location = new System.Drawing.Point(227, 224);
            this.LabelFps.Name = "LabelFps";
            this.LabelFps.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelFps.Size = new System.Drawing.Size(45, 13);
            this.LabelFps.TabIndex = 2;
            this.LabelFps.Text = "FPS: 0  ";
            this.LabelFps.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // FormServerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.LabelFps);
            this.Controls.Add(this.ButtonStartStop);
            this.Controls.Add(this.TextBoxConsole);
            this.Name = "FormServerMain";
            this.Text = "Viking Game Server";
            this.Load += new System.EventHandler(this.FormServerMain_Load);
            this.FormClosing += new FormClosingEventHandler(this.FormServerMain_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private volatile System.Windows.Forms.TextBox TextBoxConsole;
        private System.Windows.Forms.Button ButtonStartStop;
        private System.Windows.Forms.StatusBar StatusBar;
        private System.Windows.Forms.Label LabelFps;
    }
}

