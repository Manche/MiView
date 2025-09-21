using MiView.Common.Connection.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.ScreenForms.Controls.Combo
{
    class CmbInstance
    {
        public TimeLineBasic.ConnectTimeLineKind _TLKind;
        public string _ViewText;

        public CmbInstance(TimeLineBasic.ConnectTimeLineKind TLKind, string ViewText)
        {
            _TLKind = TLKind;
            _ViewText = ViewText;
        }

        public override string ToString()
        {
            return _ViewText;
        }
    }
}
