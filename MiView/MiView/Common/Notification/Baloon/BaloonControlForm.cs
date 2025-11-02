using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Notification.Baloon
{
    public partial class BaloonControlForm : NotificationControlForm
    {
        public BaloonControlForm()
        {
            base.Initialize();

            int PosY = 0;
            int PosX = 0;

            PosX = _MarginX;
            PosY = 0;
            this.Controls.Add(CreateLabel("lblTitle", "タイトル", ref PosY, ref PosX));
            this.Controls.Add(CreateLabel("lblContent", "本文", ref PosY, ref PosX));

            PosX = _MarginX;
            PosY = 0;
            this.Controls.Add(CreateTextBox("txtTitle", "", ref PosY, ref PosX));
            this.Controls.Add(CreateTextBox("txtContent", "", ref PosY, ref PosX));

            this.SetTextBoxLength((TextBox)this._CreatedControls["txtTitle"], 15);
            this.SetTextBoxLength((TextBox)this._CreatedControls["txtContent"], 25);
            this.SetTextBoxHeight((TextBox)this._CreatedControls["txtContent"], 10);
        }

        public override void LoadDataToControl(NotificationController Controller)
        {
            ((TextBox)this._CreatedControls["txtTitle"]).Text = ((BaloonController)Controller).BaloonTitle;
            ((TextBox)this._CreatedControls["txtContent"]).Text = ((BaloonController)Controller).BaloonContent;
        }

        public override NotificationController SaveDataToControl(NotificationController Controller)
        {
            ((BaloonController)Controller).BaloonTitle = ((TextBox)this._CreatedControls["txtTitle"]).Text;
            ((BaloonController)Controller).BaloonContent = ((TextBox)this._CreatedControls["txtContent"]).Text;

            return Controller;
        }
    }
}
