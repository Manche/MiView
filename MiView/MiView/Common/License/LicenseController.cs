using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.License
{
    /// <summary>
    /// ライセンスコントローラ
    /// </summary>
    public class LicenseController
    {
        public static LicenseController Instance { get; } = new LicenseController();
    }
}
