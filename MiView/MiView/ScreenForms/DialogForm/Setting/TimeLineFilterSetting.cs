using MiView.Common.Connection.WebSocket;
using MiView.Common.TimeLine;
using MiView.Common.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static MiView.Common.TimeLine.TimeLineFilterlingOption;

namespace MiView.ScreenForms.DialogForm.Setting
{
    public partial class TimeLineFilterSetting : Form
    {
        //public WebSocketManager? _WSManager { get; set; }
        //public Dictionary<string, string>? _TmpTLNames = new Dictionary<string, string>();
        //public Dictionary<string, DataGridTimeLine>? _TLGrid = new Dictionary<string, DataGridTimeLine>();
        public DataGridTimeLine? _TimeLine = null;
        private const string CtlMatchField_Prefix = "_Match_";
        private const string CtlPatternField_Prefix = "_Pattern_";
        private const string CtlField_Suffix = "_Name";
        private static string[] CtlName_Basics = {  "UserId",
                                                     "UserName",
                                                     "Detail",
                                                     "Software",
                                                     "ChannelName"};

        public TimeLineFilterSetting()
        {
            InitializeComponent();
            this.SetFilterDisp += SetFilterValues;
            this.SaveFilter += SaveFilterValues;
            this.DeleteFilter += DeleteFilterValues;
            this.FinishEdit += FinishEditValues;

            // 一致方法
            this.cmbMatchMode.Items.Clear();
            var DefaultMatchCmb = new TimeLineMatchCombo(TimeLineFilterlingOption.MATCH_MODE.NONE);
            this.cmbMatchMode.Items.Add(DefaultMatchCmb);
            this.cmbMatchMode.Items.Add(new TimeLineMatchCombo(TimeLineFilterlingOption.MATCH_MODE.ALL));
            this.cmbMatchMode.Items.Add(new TimeLineMatchCombo(TimeLineFilterlingOption.MATCH_MODE.PARTIAL));
            this.cmbMatchMode.Items.Add(new TimeLineMatchCombo(TimeLineFilterlingOption.MATCH_MODE.IMPORTANCE));
            this.cmbMatchMode.SelectedIndexChanged += ttt;

            this.pnFilter.Enabled = false;

            CreateFilterSettingControl();
        }
        /// <summary>
        /// コントロール配置
        /// </summary>
        private void CreateFilterSettingControl()
        {
            int StartYPos = 0; // 開始場所
            int MarginYPos = 5; // 余白

            System.Windows.Forms.Label CtlLabel; // 見出し
            System.Windows.Forms.CheckBox CtlCheck;
            System.Windows.Forms.ComboBox CtlPattern; // 一致方法
            System.Windows.Forms.Control CtlInput; // 入力コントロール

            // 開始 + 余白
            StartYPos = chkConstraintInvert.Location.Y + chkConstraintInvert.Size.Height + MarginYPos;

            TimeLinePatternCombo[] CtlPatterns = {
                new TimeLinePatternCombo(TimeLineFilterlingOption.MATCHER_PATTERN.NONE),
                new TimeLinePatternCombo(TimeLineFilterlingOption.MATCHER_PATTERN.MATCH),
                new TimeLinePatternCombo(TimeLineFilterlingOption.MATCHER_PATTERN.PATTERN),
                new TimeLinePatternCombo(TimeLineFilterlingOption.MATCHER_PATTERN.START),
                new TimeLinePatternCombo(TimeLineFilterlingOption.MATCHER_PATTERN.END),
                new TimeLinePatternCombo(TimeLineFilterlingOption.MATCHER_PATTERN.REGEXP)
            };
            string CtlCheckBasic;

            List<System.Windows.Forms.Label?> CtlLabels;
            List<System.Windows.Forms.CheckBox> CtlChecks;
            List<System.Windows.Forms.TextBox> CtlTexts;
            List<System.Windows.Forms.ComboBox> CtlCombos;

            int RightPos = 0;

            // 入力系項目
            // 見出しラベル
            CtlLabels = CtlName_Basics.Select(sBasic =>
            {
                var ct = ClassUtil.GetConstValue(typeof(TimeLineFilterlingOption), CtlMatchField_Prefix + sBasic + CtlField_Suffix);
                if (ct == null)
                {
                    return null;
                }
                return new System.Windows.Forms.Label()
                {
                    Text = ct.ToString(),
                    Name = "lbl" + sBasic,
                    Location = new Point(0, StartYPos + (CtlName_Basics.ToList().IndexOf(sBasic) * 30)),
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSize = true,
                };
            }).ToList();
            CtlLabels.ForEach(r => { this.pnFilter.Controls.Add(r); });
            RightPos = CtlLabels.Max(r => { return r.Location.X + r.Size.Width; }) + 5;

            // 有効かどうかのチェック
            CtlChecks = CtlName_Basics.Select(sBasic =>
            {
                return new System.Windows.Forms.CheckBox()
                {
                    Text = "有効にする",
                    Name = "chk" + CtlMatchField_Prefix + sBasic + CtlField_Suffix,
                    Location = new Point(RightPos, StartYPos + (CtlName_Basics.ToList().IndexOf(sBasic) * 30)),
                    Checked = false
                };
            }).ToList();
            CtlChecks.ForEach(r => { this.pnFilter.Controls.Add(r); });
            RightPos = CtlChecks.Max(r => { return r.Location.X + r.Size.Width; }) + 5;

            // 入力欄
            CtlTexts = CtlName_Basics.Select(sBasic =>
            {
                return new System.Windows.Forms.TextBox()
                {
                    Name = "txt" + CtlMatchField_Prefix + sBasic + CtlField_Suffix,
                    Location = new Point(RightPos, StartYPos + (CtlName_Basics.ToList().IndexOf(sBasic) * 30)),
                    MaxLength = 200,
                    Width = 50 * 7 + 7
                };
            }).ToList();
            CtlTexts.ForEach(r => { this.pnFilter.Controls.Add(r); });
            RightPos = CtlTexts.Max(r => { return r.Location.X + r.Size.Width; }) + 5;

            // パターン
            CtlCombos = CtlName_Basics.Select(sBasic =>
            {
                var Cmb = new System.Windows.Forms.ComboBox();
                Cmb.Items.AddRange(CtlPatterns);
                Cmb.Name = "cmb" + CtlMatchField_Prefix + sBasic + CtlField_Suffix;
                Cmb.Location = new Point(RightPos, StartYPos + (CtlName_Basics.ToList().IndexOf(sBasic) * 30));
                Cmb.DropDownStyle = ComboBoxStyle.DropDownList;
                return Cmb;
            }).ToList();
            CtlCombos.ForEach(r => { this.pnFilter.Controls.Add(r); });
            RightPos = CtlCombos.Max(r => { return r.Location.X + r.Size.Width; }) + 5;

        }

