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
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Resources;
using static AlgoNature.Components.Generals;
using static AlgoNature.Components.Geometry;

namespace AlgoNature.Components
{
    public partial class LeafPlant: DockableUserControl<LeafPlant>, IResettableGraphicComponentForVisualisationDocking<LeafPlant>, IGrowableGraphicChild, ITranslatable
    {
        // ITranslatable
        private bool _translatable = true;
        private bool _translatableForThisCulture = true;
        public string TryTranslate(string translateKey)
        {
            if (_translatable)
            {
                string culture = _translatableForThisCulture ? Thread.CurrentThread.CurrentCulture.Name : DEFAULT_LOCALE_KEY;
                if (_translationDictionaries.Count == 0)
                {
                    if (_translatableForThisCulture/* && _translationDictionaries == null*/) // trying current culture if not previously restricted
                    {
                        _translatable = tryInitializeTranslationDictionary(culture);
                    }
                    /*if (_translationDictionaries.Count == 0) // trying default culture
                    {
                        _translatableForThisCulture = false;
                        culture = DEFAULT_LOCALE_KEY;
                        _translatable = tryInitializeTranslationDictionary(culture);
                    }*/
                }

                /*else if (_translationDictionaries[culture] == null)
                {
                    if (!tryInitializeTranslationDictionary(culture))
                    {
                        _translatable = tryInitializeTranslationDictionary(culture);
                    }
                }*/
                if (_translatable)
                {
                    string res = _translationDictionaries[culture][translateKey];
                    return (res != null) ? res : translateKey;
                }
            }
            return translateKey;
        }
        private Dictionary<string, Dictionary<string, string>> _translationDictionaries = new Dictionary<string, Dictionary<string, string>>();
        private bool tryInitializeTranslationDictionary(string locale)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Type thisType = this.GetType();

            try
            {
                //ResourceManager resmgr = new ResourceManager(thisType.Namespace + ".resources", Assembly.GetExecutingAssembly());
                //var strs = new ResourceReader()
                //var strs = assembly.GetManifestResourceNames();
                using (Stream stream = assembly.GetManifestResourceStream(thisType.FullName + ".PropertiesToTranslate.resources"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    _translationDictionaries[locale] = new Dictionary<string, string>();

                    string line;
                    string[] splitLine;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("System.Resources.ResourceReader")) continue;
                        splitLine = line.Split(new char[6] { '=', '"', '\\', '\t', '\u0002', '\u0004' }); //cleaning firstrow mess
                        /*if (splitLine.Length >= 6) //cleaning firstrow mess
                        {
                            var _splln = new string[splitLine.Length == 7 ? 2 : 1];
                            _splln[0] = splitLine[5];
                            if (splitLine.Length > 6) _splln[1] = splitLine[6];
                            splitLine = _splln;
                        }
                        if (splitLine.Length <= 2)
                        {
                            // cleaning mess
                            if (splitLine[0].Contains('\u0002')) _translationDictionaries[locale].Add(splitLine[1], splitLine[2]);
                            else _translationDictionaries[locale].Add(splitLine[0], splitLine[1]);
                        }
                        else if (splitLine.Length == 1)
                        {
                            _translationDictionaries[locale].Add(splitLine[0], splitLine[0]);
                        }*/
                        try // throws an exception if already exists
                        {
                            if (splitLine[splitLine.Length - 2] == "") // empty
                                _translationDictionaries[locale].Add(splitLine[splitLine.Length - 4], splitLine[splitLine.Length - 4]);
                            else
                                _translationDictionaries[locale].Add(splitLine[splitLine.Length - 4], splitLine[splitLine.Length - 2]);
                        }
                        catch { continue; }
                    }
                }
                return true;
            }
            catch { return false; }
        }

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

            _drawToGraphics = true;

            // IGrowable
            _zeroStateOneLengthPixels = 0.05F;
            _onePartGrowOneLengthPixels = 0.05F;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            TimeToGrowOneStepAfter = new TimeSpan(0, 0, 5);
            TimeToAverageDieAfter = new TimeSpan(0, 5, 0);
            DeathTimeSpanFromAveragePart = 0.1;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            LifeTimer.Start();


            
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
        public bool DrawToGraphics
        {
            get { return _drawToGraphics; }
            set
            {
                _drawToGraphics = value;
                Refresh();
            }
        }

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

