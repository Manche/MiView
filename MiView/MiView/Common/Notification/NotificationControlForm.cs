using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiView.Common.Notification
{
    public partial class NotificationControlForm : UserControl
    {
        public NotificationControlForm()
        {
        }

        protected void Initialize()
        {
            InitializeComponent();

            this.AutoSize = true;
        }

        protected Dictionary<string, Control> _CreatedControls = new Dictionary<string, Control>();
        protected int _MarginX = 0;
        protected int _MarginY = 0;
        protected Label CreateLabel(string Name, string Text, ref int PosY, ref int PosX)
        {
            Label lbl = new Label();
            lbl.Name = Name;
            lbl.AutoSize = true;
            lbl.Text = Text;
            lbl.Location = new Point(PosX, PosY);

            PosY += lbl.Size.Height;

            if (lbl.Size.Width + lbl.Location.X > _MarginX)
            {
                _MarginX = lbl.Size.Width + lbl.Location.X;
            }

            this._CreatedControls.Add(Name, lbl);
            return lbl;
        }

        protected TextBox CreateTextBox(string Name,
                                        string Text,
                                        ref int PosY,
                                        ref int PosX)
        {
            TextBox txt = new TextBox();
            txt.Name = Name;
            txt.Text = Text;
            txt.Size = new Size((int)(txt.Font.Size * Text.Length + 7), txt.Size.Height);
            txt.Location = new Point(PosX, PosY);

            PosY += txt.Size.Height;
            if (txt.Size.Width + txt.Location.X > _MarginX)
            {
                _MarginX = txt.Size.Width + txt.Location.X;
            }

            this._CreatedControls.Add(Name, txt);
            return txt;
        }
        protected void SetTextBoxLength(TextBox txt, int Length)
        {
            txt.Size = new Size((int)(txt.Font.Size * Length + 7), txt.Size.Height);
        }
        protected void SetTextBoxHeight(TextBox txt, int Height)
        {
            txt.Multiline = Height != 1;
            txt.Size = new Size(txt.Size.Width, (int)txt.Font.Size * Height);
        }

        public virtual NotificationController SaveDataToControl(NotificationController Controller)
        {
            throw new Exception();
        }
    }
}
