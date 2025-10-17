namespace MiView.ScreenForms.DialogForm.Setting
{
    partial class APIReflexSetting
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
            label2 = new Label();
            lstTimeLine = new ListBox();
            cmdSaveTimeLine = new Button();
            cmdAddTimeLine = new Button();
            cmdRemoveTimeLine = new Button();
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
            // cmbTimeLineSelect
            // 
            cmbTimeLineSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTimeLineSelect.FormattingEnabled = true;
            cmbTimeLineSelect.Location = new Point(12, 27);
            cmbTimeLineSelect.Name = "cmbTimeLineSelect";
            cmbTimeLineSelect.Size = new Size(776, 23);
            cmbTimeLineSelect.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 90);
            label2.Name = "label2";
            label2.Size = new Size(130, 15);
            label2.TabIndex = 2;
            label2.Text = "現在反映中のタイムライン";
            // 
            // lstTimeLine
            // 
            lstTimeLine.FormattingEnabled = true;
            lstTimeLine.ItemHeight = 15;
            lstTimeLine.Location = new Point(12, 108);
            lstTimeLine.Name = "lstTimeLine";
            lstTimeLine.Size = new Size(776, 109);
            lstTimeLine.TabIndex = 3;
            // 
            // cmdSaveTimeLine
            // 
            cmdSaveTimeLine.Location = new Point(12, 224);
            cmdSaveTimeLine.Name = "cmdSaveTimeLine";
            cmdSaveTimeLine.Size = new Size(75, 23);
            cmdSaveTimeLine.TabIndex = 4;
            cmdSaveTimeLine.Text = "保存";
            cmdSaveTimeLine.UseVisualStyleBackColor = true;
            cmdSaveTimeLine.Click += cmdSaveTimeLine_Click;
            // 
            // cmdAddTimeLine
            // 
            cmdAddTimeLine.Location = new Point(12, 56);
            cmdAddTimeLine.Name = "cmdAddTimeLine";
            cmdAddTimeLine.Size = new Size(75, 23);
            cmdAddTimeLine.TabIndex = 5;
            cmdAddTimeLine.Text = "button2";
            cmdAddTimeLine.UseVisualStyleBackColor = true;
            cmdAddTimeLine.Click += cmdAddTimeLine_Click;
            // 
            // cmdRemoveTimeLine
            // 
            cmdRemoveTimeLine.Location = new Point(93, 56);
            cmdRemoveTimeLine.Name = "cmdRemoveTimeLine";
            cmdRemoveTimeLine.Size = new Size(75, 23);
            cmdRemoveTimeLine.TabIndex = 5;
            cmdRemoveTimeLine.Text = "button2";
            cmdRemoveTimeLine.UseVisualStyleBackColor = true;
            cmdRemoveTimeLine.Click += cmdRemoveTimeLine_Click;
            // 
            // TimeLineReflexSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 259);
            Controls.Add(cmdRemoveTimeLine);
            Controls.Add(cmdAddTimeLine);
            Controls.Add(cmdSaveTimeLine);
            Controls.Add(lstTimeLine);
            Controls.Add(label2);
            Controls.Add(cmbTimeLineSelect);
            Controls.Add(label1);
            Name = "TimeLineReflexSetting";
            Text = "反映先タイムライン設定";
            Load += TimeLineReflexSetting_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox cmbTimeLineSelect;
        private Label label2;
        private ListBox lstTimeLine;
        private Button cmdSaveTimeLine;
        private Button cmdAddTimeLine;
        private Button cmdRemoveTimeLine;
    }
}