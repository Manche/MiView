using MiView.Common.Connection.WebSocket;
using MiView.Common.TimeLine;
using MiView.ScreenForms.DialogForm.Event;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiView.ScreenForms.DialogForm.Setting
{
    public partial class APISetting : Form
    {
        private Dictionary<string, WebSocketManager> _TLManager = new Dictionary<string, WebSocketManager>();
        private Dictionary<string, string> _TmpTLManager = new Dictionary<string, string>();
        private Dictionary<string, DataGridTimeLine> _TLGrid = new Dictionary<string, DataGridTimeLine>();
        private List<WebSocketManager> _WSManager = new List<WebSocketManager>();
        private List<string> _TPName = new List<string>();
        private DateTime _CurrentDateTime = DateTime.Now;
        private DateTime? _LastUpdate = null;
        private TimeLineCreator _TLCreator = new TimeLineCreator();

        public APISetting()
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

        public void SetTLManagers(Dictionary<string, string> TmpTLManager,
                                  List<WebSocketManager> WSManager)
        {
            //this._TLManager = TLManager;
            //this._TmpTLManager = TmpTLManager;
            this._WSManager = WSManager;

            this.listBox1.Items.Clear();
            this.listBox1.Items.AddRange(WSManager.Select(r => { return new WebSocketCombo(r._HostDefinition, r.TLKind, r, this._WSManager.IndexOf(r)); }).ToArray());
        }
        public void SetTLGrids(Dictionary<string, DataGridTimeLine> TLGrid)
        {
            this._TLGrid = TLGrid;
        }

        private int _CurrentWSManagerIndex = -1;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var CurrentWSObj = this.listBox1.SelectedItem;
            if (CurrentWSObj == null)
            {
                return;
            }
            WebSocketCombo CurrentWSCombo;
            WebSocketManager CurrentWSManager;
            try
            {
                CurrentWSCombo = (WebSocketCombo)CurrentWSObj;
                CurrentWSManager = (WebSocketManager)CurrentWSCombo.WSManager;
            }
            catch (Exception ex)
            {
                return;
            }
            _CurrentWSManagerIndex = CurrentWSCombo.ListIndex;
            APIStatusDispData APIDisp = new APIStatusDispData()
            {
                _HostUrl = CurrentWSManager._Host,
                _Host = CurrentWSManager._HostUrl,
                _ConnectStatus = CurrentWSManager.GetSocketState() == System.Net.WebSockets.WebSocketState.Open &&
                                 CurrentWSManager._IsOpenTimeLine,
                _LastReceived = CurrentWSManager._LastDataReceived,
                _ConnectionClosed = CurrentWSManager._ConnectionClosed
            };
            SetStatus(new List<APIStatusDispData>() { APIDisp });
            LoadSettingWSManager(CurrentWSManager);
        }

        public void SetStatus(List<APIStatusDispData> APIDisps)
        {
            if (InvokeRequired)
            {
                this.Invoke(SetStatus, APIDisps);
            }
            if (APIDisps.Count == 0)
            {
                return;
            }
            var APIDisp = APIDisps[0];
            this._LastUpdate = APIDisp._LastReceived;
            this.lbltxtLastReceivedDatetime.Text = APIDisp._LastReceived.ToString();
            this.lbltxtCurrentReceiveState.BackColor = APIDisp._ConnectStatus ? Color.LightGreen : Color.IndianRed;
            this.lbltxtCurrentReceiveState.Text = APIDisp._ConnectStatus ? "受信中" : "未接続/切断中";
        }

        private void LoadSettingWSManager(WebSocketManager? WSManager)
        {
            this.cmdSettingSave.Enabled = true;

            // 設定値反映
            if (WSManager != null) this.lblHostDefinition.Text = WSManager._HostDefinition;
            if (WSManager != null) this.lbltxtSoftwareName.Text = WSManager.SoftwareVersion.SoftwareName;
            if (WSManager != null) this.lbltxtSoftwareVersion.Text = $"{WSManager.SoftwareVersion.Version.RawVersion}";
            if (WSManager != null) this.lbltxtTimeLineKind.Text = WSManager.TLKind.ToString();

            this.chkSetIntg.Enabled = WSManager != null;
            if (WSManager != null)
            {
                this.chkSetIntg.Checked = WSManager.IncludedDataGridTimeLine(new List<Func<DataGridTimeLine, bool>>() { new Func<DataGridTimeLine, bool>(r => { return r._Definition == "Main"; }) }.ToArray());
            }

#if DEBUG
            if (WSManager != null) this.txtAPIKey.Text = WSManager.APIKey;
#else
            if (WSManager != null) this.txtAPIKey.Text = "";
            if (WSManager != null) this.txtAPIKey.Enabled = false;
#endif
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
            // 設定値を書き込む
            //if (this.chkIsUpdateTL.Enabled) this._TLGrid[_CurrentTabDefinition]._IsUpdateTL = this.chkIsUpdateTL.Checked;
            if (this.chkSetIntg.Enabled)
            {
                if (this.chkSetIntg.Checked)
                {
                    this._WSManager[_CurrentWSManagerIndex].SetDataGridTimeLine(this._TLGrid["Main"]);
                }
                else
                {
                    this._WSManager[_CurrentWSManagerIndex].DetachDataGridTimeLine(new List<Func<DataGridTimeLine, bool>>() { new Func<DataGridTimeLine, bool>(r => { return r._Definition == "Main"; }) }.ToArray());
                }
            }

            // 設定適用
            var EventArg = new SettingChangeEventArgs();
            EventArg._WSManager = this._WSManager[_CurrentWSManagerIndex];
            EventArg._WSDefinition = _CurrentWSManagerIndex;
            this.SettingChanged?.Invoke(this, EventArg);
        }
        #endregion

        private APIReflexSetting TimeLineReflexSetting { get; set; }
        /// <summary>
        /// 反映タイムライン設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdTimeLineReflex_Click(object sender, EventArgs e)
        {
            TimeLineReflexSetting = new APIReflexSetting();
            TimeLineReflexSetting._WSManager = this._WSManager[_CurrentWSManagerIndex];
            TimeLineReflexSetting._TmpTLNames = _TmpTLManager;
            TimeLineReflexSetting._TLGrid = _TLGrid;
            TimeLineReflexSetting.ShowDialog();

            // 設定適用
            var EventArg = new SettingChangeEventArgs();
            EventArg._WSManager = this._WSManager[_CurrentWSManagerIndex];
            EventArg._WSDefinition = _CurrentWSManagerIndex;
            this.SettingChanged?.Invoke(this, EventArg);
        }

        public class WebSocketCombo
        {
            public string Host { get; set; }
            public TimeLineBasic.ConnectTimeLineKind TLKind { get; set; }
            public WebSocketManager WSManager { get; set; }
            public int ListIndex { get; set; }
            public WebSocketCombo(string host, TimeLineBasic.ConnectTimeLineKind tlKind, WebSocketManager wsManager, int listIndex)
            {
                Host = host;
                TLKind = tlKind;
                WSManager = wsManager;
                ListIndex = listIndex;
            }

            public override string ToString()
            {
                return Host + "/" + TLKind.ToString();
            }
        }

        private void cmdReConnect_Click(object sender, EventArgs e)
        {
            try
            {
                this._WSManager[_CurrentWSManagerIndex].CreateAndReOpen();
            }
            catch
            {
            }
        }
    }
}
