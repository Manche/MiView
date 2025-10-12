namespace MiView.ScreenForms.DialogForm
{
    partial class AddInstanceWithAPIKey
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
            txtInstanceURL = new TextBox();
            txtAPIKey = new TextBox();
            label2 = new Label();
            cmdApply = new Button();
            label3 = new Label();
            txtTabName = new TextBox();
            cmbTLKind = new ComboBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            txtSoftwareVersion = new TextBox();
            cmdGetVersionInfo = new Button();
            panel1 = new Panel();
            panel2 = new Panel();
            statusStrip1 = new StatusStrip();
            tstxtStatus = new ToolStripStatusLabel();
            txtSoftwareName = new TextBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(11, 8);
            label1.Name = "label1";
            label1.Size = new Size(82, 15);
            label1.TabIndex = 0;
            label1.Text = "インスタンスURL";
            // 
            // txtInstanceURL
            // 
            txtInstanceURL.Location = new Point(99, 5);
            txtInstanceURL.Name = "txtInstanceURL";
            txtInstanceURL.Size = new Size(348, 23);
            txtInstanceURL.TabIndex = 1;
            // 
            // txtAPIKey
            // 
            txtAPIKey.Location = new Point(99, 34);
            txtAPIKey.Name = "txtAPIKey";
            txtAPIKey.Size = new Size(348, 23);
            txtAPIKey.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(11, 37);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 0;
            label2.Text = "APIキー";
            // 
            // cmdApply
            // 
            cmdApply.Enabled = false;
            cmdApply.Location = new Point(372, 78);
            cmdApply.Name = "cmdApply";
            cmdApply.Size = new Size(75, 23);
            cmdApply.TabIndex = 2;
            cmdApply.Text = "追加";
            cmdApply.UseVisualStyleBackColor = true;
            cmdApply.Click += cmdApply_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 66);
            label3.Name = "label3";
            label3.Size = new Size(49, 15);
            label3.TabIndex = 0;
            label3.Text = "タブ名称";
            // 
            // txtTabName
            // 
            txtTabName.Location = new Point(99, 63);
            txtTabName.Name = "txtTabName";
            txtTabName.Size = new Size(348, 23);
            txtTabName.TabIndex = 1;
            // 
            // cmbTLKind
            // 
            cmbTLKind.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTLKind.FormattingEnabled = true;
            cmbTLKind.Items.AddRange(new object[] { "ホームTL", "ローカルTL", "ソーシャルTL", "グローバルTL" });
            cmbTLKind.Location = new Point(99, 92);
            cmbTLKind.Name = "cmbTLKind";
            cmbTLKind.Size = new Size(348, 23);
            cmbTLKind.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(11, 95);
            label4.Name = "label4";
            label4.Size = new Size(43, 15);
            label4.TabIndex = 0;
            label4.Text = "TL種類";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(11, 9);
            label5.Name = "label5";
            label5.Size = new Size(57, 15);
            label5.TabIndex = 0;
            label5.Text = "ソフトウェア";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(11, 38);
            label6.Name = "label6";
            label6.Size = new Size(51, 15);
            label6.TabIndex = 0;
            label6.Text = "バージョン";
            // 
            // txtSoftwareVersion
            // 
            txtSoftwareVersion.Enabled = false;
            txtSoftwareVersion.Location = new Point(99, 35);
            txtSoftwareVersion.Name = "txtSoftwareVersion";
            txtSoftwareVersion.Size = new Size(348, 23);
            txtSoftwareVersion.TabIndex = 1;
            // 
            // cmdGetVersionInfo
            // 
            cmdGetVersionInfo.Location = new Point(372, 121);
            cmdGetVersionInfo.Name = "cmdGetVersionInfo";
            cmdGetVersionInfo.Size = new Size(75, 23);
            cmdGetVersionInfo.TabIndex = 2;
            cmdGetVersionInfo.Text = "認証";
            cmdGetVersionInfo.UseVisualStyleBackColor = true;
            cmdGetVersionInfo.Click += cmdGetVersionInfo_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(cmbTLKind);
            panel1.Controls.Add(txtInstanceURL);
            panel1.Controls.Add(cmdGetVersionInfo);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(txtAPIKey);
            panel1.Controls.Add(txtTabName);
            panel1.Location = new Point(1, 1);
            panel1.Name = "panel1";
            panel1.Size = new Size(459, 153);
            panel1.TabIndex = 4;
            // 
            // panel2
            // 
            panel2.Controls.Add(label5);
            panel2.Controls.Add(label6);
            panel2.Controls.Add(txtSoftwareName);
            panel2.Controls.Add(txtSoftwareVersion);
            panel2.Controls.Add(cmdApply);
            panel2.Location = new Point(1, 155);
            panel2.Name = "panel2";
            panel2.Size = new Size(459, 107);
            panel2.TabIndex = 5;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tstxtStatus });
            statusStrip1.Location = new Point(0, 321);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(460, 22);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // tstxtStatus
            // 
            tstxtStatus.Name = "tstxtStatus";
            tstxtStatus.Size = new Size(10, 17);
            tstxtStatus.Text = ".";
            // 
            // txtSoftwareName
            // 
            txtSoftwareName.Enabled = false;
            txtSoftwareName.Location = new Point(99, 6);
            txtSoftwareName.Name = "txtSoftwareName";
            txtSoftwareName.Size = new Size(348, 23);
            txtSoftwareName.TabIndex = 1;
            // 
            // AddInstanceWithAPIKey
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(460, 343);
            Controls.Add(statusStrip1);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "AddInstanceWithAPIKey";
            StartPosition = FormStartPosition.CenterParent;
            Text = "インスタンス追加";
            Load += AddInstanceWithAPIKey_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtInstanceURL;
        private TextBox txtAPIKey;
        private Label label2;
        private Button cmdApply;
        private Label label3;
        private TextBox txtTabName;
        private ComboBox cmbTLKind;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox txtSoftwareVersion;
        private Button cmdGetVersionInfo;
        private Panel panel1;
        private Panel panel2;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel tstxtStatus;
        private TextBox txtSoftwareName;
    }
}