using MiView.ScreenForms.DialogForm.Event;
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
    public partial class AddTimeLine : Form
    {

        public AddTimeLine()
        {
            InitializeComponent();
            this.AddTimeLineExecuted += AddTimeLineExecute;
        }

        public event EventHandler<AddTimeLineEventArgs> AddTimeLineExecuted;
        private void cmdAddTab_Click(object sender, EventArgs e)
        {
            AddTimeLineEventArgs ev = new AddTimeLineEventArgs(this.txtTabDefinition.Text, this.txtTabName.Text, true, false);
            this.AddTimeLineExecuted(null, ev);
            this.Close();
        }
        private void AddTimeLineExecute(object? sender, AddTimeLineEventArgs e)
        {
        }

        private void AddTimeLine_Load(object sender, EventArgs e)
        {
            this.txtTabDefinition.Text = Guid.NewGuid().ToString();
            this.txtTabName.Text = string.Empty;
        }
    }
}
