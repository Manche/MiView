using MiView.Common.Notification.Baloon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MiView.Common.Notification.Sound
{
    public partial class NotificationSoundControlForm : NotificationControlForm
    {
        public NotificationSoundControlForm()
        {
            base.Initialize();

            int PosY = 0;
            int PosX = 0;
            int TmPos = 0;

            PosX = _MarginX;
            PosY = 0;
            this.Controls.Add(CreateLabel("lblFilepath", "ファイルパス", ref PosY, ref PosX));
            this.Controls.Add(CreateLabel("lblVolume", "音量", ref PosY, ref PosX));
            this.Controls.Add(CreateLabel("lblPlayTimes", "回数", ref PosY, ref PosX));
            this.Controls.Add(CreateLabel("lblDistance", "再生間隔", ref PosY, ref PosX));
            this.Controls.Add(CreateLabel("lblListen", "視聴", ref PosY, ref PosX));


            PosX = _MarginX;
            PosY = 0;
            this.Controls.Add(CreateTextBox("txtFilepath", "", ref PosY, ref PosX));
            this.Controls.Add(CreateNumberBox("numVolume", 100, ref PosY, ref PosX));
            this.Controls.Add(CreateNumberBox("numPlayTimes", 1, ref PosY, ref PosX));
            this.Controls.Add(CreateNumberBox("numDistance", 1000, ref PosY, ref PosX, 0, 60000));
            this.Controls.Add(CreateButton("btnListen", "視聴", ref PosY, ref PosX));

            try
            {
                this.SetTextBoxLength((TextBox)this._CreatedControls["txtFilepath"], 32);
                this.SetTextBoxLength((NumericUpDown)this._CreatedControls["numVolume"], 5);
                this.SetTextBoxLength((NumericUpDown)this._CreatedControls["numPlayTimes"], 5);
                this.SetTextBoxLength((NumericUpDown)this._CreatedControls["numDistance"], 5);
            }
            catch
            {
            }
            if (this._CreatedControls["txtFilepath"].Size.Width + this._CreatedControls["txtFilepath"].Location.X > _MarginX)
            {
                _MarginX = this._CreatedControls["txtFilepath"].Size.Width + this._CreatedControls["txtFilepath"].Location.X;
            }

            PosX = _MarginX;
            PosY = 0;
            TmPos = this._CreatedControls["txtFilepath"].Location.Y;
            this.Controls.Add(CreateButton("btnFile", "参照", ref TmPos, ref PosX));
            this._CreatedControls["btnFile"].Click += btnFile_Click;

            this._CreatedControls["btnListen"].Click += btnListen_Click;
        }

        private void btnFile_Click(object? sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.AddExtension = true;
            dialog.ShowDialog();
            if (dialog.FileName == string.Empty || dialog.FileName == null)
            {
                return;
            }

            ((TextBox)this._CreatedControls["txtFilepath"]).Text = dialog.FileName;
        }

        private void btnListen_Click(object? sender, EventArgs e)
        {
            NotificationController _Controller = new NotificationSoundController();
            _Controller = SaveDataToControl(_Controller);
            _Controller.Execute();
        }

        public override void LoadDataToControl(NotificationController Controller)
        {
            ((TextBox)this._CreatedControls["txtFilepath"]).Text = ((NotificationSoundController)Controller).FilePath;
            ((NumericUpDown)this._CreatedControls["numVolume"]).Value = ((NotificationSoundController)Controller).Volume;
            ((NumericUpDown)this._CreatedControls["numPlayTimes"]).Value = ((NotificationSoundController)Controller).PlayTimes;
            ((NumericUpDown)this._CreatedControls["numDistance"]).Value = ((NotificationSoundController)Controller).Distance;
        }

        public override NotificationController SaveDataToControl(NotificationController Controller)
        {
            ((NotificationSoundController)Controller).FilePath = ((TextBox)this._CreatedControls["txtFilepath"]).Text;
            ((NotificationSoundController)Controller).Volume = (int)((NumericUpDown)this._CreatedControls["numVolume"]).Value;
            ((NotificationSoundController)Controller).PlayTimes = (int)((NumericUpDown)this._CreatedControls["numPlayTimes"]).Value;
            ((NotificationSoundController)Controller).Distance = (int)((NumericUpDown)this._CreatedControls["numDistance"]).Value;

            return Controller;
        }
    }
}
