namespace MiView.ScreenForms.DialogForm.Viewer
{
    partial class StasticTimeLine
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
            label1 = new Label();
            cmbTimeLine = new ComboBox();
            panel1 = new Panel();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(84, 15);
            label1.TabIndex = 0;
            label1.Text = "タイムライン選択";
            // 
            // cmbTimeLine
            // 
            cmbTimeLine.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTimeLine.FormattingEnabled = true;
            cmbTimeLine.Location = new Point(102, 6);
            cmbTimeLine.Name = "cmbTimeLine";
            cmbTimeLine.Size = new Size(383, 23);
            cmbTimeLine.TabIndex = 1;
            cmbTimeLine.SelectedIndexChanged += cmbTimeLine_SelectedIndexChanged;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Location = new Point(2, 35);
            panel1.Name = "panel1";
            panel1.Size = new Size(492, 413);
            panel1.TabIndex = 2;
            // 
            // StasticTimeLine
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(495, 450);
            Controls.Add(panel1);
            Controls.Add(cmbTimeLine);
            Controls.Add(label1);
            Name = "StasticTimeLine";
            Text = "StasticTimeLine";
            VisibleChanged += StasticTimeLine_VisibleChanged;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox cmbTimeLine;
        private Panel panel1;
    }
}