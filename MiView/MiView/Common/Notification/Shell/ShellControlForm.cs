using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Notification.Shell
{
    public partial class ShellControlForm : NotificationControlForm
    {
        public ShellControlForm()
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
