using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoNature.Components
{
    public interface IPropertiesEditingSuspendable
    {
        bool PropertiesEditingMode { get; set; }
        void SuspendForPropertiesEditing();
        void RefreshAfterPropertiesEditing();
    }
}
