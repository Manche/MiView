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
    public partial class APIReflexSetting : Form
    {
        public WebSocketManager? _WSManager { get; set; }
        public Dictionary<string, string>? _TmpTLNames = new Dictionary<string, string>();
        public Dictionary<string, DataGridTimeLine>? _TLGrid = new Dictionary<string, DataGridTimeLine>();

        public APIReflexSetting()
        {
            InitializeComponent();
        }

        private void TimeLineReflexSetting_Load(object sender, EventArgs e)
        {
            if (_TLGrid == null)
            {
                return;
            }
            this.lstTimeLine.Items.Clear();
            if (_WSManager != null &&
                _WSManager.TimeLineObject != null && _WSManager.TimeLineObject.Count() > 0)
            {
                foreach (DataGridTimeLine t in _TLGrid.Values
                                                      .ToList()
                                                      .FindAll(r => { return r._Definition != "Main" && _WSManager.TimeLineObject.ToList().FindAll(k => { return k._Definition == r._Definition; }).Count == 0; }))
                {
                    this.cmbTimeLineSelect.Items.Add(new TimeLineCombo(t._TabName, t._Definition));
                }
                foreach (TimeLineCombo Cb in _TLGrid.Values
                                                  .ToList()
                                                  .FindAll(r => { return _WSManager.TimeLineObject.ToList().FindAll(k => { return k._Definition == r._Definition; }).Count > 0; })
                                                  .Select(r => { return new TimeLineCombo(r._TabName, r._Definition); })
                                                  .ToList())
                {
                    this.lstTimeLine.Items.Add(Cb);
                }
            }
            else
            {
                foreach (DataGridTimeLine t in _TLGrid.Values
                                                      .ToList()
                                                      .FindAll(r => { return r._Definition != "Main"; }))
                {
                    this.cmbTimeLineSelect.Items.Add(new TimeLineCombo(t._TabName, t._Definition));
                }
                //foreach (TimeLineCombo Cb in _TLGrid.Values
                //                                  .ToList()
                //                                  .Select(r => { return new TimeLineCombo(r._TabName, r._Definition); })
                //                                  .ToList())
                //{
                //    this.lstTimeLine.Items.Add(Cb);
                //}
            }
            // this.cmbTimeLineSelect.Enabled = this.cmbTimeLineSelect.Items.Count > 0;
        }

        private void cmdAddTimeLine_Click(object sender, EventArgs e)
        {
            var Tmp = cmbTimeLineSelect.SelectedItem as TimeLineCombo;
            if (Tmp == null)
            {
                return;
            }
            this.lstTimeLine.Items.Add(Tmp);
            this.cmbTimeLineSelect.Items.Remove(Tmp);
        }

        private void cmdRemoveTimeLine_Click(object sender, EventArgs e)
        {
            var Tmp = lstTimeLine.SelectedItem as TimeLineCombo;
            if(Tmp == null)
            {
                return;
            }
            this.cmbTimeLineSelect.Items.Add(Tmp);
            this.lstTimeLine.Items.Remove(Tmp);
        }

        private void cmdSaveTimeLine_Click(object sender, EventArgs e)
        {
            if (this._TLGrid == null ||
                this._WSManager == null)
            {
                return;
            }
            var SelectedTimeLineObjects = this.lstTimeLine.Items;
            var SelectedTimeLines = SelectedTimeLineObjects.Cast<TimeLineCombo>().ToList();

            var CurrentTimeLines = this._TLGrid.Values
                                               .ToList()
                                               .FindAll(r => { return SelectedTimeLines.FindAll(k => { return k.TabDefinition == r._Definition; }).Count > 0; })
                                               .ToArray();
            this._WSManager.SetTimeLineObject(CurrentTimeLines);
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
}
