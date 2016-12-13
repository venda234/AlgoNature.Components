using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Threading;
using static AlgoNature.Components.Generals;
using static AlgoNature.Components.Geometry;
//using System.Timers;

namespace AlgoNature.Components
{
    public partial class Leaf : DockableUserControl<Leaf>, IResettableGraphicComponentForVisualisationDocking<Leaf>, IGrowableGraphicChild, IToRedrawEventHandlingList, ITranslatable //IObservable<bool>
    {
        // ITranslatable
        private bool _translatable = true;
        private bool _translatableForThsCulture = true;
        public string TryTranslate(string translateKey)
        {
            if (_translatable)
            {
                string culture = _translatableForThsCulture ? Thread.CurrentThread.CurrentCulture.Name : DEFAULT_LOCALE_KEY;
                if (_translationDictionaries.Count == 0)
                {
                    if (_translatableForThsCulture/* && _translationDictionaries == null*/) // trying current culture if not previously restricted
                    {
                        _translatable = tryInitializeTranslationDictionary(culture);
                    }
                    if (_translationDictionaries.Count == 0) // trying default culture
                    {
                        _translatableForThsCulture = false;
                        culture = DEFAULT_LOCALE_KEY;
                        _translatable = tryInitializeTranslationDictionary(culture);
                    }
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
            var resourceName = thisType.Name;
            if (resourceName.Contains(thisType.Namespace))
                resourceName.Remove(0, thisType.Namespace.Length + 1);
            resourceName += ".PropertiesToTranslate." + locale + ".txt";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    _translationDictionaries[locale] = new Dictionary<string, string>();

                    string line;
                    string[] splitLine;
                    while ((line = reader.ReadLine()) != null)
                    {
                        splitLine = line.Split(new char[2] { '=', '"' }, StringSplitOptions.RemoveEmptyEntries);
                        if (splitLine.Length == 2)
                        {
                            _translationDictionaries[locale].Add(splitLine[0], splitLine[1]);
                        }
                        else if (splitLine.Length == 1)
                        {
                            _translationDictionaries[locale].Add(splitLine[0], splitLine[0]);
                        }
                    }
                }
                return true;
            }
            catch { return false; }
        }

        // IResettableGraphicComponentForVisualisationDocking Implementation
        public Leaf ResetGraphicalAppearanceForImmediateDocking()
        {
            secondPaint = true;

            Itself.Dispose();

            panelNature.Size = this.Size;

            _oneLengthPixels = 6;

            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle * (180 - _onePartPossitionDegrees - _beginingAnglePhase + 1) / 180F));
            _centerPoint = new Point(xCenter, yCenter);

            _fillBrush = _vitalFillBrush;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = false;

