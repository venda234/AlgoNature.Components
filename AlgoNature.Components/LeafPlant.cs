using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Numerics;
using static AlgoNature.Components.Generals;
using static AlgoNature.Components.Geometry;

namespace AlgoNature.Components
{
    public partial class LeafPlant: DockableUserControl<LeafPlant>, IResettableGraphicComponentForVisualisationDocking<LeafPlant>, IGrowableGraphicChild
    {
        // IResettableGraphicComponentForVisualisationDocking Implementation
        public LeafPlant ResetGraphicalAppearanceForImmediateDocking()
        {
            foreach (Control c in panelNature.Controls) c.Dispose();
            panelNature.Controls.Clear();
            try { Itself.Dispose(); } catch { }

            panelNature.Size = this.Size;

            _centerPoint = new Point(this.Width / 2, this.Height / 2);

            _fylotaxisAngle = Convert.ToSingle(GoldenAngleRad);
            _currentFylotaxisAngle = -_fylotaxisAngle;

            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            //LifeTimer.Start();

            return this;
        }

        protected override LeafPlant Redock()
        {
            CenterPoint = new Point(this.Width / 2, this.Height / 2); // Will serve all leaves

            return this;
        }

        #region Constructors
        public LeafPlant()
        {
            InitializeComponent();
            //this.Size = new Size(900, 900);
            _centerPoint = new Point(this.Width / 2, this.Height / 2);
            _oneLengthPixels = 0.2F;
            _drawToGraphics = true;
            //_childrenLeaves = new RedrawHandlingList<Leaf>();
            //_childrenLeaves.Redraw += RedrawPanel;
            //Redraw += delegRdrw;
            //_leafTemplate = new Leaf(_centerPoint, 1, 10, 0, 1, _oneLengthPixels, _oneLengthPixels * 2, _oneLengthPixels * 3,
            //    new TimeSpan(0, 0, 5), new TimeSpan(0, 10, 0), 0.2, 0, true);
            //this.Controls.Add(_leafTemplate);

            _leafTemplate = new Leaf(_centerPoint, 1, 10, 0, 1, _oneLengthPixels, _oneLengthPixels, _oneLengthPixels, 1,
                new TimeSpan(0, 0, 2), new TimeSpan(0, 5, 0), 0.2, _currentFylotaxisAngle, false, false, false, false);

            _fylotaxisAngle = Convert.ToSingle(GoldenAngleRad);
            _currentFylotaxisAngle = -_fylotaxisAngle;

            //GrowOneStep();

            // IGrowable
            _zeroStateOneLengthPixels = 0.05F;
            _onePartGrowOneLengthPixels = 0.05F;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            TimeToGrowOneStepAfter = new TimeSpan(0, 0, 2);
            TimeToAverageDieAfter = new TimeSpan(0, 5, 0);
            DeathTimeSpanFromAveragePart = 0.1;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            //LifeTimer.Start();


            
            //this.Controls[]
        }

        //event RedrawEventHandler IGrowableGraphicChild.Redraw
        //{
        //    add
        //    {
        //        throw new NotImplementedException();
        //    }

        //    remove
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        #endregion

        #region Properties   
        private bool _drawToGraphics;
               
        private Point _centerPoint;
        public Point CenterPoint
        {
            get
            {
                return _centerPoint;
            }
            set
            {
                _centerPoint = value;
                foreach (Leaf leaf in panelNature.Controls)
                {
                    leaf.CenterPointParentAbsoluteLocation = _centerPoint;
                }
                panelNature.Refresh();
            }
        }

        private float _oneLengthPixels;
        public float OneLengthPixels
        {
            get { return _oneLengthPixels; }
            set
            {
                _oneLengthPixels = value;
                panelNature.Refresh();
            }
        }

        private float _fylotaxisAngle;
        public float FylotaxisAngle
        {
            get { return _fylotaxisAngle; }
            set
            {
                _fylotaxisAngle = value;
                panelNature.Refresh();
            }
        }

        private float _currentFylotaxisAngle;

        private volatile Leaf _leafTemplate;
        public Leaf LeafTemplate
        {
            get { return _leafTemplate; }
            set
            {
                _leafTemplate = value;
                panelNature.Refresh();
            }
        }

        //private RedrawHandlingList<Leaf> _childrenLeaves;
        #endregion


        protected override CreateParams CreateParams // Transparent
        {
            get
            {
                CreateParams parms = base.CreateParams;
                parms.ExStyle |= 0x20;
                return parms;
            }
        }

        /*private void RedrawPanel(object sender, EventArgs e)
        {
            panelNature.Refresh();
        }*/

