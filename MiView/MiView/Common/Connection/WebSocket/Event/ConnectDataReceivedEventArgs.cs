using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Connection.WebSocket.Event
{
    public class ConnectDataReceivedEventArgs : EventArgs
    {
        public string MessageRaw
        {
            get; set;
        } = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectDataReceivedEventArgs()
        {
        }
    }
}
