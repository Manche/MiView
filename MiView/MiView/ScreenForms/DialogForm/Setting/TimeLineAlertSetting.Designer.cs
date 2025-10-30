namespace MiView.ScreenForms.DialogForm.Setting
{
    partial class TimeLineAlertSetting
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
            cmdCopyAlert = new Button();
            cmdCreateAlert = new Button();
            cmdLoadAlert = new Button();
            cmbAlertSelect = new ComboBox();
            label1 = new Label();
            pnAlert = new Panel();
            cmdDeleteAlertNotification = new Button();
            cmdAddAlertNotification = new Button();
            cmbAlertTiming = new ComboBox();
            label5 = new Label();
            lstNotification = new ListBox();
            txtAlertDefinition = new TextBox();
            txtAlertName = new TextBox();
            label3 = new Label();
            label4 = new Label();
            label2 = new Label();
            cmdDeleteAlert = new Button();
            cmdRevertAlert = new Button();
            cmdSaveAlert = new Button();
            cmdEditAlertNotification = new Button();
            pnAlert.SuspendLayout();
            SuspendLayout();
            // 
            // cmdCopyAlert
            // 
            cmdCopyAlert.Location = new Point(93, 56);
            cmdCopyAlert.Name = "cmdCopyAlert";
            cmdCopyAlert.Size = new Size(75, 23);
            cmdCopyAlert.TabIndex = 10;
            cmdCopyAlert.Text = "コピー";
            cmdCopyAlert.UseVisualStyleBackColor = true;
            cmdCopyAlert.Click += cmdCopyAlert_Click;
            // 
            // cmdCreateAlert
            // 
            cmdCreateAlert.Location = new Point(12, 85);
            cmdCreateAlert.Name = "cmdCreateAlert";
            cmdCreateAlert.Size = new Size(75, 23);
            cmdCreateAlert.TabIndex = 11;
            cmdCreateAlert.Text = "新規作成";
            cmdCreateAlert.UseVisualStyleBackColor = true;
            cmdCreateAlert.Click += cmdCreateAlert_Click;
            // 
            // cmdLoadAlert
            // 
            cmdLoadAlert.Location = new Point(12, 56);
            cmdLoadAlert.Name = "cmdLoadAlert";
            cmdLoadAlert.Size = new Size(75, 23);
            cmdLoadAlert.TabIndex = 12;
            cmdLoadAlert.Text = "読み込み";
            cmdLoadAlert.UseVisualStyleBackColor = true;
            cmdLoadAlert.Click += cmdLoadAlert_Click;
            // 
            // cmbAlertSelect
            // 
            cmbAlertSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAlertSelect.FormattingEnabled = true;
            cmbAlertSelect.Location = new Point(12, 27);
            cmbAlertSelect.Name = "cmbAlertSelect";
            cmbAlertSelect.Size = new Size(776, 23);
            cmbAlertSelect.TabIndex = 9;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(64, 15);
            label1.TabIndex = 8;
            label1.Text = "アラート選択";
            // 
            // pnAlert
            // 
            pnAlert.Controls.Add(cmdEditAlertNotification);
            pnAlert.Controls.Add(cmdDeleteAlertNotification);
            pnAlert.Controls.Add(cmdAddAlertNotification);
            pnAlert.Controls.Add(cmbAlertTiming);
            pnAlert.Controls.Add(label5);
            pnAlert.Controls.Add(lstNotification);
            pnAlert.Controls.Add(txtAlertDefinition);
            pnAlert.Controls.Add(txtAlertName);
            pnAlert.Controls.Add(label3);
            pnAlert.Controls.Add(label4);
            pnAlert.Controls.Add(label2);
            pnAlert.Controls.Add(cmdDeleteAlert);
            pnAlert.Controls.Add(cmdRevertAlert);
            pnAlert.Controls.Add(cmdSaveAlert);
            pnAlert.Location = new Point(12, 114);
            pnAlert.Name = "pnAlert";
            pnAlert.Size = new Size(776, 324);
            pnAlert.TabIndex = 13;
            // 
            // cmdDeleteAlertNotification
            // 
            cmdDeleteAlertNotification.Location = new Point(430, 248);
            cmdDeleteAlertNotification.Name = "cmdDeleteAlertNotification";
            cmdDeleteAlertNotification.Size = new Size(75, 23);
            cmdDeleteAlertNotification.TabIndex = 18;
            cmdDeleteAlertNotification.Text = "削除";
            cmdDeleteAlertNotification.UseVisualStyleBackColor = true;
            cmdDeleteAlertNotification.Click += cmdDeleteAlertNotification_Click;
            // 
            // cmdAddAlertNotification
            // 
            cmdAddAlertNotification.Location = new Point(430, 177);
            cmdAddAlertNotification.Name = "cmdAddAlertNotification";
            cmdAddAlertNotification.Size = new Size(75, 23);
            cmdAddAlertNotification.TabIndex = 18;
            cmdAddAlertNotification.Text = "追加";
            cmdAddAlertNotification.UseVisualStyleBackColor = true;
            cmdAddAlertNotification.Click += cmdAddAlertNotification_Click;
            // 
            // cmbAlertTiming
            // 
            cmbAlertTiming.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAlertTiming.FormattingEnabled = true;
            cmbAlertTiming.Location = new Point(81, 109);
            cmbAlertTiming.Name = "cmbAlertTiming";
            cmbAlertTiming.Size = new Size(266, 23);
            cmbAlertTiming.TabIndex = 17;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 112);
            label5.Name = "label5";
            label5.Size = new Size(74, 15);
            label5.TabIndex = 16;
            label5.Text = "通知タイミング";
            // 
            // lstNotification
            // 
            lstNotification.FormattingEnabled = true;
            lstNotification.ItemHeight = 15;
            lstNotification.Location = new Point(81, 177);
            lstNotification.Name = "lstNotification";
            lstNotification.Size = new Size(343, 124);
            lstNotification.TabIndex = 15;
            // 
            // txtAlertDefinition
            // 
            txtAlertDefinition.Enabled = false;
            txtAlertDefinition.Location = new Point(81, 36);
            txtAlertDefinition.Name = "txtAlertDefinition";
            txtAlertDefinition.Size = new Size(266, 23);
            txtAlertDefinition.TabIndex = 13;
            // 
            // txtAlertName
            // 
            txtAlertName.Location = new Point(81, 65);
            txtAlertName.Name = "txtAlertName";
            txtAlertName.Size = new Size(266, 23);
            txtAlertName.TabIndex = 14;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 39);
            label3.Name = "label3";
            label3.Size = new Size(76, 15);
            label3.TabIndex = 11;
            label3.Text = "アラート識別子";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(3, 177);
            label4.Name = "label4";
            label4.Size = new Size(64, 15);
            label4.TabIndex = 12;
            label4.Text = "アラート方法";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 68);
            label2.Name = "label2";
            label2.Size = new Size(52, 15);
            label2.TabIndex = 12;
            label2.Text = "アラート名";
            // 
            // cmdDeleteAlert
            // 
            cmdDeleteAlert.Location = new Point(174, 3);
            cmdDeleteAlert.Name = "cmdDeleteAlert";
            cmdDeleteAlert.Size = new Size(75, 23);
            cmdDeleteAlert.TabIndex = 8;
            cmdDeleteAlert.Text = "削除";
            cmdDeleteAlert.UseVisualStyleBackColor = true;
            cmdDeleteAlert.Click += cmdDeleteAlert_Click;
            // 
            // cmdRevertAlert
            // 
            cmdRevertAlert.Location = new Point(81, 3);
            cmdRevertAlert.Name = "cmdRevertAlert";
            cmdRevertAlert.Size = new Size(87, 23);
            cmdRevertAlert.TabIndex = 9;
            cmdRevertAlert.Text = "編集取り消し";
            cmdRevertAlert.UseVisualStyleBackColor = true;
            cmdRevertAlert.Click += cmdRevertAlert_Click;
            // 
            // cmdSaveAlert
            // 
            cmdSaveAlert.Location = new Point(3, 3);
            cmdSaveAlert.Name = "cmdSaveAlert";
            cmdSaveAlert.Size = new Size(75, 23);
            cmdSaveAlert.TabIndex = 10;
            cmdSaveAlert.Text = "保存";
            cmdSaveAlert.UseVisualStyleBackColor = true;
            cmdSaveAlert.Click += cmdSaveAlert_Click;
            // 
            // cmdEditAlertNotification
            // 
            cmdEditAlertNotification.Location = new Point(430, 206);
            cmdEditAlertNotification.Name = "cmdEditAlertNotification";
            cmdEditAlertNotification.Size = new Size(75, 23);
            cmdEditAlertNotification.TabIndex = 18;
            cmdEditAlertNotification.Text = "削除";
            cmdEditAlertNotification.UseVisualStyleBackColor = true;
            cmdEditAlertNotification.Click += cmdEditAlertNotification_Click;
            // 
            // TimeLineAlertSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pnAlert);
            Controls.Add(cmdCopyAlert);
            Controls.Add(cmdCreateAlert);
            Controls.Add(cmdLoadAlert);
            Controls.Add(cmbAlertSelect);
            Controls.Add(label1);
            Name = "TimeLineAlertSetting";
            Text = "アラート設定";
            Load += TimeLineAlertSetting_Load;
            pnAlert.ResumeLayout(false);
            pnAlert.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button cmdCopyAlert;
        private Button cmdCreateAlert;
        private Button cmdLoadAlert;
        private ComboBox cmbAlertSelect;
        private Label label1;
        private Panel pnAlert;
        private Button cmdDeleteAlert;
        private Button cmdRevertAlert;
        private Button cmdSaveAlert;
        private TextBox txtAlertDefinition;
        private TextBox txtAlertName;
        private Label label3;
        private Label label2;
        private Label label4;
        private ListBox lstNotification;
        private ComboBox cmbAlertTiming;
        private Label label5;
        private Button cmdAddAlertNotification;
        private Button cmdDeleteAlertNotification;
        private Button cmdEditAlertNotification;
    }
}