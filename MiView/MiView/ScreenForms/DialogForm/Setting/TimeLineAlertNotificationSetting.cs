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
        private NotificationController? _CurrentController;

        public TimeLineAlertNotificationSetting()
        {
            InitializeComponent();
        }

        public void SetNotificationData(NotificationController Controller)
        {
            _CurrentController = Controller;

            this.label2.Text = Controller.ControllerKindToString;
            var tmCtl = Controller.GetControllerForm();
            tmCtl.Location = new Point(this.lblNotificationMethod.Location.X, this.lblNotificationMethod.Location.Y + this.lblNotificationMethod.Size.Height + 10);
            this.Controls.Add(tmCtl);
        }
    }
}
