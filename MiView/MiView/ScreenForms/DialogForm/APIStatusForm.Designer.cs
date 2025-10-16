namespace MiView.ScreenForms.DialogForm
{
    partial class APIStatusForm
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
            dataGridView1 = new DataGridView();
            TLKind = new DataGridViewTextBoxColumn();
            HostUrl = new DataGridViewTextBoxColumn();
            ConnectStatus = new DataGridViewTextBoxColumn();
            LastReceived = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { TLKind, HostUrl, ConnectStatus, LastReceived });
            dataGridView1.Location = new Point(12, 12);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(486, 334);
            dataGridView1.TabIndex = 0;
            // 
            // TLKind
            // 
            TLKind.HeaderText = "種類";
            TLKind.Name = "TLKind";
            // 
            // HostUrl
            // 
            HostUrl.HeaderText = "接続先";
            HostUrl.Name = "HostUrl";
            HostUrl.Width = 240;
            // 
            // ConnectStatus
            // 
            ConnectStatus.HeaderText = "接続状態";
            ConnectStatus.Name = "ConnectStatus";
            // 
            // LastReceived
            // 
            LastReceived.HeaderText = "最終受信日時";
            LastReceived.Name = "LastReceived";
            // 
            // APIStatusForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(576, 360);
            Controls.Add(dataGridView1);
            Name = "APIStatusForm";
            Text = "APIStatusForm";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn TLKind;
        private DataGridViewTextBoxColumn HostUrl;
        private DataGridViewTextBoxColumn ConnectStatus;
        private DataGridViewTextBoxColumn LastReceived;
    }
}