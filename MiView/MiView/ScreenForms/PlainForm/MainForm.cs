using MiView.Common.AnalyzeData;
using MiView.Common.Connection.REST.Misskey;
using MiView.Common.Connection.REST.Misskey.v2025.API.Notes;
using MiView.Common.Connection.VersionInfo;
using MiView.Common.Connection.WebSocket;
using MiView.Common.Connection.WebSocket.Controller;
using MiView.Common.Connection.WebSocket.Event;
using MiView.Common.Connection.WebSocket.Misskey.v2025;
using MiView.Common.Fonts;
using MiView.Common.Fonts.Material;
using MiView.Common.Notification.Baloon;
using MiView.Common.Notification.Http;
using MiView.Common.Notification.Toast;
using MiView.Common.Setting;
using MiView.Common.TimeLine;
using MiView.ScreenForms.Controls.Combo;
using MiView.ScreenForms.Controls.Notify;
using MiView.ScreenForms.DialogForm;
using MiView.ScreenForms.DialogForm.Event;
using MiView.ScreenForms.DialogForm.Setting;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Nodes;
using System.Windows.Forms;

namespace MiView
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// タイムラインクリエータ
        /// </summary>
        private TimeLineCreator _TLCreator = new TimeLineCreator();

        /// <summary>
        /// WebSocketマネージャ
        /// </summary>
        private List<WebSocketManager> _WSManager = new List<WebSocketManager>();

        /// <summary>
        /// タイムラインマネージャ
        /// </summary>
        private Dictionary<string, WebSocketManager> _TLManager = new Dictionary<string, WebSocketManager>();

        /// <summary>
        /// 一時タイムラインマネージャ
        /// </summary>
        private Dictionary<string, string> _TmpTLManager = new Dictionary<string, string>();

        public NotifyView NotifyView { get; set; }

        private APIStatusForm _APIStatusForm = new APIStatusForm();
        private APISetting _TLSettingForm = new APISetting();

        /// <summary>
        /// このフォーム
        /// </summary>
        private MainForm MainFormObj;

        public MainForm()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.MainFormObj = this;

            // 今のところ通知を表示しないようにする
            this.pnSub.Visible = false;
            this.pnMain.Location = new Point(this.pnMain.Location.X, this.pnMain.Location.Y + this.pnSub.Size.Height);
            this.tabControl1.Size = new Size(this.tabControl1.Size.Width, this.tabControl1.Size.Height + this.pnSub.Size.Height);
            this.tbMain.Size = new Size(this.tbMain.Size.Width, this.tbMain.Size.Height + this.pnSub.Size.Height);

            // イベント設定
            DataAccepted += OnDataAccepted;
            DataRejected += OnDataRejected;

            this._TLSettingForm.SettingChanged += SettingFormSettingChanged;
        }

        private List<DataGridTimeLine> DGrids = new List<DataGridTimeLine>();
        private void TabUpdate()
        {
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _TLCreator.CreateTimeLine(ref this.MainFormObj, "Main", "tpMain");
            while (!this.Visible)
            {
            }

            var Ac = new Task(async () => { await ConnectWatcher(); });
            Ac.Start();
            this._APIStatusForm.Show();

            var sts = SettingController.LoadWebSockets();
            var smc = SettingController.LoadTimeLine();

            LoadTimeLineManually(smc);
            LoadWebSocketManually(sts);
        }

        private async Task ConnectWatcher()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(async () => { await ConnectWatcher(); }));
                return;
            }
            while (true)
            {
                List<APIStatusDispData> APIDisp = new List<APIStatusDispData>();
                foreach (var TLCon in _WSManager)
                {
                    try
                    {
                        //System.Diagnostics.Debug.WriteLine(_TLManager[TLCon.Value]._Host);
                        //System.Diagnostics.Debug.WriteLine(_TLManager[TLCon.Value].GetSocketState());
                        //System.Diagnostics.Debug.WriteLine(_TLManager[TLCon.Value]._ConnectionClosed);
                        //System.Diagnostics.Debug.WriteLine(_TLManager[TLCon.Value].WebSocket.State);
                        APIDisp.Add(new APIStatusDispData()
                        {
                            _TLKind = TLCon.TLKind.ToString(),
                            _HostUrl = TLCon._Host,
                            _Host = TLCon._HostUrl,
                            _ConnectStatus = TLCon.GetSocketState() == System.Net.WebSockets.WebSocketState.Open && TLCon._IsOpenTimeLine,
                            _LastReceived = TLCon._LastDataReceived,
                            _ConnectionClosed = TLCon._ConnectionClosed
                        });
                        if (TLCon.GetSocketState() != System.Net.WebSockets.WebSocketState.Open ||
                            TLCon._IsOpenTimeLine == false)
                        {
                            Task Tj = new Task(() =>
                            {
                                int Wait = 0;
                                while (Wait < 10)
                                {
                                    TLCon.CreateAndReOpen();
                                    int Wait2 = 0;
                                    while (Wait2 < 10)
                                    {
                                        if (TLCon.GetSocketState() == System.Net.WebSockets.WebSocketState.Open)
                                        {
                                            break;
                                        }
                                        Task.Delay(1000);
                                        Wait2++;
                                    }
                                    if (Wait2 == 10)
                                    {
                                        break;
                                    }
                                    try
                                    {
                                        TLCon.ReadTimeLineContinuous(TLCon);

                                        if (TLCon.APIKey != string.Empty)
                                        {
                                            var WTManager = WebSocketMainController.CreateWSTLManager(TLCon.SoftwareVersion.SoftwareType, TLCon.SoftwareVersion.Version);
                                            if (WTManager == null)
                                            {
                                                Thread.Sleep(1000);
                                                continue;
                                            }

                                            WTManager.OpenMain(TLCon._HostDefinition, TLCon.APIKey);
                                            WebSocketMain.ReadMainContinuous(WTManager);
                                            int Wt = 0;
                                            while (Wt < 10)
                                            {
                                                if (WTManager.GetSocketState() == System.Net.WebSockets.WebSocketState.Open)
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    TLCon.SetSocketState(System.Net.WebSockets.WebSocketState.Closed);
                                                }
                                                Wt++;
                                                Thread.Sleep(1000);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                                    }

                                    Wait++;
                                    System.Diagnostics.Debug.WriteLine(TLCon._HostDefinition);
                                    System.Diagnostics.Debug.WriteLine($"{Wait}秒待機中");

                                    if (TLCon.GetSocketState() == System.Net.WebSockets.WebSocketState.Open)
                                    {
                                        break;
                                    }

                                    Thread.Sleep(1000);
                                }
                            });
                            Tj.Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
                //foreach (var TLCon in _TmpTLManager)
                //{
                //    if (!_TLManager.ContainsKey(TLCon.Value))
                //    {
                //        continue;
                //    }
                //    try
                //    {
                //        //System.Diagnostics.Debug.WriteLine(_TLManager[TLCon.Value]._Host);
                //        //System.Diagnostics.Debug.WriteLine(_TLManager[TLCon.Value].GetSocketState());
                //        //System.Diagnostics.Debug.WriteLine(_TLManager[TLCon.Value]._ConnectionClosed);
                //        //System.Diagnostics.Debug.WriteLine(_TLManager[TLCon.Value].WebSocket.State);
                //        APIDisp.Add(new APIStatusDispData()
                //        {
                //            _TabDefinition = TLCon.Value,
                //            _HostUrl = _TLManager[TLCon.Value]._Host,
                //            _Host = _TLManager[TLCon.Value]._HostUrl,
                //            _ConnectStatus = _TLManager[TLCon.Value].GetSocketState() == System.Net.WebSockets.WebSocketState.Open && _TLManager[TLCon.Value]._IsOpenTimeLine,
                //            _LastReceived = _TLManager[TLCon.Value]._LastDataReceived,
                //            _ConnectionClosed = _TLManager[TLCon.Value]._ConnectionClosed
                //        });
                //        if (_TLManager[TLCon.Value].GetSocketState() != System.Net.WebSockets.WebSocketState.Open ||
                //            _TLManager[TLCon.Value]._IsOpenTimeLine == false)
                //        {
                //            Task Tj = new Task(() =>
                //            {
                //                int Wait = 0;
                //                while (Wait < 10)
                //                {
                //                    _TLManager[TLCon.Value].CreateAndReOpen();
                //                    int Wait2 = 0;
                //                    while (Wait2 < 10)
                //                    {
                //                        if (_TLManager[TLCon.Value].GetSocketState() == System.Net.WebSockets.WebSocketState.Open)
                //                        {
                //                            break;
                //                        }
                //                        Task.Delay(1000);
                //                        Wait2++;
                //                    }
                //                    if (Wait2 == 10)
                //                    {
                //                        break;
                //                    }
                //                    try
                //                    {
                //                        _TLManager[TLCon.Value].ReadTimeLineContinuous(_TLManager[TLCon.Value]);

                //                        if (_TLManager[TLCon.Value].APIKey != string.Empty)
                //                        {
                //                            var WTManager = WebSocketMainController.CreateWSTLManager(_TLManager[TLCon.Value].SoftwareVersion.SoftwareType, _TLManager[TLCon.Value].SoftwareVersion.Version);
                //                            if (WTManager == null)
                //                            {
                //                                Thread.Sleep(1000);
                //                                continue;
                //                            }

                //                            WTManager.OpenMain(_TLManager[TLCon.Value]._HostDefinition, _TLManager[TLCon.Value].APIKey);
                //                            WebSocketMain.ReadMainContinuous(WTManager);
                //                            int Wt = 0;
                //                            while (Wt < 10)
                //                            {
                //                                if (WTManager.GetSocketState() == System.Net.WebSockets.WebSocketState.Open)
                //                                {
                //                                    break;
                //                                }
                //                                else
                //                                {
                //                                    _TLManager[TLCon.Value].SetSocketState(System.Net.WebSockets.WebSocketState.Closed);
                //                                }
                //                                Wt++;
                //                                Thread.Sleep(1000);
                //                            }
                //                        }
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                //                    }

                //                    Wait++;
                //                    System.Diagnostics.Debug.WriteLine(_TLManager[TLCon.Value]._HostDefinition);
                //                    System.Diagnostics.Debug.WriteLine($"{Wait}秒待機中");

                //                    if (_TLManager[TLCon.Value].GetSocketState() == System.Net.WebSockets.WebSocketState.Open)
                //                    {
                //                        break;
                //                    }

                //                    Thread.Sleep(1000);
                //                }
                //            });
                //            Tj.Start();
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        System.Diagnostics.Debug.WriteLine(ex.ToString());
                //    }
                //}
                try
                {
                    this._APIStatusForm.SetStatus(APIDisp);
                }
                catch (Exception ex)
                {
                }
                try
                {
                    this._TLSettingForm.SetStatus(APIDisp);
                }
                catch (Exception ex)
                {
                }
                await Task.Delay(1000);
            }
        }

        // 呼び出し元で TabDef が _TmpTLManager に登録されるまで待つ
        private void WaitForTimeLineObject(string TabName, int timeoutMs = 5000)
        {
            int waited = 0;
            const int interval = 10; // 10ms ごとにチェック

            while (!_TmpTLManager.ContainsKey(TabName) && waited < timeoutMs)
            {
                Thread.Sleep(interval);
                waited += interval;
            }

            if (!_TmpTLManager.ContainsKey(TabName))
            {
                throw new TimeoutException($"TimeLine {TabName} が生成されませんでした");
            }
        }

        public void SelectTabPage(string TabName)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(SelectTabPage, TabName);
                return;
            }
            var Tb = this.tbMain.TabPages[TabName];
            if (Tb == null)
            {
                return;
            }
            else
            {
                Tb.Select();
            }
        }

        public void AddTimeLine(string InstanceURL,
                                string TabName,
                                string APIKey,
                                TimeLineBasic.ConnectTimeLineKind sTLKind,
                                CSoftwareVersionInfo? SoftwareVersionInfo,
                                bool IsFiltered = false,
                                bool AvoidIntg = false,
                                bool IsVisible = true)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(AddTimeLine, InstanceURL, TabName, APIKey, sTLKind, IsFiltered, AvoidIntg, IsVisible);
                return;
            }

            var TLKind = sTLKind;

            WebSocketManager? WSManager = WebSocketTimeLineController.CreateWSTLManager(SoftwareVersionInfo.SoftwareType, SoftwareVersionInfo.Version, TLKind);
            if (WSManager == null)
            {
                MessageBox.Show("非対応APIが使用されています。");
                return;
            }
            var WTManager = WebSocketMainController.CreateWSTLManager(WSManager.SoftwareVersion.SoftwareType, WSManager.SoftwareVersion.Version);
            if (WTManager == null)
            {
                MessageBox.Show("非対応APIが使用されています。");
                return;
            }

            // タブ識別
            var TabDef = System.Guid.NewGuid().ToString();

            // タブ追加
            _TLCreator.CreateTimeLineTab(ref this.MainFormObj, TabDef, TabName, IsVisible);
            _TLCreator.CreateTimeLine(ref this.MainFormObj, TabDef, TabDef, IsFiltered: IsFiltered);
            try
            {
                WSManager.OpenTimeLine(InstanceURL, APIKey);
                if (AvoidIntg == false)
                {
                    WSManager.SetDataGridTimeLine(_TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, "Main"));
                }
                WSManager.SetDataGridTimeLine(_TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, TabDef));
                _TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, TabDef).Visible = IsVisible;
                _TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, TabDef)._IsUpdateTL = IsVisible;
                try
                {
                    WSManager.ReadTimeLineContinuous(WSManager);

                    if (APIKey != string.Empty)
                    {
                        WTManager.OpenMain(WSManager._HostDefinition, WSManager.APIKey);
                        WebSocketMain.ReadMainContinuous(WTManager);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }

                // ここで手動で入れておく
                WSManager._IsOpenTimeLine = true;
                WSManager._LastDataReceived = DateTime.Now;

                _TLManager.Add(TabDef, WSManager);
                _TmpTLManager.Add(TabName, TabDef);

                //var c = MisskeyAPIController.CreateInstance(MisskeyAPIConst.API_ENDPOINT.NOTES_TIMELINE);
                //c.Request(WSManager._HostDefinition, WSManager.APIKey, null, null);
                //c.GetNotes();
                //var tm = c.GetNotes();
            }
            catch (Exception ce)
            {
                System.Diagnostics.Debug.WriteLine(ce.ToString());
            }
            if (WSManager.GetSocketState() != System.Net.WebSockets.WebSocketState.Open)
            {
                MessageBox.Show("インスタンスの読み込みに失敗しました。");
                return;
            }
            else
            {
                this._WSManager.Add(WSManager);
            }
        }

        public void LoadTimeLineManually(SettingTimeLine[] WSTimeLines)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(LoadTimeLineManually, WSTimeLines);
                return;
            }
            foreach (Common.Setting.SettingTimeLine WSTimeLine in WSTimeLines)
            {
                _TLCreator.CreateTimeLineTab(ref this.MainFormObj, WSTimeLine.Definition, WSTimeLine.TabName, WSTimeLine.IsVisible);
                _TLCreator.CreateTimeLine(ref this.MainFormObj, WSTimeLine.Definition, WSTimeLine.Definition, IsFiltered: WSTimeLine.IsFiltered);
                _TmpTLManager.Add(WSTimeLine.TabName, WSTimeLine.Definition);
            }
        }
        public void LoadWebSocketManually(SettingWebSocket[] WSManagers)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(LoadWebSocketManually, WSManagers);
                return;
            }
            foreach (Common.Setting.SettingWebSocket SWSManager in WSManagers)
            {
                WebSocketManager? WSManager = WebSocketTimeLineController.CreateWSTLManager(SWSManager.SoftwareVersionInfo.SoftwareType, SWSManager.SoftwareVersionInfo.Version, SWSManager.ConnectTimeLineKind);
                if (WSManager == null)
                {
                    MessageBox.Show("非対応APIが使用されています。");
                    return;
                }
                var WTManager = WebSocketMainController.CreateWSTLManager(WSManager.SoftwareVersion.SoftwareType, WSManager.SoftwareVersion.Version);
                if (WTManager == null)
                {
                    MessageBox.Show("非対応APIが使用されています。");
                    return;
                }
                try
                {
                    WSManager.OpenTimeLine(SWSManager.InstanceURL, SWSManager.APIKey);
                    if (SWSManager.AvoidIntg == false)
                    {
                        WSManager.SetDataGridTimeLine(_TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, "Main"));
                    }
                    foreach (string TTabDef in SWSManager.TimeLineDefinition ?? [])
                    {
                        WSManager.SetDataGridTimeLine(_TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, TTabDef));
                    }
                    try
                    {
                        WSManager.ReadTimeLineContinuous(WSManager);

                        if (SWSManager.APIKey != string.Empty)
                        {
                            WTManager.OpenMain(WSManager._HostDefinition, WSManager.APIKey);
                            WebSocketMain.ReadMainContinuous(WTManager);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                    // ここで手動で入れておく
                    WSManager._IsOpenTimeLine = true;
                    WSManager._LastDataReceived = DateTime.Now;

                    try
                    {
                        string TMTabDef = SWSManager.TimeLineDefinition[1] ?? SWSManager.TimeLineDefinition[0] ?? "";
                        if (TMTabDef != string.Empty)
                        {
                            _TLManager.Add(TMTabDef, WSManager);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    this._WSManager.Add(WSManager);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }
        }

        public void AddStaticTimeLine(string TabName, string? AttachDef = null, string? AttachName = null, bool IsFiltered = true)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(AddStaticTimeLine, TabName, AttachDef, AttachName, IsFiltered);
                return;
            }

            // タブ識別
            var TabDef = System.Guid.NewGuid().ToString();

            // タブ追加
            _TLCreator.CreateTimeLineTab(ref this.MainFormObj, TabDef, TabName);
            _TLCreator.CreateTimeLine(ref this.MainFormObj, TabDef, TabName, IsFiltered: IsFiltered);
            _TmpTLManager.Add(TabName, TabDef);

            if (AttachName == null)
            {
                return;
            }
            _TLManager[_TmpTLManager[AttachDef]].SetDataGridTimeLine(_TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, TabDef));
        }

        public void AppendStaticTimeLine(string TabName, string AttachDef, string? AttachName = null, bool IsFiltered = true)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(AddStaticTimeLine, TabName, AttachDef, IsFiltered);
                return;
            }

            _TLManager[_TmpTLManager[AttachDef]].SetDataGridTimeLine(_TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, _TmpTLManager[TabName]));
        }

        private void AppendTimelineFilter(string TabName, string AttachDef, TimeLineFilterlingOption FilterOption)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AppendTimelineFilter(TabName, AttachDef, FilterOption)));
                return;
            }

            _TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, _TmpTLManager[TabName])._FilteringOptions.Add(FilterOption);
        }

        private void AppendTimelineMatchMode(string TabName, string AttachDef, bool FilterMode)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AppendTimelineMatchMode(TabName, AttachDef, FilterMode)));
                return;
            }

            _TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, _TmpTLManager[TabName])._FilterMode = FilterMode;
        }


        private void AppendTimelineAlert(string TabName, string AttachDef, TimeLineAlertOption FilterOption)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AppendTimelineAlert(TabName, AttachDef, FilterOption)));
                return;
            }

            _TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, _TmpTLManager[TabName])._AlertOptions.Add(FilterOption);
        }

        private void cmdAddInstance_Click(object sender, EventArgs e)
        {
            AddInstanceWithAPIKey AddFrm = new AddInstanceWithAPIKey(this);
            AddFrm.ShowDialog();
        }

        //private void tbMain_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //((TabControl)sender).SuspendLayout();

        //    var TPages = ((TabControl)sender).TabPages;
        //    foreach (TabPage TPage in TPages)
        //    {
        //        foreach (DataGridTimeLine DGView in TPage.Controls.Cast<Control>().ToList().FindAll(r => { return r.GetType() == typeof(DataGridTimeLine); }))
        //        {
        //            DGView.Visible = true;
        //            // DGView.Visible = TPages.IndexOf(TPage) == ((TabControl)sender).SelectedIndex;

        //            //if (DGView.Visible)
        //            //{
        //            //    DGView.Refresh();
        //            //}
        //        }
        //    }
        //    //((TabControl)sender).ResumeLayout(false);
        //}

        public void SetTimeLineContents(string OriginalHost, JsonNode Node)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(SetTimeLineContents, OriginalHost, Node);
            }

            // 変換
            TimeLineContainer TL = ChannelToTimeLineContainer.ConvertTimeLineContainer(OriginalHost, Node);

            this.pnMain.SuspendLayout();

            this.txtDetail.Text = string.Empty;
            this.lblUser.Text = string.Empty;
            this.lblSoftware.Text = string.Empty;
            this.lblUpdatedAt.Text = string.Empty;

            // ユーザID/名
            string txtUserId = TL.USERID;
            string txtUserName = TL.USERNAME;
            string txtUserInstance = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.User.Host ?? OriginalHost);
            this.lblUser.Text += "@" + txtUserId + "@" + txtUserInstance + "/" + txtUserName;
            this.lblTLFrom.Text = "source:" + TL.TLFROM;

            //CW
            string txtCW = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.CW);
            if (txtCW != string.Empty)
            {
                this.txtDetail.Text += "【CW】";
                this.txtDetail.Text += txtCW + "\r\n";
                this.txtDetail.Text += "\r\n";
            }

            // 本文
            string txtDetail = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.Text);
            if (txtDetail != string.Empty)
            {
                this.txtDetail.Text += txtDetail;
            }
            if (TL.RENOTED)
            {
                if (txtDetail != string.Empty)
                {
                    this.txtDetail.Text += "\r\n";
                    this.txtDetail.Text += "--------------------\r\n";
                }
                if (JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.Renote.CW) != string.Empty)
                {
                    this.txtDetail.Text += "【CW】";
                    this.txtDetail.Text += JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.Renote.CW) + "\r\n";
                    this.txtDetail.Text += "\r\n";
                }
                this.txtDetail.Text += "RN: \r\n\r\n";
                this.txtDetail.Text += JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.Renote.User.UserName) + "/" + JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.Renote.User.Name) + "\r\n";
                this.txtDetail.Text += JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.Renote.Text) + "\r\n";
            }
            if (TL.REPLAYED)
            {
                if (txtDetail != string.Empty)
                {
                    this.txtDetail.Text += "\r\n";
                    this.txtDetail.Text += "--------------------\r\n";
                }
                if (JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.Reply.CW) != string.Empty)
                {
                    this.txtDetail.Text += "【CW】";
                    this.txtDetail.Text += JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.Reply.CW) + "\r\n";
                    this.txtDetail.Text += "\r\n";
                }
                this.txtDetail.Text += JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.Reply.User.UserName) + "/" + JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.Reply.User.Name) + "\r\n";
                this.txtDetail.Text += JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.Reply.Text) + "\r\n";
            }

            string txtSoftware = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.User.Instance.SoftwareName);
            string txtSoftwareVer = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Node).Note.User.Instance.SoftwareVersion);
            if (txtSoftware + txtSoftwareVer != string.Empty)
            {
                this.lblSoftware.Text += txtSoftware + txtSoftwareVer;
            }

            this.pnMain.ResumeLayout(false);
        }

        #region 外部から呼び出し
        public event EventHandler<DataContainerEventArgs>? DataAccepted;
        public void CallDataAccepted(TimeLineContainer? Container) => DataAccepted?.Invoke(this, new DataContainerEventArgs() { Container = Container });
        private void OnDataAccepted(object? sender, DataContainerEventArgs? Container)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(OnDataAccepted, sender, Container);
            }
            if (Container == null)
            {
                return;
            }
        }

        public event EventHandler<DataContainerEventArgs>? DataRejected;
        public void CallDataRejected(TimeLineContainer? Container) => DataRejected?.Invoke(this, new DataContainerEventArgs() { Container = Container });
        private void OnDataRejected(object? sender, DataContainerEventArgs? Container)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(OnDataRejected, sender, Container);
            }
            if (Container == null)
            {
                return;
            }
        }
        #endregion

        #region 設定イベント
        /// <summary>
        /// 設定変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingFormSettingChanged(object? sender, SettingChangeEventArgs e)
        {
            WebSocketManager? WSManager = e._WSManager;
            int? WSDefinition = e._WSDefinition;
            DataGridTimeLine? Grid = e._GridTimeLine;
            bool? UpdateIntg = e.UpdateIntg;

            if (WSManager != null & WSDefinition != null)
            {
                // TimeLineManager更新
                this._WSManager[(int)WSDefinition] = WSManager;
            }
            if (Grid != null)
            {
                // DataGridTimeLine更新
                // _TLCreator.SetTimeLineObjectDirect(ref this.MainFormObj, e._WSDefinition, Grid);
            }
            if (UpdateIntg != null)
            {
                // 統合TLへの反映をするかどうか
            }
        }
        #endregion

        #region フォームイベント
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                var n = this._WSManager.ToArray<WebSocketManager>()
                                       .Select(r => { return SettingWebSocket.ConvertWebSocketManagerToSettingObj(r); })
                                       .ToArray();
                SettingController.SaveWebSockets(n);
                var j = this._TmpTLManager.ToArray()
                                          .Select(r => { return SettingTimeLine.ConvertDataGridTimeLineToSettingObj(_TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, r.Value)); })
                                          .ToArray();
                SettingController.SaveTimeLine(j);
            }
            catch
            {
            }
        }
        #endregion

        #region ToolStripMenuItem
        /// <summary>
        /// API設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tspAPISetting_Click(object sender, EventArgs e)
        {
            Dictionary<string, DataGridTimeLine> Grids = new Dictionary<string, DataGridTimeLine>();
            var MainGrid = _TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, "Main");
            Grids.Add("Main", MainGrid);
            foreach (TabPage tp in this.tbMain.TabPages)
            {
                try
                {
                    var tpGrid = _TLCreator.GetTimeLineObjectDirect(ref this.MainFormObj, _TmpTLManager[tp.Text]);
                    Grids.Add(_TmpTLManager[tp.Text], tpGrid);
                }
                catch (Exception ex)
                {
                }
            }
            _TLSettingForm.SetTLGrids(Grids);
            _TLSettingForm.SetTLManagers(this._TLManager, this._TmpTLManager, this._WSManager);
            _TLSettingForm.ShowDialog();
        }

        /// <summary>
        /// タイムライン設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tspTimeLineSetting_Click(object sender, EventArgs e)
        {
        }
        #endregion
    }
}
