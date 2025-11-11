using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Setting
{
    public class SettingState
    {
        public static SettingState Instance { get; } = new SettingState();

        public bool IsMuted { get; set; } = false;

        // public EventHandler MuteChanged;
    }
}