#if DEBUG
        private bool _writtenProps = false;
#endif

        

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
            if (_drawToGraphics) gr.DrawImage(bmp, 0, 0);
            panelNature.ResumeLayout();
            //this.Enabled = true;
            //if (_drawToGraphics) e.Graphics.DrawImageUnscaled(Itself, 0, 0);
#if DEBUG
            if (!_writtenProps)
            {
                _writtenProps = true;
                var props = this.GetType().GetProperties();
                List<PropertyInfo> ignoreProps = new List<PropertyInfo>();
                ignoreProps.AddRange(typeof(IBitmapGraphicChild).GetProperties());
                ignoreProps.AddRange(typeof(UserControl).GetProperties());

                try
                {
                    StreamWriter swf = new StreamWriter(new FileStream("LeafPlant.PropertiesToTranslate.txt", FileMode.Truncate));


                    foreach (PropertyInfo property in props)
                    {
                        if (property?.GetMethod?.IsPublic == true)
                            if ((ignoreProps.Where(new Func<PropertyInfo, bool>((proprt) => property.Name == proprt.Name)).ToArray().Length) == 0)
                            {
                                swf.WriteLine(property.Name + "=\"\"");
                            }
                    }
                    swf.Close();
                }
                catch { }
            }

            Console.WriteLine(this.TryTranslate("CenterPoint"));
