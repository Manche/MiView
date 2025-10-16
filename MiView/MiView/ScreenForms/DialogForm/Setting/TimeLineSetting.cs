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

namespace MiView.ScreenForms.DialogForm.Setting
{
    public partial class TimeLineSetting : Form
    {
        private Dictionary<string, WebSocketManager> _TLManager = new Dictionary<string, WebSocketManager>();
        private Dictionary<string, string> _TmpTLManager = new Dictionary<string, string>();
        private Dictionary<string, DataGridTimeLine> _TLGrid = new Dictionary<string, DataGridTimeLine>();
        private List<string> _TPName = new List<string>();
        private DateTime _CurrentDateTime = DateTime.Now;
        private DateTime? _LastUpdate = null;
        private TimeLineCreator _TLCreator = new TimeLineCreator();

        public TimeLineSetting()
        {
            InitializeComponent();
            var tk = new Task(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    DifUpdate();
                }
            });
            tk.Start();
        }
        private void DifUpdate()
        {
            if (!this.Visible)
            {
                return;
            }
            if (InvokeRequired)
            {
                try
                {
                    // disposeされてることがある
                    this.Invoke(DifUpdate);
                }
                catch
                {
                }
                return;
            }
            if (_LastUpdate == null)
            {
                return;
            }
            _CurrentDateTime = DateTime.Now;
            TimeSpan Dif = (TimeSpan)(_CurrentDateTime - _LastUpdate);
            var DifTxt = "";
            if (int.Parse(Dif.ToString("%d")) > 0)
            {
                DifTxt = $"{Dif.ToString("%d")}日前";
            }
            else if (int.Parse(Dif.ToString("%h")) > 0)
            {
                DifTxt = $"{Dif.ToString("%h")}時間前";
            }
            else if (int.Parse(Dif.ToString("%m")) > 0)
            {
                DifTxt = $"{Dif.ToString("%m")}分前";
            }
            else if (int.Parse(Dif.ToString("%s")) > 0)
            {
                DifTxt = $"{Dif.ToString("%s")}秒前";
            }
            else
            {
                DifTxt = "受信直後";
            }
            this.lbltxtLastReceivedDiff.Text = DifTxt;
        }

        public void SetTLManagers(Dictionary<string, WebSocketManager> TLManager, Dictionary<string, string> TmpTLManager)
        {
            this._TLManager = TLManager;
            this._TmpTLManager = TmpTLManager;
        }

        public void SetTPNames(List<string> TabPageNames)
        {
            this.listBox1.Items.Clear();

            this._TPName = TabPageNames;
            foreach (string t in TabPageNames)
            {
                this.listBox1.Items.Add($"{t}");
            }
        }
        public void SetTLGrids(Dictionary<string, DataGridTimeLine> TLGrid)
        {
            this._TLGrid = TLGrid;
        }

        private string? _CurrentTabDefinition = null;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            object? SelectedItem = ((ListBox)sender).SelectedItem;
            if (SelectedItem == null)
            {
                return;
            }
            string SelectedDefinition = SelectedItem.ToString();
            if (SelectedDefinition == null)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine(SelectedDefinition);
            if (this._TmpTLManager.ContainsKey(SelectedDefinition))
            {
                _CurrentTabDefinition = this._TmpTLManager[SelectedItem.ToString()];

                WebSocketManager? WStimeLine = null;
                if (this._TLManager.ContainsKey(this._TmpTLManager[SelectedItem.ToString()]))
                {
                    SetTLManagerData(this._TLManager[this._TmpTLManager[SelectedItem.ToString()]],
                                     (this._TLGrid.ContainsKey(SelectedItem.ToString()) ? this._TLGrid[SelectedItem.ToString()] : null));
                    WStimeLine = this._TLManager[_CurrentTabDefinition];

                    APIStatusDispData APIDisp = new APIStatusDispData()
                    {
                        _TabDefinition = this._TmpTLManager[SelectedItem.ToString()],
                        _HostUrl = _TLManager[this._TmpTLManager[SelectedItem.ToString()]]._Host,
                        _Host = _TLManager[this._TmpTLManager[SelectedItem.ToString()]]._HostUrl,
                        _ConnectStatus = _TLManager[this._TmpTLManager[SelectedItem.ToString()]].GetSocketState() == System.Net.WebSockets.WebSocketState.Open && _TLManager[this._TmpTLManager[SelectedItem.ToString()]]._IsOpenTimeLine,
                        _LastReceived = _TLManager[this._TmpTLManager[SelectedItem.ToString()]]._LastDataReceived,
                        _ConnectionClosed = _TLManager[this._TmpTLManager[SelectedItem.ToString()]]._ConnectionClosed
                    };
                    SetStatus(new List<APIStatusDispData>() { APIDisp });

                }
                DataGridTimeLine? Grid = null;
                if (this._TLGrid.ContainsKey(_CurrentTabDefinition))
                {
                    Grid = this._TLGrid[_CurrentTabDefinition];
                }
                LoadSettingWSManager(WStimeLine, Grid);

                System.Diagnostics.Debug.WriteLine(this._TmpTLManager[SelectedItem.ToString()]);
            }
        }

        public void SetStatus(List<APIStatusDispData> APIDisp)
        {
            if (InvokeRequired)
            {
                this.Invoke(SetStatus, APIDisp);
            }
            if (_CurrentTabDefinition == null)
            {
                this.cmdSettingSave.Enabled = false;
                return;
            }
            APIStatusDispData[] Disp = APIDisp.FindAll(x => x._TabDefinition == _CurrentTabDefinition).ToArray();
            if (Disp.Length == 0)
            {
                _CurrentTabDefinition = null;
                this.cmdSettingSave.Enabled = false;
                return;
            }
            this._LastUpdate = Disp[0]._LastReceived;
            this.lbltxtLastReceivedDatetime.Text = Disp[0]._LastReceived.ToString();
            this.lbltxtCurrentReceiveState.BackColor = Disp[0]._ConnectStatus ? Color.LightGreen : Color.IndianRed;
            this.lbltxtCurrentReceiveState.Text = Disp[0]._ConnectStatus ? "受信中" : "未接続/切断中";
        }

        private void LoadSettingWSManager(WebSocketManager? WSManager, DataGridTimeLine? Grid = null)
        {
            this.cmdSettingSave.Enabled = true;

            // 設定値反映
            if (WSManager != null) this.lblHostDefinition.Text = WSManager._HostDefinition;
            if (WSManager != null) this.lbltxtSoftwareName.Text = WSManager.SoftwareVersion.SoftwareName;
            if (WSManager != null) this.lbltxtSoftwareVersion.Text = $"{WSManager.SoftwareVersion.Version.RawVersion}";
            if (WSManager != null) this.lbltxtTimeLineKind.Text = WSManager.TLKind.ToString();

            DataGridTimeLine? MainGrid = null;
            if (this._TLGrid.ContainsKey("Main"))
            {
                MainGrid = this._TLGrid["Main"];
            }

            this.chkSetIntg.Enabled = WSManager != null;
            if (WSManager != null)
            {
                this.chkSetIntg.Checked = WSManager.IncludedDataGridTimeLine(new List<Func<DataGridTimeLine, bool>>() { new Func<DataGridTimeLine, bool>(r => { return r._Definition == "Main"; }) }.ToArray());
            }

            this.chkIsUpdateTL.Enabled = Grid != null;
            if (Grid != null) this.chkIsUpdateTL.Checked = Grid._IsUpdateTL;

            // まだpending
            //this.chkIsVisibleTL.Enabled = Grid != null;
            //if (Grid != null && _CurrentTabDefinition != null)
            //{
            //    this.chkIsVisibleTL.Checked = Grid.Visible;
            //}
            this.chkIsVisibleTL.Enabled = false;
            this.chkIsVisibleTL.Checked = true;

#if DEBUG
            if (WSManager != null) this.txtAPIKey.Text = WSManager.APIKey;
#else
            if (WSManager != null) this.txtAPIKey.Text = "";
            if (WSManager != null) this.txtAPIKey.Enabled = false;
#endif
            this.cmdOpenFilteringSetting.Enabled = WSManager != null;

            this.cmdTimeLineReflex.Enabled = WSManager != null && _CurrentTabDefinition != null;
        }

        private void SetTLManagerData(WebSocketManager WSManager, DataGridTimeLine? Grid = null)
        {
        }

        #region イベント
        /// <summary>
        /// 設定変更イベント
        /// </summary>
        public event EventHandler<SettingChangeEventArgs>? SettingChanged;

        /// <summary>
        /// 設定変更ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSettingSave_Click(object sender, EventArgs e)
        {
            // 現在のタブ識別値がなければ処理しない
            if (_CurrentTabDefinition == null)
            {
                return;
            }
            // 設定値を書き込む
            if (this.chkIsUpdateTL.Enabled) this._TLGrid[_CurrentTabDefinition]._IsUpdateTL = this.chkIsUpdateTL.Checked;
            if (this.chkSetIntg.Enabled)
            {
                if (this.chkSetIntg.Checked)
                {
                    this._TLManager[_CurrentTabDefinition].SetDataGridTimeLine(this._TLGrid["Main"]);
                }
                else
                {
                    this._TLManager[_CurrentTabDefinition].DetachDataGridTimeLine(new List<Func<DataGridTimeLine, bool>>() { new Func<DataGridTimeLine, bool>(r => { return r._Definition == "Main"; }) }.ToArray());
                }
            }

            // 設定適用
            var EventArg = new SettingChangeEventArgs();
            EventArg._WSManager = this._TLManager[_CurrentTabDefinition];
            EventArg._WSDefinition = this._CurrentTabDefinition;
            EventArg._GridTimeLine = this._TLGrid[_CurrentTabDefinition];
            this.SettingChanged?.Invoke(this, EventArg);
        }
        #endregion

        private TimeLineFilterSetting TimeLineFilterSetting { get; set; }
        /// <summary>
        /// タイムラインフィルタ設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdOpenFilteringSetting_Click(object sender, EventArgs e)
        {
            if (_CurrentTabDefinition == null)
            {
                return;
            }
            var WStimeLine = this._TLManager[_CurrentTabDefinition];
            if (WStimeLine == null)
            {
                return;
            }

            TimeLineFilterSetting = new TimeLineFilterSetting();
            TimeLineFilterSetting._WSManager = WStimeLine;
            TimeLineFilterSetting._TmpTLNames = _TmpTLManager;
            TimeLineFilterSetting._TLGrid = _TLGrid;
            TimeLineFilterSetting.ShowDialog();

            // 設定適用
            var EventArg = new SettingChangeEventArgs();
            EventArg._WSManager = TimeLineFilterSetting._WSManager;
            EventArg._WSDefinition = this._CurrentTabDefinition;
            this.SettingChanged?.Invoke(this, EventArg);
        }

        private TimeLineReflexSetting TimeLineReflexSetting { get; set; }
        /// <summary>
        /// 反映タイムライン設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdTimeLineReflex_Click(object sender, EventArgs e)
        {
            if (_CurrentTabDefinition == null)
            {
                return;
            }
            var WStimeLine = this._TLManager[_CurrentTabDefinition];
            if (WStimeLine == null)
            {
                return;
            }
            TimeLineReflexSetting = new TimeLineReflexSetting();
            TimeLineReflexSetting._WSManager = WStimeLine;
            TimeLineReflexSetting._TmpTLNames = _TmpTLManager;
            TimeLineReflexSetting._TLGrid = _TLGrid;
            TimeLineReflexSetting.ShowDialog();

            // 設定適用
            var EventArg = new SettingChangeEventArgs();
            EventArg._WSManager = TimeLineReflexSetting._WSManager;
            EventArg._WSDefinition = this._CurrentTabDefinition;
            this.SettingChanged?.Invoke(this, EventArg);
        }
    }
}
