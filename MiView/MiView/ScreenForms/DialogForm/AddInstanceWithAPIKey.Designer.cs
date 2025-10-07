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
            cmbSoftware = new ComboBox();
            label5 = new Label();
            label6 = new Label();
            txtSoftwareVersion = new TextBox();
            cmdGetVersionInfo = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(82, 15);
            label1.TabIndex = 0;
            label1.Text = "インスタンスURL";
            // 
            // txtInstanceURL
            // 
            txtInstanceURL.Location = new Point(100, 6);
            txtInstanceURL.Name = "txtInstanceURL";
            txtInstanceURL.Size = new Size(348, 23);
            txtInstanceURL.TabIndex = 1;
            // 
            // txtAPIKey
            // 
            txtAPIKey.Location = new Point(100, 35);
            txtAPIKey.Name = "txtAPIKey";
            txtAPIKey.Size = new Size(348, 23);
            txtAPIKey.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 38);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 0;
            label2.Text = "APIキー";
            // 
            // cmdApply
            // 
            cmdApply.Location = new Point(373, 194);
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
            label3.Location = new Point(12, 67);
            label3.Name = "label3";
            label3.Size = new Size(49, 15);
            label3.TabIndex = 0;
            label3.Text = "タブ名称";
            // 
            // txtTabName
            // 
            txtTabName.Location = new Point(100, 64);
            txtTabName.Name = "txtTabName";
            txtTabName.Size = new Size(348, 23);
            txtTabName.TabIndex = 1;
            // 
            // cmbTLKind
            // 
            cmbTLKind.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTLKind.FormattingEnabled = true;
            cmbTLKind.Items.AddRange(new object[] { "ホームTL", "ローカルTL", "ソーシャルTL", "グローバルTL" });
            cmbTLKind.Location = new Point(100, 93);
            cmbTLKind.Name = "cmbTLKind";
            cmbTLKind.Size = new Size(348, 23);
            cmbTLKind.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 96);
            label4.Name = "label4";
            label4.Size = new Size(43, 15);
            label4.TabIndex = 0;
            label4.Text = "TL種類";
            // 
            // cmbSoftware
            // 
            cmbSoftware.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSoftware.Enabled = false;
            cmbSoftware.FormattingEnabled = true;
            cmbSoftware.Items.AddRange(new object[] { "ホームTL", "ローカルTL", "ソーシャルTL", "グローバルTL" });
            cmbSoftware.Location = new Point(100, 122);
            cmbSoftware.Name = "cmbSoftware";
            cmbSoftware.Size = new Size(348, 23);
            cmbSoftware.TabIndex = 3;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 125);
            label5.Name = "label5";
            label5.Size = new Size(57, 15);
            label5.TabIndex = 0;
            label5.Text = "ソフトウェア";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 154);
            label6.Name = "label6";
            label6.Size = new Size(51, 15);
            label6.TabIndex = 0;
            label6.Text = "バージョン";
            // 
            // txtSoftwareVersion
            // 
            txtSoftwareVersion.Enabled = false;
            txtSoftwareVersion.Location = new Point(100, 151);
            txtSoftwareVersion.Name = "txtSoftwareVersion";
            txtSoftwareVersion.Size = new Size(348, 23);
            txtSoftwareVersion.TabIndex = 1;
            // 
            // cmdGetVersionInfo
            // 
            cmdGetVersionInfo.Location = new Point(292, 194);
            cmdGetVersionInfo.Name = "cmdGetVersionInfo";
            cmdGetVersionInfo.Size = new Size(75, 23);
            cmdGetVersionInfo.TabIndex = 2;
            cmdGetVersionInfo.Text = "認証";
            cmdGetVersionInfo.UseVisualStyleBackColor = true;
            cmdGetVersionInfo.Click += cmdGetVersionInfo_Click;
            // 
            // AddInstanceWithAPIKey
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(460, 229);
            Controls.Add(cmbSoftware);
            Controls.Add(cmbTLKind);
            Controls.Add(cmdGetVersionInfo);
            Controls.Add(cmdApply);
            Controls.Add(txtSoftwareVersion);
            Controls.Add(txtTabName);
            Controls.Add(txtAPIKey);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(txtInstanceURL);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "AddInstanceWithAPIKey";
            StartPosition = FormStartPosition.CenterParent;
            Text = "インスタンス追加";
            Load += AddInstanceWithAPIKey_Load;
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
        private ComboBox cmbSoftware;
        private Label label5;
        private Label label6;
        private TextBox txtSoftwareVersion;
        private Button cmdGetVersionInfo;
    }
}