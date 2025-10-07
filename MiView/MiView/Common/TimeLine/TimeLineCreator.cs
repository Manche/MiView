using MiView.Common.AnalyzeData;
using MiView.Common.Connection.VersionInfo;
using MiView.Common.Fonts;
using MiView.Common.Fonts.Material;
using MiView.Common.Notification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static MiView.Common.TimeLine.TimeLineCreator;

namespace MiView.Common.TimeLine
{
    /// <summary>
    /// タイムラインコンテナ作成・識別
    /// </summary>
    internal class TimeLineCreator
    {
        public enum TIMELINE_ELEMENT
        {
            UNDESIGNATED = -1,
            /// <summary>
            /// 投稿識別キー
            /// </summary>
            IDENTIFIED,
            /// <summary>
            /// アイコン
            /// </summary>
            ICON,
            /// <summary>
            /// ユーザー名
            /// </summary>
            USERNAME,
            /// <summary>
            /// ユーザID
            /// </summary>
            USERID,
            /// <summary>
            /// リプライ
            /// </summary>
            REPLAYED,
            /// <summary>
            /// リプライ表示
            /// </summary>
            REPLAYED_DISP,
            /// <summary>
            /// 公開範囲
            /// </summary>
            PROTECTED,
            /// <summary>
            /// 公開範囲表示
            /// </summary>
            PROTECTED_DISP,
            /// <summary>
            /// リノート
            /// </summary>
            RENOTED,
            /// <summary>
            /// リノート表示
            /// </summary>
            RENOTED_DISP,
            /// <summary>
            /// ローカル
            /// </summary>
            ISLOCAL,
            /// <summary>
            /// ローカル表示
            /// </summary>
            ISLOCAL_DISP,
            /// <summary>
            /// チャンネル
            /// </summary>
            ISCHANNEL,
            /// <summary>
            /// チャンネル名
            /// </summary>
            CHANNEL_NAME,
            /// <summary>
            /// チャンネル表示
            /// </summary>
            ISCHANNEL_DISP,
            /// <summary>
            /// CW
            /// </summary>
            CW,
            /// <summary>
            /// CW表示
            /// </summary>
            CW_DISP,
            /// <summary>
            /// 投稿内容
            /// </summary>
            DETAIL,
            /// <summary>
            /// 投稿日時
            /// </summary>
            UPDATEDAT,
            /// <summary>
            /// 投稿元インスタンス
            /// </summary>
            SOURCE,
            /// <summary>
            /// 投稿元ソフトウェア情報
            /// </summary>
            SOFTWARE,
            /// <summary>
            /// ソフトウェア偽装有無
            /// </summary>
            SOFTWARE_INVALIDATED,
            /// <summary>
            /// 投稿元オリジナルjson情報
            /// </summary>
            ORIGINAL,
            /// <summary>
            /// 投稿元オリジナルホスト
            /// </summary>
            ORIGINAL_HOST,
            /// <summary>
            /// 読み取り元
            /// </summary>
            TLFROM,
            /// <summary>
            /// バージョン
            /// </summary>
            VERSION,
        }

        /// <summary>
        /// 非表示対象
        /// </summary>
        public static TIMELINE_ELEMENT[] _DisabledElements = new TIMELINE_ELEMENT[]
        {
            TIMELINE_ELEMENT.CW,
            TIMELINE_ELEMENT.UNDESIGNATED,
            TIMELINE_ELEMENT.IDENTIFIED,
            TIMELINE_ELEMENT.RENOTED,
            TIMELINE_ELEMENT.PROTECTED,
            TIMELINE_ELEMENT.ISLOCAL,
            TIMELINE_ELEMENT.REPLAYED,
            TIMELINE_ELEMENT.ORIGINAL,
            TIMELINE_ELEMENT.CHANNEL_NAME,
            TIMELINE_ELEMENT.ISCHANNEL,
            // TIMELINE_ELEMENT.TLFROM,
            TIMELINE_ELEMENT.SOFTWARE_INVALIDATED,
            TIMELINE_ELEMENT.ORIGINAL_HOST,
            TIMELINE_ELEMENT.VERSION,
        };

        public List<TimeLineContainer> TimeLineData = new List<TimeLineContainer>();

        private MainForm? _MainForm { get; set; }

        /// <summary>
        /// タイムライン管理オブジェクト
        /// </summary>
        private Dictionary<string, DataGridTimeLine> Grids = new Dictionary<string, DataGridTimeLine>();

        public TimeLineCreator()
        {
        }

        /// <summary>
        /// フォームオブジェクトの取得
        /// </summary>
        /// <param name="MainForm"></param>
        /// <param name="Definition"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public DataGridTimeLine GetTimeLineObjectDirect(ref MainForm MainForm, string Definition)
        {
            if (!this.Grids.ContainsKey(Definition))
            {
                throw new KeyNotFoundException();
            }
            return this.Grids[Definition];
        }

