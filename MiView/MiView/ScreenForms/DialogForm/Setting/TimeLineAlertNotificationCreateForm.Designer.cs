namespace MiView.ScreenForms.DialogForm.Setting
{
    partial class TimeLineAlertNotificationCreateForm
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
            cmbNotificationKind = new ComboBox();
            cmdCreateNotificationKind = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(108, 15);
            label1.TabIndex = 0;
            label1.Text = "作成する通知の種類";
            // 
            // cmbNotificationKind
            // 
            cmbNotificationKind.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNotificationKind.FormattingEnabled = true;
            cmbNotificationKind.Location = new Point(126, 12);
            cmbNotificationKind.Name = "cmbNotificationKind";
            cmbNotificationKind.Size = new Size(380, 23);
            cmbNotificationKind.TabIndex = 1;
            // 
            // cmdCreateNotificationKind
            // 
            cmdCreateNotificationKind.Location = new Point(431, 41);
            cmdCreateNotificationKind.Name = "cmdCreateNotificationKind";
            cmdCreateNotificationKind.Size = new Size(75, 23);
            cmdCreateNotificationKind.TabIndex = 2;
            cmdCreateNotificationKind.Text = "作成";
            cmdCreateNotificationKind.UseVisualStyleBackColor = true;
            cmdCreateNotificationKind.Click += cmdCreateNotificationKind_Click;
            // 
            // TimeLineAlertNotificationCreateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 80);
            Controls.Add(cmdCreateNotificationKind);
            Controls.Add(cmbNotificationKind);
            Controls.Add(label1);
            Name = "TimeLineAlertNotificationCreateForm";
            Text = "作成する通知の種類";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox cmbNotificationKind;
        private Button cmdCreateNotificationKind;
    }
}