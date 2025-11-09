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
    public partial class TimeLineAlertNotificationCreateForm : Form
    {
        public TimeLineAlertNotificationCreateForm()
        {
            InitializeComponent();

            this.cmbNotificationKind.Items.Clear();
            this.cmbNotificationKind.Items.Add(new NotificationControllerCombo(NotificationController.CONTROLLER_KIND.Baloon));
            this.cmbNotificationKind.Items.Add(new NotificationControllerCombo(NotificationController.CONTROLLER_KIND.HttpRequest));
            this.cmbNotificationKind.Items.Add(new NotificationControllerCombo(NotificationController.CONTROLLER_KIND.Mail));
            this.cmbNotificationKind.Items.Add(new NotificationControllerCombo(NotificationController.CONTROLLER_KIND.Shell));
            this.cmbNotificationKind.Items.Add(new NotificationControllerCombo(NotificationController.CONTROLLER_KIND.Toast));
            this.cmbNotificationKind.Items.Add(new NotificationControllerCombo(NotificationController.CONTROLLER_KIND.NotificationSound));
        }

        private NotificationController _n { get; set; }
        public NotificationController NotificationControl { get { return this._n; } }
        /// <summary>
        /// 作成ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdCreateNotificationKind_Click(object sender, EventArgs e)
        {
            var Controller = this.cmbNotificationKind.SelectedItem;
            if (Controller == null)
            {
                return;
            }
            if (Controller?.GetType() != typeof(NotificationControllerCombo))
            {
                return;
            }
            NotificationController Cnt = NotificationController.Create(((NotificationControllerCombo)Controller).Kind);
            this._n = Cnt;

            this.Close();
        }
    }
}
