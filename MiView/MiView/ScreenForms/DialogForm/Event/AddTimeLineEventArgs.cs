using MiView.Common.Connection.REST.Misskey.v2025.API.Notes;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.ScreenForms.DialogForm.Event
{
    public class AddTimeLineEventArgs
    {
        public string TabDefinition;
        public string TabName;
        public bool IsVisible;
        public bool IsFiltered;

        public AddTimeLineEventArgs(string tabDefinition, string tabName, bool isVisible = true, bool isFiltered = false)
        {
            TabDefinition = tabDefinition;
            TabName = tabName;
            IsVisible = isVisible;
            IsFiltered = isFiltered;
        }
    }

    public class DeleteTimeLineEventArgs
    {
        public string TabDefinition;
        public string TabName;
        public DeleteTimeLineEventArgs(string tabDefinition, string tabName)
        {
            TabDefinition = tabDefinition;
            TabName = tabName;
        }
    }
}
