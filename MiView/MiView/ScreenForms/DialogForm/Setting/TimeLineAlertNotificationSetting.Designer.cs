namespace MiView.ScreenForms.DialogForm.Setting
{
    partial class TimeLineAlertNotificationSetting
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
            lblNotificationMethod = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // lblNotificationMethod
            // 
            lblNotificationMethod.AutoSize = true;
            lblNotificationMethod.Location = new Point(12, 9);
            lblNotificationMethod.Name = "lblNotificationMethod";
            lblNotificationMethod.Size = new Size(55, 15);
            lblNotificationMethod.TabIndex = 0;
            lblNotificationMethod.Text = "通知方法";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(125, 9);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 1;
            label2.Text = "label2";
            // 
            // TimeLineAlertNotificationSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(lblNotificationMethod);
            Name = "TimeLineAlertNotificationSetting";
            Text = "通知方法設定";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNotificationMethod;
        private Label label2;
    }
}