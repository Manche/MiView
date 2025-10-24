using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.ScreenForms.Controls.Combo
{
    public class CmbGeneric
    {
        public object? Key { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        public CmbGeneric(object? key, string name)
        {
            this.Key = key;
            this.Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
