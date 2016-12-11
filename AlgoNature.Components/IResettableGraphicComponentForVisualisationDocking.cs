using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoNature.Components
{
    public interface IResettableGraphicComponentForVisualisationDocking<T>
    {
        /// <summary>
        /// Resets its graphical appearance and returns itself
        /// </summary>
        T ResetGraphicalAppearanceForImmediateDocking();
    }
}
