using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

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

        protected NumericUpDown CreateNumberBox(string Name,
                                        int Value,
                                        ref int PosY,
                                        ref int PosX,
                                        int Min = 0,
                                        int Max = 100)
        {
            NumericUpDown num = new NumericUpDown();
            num.Name = Name;
            num.Minimum = Min;
            num.Maximum = Max;
            num.Value = Value;
            num.Size = new Size((int)(num.Font.Size * Text.Length + 7), num.Size.Height);
            num.Location = new Point(PosX, PosY);

            PosY += num.Size.Height;
            if (num.Size.Width + num.Location.X > _MarginX)
            {
                _MarginX = num.Size.Width + num.Location.X;
            }

            this._CreatedControls.Add(Name, num);
            return num;
        }

        protected Button CreateButton(string Name,
                                      string Text,
                                      ref int PosY,
                                      ref int PosX)
        {
            Button btn = new Button();
            btn.Name = Name;
            btn.Text = Text;
            btn.Size = new Size((int)(btn.Font.Size * Text.Length + 7), btn.Size.Height);
            btn.Location = new Point(PosX, PosY);

            PosY += btn.Size.Height;
            if (btn.Size.Width + btn.Location.X > _MarginX)
            {
                _MarginX = btn.Size.Width + btn.Location.X;
            }

            this._CreatedControls.Add(Name, btn);
            return btn;
        }

        protected ComboBox CreateComboBox(string Name,
                                         string Text,
                                         object[] Items,
                                         ref int PosY,
                                         ref int PosX)
        {
            ComboBox cmb = new ComboBox();
            cmb.Name = Name;
            cmb.Text = Text;
            cmb.Items.AddRange(Items);
            cmb.Size = new Size((Items.Select(r => { return r.ToString()?.Length; }).Max() * (int)cmb.Font.Size + 7) ?? cmb.Size.Width, cmb.Size.Height);
            cmb.Location = new Point(PosX, PosY);

            PosY += cmb.Size.Height;
            if (cmb.Size.Width + cmb.Location.X > _MarginX)
            {
                _MarginX = cmb.Size.Width + cmb.Location.X;
            }

            this._CreatedControls.Add(Name, cmb);
            return cmb;
        }
        protected void SetTextBoxLength(TextBox txt, int Length)
        {
            txt.Size = new Size((int)(txt.Font.Size * Length + 7), txt.Size.Height);
        }
        protected void SetTextBoxLength(NumericUpDown txt, int Length)
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

        public virtual void LoadDataToControl(NotificationController Controller)
        {
            throw new Exception();
        }
    }
}
