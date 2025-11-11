using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiView.ScreenForms.DialogForm
{
    public partial class Splash : Form
    {
        /// <summary>
        /// シングルトン
        /// </summary>
        public static Splash instance { get; } = new Splash();

        public Splash()
        {
            InitializeComponent();

            this.ControlBox = false;
            this.Text = string.Empty;
        }

        public void SetMessageAndProgress(string Txt, int Num)
        {
            SetMessage(Txt);
            SetProgress(Num);
            this.Refresh();
            Thread.Sleep(100);
        }

        public void SetMessage(string Txt)
        {
            if (InvokeRequired)
            {
                this.Invoke(SetMessage, Txt);
                return;
            }
            this.lblMsg.Text = Txt;
            this.lblMsg.AutoSize = true;
        }

        public void SetProgress(int Num)
        {
            if (InvokeRequired)
            {
                this.Invoke(SetProgress, Num);
                return;
            }
            this.progWait.Value = Num;
        }

        public void CloseForm()
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(this.CloseForm));
                    return;
                }
                this.Close();
            }
            catch
            {
            }
        }

        private void Splash_Load(object sender, EventArgs e)
        {
        }
    }
}
