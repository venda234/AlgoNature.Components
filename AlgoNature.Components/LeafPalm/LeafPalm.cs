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
using static AlgoNature.Components.Generals;
using static AlgoNature.Components.Geometry;
//using System.Timers;

namespace AlgoNature.Components
{
    public partial class LeafPalm : UserControl, IGrowableGraphicChild, IToRedrawEventHandlingList //IObservable<bool>
    {
        #region Constructors

        public LeafPalm()
        {
            InitializeComponent();
            _leafChildTemplate = new Leaf();

            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle));
            _centerPoint = new Point(xCenter, yCenter);

            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            tempSize = this.Size;

            //IGrowable
            _zeroStateOneLengthPixels = 0.5F;
            _onePartGrowOneLengthPixels = 0.5F;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = new TimeSpan(0, 0, 10);
            _timeToAverageDieAfter = new TimeSpan(0, 5, 0);
            _deathTimeSpanFromAveragePart = 0.1;
            LifeTimer = new Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            //LifeTimer.Start();
            //this.Refresh();

            doItselfRefresh();
        }
        public LeafPalm(bool Growing)
        {
            InitializeComponent();
            _leafChildTemplate = new Leaf(Growing);

            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle));
            _centerPoint = new Point(xCenter, yCenter);
            _rotationAngleRad = 0;
            _oneLengthPixels = 3;
            _branchPen = new Pen(Color.DarkGreen, _oneLengthPixels * 2);

            //IGrowable
            _zeroStateOneLengthPixels = 0.5F;
            _onePartGrowOneLengthPixels = 0.5F;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = new TimeSpan(0, 0, 10);
            _timeToAverageDieAfter = new TimeSpan(0, 5, 0);
            _deathTimeSpanFromAveragePart = 0.1;
            LifeTimer = new Timer();
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

            _leafChildTemplate = new Leaf(false);

            doItselfRefresh();
        }
        public LeafPalm(Point AbsoluteCenterPoint)
        {
            InitializeComponent();
            secondPaint = false;
            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle));
            _centerPoint = new Point(xCenter, yCenter);
            _rotationAngleRad = 0;
            _oneLengthPixels = 3;                       
            
            _userEditedCenterPoint = true;
            changeLocation = AbsoluteCenterPoint.Substract(_centerPoint);
            _hasBranch = false;
            _branchLength = 0;
            _centerPointBelongsToBranch = false;
            _branchPen = new Pen(Color.DarkGreen, _oneLengthPixels * 2);

            //IGrowable
            _zeroStateOneLengthPixels = 0.5F;
            _onePartGrowOneLengthPixels = 0.5F;
            _alreadyGrownState = 0;
            _currentTimeAfterLastGrowth = new TimeSpan(0);
            _isDead = false;
            _timeToGrowOneStepAfter = new TimeSpan(0, 0, 10);
            _timeToAverageDieAfter = new TimeSpan(0, 5, 0);
            _deathTimeSpanFromAveragePart = 0.1;
            LifeTimer = new Timer();
            LifeTimer.Interval = 500;
            LifeTimer.Tick += new EventHandler(LifeTimerTickHandler);
            //LifeTimer.Start();
            _drawToGraphics = true;
            Redraw += delegRdrw;
            itselfRefresh = true;
            //this.Refresh();
            tempSize = this.Size;
            _invertedBegining = false;

            _leafChildTemplate = new Leaf();

            doItselfRefresh();
        }
        public LeafPalm(Point AbsoluteCenterPoint, bool Growing)
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
            _beginingAnglePhase = 1;
            _leftCurveTension = _rightCurveTension = 0.5F;
            _invertedCurving = false;
            _invertedCurvingCenterAngle = 7;
            _invertedCurvingSpan = 1;
            int xCenter = panelNature.Width / 2;
            int yCenter = Convert.ToInt32(_oneLengthPixels * Math.Pow(Phi, _divideAngle * (180 - _onePartPossitionDegrees - _beginingAnglePhase + 1) / 180F));
            _centerPoint = new Point(xCenter, yCenter);
            _fill = true;
            _borderPen = Pens.DarkGreen;
            _fillBrush = new SolidBrush(Color.Green);
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
            _branchLength = 0;
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
            LifeTimer = new Timer();
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
        public LeafPalm(Point AbsoluteCenterPointLocation, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels)
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
            _fillBrush = new SolidBrush(Color.Green);
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
            _branchLength = 0;
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
            LifeTimer = new Timer();
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
        public LeafPalm(Point AbsoluteCenterPointLocation, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels, bool Growing)
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
            _fillBrush = new SolidBrush(Color.Green);
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
            _branchLength = 0;
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
            LifeTimer = new Timer();
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
        public LeafPalm(Point AbsoluteCenterPointLocation, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
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
            _fillBrush = new SolidBrush(Color.Green);
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
            _branchLength = 0;
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
            LifeTimer = new Timer();
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
        public LeafPalm(Point PointFromWhereToGrowBranch, float BranchLngth, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
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
            _fillBrush = new SolidBrush(Color.Green);
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
            _branchLength = BranchLngth;
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
            LifeTimer = new Timer();
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
        public LeafPalm(Point PointFromWhereToGrowBranch, float BranchLngth, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
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
            _fillBrush = new SolidBrush(Color.Green);
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
            _branchLength = BranchLngth;
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
            LifeTimer = new Timer();
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

        public LeafPalm(Point PointFromWhereToGrowBranch, float BranchLngth, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
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
            _fillBrush = new SolidBrush(Color.Green);
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
            _branchLength = BranchLngth;
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
            LifeTimer = new Timer();
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
        public LeafPalm(Point PointFromWhereToGrowBranch, float BranchLngth, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
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
            _fillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = VeinsFractalisation;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            changeLocation = PointFromWhereToGrowBranch.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = true;
            _branchLength = BranchLngth;
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
            LifeTimer = new Timer();
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

        public LeafPalm(Point PointFromWhereToGrowBranch, float BranchLngth, int DivideAngle, int StartPart, int OnePartRelativePosition, float OneLengthPixels,
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
            _fillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = VeinsFractalisation;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            changeLocation = PointFromWhereToGrowBranch.Substract(_centerPoint);
            //_locationBasedOnCenterPoint = true;
            _hasBranch = true;
            _branchLength = BranchLngth;
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
            LifeTimer = new Timer();
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

        internal LeafPalm(LineSegment rootLineSegment, float BranchLngthPixels, int DivideAngle, int StartPart, int OnePartRelativePosition,
            float zeroStateOneLengthPixels, float onePartGrowOneLengthPixels, int VeinsFractalisation,
            double deathTimeSpanFromAveragePart, float RotationRad, bool invertedLeaf, bool invertedBegining, bool DrawToGraphics)
        {
            InitializeComponent();
            secondPaint = false;
            _isBilaterallySymetric = true;
            _leftDivideAngle = _rightDivideAngle = _divideAngle = DivideAngle;
            _curveBehindCenterPoint = true;
            _smoothTop = false;
            _rotationAngleRad = 0;
            _oneLengthPixels = (rootLineSegment.Length - BranchLngthPixels) / (float)(Math.Pow(Phi, DivideAngle) + 1);
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
            _fillBrush = new SolidBrush(Color.Green);
            _veinsColor = Color.GreenYellow;
            _veins = true;
            _veinsFractalisation = VeinsFractalisation;
            _veinsBorderReachPart = 0.95F;
            _centralVeinPixelThickness = 2;
            //_panelBitmap = new Bitmap(this.Width, this.Height);
            _userEditedCenterPoint = true;
            changeLocation = (new Vector2(0, 1).Rotated(RotationRad) * _oneLengthPixels * (float)Math.Pow(Phi, -(180F / _beginingAnglePhase))).ToPoint();
            //_locationBasedOnCenterPoint = true;
            _hasBranch = true;
            _branchLength = BranchLngthPixels / _oneLengthPixels;
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
            _timeToGrowOneStepAfter = new TimeSpan(0, 10, 0);
            _timeToAverageDieAfter = new TimeSpan(0, 10, 0);
            _deathTimeSpanFromAveragePart = deathTimeSpanFromAveragePart;
            LifeTimer = new Timer();
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
                _branchPen = value;
                doRefresh();
            }
        }

        public Color BranchColor
        {
            get { return _branchPen.Color; }
            set
            {
                _branchPen.Color = value;
                doRefresh();
            }
        }

        private Leaf _leafChildTemplate;
        public Leaf LeafChildTemplate
        {
            get { return _leafChildTemplate; }
            set
            {

            }
        }
        #endregion

        internal void RescaleOnLineSegment(LineSegment root)
        {
            float rootLngth = root.Length;
            float ratio;

            ratio = _branchLength / (float)(Math.Pow(Phi, _divideAngle) + 1) * _oneLengthPixels;

            _alreadyGrownState = 0;
            _oneLengthPixels = _zeroStateOneLengthPixels = rootLngth / (1 + ratio);
            _branchLength = _zeroStateBranchOneLengthPixels = rootLngth / (1 + ratio) * ratio;

            _rotationAngleRad = new Vector2(0, 1).AngleBetween(-root.DirectionVector);

            doRefresh();
        }

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
                (_centerPointBelongsToBranch ? _centerPoint.Add(new Vector2(0, -_branchLength * _oneLengthPixels).Rotated(_rotationAngleRad)) : _centerPoint) :
                _centerPoint.Add(new Vector2(0, -1).Rotated(_rotationAngleRad) * _oneLengthPixels);

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
                    /*if (loc >= 0)*/
                    _itselfResizedArr[0] = true;
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
                    /*if (loc >= 0)*/
                    _itselfResizedArr[2] = true;
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

        private void LeafPalm_Resize(object sender, EventArgs e)
        {
            secondPaint = false;
            panelNature.Size = this.Size;
        }

        private void LeafPalm_Layout(object sender, LayoutEventArgs e)
        {
            secondPaint = false;
        }

        private void LeafPalm_Move(object sender, EventArgs e)
        {
            secondPaint = false;
        }

        private void LeafPalm_Paint(object sender, PaintEventArgs e)
        {
            secondPaint = false;
            if (changeLocation != new Point(Int32.MaxValue, Int32.MaxValue))
            {
                this.Location = changeLocation;
                changeLocation = new Point(Int32.MaxValue, Int32.MaxValue);
            }
            //panelLeaf_Paint(sender, e);
        }
        private Point changeLocation = new Point(Int32.MaxValue, Int32.MaxValue);

        private bool secondPaint;
        private void doRefresh()
        {
            secondPaint = false;
            _itselfResized = false;
            panelNature.Refresh();
        }

        private async void panelLeaf_Paint(object sender, PaintEventArgs e)
        {
            await doPanelPaint(e);
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

                    graphics.DrawCurve(Pens.DarkGreen, LPoints, _leftCurveTension);
                    graphics.DrawCurve(Pens.DarkGreen, RPoints, _rightCurveTension);
                }
                else
                {
                    Point[] curvePoints = RightCurvePoints.ReversedPointsWithoutFirst();
                    float tension = (_leftCurveTension + _rightCurveTension) / 2;

                    //resizeToShowAll(curvePoints, tension);

                    if (_fill)
                    {
                        Point[] unified = LeftCurvePoints.Union(curvePoints);
                        graphics.FillClosedCurve(Brushes.Green, unified, FillMode.Alternate, tension);
                    }
                    else
                    {
                        graphics.DrawCurve(Pens.DarkGreen, curvePoints, tension);
                    }
                }

                if (_hasBranch) graphics.DrawLine(_branchPen, _centerPoint, getCenterPoint());
                
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

        private bool _invertedBegining;
        public bool InvertedBegining
        {
            get { return _invertedBegining; }
            set
            {
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
        
                        
        private Point[] LeafCurvePoints()
        {
            Point[] res = new Point[361];
            Vector2 startVector = new Vector2(0, 1).Rotated(_rotationAngleRad);
            Point center = getCenterPoint();

            float radI;

            int iSign = -1;

            for (int i = -180; i <= 180; i++)
            {
                if (i == 0) iSign = 1;
                radI = RAD(i);
                res[i] = center.Add(startVector.Rotated(radI) * (float)Math.Pow(Phi, DivideAngle * i * iSign / 180F) * _oneLengthPixels);
            }

            return res;
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
                _isDead = value;
                if (_isDead)
                {
                    LifeTimer.Stop();
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
                _branchLength += (float)Phi * part;
                doRefresh();
            }
        }

        public void Die()
        {
            //panelLeaf.Paint -= panelLeaf_Paint;
            //panelLeaf.Paint += (object sender, PaintEventArgs e) => { };
            _isDead = true;
            LifeTimer.Stop();
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

        public Timer LifeTimer
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
                double res = 0;
                foreach (Leaf leaf in panelNature.Controls)
                {
                    res += leaf.IdealArea;
                }
                return res;
            }
        }

        /// <summary>
        /// Tangent angle of leaf palm's border curve. It means angle between radius line and tangent and is constant from properties of logarithmic spiral.
        /// </summary>
        public double IdealTangentAngle
        {
            get { return Math.Atan(Math.PI / (_divideAngle * Generals.LNPhi)); }
        }
        #endregion
    }
}