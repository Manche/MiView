using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiViewUpdater.Controller
{
    internal class UpdateController
    {
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        public UpdateController Instance { get; } = new UpdateController();

        public void UpdateLoop()
        {
        }

        private void UpdateForm()
        {
        }
    }
}
