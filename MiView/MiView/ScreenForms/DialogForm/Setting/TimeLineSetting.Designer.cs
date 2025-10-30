namespace MiView.ScreenForms.DialogForm.Setting
{
    partial class TimeLineSetting
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
            cmbTimeLineSelect = new ComboBox();
            cmdOpenFilterSetting = new Button();
            cmdOpenReflexTLSetting = new Button();
            cmdAddTimeLine = new Button();
            cmdDeleteTimeLine = new Button();
            cmdOpenAlertSetting = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(130, 15);
            label1.TabIndex = 2;
            label1.Text = "現在展開中のタイムライン";
            // 
            // cmbTimeLineSelect
            // 
            cmbTimeLineSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTimeLineSelect.FormattingEnabled = true;
            cmbTimeLineSelect.Location = new Point(12, 33);
            cmbTimeLineSelect.Name = "cmbTimeLineSelect";
            cmbTimeLineSelect.Size = new Size(776, 23);
            cmbTimeLineSelect.TabIndex = 5;
            // 
            // cmdOpenFilterSetting
            // 
            cmdOpenFilterSetting.Location = new Point(12, 62);
            cmdOpenFilterSetting.Name = "cmdOpenFilterSetting";
            cmdOpenFilterSetting.Size = new Size(75, 23);
            cmdOpenFilterSetting.TabIndex = 6;
            cmdOpenFilterSetting.Text = "フィルタ設定";
            cmdOpenFilterSetting.UseVisualStyleBackColor = true;
            cmdOpenFilterSetting.Click += cmdOpenFilterSetting_Click;
            // 
            // cmdOpenReflexTLSetting
            // 
            cmdOpenReflexTLSetting.Location = new Point(286, 62);
            cmdOpenReflexTLSetting.Name = "cmdOpenReflexTLSetting";
            cmdOpenReflexTLSetting.Size = new Size(75, 23);
            cmdOpenReflexTLSetting.TabIndex = 6;
            cmdOpenReflexTLSetting.Text = "表示TL設定";
            cmdOpenReflexTLSetting.UseVisualStyleBackColor = true;
            cmdOpenReflexTLSetting.Click += cmdOpenReflexTLSetting_Click;
            // 
            // cmdAddTimeLine
            // 
            cmdAddTimeLine.Location = new Point(12, 102);
            cmdAddTimeLine.Name = "cmdAddTimeLine";
            cmdAddTimeLine.Size = new Size(94, 23);
            cmdAddTimeLine.TabIndex = 7;
            cmdAddTimeLine.Text = "タイムライン追加";
            cmdAddTimeLine.UseVisualStyleBackColor = true;
            cmdAddTimeLine.Click += cmdAddTimeLine_Click;
            // 
            // cmdDeleteTimeLine
            // 
            cmdDeleteTimeLine.Location = new Point(112, 102);
            cmdDeleteTimeLine.Name = "cmdDeleteTimeLine";
            cmdDeleteTimeLine.Size = new Size(94, 23);
            cmdDeleteTimeLine.TabIndex = 7;
            cmdDeleteTimeLine.Text = "タイムライン削除";
            cmdDeleteTimeLine.UseVisualStyleBackColor = true;
            cmdDeleteTimeLine.Click += cmdDeleteTimeLine_Click;
            // 
            // cmdOpenAlertSetting
            // 
            cmdOpenAlertSetting.Location = new Point(93, 62);
            cmdOpenAlertSetting.Name = "cmdOpenAlertSetting";
            cmdOpenAlertSetting.Size = new Size(75, 23);
            cmdOpenAlertSetting.TabIndex = 6;
            cmdOpenAlertSetting.Text = "アラート設定";
            cmdOpenAlertSetting.UseVisualStyleBackColor = true;
            cmdOpenAlertSetting.Click += cmdOpenAlertSetting_Click;
            // 
            // TimeLineSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 137);
            Controls.Add(cmdDeleteTimeLine);
            Controls.Add(cmdAddTimeLine);
            Controls.Add(cmdOpenReflexTLSetting);
            Controls.Add(cmdOpenAlertSetting);
            Controls.Add(cmdOpenFilterSetting);
            Controls.Add(cmbTimeLineSelect);
            Controls.Add(label1);
            Name = "TimeLineSetting";
            Text = "タイムライン設定";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox cmbTimeLineSelect;
        private Button cmdOpenFilterSetting;
        private Button cmdOpenReflexTLSetting;
        private Button cmdAddTimeLine;
        private Button cmdDeleteTimeLine;
        private Button cmdOpenAlertSetting;
    }
}