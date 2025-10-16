using MiView.ScreenForms.DialogForm.Setting;
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
    public partial class APIStatusForm : Form
    {
        public APIStatusForm()
        {
            InitializeComponent();
            this.dataGridView1.Rows.Clear();
        }

        private Dictionary<string, int> _InDispData = new Dictionary<string, int>();

        public void SetStatus(List<APIStatusDispData> DispDatas)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => { SetStatus(DispDatas); }));
            }
            if (this.Visible == false)
            {
                return;
            }
            if (_InDispData.Count < DispDatas.Count)
            {
                foreach (var DispData in DispDatas)
                {
                    if (_InDispData.ContainsKey(DispData._HostUrl))
                    {
                        continue;
                    }
                    _InDispData.Add(DispData._HostUrl, this.dataGridView1.Rows.Count);
                    this.dataGridView1.Rows.Add();
                }
            }
            foreach (var DispData in DispDatas)
            {
                int InRow = _InDispData[DispData._HostUrl];
                this.dataGridView1.Rows[InRow].Cells[0].Value = DispData._TLKind;
                this.dataGridView1.Rows[InRow].Cells[1].Value = DispData._HostUrl;
                this.dataGridView1.Rows[InRow].Cells[2].Value = DispData._ConnectionClosed ? "NG" : DispData._ConnectStatus;
                this.dataGridView1.Rows[InRow].Cells[3].Value = DispData._LastReceived;
            }
        }
    }

    public struct APIStatusDispData
    {
        public string _TabDefinition;
        public string _TLKind;
        public string _Host;
        public string _HostUrl;
        public bool _ConnectStatus;
        public DateTime _LastReceived;
        public bool _ConnectionClosed;
    }
}
