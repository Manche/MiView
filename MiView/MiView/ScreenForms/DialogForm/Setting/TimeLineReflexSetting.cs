using MiView.Common.Connection.WebSocket;
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
    public partial class TimeLineReflexSetting : Form
    {
        public WebSocketManager _WSManager {  get; set; }

        public TimeLineReflexSetting()
        {
            InitializeComponent();
        }
    }
}
