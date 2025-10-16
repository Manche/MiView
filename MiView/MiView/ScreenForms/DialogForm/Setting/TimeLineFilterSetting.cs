using MiView.Common.Connection.WebSocket;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiView.ScreenForms.DialogForm.Setting
{
    public partial class TimeLineFilterSetting : Form
    {
        public WebSocketManager? _WSManager { get; set; }
        public Dictionary<string, string>? _TmpTLNames = new Dictionary<string, string>();
        public Dictionary<string, DataGridTimeLine>? _TLGrid = new Dictionary<string, DataGridTimeLine>();

        public TimeLineFilterSetting()
        {
            InitializeComponent();
        }

        private void TimeLineFilterSetting_Load(object sender, EventArgs e)
        {
            this.cmbTimeLineSelect.Items.Clear();
            if (_TLGrid == null)
            {
                return;
            }
            if (_WSManager != null &&
                _WSManager.TimeLineObject != null && _WSManager.TimeLineObject.Count() > 0)
            {
                this.cmbTimeLineSelect.Items.Add(_WSManager.TimeLineObject
                                                           .ToList()
                                                           .Select(r => { return new TimeLineCombo(r._TabName, r._Definition); }));
            }
        }
    }

    public class TimeLineCombo
    {
        public string TabName { get; set; }
        public string TabDefinition { get; set; }

        public TimeLineCombo(string tabName, string tabDefinition)
        {
            TabName = tabName;
            TabDefinition = tabDefinition;
        }

        public override string ToString()
        {
            return TabName;
        }
    }
}
