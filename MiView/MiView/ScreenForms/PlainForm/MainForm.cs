using MiView.Common.AnalyzeData;
using MiView.Common.Connection.WebSocket;
using MiView.Common.Connection.WebSocket.Event;
using MiView.Common.Connection.WebSocket.Misskey.v2025;
using MiView.Common.Fonts;
using MiView.Common.Fonts.Material;
using MiView.Common.Notification.Baloon;
using MiView.Common.Notification.Toast;
using MiView.Common.TimeLine;
using MiView.ScreenForms.Controls.Combo;
using MiView.ScreenForms.Controls.Notify;
using MiView.ScreenForms.DialogForm;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Policy;
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
        /// タイムラインマネージャ
        /// </summary>
        private Dictionary<string, WebSocketTimeLineCommon> _TLManager = new Dictionary<string, WebSocketTimeLineCommon>();

        /// <summary>
        /// 一時タイムラインマネージャ
        /// </summary>
        private Dictionary<string, string> _TmpTLManager = new Dictionary<string, string>();

        public NotifyView NotifyView { get; set; }

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
        }

        private List<DataGridTimeLine> DGrids = new List<DataGridTimeLine>();
        private void TabUpdate()
        {
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _TLCreator.CreateTimeLine(ref this.MainFormObj, "Main", "tpMain");

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

        public void AddTimeLine(string InstanceURL, string TabName, string APIKey, TimeLineBasic.ConnectTimeLineKind sTLKind, bool IsFiltered = false, bool AvoidIntg = false, bool IsVisible = true)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(AddTimeLine, InstanceURL, TabName, APIKey, sTLKind, IsFiltered, AvoidIntg, IsVisible);
                return;
            }

            var TLKind = sTLKind;

            // タブ識別
            var TabDef = System.Guid.NewGuid().ToString();

            // タブ追加
            _TLCreator.CreateTimeLineTab(ref this.MainFormObj, TabDef, TabName, IsVisible);
            _TLCreator.CreateTimeLine(ref this.MainFormObj, TabDef, TabDef, IsFiltered: IsFiltered);

            var WSManager = WebSocketTimeLineCommon.CreateInstance(TLKind);
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
                    WebSocketTimeLineCommon.ReadTimeLineContinuous(WSManager);

                    if (APIKey != string.Empty)
                    {
                        var WTManager = WebSocketMain.CreateInstance().OpenMain(InstanceURL, APIKey);
                        WebSocketMain.ReadMainContinuous(WTManager);
                    }
                }
                catch (Exception ex)
                {
                }

                _TLManager.Add(TabDef, WSManager);
                _TmpTLManager.Add(TabName, TabDef);
            }
            catch
            {
            }
            if (WSManager.GetSocketState() != System.Net.WebSockets.WebSocketState.Open)
            {
                MessageBox.Show("インスタンスの読み込みに失敗しました。");
                return;
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
            _TLCreator.CreateTimeLine(ref this.MainFormObj, TabDef, TabDef, IsFiltered: IsFiltered);
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

        private void tbMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            //((TabControl)sender).SuspendLayout();

            var TPages = ((TabControl)sender).TabPages;
            foreach (TabPage TPage in TPages)
            {
                foreach (DataGridTimeLine DGView in TPage.Controls.Cast<Control>().ToList().FindAll(r => { return r.GetType() == typeof(DataGridTimeLine); }))
                {
                    DGView.Visible = true;
                    // DGView.Visible = TPages.IndexOf(TPage) == ((TabControl)sender).SelectedIndex;

                    //if (DGView.Visible)
                    //{
                    //    DGView.Refresh();
                    //}
                }
            }
            //((TabControl)sender).ResumeLayout(false);
        }

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
    }
}
