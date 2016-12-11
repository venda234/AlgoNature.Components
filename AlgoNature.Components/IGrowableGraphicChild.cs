using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace AlgoNature.Components
{
    public delegate void RedrawEventHandler(object sender, EventArgs e);

    public interface IGrowableGraphicChild : IBitmapGraphicChild
    {
        // Growable
        float ZeroStateOneLengthPixels { get; set; }
        float OnePartGrowOneLengthPixels { get; set; }
        int AlreadyGrownState { get; set; }
        void GrowOneStep();
        void GrowPart(float part);
        //TimeSpan GrowOneStepTime { get; set; }
        TimeSpan CurrentTimeAfterLastGrowth { get; }
        void StopGrowing();
        void Die();
        void Revive();
        bool IsDead { get; set; }
        TimeSpan TimeToGrowOneStepAfter { get; set; }
        TimeSpan TimeToAverageDieAfter { get; set; }
        double DeathTimeSpanFromAveragePart { get; set; }
        Timer LifeTimer { get; set; }
        void LifeTimerTickHandler(object sender, EventArgs e);
        Point CenterPointParentAbsoluteLocation { get; set; }
        Point Location { get; set; }

        // Graphic
        //Panel PanelNature { get; set; }
        //Bitmap Itself { get; }
        //Bitmap GetItselfBitmap();

        event RedrawEventHandler Redraw;
    }
}
