using MiView.Common.Notification;
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
    public partial class TimeLineAlertNotificationSetting : Form
    {
        public static TimeLineAlertNotificationSetting Instance { get; } = new TimeLineAlertNotificationSetting();

        public NotificationController? CurrentController { get { return _CurrentController; } }
        private NotificationController? _CurrentController;
        private Button _SaveBtn = new Button();

        public TimeLineAlertNotificationSetting()
        {
            InitializeComponent();
            this.AutoSize = true;

            _SaveBtn.Name = "cmdSave";
            _SaveBtn.Text = "保存";
            _SaveBtn.Location = new Point(0, 0);
            _SaveBtn.Click += SaveNotificationData;

            this.Controls.Add(_SaveBtn);
        }

        private NotificationControlForm? NotificationInput;
        public void SetNotificationData(NotificationController Controller)
        {
            this.DialogResult = DialogResult.None;

            _CurrentController = Controller;
            this.Controls.Remove(NotificationInput);

            this.label2.Text = Controller.ControllerKindToString();
            NotificationInput = Controller.GetControllerForm();
            NotificationInput.Location = new Point(this.lblNotificationMethod.Location.X, this.lblNotificationMethod.Location.Y + this.lblNotificationMethod.Size.Height + 20);
            this.Controls.Add(NotificationInput);

            this._SaveBtn.Location = new Point(NotificationInput.Location.X, NotificationInput.Location.Y + NotificationInput.Size.Height + 20);

            LoadNotificationData();
        }

        private void LoadNotificationData()
        {
            if (NotificationInput == null || _CurrentController == null)
            {
                return;
            }

            NotificationInput.LoadDataToControl(_CurrentController);
        }

        private void SaveNotificationData(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            if (NotificationInput == null || _CurrentController == null)
            {
                return;
            }

            _CurrentController = NotificationInput.SaveDataToControl(_CurrentController);
        }
    }
}