        private void ttt(object? sender, EventArgs ag)
        {
            var tm = this.cmbMatchMode.SelectedItem;
        }

        public void SetTimeLineData(DataGridTimeLine TimeLine)
        {
            this._TimeLine = TimeLine;
            this.TimeLineFilterSetting_Load(null, new EventArgs());
        }

        private TimeLineFilterlingOption? _CurrentFilter = null;

        private void TimeLineFilterSetting_Load(object? sender, EventArgs e)
        {
            this.cmbTimeLineSelect.Enabled = this._TimeLine?._FilteringOptions.Count > 0;
            this.cmbTimeLineSelect.Items.Clear();
            this.cmbTimeLineSelect.Items.AddRange(this._TimeLine._FilteringOptions.ToList().Select(r => { return new FilterCombo(r.FilterDefinition, r); }).ToArray());

            // ボタンの状態設定
            this.cmdCreateFilter.Enabled = true;
            this.cmdCopyFilter.Enabled = this._TimeLine._FilteringOptions.Count > 0;
            this.cmdLoadFilter.Enabled = this._TimeLine._FilteringOptions.Count > 0;
            this.pnFilter.Enabled = false;
        }

        private EventHandler SetFilterDisp;
        /// <summary>
        /// フィルタ読み込みイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetFilterValues(object? sender, EventArgs e)
        {
            // 読み込み失敗時など不正な時
            if (_CurrentFilter == null)
            {
                this.SetFilterProperty(null);
                return;
            }
            this.txtFilterDefinition.Text = _CurrentFilter.FilterDefinition;
            this.txtFilterName.Text = _CurrentFilter.FilterName;
            this.cmbMatchMode.SelectedItem = new TimeLineMatchCombo(_CurrentFilter._MODE);
            this.SetFilterProperty(_CurrentFilter);
            this.pnFilter.Enabled = true;

            this.cmdCreateFilter.Enabled = false;
            this.cmdCopyFilter.Enabled = false;
            this.cmdLoadFilter.Enabled = false;
        }

