using MiView.Common.Connection.REST;
using MiView.Common.TimeLine;
using MiView.ScreenForms.DialogForm.Setting;
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

        private Dictionary<string, DataGridTimeLine> _Grids = new Dictionary<string, DataGridTimeLine>();

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

            foreach (KeyValuePair<string, DataGridTimeLine> Grid in this._Grids)
            {
                this.cmbTimeLine.Items.Add(new TimeLineCombo(Grid.Value._TabName, Grid.Key));
            }
        }

        public void SetDataGrids(Dictionary<string, DataGridTimeLine> Grids)
        {
            this._Grids = Grids;
        }

        private void cmbTimeLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimeLineCombo? Cmb = (TimeLineCombo?)this.cmbTimeLine.SelectedItem;
            if (Cmb == null)
            {
                return;
            }

            DataGridTimeLine Grid = this._Grids[Cmb.TabDefinition];
        }
    }
}