            _branchLength = _zeroStateBranchOneLengthPixels;
            _centerPointBelongsToBranch = true;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);
            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            tempSize = this.Size;

            //IGrowable
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            if (LifeTimer.Enabled)
            {
                LifeTimer = new System.Windows.Forms.Timer();
                LifeTimer.Interval = 500;
                LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
                LifeTimer.Start();
            }
            /*else -------- caused repeated rerendering
            {
                doItselfRefresh();
            }*/
            //this.Refresh();

            //throw new NotImplementedException();
            doRefresh();
            return this;
        }

        protected override Leaf Redock()
        {
            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle * (180 - _onePartPossitionDegrees - _beginingAnglePhase + 1) / 180F));
            _centerPoint = new Point(xCenter, yCenter);

            doRefresh();

            return this;
        }

        #region Constructors
        //public static Leaf FromTemplate(Leaf Template)
        //    => new Leaf(Template.CenterPoint, Template.BranchLength, Template.DivideAngle, T)

        public Leaf()
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = 10;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0F;
            _oneLengthPixels = 6;
            _onePartPossitionDegrees = 1;
            _beginingAnglePhase = 0;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;
            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle * (180 - _onePartPossitionDegrees - _beginingAnglePhase + 1) / 180F));
            _centerPoint = new Point(xCenter, yCenter);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = 1;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = false;
            _hasBranch = true;
            _branchLength = _zeroStateBranchOneLengthPixels = 5;
            _centerPointBelongsToBranch = true;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);
            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            tempSize = this.Size;
            _invertedBegining = false;
            
            //IGrowable
            _zeroStateOneLengthPixels = 0.5F;
            _onePartGrowOneLengthPixels = 0.5F;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = new TimeSpan(0, 0, 10);
            _timeToAverageDieAfter = new TimeSpan(0, 5, 0);
            _deathTimeSpanFromAveragePart = 0.1;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            //LifeTimer.Start();
            //this.Refresh();

            doItselfRefresh();
        }
        public Leaf(bool Growing)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = 10;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = 3;
            _onePartPossitionDegrees = 1;
            _beginingAnglePhase = 0;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;
            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle * (180 - _onePartPossitionDegrees - _beginingAnglePhase + 1) / 180F));
            _centerPoint = new Point(xCenter, yCenter);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = 1;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = false;
            _hasBranch = false;
            _branchLength = _zeroStateBranchOneLengthPixels = 0;
            _centerPointBelongsToBranch = false;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);

            //IGrowable
            _zeroStateOneLengthPixels = 0.5F;
            _onePartGrowOneLengthPixels = 0.5F;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = new TimeSpan(0, 0, 10);
            _timeToAverageDieAfter = new TimeSpan(0, 5, 0);
            _deathTimeSpanFromAveragePart = 0.1;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            if (Growing)
            {
                LifeTimer.Start();
            }

            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            _invertedBegining = false;
            doItselfRefresh();
        }
        public Leaf(Point AbsoluteCenterPoint)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = 10;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = 3;
            _onePartPossitionDegrees = 1;
            _beginingAnglePhase = 0;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;
            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle * (180 - _onePartPossitionDegrees - _beginingAnglePhase + 1) / 180F));
            _centerPoint = new Point(xCenter, yCenter);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = 1;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            changeLocation = AbsoluteCenterPoint.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = false;
            _branchLength = _zeroStateBranchOneLengthPixels = 0;
            _centerPointBelongsToBranch = false;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);

            //IGrowable
            _zeroStateOneLengthPixels = 0.5F;
            _onePartGrowOneLengthPixels = 0.5F;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = new TimeSpan(0, 0, 10);
            _timeToAverageDieAfter = new TimeSpan(0, 5, 0);
            _deathTimeSpanFromAveragePart = 0.1;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            //LifeTimer.Start();
            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            _invertedBegining = false;
            doItselfRefresh();
        }
        public Leaf(Point AbsoluteCenterPoint, bool Growing)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = 10;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = 3;
            _onePartPossitionDegrees = 1;
            _beginingAnglePhase = 0;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;
            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle * (180 - _onePartPossitionDegrees - _beginingAnglePhase + 1) / 180F));
            _centerPoint = new Point(xCenter, yCenter);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = 1;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            changeLocation = AbsoluteCenterPoint.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = false;
            _branchLength = _zeroStateBranchOneLengthPixels = 0;
            _centerPointBelongsToBranch = false;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);

            //IGrowable
            _zeroStateOneLengthPixels = 0.5F;
            _onePartGrowOneLengthPixels = 0.5F;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = new TimeSpan(0, 0, 10);
            _timeToAverageDieAfter = new TimeSpan(0, 5, 0);
            _deathTimeSpanFromAveragePart = 0.1;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            if (Growing)
            {     
                LifeTimer.Start();
            }

            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            _invertedBegining = false;
            doItselfRefresh();
        }
        public Leaf(Point AbsoluteCenterPointLocation, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = DivideAngle;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = OneLengthPixels;
            _onePartPossitionDegrees = OnePartRelativePosition;
            _beginingAnglePhase = StartPart;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;
            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle * (180 - _onePartPossitionDegrees - _beginingAnglePhase + 1) / 180F));
            _centerPoint = new Point(xCenter, yCenter);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = 1;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            changeLocation = AbsoluteCenterPointLocation.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = false;
            _branchLength = _zeroStateBranchOneLengthPixels = 0;
            _centerPointBelongsToBranch = false;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);

            //IGrowable
            _zeroStateOneLengthPixels = 0.5F;
            _onePartGrowOneLengthPixels = 0.5F;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = new TimeSpan(0, 0, 10);
            _timeToAverageDieAfter = new TimeSpan(0, 5, 0);
            _deathTimeSpanFromAveragePart = 0.1;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            //LifeTimer.Start();

            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            _invertedBegining = false;
            doItselfRefresh();
        }
        public Leaf(Point AbsoluteCenterPointLocation, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels, bool Growing)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = DivideAngle;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = OneLengthPixels;
            _onePartPossitionDegrees = OnePartRelativePosition;
            _beginingAnglePhase = StartPart;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;
            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle * (180 - _onePartPossitionDegrees - _beginingAnglePhase + 1) / 180F));
            _centerPoint = new Point(xCenter, yCenter);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = 1;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            changeLocation = AbsoluteCenterPointLocation.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = false;
            _branchLength = _zeroStateBranchOneLengthPixels = 0;
            _centerPointBelongsToBranch = false;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);

            //IGrowable
            _zeroStateOneLengthPixels = OneLengthPixels;
            _onePartGrowOneLengthPixels = OneLengthPixels;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = new TimeSpan(0, 0, 10);
            _timeToAverageDieAfter = new TimeSpan(0, 5, 0);
            _deathTimeSpanFromAveragePart = 0.1;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            if (Growing)
            {                
                LifeTimer.Start();
            }

            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            _invertedBegining = false;
            doItselfRefresh();
        }
        public Leaf(Point AbsoluteCenterPointLocation, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels, 
            float zeroStateOneLengthPixels, float onePartGrowOneLengthPixels, TimeSpan timeToGrowOneStepAfter,
            TimeSpan timeToAverageDieAfter, double deathTimeSpanFromAveragePart)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = DivideAngle;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = OneLengthPixels;
            _onePartPossitionDegrees = OnePartRelativePosition;
            _beginingAnglePhase = StartPart;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;

            _centerPoint = AbsoluteCenterPointLocation - new Size(this.Location);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = 1;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            changeLocation = new Point(0, 0);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = false;
            _branchLength = _zeroStateBranchOneLengthPixels = 0;
            _centerPointBelongsToBranch = false;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);

            //IGrowable
            _zeroStateOneLengthPixels = zeroStateOneLengthPixels;
            _onePartGrowOneLengthPixels = onePartGrowOneLengthPixels;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = timeToGrowOneStepAfter;
            _timeToAverageDieAfter = timeToAverageDieAfter;
            _deathTimeSpanFromAveragePart = deathTimeSpanFromAveragePart;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            LifeTimer.Start();
            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            _invertedBegining = false;
            doItselfRefresh();
        }
        public Leaf(Point PointFromWhereToGrowBranch, float BranchLngth, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
            float zeroStateOneLengthPixels, float onePartGrowOneLengthPixels, TimeSpan timeToGrowOneStepAfter,
            TimeSpan timeToAverageDieAfter, double deathTimeSpanFromAveragePart)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = DivideAngle;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = OneLengthPixels;
            _onePartPossitionDegrees = OnePartRelativePosition;
            _beginingAnglePhase = StartPart;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;
           
            _centerPoint = PointFromWhereToGrowBranch.Add(new Vector2(0, 1).Rotated(_rotationAngleRad) * BranchLngth) - new Size(this.Location);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = 1;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            changeLocation = PointFromWhereToGrowBranch.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = true;
            _branchLength = _zeroStateBranchOneLengthPixels = BranchLngth;
            _centerPointBelongsToBranch = true;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);

            //IGrowable
            _zeroStateOneLengthPixels = zeroStateOneLengthPixels;
            _onePartGrowOneLengthPixels = onePartGrowOneLengthPixels;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = timeToGrowOneStepAfter;
            _timeToAverageDieAfter = timeToAverageDieAfter;
            _deathTimeSpanFromAveragePart = deathTimeSpanFromAveragePart;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            LifeTimer.Start();
            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            _invertedBegining = false;
            doItselfRefresh();
        }
        public Leaf(Point PointFromWhereToGrowBranch, float BranchLngth, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
            float zeroStateOneLengthPixels, float onePartGrowOneLengthPixels, TimeSpan timeToGrowOneStepAfter,
            TimeSpan timeToAverageDieAfter, double deathTimeSpanFromAveragePart, float RotationRad)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = DivideAngle;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = OneLengthPixels;
            _onePartPossitionDegrees = OnePartRelativePosition;
            _beginingAnglePhase = StartPart;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;

            _centerPoint = PointFromWhereToGrowBranch.Add(new Vector2(0, 1).Rotated(_rotationAngleRad) * BranchLngth) - new Size(this.Location);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = 1;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            //changeLocation = PointFromWhereToGrowBranch.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = true;
            _branchLength = _zeroStateBranchOneLengthPixels = BranchLngth;
            _centerPointBelongsToBranch = true;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);
            _rotationAngleRad = RotationRad;

            //IGrowable
            _zeroStateOneLengthPixels = zeroStateOneLengthPixels;
            _onePartGrowOneLengthPixels = onePartGrowOneLengthPixels;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = timeToGrowOneStepAfter;
            _timeToAverageDieAfter = timeToAverageDieAfter;
            _deathTimeSpanFromAveragePart = deathTimeSpanFromAveragePart;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            LifeTimer.Start();
            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            _invertedBegining = false;
            doItselfRefresh();
        }

        public Leaf(Point PointFromWhereToGrowBranch, float BranchLngth, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
            float zeroStateOneLengthPixels, float onePartGrowOneLengthPixels, TimeSpan timeToGrowOneStepAfter,
            TimeSpan timeToAverageDieAfter, double deathTimeSpanFromAveragePart, float RotationRad, bool DrawToGraphics)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = DivideAngle;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = OneLengthPixels;
            _onePartPossitionDegrees = OnePartRelativePosition;
            _beginingAnglePhase = StartPart;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;

            _centerPoint = PointFromWhereToGrowBranch.Add(new Vector2(0, 1).Rotated(_rotationAngleRad) * BranchLngth) - new Size(this.Location);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = 1;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            //changeLocation = PointFromWhereToGrowBranch.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = true;
            _branchLength = _zeroStateBranchOneLengthPixels = BranchLngth;
            _centerPointBelongsToBranch = true;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);
            _rotationAngleRad = RotationRad;

            //IGrowable
            _zeroStateOneLengthPixels = zeroStateOneLengthPixels;
            _onePartGrowOneLengthPixels = onePartGrowOneLengthPixels;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = timeToGrowOneStepAfter;
            _timeToAverageDieAfter = timeToAverageDieAfter;
            _deathTimeSpanFromAveragePart = deathTimeSpanFromAveragePart;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            LifeTimer.Start();
            _drawToGraphics = DrawToGraphics;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            _invertedBegining = false;
            doItselfRefresh();
        }
        public Leaf(Point PointFromWhereToGrowBranch, float BranchLngth, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
            float zeroStateOneLengthPixels, float onePartGrowOneLengthPixels, int VeinsFractalisation, TimeSpan timeToGrowOneStepAfter,
            TimeSpan timeToAverageDieAfter, double deathTimeSpanFromAveragePart, float RotationRad, bool DrawToGraphics)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = DivideAngle;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = OneLengthPixels;
            _onePartPossitionDegrees = OnePartRelativePosition;
            _beginingAnglePhase = StartPart;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;

            _centerPoint = PointFromWhereToGrowBranch.Add(new Vector2(0, 1).Rotated(_rotationAngleRad) * BranchLngth) - new Size(this.Location);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = VeinsFractalisation;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            //changeLocation = PointFromWhereToGrowBranch.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = true;
            _branchLength = _zeroStateBranchOneLengthPixels = BranchLngth;
            _centerPointBelongsToBranch = true;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);
            _rotationAngleRad = RotationRad;

            //IGrowable
            _zeroStateOneLengthPixels = zeroStateOneLengthPixels;
            _onePartGrowOneLengthPixels = onePartGrowOneLengthPixels;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = timeToGrowOneStepAfter;
            _timeToAverageDieAfter = timeToAverageDieAfter;
            _deathTimeSpanFromAveragePart = deathTimeSpanFromAveragePart;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            LifeTimer.Start();
            _drawToGraphics = DrawToGraphics;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            _invertedBegining = false;
            doItselfRefresh();
        }

        public Leaf(Point PointFromWhereToGrowBranch, float BranchLngth, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
            float zeroStateOneLengthPixels, float onePartGrowOneLengthPixels, int VeinsFractalisation, TimeSpan timeToGrowOneStepAfter,
            TimeSpan timeToAverageDieAfter, double deathTimeSpanFromAveragePart, float RotationRad, bool invertedLeaf, bool invertedBegining, bool DrawToGraphics)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = DivideAngle;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = OneLengthPixels;
            _onePartPossitionDegrees = OnePartRelativePosition;
            _beginingAnglePhase = StartPart;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;

            _centerPoint = PointFromWhereToGrowBranch.Add(new Vector2(0, 1).Rotated(_rotationAngleRad) * BranchLngth) - new Size(this.Location);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = VeinsFractalisation;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            //changeLocation = PointFromWhereToGrowBranch.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = true;
            _branchLength = _zeroStateBranchOneLengthPixels = BranchLngth;
            _centerPointBelongsToBranch = true;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);
            _rotationAngleRad = RotationRad;
            _invertedLeaf = invertedLeaf;
            _invertedBegining = invertedBegining;

            //IGrowable
            _zeroStateOneLengthPixels = zeroStateOneLengthPixels;
            _onePartGrowOneLengthPixels = onePartGrowOneLengthPixels;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = timeToGrowOneStepAfter;
            _timeToAverageDieAfter = timeToAverageDieAfter;
            _deathTimeSpanFromAveragePart = deathTimeSpanFromAveragePart;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            LifeTimer.Start();
            _drawToGraphics = DrawToGraphics;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            
            doItselfRefresh();
        }

        /// <summary>
        /// As template for other components.
        /// </summary>
        /// <param name="PointFromWhereToGrowBranch"></param>
        /// <param name="BranchLngth"></param>
        /// <param name="DivideAngle"></param>
        /// <param name="StartPart"></param>
        /// <param name="OnePartRelativePosition"></param>
        /// <param name="OneLengthPixels"></param>
        /// <param name="zeroStateOneLengthPixels"></param>
        /// <param name="onePartGrowOneLengthPixels"></param>
        /// <param name="VeinsFractalisation"></param>
        /// <param name="timeToGrowOneStepAfter"></param>
        /// <param name="timeToAverageDieAfter"></param>
        /// <param name="deathTimeSpanFromAveragePart"></param>
        /// <param name="RotationRad"></param>
        /// <param name="invertedLeaf"></param>
        /// <param name="invertedBegining"></param>
        /// <param name="DrawToGraphics"></param>
        /// <param name="StartGrowing"></param>
        internal Leaf(Point PointFromWhereToGrowBranch, float BranchLngth, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
            float zeroStateOneLengthPixels, float onePartGrowOneLengthPixels, int VeinsFractalisation, TimeSpan timeToGrowOneStepAfter,
            TimeSpan timeToAverageDieAfter, double deathTimeSpanFromAveragePart, float RotationRad, bool invertedLeaf, bool invertedBegining, bool DrawToGraphics, bool StartGrowing)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = DivideAngle;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = OneLengthPixels;
            _onePartPossitionDegrees = OnePartRelativePosition;
            _beginingAnglePhase = StartPart;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;

            _centerPoint = PointFromWhereToGrowBranch.Add(new Vector2(0, 1).Rotated(_rotationAngleRad) * BranchLngth) - new Size(this.Location);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = VeinsFractalisation;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            //changeLocation = PointFromWhereToGrowBranch.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = true;
            _branchLength = _zeroStateBranchOneLengthPixels = BranchLngth;
            _centerPointBelongsToBranch = true;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);
            _rotationAngleRad = RotationRad;
            _invertedLeaf = invertedLeaf;
            _invertedBegining = invertedBegining;

            //IGrowable
            _zeroStateOneLengthPixels = zeroStateOneLengthPixels;
            _onePartGrowOneLengthPixels = onePartGrowOneLengthPixels;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = timeToGrowOneStepAfter;
            _timeToAverageDieAfter = timeToAverageDieAfter;
            _deathTimeSpanFromAveragePart = deathTimeSpanFromAveragePart;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            if (StartGrowing) LifeTimer.Start();
            _drawToGraphics = DrawToGraphics;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;

            doItselfRefresh();
        }

        internal Leaf(LineSegment rootLineSegment, float BranchLngthPixels, int DivideAngle, int StartPart, int OnePartRelativePosition,
            float onePartGrowOneLengthPixels, int VeinsFractalisation,
            double deathTimeSpanFromAveragePart, float RotationRad, bool invertedLeaf, bool invertedBegining, bool DrawToGraphics)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = DivideAngle;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = (rootLineSegment.Length - BranchLngthPixels) / (float)(Math.Pow(Phi, _divideAngle * (180 - _onePartPossitionDegrees - _beginingAnglePhase + 1) / 180F) + 1);
            _onePartPossitionDegrees = OnePartRelativePosition;
            _beginingAnglePhase = StartPart;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;
            
            //dopsat
            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle * (180 - _onePartPossitionDegrees - _beginingAnglePhase + 1) / 180F));
            _centerPoint = new Point(xCenter, yCenter);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = _vitalFillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = VeinsFractalisation;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            //changeLocation = (new Vector2(0 ,1).Rotated(RotationRad) * _oneLengthPixels * (float)Math.Pow(Phi, -(180F/_beginingAnglePhase))).ToPoint();
            //_locationBasedOnCenterPoint = true;
            _hasBranch = true;
            _branchLength = _zeroStateBranchOneLengthPixels = BranchLngthPixels;
            _centerPointBelongsToBranch = true;
            _branchPen = new Pen(_borderPen.Color, _centralVeinPixelThickness * 2);
            _rotationAngleRad = RotationRad;
            _invertedLeaf = invertedLeaf;
            _invertedBegining = invertedBegining;

            //IGrowable
            _zeroStateOneLengthPixels = _oneLengthPixels;
            _onePartGrowOneLengthPixels = onePartGrowOneLengthPixels;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = new TimeSpan(0, 10, 0);
            _timeToAverageDieAfter = new TimeSpan(0, 10, 0);
            _deathTimeSpanFromAveragePart = deathTimeSpanFromAveragePart;
            LifeTimer = new System.Windows.Forms.Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            LifeTimer.Start();
            _drawToGraphics = DrawToGraphics;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;

            this.RescaleOnLineSegment(rootLineSegment);
            
            doItselfRefresh();
        }
        #endregion

        #region Properties
        //private bool _locationBasedOnCenterPoint;
        //private Bitmap _panelBitmap;

        private bool _drawToGraphics;

        public Point AbsoluteCenterPointLocation
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

        private Point _centerPoint;
        public Point CenterPoint
        {
            get { return _centerPoint; }
            set
            {
                _centerPoint = value;
                _userEditedCenterPoint = true;
                doRefresh();
            }
        }
        private bool _userEditedCenterPoint;

        private bool _growFrom_CenterPoint;
        public bool GrowFrom_CenterPoint
        {
            get { return _growFrom_CenterPoint; }
            set
            {
                _growFrom_CenterPoint = value;
                doRefresh();
            }

        }

        private bool _accurateLogarithmicSpiral;
        public bool AccurateLogarithmicSpiral
        {
            get { return _accurateLogarithmicSpiral; }
            set
            {
                _accurateLogarithmicSpiral = value;
                if (!value) _invertedBegining = false;
                doRefresh();
            }
        }

        private float _oneLengthPixels;
        public float OneLengthPixels
        {
            get { return _oneLengthPixels; }
            set
            {
                _oneLengthPixels = value;
                doRefresh();
            }
        }

        private int _onePartPossitionDegrees;
        public int OnePartPossitinon
        {
            get { return _onePartPossitionDegrees; }
            set
            {
                if (value < 0) _onePartPossitionDegrees = 0;
                else if (value > _divideAngle) _onePartPossitionDegrees = _divideAngle;
                else _onePartPossitionDegrees = value;
                doRefresh();
            }
        }
        private int _beginingAnglePhase;
        public int BeginingAnglePhase
        {
            get { return _beginingAnglePhase; }
            set
            {
                if (value < 0) _beginingAnglePhase = 0;
                else if (value > 180) _beginingAnglePhase = 180;
                else _beginingAnglePhase = value;
                doRefresh();
            }
        }

        private bool _veins;
        public bool Veins
        {
            get { return _veins; }
            set
            {
                _veins = value;
                doRefresh();
            }
        }
        //private bool _veinsEvenFractalisation;
        //public bool VeinsEvenFractalisattion
        //{
        //    get { return _veinsEvenFractalisation; }
        //    set
        //    {
        //        _veinsEvenFractalisation = value;
        //        panelLeaf.Refresh();
        //    }
        //}
        private int _veinsFractalisation;
        public int VeinsFractalisation
        {
            get { return _veinsFractalisation; }
            set
            {
                _veinsFractalisation = value;
                doRefresh();
            }
        }
        private float _veinsBorderReachPart;
        public float VeinsBorderReachPart
        {
            get { return _veinsBorderReachPart; }
            set
            {
                _veinsBorderReachPart = value;
                doRefresh();
            }
        }
        private float _centralVeinPixelThickness;
        public float CentralVeinPixelThickness
        {
            get { return _centralVeinPixelThickness; }
            set
            {
                _centralVeinPixelThickness = value;
                doRefresh();
            }
        }

        private bool _fill;
        public bool Fill
        {
            get { return _fill; }
            set
            {
                _fill = value;
                doRefresh();
            }
        }

        public Color BorderColor
        {
            get { return _borderPen.Color; }
            set
            {
                _borderPen = new Pen(new SolidBrush(value), _borderPen.Width);
                doRefresh();
            }
        }

        private Pen _borderPen;
        public Pen BorderPen
        {
            get { return _borderPen; }
            set
            {
                _borderPen = new Pen(value.Brush, value.Width);
                doRefresh();
            }
        }

        public Color VitalFillColor
        {
            get { return _vitalFillBrush.Color; }
            set
            {
                _vitalFillBrush.Color = value;
                doRefresh();
            }
        }

        public Color CurrentFillColor
        {
            get { return _fillBrush.Color; }
            set
            {
                _fillBrush.Color = value;
                doRefresh();
            }
        }

        private SolidBrush _fillBrush;
        private SolidBrush _vitalFillBrush;
        public SolidBrush CurrentFillBrush
        {
            get { return _fillBrush; }
            set
            {
                _fillBrush = value;
                doRefresh();
            }
        }
        public SolidBrush VitalFillBrush
        {
            get { return _vitalFillBrush; }
            set
            {
                _vitalFillBrush = value;
                doRefresh();
            }
        }
        private Color _veinsColor;
        public Color VeinsColor
        {
            get { return _veinsColor; }
            set
            {
                _veinsColor = value;
                doRefresh();
            }
        }

        private float _leftCurveTension;
        public float LeftCurveTension
        {
            get { return _leftCurveTension; }
            set
            {
                _leftCurveTension = value;
                doRefresh();
            }
        }
        private float _rightCurveTension;
        public float RightCurveTension
        {
            get { return _rightCurveTension; }
            set
            {
                _rightCurveTension = value;
                doRefresh();
            }
        }

        private bool _curveBehindCenterPoint;
        public bool CurveBehindCenterPoint

        {
            get { return _curveBehindCenterPoint; }
            set
            {
                _curveBehindCenterPoint = value;
                doRefresh();
            }
        }

        private bool _smoothTop;
        public bool SmoothTop
        {
            get { return _smoothTop; }
            set
            {
                _smoothTop = value;
                doRefresh();
            }
        }

        private bool _invertedCurving;   
        public bool InvertedCurving
        {
            get { return _invertedCurving; }
            set
            {
                _invertedCurving = value;
                doRefresh();
            }
        }

        private bool _continueAfterInvertedCurving = true;
        public bool ContinueAfterInvertedCurving
        {
            get { return _continueAfterInvertedCurving; }
            set
            {
                _continueAfterInvertedCurving = value;
                doRefresh();
            }
        }

        private int _invertedCurvingCenterAngle;
        public int InvertedCurvingCenterAngle
        {
            get { return _invertedCurvingCenterAngle; }
            set
            {
                _invertedCurvingCenterAngle = value;
                doRefresh();
            }
        }

        private int _invertedCurvingSpan;
        public int InvertedCurvingSpan
        {
            get { return _invertedCurvingSpan; }
            set
            {
                _invertedCurvingSpan = value;
                doRefresh();
            }
        }

        private float _rotationAngleRad;
        public float RotationAngleRad
        {
            get { return _rotationAngleRad; }
            set
            {
                _rotationAngleRad = value;
                int xCenter = panelNature.Width / 2;
                int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle - _onePartPossitionDegrees));
                Vector2 centerOrigVect = new Vector2(xCenter - (panelNature.Width / 2), yCenter - (panelNature.Height / 2));
                Vector2 rotated = centerOrigVect.Rotated(_rotationAngleRad);
                //_centerPoint.X = (int)rotated.X + (panelLeaf.Width / 2);
                //_centerPoint.Y = (int)rotated.Y + (panelLeaf.Height / 2); 
                if (!_userEditedCenterPoint)
                {
                    CenterPoint = new Point((int)rotated.X + (panelNature.Width / 2), (int)rotated.Y + (panelNature.Height / 2));
                }
                doRefresh();
            }
        }

        private bool _isBilaterallySymetric;
        public bool IsBilaterallySymetric
        {
            get { return _isBilaterallySymetric; }
            set
            {
                if (value == true) _leftDivideAngle = _rightDivideAngle = _divideAngle;
                _isBilaterallySymetric = value;
                doRefresh();
            }
        }

        private int _divideAngle;
        public int DivideAngle
        {
            get { return _divideAngle; }
            set
            {
                if (_isBilaterallySymetric) _leftDivideAngle = _rightDivideAngle = value;
                _divideAngle = value;
                doRefresh();
            }
        }
        private int _leftDivideAngle;
        public int LeftDivideAngle
        {
            get { return _leftDivideAngle; }
            set
            {
                _isBilaterallySymetric = false;
                _leftDivideAngle = value;
                if (_isBilaterallySymetric) _rightDivideAngle = value;
                doRefresh();
            }
        }
        private int _rightDivideAngle;
        public int RightDivideAngle
        {
            get { return _rightDivideAngle; }
            set
            {
                _isBilaterallySymetric = false;
                _rightDivideAngle = value;
                if (_isBilaterallySymetric) _leftDivideAngle = value;
                doRefresh();
            }
        }

        // Branch
        private float _branchLength;
        public float BranchLength
        {
            get { return _branchLength; }
            set
            {
                if (value >= 0) _branchLength = value;
                else _branchLength = 0;
                doRefresh();
            }
        }

        private bool _hasBranch;
        public bool HasBranch
        {
            get { return _hasBranch; }
            set
            {
                _hasBranch = value;
                _centerPointBelongsToBranch = value;
                doRefresh();
            }
        }

        private bool _centerPointBelongsToBranch;

        private Pen _branchPen;
        public Pen BranchPen
        {
            get { return _branchPen; }
            set
            {
                _branchPen = new Pen(new SolidBrush(value.Color), value.Width);
                doRefresh();
            }
        }
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

        private Point getCenterPoint()
            => _growFrom_CenterPoint ?
                (_centerPointBelongsToBranch ?
                    _centerPoint.Add(new Vector2(0, -_branchLength * _oneLengthPixels).Rotated(_rotationAngleRad)) :
                    _centerPoint)
            : _centerPoint.Add(
                ((_invertedBegining && !_invertedLeaf ?
                    getInvertedBeginingPseudoCenterVector() :
                    new Vector2(0, -(1F - _onePartPossitionDegrees / 180F)).Rotated(_rotationAngleRad)
                ) + (_centerPointBelongsToBranch ?
                    new Vector2(0, -_branchLength).Rotated(_rotationAngleRad)
                    : new Vector2(0, 0))) * _oneLengthPixels);

        private Vector2 getInvertedBeginingPseudoCenterVector()
        {
            double beta = Math.Atan(Math.PI / DivideAngle);
            return (new Vector2(0, -1).Rotated(_rotationAngleRad)) *
                (float)((2 * Math.Pow(Phi, _divideAngle * (0.5 - beta / Math.PI)) * Math.Sin(beta)) - 1);
        }

        /*private Point getBranchOrigin(Point reachPoint)
        {

        }*/

        // Resizing in case of painting out of the control
        private void resizeToShowAll(Point[] CurvePoints, float Tension)
        {
            int xMin = 0;
            int xMax = 0;
            int yMin = 0;
            int yMax = 0;
            tempSize = this.Size;
            tempLocation = this.Location;
            Point[] curPts = CurvePoints.Union(new Point[1] { _centerPoint });
            foreach (Point p in curPts)
            {
                //Point p = CurvePoints[i];
                Point vect = p.Substract(_centerPoint);
                Point tensioned = _centerPoint.Add((1 + (0.1F + Tension)) * new Vector2(vect.X, vect.Y));
                if (tensioned.X < xMin) xMin = tensioned.X;
                if (tensioned.X > xMax) xMax = tensioned.X;
                if (tensioned.Y < yMin) yMin = tensioned.Y;
                if (tensioned.Y > yMax) yMax = tensioned.Y;
            }
            if (xMin < 0)
            {
                if (!_itselfResizedArr[0])
                {
                    int loc = (tempLocation.X + xMin >= 0) ? (tempLocation.X + xMin) : 0;
                    Point location = new Point(/*(loc < 0) ? 0 :*/ loc, tempLocation.Y);
                    Point subst = tempLocation.Substract(location);
                    tempSize = tempSize + new Size(subst);
                    tempLocation = location;
                    if (!itselfRefresh)
                    {
                        tempLocation = location;
                        tempSize = panelNature.Size = tempSize;
                    }
                    _centerPoint = _centerPoint.Add(subst);
                    _userEditedCenterPoint = true;
                    for (int j = 0; j < CurvePoints.Length; j++)
                    {
                        CurvePoints[j] = CurvePoints[j].Add(subst);
                    }
                    /*if (loc >= 0)*/ _itselfResizedArr[0] = true;
                }
            }
            if (xMax > this.Width)
            {
                if (!_itselfResizedArr[1])
                {
                    Size add = new Size(xMax - this.Width, 0);
                    //Point subst = tempLocation.Substract(location);
                    //tempLocation = location;
                    tempSize = tempSize + add;
                    if (!itselfRefresh)
                    {
                        tempSize = panelNature.Size = tempSize;
                    }
                    //tempSize = panelNature.Size = tempSize + add;
                    //_centerPoint = _centerPoint.Add(subst);
                    _userEditedCenterPoint = true;
                    _itselfResizedArr[1] = true;
                }
            }
            if (yMin < 0)
            {
                if (!_itselfResizedArr[2])
                {
                    int loc = (tempLocation.X + yMin >= 0) ? (tempLocation.X + yMin) : 0;
                    Point location = new Point(tempLocation.X, /*(loc < 0) ? 0 :*/ loc);
                    Point subst = tempLocation.Substract(location);
                    tempSize = tempSize + new Size(subst);
                    tempLocation = location;
                    if (!itselfRefresh)
                    {
                        tempLocation = location;
                        tempSize = panelNature.Size = tempSize;
                    }
                    _centerPoint = _centerPoint.Add(subst);
                    _userEditedCenterPoint = true;
                    for (int j = 0; j < CurvePoints.Length; j++)
                    {
                        CurvePoints[j] = CurvePoints[j].Add(subst);
                    }
                    /*if (loc >= 0)*/ _itselfResizedArr[2] = true;
                }
            }
            if (yMax > this.Height)
            {
                if (!_itselfResizedArr[3])
                {
                    Size add = new Size(0, yMax - this.Height);
                    //Point subst = tempLocation.Substract(location);
                    //tempLocation = location;
                    tempSize = tempSize + add;
                    if (!itselfRefresh)
                    {
                        tempSize = panelNature.Size = tempSize;
                    }
                    //_centerPoint = _centerPoint.Add(subst);
                    _userEditedCenterPoint = true;
                    _itselfResizedArr[3] = true;
                }
            }
            //_itself = new Bitmap(_itself, tempSize);
            if (!itselfRefresh) doItselfRefresh();
            else
            {

            }
            //return Task.CompletedTask;
        }

        private void Leaf_Resize(object sender, EventArgs e)
        {
            secondPaint = false;
            panelNature.Size = this.Size;
        }

        private void Leaf_Layout(object sender, LayoutEventArgs e)
        {
            secondPaint = false;
        }

        private void Leaf_Move(object sender, EventArgs e)
        {
            secondPaint = false;
        }

        public new void Refresh()
        {
            base.Refresh();
            doRefresh();
        }