        /// <summary>
        /// メインフォームへタイムラインを追加
        /// </summary>
        /// <param name="MainForm"></param>
        public void CreateTimeLine(ref MainForm MainForm, string Definition, string? ChildDefinition = null, bool IsFiltered = false, bool IsVisible = true)
        {
            // コントロールがあるか検索
            var tpObj = GetControlFromMainForm(ref MainForm, ChildDefinition);
            if (tpObj != null)
            {
                this._MainForm = MainForm;

                System.Diagnostics.Debug.WriteLine("hoge");
                DataGridTimeLine Grid = new DataGridTimeLine();
                ((System.ComponentModel.ISupportInitialize)Grid).BeginInit();
                Grid.Visible = IsVisible;

                //
                // Property
                //
                Grid._IsFiltered = IsFiltered;

                // 
                // Grid
                // 
                Grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                Grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                Grid.Location = new Point(3, 3);
                Grid.Name = "dataGridTimeLine1";
                Grid.Size = new Size(770, 299);
                Grid.TabIndex = 0;
                Grid.RowHeadersVisible = false;
#if !DEBUG
                            Grid.ColumnHeadersVisible = false;
#endif
                Grid.CellBorderStyle = DataGridViewCellBorderStyle.None;
                Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                Grid.AllowUserToAddRows = false;
                Grid.AllowUserToDeleteRows = false;
                Grid.AllowUserToResizeColumns = false;
                Grid.AllowUserToResizeRows = false;
                Grid.ReadOnly = true;

                ((System.ComponentModel.ISupportInitialize)Grid).EndInit();

                this.AddDbg(Grid);

                Grid.CurrentCellChanged += CurrentGridCellChanged;

                if (tpObj.InvokeRequired)
                {
                    tpObj.Invoke(() => {
                        tpObj.Controls.Add(Grid);
                    });
                }
                else
                {
                    tpObj.Controls.Add(Grid);
                }
                Grids.Add(Definition, Grid);

                //tpObj.Select();
                if (tpObj.GetType() == typeof(TabPage))
                {
                    this._MainForm.SelectTabPage(tpObj.Name);
                }
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        private void CurrentGridCellChanged(object? sender, EventArgs e)
        {
            // Object未セット
            if (this._MainForm == null || sender == null)
            {
                return;
            }
            var Grid = (DataGridTimeLine)sender;

            if (Grid.Visible == false)
            {
                return;
            }

            var CurrentCell = Grid.CurrentCell;
            var CurrentRow = Grid.CurrentRow;

            // 初期読み込み時
            if (CurrentCell == null || CurrentRow == null)
            {
                return;
            }

            var CurrentRowData = Grid.Rows[CurrentRow.Index];
            string OriginalHost = CurrentRowData.Cells[(int)TIMELINE_ELEMENT.ORIGINAL_HOST].Value.ToString() ?? string.Empty;
            var Node = CurrentRowData.Cells[(int)TIMELINE_ELEMENT.ORIGINAL].Value;

            if (Node == null || Node.ToString() == string.Empty)
            {
                return;
            }

            // TL情報をセット
            this._MainForm.SetTimeLineContents(OriginalHost, (JsonNode)Node);
        }

        public void CreateTimeLineTab(ref MainForm MainForm, string Name, string Text, bool Visible = true)
        {
            var tpObj = GetControlFromMainForm(ref MainForm, null);
            if (tpObj != null)
            {
                TabPage tp = new TabPage();
                // 
                // tpMain
                // 
                tp.Location = new Point(4, 4);
                tp.Name = Name;
                tp.Padding = new Padding(3);
                tp.Size = new Size(776, 305);
                tp.TabIndex = 0;
                tp.Text = Text;
                tp.UseVisualStyleBackColor = true;

                if (tpObj.InvokeRequired)
                {
                    tpObj.Invoke(() => { tpObj.Controls.Add(tp); });
                }
                else
                {
                    tpObj.Controls.Add(tp);
                }
                if (MainForm.InvokeRequired)
                {
                    MainForm.Invoke(() => { tp.Focus(); });
                }
                else
                {
                    tp.Focus();
                }
            }
        }

        private Control? GetControlFromMainForm(ref MainForm MainForm, string? ChildDefinition)
        {
            var tpObj = MainForm.Controls.Cast<Control>().ToList().Find(r => { return r.Name == "tbMain"; });
            if (ChildDefinition != null)
            {
                var tpObjb = tpObj.Controls.Find(ChildDefinition, false);
                if (tpObjb.Length > 0)
                {
                    tpObj = tpObj.Controls.Find(ChildDefinition, false)[0];
                }
            }
            return tpObj;
        }

        /// <summary>
        /// デバッグ情報付記
        /// </summary>
        /// <param name="DgTimeLine"></param>
        private void AddDbg(DataGridTimeLine DgTimeLine)
        {
            var UpdDefault = new DateTime(1900, 1, 1, 1, 1, 1);
#if DEBUG
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct,
                ISLOCAL = true,
                DETAIL = "これはデバッグ実行時に表示されます。",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault,
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct,
                ISLOCAL = true,
                RENOTED = true,
                DETAIL = "リノート表示。",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public,
                ISLOCAL = true,
                RENOTED = false,
                REPLAYED = true,
                DETAIL = "リプライ表示。",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public,
                ISLOCAL = true,
                RENOTED = false,
                REPLAYED = false,
                ISCHANNEL = true,
                CHANNEL_NAME = "test",
                DETAIL = "チャンネル表示。",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public,
                ISLOCAL = true,
                RENOTED = false,
                REPLAYED = true,
                CW = true,
                DETAIL = "CW",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public,
                ISLOCAL = true,
                RENOTED = true,
                REPLAYED = true,
                CW = true,
                DETAIL = "ごった煮",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public,
                ISLOCAL = true,
                RENOTED = false,
                DETAIL = "パブリック",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.SemiPublic,
                ISLOCAL = true,
                RENOTED = false,
                DETAIL = "セミパブリック",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Home,
                ISLOCAL = true,
                RENOTED = false,
                DETAIL = "ホーム",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Follower,
                ISLOCAL = true,
                RENOTED = false,
                DETAIL = "フォロワー",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct,
                ISLOCAL = true,
                RENOTED = false,
                DETAIL = "ダイレクトメッセージ",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct,
                ISLOCAL = false,
                RENOTED = false,
                DETAIL = "abcdefghijklmnopqrstuvwxyz",
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
            DgTimeLine.InsertTimeLineData(new TimeLineContainer()
            {
                PROTECTED = TimeLineContainer.PROTECTED_STATUS.Direct,
                ISLOCAL = false,
                RENOTED = false,
                DETAIL = "abcdefghijklmnopqrstuvwxyz".ToUpper(),
                USERID = "MiVIEW-SYSTEM",
                USERNAME = "アプリ",
                SOFTWARE = "MiView 0.0.1",
                SOURCE = "localhost",
                UPDATEDAT = UpdDefault
            });
#endif
        }

        /// <summary>
        /// メインフォームからタイムラインを除去
        /// </summary>
        /// <param name="MainForm"></param>
        /// <param name="Definition"></param>
        /// <param name="ChildDefinition"></param>
        /// <exception cref="TimeLineNotFoundException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public void DeleteTimeLine(ref MainForm MainForm, string Definition, string? ChildDefinition = null)
        {
            if (!this.Grids.ContainsKey(Definition))
            {
                throw new TimeLineNotFoundException(null, Definition);
            }
            // コントロールがあるか検索
            var tpObj = GetControlFromMainForm(ref MainForm, ChildDefinition);
            if (tpObj != null)
            {
                tpObj.Controls.Remove(this.Grids[Definition]);
                this.Grids.Remove(Definition);
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }

    internal class TimeLineNotFoundException : Exception
    {
        /// <summary>
        /// 定義名
        /// </summary>
        private string Definition {  get; set; } = string.Empty;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimeLineNotFoundException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message"></param>
        public TimeLineNotFoundException(string? message) : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Definition"></param>
        public TimeLineNotFoundException(string? message, string Definition)
        {
        }

        public override string ToString()
        {
            return this.Definition != string.Empty ? this.Definition: base.ToString();
        }

        public string CallDefinition()
        {
            return this.Definition;
        }
    }

    /// <summary>
    /// タイムラインオブジェクト
    /// </summary>
    public class TimeLineContainer
    {
        public TimeLineContainer() { }

        public enum PROTECTED_STATUS
        {
            Public,
            SemiPublic,
            Home,
            Follower,
            Direct,
        }

        public string IDENTIFIED { get; set; } = string.Empty;
        public string ICON { get; set; } = string.Empty;
        public string USERNAME { get; set; } = string.Empty;
        public string USERID { get; set; } = string.Empty;
        public bool REPLAYED { get; set; } = false;
        public string? REPLAYED_DISP {  get; set; }
        public PROTECTED_STATUS PROTECTED { get; set; } = PROTECTED_STATUS.Public;
        public string? PROTECTED_DISP { get; set; }
        public bool RENOTED { get; set; } = false;
        public string? RENOTED_DISP { get; set; }
        public bool ISLOCAL { get; set; } = false;
        public string? ISLOCAL_DISP {  get; set; }
        public bool ISCHANNEL { get; set; } = false;
        public string? CHANNEL_NAME { get; set; }
        public string? ISCHANNEL_DISP { get; set; }
        public bool CW {  get; set; } = false;
        public string? CW_DISP { get; set; }
        public string DETAIL { get; set; } = string.Empty;
        public string CONTENT {  get; set; } = string.Empty;
        public DateTime? UPDATEDAT { get; set; } = null;
        public string SOURCE { get; set; } = string.Empty;
        public string SOFTWARE { get; set; } = string.Empty;
        public bool SOFTWARE_INVALIDATED { get; set; } = false;
        public JsonNode ORIGINAL { get; set; } = string.Empty;
        public string ORIGINAL_HOST {  get; set; } = string.Empty;
        public string TLFROM { get; set; } = string.Empty;

        public MMisskeyVersionInfo VERSION { get; set; }

        public static string[] TRANSABLE =
        {
            "USERNAME",
            "USERID",
            "CHANNEL_NAME",
            "CW",
            "DETAIL",
            "UPDATEAT",
            "SOURCE",
            "SOFTWARE",
        };

    }

    /// <summary>
    /// タイムラインコントロール
    /// </summary>
    partial class DataGridTimeLine : System.Windows.Forms.DataGridView
    {
        private List<TimeLineContainer> _TimeLineData = new List<TimeLineContainer>();

        /// <summary>
        /// 空文字
        /// </summary>
        private static string _Common_Empty = string.Empty;

        /// <summary>
        /// メンション(あっとまーく)
        /// </summary>
        private static string _Common_Alternate_Email = MaterialIcons.AlternateEmail;

        /// <summary>
        /// RN
        /// </summary>
        private static string _Common_Repeat = MaterialIcons.Repeat;

        /// <summary>
        /// パブリック
        /// </summary>
        private static string _Common_Public = MaterialIcons.Language;
        /// <summary>
        /// はなモード・セミパブリック
        /// </summary>
        private static string _Common_Wifi = MaterialIcons.Wifi;
        /// <summary>
        /// ホーム
        /// </summary>
        private static string _Common_Home = MaterialIcons.Home;
        /// <summary>
        /// DM
        /// </summary>
        private static string _Common_Direct = MaterialIcons.Mail;
        /// <summary>
        /// フォロワー
        /// </summary>
        private static string _Common_Locked = MaterialIcons.Lock;

        /// <summary>
        /// 連合
        /// </summary>
        private static string _Common_Rocket_Launch = MaterialIcons.RocketLaunch;
        /// <summary>
        /// ローカルのみ
        /// </summary>
        /// <remarks>
        /// 赤字にすること
        /// </remarks>
        private static string _Common_Rocket = MaterialIcons.Rocket;

        /// <summary>
        /// CW表示
        /// </summary>
        private static string _Common_Visibility_Off = MaterialIcons.VisibilityOff;
        /// <summary>
        /// チャンネル表示
        /// </summary>
        private static string _Common_Channel = MaterialIcons.Tv;

        /// <summary>
        /// フィルタTLかどうか
        /// </summary>
        public bool _IsFiltered = false;
        /// <summary>
        /// フィルタリングオプション
        /// </summary>
        public List<TimeLineFilterlingOption> _FilteringOptions = new List<TimeLineFilterlingOption>();
        /// <summary>
        /// Grid別条件一致モード
        /// 
        /// true=全部一致 false=いずれか一致
        /// </summary>
        public bool _FilterMode = true;
        /// <summary>
        /// アラートオプション
        /// </summary>
        public List<TimeLineAlertOption> _AlertOptions = new List<TimeLineAlertOption>();
        public List<TimeLineAlertOption> _AlertAccept
        {
            get
            {
                return _AlertOptions.FindAll(r => { return r._Alert_Timing == TimeLineAlertOption.ALERT_TIMING.ACCEPT; });
            }
        }
        public List<TimeLineAlertOption> _AlertReject
        {
            get
            {
                return _AlertOptions.FindAll(r => { return r._Alert_Timing == TimeLineAlertOption.ALERT_TIMING.REJECT; });
            }
        }

        /// <summary>
        /// タイムライン更新描画をするかどうか
        /// </summary>
        /// <remarks>
        /// デフォルトではON
        /// </remarks>
        public bool _IsUpdateTL = true;

        /// <summary>
        /// フィルタに投稿を設定
        /// </summary>
        /// <param name="Container"></param>
        public void SetTimeLineFilter(TimeLineContainer Container)
        {
            if (this._FilteringOptions == null)
            {
                return;
            }
            foreach (TimeLineFilterlingOption Opt in this._FilteringOptions)
            {
                Opt._Container = Container;
            }
            foreach (TimeLineAlertOption Alt in this._AlertOptions)
            {
                foreach (TimeLineFilterlingOption Flt in Alt._FilterOptions)
                {
                    Flt._Container = Container;
                }
            }
        }

        /// <summary>
        /// 列幅
        /// </summary>
        private static Dictionary<TimeLineCreator.TIMELINE_ELEMENT, int> _ColumWidths = new Dictionary<TIMELINE_ELEMENT, int>()
        {
            { TIMELINE_ELEMENT.UNDESIGNATED, -1 },
            { TIMELINE_ELEMENT.ICON, 20 },
            { TIMELINE_ELEMENT.USERNAME, 60 },
            { TIMELINE_ELEMENT.USERID, 60 },
            { TIMELINE_ELEMENT.REPLAYED_DISP, 20 },
            { TIMELINE_ELEMENT.PROTECTED_DISP, 20 },
            { TIMELINE_ELEMENT.ISLOCAL_DISP, 20 },
            { TIMELINE_ELEMENT.RENOTED_DISP, 20 },
            { TIMELINE_ELEMENT.CW_DISP, 20 },
            { TIMELINE_ELEMENT.ISCHANNEL_DISP, 20 },
            { TIMELINE_ELEMENT.DETAIL, 350 },
            { TIMELINE_ELEMENT.SOFTWARE, 40 },
            { TIMELINE_ELEMENT.UPDATEDAT, 140 },
            { TIMELINE_ELEMENT.SOURCE, 60 }
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataGridTimeLine()
        {
            // コントロールが黒くなる不具合ある
            // this.DoubleBuffered = true;
            this.VirtualMode = true;
            this.CellValueNeeded += OnCellValueNeeded;
            this.CellFormatting += OnCellFormatting;
            this.ReadOnly = true;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;

            // 初期設定
            var DefaultMaterialFont = FontLoader.Instance.LoadFontFromFile(FontLoader.FONT_SELECTOR.MATERIALICONS, 8);
            foreach (string ColName in Enum.GetNames(typeof(TimeLineCreator.TIMELINE_ELEMENT)))
            {
                DataGridViewColumn Col = new DataGridViewColumn();
                Col.Name = ColName;
                Col.CellTemplate = new DataGridViewTextBoxCell();

                // 列幅
                if (_ColumWidths.ContainsKey((TimeLineCreator.TIMELINE_ELEMENT)Enum.Parse(typeof(TimeLineCreator.TIMELINE_ELEMENT), ColName)))
                {
                    Col.Width = _ColumWidths[(TimeLineCreator.TIMELINE_ELEMENT)Enum.Parse(typeof(TimeLineCreator.TIMELINE_ELEMENT), ColName)];
                }

                // マーク列
                if (ColName == TimeLineCreator.TIMELINE_ELEMENT.REPLAYED_DISP.ToString() ||
                    ColName == TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP.ToString() ||
                    ColName == TimeLineCreator.TIMELINE_ELEMENT.ISLOCAL_DISP.ToString() ||
                    ColName == TimeLineCreator.TIMELINE_ELEMENT.RENOTED_DISP.ToString() ||
                    ColName == TimeLineCreator.TIMELINE_ELEMENT.CW_DISP.ToString() ||
                    ColName == TimeLineCreator.TIMELINE_ELEMENT.ISCHANNEL_DISP.ToString())
                {
                    Col.DefaultCellStyle.Font = DefaultMaterialFont;
                }
                // 制御列
                if (TimeLineCreator._DisabledElements.Contains((TimeLineCreator.TIMELINE_ELEMENT)Enum.Parse(typeof(TimeLineCreator.TIMELINE_ELEMENT), ColName)))
                {
                    Col.Visible = false;
                }
                this.Columns.Add(Col);
            }
        }

        private void OnCellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _TimeLineData.Count) return;

            var TLData = _TimeLineData[e.RowIndex];
            string colName = this.Columns[e.ColumnIndex].Name;
            var prop = TLData.GetType().GetProperty(colName);
            if (prop != null)
            {
                e.Value = prop.GetValue(TLData);
            }
        }

        private void OnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _TimeLineData.Count)
                return;

            var TLData = _TimeLineData[e.RowIndex];

            foreach (string ColName in Enum.GetNames(typeof(TimeLineCreator.TIMELINE_ELEMENT)))
            {
                var Prop = typeof(TimeLineContainer).GetProperty(ColName);
                if (Prop == null)
                {
                    continue;
                }
                this.ArrangeTimeLine(e.RowIndex, (int)Enum.Parse(typeof(TimeLineCreator.TIMELINE_ELEMENT), ColName));
            }
            var CCellStyle = e;
            this.ChangeDispColor(ref CCellStyle, TLData);
        }

        private static int _cntGlobal = 0;

        /// <summary>
        /// 行挿入
        /// </summary>
        /// <param name="Container"></param>
        public void InsertTimeLineData(TimeLineContainer Container)
        {
            if (!_IsUpdateTL)
            {
                return;
            }
            try
            {
                _cntGlobal++;
                System.Diagnostics.Debug.WriteLine(_cntGlobal);

                // TL統合
                var Intg = this._TimeLineData.Cast<TimeLineContainer>().Where(r => r.IDENTIFIED.Equals(Container.IDENTIFIED)).ToArray();
                if (Intg.Count() > 0)
                {
                    var CtlVal = (Intg[0]).TLFROM.ToString();
                    if (CtlVal != string.Empty)
                    {
                        if (!CtlVal.Split(',').Contains(Container.TLFROM))
                        {
                            (Intg[0]).TLFROM = CtlVal + "," + Container.TLFROM;
                        }
                    }
                    else
                    {
                        (Intg[0]).TLFROM = CtlVal + "," + Container.TLFROM;
                    }
                    //this.ResumeLayout();
                    return;
                }


                // 行挿入
                //this.Rows.Add();
                this._TimeLineData.Add(Container);
                this.RowCount = this._TimeLineData.Count;

                int CurrentRowIndex = this.RowCount - 1;

                // 基本行高さ
                this.Rows[CurrentRowIndex].Height = 20;

                // フォントは行ごとに定義する
                // defaultだと反映されない
                //var DefaultMaterialFont = new FontLoader().LoadFontFromFile(FontLoader.FONT_SELECTOR.MATERIALICONS, 12);
                //this.Rows[0].Cells[(int)TIMELINE_ELEMENT.REPLAYED_DISP].Style.Font = DefaultMaterialFont;
                //this.Rows[0].Cells[(int)TIMELINE_ELEMENT.ISLOCAL_DISP].Style.Font = DefaultMaterialFont;
                //this.Rows[0].Cells[(int)TIMELINE_ELEMENT.PROTECTED_DISP].Style.Font = DefaultMaterialFont;
                //this.Rows[0].Cells[(int)TIMELINE_ELEMENT.RENOTED_DISP].Style.Font = DefaultMaterialFont;
                //this.Rows[0].Cells[(int)TIMELINE_ELEMENT.CW_DISP].Style.Font = DefaultMaterialFont;
                //this.Rows[0].Cells[(int)TIMELINE_ELEMENT.ISCHANNEL_DISP].Style.Font = DefaultMaterialFont;
                //DefaultMaterialFont.Dispose();

                // カラム別処理
                //foreach (string ColName in Enum.GetNames(typeof(TimeLineCreator.TIMELINE_ELEMENT)))
                //{
                //    var Prop = typeof(TimeLineContainer).GetProperty(ColName);
                //    if (Prop == null)
                //    {
                //        continue;
                //    }
                //    var PropVal = Prop.GetValue(Container);

                //    if (PropVal != null)
                //    {
                //        this.Rows[CurrentRowIndex].Cells[ColName].Value = PropVal;
                //    }

                //    this.ArrangeTimeLine(CurrentRowIndex, (int)Enum.Parse(typeof(TimeLineCreator.TIMELINE_ELEMENT), ColName));

                //    var Row = this.Rows[CurrentRowIndex];

                //    // 色変更
                //    // this.ChangeDispColor(ref Row, Container);
                //}
            }
            catch(Exception ce)
            {
                System.Diagnostics.Debug.WriteLine(ce);
            }
            finally
            {
                this.ResumeLayout(false);
            }
            System.Diagnostics.Debug.WriteLine("ttt");
            //this.Refresh();
        }

        /// <summary>
        /// タイムライン整形
        /// </summary>
        /// <param name="RowIndex"></param>
        /// <param name="ColumnIndex"></param>
        private void ArrangeTimeLine(int RowIndex, int ColumnIndex)
        {
            if (RowIndex == -1)
            {
                return;
            }
            if (ColumnIndex != (int)TimeLineCreator.TIMELINE_ELEMENT.REPLAYED &&
                ColumnIndex != (int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED &&
                ColumnIndex != (int)TimeLineCreator.TIMELINE_ELEMENT.ISLOCAL &&
                ColumnIndex != (int)TimeLineCreator.TIMELINE_ELEMENT.RENOTED &&
                ColumnIndex != (int)TimeLineCreator.TIMELINE_ELEMENT.CW &&
                ColumnIndex != (int)TimeLineCreator.TIMELINE_ELEMENT.ISCHANNEL &&
                ColumnIndex != (int)TimeLineCreator.TIMELINE_ELEMENT.CHANNEL_NAME &&
                ColumnIndex != (int)TimeLineCreator.TIMELINE_ELEMENT.SOFTWARE_INVALIDATED &&
                ColumnIndex != (int)TimeLineCreator.TIMELINE_ELEMENT.UPDATEDAT)
            {
                return;
            }

            var CellValue = this.Rows[RowIndex].Cells[ColumnIndex].Value;
            // var CellValue = this._TimeLineData[RowIndex]
            if (CellValue == null)
            {
                CellValue = string.Empty;
            }
            switch (ColumnIndex)
            {
                case (int)TimeLineCreator.TIMELINE_ELEMENT.REPLAYED:
                    // this.Rows[RowIndex].Cells[TimeLineCreator.TIMELINE_ELEMENT.REPLAYED_DISP.ToString()].Value
                    this._TimeLineData[RowIndex].REPLAYED_DISP
                            = (bool)CellValue ? _Common_Alternate_Email : _Common_Empty;
                    this.Rows[RowIndex].Cells[TimeLineCreator.TIMELINE_ELEMENT.REPLAYED_DISP.ToString()].ToolTipText
                            = (bool)CellValue ? "リプライ" : "";
                    this.Rows[RowIndex].Cells[TimeLineCreator.TIMELINE_ELEMENT.REPLAYED_DISP.ToString()].Style.ForeColor
                            = (bool)CellValue ? Color.Orange : Color.Red;
                    break;
                case (int)TimeLineCreator.TIMELINE_ELEMENT.CW:
                    if ((bool)CellValue)
                    {
                        // CWはdetailに突っ込む時に処理させる
                        this._TimeLineData[RowIndex].CW_DISP = _Common_Visibility_Off;
                        // this.Rows[RowIndex].Cells[TimeLineCreator.TIMELINE_ELEMENT.CW_DISP.ToString()].Value = _Common_Visibility_Off;
                        this.Rows[RowIndex].Cells[TimeLineCreator.TIMELINE_ELEMENT.CW_DISP.ToString()].ToolTipText = "CW";
                    }
                    break;
                case (int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED:
                    switch ((TimeLineContainer.PROTECTED_STATUS)CellValue)
                    {
                        case TimeLineContainer.PROTECTED_STATUS.Public:
                            this._TimeLineData[RowIndex].PROTECTED_DISP = _Common_Public;
                            //this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP].Value
                            //        = _Common_Public;
                            this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP].ToolTipText
                                    = "パブリック";
                            break;
                        case TimeLineContainer.PROTECTED_STATUS.SemiPublic:
                            this._TimeLineData[RowIndex].PROTECTED_DISP = _Common_Wifi;
                            //this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP].Value
                            //        = _Common_Wifi;
                            this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP].ToolTipText
                                    = "セミパブリック";
                            break;
                        case TimeLineContainer.PROTECTED_STATUS.Home:
                            this._TimeLineData[RowIndex].PROTECTED_DISP = _Common_Home;
                            //this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP].Value
                            //        = _Common_Home;
                            this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP].ToolTipText
                                    = "ホーム";
                            break;
                        case TimeLineContainer.PROTECTED_STATUS.Direct:
                            this._TimeLineData[RowIndex].PROTECTED_DISP = _Common_Direct;
                            //this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP].Value
                            //        = _Common_Direct;
                            this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP].ToolTipText
                                    = "ダイレクトメッセージ";
                            break;
                        case TimeLineContainer.PROTECTED_STATUS.Follower:
                            this._TimeLineData[RowIndex].PROTECTED_DISP = _Common_Locked;
                            //this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP].Value
                            //        = _Common_Locked;
                            this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP].ToolTipText
                                    = "フォロワー";
                            break;
                    }
                    break;
                case (int)TimeLineCreator.TIMELINE_ELEMENT.ISLOCAL:
                    this._TimeLineData[RowIndex].ISLOCAL_DISP
                    //this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.ISLOCAL_DISP].Value
                            = (bool)CellValue ? _Common_Rocket : _Common_Rocket_Launch;
                    this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.ISLOCAL_DISP].ToolTipText
                            = (bool)CellValue ? "ローカルのみ" : "連合";
                    this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.ISLOCAL_DISP].Style.ForeColor
                            = (bool)CellValue ? Color.Red : Color.Green;
                    break;
                case (int)TimeLineCreator.TIMELINE_ELEMENT.RENOTED:
                    this._TimeLineData[RowIndex].RENOTED_DISP
                    //this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.RENOTED_DISP].Value
                            = (bool)CellValue ? _Common_Repeat : _Common_Empty;
                    this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.RENOTED_DISP].ToolTipText
                            = (bool)CellValue ? "リノート" : "";
                    this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.RENOTED_DISP].Style.ForeColor
                            = (bool)CellValue ? Color.Green : Color.Red;
                    break;
                case (int)TimeLineCreator.TIMELINE_ELEMENT.ISCHANNEL:
                    this._TimeLineData[RowIndex].ISCHANNEL_DISP
                    //this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.ISCHANNEL_DISP].Value
                            = (bool)CellValue ? _Common_Channel : _Common_Empty;
                    this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.ISCHANNEL_DISP].Style.ForeColor
                            = (bool)CellValue ? Color.Green : Color.Red;
                    break;
                case (int)TimeLineCreator.TIMELINE_ELEMENT.CHANNEL_NAME:
                    if (this._TimeLineData[RowIndex].CHANNEL_NAME != null)
                    {
                        this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.ISCHANNEL_DISP].ToolTipText
                                = this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.CHANNEL_NAME].Value.ToString();
                    }
                    break;
                case (int)TimeLineCreator.TIMELINE_ELEMENT.SOFTWARE_INVALIDATED:
                    if ((bool)this._TimeLineData[RowIndex].SOFTWARE_INVALIDATED == true)
                    {
                        this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.SOFTWARE].ToolTipText
                                = "ソフトウェア偽装の可能性あり";
                    }
                    break;
                case (int)TimeLineCreator.TIMELINE_ELEMENT.UPDATEDAT:
                    if (this._TimeLineData[RowIndex].UPDATEDAT != null)
                    {
                        try
                        {
                            this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.UPDATEDAT].Value =
                                _TimeLineData[RowIndex].UPDATEDAT;
                        }
                        catch(Exception ce)
                        {
                            this.Rows[RowIndex].Cells[(int)TimeLineCreator.TIMELINE_ELEMENT.UPDATEDAT].Value = "1900/01/01 00:00:00";
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 文字色変更
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Container"></param>
        private void ChangeDispColor(ref DataGridViewCellFormattingEventArgs Row, TimeLineContainer Container)
        {
            if (Container.RENOTED)
            {
                this.ChangeDispFgColorCommon(ref Row, Color.Green);
            }
            if (Container.REPLAYED)
            {
                this.ChangeDispBgColorCommon(ref Row, Color.Beige);
            }
            if (Container.CW)
            {
                this.ChangeDispBgColorCommon(ref Row, Color.LightGray);
            }
        }

        /// <summary>
        /// フロント文字色変更
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="DesignColor"></param>
        private void ChangeDispFgColorCommon(ref DataGridViewCellFormattingEventArgs Row, Color DesignColor)
        {
            if (Row.CellStyle == null)
            {
                return;
            }
            try
            {
                Row.CellStyle.ForeColor = DesignColor;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 背景文字色変更
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="DesignColor"></param>
        private void ChangeDispBgColorCommon(ref DataGridViewCellFormattingEventArgs Row, Color DesignColor)
        {
            if (Row.CellStyle == null)
            {
                return;
            }
            try
            {
                Row.CellStyle.BackColor = DesignColor;
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// メインフォームと連携するためのイベントArgs
    /// </summary>
    internal class CurrentGridCellEventArgs : EventArgs
    {
        private MainForm _CurrentForm;

        public CurrentGridCellEventArgs(MainForm CurrentForm)
        {
            _CurrentForm = CurrentForm;
        }
    }

    /// <summary>
    /// タイムラインフィルタリング設定
    /// </summary>
    internal class TimeLineFilterlingOption
    {
        /// <summary>
        /// 一致条件
        /// </summary>
        public enum MATCH_MODE
        {
            /// <summary>
            /// 未指定
            /// </summary>
            NONE = -1,
            /// <summary>
            /// 全てが真
            /// </summary>
            ALL = 0,
            /// <summary>
            /// いずれかが真
            /// </summary>
            PARTIAL = 1,
            /// <summary>
            /// 一部重視
            /// </summary>
            IMPORTANCE = 2,
        }
        public MATCH_MODE _MODE = MATCH_MODE.NONE;

        /// <summary>
        /// 一致方法指定
        /// </summary>
        public enum MATCHER_PATTERN
        {
            /// <summary>
            /// なし
            /// </summary>
            NONE = 0,
            /// <summary>
            /// 一致
            /// </summary>
            MATCH = 1,
            /// <summary>
            /// 含む
            /// </summary>
            PATTERN = 2,
            /// <summary>
            /// ～で始まる
            /// </summary>
            START = 3,
            /// <summary>
            /// ～で終わる
            /// </summary>
            END = 4,
            /// <summary>
            /// 正規表現
            /// </summary>
            REGEXP = 5,
        }
        /// <summary>
        /// 一致方法
        /// </summary>
        public MATCHER_PATTERN _PATTERN = MATCHER_PATTERN.NONE;

        /// <summary>
        /// 反転(not)条件
        /// </summary>
        public bool CONSTRAINT_INVERT = false;

        /// <summary>
        /// ユーザID指定
        /// </summary>
        public bool _Match_UserId = false;
        /// <summary>
        /// ユーザID
        /// </summary>
        public List<string> _UserIds = new List<string>();
        /// <summary>
        /// ユーザ名指定
        /// </summary>
        public bool _Match_UserName = false;
        /// <summary>
        /// ユーザ名
        /// </summary>
        public List<string> _UserNames = new List<string>();
        /// <summary>
        /// 詳細指定
        /// </summary>
        public bool _Match_Detail = false;
        /// <summary>
        /// 詳細
        /// </summary>
        public List<string> _Details = new List<string>();
        /// <summary>
        /// ソフトウェア指定
        /// </summary>
        public bool _Match_Software = false;
        /// <summary>
        /// ソフトウェア名
        /// </summary>
        public List<string> _Software = new List<string>();
        /// <summary>
        /// チャンネル指定
        /// </summary>
        public bool _Match_Channel = false;
        /// <summary>
        /// チャンネル名
        /// </summary>
        public List<string> _ChannelNames = new List<string>();

        /// <summary>
        /// 一致した件数_開始
        /// </summary>
        public int _Matched_Count_Min = 0;
        /// <summary>
        /// 一致した件数_終了
        /// </summary>
        public int _Matched_Count_Max = 0;

        /// <summary>
        /// CW指定
        /// </summary>
        public bool _Match_CW = false;
        /// <summary>
        /// CWを含める
        /// </summary>
        public bool _Contain_CW = false;
        /// <summary>
        /// Reply指定
        /// </summary>
        public bool _Match_Reply = false;
        /// <summary>
        /// Replyを含める
        /// </summary>
        public bool _Contain_Reply = false;
        /// <summary>
        /// RN指定
        /// </summary>
        public bool _Match_RN = false;
        /// <summary>
        /// ReNoteを含める
        /// </summary>
        public bool _Contain_RN = false;

        private TimeLineContainer? _containerBacking;
        public TimeLineContainer? _Container
        {
            get => _containerBacking;
            set => _containerBacking = value;
        }

        public TimeLineFilterlingOption()
        {
        }

        public bool FilterResult()
        {
            bool Result = false;
            switch (_MODE)
            {
                case MATCH_MODE.ALL:
                    Result = MatchUserId() &&
                           MatchUserName() &&
                           MatchDetail() &&
                           MatchSoftware() &&
                           MatchChannel() &&
                           ContainCW() &&
                           ContainReply() &&
                           ContainRN();
                    break;
                case MATCH_MODE.PARTIAL:
                    Result = MatchUserId() ||
                           MatchUserName() ||
                           MatchDetail() ||
                           MatchSoftware() ||
                           MatchChannel() ||
                           ContainCW() ||
                           ContainReply() ||
                           ContainRN();
                    break;

                default:
                    Result = false;
                    break;
            }

            //System.Diagnostics.Debug.WriteLine("チェック結果：" + Result);

            return !CONSTRAINT_INVERT ? Result : !Result;
        }

        public bool MatchUserId()
        {
            if (_Container == null)
            {
                return true;
            }
            return ListMatch(_Match_UserId, _UserIds, _Container.USERID);
        }

        public bool MatchUserName()
        {
            if (_Container == null)
            {
                return true;
            }
            return ListMatch(_Match_UserName, _UserNames, _Container.USERNAME);
        }

        public bool MatchDetail()
        {
            if (_Container == null)
            {
                return true;
            }
            return ListMatch(_Match_Detail, _Details, _Container.DETAIL);
        }

        public bool MatchSoftware()
        {
            if (_Container == null)
            {
                return true;
            }
            return ListMatch(_Match_Software, _Software, _Container.SOFTWARE);
        }
        
        public bool MatchChannel()
        {
            if (_Container == null)
            {
                return true;
            }
            if (_Match_Channel)
            {
                if (_Container.CHANNEL_NAME == null)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
            return ListMatch(_Match_Channel, _ChannelNames, _Container.CHANNEL_NAME);
        }

        public bool ContainCW()
        {
            if (_Container == null)
            {
                return true;
            }
            if (!_Match_CW)
            {
                return true;
            }
            if (!_Contain_CW)
            {
                return true;
            }
            return _Container.CW;
        }

        public bool ContainReply()
        {
            if (_Container == null)
            {
                return true;
            }
            if (!_Match_Reply)
            {
                return true;
            }
            if (!_Contain_Reply)
            {
                return true;
            }
            return _Container.REPLAYED;
        }

        public bool ContainRN()
        {
            if (_Container == null)
            {
                return true;
            }
            if (!_Match_RN)
            {
                return true;
            }
            if (!_Contain_RN)
            {
                return true;
            }
            return _Container.RENOTED;
        }

        public bool ListMatch(bool AppliedMatch, List<string> Patterns, string Value)
        {
            if (AppliedMatch == false)
            {
                return true;
            }
            int MatchedCount = 0;
            switch(_PATTERN)
            {
                case MATCHER_PATTERN.NONE: // 未指定
                    return false;
                case MATCHER_PATTERN.MATCH: // 一致
                    MatchedCount = Patterns.FindAll(r => { return Value == r; }).Count;
                    break;
                case MATCHER_PATTERN.PATTERN: // 含む
                    MatchedCount = Patterns.FindAll(r => { return Value.Contains(r); }).Count;
                    break;
                case MATCHER_PATTERN.START:
                    MatchedCount = Patterns.FindAll(r => { return r.StartsWith(Value); }).Count;
                    break;
                case MATCHER_PATTERN.END:
                    MatchedCount = Patterns.FindAll(r => { return r.EndsWith(Value); }).Count;
                    break;
                case MATCHER_PATTERN.REGEXP:
                    MatchedCount = Patterns.FindAll(r => { return Regex.Matches(r, Value).Count > 0; }).Count;
                    break;

                default:
                    return false;
            }
            if (_Matched_Count_Max != 0 && _Matched_Count_Min != 0)
            {
                // 範囲
                return _Matched_Count_Min < MatchedCount && MatchedCount < _Matched_Count_Max;
            }
            else
            {
                if (_Matched_Count_Max != 0)
                {
                    // 最大が決まっている
                    return MatchedCount < _Matched_Count_Max;
                }
                else
                {
                    // 最小だけ
                    return MatchedCount > _Matched_Count_Min;
                }
            }
        }
    }

    /// <summary>
    /// タイムラインアラート設定
    /// </summary>
    internal class TimeLineAlertOption
    {
        public enum ALERT_TIMING
        {
            NONE = 0,
            ACCEPT,
            REJECT
        }

        public ALERT_TIMING _Alert_Timing = ALERT_TIMING.NONE;

        /// <summary>
        /// タイムラインフィルタリング設定
        /// </summary>
        public List<TimeLineFilterlingOption> _FilterOptions = new List<TimeLineFilterlingOption>();

        /// <summary>
        /// フィルタ一致モード
        /// 
        /// true=全部一致 false=いずれか一致
        /// </summary>
        public bool _FilterMode = true;

        /// <summary>
        /// アラート方法
        /// </summary>
        public enum ALERT_METHOD
        {
            NONE = 0,
            /// <summary>
            /// シェル実行
            /// </summary>
            SHELL,
            /// <summary>
            /// メール
            /// </summary>
            EMAIL,
            /// <summary>
            /// トースト
            /// </summary>
            TOAST,
            /// <summary>
            /// バルーン
            /// </summary>
            BALOON,
            /// <summary>
            /// HTTP request
            /// </summary>
            HTTP,
        }

        /// <summary>
        /// アラート方法設定
        /// </summary>
        public List<ALERT_METHOD> _AlertMethods = new List<ALERT_METHOD>();

        /// <summary>
        /// アラート処理本体
        /// </summary>
        public List<NotificationController> _AlertExecution = new List<NotificationController>();

        /// <summary>
        /// アラート実行
        /// </summary>
        /// <returns></returns>
        public void ExecuteAlert(TimeLineContainer Container)
        {
            try
            {
                int Found = this._FilterOptions.Count();
                int Filted = this._FilterOptions.FindAll(r => { return r.FilterResult(); }).Count();

                bool CountRet = false;
                if (this._FilterMode)
                {
                    CountRet = Found == Filted;
                }
                else
                {
                    CountRet = Found > 0;
                }
                if (CountRet)
                {
                    foreach (var Alert in this._AlertExecution)
                    {
                        Alert.SetTimeLineContainer(Container);
                        Alert.Execute();
                    }
                }
            }
            catch (Exception ce)
            {
                System.Diagnostics.Debug.WriteLine($"Alert: {ce.Message}");
            }
        }
    }
}
