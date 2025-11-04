using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MiView.ScreenForms.Controls.Notify
{
    /// <summary>
    /// 通知共通コンポーネント
    /// </summary>
    public class NotifyView
    {
        public NotifyIcon _Notify {  get; set; }

        public NotifyView()
        {
            this._Notify = new NotifyIcon();
            // this._Notify.Icon = new Icon(@"C:\Users\manch\Downloads\favicon.ico");
            this._Notify.Visible = true;

            this._Notify.ContextMenuStrip = new NotifyContextStrip();

        }
    }

    public class NotifyContextStrip : ContextMenuStrip
    {
        /// <summary>
        /// コンテキストメニュー
        /// </summary>
        private List<ToolStripMenuItem> ContextItems = new List<ToolStripMenuItem>();

        public NotifyContextStrip()
        {
            this.Items.Clear();

            ToolStripMenuItem TItem;

            // 閉じるボタン
            TItem = new ToolStripMenuItem();
            TItem.Click += Click_Close;
            TItem.Text = "閉じる";

            ContextItems.Add(TItem);

            this.Items.AddRange(ContextItems.ToArray());
        }

        /// <summary>
        /// 閉じるボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Close(object? sender, EventArgs e)
        {
            // 終了
            Application.Exit();
        }
    }
}