#endif
            //Redraw.Invoke(this, EventArgs.Empty);
        }
        private Task panelPaint(out Bitmap bitmap)
        {
            Bitmap bmp = new Bitmap(panelNature.Width, panelNature.Height);
            bmp.MakeTransparent();
            Graphics g = Graphics.FromImage(bmp);

            for (int i = 0; i < panelNature.Controls.Count; i++)
            {
                g.DrawImage(new Bitmap((Bitmap)((IGrowableGraphicChild)panelNature.Controls[i]).Itself.Clone()), ((IGrowableGraphicChild)panelNature.Controls[i]).Location);
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
                    _currentTimeAfterLastGrowth += value;
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

            Leaf toAdd = (Leaf)_leafTemplate.CloneForFastGraphics(false, _currentFylotaxisAngle, true);
            //toAdd.RotationAngleRad = _currentFylotaxisAngle;
            //toAdd._drawToGraphics = false;
            /*Leaf t = _leafTemplate;
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
                ContinueAfterInvertedCurving = t.ContinueAfterInvertedCurving,

                AccurateLogarithmicSpiral;
                Veins;
                VeinsBorderReachPart;
                CentralVeinPixelThickness;
                AbsoluteCenterPointLocation=""
                CenterPoint=""
                GrowFrom_CenterPoint=""
                AccurateLogarithmicSpiral=""
                OneLengthPixels=""
                OnePartPossitinon=""
                BeginingAnglePhase=""
                Veins=""
                VeinsFractalisation=""
                VeinsBorderReachPart=""
                CentralVeinPixelThickness=""
                Fill=""
                BorderColor=""
                BorderPen=""
                VitalFillColor=""
                CurrentFillColor=""
                CurrentFillBrush=""
                VitalFillBrush=""
                VeinsColor=""
                LeftCurveTension=""
                RightCurveTension=""
                CurveBehindCenterPoint=""
                SmoothTop=""
                InvertedCurving=""
                ContinueAfterInvertedCurving=""
                InvertedCurvingCenterAngle=""
                InvertedCurvingSpan=""
                RotationAngleRad=""
                IsBilaterallySymetric=""
                DivideAngle=""
                LeftDivideAngle=""
                RightDivideAngle=""
                BranchLength=""
                HasBranch=""
                BranchPen=""
                PropertiesEditingMode=""
                InvertedBegining=""
                InvertedLeaf=""
                ZeroStateOneLengthPixels=""
                OnePartGrowOneLengthPixels=""
                AlreadyGrownState=""
                CurrentTimeAfterLastGrowth=""
                IsDead=""
                BranchGrowthType=""
                LeafGrowthType=""
                ZeroStateBranchOneLengthPixels=""
                TimeToGrowOneStepAfter=""
                TimeToAverageDieAfter=""
                DeathTimeSpanFromAveragePart=""
                LifeTimer=""
                CenterPointParentAbsoluteLocation=""
            };*/

            //toAdd.Size = new Size(900, 900);
            //Panel panel = new Panel() { Size = this.Size, BackColor = Color.Transparent };
            //Bitmap bmp = toAdd.Itself;
            //bmp.Dispose();
            //toAdd.LifeTimer.Tick += new EventHandler(toAdd.Gr)
            //toAdd.LifeTimer.Start();
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

    public partial class Leaf
    {
        public Leaf CloneForFastGraphics()
        {
            Leaf res = new Leaf(this._centerPoint,
                this.BranchLength,
                this.DivideAngle,
                this.BeginingAnglePhase,
                this.OnePartPossitinon,
                this.OneLengthPixels,
                this.ZeroStateOneLengthPixels,
                this.OnePartGrowOneLengthPixels,
                this.VeinsFractalisation,
                this.TimeToGrowOneStepAfter,
                this.TimeToAverageDieAfter,
                this.DeathTimeSpanFromAveragePart,
                0,
                this.InvertedLeaf,
                this.InvertedBegining, 
                this._drawToGraphics, 
                this.LifeTimer.Enabled, 
                true)
            {
                _translatable = this._translatable,
                _translatableForThisCulture = this._translatableForThisCulture,
                _translationDictionaries = (this._translationDictionaries == null) ?
                    new Dictionary<string, Dictionary<string, string>>()
                    : this._translationDictionaries,
                _userEditedCenterPoint = this._userEditedCenterPoint,
                _growFrom_CenterPoint = this._growFrom_CenterPoint,
                _accurateLogarithmicSpiral = this._accurateLogarithmicSpiral,
                //_oneLengthPixels = this._oneLengthPixels,
                //_onePartPossitionDegrees = this._onePartPossitionDegrees,
                //_beginingAnglePhase = this._beginingAnglePhase,
                _veins = this._veins,
                //_veinsFractalisation = this._veinsFractalisation,
                _veinsBorderReachPart = this._veinsBorderReachPart,
                _centralVeinPixelThickness = this._centralVeinPixelThickness,
                _fill = this._fill,
                _borderPen = this._borderPen,
                _fillBrush = this._fillBrush,
                _vitalFillBrush = this._vitalFillBrush,
                _veinsColor = this._veinsColor,
                _leftCurveTension = this._leftCurveTension,
                _rightCurveTension = this._rightCurveTension,
                _curveBehindCenterPoint = this._curveBehindCenterPoint,
                _smoothTop = this._smoothTop,
                _invertedCurving = this._invertedCurving,
                _continueAfterInvertedCurving = this._continueAfterInvertedCurving,
                _invertedCurvingCenterAngle = this._invertedCurvingCenterAngle,
                _invertedCurvingSpan = this._invertedCurvingSpan,
                //_rotationAngleRad = this._rotationAngleRad,
                _isBilaterallySymetric = this._isBilaterallySymetric,
                _divideAngle = this._divideAngle,
                _leftDivideAngle = this._leftDivideAngle,
                _rightDivideAngle = this._rightDivideAngle,
                //_branchLength = this._branchLength,
                _hasBranch = this._hasBranch,
                _centerPointBelongsToBranch = this._centerPointBelongsToBranch,
                _branchPen = this._branchPen,
                _propertiesEditingMode = this._propertiesEditingMode,
                _hasBeenAccurate = this._hasBeenAccurate,
                //_invertedBegining = this._invertedBegining,
                //_invertedLeaf = this._invertedLeaf,
                _zeroStateOneLengthPixels = this._zeroStateOneLengthPixels,
                //_onePartGrowOneLengthPixels = this._onePartGrowOneLengthPixels,
                _alreadyGrownState = this._alreadyGrownState,
                _currentTimeAfterLastGrowth = this._currentTimeAfterLastGrowth,
                _isDead = this._isDead,
                _branchGrowthType = this._branchGrowthType,
                _leafGrowthType = this._leafGrowthType//,
                //_zeroStateBranchOneLengthPixels = this._zeroStateBranchOneLengthPixels,
                //_timeToGrowOneStepAfter = this._timeToGrowOneStepAfter,
                //_timeToAverageDieAfter = this._timeToAverageDieAfter,
                //_deathTimeSpanFromAveragePart = this._deathTimeSpanFromAveragePart,
                //_itself = this._itself
            };
            res.RefreshAfterPropertiesEditing();
            return res;
        }
        public Leaf CloneForFastGraphics(bool drawToGraphics, float fylotaxisAngle, bool startGrowing)
        {
            Leaf res = new Leaf(this._centerPoint,
                this.BranchLength,
                this.DivideAngle,
                this.BeginingAnglePhase,
                this.OnePartPossitinon,
                this.OneLengthPixels,
                this.ZeroStateOneLengthPixels,
                this.OnePartGrowOneLengthPixels,
                this.VeinsFractalisation,
                this.TimeToGrowOneStepAfter,
                this.TimeToAverageDieAfter,
                this.DeathTimeSpanFromAveragePart,
                fylotaxisAngle,
                this.InvertedLeaf,
                this.InvertedBegining,
                drawToGraphics, startGrowing, true)
            {
                _translatable = this._translatable,
                _translatableForThisCulture = this._translatableForThisCulture,
                _translationDictionaries = (this._translationDictionaries == null) ?
                    new Dictionary<string, Dictionary<string, string>>() 
                    : this._translationDictionaries,
                _userEditedCenterPoint = this._userEditedCenterPoint,
                _growFrom_CenterPoint = this._growFrom_CenterPoint,
                _accurateLogarithmicSpiral = this._accurateLogarithmicSpiral,
                //_oneLengthPixels = this._oneLengthPixels,
                //_onePartPossitionDegrees = this._onePartPossitionDegrees,
                //_beginingAnglePhase = this._beginingAnglePhase,
                _veins = this._veins,
                //_veinsFractalisation = this._veinsFractalisation,
                _veinsBorderReachPart = this._veinsBorderReachPart,
                _centralVeinPixelThickness = this._centralVeinPixelThickness,
                _fill = this._fill,
                _borderPen = this._borderPen,
                _fillBrush = this._fillBrush,
                _vitalFillBrush = this._vitalFillBrush,
                _veinsColor = this._veinsColor,
                _leftCurveTension = this._leftCurveTension,
                _rightCurveTension = this._rightCurveTension,
                _curveBehindCenterPoint = this._curveBehindCenterPoint,
                _smoothTop = this._smoothTop,
                _invertedCurving = this._invertedCurving,
                _continueAfterInvertedCurving = this._continueAfterInvertedCurving,
                _invertedCurvingCenterAngle = this._invertedCurvingCenterAngle,
                _invertedCurvingSpan = this._invertedCurvingSpan,
                //_rotationAngleRad = this._rotationAngleRad,
                _isBilaterallySymetric = this._isBilaterallySymetric,
                _divideAngle = this._divideAngle,
                _leftDivideAngle = this._leftDivideAngle,
                _rightDivideAngle = this._rightDivideAngle,
                //_branchLength = this._branchLength,
                _hasBranch = this._hasBranch,
                _centerPointBelongsToBranch = this._centerPointBelongsToBranch,
                _branchPen = this._branchPen,
                _propertiesEditingMode = this._propertiesEditingMode,
                _hasBeenAccurate = this._hasBeenAccurate,
                //_invertedBegining = this._invertedBegining,
                //_invertedLeaf = this._invertedLeaf,
                _zeroStateOneLengthPixels = this._zeroStateOneLengthPixels,
                //_onePartGrowOneLengthPixels = this._onePartGrowOneLengthPixels,
                _alreadyGrownState = this._alreadyGrownState,
                _currentTimeAfterLastGrowth = this._currentTimeAfterLastGrowth,
                _isDead = this._isDead,
                _branchGrowthType = this._branchGrowthType,
                _leafGrowthType = this._leafGrowthType//,
                //_zeroStateBranchOneLengthPixels = this._zeroStateBranchOneLengthPixels,
                //_timeToGrowOneStepAfter = this._timeToGrowOneStepAfter,
                //_timeToAverageDieAfter = this._timeToAverageDieAfter,
                //_deathTimeSpanFromAveragePart = this._deathTimeSpanFromAveragePart,
                //_itself = this._itself
            };
            res.RefreshAfterPropertiesEditing();
            return res;
        }
        //    public struct LeafTemplate : ITranslatable
        //    {
        //        //private Leaf translateLeaf;
        //        /*public string TryTranslate(string translationKey)
        //        {
        //            if (translateLeaf == null) translateLeaf = new Leaf() { _oneLengthPixels = 0.001F, _drawToGraphics = false };
        //            return translateLeaf.TryTranslate()
        //        }*/

        //        public LeafTemplate(Leaf leafPattern)
        //        {
        //            try
        //            {
        //                _translationDictionaries = leafPattern._translationDictionaries;
        //            }
        //            catch
        //            {
        //                string trans = leafPattern.TryTranslate("a");
        //                _translationDictionaries = leafPattern._translationDictionaries;
        //            }
        //            if (_translationDictionaries == null)
        //                _translationDictionaries = new Dictionary<string, Dictionary<string, string>>();
        //            _translatable = leafPattern._translatable;
        //            _translatableForThisCulture = leafPattern._translatableForThisCulture;

        //            BranchLength = leafPattern._branchLength;
        //            DivideAngle = leafPattern._divideAngle;
        //            BeginingAnglePhase = leafPattern._beginingAnglePhase;
        //            OnePartPossitinon = leafPattern._onePartPossitionDegrees;
        //            OneLengthPixels = leafPattern._oneLengthPixels;
        //            ZeroStateOneLengthPixels = leafPattern._zeroStateOneLengthPixels;
        //            OnePartGrowOneLengthPixels = leafPattern._onePartGrowOneLengthPixels;
        //            VeinsFractalisation = leafPattern._veinsFractalisation;
        //            TimeToGrowOneStepAfter = leafPattern._timeToGrowOneStepAfter;
        //            TimeToAverageDieAfter = leafPattern._timeToAverageDieAfter;
        //            DeathTimeSpanFromAveragePart = leafPattern._deathTimeSpanFromAveragePart;
        //            InvertedLeaf = leafPattern._invertedLeaf;
        //            InvertedBegining = leafPattern._invertedBegining;
        //            InvertedCurving = leafPattern._invertedCurving;
        //            InvertedCurvingCenterAngle = leafPattern._invertedCurvingCenterAngle;
        //            InvertedCurvingSpan = leafPattern._invertedCurvingSpan;
        //            ContinueAfterInvertedCurving = leafPattern._continueAfterInvertedCurving;
        //        }

        //        public float BranchLength;
        //        public float DivideAngle;
        //        public int BeginingAnglePhase;
        //        public int OnePartPossitinon;
        //        public float OneLengthPixels;
        //        public float ZeroStateOneLengthPixels;
        //        public float OnePartGrowOneLengthPixels;
        //        public int VeinsFractalisation;
        //        public TimeSpan TimeToGrowOneStepAfter;
        //        public TimeSpan TimeToAverageDieAfter;
        //        public double DeathTimeSpanFromAveragePart;
        //        public bool InvertedLeaf;
        //        public bool InvertedBegining;
        //        public bool InvertedCurving;
        //        public int InvertedCurvingCenterAngle;
        //        public int InvertedCurvingSpan;
        //        public bool ContinueAfterInvertedCurving;

        //        public bool AccurateLogarithmicSpiral;
        //        public bool Veins;
        //        public float VeinsBorderReachPart;
        //        public float CentralVeinPixelThickness;
        //        AbsoluteCenterPointLocation=""
        //        CenterPoint=""
        //        GrowFrom_CenterPoint=""
        //        AccurateLogarithmicSpiral=""
        //        OneLengthPixels=""
        //        OnePartPossitinon=""
        //        BeginingAnglePhase=""
        //        Veins=""
        //        VeinsFractalisation=""
        //        VeinsBorderReachPart=""
        //        CentralVeinPixelThickness=""
        //        Fill=""
        //        BorderColor=""
        //        BorderPen=""
        //        VitalFillColor=""
        //        CurrentFillColor=""
        //        CurrentFillBrush=""
        //        VitalFillBrush=""
        //        VeinsColor=""
        //        LeftCurveTension=""
        //        RightCurveTension=""
        //        CurveBehindCenterPoint=""
        //        SmoothTop=""
        //        InvertedCurving=""
        //        ContinueAfterInvertedCurving=""
        //        InvertedCurvingCenterAngle=""
        //        InvertedCurvingSpan=""
        //        RotationAngleRad=""
        //        IsBilaterallySymetric=""
        //        DivideAngle=""
        //        LeftDivideAngle=""
        //        RightDivideAngle=""
        //        BranchLength=""
        //        HasBranch=""
        //        BranchPen=""
        //        PropertiesEditingMode=""
        //        InvertedBegining=""
        //        InvertedLeaf=""
        //        ZeroStateOneLengthPixels=""
        //        OnePartGrowOneLengthPixels=""
        //        AlreadyGrownState=""
        //        CurrentTimeAfterLastGrowth=""
        //        IsDead=""
        //        BranchGrowthType=""
        //        LeafGrowthType=""
        //        ZeroStateBranchOneLengthPixels=""
        //        TimeToGrowOneStepAfter=""
        //        TimeToAverageDieAfter=""
        //        DeathTimeSpanFromAveragePart=""
        //        LifeTimer=""
        //        CenterPointParentAbsoluteLocation=""

        //        // ITranslatable
        //        private bool _translatable;
        //        private bool _translatableForThisCulture;
        //        public string TryTranslate(string translateKey)
        //        {
        //            if (_translatable)
        //            {
        //                string culture = _translatableForThisCulture ? Thread.CurrentThread.CurrentCulture.Name : DEFAULT_LOCALE_KEY;
        //                if (_translationDictionaries.Count == 0)
        //                {
        //                    if (_translatableForThisCulture/* && _translationDictionaries == null*/) // trying current culture if not previously restricted
        //                    {
        //                        _translatable = tryInitializeTranslationDictionary(culture);
        //                    }
        //                    /*if (_translationDictionaries.Count == 0) // trying default culture
        //                    {
        //                        _translatableForThisCulture = false;
        //                        culture = DEFAULT_LOCALE_KEY;
        //                        _translatable = tryInitializeTranslationDictionary(culture);
        //                    }*/
        //                }

        //                /*else if (_translationDictionaries[culture] == null)
        //                {
        //                    if (!tryInitializeTranslationDictionary(culture))
        //                    {
        //                        _translatable = tryInitializeTranslationDictionary(culture);
        //                    }
        //                }*/
        //                if (_translatable)
        //                {
        //                    string res = _translationDictionaries[culture][translateKey];
        //                    return (res != null) ? res : translateKey;
        //                }
        //            }
        //            return translateKey;
        //        }
        //        private Dictionary<string, Dictionary<string, string>> _translationDictionaries;
        //        private bool tryInitializeTranslationDictionary(string locale)
        //        {
        //            var assembly = Assembly.GetExecutingAssembly();
        //            Type thisType = this.GetType();

        //            try
        //            {
        //                //ResourceManager resmgr = new ResourceManager(thisType.Namespace + ".resources", Assembly.GetExecutingAssembly());
        //                //var strs = new ResourceReader()
        //                //var strs = assembly.GetManifestResourceNames();
        //                using (Stream stream = assembly.GetManifestResourceStream(thisType.FullName + ".PropertiesToTranslate.resources"))
        //                using (StreamReader reader = new StreamReader(stream))
        //                {
        //                    _translationDictionaries[locale] = new Dictionary<string, string>();

        //                    string line;
        //                    string[] splitLine;
        //                    while ((line = reader.ReadLine()) != null)
        //                    {
        //                        if (line.Contains("System.Resources.ResourceReader")) continue;
        //                        splitLine = line.Split(new char[6] { '=', '"', '\\', '\t', '\u0002', '\u0004' }); //cleaning firstrow mess
        //                        try // throws an exception if already exists
        //                        {
        //                            if (splitLine[splitLine.Length - 2] == "") // empty
        //                                _translationDictionaries[locale].Add(splitLine[splitLine.Length - 4], splitLine[splitLine.Length - 4]);
        //                            else
        //                                _translationDictionaries[locale].Add(splitLine[splitLine.Length - 4], splitLine[splitLine.Length - 2]);
        //                        }
        //                        catch { continue; }
        //                    }
        //                }
        //                return true;
        //            }
        //            catch { return false; }
        //        }
        //    }

        //    public LeafTemplate CopyTemplateForLeafPlant()
        //    {
        //        string str = this.TryTranslate("a");
        //        return new LeafTemplate(this);
        //    }

        //    /*public Leaf CloneMemberwise()
        //    {
        //        return (Leaf)this.MemberwiseClone();
        //    }*/
    }
}
