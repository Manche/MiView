using MiView.Common.TimeLine;

namespace MiView
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new MenuStrip();
            設定ToolStripMenuItem = new ToolStripMenuItem();
            tspAPISetting = new ToolStripMenuItem();
            tspTimeLineSetting = new ToolStripMenuItem();
            tbMain = new TabControl();
            tpMain = new TabPage();
            statusStrip1 = new StatusStrip();
            tsLabelMain = new ToolStripStatusLabel();
            tsLabelNoteCount = new ToolStripStatusLabel();
            textBox1 = new TextBox();
            cmbInstanceSelect = new ComboBox();
            cmdAddInstance = new Button();
            pnMain = new Panel();
            txtDetail = new TextBox();
            lblTLFrom = new Label();
            lblSoftware = new Label();
            lblUpdatedAt = new Label();
            lblUser = new Label();
            pnSub = new Panel();
            tabControl1 = new TabControl();
            tpNotification = new TabPage();
            tpDebug = new TabPage();
            menuStrip1.SuspendLayout();
            tbMain.SuspendLayout();
            statusStrip1.SuspendLayout();
            pnMain.SuspendLayout();
            pnSub.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { 設定ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(784, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // 設定ToolStripMenuItem
            // 
            設定ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tspAPISetting, tspTimeLineSetting });
            設定ToolStripMenuItem.Name = "設定ToolStripMenuItem";
            設定ToolStripMenuItem.Size = new Size(43, 20);
            設定ToolStripMenuItem.Text = "設定";
            // 
            // tspAPISetting
            // 
            tspAPISetting.Name = "tspAPISetting";
            tspAPISetting.Size = new Size(180, 22);
            tspAPISetting.Text = "API設定";
            tspAPISetting.Click += tspAPISetting_Click;
            // 
            // tspTimeLineSetting
            // 
            tspTimeLineSetting.Name = "tspTimeLineSetting";
            tspTimeLineSetting.Size = new Size(180, 22);
            tspTimeLineSetting.Text = "タイムライン設定";
            tspTimeLineSetting.Click += tspTimeLineSetting_Click;
            // 
            // tbMain
            // 
            tbMain.Alignment = TabAlignment.Bottom;
            tbMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tbMain.Controls.Add(tpMain);
            tbMain.Location = new Point(0, 27);
            tbMain.Multiline = true;
            tbMain.Name = "tbMain";
            tbMain.SelectedIndex = 0;
            tbMain.Size = new Size(784, 242);
            tbMain.TabIndex = 1;
            // 
            // tpMain
            // 
            tpMain.Location = new Point(4, 4);
            tpMain.Name = "tpMain";
            tpMain.Padding = new Padding(3);
            tpMain.Size = new Size(776, 214);
            tpMain.TabIndex = 0;
            tpMain.Text = "統合TL";
            tpMain.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsLabelMain, tsLabelNoteCount });
            statusStrip1.Location = new Point(0, 651);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(784, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // tsLabelMain
            // 
            tsLabelMain.Name = "tsLabelMain";
            tsLabelMain.Size = new Size(61, 17);
            tsLabelMain.Text = "タブ件数：";
            // 
            // tsLabelNoteCount
            // 
            tsLabelNoteCount.ImageAlign = ContentAlignment.TopLeft;
            tsLabelNoteCount.Name = "tsLabelNoteCount";
            tsLabelNoteCount.Size = new Size(60, 17);
            tsLabelNoteCount.Text = "9999/9999";
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(0, 560);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(784, 59);
            textBox1.TabIndex = 3;
            // 
            // cmbInstanceSelect
            // 
            cmbInstanceSelect.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmbInstanceSelect.FormattingEnabled = true;
            cmbInstanceSelect.Location = new Point(466, 625);
            cmbInstanceSelect.Name = "cmbInstanceSelect";
            cmbInstanceSelect.Size = new Size(314, 23);
            cmbInstanceSelect.TabIndex = 4;
            // 
            // cmdAddInstance
            // 
            cmdAddInstance.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmdAddInstance.Location = new Point(4, 624);
            cmdAddInstance.Name = "cmdAddInstance";
            cmdAddInstance.Size = new Size(106, 23);
            cmdAddInstance.TabIndex = 7;
            cmdAddInstance.Text = "インスタンス追加";
            cmdAddInstance.UseVisualStyleBackColor = true;
            cmdAddInstance.Click += cmdAddInstance_Click;
            // 
            // pnMain
            // 
            pnMain.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnMain.Controls.Add(txtDetail);
            pnMain.Controls.Add(lblTLFrom);
            pnMain.Controls.Add(lblSoftware);
            pnMain.Controls.Add(lblUpdatedAt);
            pnMain.Controls.Add(lblUser);
            pnMain.Location = new Point(0, 275);
            pnMain.Name = "pnMain";
            pnMain.Size = new Size(784, 171);
            pnMain.TabIndex = 8;
            // 
            // txtDetail
            // 
            txtDetail.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetail.Location = new Point(113, 28);
            txtDetail.Multiline = true;
            txtDetail.Name = "txtDetail";
            txtDetail.ReadOnly = true;
            txtDetail.ScrollBars = ScrollBars.Vertical;
            txtDetail.Size = new Size(667, 95);
            txtDetail.TabIndex = 1;
            // 
            // lblTLFrom
            // 
            lblTLFrom.AutoSize = true;
            lblTLFrom.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblTLFrom.Location = new Point(113, 126);
            lblTLFrom.Name = "lblTLFrom";
            lblTLFrom.Size = new Size(129, 17);
            lblTLFrom.TabIndex = 0;
            lblTLFrom.Text = "misskey.io/misskey.io";
            // 
            // lblSoftware
            // 
            lblSoftware.AutoSize = true;
            lblSoftware.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblSoftware.Location = new Point(646, 126);
            lblSoftware.Name = "lblSoftware";
            lblSoftware.Size = new Size(129, 17);
            lblSoftware.TabIndex = 0;
            lblSoftware.Text = "misskey.io/misskey.io";
            // 
            // lblUpdatedAt
            // 
            lblUpdatedAt.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUpdatedAt.AutoSize = true;
            lblUpdatedAt.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblUpdatedAt.Location = new Point(646, 8);
            lblUpdatedAt.Name = "lblUpdatedAt";
            lblUpdatedAt.Size = new Size(126, 17);
            lblUpdatedAt.TabIndex = 0;
            lblUpdatedAt.Text = "1900/11/11 00:00:00";
            lblUpdatedAt.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Font = new Font("Yu Gothic UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 128);
            lblUser.Location = new Point(113, 6);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(48, 20);
            lblUser.TabIndex = 0;
            lblUser.Text = "label1";
            // 
            // pnSub
            // 
            pnSub.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnSub.Controls.Add(tabControl1);
            pnSub.Location = new Point(0, 446);
            pnSub.Name = "pnSub";
            pnSub.Size = new Size(784, 108);
            pnSub.TabIndex = 9;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tpNotification);
            tabControl1.Controls.Add(tpDebug);
            tabControl1.Location = new Point(1, 6);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(780, 102);
            tabControl1.TabIndex = 0;
            // 
            // tpNotification
            // 
            tpNotification.Location = new Point(4, 24);
            tpNotification.Name = "tpNotification";
            tpNotification.Padding = new Padding(3);
            tpNotification.Size = new Size(772, 74);
            tpNotification.TabIndex = 0;
            tpNotification.Text = "通知";
            tpNotification.UseVisualStyleBackColor = true;
            // 
            // tpDebug
            // 
            tpDebug.Location = new Point(4, 24);
            tpDebug.Name = "tpDebug";
            tpDebug.Padding = new Padding(3);
            tpDebug.Size = new Size(772, 74);
            tpDebug.TabIndex = 1;
            tpDebug.Text = "デバッグ";
            tpDebug.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 673);
            Controls.Add(pnSub);
            Controls.Add(pnMain);
            Controls.Add(cmdAddInstance);
            Controls.Add(cmbInstanceSelect);
            Controls.Add(textBox1);
            Controls.Add(statusStrip1);
            Controls.Add(tbMain);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "MiView - MainForm";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            tbMain.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            pnMain.ResumeLayout(false);
            pnMain.PerformLayout();
            pnSub.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private TabControl tbMain;
        private TabPage tpMain;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel tsLabelMain;
        private ToolStripStatusLabel tsLabelNoteCount;
        private TextBox textBox1;
        private ComboBox cmbInstanceSelect;
        private Button cmdAddInstance;
        private Panel pnMain;
        private Label lblUser;
        private TextBox txtDetail;
        private Label lblUpdatedAt;
        private Label lblSoftware;
        private Panel pnSub;
        private TabControl tabControl1;
        private TabPage tpNotification;
        private TabPage tpDebug;
        private Label lblTLFrom;
        private ToolStripMenuItem 設定ToolStripMenuItem;
        private ToolStripMenuItem tspAPISetting;
        private ToolStripMenuItem tspTimeLineSetting;
    }
}