        private async void panelPlant_Paint(object sender, PaintEventArgs e)
        {
            //Itself = new Bitmap(panelNature.Width, panelNature.Height);

            //this.Invalidate();
            this.Enabled = false;
            Graphics gr = e.Graphics;
            panelNature.SuspendLayout();
            Bitmap bmp;
            await panelPaint(out bmp);
            //gr.Clear(Color.Transparent);
            gr.DrawImage(bmp, 0, 0);
            panelNature.ResumeLayout();
            //this.Enabled = true;
            //if (_drawToGraphics) e.Graphics.DrawImageUnscaled(Itself, 0, 0);

            //Redraw.Invoke(this, EventArgs.Empty);
        }
        private Task panelPaint(out Bitmap bitmap)
        {
            Bitmap bmp = new Bitmap(panelNature.Width, panelNature.Height);
            bmp.MakeTransparent();
            Graphics g = Graphics.FromImage(bmp);

            for (int i = 0; i < panelNature.Controls.Count; i++)
            {
                g.DrawImage(new Bitmap(((IGrowableGraphicChild)panelNature.Controls[i]).Itself), ((IGrowableGraphicChild)panelNature.Controls[i]).Location);
            }
            //bitmap = ((IGrowableGraphicChild)panelNature.Controls[0]).Itself; 
            // Pravděpodobně se předává pointer na bitmapu místo statické bitmapy
            // => Vykresluje se automaticky, když se změní bitmapa v paměti
            bitmap = bmp;
            return Task.CompletedTask;
        }

        #region IGrowableGraphicChild implementation
        private int _alreadyGrownState;
        public int AlreadyGrownState
        {
            get
            {
                return _alreadyGrownState;
            }

            set
            {
                _alreadyGrownState = value;
                _oneLengthPixels = _zeroStateOneLengthPixels + (_alreadyGrownState * _onePartGrowOneLengthPixels);
                panelNature.Refresh();
            }
        }

        public Point CenterPointParentAbsoluteLocation
        {
            get
            {
                return this.Location.Add(_centerPoint);
            }

            set
            {
                this.Location = value.Substract(_centerPoint);
            }
        }

        private TimeSpan _currentTimeAfterLastGrowth;
        public TimeSpan CurrentTimeAfterLastGrowth
        {
            get
            {
                return _currentTimeAfterLastGrowth;
            }
            private set
            {
                if (value < TimeToGrowOneStepAfter)
                {
                    _currentTimeAfterLastGrowth = value;
                }
                else
                {
                    _currentTimeAfterLastGrowth = value - TimeToGrowOneStepAfter;
                    GrowOneStep();
                    Thread.Sleep(1000);
                    using (var g = panelNature.CreateGraphics())
                    {
                        g.Dispose();
                    }
                    this.Invalidate();
                }
            }
        }

        public double DeathTimeSpanFromAveragePart
        {
            get;
            set;
        }

        private bool _isDead;
        public bool IsDead
        {
            get { return _isDead; }
            set
            {                
                if (!_isDead && value)
                {
                    LifeTimer.Stop();
                    for (int i = 0; i < panelNature.Controls.Count; i++)
                    {
                        if (panelNature.Controls[i] is Leaf)
                        {
                            ((Leaf)panelNature.Controls[i]).Die();
                        }
                    }
                }
                if (_isDead && !value)
                {
                    LifeTimer.Start();
                }
                _isDead = value;
            }
        }

        public System.Windows.Forms.Timer LifeTimer
        {
            get;
            set;
        }

        public TimeSpan TimeToAverageDieAfter
        {
            get;
            set;
        }

        public TimeSpan TimeToGrowOneStepAfter
        {
            get;
            set;
        }

        private float _zeroStateOneLengthPixels;
        public float ZeroStateOneLengthPixels
        {
            get
            {
                return _zeroStateOneLengthPixels;
            }

            set
            {
                _zeroStateOneLengthPixels = value;
                _oneLengthPixels = _zeroStateOneLengthPixels + (_alreadyGrownState * _onePartGrowOneLengthPixels);
                panelNature.Refresh();
            }
        }

        private float _onePartGrowOneLengthPixels;
                
        public float OnePartGrowOneLengthPixels
        {
            get
            {
                return _onePartGrowOneLengthPixels;
            }

            set
            {
                _onePartGrowOneLengthPixels = value;
                _oneLengthPixels = _zeroStateOneLengthPixels + (_alreadyGrownState * _onePartGrowOneLengthPixels);
                panelNature.Refresh();
            }
        }

        public void StopGrowing()
        {
            LifeTimer.Stop();
            foreach (IGrowableGraphicChild child in panelNature.Controls)
            {
                child.StopGrowing();
            }
        }
        
        public void Die()
        {
            IsDead = true;
            throw new NotImplementedException();
        }

        public void Revive()
        {
            IsDead = false;
            LifeTimer.Start();
            for (int i = 0; i < panelNature.Controls.Count; i++)
            {
                if (panelNature.Controls[i] is Leaf) ((Leaf)panelNature.Controls[i]).Revive();
            }
        }

