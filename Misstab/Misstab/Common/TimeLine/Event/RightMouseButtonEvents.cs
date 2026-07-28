using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Misstab.Common.TimeLine.Event
{
    internal class RightMouseButtonEvents : ContextMenuStrip
    {
        public static RightMouseButtonEvents Instance = new RightMouseButtonEvents();
        private int _index = -1;
        public int RightMouseIndex
        {
            set
            {
                this._index = value;
            }
        }

        public RightMouseButtonEvents()
        {
            this.Items.Add("test");
            this.Items.Add("aa");
            this.ItemClicked += this.OnItemClicked;
            this.CreateMenu();
        }

        private void CreateMenu()
        {
            this.Items.Add(new RightMouseMenuProcess() { Text = "test" });
        }

        private void OnItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            //var o = (RightMouse_MenuItem_Interface)e;
            System.Diagnostics.Debug.WriteLine(sender);
            System.Diagnostics.Debug.WriteLine(e);
            if (e.ClickedItem == null)
            {
                return;
            }
            var SelectedItem = (RightMouseMenuProcess)e.ClickedItem;
        }
    }

    public class RightMouseMenuProcess : ToolStripMenuItem
    {
        public override string ToString()
        {
            return this.Text ?? string.Empty;
        }
        public Action? _f { get; set; } = null;

        public void Execute()
        {
            this._f?.Invoke();
        }
    }
}
