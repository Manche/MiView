using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misstab.Common.Notification.Form.Template
{
    public class DataGridNotificationConst
    {
        public enum NOTIFICATION_ELEMENT
        {
            UNDESIGNATED = -1,
            FROM_USER = 0,
            TO_USER = 1,
            NOTIFICATION_TYPE = 2,
            DETAIL = 3,
        }

        public static Dictionary<NOTIFICATION_ELEMENT, float> DataGridColumnWidth = new Dictionary<NOTIFICATION_ELEMENT, float>()
        {
            {NOTIFICATION_ELEMENT.UNDESIGNATED, 0},
            {NOTIFICATION_ELEMENT.FROM_USER, 40 },
            {NOTIFICATION_ELEMENT.TO_USER, 40 },
            {NOTIFICATION_ELEMENT.NOTIFICATION_TYPE, 25 },
            {NOTIFICATION_ELEMENT.DETAIL, 50 }
        };

        public enum NOTIFICATION_TYPE
        {
            UNKNOW = -1,
            REACTION = 0,
            MENTHION = 1,
            REPLY = 2,
            FOLLOWED = 3,
            FOLLOW_REQUEST = 4,
        }

        public static Dictionary<NOTIFICATION_ELEMENT, int> COLUMN_WIDTH = new Dictionary<NOTIFICATION_ELEMENT, int>
        {
            {NOTIFICATION_ELEMENT.UNDESIGNATED, -1},
            {NOTIFICATION_ELEMENT.FROM_USER, 10},
            {NOTIFICATION_ELEMENT.TO_USER, 10},
            {NOTIFICATION_ELEMENT.NOTIFICATION_TYPE, 10 },
            {NOTIFICATION_ELEMENT.DETAIL, 10 },
        };
    }

    public class DataGridNotificationContainer
    {
        public string? FROM_USER { get; set; }
        public string? TO_USER { get; set; }
        public DataGridNotificationConst.NOTIFICATION_TYPE NOTIFICATION_TYPE { get; set; } = DataGridNotificationConst.NOTIFICATION_TYPE.UNKNOW;
        public string? DETAIL { get; set; }
    }

    public partial class DataGridNotification : System.Windows.Forms.DataGridView
    {
        public DataGridNotification()
        {
            foreach (var i in Enum.GetValues(typeof(DataGridNotificationConst.NOTIFICATION_ELEMENT)))
            {
            }
        }
    }
}
