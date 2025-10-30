using MiView.Common.Connection.WebSocket;
using MiView.Common.TimeLine;
using MiView.ScreenForms.DialogForm.Event;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MiView.ScreenForms.DialogForm.Setting.APISetting;

namespace MiView.ScreenForms.DialogForm.Setting
{
    public partial class TimeLineSetting : Form
    {
        private Dictionary<string, WebSocketManager> _TLManager = new Dictionary<string, WebSocketManager>();
        private Dictionary<string, string> _TmpTLManager = new Dictionary<string, string>();
        private Dictionary<string, DataGridTimeLine> _TLGrid = new Dictionary<string, DataGridTimeLine>();
        private List<WebSocketManager> _WSManager = new List<WebSocketManager>();
        private List<string> _TPName = new List<string>();
        private DateTime _CurrentDateTime = DateTime.Now;
        private DateTime? _LastUpdate = null;
        private TimeLineCreator _TLCreator = new TimeLineCreator();

        private TimeLineFilterSetting _FilterForm = new TimeLineFilterSetting();
        private TimeLineAlertSetting _AlertForm = new TimeLineAlertSetting();
        private APIReflexSetting _ReflexForm = new APIReflexSetting();
        private AddTimeLine _AddTLForm = new AddTimeLine();

        public TimeLineSetting()
        {
            InitializeComponent();

            this.AddTimeLineExecuted += this.AddTimeLineExecute;
            this._AddTLForm.AddTimeLineExecuted += this.AddTLForm_AddTimeLineExecute;
            this.DeleteTimeLineExecuted += this.DeleteTimeLineExecute;
        }
        public void SetTLList(Dictionary<string, DataGridTimeLine> TLGrids,
                              Dictionary<string, string> TabSets)
        {
            this._TLGrid = TLGrids;
            this._TmpTLManager = TabSets;

            this.cmbTimeLineSelect.Items.Clear();
            this.cmbTimeLineSelect.Items.AddRange(this._TLGrid.Values.ToArray().Select(r => { return new TimeLineCombo(r._TabName, r._Definition); }).ToArray());
        }

        private void cmdOpenFilterSetting_Click(object sender, EventArgs e)
        {
            var SelectedObj = this.cmbTimeLineSelect.SelectedItem;
            if (SelectedObj == null)
            {
                return;
            }
            var SelectedCombo = (TimeLineCombo)SelectedObj;
            if (SelectedCombo == null)
            {
                return;
            }
            if (!this._TLGrid.ContainsKey(SelectedCombo.TabDefinition))
            {
                // 本来はない
                MessageBox.Show("エラー！");
                return;
            }
            this._FilterForm.SetTimeLineData(this._TLGrid[SelectedCombo.TabDefinition]);
            this._FilterForm.ShowDialog();
        }

        private void cmdOpenAlertSetting_Click(object sender, EventArgs e)
        {
            var SelectedObj = this.cmbTimeLineSelect.SelectedItem;
            if (SelectedObj == null)
            {
                return;
            }
            var SelectedCombo = (TimeLineCombo)SelectedObj;
            if (SelectedCombo == null)
            {
                return;
            }
            if (!this._TLGrid.ContainsKey(SelectedCombo.TabDefinition))
            {
                // 本来はない
                MessageBox.Show("エラー！");
                return;
            }
            this._AlertForm.SetAlertData(this._TLGrid[SelectedCombo.TabDefinition]);
            this._AlertForm.ShowDialog();
        }

        private void cmdOpenReflexTLSetting_Click(object sender, EventArgs e)
        {
            var SelectedObj = this.cmbTimeLineSelect.SelectedItem;
            if (SelectedObj == null)
            {
                return;
            }
            var SelectedCombo = (TimeLineCombo)SelectedObj;
            if (SelectedCombo == null)
            {
                return;
            }
            if (!this._TLGrid.ContainsKey(SelectedCombo.TabDefinition))
            {
                // 本来はない
                MessageBox.Show("エラー！");
                return;
            }
        }

        #region イベント
        /// <summary>
        /// 設定変更イベント
        /// </summary>
        public event EventHandler<SettingChangeEventArgs> SettingChanged;
        public event EventHandler<AddTimeLineEventArgs> AddTimeLineExecuted;
        private void AddTimeLineExecute(object? sender, AddTimeLineEventArgs e)
        {
        }
        private void AddTLForm_AddTimeLineExecute(object? sender, AddTimeLineEventArgs e)
        {
            this.cmbTimeLineSelect.Items.Add(new TimeLineCombo(e.TabName, e.TabDefinition));
            this.AddTimeLineExecuted(sender, e);
        }

        public event EventHandler<DeleteTimeLineEventArgs> DeleteTimeLineExecuted;
        private void DeleteTimeLineExecute(object? sender, DeleteTimeLineEventArgs e)
        {
        }
        #endregion

        private void cmdAddTimeLine_Click(object sender, EventArgs e)
        {
            this._AddTLForm.ShowDialog();
        }

        private void cmdDeleteTimeLine_Click(object sender, EventArgs e)
        {
            var SelectedObj = this.cmbTimeLineSelect.SelectedItem;
            if (SelectedObj == null)
            {
                return;
            }
            var SelectedCombo = (TimeLineCombo)SelectedObj;
            if (SelectedCombo == null)
            {
                return;
            }
            if (!this._TLGrid.ContainsKey(SelectedCombo.TabDefinition))
            {
                // 本来はない
                MessageBox.Show("エラー！");
                return;
            }
            DeleteTimeLineEventArgs ce = new DeleteTimeLineEventArgs(SelectedCombo.TabDefinition, SelectedCombo.TabName);
            this.DeleteTimeLineExecuted(sender, ce);
        }
    }
}
