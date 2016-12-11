using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;


namespace AlgoNature.Components
{
    public abstract class DockableUserControl<T> : UserControl, IResettableGraphicComponentForVisualisationDocking<T>
        where T : UserControl, IResettableGraphicComponentForVisualisationDocking<T>
    {
        public virtual T DockOnSize(Size parentsSize)
        {
            this.Dock = DockStyle.Fill;
            this.Size = parentsSize;
            return this.ResetGraphicalAppearanceForImmediateDocking();
        }

        public virtual T ResetGraphicalAppearanceForImmediateDocking()
        {
            throw new NotImplementedException();
        }
    }
}
