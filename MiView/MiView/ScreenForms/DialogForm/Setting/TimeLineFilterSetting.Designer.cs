namespace MiView.ScreenForms.DialogForm.Setting
{
    partial class TimeLineFilterSetting
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
            cmbTimeLineSelect = new ComboBox();
            label1 = new Label();
            cmdOpenFilterSetting = new Button();
            SuspendLayout();
            // 
            // cmbTimeLineSelect
            // 
            cmbTimeLineSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTimeLineSelect.FormattingEnabled = true;
            cmbTimeLineSelect.Location = new Point(12, 27);
            cmbTimeLineSelect.Name = "cmbTimeLineSelect";
            cmbTimeLineSelect.Size = new Size(776, 23);
            cmbTimeLineSelect.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(84, 15);
            label1.TabIndex = 2;
            label1.Text = "タイムライン選択";
            // 
            // cmdOpenFilterSetting
            // 
            cmdOpenFilterSetting.Location = new Point(12, 56);
            cmdOpenFilterSetting.Name = "cmdOpenFilterSetting";
            cmdOpenFilterSetting.Size = new Size(75, 23);
            cmdOpenFilterSetting.TabIndex = 4;
            cmdOpenFilterSetting.Text = "設定を開く";
            cmdOpenFilterSetting.UseVisualStyleBackColor = true;
            // 
            // TimeLineFilterSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(cmdOpenFilterSetting);
            Controls.Add(cmbTimeLineSelect);
            Controls.Add(label1);
            Name = "TimeLineFilterSetting";
            Text = "フィルタリング設定";
            Load += TimeLineFilterSetting_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbTimeLineSelect;
        private Label label1;
        private Button cmdOpenFilterSetting;
    }
}