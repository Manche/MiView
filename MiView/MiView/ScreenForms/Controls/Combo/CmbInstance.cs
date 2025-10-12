using MiView.Common.Connection.VersionInfo;
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

    class CmbSoftware
    {
        public CSoftwareVersionInfo.SOFTWARE_LIST _SOFTWARE;

        public CmbSoftware(CSoftwareVersionInfo.SOFTWARE_LIST Software)
        {
            _SOFTWARE = Software;
        }

        public override string ToString()
        {
            return CSoftwareVersionInfo.SoftwareNames[_SOFTWARE];
        }
    }
}
