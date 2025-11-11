namespace MiView.ScreenForms.DialogForm
{
    partial class Splash
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            pictureBox1 = new PictureBox();
            lblTitle = new Label();
            lblVerTitle = new Label();
            lblVersion = new Label();
            progWait = new ProgressBar();
            lblMsg = new Label();
            lblAnim = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(32, 96);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(407, 95);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(32, 209);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(46, 15);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "MiView";
            // 
            // lblVerTitle
            // 
            lblVerTitle.AutoSize = true;
            lblVerTitle.Location = new Point(32, 236);
            lblVerTitle.Name = "lblVerTitle";
            lblVerTitle.Size = new Size(45, 15);
            lblVerTitle.TabIndex = 1;
            lblVerTitle.Text = "Version";
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Location = new Point(83, 236);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(45, 15);
            lblVersion.TabIndex = 1;
            lblVersion.Text = "Version";
            // 
            // progWait
            // 
            progWait.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progWait.BackColor = SystemColors.ControlLightLight;
            progWait.Location = new Point(0, 299);
            progWait.Name = "progWait";
            progWait.Size = new Size(480, 23);
            progWait.TabIndex = 2;
            // 
            // lblMsg
            // 
            lblMsg.Location = new Point(12, 272);
            lblMsg.Name = "lblMsg";
            lblMsg.Size = new Size(456, 24);
            lblMsg.TabIndex = 3;
            // 
            // lblAnim
            // 
            lblAnim.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblAnim.Location = new Point(12, 340);
            lblAnim.Name = "lblAnim";
            lblAnim.Size = new Size(456, 36);
            lblAnim.TabIndex = 4;
            lblAnim.Text = "label1";
            lblAnim.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Splash
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(480, 385);
            Controls.Add(lblAnim);
            Controls.Add(lblMsg);
            Controls.Add(progWait);
            Controls.Add(lblVersion);
            Controls.Add(lblVerTitle);
            Controls.Add(lblTitle);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Splash";
            Text = "Splash";
            Load += Splash_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lblTitle;
        private Label lblVerTitle;
        private Label lblVersion;
        private ProgressBar progWait;
        private Label lblMsg;
        private Label lblAnim;
    }
}