        private void SetFilterProperty(TimeLineFilterlingOption? FilterOption = null)
        {
            foreach (string CtlName in CtlName_Basics)
            {
                if (FilterOption != null)
                {
                    // フィルタ設定がある
                    try
                    {
                        ((System.Windows.Forms.CheckBox)this.pnFilter.Controls.Find("chk" + CtlMatchField_Prefix + CtlName + CtlField_Suffix, false)[0]).Checked
                            = (bool?)typeof(TimeLineFilterlingOption).GetProperty(CtlMatchField_Prefix + CtlName)?.GetValue(FilterOption) ?? false;
                        ((System.Windows.Forms.TextBox)this.pnFilter.Controls.Find("txt" + CtlMatchField_Prefix + CtlName + CtlField_Suffix, false)[0]).Text
                            = string.Join(',', ((List<string>?)typeof(TimeLineFilterlingOption).GetProperty("_" + CtlName)?.GetValue(FilterOption) ?? []));
                        ((System.Windows.Forms.ComboBox)this.pnFilter.Controls.Find("cmb" + CtlMatchField_Prefix + CtlName + CtlField_Suffix, false)[0]).SelectedItem
                            = new TimeLinePatternCombo((MATCHER_PATTERN?)typeof(TimeLineFilterlingOption).GetProperty(CtlPatternField_Prefix + CtlName)?.GetValue(FilterOption) ?? MATCHER_PATTERN.NONE);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    // フィルタ設定がない
                    ((System.Windows.Forms.CheckBox)this.pnFilter.Controls.Find("chk" + CtlMatchField_Prefix + CtlName + CtlField_Suffix, false)[0]).Checked = false;
                    ((System.Windows.Forms.TextBox)this.pnFilter.Controls.Find("txt" + CtlMatchField_Prefix + CtlName + CtlField_Suffix, false)[0]).Text = string.Empty;
                    ((System.Windows.Forms.ComboBox)this.pnFilter.Controls.Find("cmb" + CtlMatchField_Prefix + CtlName + CtlField_Suffix, false)[0]).SelectedItem = new TimeLineMatchCombo(TimeLineFilterlingOption.MATCH_MODE.NONE);
                }
            }
        }
        private EventHandler SaveFilter;
        /// <summary>
        /// フィルタ保存イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFilterValues(object? sender, EventArgs e)
        {
            // 読み込み失敗時など不正な時
            if (_CurrentFilter == null ||
                this._TimeLine == null ||
                this._TimeLine._FilteringOptions == null)
            {
                return;
            }
            _CurrentFilter.FilterDefinition = this.txtFilterDefinition.Text;
            _CurrentFilter.FilterName = this.txtFilterName.Text;
            if (this.cmbMatchMode.SelectedItem != null)
                _CurrentFilter._MODE = ((TimeLineMatchCombo)this.cmbMatchMode.SelectedItem).MatchMode;
            this.SaveFilterProperty(ref _CurrentFilter);
            this.pnFilter.Enabled = false;

            if (this._TimeLine._FilteringOptions != null &&
                this._TimeLine._FilteringOptions.Contains(_CurrentFilter) &&
                this._TimeLine._FilteringOptions.FindAll(r => { return r.FilterDefinition == _CurrentFilter.FilterDefinition; }).Count > 0)
            {
                this._TimeLine._FilteringOptions[this._TimeLine._FilteringOptions.IndexOf(_CurrentFilter)] = _CurrentFilter;
            }
            else
            {
                this._TimeLine._FilteringOptions.Add(_CurrentFilter);
            }
            this.FinishEditValues(sender, e);
        }

        private void SaveFilterProperty(ref TimeLineFilterlingOption FilterOption)
        {
            foreach (string CtlName in CtlName_Basics)
            {
                try
                {
                    typeof(TimeLineFilterlingOption).GetProperty(CtlMatchField_Prefix + CtlName)?.SetValue(FilterOption, ((System.Windows.Forms.CheckBox)this.pnFilter.Controls.Find("chk" + CtlMatchField_Prefix + CtlName + CtlField_Suffix, false)[0]).Checked);
                    // カンマ区切りはちょっと微妙なので対策考える
                    typeof(TimeLineFilterlingOption).GetProperty("_" + CtlName)?.SetValue(FilterOption, ((System.Windows.Forms.TextBox)this.pnFilter.Controls.Find("txt" + CtlMatchField_Prefix + CtlName + CtlField_Suffix, false)[0]).Text.Split(',').ToList());
                    typeof(TimeLineFilterlingOption).GetProperty(CtlPatternField_Prefix + CtlName)?.SetValue(FilterOption, ((TimeLinePatternCombo)((System.Windows.Forms.ComboBox)this.pnFilter.Controls.Find("cmb" + CtlMatchField_Prefix + CtlName + CtlField_Suffix, false)[0]).SelectedItem ?? new TimeLinePatternCombo(TimeLineFilterlingOption.MATCHER_PATTERN.NONE)).MATCHER_PATTERN);
                }
                catch
                {
                }
            }
        }

        private EventHandler DeleteFilter;
        /// <summary>
        /// フィルタ保存イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFilterValues(object? sender, EventArgs e)
        {
            // 読み込み失敗時など不正な時
            if (_CurrentFilter == null ||
                this._TimeLine == null ||
                this._TimeLine._FilteringOptions == null)
            {
                return;
            }
            this.pnFilter.Enabled = false;
            if (this._TimeLine._FilteringOptions.Contains(_CurrentFilter))
            {
                this._TimeLine._FilteringOptions.RemoveAt(this._TimeLine._FilteringOptions.IndexOf(_CurrentFilter));
            }
            this.FinishEditValues(sender, e);
        }
        private EventHandler FinishEdit;
        /// <summary>
        /// フィルタ保存終了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinishEditValues(object? sender, EventArgs e)
        {
            this._CurrentFilter = null;
            this.TimeLineFilterSetting_Load(null, new EventArgs());
        }

        /// <summary>
        /// 新規作成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdCreateFilter_Click(object sender, EventArgs e)
        {
            _CurrentFilter = new TimeLineFilterlingOption();
            this.SetFilterDisp(null, new EventArgs());
        }

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdLoadFilter_Click(object sender, EventArgs e)
        {
            if (this.cmbTimeLineSelect == null ||
                this.cmbTimeLineSelect.SelectedItem == null)
            {
                return;
            }
            _CurrentFilter = ((FilterCombo)this.cmbTimeLineSelect.SelectedItem).FilterOption;
            this.SetFilterDisp(null, new EventArgs());
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdDeleteFilter_Click(object sender, EventArgs e)
        {
            this.DeleteFilter(null, new EventArgs());
        }

        /// <summary>
        /// コピー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdCopyFilter_Click(object sender, EventArgs e)
        {
            if (this.cmbTimeLineSelect == null ||
                this.cmbTimeLineSelect.SelectedItem == null)
            {
                return;
            }
            _CurrentFilter = ((FilterCombo)this.cmbTimeLineSelect.SelectedItem).FilterOption;
            _CurrentFilter = JsonSerializer.Deserialize<TimeLineFilterlingOption>(JsonSerializer.SerializeToUtf8Bytes<TimeLineFilterlingOption>(_CurrentFilter));
            if (_CurrentFilter == null)
            {
                // たぶんありえない
                return;
            }
            _CurrentFilter.SetNewDefinition();
            this.SetFilterDisp(null, new EventArgs());
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSaveFilter_Click(object sender, EventArgs e)
        {
            this.SaveFilter(null, new EventArgs());
        }
        /// <summary>
        /// 編集取り消し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdRevertFilter_Click(object sender, EventArgs e)
        {
            this.FinishEdit(null, new EventArgs());
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

    public class FilterCombo
    {
        public string FilterDefinition { get; set; }
        public TimeLineFilterlingOption FilterOption { get; set; }
        public FilterCombo(string filterDefinition, TimeLineFilterlingOption filterOption)
        {
            FilterDefinition = filterDefinition;
            FilterOption = filterOption;
        }

        public override string ToString()
        {
            return FilterOption.FilterName;
        }
    }

    public class TimeLineMatchCombo
    {
        public TimeLineFilterlingOption.MATCH_MODE MatchMode { get; set; }
        public TimeLineMatchCombo(TimeLineFilterlingOption.MATCH_MODE matchMode)
        {
            MatchMode = matchMode;
        }

        public override string? ToString()
        {
            return TimeLineFilterlingOption.DispMatchMode(MatchMode);
        }
    }
    public class TimeLinePatternCombo
    {
        public TimeLineFilterlingOption.MATCHER_PATTERN MATCHER_PATTERN { get; set; }
        public TimeLinePatternCombo(TimeLineFilterlingOption.MATCHER_PATTERN mATCHER_PATTERN)
        {
            MATCHER_PATTERN = mATCHER_PATTERN;
        }
        public override string? ToString()
        {
            return TimeLineFilterlingOption.DispMatcherPattern(MATCHER_PATTERN);
        }
    }
}
