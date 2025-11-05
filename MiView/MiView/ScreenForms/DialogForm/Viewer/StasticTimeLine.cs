using MiView.Common.Connection.REST;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiView.ScreenForms.DialogForm.Viewer
{
    public partial class StasticTimeLine : Form
    {
        public static StasticTimeLine Instance { get; } = new StasticTimeLine();
        public StasticTimeLine()
        {
            InitializeComponent();
        }

        private void cmdSelectTimeLine_Click(object sender, EventArgs e)
        {
        }

        private void StasticTimeLine_VisibleChanged(object sender, EventArgs e)
        {
            this.cmbTimeLine.Items.Clear();

            if (this.Visible == false)
            {
                return;
            }
        }
    }
}