        public void GrowOneStep()
        {
            this._alreadyGrownState++;
            _currentFylotaxisAngle += _fylotaxisAngle;
            //Leaf toAdd = new Leaf(_centerPoint, 3, 5, 10, 0, _oneLengthPixels, _oneLengthPixels * 2, _oneLengthPixels * 3,
            //    new TimeSpan(0, 0, 30), new TimeSpan(0, 10, 0), 0.1, _currentFylotaxisAngle);
            //toAdd.RotationAngleRad = _currentFylotaxisAngle;
            //this.SuspendLayout();
            
            Leaf t = _leafTemplate;
            Leaf toAdd = new Leaf(_centerPoint,
                t.BranchLength,
                t.DivideAngle,
                t.BeginingAnglePhase,
                t.OnePartPossitinon,
                t.OneLengthPixels,
                t.ZeroStateOneLengthPixels,
                t.OnePartGrowOneLengthPixels,
                t.VeinsFractalisation,
                t.TimeToGrowOneStepAfter,
                t.TimeToAverageDieAfter,
                t.DeathTimeSpanFromAveragePart,
                _currentFylotaxisAngle,
                t.InvertedLeaf,
                t.InvertedBegining, false, true)
            {
                InvertedCurving = t.InvertedCurving,
                InvertedCurvingCenterAngle = t.InvertedCurvingCenterAngle,
                InvertedCurvingSpan = t.InvertedCurvingSpan,
                ContinueAfterInvertedCurving = t.ContinueAfterInvertedCurving
            };
            
            //toAdd.Size = new Size(900, 900);
            //Panel panel = new Panel() { Size = this.Size, BackColor = Color.Transparent };
            //Bitmap bmp = toAdd.Itself;
            //bmp.Dispose();
            panelNature.Controls.Add(toAdd);
            //panelNature.Controls[panelNature.Controls.IndexOf(toAdd)].BringToFront();
            //panelNature.Controls.SetChildIndex(panelNature.Controls[panelNature.Controls.Count - 1], 0);
            //panelNature.Controls[_alreadyGrownState - 1].BringToFront();
            //((Leaf)panelNature.Controls[0]).Location = ((Leaf)panelNature.Controls[0]).Location.Add(this.CenterPoint.Substract(((Leaf)panelNature.Controls[0]).CenterPointParentAbsoluteLocation));
            //this.ResumeLayout();
            
        }

        public void GrowPart(float part)
        {
            //throw new NotImplementedException();
        }

        public void LifeTimerTickHandler(object sender, EventArgs e)
        {
            CurrentTimeAfterLastGrowth += new TimeSpan(0, 0, 0, 0, LifeTimer.Interval);
            //panelNature.Refresh();
        }

        public event RedrawEventHandler Redraw;
        private void delegRdrw(object sender, EventArgs e) { }

        public Bitmap Itself
        {
            get
            {
                int controlsCount = panelNature.Controls.Count;
                // Assuming temporary bitmaps for preventing redrawing while composing the result
                Bitmap[] bmps = new Bitmap[controlsCount];
                Point[] locations = new Point[controlsCount];
                Point loc;
                for (int i = 0; i < controlsCount; i++)
                {
                    bmps[i] = (Bitmap)((IGrowableGraphicChild)panelNature.Controls[i]).Itself.Clone();
                    loc = ((IGrowableGraphicChild)panelNature.Controls[i]).Location;
                    locations[i] = new Point(loc.X, loc.Y);
                }

                // TODO ošetřit řádné vykreslení (StopGrow() -> Revive())
                int minX = 0, minY = 0, maxX = panelNature.Width, maxY = panelNature.Height;
                Point location, maxCorner;
                Size size;
                for (int i = 0; i < controlsCount; i++)
                {
                    location = panelNature.Controls[i].Location;
                    size = panelNature.Controls[i].Size;
                    maxCorner = location.Add(new Point(size.Width - 1, size.Height - 1));

                    if (location.X < minX) minX = location.X;
                    if (location.Y < minY) minY = location.Y;
                    if (maxCorner.X > maxX) maxX = maxCorner.X;
                    if (maxCorner.Y > maxY) maxY = maxCorner.Y;
                }
                Size minShift = new Size(-minX, -minY);
                Size resultSize = new Size(maxX + 1, maxY + 1) + minShift;

                Bitmap res = new Bitmap(resultSize.Width, resultSize.Height);
                //panelNature.DrawToBitmap(res, new Rectangle(new Point(0, 0), panelNature.Size));
                Graphics g = Graphics.FromImage(res);
                for (int i = 0; i < controlsCount; i++)
                {
                    g.DrawImage(bmps[i], locations[i] + minShift);
                }
                return res;                    
            }
        }
        public Bitmap GetItselfBitmap()
        {
            return Itself;
        }
        #endregion

        private void panelNature_DoubleClick(object sender, EventArgs e)
        {
            IsDead = !IsDead;
        }

        private void LeafPlant_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            IsDead = !IsDead;
        }
    }
}
