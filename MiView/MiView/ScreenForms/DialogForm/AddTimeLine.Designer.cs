namespace MiView.ScreenForms.DialogForm
{
    partial class AddTimeLine
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
            txtTabName = new TextBox();
            label2 = new Label();
            txtTabDefinition = new TextBox();
            cmdAddTab = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(37, 15);
            label1.TabIndex = 0;
            label1.Text = "タブ名";
            // 
            // txtTabName
            // 
            txtTabName.Location = new Point(77, 6);
            txtTabName.Name = "txtTabName";
            txtTabName.Size = new Size(401, 23);
            txtTabName.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 38);
            label2.Name = "label2";
            label2.Size = new Size(43, 15);
            label2.TabIndex = 0;
            label2.Text = "識別値";
            // 
            // txtTabDefinition
            // 
            txtTabDefinition.Enabled = false;
            txtTabDefinition.Location = new Point(77, 35);
            txtTabDefinition.Name = "txtTabDefinition";
            txtTabDefinition.ReadOnly = true;
            txtTabDefinition.Size = new Size(401, 23);
            txtTabDefinition.TabIndex = 1;
            // 
            // cmdAddTab
            // 
            cmdAddTab.Location = new Point(12, 75);
            cmdAddTab.Name = "cmdAddTab";
            cmdAddTab.Size = new Size(75, 23);
            cmdAddTab.TabIndex = 2;
            cmdAddTab.Text = "追加";
            cmdAddTab.UseVisualStyleBackColor = true;
            cmdAddTab.Click += cmdAddTab_Click;
            // 
            // AddTimeLine
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(509, 113);
            Controls.Add(cmdAddTab);
            Controls.Add(txtTabDefinition);
            Controls.Add(txtTabName);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "AddTimeLine";
            Text = "タブ追加";
            Load += AddTimeLine_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtTabName;
        private Label label2;
        private TextBox txtTabDefinition;
        private Button cmdAddTab;
    }
}