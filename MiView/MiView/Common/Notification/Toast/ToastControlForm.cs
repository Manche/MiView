using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Notification.Toast
{
    public partial class ToastControlForm : NotificationControlForm
    {
        public ToastControlForm()
        {
            base.Initialize();
        }

        public override void LoadDataToControl(NotificationController Controller)
        {
        }

        public override NotificationController SaveDataToControl(NotificationController Controller)
        {
            return Controller;
        }
    }
}
