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
            label1 = new Label();
            cmbTimeLineSelect = new ComboBox();
            cmdLoadFilter = new Button();
            cmdCreateFilter = new Button();
            cmdDeleteFilter = new Button();
            cmdCopyFilter = new Button();
            pnFilter = new Panel();
            chkConstraintInvert = new CheckBox();
            cmbMatchMode = new ComboBox();
            txtFilterDefinition = new TextBox();
            txtFilterName = new TextBox();
            label3 = new Label();
            label4 = new Label();
            label2 = new Label();
            cmdRevertFilter = new Button();
            cmdSaveFilter = new Button();
            pnFilter.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(65, 15);
            label1.TabIndex = 2;
            label1.Text = "フィルタ選択";
            // 
            // cmbTimeLineSelect
            // 
            cmbTimeLineSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTimeLineSelect.FormattingEnabled = true;
            cmbTimeLineSelect.Location = new Point(12, 27);
            cmbTimeLineSelect.Name = "cmbTimeLineSelect";
            cmbTimeLineSelect.Size = new Size(776, 23);
            cmbTimeLineSelect.TabIndex = 6;
            // 
            // cmdLoadFilter
            // 
            cmdLoadFilter.Location = new Point(12, 56);
            cmdLoadFilter.Name = "cmdLoadFilter";
            cmdLoadFilter.Size = new Size(75, 23);
            cmdLoadFilter.TabIndex = 7;
            cmdLoadFilter.Text = "読み込み";
            cmdLoadFilter.UseVisualStyleBackColor = true;
            cmdLoadFilter.Click += cmdLoadFilter_Click;
            // 
            // cmdCreateFilter
            // 
            cmdCreateFilter.Location = new Point(12, 85);
            cmdCreateFilter.Name = "cmdCreateFilter";
            cmdCreateFilter.Size = new Size(75, 23);
            cmdCreateFilter.TabIndex = 7;
            cmdCreateFilter.Text = "新規作成";
            cmdCreateFilter.UseVisualStyleBackColor = true;
            cmdCreateFilter.Click += cmdCreateFilter_Click;
            // 
            // cmdDeleteFilter
            // 
            cmdDeleteFilter.Location = new Point(174, 3);
            cmdDeleteFilter.Name = "cmdDeleteFilter";
            cmdDeleteFilter.Size = new Size(75, 23);
            cmdDeleteFilter.TabIndex = 7;
            cmdDeleteFilter.Text = "削除";
            cmdDeleteFilter.UseVisualStyleBackColor = true;
            cmdDeleteFilter.Click += cmdDeleteFilter_Click;
            // 
            // cmdCopyFilter
            // 
            cmdCopyFilter.Location = new Point(93, 56);
            cmdCopyFilter.Name = "cmdCopyFilter";
            cmdCopyFilter.Size = new Size(75, 23);
            cmdCopyFilter.TabIndex = 7;
            cmdCopyFilter.Text = "コピー";
            cmdCopyFilter.UseVisualStyleBackColor = true;
            cmdCopyFilter.Click += cmdCopyFilter_Click;
            // 
            // pnFilter
            // 
            pnFilter.Controls.Add(chkConstraintInvert);
            pnFilter.Controls.Add(cmbMatchMode);
            pnFilter.Controls.Add(cmdDeleteFilter);
            pnFilter.Controls.Add(txtFilterDefinition);
            pnFilter.Controls.Add(txtFilterName);
            pnFilter.Controls.Add(label3);
            pnFilter.Controls.Add(label4);
            pnFilter.Controls.Add(label2);
            pnFilter.Controls.Add(cmdRevertFilter);
            pnFilter.Controls.Add(cmdSaveFilter);
            pnFilter.Location = new Point(12, 114);
            pnFilter.Name = "pnFilter";
            pnFilter.Size = new Size(776, 430);
            pnFilter.TabIndex = 8;
            // 
            // chkConstraintInvert
            // 
            chkConstraintInvert.AutoSize = true;
            chkConstraintInvert.Location = new Point(81, 146);
            chkConstraintInvert.Name = "chkConstraintInvert";
            chkConstraintInvert.Size = new Size(102, 19);
            chkConstraintInvert.TabIndex = 11;
            chkConstraintInvert.Text = "条件を反転する";
            chkConstraintInvert.UseVisualStyleBackColor = true;
            // 
            // cmbMatchMode
            // 
            cmbMatchMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMatchMode.FormattingEnabled = true;
            cmbMatchMode.Location = new Point(81, 117);
            cmbMatchMode.Name = "cmbMatchMode";
            cmbMatchMode.Size = new Size(266, 23);
            cmbMatchMode.TabIndex = 10;
            // 
            // txtFilterDefinition
            // 
            txtFilterDefinition.Enabled = false;
            txtFilterDefinition.Location = new Point(81, 39);
            txtFilterDefinition.Name = "txtFilterDefinition";
            txtFilterDefinition.Size = new Size(266, 23);
            txtFilterDefinition.TabIndex = 9;
            // 
            // txtFilterName
            // 
            txtFilterName.Location = new Point(81, 68);
            txtFilterName.Name = "txtFilterName";
            txtFilterName.Size = new Size(266, 23);
            txtFilterName.TabIndex = 9;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 42);
            label3.Name = "label3";
            label3.Size = new Size(77, 15);
            label3.TabIndex = 8;
            label3.Text = "フィルタ識別子";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(3, 120);
            label4.Name = "label4";
            label4.Size = new Size(56, 15);
            label4.TabIndex = 8;
            label4.Text = "一致モード";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 71);
            label2.Name = "label2";
            label2.Size = new Size(53, 15);
            label2.TabIndex = 8;
            label2.Text = "フィルタ名";
            // 
            // cmdRevertFilter
            // 
            cmdRevertFilter.Location = new Point(81, 3);
            cmdRevertFilter.Name = "cmdRevertFilter";
            cmdRevertFilter.Size = new Size(87, 23);
            cmdRevertFilter.TabIndex = 7;
            cmdRevertFilter.Text = "編集取り消し";
            cmdRevertFilter.UseVisualStyleBackColor = true;
            cmdRevertFilter.Click += cmdRevertFilter_Click;
            // 
            // cmdSaveFilter
            // 
            cmdSaveFilter.Location = new Point(3, 3);
            cmdSaveFilter.Name = "cmdSaveFilter";
            cmdSaveFilter.Size = new Size(75, 23);
            cmdSaveFilter.TabIndex = 7;
            cmdSaveFilter.Text = "保存";
            cmdSaveFilter.UseVisualStyleBackColor = true;
            cmdSaveFilter.Click += cmdSaveFilter_Click;
            // 
            // TimeLineFilterSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 556);
            Controls.Add(pnFilter);
            Controls.Add(cmdCopyFilter);
            Controls.Add(cmdCreateFilter);
            Controls.Add(cmdLoadFilter);
            Controls.Add(cmbTimeLineSelect);
            Controls.Add(label1);
            Name = "TimeLineFilterSetting";
            Text = "フィルタリング設定";
            Load += TimeLineFilterSetting_Load;
            pnFilter.ResumeLayout(false);
            pnFilter.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private ComboBox cmbTimeLineSelect;
        private Button cmdLoadFilter;
        private Button cmdCreateFilter;
        private Button cmdDeleteFilter;
        private Button cmdCopyFilter;
        private Panel pnFilter;
        private Button cmdSaveFilter;
        private Button cmdRevertFilter;
        private TextBox txtFilterDefinition;
        private TextBox txtFilterName;
        private Label label3;
        private Label label2;
        private ComboBox cmbMatchMode;
        private Label label4;
        private CheckBox chkConstraintInvert;
    }
}