namespace MiView.ScreenForms.DialogForm.Setting
{
    partial class APISetting
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
            listBox1 = new ListBox();
            label1 = new Label();
            panel1 = new Panel();
            lbltxtLastReceivedDiff = new Label();
            cmdTimeLineReflex = new Button();
            txtAPIKey = new TextBox();
            chkSetIntg = new CheckBox();
            cmdSettingSave = new Button();
            lbltxtTimeLineKind = new Label();
            lbltxtSoftwareVersion = new Label();
            lbltxtCurrentReceiveState = new Label();
            lbltxtSoftwareName = new Label();
            lbltxtLastReceivedDatetime = new Label();
            lblHostDefinition = new Label();
            lblSetIntg = new Label();
            lblTimeLineKind = new Label();
            lblSoftwareVersion = new Label();
            lblCurrentReceiveState = new Label();
            lblSoftwareName = new Label();
            lblLastReceivedDatetime = new Label();
            lblAPIkey = new Label();
            label4 = new Label();
            label2 = new Label();
            label3 = new Label();
            cmdReConnect = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.Font = new Font("游ゴシック", 12F, FontStyle.Regular, GraphicsUnit.Point, 128);
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 21;
            listBox1.Location = new Point(12, 86);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(159, 361);
            listBox1.TabIndex = 0;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(130, 15);
            label1.TabIndex = 1;
            label1.Text = "現在展開中のタイムライン";
            // 
            // panel1
            // 
            panel1.Controls.Add(cmdReConnect);
            panel1.Controls.Add(lbltxtLastReceivedDiff);
            panel1.Controls.Add(cmdTimeLineReflex);
            panel1.Controls.Add(txtAPIKey);
            panel1.Controls.Add(chkSetIntg);
            panel1.Controls.Add(cmdSettingSave);
            panel1.Controls.Add(lbltxtTimeLineKind);
            panel1.Controls.Add(lbltxtSoftwareVersion);
            panel1.Controls.Add(lbltxtCurrentReceiveState);
            panel1.Controls.Add(lbltxtSoftwareName);
            panel1.Controls.Add(lbltxtLastReceivedDatetime);
            panel1.Controls.Add(lblHostDefinition);
            panel1.Controls.Add(lblSetIntg);
            panel1.Controls.Add(lblTimeLineKind);
            panel1.Controls.Add(lblSoftwareVersion);
            panel1.Controls.Add(lblCurrentReceiveState);
            panel1.Controls.Add(lblSoftwareName);
            panel1.Controls.Add(lblLastReceivedDatetime);
            panel1.Controls.Add(lblAPIkey);
            panel1.Controls.Add(label4);
            panel1.Location = new Point(177, 86);
            panel1.Name = "panel1";
            panel1.Size = new Size(611, 352);
            panel1.TabIndex = 2;
            // 
            // lbltxtLastReceivedDiff
            // 
            lbltxtLastReceivedDiff.AutoSize = true;
            lbltxtLastReceivedDiff.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lbltxtLastReceivedDiff.Location = new Point(353, 106);
            lbltxtLastReceivedDiff.Name = "lbltxtLastReceivedDiff";
            lbltxtLastReceivedDiff.Size = new Size(13, 20);
            lbltxtLastReceivedDiff.TabIndex = 7;
            lbltxtLastReceivedDiff.Text = ".";
            // 
            // cmdTimeLineReflex
            // 
            cmdTimeLineReflex.Location = new Point(5, 269);
            cmdTimeLineReflex.Name = "cmdTimeLineReflex";
            cmdTimeLineReflex.Size = new Size(137, 23);
            cmdTimeLineReflex.TabIndex = 6;
            cmdTimeLineReflex.Text = "反映先タイムライン設定";
            cmdTimeLineReflex.UseVisualStyleBackColor = true;
            cmdTimeLineReflex.Click += cmdTimeLineReflex_Click;
            // 
            // txtAPIKey
            // 
            txtAPIKey.Location = new Point(169, 244);
            txtAPIKey.Name = "txtAPIKey";
            txtAPIKey.Size = new Size(439, 23);
            txtAPIKey.TabIndex = 5;
            // 
            // chkSetIntg
            // 
            chkSetIntg.AutoSize = true;
            chkSetIntg.Location = new Point(207, 186);
            chkSetIntg.Name = "chkSetIntg";
            chkSetIntg.Size = new Size(15, 14);
            chkSetIntg.TabIndex = 4;
            chkSetIntg.UseVisualStyleBackColor = true;
            // 
            // cmdSettingSave
            // 
            cmdSettingSave.Location = new Point(3, 326);
            cmdSettingSave.Name = "cmdSettingSave";
            cmdSettingSave.Size = new Size(75, 23);
            cmdSettingSave.TabIndex = 3;
            cmdSettingSave.Text = "設定保存";
            cmdSettingSave.UseVisualStyleBackColor = true;
            cmdSettingSave.Click += cmdSettingSave_Click;
            // 
            // lbltxtTimeLineKind
            // 
            lbltxtTimeLineKind.AutoSize = true;
            lbltxtTimeLineKind.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lbltxtTimeLineKind.Location = new Point(169, 63);
            lbltxtTimeLineKind.Name = "lbltxtTimeLineKind";
            lbltxtTimeLineKind.Size = new Size(13, 20);
            lbltxtTimeLineKind.TabIndex = 2;
            lbltxtTimeLineKind.Text = ".";
            // 
            // lbltxtSoftwareVersion
            // 
            lbltxtSoftwareVersion.AutoSize = true;
            lbltxtSoftwareVersion.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lbltxtSoftwareVersion.Location = new Point(169, 43);
            lbltxtSoftwareVersion.Name = "lbltxtSoftwareVersion";
            lbltxtSoftwareVersion.Size = new Size(13, 20);
            lbltxtSoftwareVersion.TabIndex = 2;
            lbltxtSoftwareVersion.Text = ".";
            // 
            // lbltxtCurrentReceiveState
            // 
            lbltxtCurrentReceiveState.AutoSize = true;
            lbltxtCurrentReceiveState.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lbltxtCurrentReceiveState.Location = new Point(169, 126);
            lbltxtCurrentReceiveState.Name = "lbltxtCurrentReceiveState";
            lbltxtCurrentReceiveState.Size = new Size(13, 20);
            lbltxtCurrentReceiveState.TabIndex = 2;
            lbltxtCurrentReceiveState.Text = ".";
            // 
            // lbltxtSoftwareName
            // 
            lbltxtSoftwareName.AutoSize = true;
            lbltxtSoftwareName.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lbltxtSoftwareName.Location = new Point(169, 23);
            lbltxtSoftwareName.Name = "lbltxtSoftwareName";
            lbltxtSoftwareName.Size = new Size(13, 20);
            lbltxtSoftwareName.TabIndex = 2;
            lbltxtSoftwareName.Text = ".";
            // 
            // lbltxtLastReceivedDatetime
            // 
            lbltxtLastReceivedDatetime.AutoSize = true;
            lbltxtLastReceivedDatetime.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lbltxtLastReceivedDatetime.Location = new Point(169, 106);
            lbltxtLastReceivedDatetime.Name = "lbltxtLastReceivedDatetime";
            lbltxtLastReceivedDatetime.Size = new Size(13, 20);
            lbltxtLastReceivedDatetime.TabIndex = 2;
            lbltxtLastReceivedDatetime.Text = ".";
            // 
            // lblHostDefinition
            // 
            lblHostDefinition.AutoSize = true;
            lblHostDefinition.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblHostDefinition.Location = new Point(169, 3);
            lblHostDefinition.Name = "lblHostDefinition";
            lblHostDefinition.Size = new Size(13, 20);
            lblHostDefinition.TabIndex = 2;
            lblHostDefinition.Text = ".";
            // 
            // lblSetIntg
            // 
            lblSetIntg.AutoSize = true;
            lblSetIntg.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblSetIntg.Location = new Point(4, 185);
            lblSetIntg.Name = "lblSetIntg";
            lblSetIntg.Size = new Size(102, 20);
            lblSetIntg.TabIndex = 2;
            lblSetIntg.Text = "統合TLに反映";
            // 
            // lblTimeLineKind
            // 
            lblTimeLineKind.AutoSize = true;
            lblTimeLineKind.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblTimeLineKind.Location = new Point(4, 63);
            lblTimeLineKind.Name = "lblTimeLineKind";
            lblTimeLineKind.Size = new Size(127, 20);
            lblTimeLineKind.TabIndex = 2;
            lblTimeLineKind.Text = "購読タイムライン";
            // 
            // lblSoftwareVersion
            // 
            lblSoftwareVersion.AutoSize = true;
            lblSoftwareVersion.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblSoftwareVersion.Location = new Point(4, 43);
            lblSoftwareVersion.Name = "lblSoftwareVersion";
            lblSoftwareVersion.Size = new Size(84, 20);
            lblSoftwareVersion.TabIndex = 2;
            lblSoftwareVersion.Text = "バージョン";
            // 
            // lblCurrentReceiveState
            // 
            lblCurrentReceiveState.AutoSize = true;
            lblCurrentReceiveState.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblCurrentReceiveState.Location = new Point(4, 126);
            lblCurrentReceiveState.Name = "lblCurrentReceiveState";
            lblCurrentReceiveState.Size = new Size(114, 20);
            lblCurrentReceiveState.TabIndex = 2;
            lblCurrentReceiveState.Text = "現在の受信状態";
            // 
            // lblSoftwareName
            // 
            lblSoftwareName.AutoSize = true;
            lblSoftwareName.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblSoftwareName.Location = new Point(4, 23);
            lblSoftwareName.Name = "lblSoftwareName";
            lblSoftwareName.Size = new Size(114, 20);
            lblSoftwareName.TabIndex = 2;
            lblSoftwareName.Text = "ソフトウェア名";
            // 
            // lblLastReceivedDatetime
            // 
            lblLastReceivedDatetime.AutoSize = true;
            lblLastReceivedDatetime.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblLastReceivedDatetime.Location = new Point(4, 106);
            lblLastReceivedDatetime.Name = "lblLastReceivedDatetime";
            lblLastReceivedDatetime.Size = new Size(99, 20);
            lblLastReceivedDatetime.TabIndex = 2;
            lblLastReceivedDatetime.Text = "最終受信日時";
            // 
            // lblAPIkey
            // 
            lblAPIkey.AutoSize = true;
            lblAPIkey.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblAPIkey.Location = new Point(4, 244);
            lblAPIkey.Name = "lblAPIkey";
            lblAPIkey.Size = new Size(63, 20);
            lblAPIkey.TabIndex = 2;
            lblAPIkey.Text = "APIキー";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("游ゴシック", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            label4.Location = new Point(4, 3);
            label4.Name = "label4";
            label4.Size = new Size(54, 20);
            label4.TabIndex = 2;
            label4.Text = "接続先";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(177, 68);
            label2.Name = "label2";
            label2.Size = new Size(84, 15);
            label2.TabIndex = 1;
            label2.Text = "タイムライン情報";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 68);
            label3.Name = "label3";
            label3.Size = new Size(43, 15);
            label3.TabIndex = 1;
            label3.Text = "接続中";
            // 
            // cmdReConnect
            // 
            cmdReConnect.Location = new Point(169, 149);
            cmdReConnect.Name = "cmdReConnect";
            cmdReConnect.Size = new Size(75, 23);
            cmdReConnect.TabIndex = 8;
            cmdReConnect.Text = "再接続";
            cmdReConnect.UseVisualStyleBackColor = true;
            cmdReConnect.Click += cmdReConnect_Click;
            // 
            // APISetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(listBox1);
            Name = "APISetting";
            Text = "API設定";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBox1;
        private Label label1;
        private Panel panel1;
        private Label label2;
        private Label lblLastReceivedDatetime;
        private Label lbltxtLastReceivedDatetime;
        private Label lbltxtCurrentReceiveState;
        private Label lblCurrentReceiveState;
        private TextBox txtLastReceivedDatetime;
        private TextBox txtCurrentReceiveState;
        private Label label3;
        private Label lblHostDefinition;
        private Label label4;
        private Button cmdSettingSave;
        private Label lblSetIntg;
        private CheckBox chkSetIntg;
        private Label lblAPIkey;
        private TextBox txtAPIKey;
        private Label lbltxtSoftwareVersion;
        private Label lbltxtSoftwareName;
        private Label lblSoftwareVersion;
        private Label lblSoftwareName;
        private Label lbltxtLastReceivedDiff;
        private Label lbltxtTimeLineKind;
        private Label lblTimeLineKind;
        private Button cmdTimeLineReflex;
        private Button cmdReConnect;
    }
}