using MiView.Common.Notification;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiView.ScreenForms.DialogForm.Setting
{
    public partial class TimeLineAlertSetting : Form
    {
        private TimeLineAlertNotificationSetting _NotificationForm = TimeLineAlertNotificationSetting.Instance;
        public DataGridTimeLine? _TimeLine = null;

        public TimeLineAlertSetting()
        {
            InitializeComponent();
            this.SetFilterDisp += SetFilterValues;
            this.SaveAlert += SaveAlertValues;
            this.DeleteAlert += DeleteFilterValues;
            this.FinishEdit += FinishEditValues;

            // タイミング
            this.cmbAlertTiming.Items.Clear();
            this.cmbAlertTiming.Items.Add(new TimeLineAlertTimingCombo(TimeLineAlertOption.ALERT_TIMING.NONE));
            this.cmbAlertTiming.Items.Add(new TimeLineAlertTimingCombo(TimeLineAlertOption.ALERT_TIMING.ON_TIMELINE));
            this.cmbAlertTiming.Items.Add(new TimeLineAlertTimingCombo(TimeLineAlertOption.ALERT_TIMING.REJECT));
            this.cmbAlertTiming.Items.Add(new TimeLineAlertTimingCombo(TimeLineAlertOption.ALERT_TIMING.ACCEPT));

            this.pnAlert.Enabled = false;
        }

        public void SetAlertData(DataGridTimeLine TimeLine)
        {
            this._TimeLine = TimeLine;
            this.TimeLineAlertSetting_Load(null, new EventArgs());
        }

        private TimeLineAlertOption? _CurrentAlert = null;
        private void TimeLineAlertSetting_Load(object? sender, EventArgs e)
        {
            this.cmbAlertSelect.Enabled = this._TimeLine?._AlertOptions.Count > 0;
            this.cmbAlertSelect.Items.Clear();
            this.cmbAlertSelect.Items.AddRange(this._TimeLine._AlertOptions.ToList().Select(r => { return new AlertCombo(r.AlertDefinition, r); }).ToArray());

            this.lstNotification.Items.Clear();
            //this.lstNotification.Items.Clear();
            //this.lstNotification.Items.AddRange(this._TimeLine._AlertOptions[0]._AlertExecution.ToList().Select(r => { return new TimeLineAlertNotificationList(r); }).ToArray());

            // ボタンの状態設定
            this.cmdCreateAlert.Enabled = true;
            this.cmdCopyAlert.Enabled = this._TimeLine._AlertOptions.Count > 0;
            this.cmdLoadAlert.Enabled = this._TimeLine._AlertOptions.Count > 0;
            this.pnAlert.Enabled = false;
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
            if (_CurrentAlert == null)
            {
                //this.SetFilterProperty(null);
                return;
            }
            this.txtAlertDefinition.Text = _CurrentAlert.AlertDefinition;
            this.txtAlertName.Text = _CurrentAlert.AlertName;
            this.cmbAlertTiming.SelectedItem = this.cmbAlertTiming.Items
                                                 .Cast<TimeLineAlertTimingCombo>()
                                                 .ToList()
                                                 .Find(r => { return r.Timing == _CurrentAlert._Alert_Timing; });

            this.lstNotification.Items.Clear();
            this.lstNotification.Items.AddRange(_CurrentAlert._AlertExecution.ToList().Select(r => { return new TimeLineAlertNotificationList(r); }).ToArray());
            //this.SetFilterProperty(_CurrentFilter);
            this.pnAlert.Enabled = true;

            this.cmdCreateAlert.Enabled = false;
            this.cmdCopyAlert.Enabled = false;
            this.cmdLoadAlert.Enabled = false;
        }
        private EventHandler SaveAlert;
        /// <summary>
        /// フィルタ保存イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAlertValues(object? sender, EventArgs e)
        {
            // 読み込み失敗時など不正な時
            if (_CurrentAlert == null ||
                this._TimeLine == null ||
                this._TimeLine._AlertOptions == null)
            {
                return;
            }
            _CurrentAlert.AlertDefinition = this.txtAlertDefinition.Text;
            _CurrentAlert.AlertName = this.txtAlertName.Text;
            if (this.cmbAlertTiming.SelectedItem != null)
                _CurrentAlert._Alert_Timing = ((TimeLineAlertTimingCombo)this.cmbAlertTiming.SelectedItem).Timing;
            //this.SaveFilterProperty(ref _CurrentAlert);
            this.pnAlert.Enabled = false;

            if (this._TimeLine._AlertOptions != null &&
                this._TimeLine._AlertOptions.Contains(_CurrentAlert) &&
                this._TimeLine._AlertOptions.FindAll(r => { return r.AlertDefinition == _CurrentAlert.AlertDefinition; }).Count > 0)
            {
                this._TimeLine._AlertOptions[this._TimeLine._AlertOptions.IndexOf(_CurrentAlert)] = _CurrentAlert;
            }
            else
            {
                this._TimeLine._AlertOptions.Add(_CurrentAlert);
            }
            this.FinishEditValues(sender, e);
        }

        private EventHandler DeleteAlert;
        /// <summary>
        /// フィルタ保存イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFilterValues(object? sender, EventArgs e)
        {
            // 読み込み失敗時など不正な時
            if (_CurrentAlert == null ||
                this._TimeLine == null ||
                this._TimeLine._AlertOptions == null)
            {
                return;
            }
            this.pnAlert.Enabled = false;
            if (this._TimeLine._AlertOptions.Contains(_CurrentAlert))
            {
                this._TimeLine._AlertOptions.RemoveAt(this._TimeLine._AlertOptions.IndexOf(_CurrentAlert));
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
            this._CurrentAlert = null;
            this.TimeLineAlertSetting_Load(null, new EventArgs());
        }

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdLoadAlert_Click(object sender, EventArgs e)
        {
            if (this.cmbAlertSelect == null ||
                this.cmbAlertSelect.SelectedItem == null)
            {
                return;
            }
            _CurrentAlert = ((AlertCombo)this.cmbAlertSelect.SelectedItem).AlertOption;
            this.SetFilterDisp(null, new EventArgs());
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdDeleteAlert_Click(object sender, EventArgs e)
        {
            this.DeleteAlert(null, new EventArgs());
        }

        /// <summary>
        /// コピー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdCopyAlert_Click(object sender, EventArgs e)
        {
            if (this.cmbAlertSelect == null ||
                this.cmbAlertSelect.SelectedItem == null)
            {
                return;
            }
            _CurrentAlert = ((AlertCombo)this.cmbAlertSelect.SelectedItem).AlertOption;
            _CurrentAlert = JsonSerializer.Deserialize<TimeLineAlertOption>(JsonSerializer.SerializeToUtf8Bytes<TimeLineAlertOption>(_CurrentAlert));
            if (_CurrentAlert == null)
            {
                // たぶんありえない
                return;
            }
            _CurrentAlert.SetNewDefinition();
            this.SetFilterDisp(null, new EventArgs());
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSaveAlert_Click(object sender, EventArgs e)
        {
            this.SaveAlert(null, new EventArgs());
        }

        /// <summary>
        /// 編集取り消し
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdRevertAlert_Click(object sender, EventArgs e)
        {
            this.FinishEdit(null, new EventArgs());
        }

        /// <summary>
        /// 新規作成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdCreateAlert_Click(object sender, EventArgs e)
        {
            _CurrentAlert = new TimeLineAlertOption();
            this.SetFilterDisp(null, new EventArgs());
        }

        /// <summary>
        /// アラート方法追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdAddAlertNotification_Click(object sender, EventArgs e)
        {
            if (_CurrentAlert == null)
            {
                return;
            }
            TimeLineAlertNotificationCreateForm dlg = new TimeLineAlertNotificationCreateForm();
            dlg.ShowDialog();
            if (dlg.NotificationControl == null)
            {
                return;
            }
            this._NotificationForm.SetNotificationData(dlg.NotificationControl);
            this._NotificationForm.ShowDialog();
            if (this._NotificationForm.CurrentController == null)
            {
                return;
            }
            if (this._NotificationForm.DialogResult != DialogResult.OK)
            {
                return;
            }
            // 追加
            _CurrentAlert._AlertExecution.Add(this._NotificationForm.CurrentController);
            this.lstNotification.Items.Add(new TimeLineAlertNotificationList(this._NotificationForm.CurrentController));
        }

        /// <summary>
        /// アラート方法削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdDeleteAlertNotification_Click(object sender, EventArgs e)
        {
            if (_CurrentAlert == null)
            {
                return;
            }
            this._NotificationForm.ShowDialog();
            if (this._NotificationForm.CurrentController == null)
            {
                return;
            }
            if (_CurrentAlert._AlertExecution.IndexOf(this._NotificationForm.CurrentController) == -1)
            {
                return;
            }
            if (this._NotificationForm.DialogResult != DialogResult.OK)
            {
                return;
            }
            // 上書き
            // _CurrentAlert._AlertExecution[_CurrentAlert._AlertExecution.IndexOf(this._NotificationForm.CurrentController)] = this._NotificationForm.CurrentController;
        }

        /// <summary>
        /// アラート方法編集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdEditAlertNotification_Click(object sender, EventArgs e)
        {
            if (_CurrentAlert == null)
            {
                return;
            }
            TimeLineAlertNotificationList? NotificationControl = (TimeLineAlertNotificationList)this.lstNotification.SelectedItem;
            if (NotificationControl == null)
            {
                return;
            }
            this._NotificationForm.SetNotificationData(NotificationControl.Controller);
            this._NotificationForm.ShowDialog();
            if (this._NotificationForm.CurrentController == null)
            {
                return;
            }
            if (this._NotificationForm.DialogResult != DialogResult.OK)
            {
                return;
            }
            // 上書き
            NotificationControl.Controller = this._NotificationForm.CurrentController;
            try
            {
                _CurrentAlert._AlertExecution[_CurrentAlert._AlertExecution.IndexOf(this._NotificationForm.CurrentController)] = this._NotificationForm.CurrentController;
                this.lstNotification.Items[this.lstNotification.Items.IndexOf(NotificationControl)] = NotificationControl;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }

    public class AlertCombo
    {
        public string AlertDefinition { get; set; }
        public TimeLineAlertOption AlertOption { get; set; }
        public AlertCombo(string alertDefinition, TimeLineAlertOption alertOption)
        {
            this.AlertDefinition = alertDefinition;
            this.AlertOption = alertOption;
        }

        public override string ToString()
        {
            return AlertOption.AlertName;
        }
    }

    public class TimeLineAlertTimingCombo
    {
        public TimeLineAlertOption.ALERT_TIMING Timing;

        public TimeLineAlertTimingCombo(TimeLineAlertOption.ALERT_TIMING Timing)
        {
            this.Timing = Timing;
        }
        public override string ToString()
        {
            return TimeLineAlertOption.TimingName[Timing];
        }
    }

    public class NotificationControllerCombo
    {
        public NotificationController.CONTROLLER_KIND Kind;

        public NotificationControllerCombo(NotificationController.CONTROLLER_KIND Kind)
        {
            this.Kind = Kind;
        }
        public override string ToString()
        {
            return NotificationController.ControllerKindName[Kind];
        }
    }

    public class TimeLineAlertNotificationList
    {
        /// <summary>
        /// 通知コントローラ
        /// </summary>
        public NotificationController Controller;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="controller"></param>
        public TimeLineAlertNotificationList(NotificationController controller)
        {
            this.Controller = controller;
        }
        /// <summary>
        /// リスト表示用文字列
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Controller.ToString();
        }
    }
}