#if DEBUG
        private bool _writtenProps = false;
#endif

        private void Leaf_Paint(object sender, PaintEventArgs e)
        {
            secondPaint = false;
            if (changeLocation != new Point(Int32.MaxValue, Int32.MaxValue))
            {
                this.Location = changeLocation;
                changeLocation = new Point(Int32.MaxValue, Int32.MaxValue);
            }

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
                    StreamWriter sw = new StreamWriter(new FileStream("Leaf.PropertiesToTranslate.txt", FileMode.Truncate));


                    foreach (PropertyInfo property in props)
                    {
                        if (property?.GetMethod?.IsPublic == true)
                            if ((ignoreProps.Where(new Func<PropertyInfo, bool>((proprt) => property.Name == proprt.Name)).ToArray().Length) == 0)
                            {
                                sw.WriteLine(property.Name + "=\"\"");
                            }
                    }
                    sw.Close();
                }
                catch { }
            }
#endif
            //panelLeaf_Paint(sender, e);
        }
        private Point changeLocation = new Point(Int32.MaxValue, Int32.MaxValue);

        private bool _propertiesEditingMode = false;
        public bool PropertiesEditingMode
        {
            get { return _propertiesEditingMode; }
            set
            {
                _propertiesEditingMode = value;
                if (value) doRefresh();
            }
        }
        

        private bool secondPaint;
        private void doRefresh()
        {
            if (!_propertiesEditingMode)
            {
                secondPaint = false;
                _itselfResized = false;
                validPaint = true;
                panelNature.Refresh();
            }
        }

        bool validPaint = true; // Because of repeated random paint event
        private async void panelLeaf_Paint(object sender, PaintEventArgs e)
        {
            if (validPaint) await doPanelPaint(e);
            validPaint = false;
            //Task.Run(await doPanelPaint(e))            
        }
        private Task doPanelPaint(PaintEventArgs e)
        {
            if (!secondPaint)
            {
                if (Itself == null) doItselfRefresh();
                //panelNature.SuspendLayout();
                itselfRefresh = true;
                doItselfRefresh();
                if (_drawToGraphics)
                {
                    Graphics g = e.Graphics;
                    g.DrawImage(Itself, 0, 0);
                }
                secondPaint = true;
                //panelNature.ResumeLayout();
            }
            return Task.CompletedTask;
        }

        private bool _itselfResized
        {
            get
            {
                return _itselfResizedArr[0] || _itselfResizedArr[1] || _itselfResizedArr[2] || _itselfResizedArr[3];
            }
            set
            {
                _itselfResizedArr = new bool[4] { value, value, value, value }; 
            }
        }
        private bool[] _itselfResizedArr = new bool[4] { false, false, false, false };

        private void doPaint(Graphics graphics) 
        {
            //panelLeaf.DrawToBitmap(_panelBitmap, panelLeaf.ClientRectangle);
            if (!_userEditedCenterPoint)
            {
                int xCenter = panelNature.Width / 2;
                int yCenter = Convert.ToInt32(_oneLengthPixels * (Math.Pow(Phi, _divideAngle - _onePartPossitionDegrees - _beginingAnglePhase + 1) + _branchLength));
                _centerPoint = new Point(xCenter, yCenter);
            }

            if (!_smoothTop)
            {
                Point[] LPoints = LeafCurvePoints(true);
                Point[] RPoints = LeafCurvePoints(false);
                if (!_itselfResized)
                {
                    resizeToShowAll(LPoints.Union(RPoints), (_leftCurveTension > _rightCurveTension) ? _leftCurveTension : _rightCurveTension);
                    //resizeToShowAll(RPoints, _rightCurveTension);
                }                
            }
            else
            {
                Point[] curvePoints = RightCurvePoints.ReversedPointsWithoutFirst();
                float tension = (_leftCurveTension + _rightCurveTension) / 2;
                if (!_itselfResized)
                {
                    resizeToShowAll(curvePoints, tension);
                }
            }

            if (_itselfResized && !secondBitmapDraw)
            {
                //_itselfResized = false;
                secondBitmapDraw = true;
                doItselfRefresh();
                graphics.DrawImage(Itself, 0, 0);
                secondBitmapDraw = false;
            }
            else
            {
                //Point[] LPoints = LeafCurvePoints(true);

                //float tension = 0.5F;
                if (!_smoothTop)
                {
                    Point[] LPoints = LeafCurvePoints(true);
                    Point[] RPoints = LeafCurvePoints(false);
                                        
                    if (_fill)
                    {
                        GraphicsPath gp = new GraphicsPath();
                        gp.AddCurve(LPoints, _leftCurveTension);
                        gp.AddCurve(RPoints, _rightCurveTension);

                        Region reg = new Region(gp);

                        graphics.FillRegion(_fillBrush, reg);
                        reg.Dispose();
                        gp.Dispose();
                    }

                    graphics.DrawCurve(_borderPen, LPoints, _leftCurveTension);
                    graphics.DrawCurve(_borderPen, RPoints, _rightCurveTension);

                    if (_hasBranch)
                    {
                        Point branchBegining = LPoints[_invertedLeaf ? LPoints.Length - 1 : 0];
                        graphics.DrawLine(_branchPen, _centerPoint, branchBegining);
                    }
                }
                else
                {
                    Point[] curvePoints = RightCurvePoints.ReversedPointsWithoutFirst();
                    Point[] unified = LeftCurvePoints.Union(curvePoints);
                    float tension = (_leftCurveTension + _rightCurveTension) / 2;

                    //resizeToShowAll(curvePoints, tension);

                    graphics.DrawCurve(_borderPen, unified, tension);

                    if (_fill)
                    {
                        
                        graphics.FillClosedCurve(_fillBrush, unified, FillMode.Alternate, tension);
                    }

                    if (_hasBranch)
                    {
                        Point branchBegining = _invertedLeaf ? unified[unified.Length - curvePoints.Length - 1] : unified[0];
                        graphics.DrawLine(_branchPen, _centerPoint, branchBegining);
                    }
                }                

                //Bitmap bmp = new Bitmap(panelLeaf.Width, panelLeaf.Height);
                //using (var g = Graphics.FromImage(bmp))
                //{
                //    g.DrawClosedCurve(Pens.AliceBlue, RightCurvePoints);
                //    g.FillRectangle(Brushes.AliceBlue, 0, 0, 200, 200);
                //}
                //graphics.DrawImage(bmp, 0, 0);

                //Color col = bmp.GetPixel(RightCurvePoints[0].X, RightCurvePoints[0].Y);
                //Color alicbl = Color.FromKnownColor(KnownColor.AliceBlue);
                if (_veins)
                {
                    // NEW Syntax - using FractalisingVeinLineSegment struct -> WORKING
                    //Curve LeftCur = new Curve(LeftCurvePoints, LeftCurveTension);
                    //Curve RightCur = new Curve(RightCurvePoints, RightCurveTension);
                    FractalisingVeinLineSegment veins = FractalisingVeinLineSegment.OnLeaf(this, (_beginingAnglePhase / 180F * Math.PI) < GoldenAngleRad, (float)Math.PI - (float)Generals.GoldenAngleRad);

                    veins.DrawToGraphics(ref graphics);
                    // OLD Syntax
                    //Point[] centrVeinCurPts = CentralVeinCurvePoints;
                    //graphics.DrawCurve(new Pen(_veinsColor, _centralVeinPixelThickness), centrVeinCurPts);
                    //if (_veinsFractalisation > 0)
                    //{
                        // OLD Syntax - Without using FractalisingVeinLineSegment struct

                        /*
                        int LEVEL = 1;
                        //List<Point> points = new List<Point>();
                        //Bitmap curve = new Bitmap(panelLeaf.Width, panelLeaf.Height);
                        //for (int x = 0; x < panelLeaf.Width; x++)
                        //{
                        //    for (int y = 0; )
                        //}

                        // OLD SYNTAX

                        // Left side
                        //Point[] LeftStartPoints; // = new Point[_leftDivideAngle / 2];
                        //Point[] LeftEndPoints = new Point[(_leftDivideAngle / 2) - (_leftDivideAngle % 2) - _beginingAnglePhase + 1];
                        //float[] LeftCentralPointParts = new float[(_leftDivideAngle / 2) - (_leftDivideAngle % 2) - _beginingAnglePhase + 1];
                        //Point[] RightStartPoints; // = new Point[_rightDivideAngle / 2];
                        //Point[] RightEndPoints = new Point[(_rightDivideAngle / 2) - (_rightDivideAngle % 2) - _beginingAnglePhase + 1];
                        //float[] RightCentralPointParts = new float[(_rightDivideAngle / 2) - (_rightDivideAngle % 2) - _beginingAnglePhase + 1];

                        ////LeftStartPoints
                        //float onepart = 2F / _leftDivideAngle;
                        //float onepartHalved = onepart / 2;
                        //for (int i = 0; i < (_leftDivideAngle / 2) - (_leftDivideAngle % 2) - _beginingAnglePhase + 1; i++)
                        //{
                        //    LeftCentralPointParts[i] = onepartHalved + (i * onepart);
                        //}
                        //LeftStartPoints = CurvePartPoints(0.5F, centrVeinCurPts, LeftCentralPointParts);

                        ////LeftEndPoints
                        //Point[] LPoints = LeafCurvePoints(true);
                        //for (int i = 0; i < (_leftDivideAngle / 2) - (_leftDivideAngle % 2) - _beginingAnglePhase + 1; i++)
                        //{
                        //    LeftEndPoints[i] = LPoints[i + _leftDivideAngle - (_leftDivideAngle / 2) + (_leftDivideAngle % 2) - _beginingAnglePhase + 2];
                        //}

                        ////RightStartPoints
                        //onepart = 2F / _rightDivideAngle;
                        //onepartHalved = onepart / 2;
                        //for (int i = 0; i < (_rightDivideAngle / 2) - (_rightDivideAngle % 2) - _beginingAnglePhase + 1; i++)
                        //{
                        //    RightCentralPointParts[i] = onepartHalved + (i * onepart);
                        //}
                        //RightStartPoints = CurvePartPoints(0.5F, centrVeinCurPts, RightCentralPointParts);

                        ////RightEndPoints
                        //Point[] RPoints = LeafCurvePoints(false);
                        //for (int i = 0; i < (_rightDivideAngle / 2) - (_rightDivideAngle % 2) - _beginingAnglePhase + 1; i++)
                        //{
                        //    RightEndPoints[i] = RPoints[i + _rightDivideAngle - (_rightDivideAngle / 2) + (_rightDivideAngle % 2) - _beginingAnglePhase + 2];
                        //}

                        //Pen Veinpen = new Pen(_veinsColor, 2F / (1 + LEVEL));
                        ////Left minor veins
                        //for (int i = 0; i < (_leftDivideAngle / 2) - (_leftDivideAngle % 2) - _beginingAnglePhase + 1; i++)
                        //{
                        //    graphics.DrawPartLine(Veinpen, LeftStartPoints[i], LeftEndPoints[i], _veinsBorderReachPart);
                        //}

                        ////Right minor veins
                        //for (int i = 0; i < (_rightDivideAngle / 2) - (_rightDivideAngle % 2) - _beginingAnglePhase + 1; i++)
                        //{
                        //    graphics.DrawPartLine(Veinpen, RightStartPoints[i], RightEndPoints[i], _veinsBorderReachPart);
                        //}

                        // NEW SYNTAX

                        float Angle = Convert.ToSingle(Math.PI - GoldenAngleRad);

                        // Arrays
                        Point[] LeftStartPoints; // = new Point[_leftDivideAngle / 2];
                        Point[] LeftEndPoints = new Point[_leftDivideAngle];
                        float[] LeftCentralPointParts = new float[_leftDivideAngle];
                        Point[] RightStartPoints; // = new Point[_rightDivideAngle / 2];
                        Point[] RightEndPoints = new Point[_rightDivideAngle];
                        float[] RightCentralPointParts = new float[_rightDivideAngle];

                        List<LineSegment> _lineSegments = new List<LineSegment>();
                        LineSegment[] LineSegments;

                        // Central Curve
                        Curve CenterCurve = new Curve(centrVeinCurPts);

                        //LeftStartPoints
                        float onepart = 1F / (_leftDivideAngle + 1);
                        float onepartHalved = onepart / 2;
                        for (int i = 0; i < _leftDivideAngle; i++)
                        {
                            LeftCentralPointParts[i] = onepartHalved + (i * onepart);
                        }
                        LeftStartPoints = CurvePartPoints(0.5F, centrVeinCurPts, LeftCentralPointParts);

                        //LeftEndPoints
                        Curve LCurve = new Curve(LeafCurvePoints(true), _leftCurveTension);
                        //Point[] LPoints = LCurve.GetDrawnPixelsPoints();
                        Line vein;
                        for (int i = 0; i < _leftDivideAngle; i++)
                        {
                            //int index = Convert.ToInt32((onepartHalved + (i * onepart) + (5 / LPoints.Length)) * LPoints.Length - 1);                        
                            //Line vein = new Line(LeftStartPoints[i], LPoints[index]);
                            //LeftEndPoints[i] = LPoints[i + _leftDivideAngle - (_leftDivideAngle / 2) + (_leftDivideAngle % 2) - _beginingAnglePhase + 1].ToPoint();
                            //bool inters;
                            vein = CenterCurve.LineFromPointOn(LeftStartPoints[i]).Rotated(LeftStartPoints[i], -Angle);
                            LeftEndPoints[i] = vein.Intersect(LCurve);
                            _lineSegments.Add(new LineSegment(LeftStartPoints[i], LeftEndPoints[i]));
                        }

                        //RightStartPoints
                        onepart = 1F / (_rightDivideAngle + 1);
                        onepartHalved = onepart / 2;
                        for (int i = 0; i < _rightDivideAngle; i++)
                        {
                            RightCentralPointParts[i] = onepartHalved + (i * onepart);
                        }
                        RightStartPoints = CurvePartPoints(0.5F, centrVeinCurPts, RightCentralPointParts);

                        //RightEndPoints
                        Curve RCurve = new Curve(LeafCurvePoints(false), _rightCurveTension);
                        //Point[] RPoints = RCurve.CurvePoints;
                        for (int i = 0; i < _rightDivideAngle; i++)
                        {
                            //int index = Convert.ToInt32((onepartHalved + (i * onepart) + (5 / RPoints.Length)) * RPoints.Length - 1);
                            //Line vein = new Line(RightStartPoints[i], RPoints[index]);
                            //LeftEndPoints[i] = LPoints[i + _leftDivideAngle - (_leftDivideAngle / 2) + (_leftDivideAngle % 2) - _beginingAnglePhase + 1].ToPoint();
                            //bool inters;
                            vein = CenterCurve.LineFromPointOn(RightStartPoints[i]).Rotated(RightStartPoints[i], Angle);
                            RightEndPoints[i] = vein.Intersect(RCurve);
                            _lineSegments.Add(new LineSegment(RightStartPoints[i], RightEndPoints[i]));
                        }

                        LineSegments = _lineSegments.ToArray();

                        // DRAWING
                        Pen Veinpen = new Pen(_veinsColor, 2F / (1 + LEVEL));
                        ////Left minor veins
                        //for (int i = 0; i < (_leftDivideAngle / 2) - (_leftDivideAngle % 2); i++)
                        //{
                        //    graphics.DrawPartLine(Veinpen, LeftStartPoints[i], LeftEndPoints[i], _veinsBorderReachPart);
                        //}

                        ////Right minor veins
                        //for (int i = 0; i < (_rightDivideAngle / 2) - (_rightDivideAngle % 2); i++)
                        //{
                        //    graphics.DrawPartLine(Veinpen, RightStartPoints[i], RightEndPoints[i], _veinsBorderReachPart);
                        //}
                        graphics.DrawPartLineSegments(Veinpen, LineSegments, _veinsBorderReachPart);

                        LEVEL++;

                        // More fractalized
                        while (LEVEL <= _veinsFractalisation)
                        {
                            _lineSegments.Clear();
                            LineSegment[] moved;
                            foreach (LineSegment segment in LineSegments)
                            {
                                moved = LineSegments.RotatedAndRescaledVeinsLineSegments(CenterCurve, segment);
                                foreach (LineSegment sgm in moved)
                                    _lineSegments.Add(sgm);
                            }
                            LineSegments = _lineSegments.ToArray();

                            Veinpen = new Pen(_veinsColor, 2F / (1 + LEVEL));
                            graphics.DrawPartLineSegments(Veinpen, LineSegments, _veinsBorderReachPart);

                            LEVEL++;
                        }*/

                        
                    //}
                }

                _itselfResized = false;
            }
            //return Task.CompletedTask;
            //Redraw((Leaf)this, EventArgs.Empty);
        }

        internal Curve LeftCurve
        {
            get { return new Curve(LeafCurvePoints(true)); }
        }

        internal Curve RightCurve
        {
            get { return new Curve(LeafCurvePoints(false)); }
        }

        private bool _hasBeenAccurate; // Pro návrat k přesné
        private bool _invertedBegining;
        public bool InvertedBegining
        {
            get { return _invertedBegining; }
            set
            {                
                if (value)
                {
                    _hasBeenAccurate = _accurateLogarithmicSpiral;
                    _accurateLogarithmicSpiral = value;
                }
                else if (_invertedBegining)
                {
                    _accurateLogarithmicSpiral = _hasBeenAccurate; // Pro návrat k přesné
                }
                _invertedBegining = value;
                doRefresh();
            }
        }

        private bool _invertedLeaf;
        public bool InvertedLeaf
        {
            get { return _invertedLeaf; }
            set
            {
                _invertedLeaf = value;
                doRefresh();
            }
        }

        internal void RescaleOnLineSegment(LineSegment root)
        {
            float rootLngth = root.Length;
            float ratio;
            int divAngle = (_leftDivideAngle > _rightDivideAngle) ? _leftDivideAngle : _rightDivideAngle;

            if (!_invertedBegining)
                ratio = _branchLength / (float)(
                    (_invertedCurving ?
                        Math.Pow(Phi, divAngle * (180 - _invertedCurvingSpan - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F) :
                        Math.Pow(Phi, divAngle * (180 - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F))
                    + Math.Pow(Phi, divAngle * (-_onePartPossitionDegrees) / 180F)) * _oneLengthPixels;
            else
                ratio = _branchLength / (float)(
                    (_invertedCurving ?
                        Math.Pow(Phi, divAngle * (180 - _invertedCurvingSpan - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F) :
                        Math.Pow(Phi, divAngle * (180 - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F))
                    + Math.Pow(Phi, divAngle * (0.5 - (Math.Atan(Math.PI / (divAngle * LNPhi))) - (_onePartPossitionDegrees / 180F)))
                    - Math.Pow(Phi, divAngle * (-_onePartPossitionDegrees) / 180F)) * _oneLengthPixels;

            _alreadyGrownState = 0;
            _oneLengthPixels = _zeroStateOneLengthPixels = rootLngth / (1 + ratio);
            _branchLength = _zeroStateBranchOneLengthPixels = rootLngth / (1 + ratio) * ratio;

            _rotationAngleRad = new Vector2(0, 1).AngleBetween(-root.DirectionVector);

            this.Location = root.PartPoint(ratio).Substract(_centerPoint);

            doRefresh();
        }

        private Point[] LeafCurvePoints(bool isLeft)
        {
            if (_invertedBegining) return _leafCurvePoints_WithInvertedBegining(isLeft);
            else return _leafCurvePoints_NormalAndInvertedLeaf(isLeft);
        }

        private Point[] _leafCurvePoints_NormalAndInvertedLeaf(bool isLeft)
        {
            Point[] res;
            Vector2 startVector = new Vector2(0, 1).Rotated(_rotationAngleRad);
            Point center = getCenterPoint();

            int divAngle = (isLeft ? _leftDivideAngle : _rightDivideAngle);

            if (_invertedLeaf)
            {
                startVector = -startVector; // vektor je opačný    
                isLeft = !isLeft; // opačná strana
                
                if (_accurateLogarithmicSpiral)
                {
                    // startVector je již invertován
                    center = center.Add(startVector * ((float)Math.Pow(Phi, divAngle * (180 - _onePartPossitionDegrees) / 180F)) * _oneLengthPixels);
                    // i v případě, že je někde invertované křivení, bude stejně dlouhý, jelikož již je tak počítán
                }
                else
                {
                    //Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180)
                    center = center.Add(startVector * ((float)Math.Pow(Phi, divAngle - Convert.ToSingle(divAngle * _onePartPossitionDegrees / 180F))) * _oneLengthPixels);
                }
            }

            int angleRotationSign = isLeft ? 1 : -1; // kvůli znaménku rotace

            if (_accurateLogarithmicSpiral)
            {
                res = new Point[181]; //(isLeft ? _leftDivideAngle : _rightDivideAngle) - _beginingAnglePhase + 1];

                if (!_invertedCurving)
                {
                    //if (isLeft) // levá
                    //{
                        // teď se přičítá a v zápětí odečítá kvůli nulovému bodu -> počátečnímu bodu
                        for (int i = 0; i <= 180 - _beginingAnglePhase; i++)
                        {
                            if (i + _beginingAnglePhase != _onePartPossitionDegrees || _onePartPossitionDegrees < _beginingAnglePhase) // pokud není na pozici jednotkové délky
                            {
                                if (i == 0) // počáteční body -> musí se do res až do indexu [_beginingAnglePhase] přidat počáteční body
                                {

                                    if (!_curveBehindCenterPoint) res[0] = center;
                                    else res[0] = center.Add(startVector * (float)Math.Pow(Phi, divAngle * (_onePartPossitionDegrees /*+ _beginingAnglePhase*/) / 180F) * _oneLengthPixels);
                                    if (_beginingAnglePhase > 0)
                                    {
                                        center = res[0];
                                        Point toReach = center.Add(startVector.Rotated(angleRotationSign * _beginingAnglePhase / 180F * (float)Math.PI) * (float)Math.Pow(Phi, divAngle * (-_onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                                        Point[] partpts = new LineSegment(center, toReach).PartPoints(_beginingAnglePhase + 1, true);
                                        for (int j = 1; j <= _beginingAnglePhase; j++)
                                        {
                                            res[j] = partpts[j];
                                            //if (!_curveBehindCenterPoint) res[j] = center;
                                            //else res[j] = center.Add(startVector * (float)Math.Pow(Phi, divAngle * (-_onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                                        }
                                    }
                                }
                                else if (i + _beginingAnglePhase != 180) res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(angleRotationSign * (i + _beginingAnglePhase) / 180F * (float)Math.PI) * (float)Math.Pow(Phi, divAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                                else res[180] = center.Add(-startVector * (float)Math.Pow(Phi, divAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                            }
                            else // pokud je na pozici jednotkové délky
                            {
                                res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(angleRotationSign * (i + _beginingAnglePhase) / 180F * (float)Math.PI) * _oneLengthPixels);
                            }
                        }
                    //}
                    //else // pravá
                    //{
                    //    for (int i = 0; i <= 180 - _beginingAnglePhase; i++)
                    //    {
                    //        if (i + _beginingAnglePhase != _onePartPossitionDegrees || _onePartPossitionDegrees < _beginingAnglePhase) // pokud není na pozici jednotkové délky
                    //        {
                    //            if (i == 0)
                    //            {
                    //                //Point center = getCenterPoint();
                    //                if (!_curveBehindCenterPoint) res[0] = center;
                    //                else res[0] = center.Add(startVector * (float)Math.Pow(Phi, divAngle * (_onePartPossitionDegrees /*+ _beginingAnglePhase*/) / 180F) * _oneLengthPixels);
                    //                if (_beginingAnglePhase > 0)
                    //                {
                    //                    center = res[0];
                    //                    Point toReach = center.Add(startVector.Rotated(-_beginingAnglePhase / 180F * (float)Math.PI) * (float)Math.Pow(Phi, divAngle * (-_onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                    //                    Point[] partpts = new LineSegment(center, toReach).PartPoints(_beginingAnglePhase + 1, true);
                    //                    for (int j = 1; j <= _beginingAnglePhase; j++)
                    //                    {
                    //                        res[j] = partpts[j];
                    //                        //if (!_curveBehindCenterPoint) res[j] = center;
                    //                        //else res[j] = center.Add(startVector * (float)Math.Pow(Phi, divAngle * (-_onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                    //                    }
                    //                }
                    //            }
                    //            else if (i + _beginingAnglePhase != 180) res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase) / 180F * (float)Math.PI) * (float)Math.Pow(Phi, _rightDivideAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                    //            else res[180] = center.Add(-startVector * (float)Math.Pow(Phi, _rightDivideAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                    //        }
                    //        else // pokud je na pozici jednotkové délky
                    //        {
                    //            res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase) / 180F * (float)Math.PI) * _oneLengthPixels);
                    //        }
                    //    }
                    //}
                }
                else
                {
                    //if (isLeft)
                    //{
                        for (int i = 0; i <= 180 - _beginingAnglePhase; i++)
                        {
                            if (i <= _invertedCurvingCenterAngle - _invertedCurvingSpan - _beginingAnglePhase + 1) // pokud je před inverzí
                            {
                                if (i + _beginingAnglePhase != _onePartPossitionDegrees || _onePartPossitionDegrees < _beginingAnglePhase) // pokud není na pozici jednotkové délky
                                {
                                    if (i == 0) // počáteční body -> musí se do res až do indexu [_beginingAnglePhase] přidat počáteční body
                                    {
                                        //Point center = getCenterPoint();
                                        if (!_curveBehindCenterPoint) res[0] = center;
                                        else res[0] = center.Add(startVector * (float)Math.Pow(Phi, divAngle * (_onePartPossitionDegrees /*+ _beginingAnglePhase*/) / 180F) * _oneLengthPixels);
                                        if (_beginingAnglePhase > 0)
                                        {
                                            center = res[0];
                                            Point toReach = center.Add(startVector.Rotated(angleRotationSign * _beginingAnglePhase / 180F * (float)Math.PI) * (float)Math.Pow(Phi, divAngle * (-_onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                                            Point[] partpts = new LineSegment(center, toReach).PartPoints(_beginingAnglePhase + 1, true);
                                            for (int j = 1; j <= _beginingAnglePhase; j++)
                                            {
                                                res[j] = partpts[j];
                                                //if (!_curveBehindCenterPoint) res[j] = center;
                                                //else res[j] = center.Add(startVector * (float)Math.Pow(Phi, divAngle * (-_onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                                            }
                                        }
                                    }
                                    else if (i + _beginingAnglePhase != 180) res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(angleRotationSign * (i + _beginingAnglePhase) / 180F * (float)Math.PI) * (float)Math.Pow(Phi, divAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                                    else res[180] = center.Add(-startVector * (float)Math.Pow(Phi, divAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                                }
                                else
                                {
                                    res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(angleRotationSign * (i + _beginingAnglePhase) / 180F * (float)Math.PI) * _oneLengthPixels);
                                }
                            }
                            else // při inverzi
                            {
                                if (i <= _invertedCurvingCenterAngle - _beginingAnglePhase) res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(angleRotationSign * (i + _beginingAnglePhase) / 180F * (float)Math.PI) * (float)Math.Pow(Phi, divAngle * (_invertedCurvingCenterAngle - _invertedCurvingSpan - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                                else //if (i <= _invertedCurvingCenterAngle + _invertedCurvingSpan)
                                {
                                    res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(angleRotationSign * (i + _beginingAnglePhase) / 180F * (float)Math.PI) * (float)Math.Pow(Phi, divAngle * (i - _invertedCurvingSpan - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                                }
                            }
                        }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i <= 180 - _beginingAnglePhase; i++)
                    //    {
                    //        if (i <= _invertedCurvingCenterAngle - _invertedCurvingSpan - _beginingAnglePhase + 1) // pokud je před inverzí
                    //        {
                    //            if (i + _beginingAnglePhase != _onePartPossitionDegrees || _onePartPossitionDegrees < _beginingAnglePhase) // pokud není na pozici jednotkové délky
                    //            {
                    //                if (i == 0)
                    //                {
                    //                    //Point center = getCenterPoint();
                    //                    if (!_curveBehindCenterPoint) res[0] = center;
                    //                    else res[0] = center.Add(startVector * (float)Math.Pow(Phi, divAngle * (_onePartPossitionDegrees /*+ _beginingAnglePhase*/) / 180F) * _oneLengthPixels);
                    //                    if (_beginingAnglePhase > 0)
                    //                    {
                    //                        center = res[0];
                    //                        Point toReach = center.Add(startVector.Rotated(-_beginingAnglePhase / 180F * (float)Math.PI) * (float)Math.Pow(Phi, divAngle * (-_onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                    //                        Point[] partpts = new LineSegment(center, toReach).PartPoints(_beginingAnglePhase + 1, true);
                    //                        for (int j = 1; j <= _beginingAnglePhase; j++)
                    //                        {
                    //                            res[j] = partpts[j];
                    //                            //if (!_curveBehindCenterPoint) res[j] = center;
                    //                            //else res[j] = center.Add(startVector * (float)Math.Pow(Phi, divAngle * (-_onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                    //                        }
                    //                    }
                    //                }
                    //                else if (i + _beginingAnglePhase != 180) res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase) / 180F * (float)Math.PI) * (float)Math.Pow(Phi, _rightDivideAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                    //                else res[180] = center.Add(-startVector * (float)Math.Pow(Phi, _rightDivideAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                    //            }
                    //            else
                    //            {
                    //                res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase) / 180F * (float)Math.PI) * _oneLengthPixels);
                    //            }
                    //        }
                    //        else // při inverzi
                    //        {
                    //            if (i <= _invertedCurvingCenterAngle - _beginingAnglePhase) res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase) / 180F * (float)Math.PI) * (float)Math.Pow(Phi, divAngle * (_invertedCurvingCenterAngle - _invertedCurvingSpan - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                    //            else //if (i <= _invertedCurvingCenterAngle + _invertedCurvingSpan)
                    //            {
                    //                res[i + _beginingAnglePhase] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase) / 180F * (float)Math.PI) * (float)Math.Pow(Phi, _rightDivideAngle * (i - _invertedCurvingSpan - _onePartPossitionDegrees + _beginingAnglePhase) / 180F) * _oneLengthPixels);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            else
            {
                // OLD Syntax - not accurate logarithmic spiral
                float partAngle = (float)Math.PI / (isLeft ? divAngle : _rightDivideAngle);

                int i_beginingAnglePhase = Convert.ToInt32(_beginingAnglePhase * (isLeft ? divAngle : _rightDivideAngle) / 180);

                int plusOneBegAngPhase = i_beginingAnglePhase > 0 ? (i_beginingAnglePhase != _beginingAnglePhase ? 1 : 0) : 0;
                int plusTwoBegAngPhase = i_beginingAnglePhase > 0 ? 2 * plusOneBegAngPhase : 0;
                res = new Point[(isLeft ? divAngle : _rightDivideAngle) - i_beginingAnglePhase + plusOneBegAngPhase + 1];

                if (!_invertedCurving)
                {                    
                    //if (isLeft)
                    //{
                        if (i_beginingAnglePhase > 0 && plusOneBegAngPhase > 0)
                        {
                            res[1] = center.Add(startVector.Rotated(angleRotationSign * _beginingAnglePhase * (float)Math.PI / 180F) * _oneLengthPixels);
                        }
                        for (int i = 0; i <= divAngle - i_beginingAnglePhase + plusOneBegAngPhase; i++)
                        {
                            if (i != Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) || i == 0)
                            {
                                if (i == 0)
                                {
                                    if (!_curveBehindCenterPoint || i_beginingAnglePhase != 0) res[0] = center;
                                    else res[0] = center.Add(startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - plusOneBegAngPhase) * _oneLengthPixels);
                                }
                                else if ((i == 1 && plusOneBegAngPhase == 0) || i > 1)
                                {                               
                                    if (i + i_beginingAnglePhase - plusTwoBegAngPhase != divAngle) res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - plusOneBegAngPhase) * partAngle) * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - plusOneBegAngPhase) * _oneLengthPixels);
                                    else res[i] = center.Add(-startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180)) * _oneLengthPixels);
                                }
                            }
                            else
                            {
                                res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - 1) * partAngle) * _oneLengthPixels);
                            }
                        }
                    //}
                    //else
                    //{

                    //    //for (int i = 0; i <= _rightDivideAngle - i_beginingAnglePhase + plusOneBegAngPhase; i++)
                    //    //{
                    //    //    if (i != Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180))
                    //    //    {
                    //    //        if (i == 0)
                    //    //        {
                    //    //            if (_curveBehindCenterPoint) res[0] = center;
                    //    //            else res[0] = center.Add(startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                    //    //        }
                    //    //        else if (i + i_beginingAnglePhase - 1 != _rightDivideAngle) res[i] = center.Add(startVector.Rotated(-(i + i_beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                    //    //        else res[i] = center.Add(-startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        res[i] = center.Add(startVector.Rotated(-(i + i_beginingAnglePhase - 1) * partAngle) * _oneLengthPixels);
                    //    //    }
                    //    //}

                    //    if (i_beginingAnglePhase > 0 && plusOneBegAngPhase > 0)
                    //    {
                    //        res[1] = center.Add(startVector.Rotated(-_beginingAnglePhase * (float)Math.PI / 180F) * _oneLengthPixels);
                    //    }
                    //    for (int i = 0; i <= _rightDivideAngle - i_beginingAnglePhase + plusOneBegAngPhase; i++)
                    //    {
                    //        if (i != Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) || i == 0)
                    //        {
                    //            if (i == 0)
                    //            {
                    //                if (!_curveBehindCenterPoint || i_beginingAnglePhase != 0) res[0] = center;
                    //                else res[0] = center.Add(startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - plusOneBegAngPhase) * _oneLengthPixels);
                    //            }
                    //            else if ((i == 1 && plusOneBegAngPhase == 0) || i > 1)
                    //            {
                    //                if (i + i_beginingAnglePhase - plusTwoBegAngPhase != _rightDivideAngle) res[i] = center.Add(startVector.Rotated(-(i + i_beginingAnglePhase - plusOneBegAngPhase) * partAngle) * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - plusOneBegAngPhase) * _oneLengthPixels);
                    //                else res[i] = center.Add(-startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180)) * _oneLengthPixels);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            res[i] = center.Add(startVector.Rotated(-(i + i_beginingAnglePhase - 1) * partAngle) * _oneLengthPixels);
                    //        }
                    //    }
                    //}
                }
                else
                {
                    //if (isLeft)
                    //{
                        for (int i = 0; i <= divAngle - i_beginingAnglePhase + plusOneBegAngPhase; i++)
                        {
                            if (i <= _invertedCurvingCenterAngle - _invertedCurvingSpan - i_beginingAnglePhase + plusOneBegAngPhase + 1)
                            {
                                if (i != Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180))
                                {
                                    if (i == 0)
                                    {
                                        if (_curveBehindCenterPoint) res[0] = center;
                                        else res[0] = center.Add(startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                                    }
                                    else if (i != divAngle) res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                                    else res[i] = center.Add(-startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                                }
                                else
                                {
                                    res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - 1) * partAngle) * _oneLengthPixels);
                                }
                            }
                            else
                            {
                                if (i <= _invertedCurvingCenterAngle - i_beginingAnglePhase + 1) res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, _invertedCurvingCenterAngle - _invertedCurvingSpan - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                                else //if (i <= _invertedCurvingCenterAngle + _invertedCurvingSpan)
                                {
                                    res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - _invertedCurvingSpan - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                                }
                            }
                        }
                    //}
                    //else
                    //{
                    //    for (int i = 0; i <= _rightDivideAngle - i_beginingAnglePhase + 1; i++)
                    //    {
                    //        if (i <= _invertedCurvingCenterAngle - _invertedCurvingSpan - i_beginingAnglePhase + 2)
                    //        {
                    //            if (i != Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180))
                    //            {
                    //                if (i == 0)
                    //                {
                    //                    if (_curveBehindCenterPoint) res[0] = center;
                    //                    else res[0] = center.Add(startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                    //                }
                    //                else if (i != _rightDivideAngle) res[i] = center.Add(startVector.Rotated(-(i + i_beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                    //                //zde byla chyba: divAngle namísto i
                    //                else res[i] = center.Add(-startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                    //            }
                    //            else
                    //            {
                    //                res[i] = center.Add(startVector.Rotated(-(i + i_beginingAnglePhase - 1) * partAngle) * _oneLengthPixels);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (i <= _invertedCurvingCenterAngle - i_beginingAnglePhase + 1) res[i] = center.Add(startVector.Rotated(-(i + i_beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, _invertedCurvingCenterAngle - _invertedCurvingSpan - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                    //            else //if (i <= _invertedCurvingCenterAngle + _invertedCurvingSpan)
                    //            {
                    //                res[i] = center.Add(startVector.Rotated(-(i + i_beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - _invertedCurvingSpan - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            return res;

        }

        private Point[] _leafCurvePoints_WithInvertedBegining(bool isLeft)
        {
            Point[] res;
            Vector2 startVector = new Vector2(0, 1).Rotated(_rotationAngleRad);
            Point center = getCenterPoint();

            int divAngle = isLeft ? _leftDivideAngle : _rightDivideAngle;

            if (_invertedLeaf)
            {
                startVector = -startVector; // vektor je opačný    
                isLeft = !isLeft; // opačná strana
                //divAngle = (isLeft ? _leftDivideAngle : _rightDivideAngle);
                if (_accurateLogarithmicSpiral)
                {
                    // startVector je již invertován
                    center = center.Add(startVector * ((float)Math.Pow(Phi, divAngle * (180 - _onePartPossitionDegrees) / 180F)) * _oneLengthPixels);
                    // i v případě, že je někde invertované křivení, bude stejně dlouhý, jelikož již je tak počítán
                }
                else
                {
                    //Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180)
                    center = center.Add(startVector * ((float)Math.Pow(Phi, divAngle - Convert.ToSingle(divAngle * _onePartPossitionDegrees / 180F))) * _oneLengthPixels);
                }
            }

            int angleRotationSign = isLeft ? 1 : -1;
            
            float beta = (float)(Math.Atan(Math.PI / (divAngle * LNPhi))); // Úhel pro tečnu kolmou k ose listu

            float phiToNSinBeta = (float)(Math.Pow(Phi, divAngle * (0.5 - (beta / Math.PI))) * Math.Sin(beta)); // Pro zrychlení výpočtů (aby se neprováděly dvakrát)
            Point invertedBegining = new Point(
                center.X - Convert.ToInt32(2 * _oneLengthPixels * Math.Sin(_rotationAngleRad + (_invertedLeaf ? Math.PI : 0)) * phiToNSinBeta),
                center.Y + Convert.ToInt32(2 * _oneLengthPixels * Math.Cos(_rotationAngleRad + (_invertedLeaf ? Math.PI : 0)) * phiToNSinBeta));

            //beta *= AngleSign; // Nyní už se řeší znaménko úhlu

            float PiHalfMinusBeta = (float)((Math.PI / 2) - beta);

            float radI; // úhel ve foru na radiány

            if (_accurateLogarithmicSpiral)
            {
                res = new Point[181]; //(isLeft ? _leftDivideAngle : _rightDivideAngle) - _beginingAnglePhase + 2];

                if (!_invertedCurving)
                {
                    for (int i = 0; i <= 180; i++)
                    {
                        radI = RAD(i);
                        if (radI <= PiHalfMinusBeta) // Invertovaný počátek
                        {
                            res[i] = new Point(
                                invertedBegining.X + Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, divAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F) * Math.Sin(_rotationAngleRad + (_invertedLeaf ? Math.PI : 0) - angleRotationSign * radI)),
                                invertedBegining.Y - Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, divAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F) * Math.Cos(_rotationAngleRad + (_invertedLeaf ? Math.PI : 0) - angleRotationSign * radI)));
                        }

                        else
                        {
                            res[i] = center.Add(startVector.Rotated(radI * angleRotationSign) * (float)Math.Pow(Phi, divAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F) * _oneLengthPixels);
                        }
                    }
                }
                else // prohlubňovitá křivka
                {
                    for (int i = 0; i <= 180; i++)
                    {
                        radI = RAD(i);
                        if (i <= _invertedCurvingCenterAngle - _invertedCurvingSpan + 1) // pokud je před inverzí
                        {
                            if (radI <= PiHalfMinusBeta) // Invertovaný počátek
                            {
                                res[i] = new Point(
                                    invertedBegining.X + Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, divAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F) * Math.Sin(_rotationAngleRad - angleRotationSign * radI)),
                                    invertedBegining.Y - Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, divAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F) * Math.Cos(_rotationAngleRad - angleRotationSign * radI)));
                            }

                            else
                            {
                                res[i] = center.Add(startVector.Rotated(radI * angleRotationSign) * (float)Math.Pow(Phi, divAngle * (i - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F) * _oneLengthPixels);
                            }
                        }
                        else // při inverzi
                        {
                            if (i <= _invertedCurvingCenterAngle) res[i] = center.Add(startVector.Rotated(radI * angleRotationSign) * (float)Math.Pow(Phi, _leftDivideAngle * (_invertedCurvingCenterAngle - _invertedCurvingSpan - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F) * _oneLengthPixels);
                            else //if (i <= _invertedCurvingCenterAngle + _invertedCurvingSpan)
                            {
                                res[i] = center.Add(startVector.Rotated(radI * angleRotationSign) * (float)Math.Pow(Phi, _leftDivideAngle * (i - _invertedCurvingSpan - _onePartPossitionDegrees + _beginingAnglePhase - 1) / 180F) * _oneLengthPixels);
                            }
                        }
                    }
                }
            }
            else
            {
                // OLD Syntax - not accurate logarithmic spiral
                float partAngle = (float)Math.PI / (isLeft ? divAngle : _rightDivideAngle);

                int i_beginingAnglePhase = Convert.ToInt32(_beginingAnglePhase * (isLeft ? divAngle : _rightDivideAngle) / 180);

                int plusOneBegAngPhase = i_beginingAnglePhase > 0 ? (i_beginingAnglePhase != _beginingAnglePhase ? 1 : 0) : 0;
                int plusTwoBegAngPhase = i_beginingAnglePhase > 0 ? 2 * plusOneBegAngPhase : 0;
                res = new Point[(isLeft ? divAngle : _rightDivideAngle) - i_beginingAnglePhase + plusOneBegAngPhase + 1];

                if (!_invertedCurving)
                {
                    if (i_beginingAnglePhase > 0 && plusOneBegAngPhase > 0)
                    {
                        res[1] = center.Add(startVector.Rotated(angleRotationSign * _beginingAnglePhase * (float)Math.PI / 180F) * _oneLengthPixels);
                    }
                    for (int i = 0; i <= divAngle - i_beginingAnglePhase + plusOneBegAngPhase; i++)
                    {
                        if (i != Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) || i == 0)
                        {
                            if (i == 0)
                            {
                                if (!_curveBehindCenterPoint || i_beginingAnglePhase != 0) res[0] = center;
                                else res[0] = center.Add(startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - plusOneBegAngPhase) * _oneLengthPixels);
                            }
                            else if ((i == 1 && plusOneBegAngPhase == 0) || i > 1)
                            {
                                if (i + i_beginingAnglePhase - plusTwoBegAngPhase != divAngle) res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - plusOneBegAngPhase) * partAngle) * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - plusOneBegAngPhase) * _oneLengthPixels);
                                else res[i] = center.Add(-startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180)) * _oneLengthPixels);
                            }
                        }
                        else
                        {
                            res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - 1) * partAngle) * _oneLengthPixels);
                        }
                    }                    
                }
                else
                {                   
                    for (int i = 0; i <= divAngle - i_beginingAnglePhase + 1; i++)
                    {
                        if (i <= _invertedCurvingCenterAngle - _invertedCurvingSpan - i_beginingAnglePhase + 2)
                        {
                            if (i != Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180))
                            {
                                if (i == 0)
                                {
                                    if (_curveBehindCenterPoint) res[0] = center;
                                    else res[0] = center.Add(startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                                }
                                else if (i != divAngle) res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                                else res[i] = center.Add(-startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                            }
                            else
                            {
                                res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - 1) * partAngle) * _oneLengthPixels);
                            }
                        }
                        else
                        {
                            if (i <= _invertedCurvingCenterAngle - i_beginingAnglePhase + 1) res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, _invertedCurvingCenterAngle - _invertedCurvingSpan - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                            else //if (i <= _invertedCurvingCenterAngle + _invertedCurvingSpan)
                            {
                                res[i] = center.Add(startVector.Rotated(angleRotationSign * (i + i_beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - _invertedCurvingSpan - Convert.ToInt32(divAngle * _onePartPossitionDegrees / 180) + i_beginingAnglePhase - 1) * _oneLengthPixels);
                            }
                        }
                    }
                }

                //// OLD Syntax - not accurate logarithmic spiral
                //float partAngle = (float)Math.PI / _divideAngle;

                //if (!_invertedCurving)
                //{
                //    res = new Point[(isLeft ? _leftDivideAngle : _rightDivideAngle) - _beginingAnglePhase + 2];

                //    if (isLeft)
                //    {
                //        for (int i = 0; i <= _leftDivideAngle - _beginingAnglePhase + 1; i++)
                //        {
                //            if (i != Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180))
                //            {
                //                if (i == 0)
                //                {
                //                    if (_curveBehindCenterPoint) res[0] = center;
                //                    else res[0] = center.Add(startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                }
                //                else if (i + _beginingAnglePhase - 1 != _leftDivideAngle) res[i] = center.Add(startVector.Rotated((i + _beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                else res[i] = center.Add(-startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //            }
                //            else
                //            {
                //                res[i] = center.Add(startVector.Rotated((i + _beginingAnglePhase - 1) * partAngle) * _oneLengthPixels);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        for (int i = 0; i <= _rightDivideAngle - _beginingAnglePhase + 1; i++)
                //        {
                //            if (i != Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180))
                //            {
                //                if (i == 0)
                //                {
                //                    if (_curveBehindCenterPoint) res[0] = center;
                //                    else res[0] = center.Add(startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                }
                //                else if (i + _beginingAnglePhase - 1 != _rightDivideAngle) res[i] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                else res[i] = center.Add(-startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //            }
                //            else
                //            {
                //                res[i] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase - 1) * partAngle) * _oneLengthPixels);
                //            }
                //        }
                //    }
                //}
                //else // Prohlubně
                //{
                //    res = new Point[(isLeft ? _leftDivideAngle : _rightDivideAngle) - _beginingAnglePhase + 4]; // 2 (?) + 2 pro prohlubně pro přesnost 

                //    if (isLeft)
                //    {
                //        for (int i = 0; i <= _leftDivideAngle - _beginingAnglePhase + 1; i++)
                //        {
                //            if (i <= _invertedCurvingCenterAngle - _invertedCurvingSpan - _beginingAnglePhase + 2)
                //            {
                //                if (i != Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180))
                //                {
                //                    if (i == 0)
                //                    {
                //                        if (_curveBehindCenterPoint) res[0] = center;
                //                        else res[0] = center.Add(startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                    }
                //                    else if (i != _leftDivideAngle) res[i] = center.Add(startVector.Rotated((i + _beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                    else res[i] = center.Add(-startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                }
                //                else
                //                {
                //                    res[i] = center.Add(startVector.Rotated((i + _beginingAnglePhase - 1) * partAngle) * _oneLengthPixels);
                //                }
                //            }
                //            else
                //            {
                //                if (i <= _invertedCurvingCenterAngle - _beginingAnglePhase + 1) res[i] = center.Add(startVector.Rotated((i + _beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, _invertedCurvingCenterAngle - _invertedCurvingSpan - Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                else //if (i <= _invertedCurvingCenterAngle + _invertedCurvingSpan)
                //                {
                //                    res[i] = center.Add(startVector.Rotated((i + _beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - _invertedCurvingSpan - Convert.ToInt32(_leftDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                }
                //            }
                //        }
                //    }
                //    else
                //    {
                //        for (int i = 0; i <= _rightDivideAngle - _beginingAnglePhase + 1; i++)
                //        {
                //            if (i <= _invertedCurvingCenterAngle - _invertedCurvingSpan - _beginingAnglePhase + 2)
                //            {
                //                if (i != Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180))
                //                {
                //                    if (i == 0)
                //                    {
                //                        if (_curveBehindCenterPoint) res[0] = center;
                //                        else res[0] = center.Add(startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                    }
                //                    else if (i != _rightDivideAngle) res[i] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                    //zde byla chyba: _leftDivideAngle namísto i
                //                    else res[i] = center.Add(-startVector * (float)Math.Pow(Phi, i - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                }
                //                else
                //                {
                //                    res[i] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase - 1) * partAngle) * _oneLengthPixels);
                //                }
                //            }
                //            else
                //            {
                //                if (i <= _invertedCurvingCenterAngle - _beginingAnglePhase + 1) res[i] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, _invertedCurvingCenterAngle - _invertedCurvingSpan - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                else //if (i <= _invertedCurvingCenterAngle + _invertedCurvingSpan)
                //                {
                //                    res[i] = center.Add(startVector.Rotated(-(i + _beginingAnglePhase - 1) * partAngle) * (float)Math.Pow(Phi, i - _invertedCurvingSpan - Convert.ToInt32(_rightDivideAngle * _onePartPossitionDegrees / 180) + _beginingAnglePhase - 1) * _oneLengthPixels);
                //                }
                //            }
                //        }
                //    }
                //}
            }
            return res;
        }
        /*private Point[] LeafCurvePoints(bool isLeft, float distancePart)
        {
            Point[] res = new Point[(isLeft ? _leftDivideAngle : _rightDivideAngle) + 1];
            Vector2 startVector = new Vector2(0, 1).Rotated(_rotationAngleRad);
            float partAngle = (float)Math.PI / _divideAngle;

            if (!_invertedCurving)
            {
                if (isLeft)
                {
                    for (int i = 0; i <= _leftDivideAngle; i++)
                    {
                        if (i != _onePartPossitionDegrees)
                        {
                            if (i == 0)
                            {
                                if (_curveBehindCenterPoint) res[0] = getCenterPoint();
                                else res[0] = getCenterPoint().Add(startVector * (float)Math.Pow(Phi, i - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                            }
                            else if (i != _leftDivideAngle) res[i] = getCenterPoint().Add(startVector.Rotated(i * partAngle) * (float)Math.Pow(Phi, i - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                            else res[i] = getCenterPoint().Add(-startVector * (float)Math.Pow(Phi, i - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                        }
                        else
                        {
                            res[i] = getCenterPoint().Add(startVector.Rotated(i * partAngle) * _oneLengthPixels * distancePart);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i <= _rightDivideAngle; i++)
                    {
                        if (i != _onePartPossitionDegrees)
                        {
                            if (i == 0)
                            {
                                if (_curveBehindCenterPoint) res[0] = getCenterPoint();
                                else res[0] = getCenterPoint().Add(startVector * (float)Math.Pow(Phi, i - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                            }
                            else if (i != _rightDivideAngle) res[i] = getCenterPoint().Add(startVector.Rotated(-i * partAngle) * (float)Math.Pow(Phi, i - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                            else res[i] = getCenterPoint().Add(-startVector * (float)Math.Pow(Phi, _leftDivideAngle - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                        }
                        else
                        {
                            res[i] = getCenterPoint().Add(startVector.Rotated(-i * partAngle) * _oneLengthPixels * distancePart);
                        }
                    }
                }
            }
            else
            {
                if (isLeft)
                {
                    for (int i = 0; i <= _leftDivideAngle; i++)
                    {
                        if (i <= _invertedCurvingCenterAngle - _invertedCurvingSpan + 1)
                        {
                            if (i != _onePartPossitionDegrees)
                            {
                                if (i == 0)
                                {
                                    if (_curveBehindCenterPoint) res[0] = getCenterPoint();
                                    else res[0] = getCenterPoint().Add(startVector * (float)Math.Pow(Phi, i - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                                }
                                else if (i != _leftDivideAngle) res[i] = getCenterPoint().Add(startVector.Rotated(i * partAngle) * (float)Math.Pow(Phi, i - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                                else res[i] = getCenterPoint().Add(-startVector * (float)Math.Pow(Phi, i - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                            }
                            else
                            {
                                res[i] = getCenterPoint().Add(startVector.Rotated(i * partAngle) * _oneLengthPixels * distancePart);
                            }
                        }
                        else
                        {
                            if (i <= _invertedCurvingCenterAngle) res[i] = getCenterPoint().Add(startVector.Rotated(i * partAngle) * (float)Math.Pow(Phi, _invertedCurvingCenterAngle - _invertedCurvingSpan - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                            else //if (i <= _invertedCurvingCenterAngle + _invertedCurvingSpan)
                            {
                                res[i] = getCenterPoint().Add(startVector.Rotated(i * partAngle) * (float)Math.Pow(Phi, i - _invertedCurvingSpan - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i <= _rightDivideAngle; i++)
                    {
                        if (i <= _invertedCurvingCenterAngle - _invertedCurvingSpan + 1)
                        {
                            if (i != _onePartPossitionDegrees)
                            {
                                if (i == 0)
                                {
                                    if (_curveBehindCenterPoint) res[0] = getCenterPoint();
                                    else res[0] = getCenterPoint().Add(startVector * (float)Math.Pow(Phi, i - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                                }
                                else if (i != _rightDivideAngle) res[i] = getCenterPoint().Add(startVector.Rotated(-i * partAngle) * (float)Math.Pow(Phi, i - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                                else res[i] = getCenterPoint().Add(-startVector * (float)Math.Pow(Phi, _leftDivideAngle - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                            }
                            else
                            {
                                res[i] = getCenterPoint().Add(startVector.Rotated(-i * partAngle) * _oneLengthPixels * distancePart);
                            }
                        }
                        else
                        {
                            if (i <= _invertedCurvingCenterAngle) res[i] = getCenterPoint().Add(startVector.Rotated(-i * partAngle) * (float)Math.Pow(Phi, _invertedCurvingCenterAngle - _invertedCurvingSpan - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                            else //if (i <= _invertedCurvingCenterAngle + _invertedCurvingSpan)
                            {
                                res[i] = getCenterPoint().Add(startVector.Rotated(-i * partAngle) * (float)Math.Pow(Phi, i - _invertedCurvingSpan - _onePartPossitionDegrees) * _oneLengthPixels * distancePart);
                            }
                        }
                    }
                }
            }

            return res;
        }*/
        private Point[] LeftCurvePoints { get { return LeafCurvePoints(true); } }
        private Point[] RightCurvePoints { get { return LeafCurvePoints(false); } }
        private Point[] CentralVeinCurvePoints
        {
            get
            {
                Point[] left = LeftCurvePoints;
                Point[] right = RightCurvePoints;

                int lenght = (_accurateLogarithmicSpiral) ? 181 : (((_leftDivideAngle < _rightDivideAngle) ? _rightDivideAngle + 1 : _leftDivideAngle + 1) + (_invertedCurving ? 2 : 0));
                Point[] res = new Point[lenght];
                res[0] = getCenterPoint();
                for (int i = 0; i < lenght; i++)
                {
                    Point origPoint = CentralPoint(left[i], right[i]);
                    //res[i + 1] = getCenterPoint().Add(new Vector2(origPoint.X - getCenterPoint().X, origPoint.Y - getCenterPoint().Y) * _veinsBorderReachPart);
                }
                return res;

                //if (_leftDivideAngle == _rightDivideAngle)
                //{
                //    int length = (_leftDivideAngle + 2 - _beginingAnglePhase + ((_leftDivideAngle - _beginingAnglePhase + 2) % 2)) / 2;
                //    Point[] res = new Point[length + 1];
                //    res[0] = getCenterPoint();
                //    for (int i = 0; i < length; i++)
                //    {
                //        Point origPoint = CentralPoint(left[_leftDivideAngle - length + 2 + i - _beginingAnglePhase], right[_rightDivideAngle - length + 2 + i - _beginingAnglePhase]);
                //        res[i + 1] = getCenterPoint().Add(new Vector2(origPoint.X - getCenterPoint().X, origPoint.Y - getCenterPoint().Y) * _veinsBorderReachPart);
                //    }
                //    return res;
                //}
                //else
                //{
                //    throw new NotImplementedException();
                //}
            }

        }


        #region IGrowable Implementation
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
                doRefresh();
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
                doRefresh();
            }
        }

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
                doRefresh();
            }
        }

        //private TimeSpan _growOneStepTime;
        //public TimeSpan GrowOneStepTime
        //{
        //    get
        //    {
        //        return _growOneStepTime;
        //    }

        //    set
        //    {
        //        _growOneStepTime = value;
        //    }
        //}

        private TimeSpan _currentTimeAfterLastGrowth;
        public TimeSpan CurrentTimeAfterLastGrowth
        {
            get
            {
                return _currentTimeAfterLastGrowth;
            }
            private set
            {
                if (value < _timeToGrowOneStepAfter)
                {
                    _currentTimeAfterLastGrowth = value;
                }
                else
                {
                    _currentTimeAfterLastGrowth = value - _timeToGrowOneStepAfter;
                    GrowOneStep();
                }
            }
        }

        private bool _isDead;
        public bool IsDead
        {
            get
            {
                return _isDead;
            }
            set
            {
                if (_isDead && !value)
                {
                    Revive();
                }                
                if (!_isDead && value)
                {
                    Die();
                }
            }
        }

        //private Point[] CentralVeinCurveToPoints()
        //{

        //}

        public enum GrowthType
        {
            Arithmetic,
            Geometric
        }

        private GrowthType _branchGrowthType;
        public GrowthType BranchGrowthType
        {
            get { return _branchGrowthType; }
            set { _branchGrowthType = value; }
        }

        private GrowthType _leafGrowthType;
        public GrowthType LeafGrowthType
        {
            get { return _leafGrowthType; }
            set { _leafGrowthType = value; }
        }

        private float _zeroStateBranchOneLengthPixels;
        public float ZeroStateBranchOneLengthPixels
        {
            get { return _zeroStateBranchOneLengthPixels; }
            set { _zeroStateBranchOneLengthPixels = value; }
        }

        public void GrowOneStep()
        {
            if (!_isDead)
            {
                _alreadyGrownState++;
                if (_leafGrowthType == GrowthType.Arithmetic) _oneLengthPixels = _zeroStateOneLengthPixels + (_alreadyGrownState * _onePartGrowOneLengthPixels);
                else _oneLengthPixels = _zeroStateOneLengthPixels * (float)Math.Pow(_onePartGrowOneLengthPixels, _alreadyGrownState);
                float oneBranchGrow = _onePartGrowOneLengthPixels * _zeroStateOneLengthPixels / _zeroStateBranchOneLengthPixels;
                if (_branchGrowthType == GrowthType.Arithmetic) _branchLength = _zeroStateBranchOneLengthPixels + _alreadyGrownState * oneBranchGrow;
                else _branchLength = _zeroStateBranchOneLengthPixels * (float)Math.Pow(oneBranchGrow, _alreadyGrownState);
                doItselfRefresh();
                doRefresh();
            }            
        }

        public void GrowPart(float part)
        {
            if (!_isDead)
            {
                _oneLengthPixels *= (1 + part);
                _zeroStateOneLengthPixels *= (1 + part);
                //_onePartGrowOneLengthPixels *= (1 + part);
                _branchLength *= 1 + part;
                _zeroStateBranchOneLengthPixels *= 1 + part;
                doRefresh();
            }
        }

        public void StopGrowing()
        {
            LifeTimer.Stop();
        }

        public void Die()
        {
            //panelLeaf.Paint -= panelLeaf_Paint;
            //panelLeaf.Paint += (object sender, PaintEventArgs e) => { };
            _isDead = true;
            _fillBrush = new SolidBrush(Color.Beige);
            _veinsColor = _veinsColor.CombineWith(Color.Beige, 4F);
            LifeTimer.Stop();
            doRefresh();
        }

        public void Revive()
        {
            _isDead = false;
            _fillBrush = _vitalFillBrush;
            _veinsColor = _veinsColor.DecombineWith(Color.Beige, 4F);
            LifeTimer.Start();
            doRefresh();
        }

        public void LifeTimerTickHandler(object sender, EventArgs e)
        {
            CurrentTimeAfterLastGrowth += new TimeSpan(0, 0, 0, 0, LifeTimer.Interval);
        }
        
        private TimeSpan _timeToGrowOneStepAfter;
        public TimeSpan TimeToGrowOneStepAfter
        {
            get
            {
                return _timeToGrowOneStepAfter;
            }

            set
            {
                _timeToGrowOneStepAfter = value;
            }
        }

        private TimeSpan _timeToAverageDieAfter;
        public TimeSpan TimeToAverageDieAfter
        {
            get
            {
                return _timeToAverageDieAfter;
            }

            set
            {
                _timeToAverageDieAfter = value;
            }
        }

        private double _deathTimeSpanFromAveragePart;
        public double DeathTimeSpanFromAveragePart
        {
            get
            {
                return _deathTimeSpanFromAveragePart;
            }

            set
            {
                _deathTimeSpanFromAveragePart = value;
            }
        }

        public System.Windows.Forms.Timer LifeTimer
        {
            get;
            set;
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

        private void delegRdrw(object sender, EventArgs e) { }
        public event RedrawEventHandler Redraw;

        private bool itselfRefresh;
        private bool secondBitmapDraw = false;
        private Bitmap _itself;
        public Bitmap Itself
        {
            get
            {
                //if (itselfRefresh)
                //{
                //    _itself?.Dispose();
                //    _itself = new Bitmap(panelNature.Width, panelNature.Height);
                //    _itself.MakeTransparent();
                //    _itselfResized = false;
                //    doPaint(Graphics.FromImage(_itself));
                //    _itselfResized = false;
                //    itselfRefresh = false;
                //}

                return _itself;
            }            
        }
        public Bitmap GetItselfBitmap()
        {
            return Itself;
        }
        //private bool doingItselfRefresh = false;
        Bitmap tempBitmap = new Bitmap(1, 1);
        Size tempSize;
        Point tempLocation;
        //Bitmap _tempBitmapWhileRedrawing;
        private void doItselfRefresh()
        {
            if (tempLocation != null) this.Location = tempLocation;
            if (tempSize != null) this.Size = tempSize;
            
            tempBitmap = new Bitmap(tempSize.Width, tempSize.Height);
            tempBitmap.MakeTransparent();
            //_itself?.Dispose();
            
            //_itself.MakeTransparent();
            _itselfResized = false;
            itselfRefresh = true;
            doPaint(Graphics.FromImage(tempBitmap));
            _itselfResized = false;
            itselfRefresh = false;

            this.Size = panelNature.Size = tempSize;
            this.Location = tempLocation;

            
            
            _itself = tempBitmap;
        }

        //public Panel PanelNature
        #endregion

        public void ThrowRedrawException()
        {
            throw new RedrawException(UniqueIDToRedrawException());
        }

        public string UniqueIDToRedrawException()
            => this.ToString() + Convert.ToString(this.GetHashCode());

        #region Mathematically Derivated Properties
        /// <summary>
        /// Area of the leaf if it is made of an ideal logarithmic spiral on both sides.
        /// </summary>
        public double IdealArea
        {
            get
            {
                if (_leftDivideAngle == _rightDivideAngle)
                    return Math.Pow(Phi, 2 * _divideAngle) * Math.PI / (2 * _divideAngle * Generals.LNPhi) * _oneLengthPixels * _oneLengthPixels;
                else return Math.Pow(Phi, 2 * _leftDivideAngle) * Math.PI / (4 * _leftDivideAngle * Generals.LNPhi) * _oneLengthPixels * _oneLengthPixels +
                        Math.Pow(Phi, 2 * _rightDivideAngle) * Math.PI / (4 * _rightDivideAngle * Generals.LNPhi) * _oneLengthPixels * _oneLengthPixels;
            }
        }

        /// <summary>
        /// Tangent angle of leaf's left curve. It means angle between radius line and tangent and is constant from properties of logarithmic spiral.
        /// </summary>
        public double IdealTangentAngleLeft
        {
            get { return Math.Atan(Math.PI / (_leftDivideAngle * Generals.LNPhi)); }
        }

        /// <summary>
        /// Tangent angle of leaf's right curve. It means angle between radius line and tangent and is constant from properties of logarithmic spiral.
        /// </summary>
        public double IdealTangentAngleRight
        {
            get { return Math.Atan(Math.PI / (_rightDivideAngle * Generals.LNPhi)); }
        }
        #endregion
    }
}