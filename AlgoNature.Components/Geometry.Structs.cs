using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using static AlgoNature.Components.Generals;

namespace AlgoNature.Components
{
    internal static partial class Geometry
    {
        public struct Line
        {
            public Line(Vector2 Direction, Point OriginPoint)
            {
                DirectionVector = Direction.ToUnitVector();
                Origin = OriginPoint;
            }
            public Line(Vector2 vector, Point OriginPoint, bool IsVectorDirectionVector)
            {
                if (IsVectorDirectionVector)
                {
                    DirectionVector = vector.ToUnitVector();
                    Origin = OriginPoint;
                }
                else
                {
                    DirectionVector = new Vector2(vector.Y, -vector.X).ToUnitVector();
                    Origin = OriginPoint;
                }
            }
            public Line(Point point1, Point point2)
            {
                DirectionVector = Vector2FromPoints(point1, point2).ToUnitVector();
                Origin = point1;
            }
            public Line(LineSegment LS)
            {
                DirectionVector = LS.DirectionVector;
                Origin = LS.OriginPoint;
            }

            public Vector2 DirectionVector { get; set; }
            public Point Origin { get; set; }
            public Vector2 NormalVector
            {
                get
                {
                    return new Vector2(DirectionVector.Y, -DirectionVector.X).ToUnitVector();
                }
                set
                {
                    DirectionVector = new Vector2(-value.Y, value.X).ToUnitVector();
                }
            }

            public float XParameter
            {
                get { return Parameters[0]; }
            }
            public float YParameter
            {
                get { return Parameters[1]; }
            }
            public float NumParameter
            {
                get { return Parameters[2]; }
            }
            public float[] Parameters
            {
                get
                {
                    float[] res = new float[3];
                    Vector2 norm = this.NormalVector;
                    res[0] = norm.X;
                    res[1] = norm.Y;
                    res[2] = -(norm.X * Origin.X + norm.Y * Origin.Y);
                    return res;
                }
            }

            public float AngleBetween(LineSegment other) => this.DirectionVector.AngleBetween(other.DirectionVector);

            public Line Rotated(Point centerRotationPointContained, float AngleRad) => new Line(this.DirectionVector.Rotated(AngleRad), centerRotationPointContained);

            public Point Intersect(Curve curve) => this.IntersectWithCurve(curve.CurvePoints, curve.Tension);
            public Point Intersect(Curve curve, Point pointNotToIntersectWith) => this.IntersectWithCurve(curve.CurvePoints, curve.Tension, pointNotToIntersectWith);
            public Point Intersect(Line line) => this.IntersectWithLine(line).ToPoint();
        }

        public struct LineSegment : IComparable
        {
            public LineSegment(Point originPoint, Vector2 Direction, float length)
            {
                DirectionVector = Direction.ToUnitVector() * length;
                OriginPoint = originPoint;
                EndPoint = originPoint.Add(Direction, length);
            }
            //public LineSegment(Point originPoint, Vector2 vector, bool IsVectorDirectionVector)
            //{
            //    if (IsVectorDirectionVector)
            //    {
            //        DirectionVector = vector;
            //        Origin = OriginPoint;
            //    }
            //    else
            //    {
            //        DirectionVector = new Vector2(vector.Y, -vector.X);
            //        Origin = OriginPoint;
            //    }
            //}
            public LineSegment(Point point1, Point point2)
            {
                DirectionVector = Vector2FromPoints(point1, point2).ToUnitVector();
                OriginPoint = point1;
                EndPoint = point2;
            }
            public LineSegment(int x1, int y1, int x2, int y2)
            {
                Point point1 = new Point(x1, y1);
                Point point2 = new Point(x2, y2);
                DirectionVector = Vector2FromPoints(point1, point2).ToUnitVector();
                OriginPoint = point1;
                EndPoint = point2;
            }
            public LineSegment(Point point1, Point point2, float part)
            {
                DirectionVector = Vector2FromPoints(point1, point2);
                OriginPoint = point1;
                EndPoint = point1.Add(part * DirectionVector);
                DirectionVector = DirectionVector.ToUnitVector();
            }

            public Vector2 DirectionVector { get; set; }
            public Point OriginPoint { get; set; }
            public Point EndPoint { get; set; }
            public Vector2 NormalVector
            {
                get
                {
                    return new Vector2(DirectionVector.Y, -DirectionVector.X).ToUnitVector();
                }
                set
                {
                    DirectionVector = new Vector2(-value.Y, value.X).ToUnitVector();
                }
            }

            public float XParameter
            {
                get { return Parameters[0]; }
            }
            public float YParameter
            {
                get { return Parameters[1]; }
            }
            public float NumParameter
            {
                get { return Parameters[2]; }
            }
            public float[] Parameters
            {
                get
                {
                    float[] res = new float[3];
                    Vector2 norm = this.NormalVector;
                    res[0] = norm.X;
                    res[1] = norm.Y;
                    res[2] = -(norm.X * OriginPoint.X + norm.Y * OriginPoint.Y);
                    return res;
                }
            }

            public float Length
            {
                get { return PointsDistance(OriginPoint, EndPoint); }
            }

            public int CompareTo(object obj)
            {
                if (obj is LineSegment)
                {
                    LineSegment _obj = (LineSegment)obj;
                    if (_obj.OriginPoint == this.OriginPoint &&
                        _obj.EndPoint == this.EndPoint)
                    {
                        return 0;
                    }
                    else return -1;
                }
                else return -1;
            }

            public Point PartPoint(float Part) => OriginPoint.Add(DirectionVector * Part * Length);

            public Point[] PartPoints(int divisionCount, bool startFromBeginingPoint)
            {
                Point[] res = new Point[divisionCount];

                float beginMove, part;

                if (startFromBeginingPoint)
                {
                    beginMove = 0;
                    part = 1F / divisionCount;
                }
                else
                {
                    part = 1F / (divisionCount + 1);
                    beginMove = part / 2;
                }

                for (int i = 0; i < divisionCount; i++)
                {
                    res[i] = this.PartPoint(beginMove + i * part);
                }

                return res;
            }

            public float PartPointLocation(Point pointContained) => OriginPoint.PointsDistance(EndPoint) / OriginPoint.PointsDistance(pointContained);

            public void MoveFromOriginPointToNewOriginPoint(Point newOriginPoint)
            {
                Vector2 move = Vector2FromPoints(OriginPoint, newOriginPoint);
                this.OriginPoint = OriginPoint.Add(move);
                this.EndPoint = EndPoint.Add(move);
            }
            public LineSegment MovedFromOriginPointToNewOriginPoint(Point newOriginPoint)
            {
                Vector2 move = Vector2FromPoints(OriginPoint, newOriginPoint);
                return new LineSegment(OriginPoint.Add(move), EndPoint.Add(move));
            }

            public Point[] GetDrawnPixelsPoints()
            {
                List<Point> res = new List<Point>();

                // Posouvání, kdyby bylo mimo grafiku
                int moveX = 0, moveY = 0;

                if (this.OriginPoint.X < moveX) moveX = this.OriginPoint.X;
                if (this.OriginPoint.Y < moveY) moveY = this.OriginPoint.Y;
                if (this.EndPoint.X < moveX) moveX = this.EndPoint.X;
                if (this.EndPoint.Y < moveY) moveY = this.EndPoint.Y;

                Point moveP = new Point(-moveX, -moveY);
                Point origin = this.OriginPoint.Add(moveP);
                Point end = this.EndPoint.Add(moveP);

                // Výška a šířka grafiky
                int widInd = origin.X, heiInd = origin.Y; // Indexy (souřadnice) -> budou pro rozměry zvětšeny o 1
                bool revWid = false, revHei = false; // Jestli se budechodi po souřadnicích obráceně
                if (end.X > widInd) widInd = end.X; else revWid = true;
                if (end.Y > heiInd) heiInd = end.Y; else revHei = true;

                Bitmap tempBMP = new Bitmap(widInd + 1, heiInd + 1);

                Graphics.FromImage(tempBMP).DrawLine(Pens.AliceBlue, origin, end);
                
                if (revHei)
                {                    
                    for (int y = origin.Y; y >= end.Y; y--)
                    {
                        if (revWid)
                        {
                            for (int x = origin.X; x >= end.X; x--)
                            {
                                if (tempBMP.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x + moveX, y + moveY))) res.Add(new Point(x + moveX, y + moveY));
                            }
                        }
                        else
                        {                            
                            for (int x = origin.X; x <= end.X; x++)
                            {
                                if (tempBMP.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x + moveX, y + moveY))) res.Add(new Point(x + moveX, y + moveY));
                            }
                        }
                    }
                }
                else
                {
                    for (int y = origin.Y; y >= end.Y; y--)
                    {
                        if (revWid)
                        {
                            for (int x = origin.X; x >= end.X; x--)
                            {
                                if (tempBMP.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x + moveX, y + moveY))) res.Add(new Point(x + moveX, y + moveY));
                            }
                        }
                        else
                        {
                            for (int x = origin.X; x <= end.X; x++)
                            {
                                if (tempBMP.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x + moveX, y + moveY))) res.Add(new Point(x + moveX, y + moveY));
                            }
                        }
                    }
                }

                tempBMP.Dispose();
                return res.ToArray();
            }

            public LineSegment LineSegmentUntilCurve(Curve cur)
                => this.LineSegmentUntilCurve(cur.GetDrawnPixelsPoints());
            public LineSegment LineSegmentUntilCurve(Point[] curveDrawnPoints)
            {
                Point end = new Point(Int32.MinValue, Int32.MinValue);

                Point[] lineSegPoints = this.GetDrawnPixelsPoints();

                foreach (Point p in lineSegPoints)
                {
                    if (curveDrawnPoints.Contains(p))
                    {
                        end = p;
                        break;
                    }
                }

                return new LineSegment(this.OriginPoint, (end == new Point(Int32.MinValue, Int32.MinValue)) ? this.EndPoint : end);
            }

            public float AngleBetween(LineSegment other) => this.DirectionVector.AngleBetween(other.DirectionVector);
        }

        public struct FractalisingVeinLineSegment
        {
            /// <summary>
            /// Konstruktor pro novou strukturu založenou na výchozí úsečce, neohraničenou listem
            /// </summary>
            /// <param name="rootLineSegment"></param>
            /// <param name="fractalisationLevel"></param>
            /// <param name="sideBranchesCount"></param>
            /// <param name="evenFromBegining"></param>
            /// <param name="childParentRatio"></param>
            /// <param name="childrenDeclinationRad"></param>
            /// <param name="hasLeftSideBranches"></param>
            /// <param name="hasRightSideBranches"></param>
            /// <param name="thickness"></param>
            /// <param name="color"></param>
            public FractalisingVeinLineSegment(LineSegment rootLineSegment, int fractalisationLevel,
                int sideBranchesCount, bool evenFromBegining, float childParentRatio, float childrenDeclinationRad, bool hasLeftSideBranches, bool hasRightSideBranches,
                float thickness, Color color)
            {
                FractalisationParentalLevel = fractalisationLevel;
                SideBranchesCount = sideBranchesCount;
                ChildParentRatio = childParentRatio;
                ChildrenDeclinationRad = childrenDeclinationRad;
                EvenDivFromBeginingPoint = evenFromBegining;
                
                x1 = rootLineSegment.OriginPoint.X;
                y1 = rootLineSegment.OriginPoint.Y;
                x2 = rootLineSegment.EndPoint.X;
                y2 = rootLineSegment.EndPoint.Y;
                left = hasLeftSideBranches;
                right = hasRightSideBranches;
                Thickness = thickness;
                _color = color.ToArgb();

                hasLeftCurve = hasRightCurve = false;
                rightCurvePoints = leftCurvePoints = new Point[0];
                CurveReachPart = 1;

                Children = new List<FractalisingVeinLineSegment>();

                isLeftMinor = isRightMinor = false;

                parentsDivPartLength = 0; // Nemá rodiče => je hlavní
                KCosPhiHalfForChildren = 0; // Nemá rodiče => děti budou 1. třídy -> nemusí se krátit

                if (FractalisationParentalLevel > 0 && SideBranchesCount > 0 && (left || right))
                {
                    Point[] points = new LineSegment(new Point(x1, y1), new Point(x2, y2)).PartPoints(SideBranchesCount, evenFromBegining);
                    for (int i = 1; i < SideBranchesCount - 1; i++)
                    {
                        if (left) Children.Add(new FractalisingVeinLineSegment(points[i], this, _color, true));
                        if (right) Children.Add(new FractalisingVeinLineSegment(points[i], this, _color, false));
                    }
                }
            }

            // Konstruktor pro list
            /// <summary>
            /// Konstruktor vedlejších žil první třídy pro list
            /// </summary>
            /// <param name="rootLineSegment">LineSegment as the central LineSegment of this new minor vein</param>
            /// <param name="fractalisationLevel"></param>
            /// <param name="sideBranchesCount"></param>
            /// <param name="evenFromBegining"></param>
            /// <param name="childParentRatio"></param>
            /// <param name="childrenDeclinationRad"></param>
            /// <param name="hasLeftSideBranches"></param>
            /// <param name="hasRightSideBranches"></param>
            /// <param name="thickness"></param>
            /// <param name="color"></param>
            /// <param name="isLeft"></param>
            /// <param name="itsCurve"></param>
            /// <param name="borderCurveReachPart"></param>
            private FractalisingVeinLineSegment(LineSegment rootLineSegment, int fractalisationLevel,
                int sideBranchesCount, bool evenFromBegining, float childParentRatio, float childrenDeclinationRad, bool hasLeftSideBranches, bool hasRightSideBranches,
                float thickness, Color color, bool isLeft, Point[] itsCurve, float borderCurveReachPart, float parentsOneDividedPartLength)
            {
                FractalisationParentalLevel = fractalisationLevel;
                SideBranchesCount = sideBranchesCount;
                ChildParentRatio = childParentRatio;
                ChildrenDeclinationRad = childrenDeclinationRad;
                EvenDivFromBeginingPoint = evenFromBegining;

                x1 = rootLineSegment.OriginPoint.X;
                y1 = rootLineSegment.OriginPoint.Y;
                x2 = rootLineSegment.EndPoint.X;
                y2 = rootLineSegment.EndPoint.Y;
                left = hasLeftSideBranches;
                right = hasRightSideBranches;
                Thickness = thickness;
                _color = color.ToArgb();

                parentsDivPartLength = rootLineSegment.Length / (sideBranchesCount + Convert.ToInt32(!evenFromBegining)); // Pokud nezačíná od počátku, dělí se o 1 víc

                if (isLeft)
                {
                    leftCurvePoints = itsCurve;
                    rightCurvePoints = new Point[0];
                }
                else
                {
                    leftCurvePoints = new Point[0];
                    rightCurvePoints = itsCurve;
                }
                
                CurveReachPart = borderCurveReachPart;

                Children = new List<FractalisingVeinLineSegment>();

                hasLeftCurve = isLeftMinor = isLeft;
                hasRightCurve = isRightMinor = !isLeft;

                KCosPhiHalfForChildren = parentsOneDividedPartLength * (float)Math.Cos(ChildrenDeclinationRad) / 2;

                if (FractalisationParentalLevel > 0 && SideBranchesCount > 0 && (left || right))
                {
                    Point[] points = new LineSegment(new Point(x1, y1), new Point(x2, y2)).PartPoints(SideBranchesCount, evenFromBegining);
                    for (int i = 1; i < SideBranchesCount - 1; i++)
                    {
                        if (left) Children.Add(new FractalisingVeinLineSegment(points[i], this, _color, true));
                        if (right) Children.Add(new FractalisingVeinLineSegment(points[i], this, _color, false));
                    }
                }
            }
            
            /// <summary> Pro normální postranní žíly druhé třídy a výše</summary>       
            private FractalisingVeinLineSegment(Point begining, FractalisingVeinLineSegment parent, int colorARGB, bool isLeft)
            {
                FractalisationParentalLevel = parent.FractalisationParentalLevel - 1;
                SideBranchesCount = parent.SideBranchesCount;
                ChildParentRatio = parent.ChildParentRatio;
                ChildrenDeclinationRad = parent.ChildrenDeclinationRad;
                EvenDivFromBeginingPoint = parent.EvenDivFromBeginingPoint;        
                x1 = begining.X;
                y1 = begining.Y;
                Point end;
                Vector2 parentsLineVect = new Vector2(parent.x2 - parent.x1, parent.y2 - parent.y1);
                if (ChildrenDeclinationRad % (2 * Math.PI) == 0 || parent.KCosPhiHalfForChildren == 0) // když je rovnoběžná s hlavní, nebo je postranní 1. třídy
                {
                    if (isLeft) end = new Point(x1, y1).Add(parentsLineVect.Part(ChildParentRatio).Rotated(-ChildrenDeclinationRad));
                    else end = new Point(x1, y1).Add(parentsLineVect.Part(ChildParentRatio).Rotated(ChildrenDeclinationRad));
                }
                else
                {
                    if (isLeft) end = new Point(x1, y1).Add((parentsLineVect.ToUnitVector() * parent.KCosPhiHalfForChildren).Rotated(-ChildrenDeclinationRad));
                    else end = new Point(x1, y1).Add((parentsLineVect.ToUnitVector() * parent.KCosPhiHalfForChildren).Rotated(ChildrenDeclinationRad));
                }
                x2 = end.X;
                y2 = end.Y;
                //FractalisingVeinLineSegment par = Marshal.PtrToStructure<FractalisingVeinLineSegment>(parentFVLSPtr);
                Thickness = parent.Thickness * ChildParentRatio;
                left = parent.left;
                right = parent.right;
                _color = colorARGB;

                hasLeftCurve = parent.hasLeftCurve;
                hasRightCurve = parent.hasRightCurve;
                leftCurvePoints = parent.leftCurvePoints;
                rightCurvePoints = parent.rightCurvePoints;
                CurveReachPart = parent.CurveReachPart;

                parentsDivPartLength = parentsLineVect.Length() / (parent.SideBranchesCount + Convert.ToInt32(!parent.EvenDivFromBeginingPoint));

                KCosPhiHalfForChildren = parent.parentsDivPartLength * (float)Math.Cos(ChildrenDeclinationRad) / 2;

                Children = new List<FractalisingVeinLineSegment>();

                if (isLeft)
                {
                    isLeftMinor = true;
                    isRightMinor = false;
                }
                else
                {
                    isLeftMinor = false;
                    isRightMinor = true;
                }

                // Krátit počet postranních, pokud je z listu a protíná stranu
                if ((hasLeftCurve || hasRightCurve) && (isLeftMinor || isRightMinor))
                {
                    LineSegment thisLS = new LineSegment(x1, y1, x2, y2);
                    float thisLSLen = thisLS.Length;

                    //Ošetření, aby neběželo mimo list
                    thisLS = thisLS.LineSegmentUntilCurve(isLeftMinor ? leftCurvePoints : rightCurvePoints);

                    this.x2 = thisLS.EndPoint.X;
                    this.y2 = thisLS.EndPoint.Y;
                                        
                    SideBranchesCount = Convert.ToInt32((float)SideBranchesCount * thisLS.Length / thisLSLen);
                }                

                if (FractalisationParentalLevel > 0 && SideBranchesCount > 0 && (left || right))
                {
                    Point[] points = new LineSegment(new Point(x1, y1), new Point(x2, y2)).PartPoints(SideBranchesCount, EvenDivFromBeginingPoint);
                    //FractalisingVeinLineSegment* one = null;
                    //FractalisingVeinLineSegment newChild;
                    for (int i = 0; i < SideBranchesCount; i++)
                    {
                        if (left) Children.Add(new FractalisingVeinLineSegment(points[i], this, _color, true));
                        if (right) Children.Add(new FractalisingVeinLineSegment(points[i], this, _color, false));
                    }
                }
                //if (neighbourBefore != null) neighbourbefore->neighbourAfter = this;
            }
            
            public static FractalisingVeinLineSegment OnLeaf(Leaf leaf, bool evenFromBegining, float childrenDeclination)
            {
                Curve LeftCurve = leaf.LeftCurve;
                Curve RightCurve = leaf.RightCurve;

                int partDiv = leaf.DivideAngle;
                int fractalisationLevel = leaf.VeinsFractalisation;
                Color color = leaf.VeinsColor;
                float thickness = leaf.CentralVeinPixelThickness;

                LineSegment rootLS;
                if (leaf.InvertedLeaf)
                {
                    rootLS = new LineSegment(LeftCurve.CurvePoints[LeftCurve.CurvePoints.Length - 1], LeftCurve.CurvePoints[0]);
                    //evenFromBegining = false;
                }
                else
                {
                    rootLS = new LineSegment(LeftCurve.CurvePoints[0], LeftCurve.CurvePoints[LeftCurve.CurvePoints.Length - 1]);
                }                

                List<FractalisingVeinLineSegment> children = new List<FractalisingVeinLineSegment>();

                Point[] leftCurPts = leaf.LeftCurve.GetDrawnPixelsPoints();
                Point[] rightCurPts = leaf.RightCurve.GetDrawnPixelsPoints();

                // If fractalisation > 0 -> add children
                if (fractalisationLevel > 0)
                {
                    // Arrays
                    Point[] LeftStartPoints, RightStartPoints;
                    Point[] LeftEndPoints = new Point[partDiv];

                    Point[] RightEndPoints = new Point[partDiv];

                    List<LineSegment> _lineSegments = new List<LineSegment>();
                    //LineSegment[] LineSegments;

                    //LeftStartPoints & RightStartPoints
                    LeftStartPoints = RightStartPoints = rootLS.PartPoints(partDiv, evenFromBegining);

                    Line vein;
                    // Pokud rostou nějaké ze středu a je to ideální logaritmická spirála, stačí udělat otočenou úsečku
                    //if (evenFromBegining && leaf.AccurateLogarithmicSpiral)
                    //{                        
                    //    int i = Convert.ToInt32(180 * (Math.PI - childrenDeclination) / Math.PI);
                    //    _lineSegments.Add(new LineSegment(LeftStartPoints[0], LeftCurve.CurvePoints[i])); // Přidá levé dítě                        
                    //}
                    //else
                    //{
                    //    vein = new Line(rootLS).Rotated(LeftStartPoints[0], -childrenDeclination);
                    //    LeftEndPoints[0] = vein.Intersect(LeftCurve, LeftStartPoints[0]);
                    //    _lineSegments.Add(new LineSegment(LeftStartPoints[0], LeftEndPoints[0])); // Přidá levé dítě
                    //}
                    
                    //LeftEndPoints a levé děti
                    for (int i = 0; i < partDiv; i++)
                    {
                        vein = new Line(rootLS).Rotated(LeftStartPoints[i], -childrenDeclination);
                        LeftEndPoints[i] = vein.Intersect(LeftCurve, LeftStartPoints[i]);
                        _lineSegments.Add(new LineSegment(LeftStartPoints[i], LeftEndPoints[i])); // Přidá levé dítě
                    }

                    // Pokud rostou nějaké ze středu a je to ideální logaritmická spirála, stačí udělat otočenou úsečku
                    //if (evenFromBegining && leaf.AccurateLogarithmicSpiral)
                    //{
                    //    int i = Convert.ToInt32(180 * (Math.PI - childrenDeclination) / Math.PI);
                    //    _lineSegments.Add(new LineSegment(RightStartPoints[0], RightCurve.CurvePoints[i]));
                    //}
                    //else
                    //{
                    //    vein = new Line(rootLS).Rotated(RightStartPoints[0], childrenDeclination);
                    //    RightEndPoints[0] = vein.Intersect(RightCurve, RightStartPoints[0]);
                    //    _lineSegments.Add(new LineSegment(RightStartPoints[0], RightEndPoints[0])); // Přidá pravé dítě
                    //}
                    
                    //RightEndPoints a pravé děti
                    for (int i = 0; i < partDiv; i++)
                    {
                        vein = new Line(rootLS).Rotated(RightStartPoints[i], childrenDeclination);
                        RightEndPoints[i] = vein.Intersect(RightCurve, RightStartPoints[i]);
                        _lineSegments.Add(new LineSegment(RightStartPoints[i], RightEndPoints[i])); // Přidá pravé dítě
                    }

                    // this parentasOneDividedPartLength
                    float part = rootLS.Length / (partDiv + Convert.ToInt32(!evenFromBegining));

                    LineSegment[] LineSegments = _lineSegments.ToArray();
                    float rootLngth = rootLS.Length;
                    float ratio = 0.5F;



                    //  --Nejdelší žíla určuje délku a počet postranních větví zbylých--
                    float[] lenghts = new float[2 * partDiv]; // Pole všech délek, ať se nemusí již počítat znovu
                    float longestLeft = 0; // délka nejdelší levé
                    float longestRight = 0; // délka nejdelší pravé

                    // Nejdelší levá a délky levých
                    for (int i = 0; i < partDiv; i++) // prvních n (= partDiv) jsou levé
                    {
                        lenghts[i] = LineSegments[i].Length;
                        if (lenghts[i] > longestLeft) longestLeft = lenghts[i];
                    }

                    // Nejdelší pravá a délky pravých
                    for (int i = partDiv; i < 2 * partDiv; i++) // druhých n (= partDiv) jsou levé
                    {
                        lenghts[i] = LineSegments[i].Length;
                        if (lenghts[i] > longestRight) longestRight = lenghts[i];
                    }



                    // Prvních n jsou levé                    
                    for (int i = 0; i < partDiv; i++) 
                    {
                        //ratio = 0.5F; //LineSegments[i].Length / rootLngth;
                        children.Add(new FractalisingVeinLineSegment(LineSegments[i], fractalisationLevel - 1, Convert.ToInt32(partDiv * lenghts[i] / (longestLeft > 0 ? longestLeft : 1)),
                            evenFromBegining, ratio, childrenDeclination, true, true, thickness * ratio, color, true, leftCurPts, leaf.VeinsBorderReachPart, part));
                    }

                    // Druhých n jsou pravé                    
                    for (int i = partDiv; i < 2 * partDiv; i++) // Druhých 10 jsou pravé
                    {
                        //ratio = 0.5F; //LineSegments[i].Length / rootLngth;
                        children.Add(new FractalisingVeinLineSegment(LineSegments[i], fractalisationLevel - 1, Convert.ToInt32(partDiv * lenghts[i] / (longestRight > 0 ? longestRight : 1)), evenFromBegining, ratio,
                            childrenDeclination, true, true, thickness * ratio, color, false, rightCurPts, leaf.VeinsBorderReachPart, part));
                    }
                }

                float parentsDivPart = rootLS.Length / (leaf.DivideAngle + Convert.ToInt32(!evenFromBegining));

                // Return the new structure
                return new FractalisingVeinLineSegment()
                {
                    Children = children,
                    FractalisationParentalLevel = fractalisationLevel,
                    SideBranchesCount = partDiv,
                    EvenDivFromBeginingPoint = evenFromBegining,
                    ChildParentRatio = 0,
                    ChildrenDeclinationRad = childrenDeclination,
                    Thickness = thickness,
                    _color = color.ToArgb(),
                    x1 = rootLS.OriginPoint.X,
                    y1 = rootLS.OriginPoint.Y,
                    x2 = rootLS.EndPoint.X,
                    y2 = rootLS.EndPoint.Y,
                    left = true,
                    right = true,
                    hasLeftCurve = true,
                    hasRightCurve = true,
                    leftCurvePoints = leftCurPts,
                    rightCurvePoints = rightCurPts,
                    CurveReachPart = leaf.VeinsBorderReachPart,
                    parentsDivPartLength = parentsDivPart,
                    KCosPhiHalfForChildren = 0
                };
            }

            //public IntPtr parent { get; private set; }
            public List<FractalisingVeinLineSegment> Children { get; private set; }
                       
            /// <summary>
            /// Determines how many child levels are under this.
            /// </summary>
            public int FractalisationParentalLevel { get; private set; }
            public int SideBranchesCount { get; private set; }
            public bool EvenDivFromBeginingPoint { get; private set; }
            public float ChildParentRatio { get; private set; }
            public float ChildrenDeclinationRad { get; private set; }
            public float Thickness { get; private set; }
            public float CurveReachPart { get; private set; }

            private int _color;
            public Color VeinColor
            {
                get
                {
                    return Color.FromArgb(_color);
                }
                set
                {
                    SetColor(value.ToArgb());
                }
            }

            private int x1, y1, x2, y2;

            private bool left, right;
            private bool isLeftMinor, isRightMinor;

            private Point[] leftCurvePoints { get; set; }
            private Point[] rightCurvePoints { get; set; }

            private bool hasLeftCurve, hasRightCurve;

            private float parentsDivPartLength; // Když 0, je to nejvyšší rodič

            // Pro urychlení operací - kosinus se jednou, ne u všech 2n postranních větví
            private float KCosPhiHalfForChildren { get; set; } // Když 0, je to hlavní žíla -> postranní žíly (1. třídy) se nemusí se zkracovat

            public void DrawToGraphics(ref Graphics gr)
            {
                if (CurveReachPart != 1) gr.DrawPartLine(new Pen(VeinColor, Thickness), x1, y1, x2, y2, CurveReachPart);
                else gr.DrawLine(new Pen(VeinColor, Thickness), x1, y1, x2, y2);

                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].DrawToGraphics(ref gr);
                }                
            }

            public LineSegment[] ChildrenLineSegments() => ChildrenLineSegments(FractalisationParentalLevel);
            public LineSegment[] ChildrenLineSegments(int untilHowDeep)
            {
                int until = (untilHowDeep > FractalisationParentalLevel) ? FractalisationParentalLevel : untilHowDeep;
                if (until == 0) return new LineSegment[1] { new LineSegment(x1, y1, x2, y2) };
                else
                {
                    
                    LineSegment[] res = new LineSegment[1] { new LineSegment(x1, y1, x2, y2) };
                    foreach (FractalisingVeinLineSegment child in Children)
                    {
                        res = res.Join(child.ChildrenLineSegments(until - 1));
                    }
                    return res;
                }
            }

            public LineSegment[] ChildrenLineSegmentsOneLevel(int childDepth)
            {
                int depth = (childDepth > FractalisationParentalLevel) ? FractalisationParentalLevel : childDepth;
                if (depth == 0) return new LineSegment[1] { new LineSegment(x1, y1, x2, y2) };
                else
                {
                    LineSegment[] res = new LineSegment[0];
                    foreach (FractalisingVeinLineSegment child in Children)
                    {
                        res = res.Join(child.ChildrenLineSegmentsOneLevel(depth - 1));
                    }
                    return res;
                }
            }

            private void SetColor(int argb)
            {
                _color = argb;
                for (int i = 0; i < Children.Count; i++)
                {
                    Children[i].SetColor(argb);
                }
            }
        }

        // OLD Syntax
        /*
        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 54)]
        public struct FractalisingVeinLineSegment
        {            
            public unsafe FractalisingVeinLineSegment(LineSegment* rootLineSegment, int fractalisationLevel,
                int* sideBranchesCount, float* childParentRatio, float* childrenDeclinationRad, bool hasLeftSideBranches, bool hasRightSideBranches,
                float thickness, Color color)
            {
                FractalisationParentalLevel = fractalisationLevel;
                SideBranchesCount = sideBranchesCount;
                ChildParentRatio = childParentRatio;
                ChildrenDeclinationRad = childrenDeclinationRad;
                //parent = new IntPtr(null);
                firstChild = null;
                neighbourBefore = null;
                neighbourAfter = null;
                x1 = rootLineSegment->OriginPoint.X;
                y1 = rootLineSegment->OriginPoint.Y;
                x2 = rootLineSegment->EndPoint.X;
                y2 = rootLineSegment->EndPoint.Y;
                left = hasLeftSideBranches;
                right = hasRightSideBranches;
                Thickness = thickness;
                _color = color.ToArgb();

                firstChild = null;

                if (FractalisationParentalLevel > 0 && *SideBranchesCount > 0 && (left || right))
                {
                    FractalisingVeinLineSegment[] children = new FractalisingVeinLineSegment[*SideBranchesCount * ((left && right) ? 2 : 1)];
                    fixed (FractalisingVeinLineSegment* ptr = children)
                    {
                        //FractalisingVeinLineSegment* p = children;
                        FractalisingVeinLineSegment* p = ptr + 1;
                        Point[] points = new LineSegment(new Point(x1, y1), new Point(x2, y2)).PartPoints(*SideBranchesCount, false);
                        //FractalisingVeinLineSegment* one = null;
                        //FractalisingVeinLineSegment newChild;
                        if (left && !right)
                        {
                            if (*SideBranchesCount > 1) *p = new FractalisingVeinLineSegment(null, p + 1, points[0], this, _color, true);
                            else *p = new FractalisingVeinLineSegment(null, null, points[0], this, _color, true);
                            p++;
                        }
                        else if (!left && right)
                        {
                            if (*SideBranchesCount > 1) *p = new FractalisingVeinLineSegment(null, p + 1, points[0], this, _color, false);
                            else *p = new FractalisingVeinLineSegment(null, null, points[0], this, _color, false);
                            p++;
                        }
                        else
                        {
                            if (*SideBranchesCount > 1)
                            {
                                *p = new FractalisingVeinLineSegment(null, p + 1, points[0], this, _color, true);
                                p++;
                                *p = new FractalisingVeinLineSegment(null, p + 1, points[0], this, _color, false);
                            }
                            else
                            {
                                *p = new FractalisingVeinLineSegment(null, p + 1, points[0], this, _color, true);
                                p++;
                                *p = new FractalisingVeinLineSegment(null, null, points[0], this, _color, false);
                            }
                            p++;
                        }

                        if (*SideBranchesCount > 1)
                        {
                            for (int i = 1; i < *SideBranchesCount - 1; i++)
                            {
                                //FractalisingVeinLineSegment newChild;
                                *p = new FractalisingVeinLineSegment(p - 1, p + 1, points[i], this, _color, true);
                                p++;
                                *p = new FractalisingVeinLineSegment(p - 1, p + 1, points[i], this, _color, false);
                                p++;
                                //one = &newChild;
                            }

                            *p = new FractalisingVeinLineSegment(p - 1, p + 1, points[points.Length - 1], this, _color, true);
                            p++;
                            *p = new FractalisingVeinLineSegment(p - 1, null, points[points.Length - 1], this, _color, false);
                            p++;
                        }
                    }
                }
            }
            private unsafe FractalisingVeinLineSegment(FractalisingVeinLineSegment* neighbourbefore, FractalisingVeinLineSegment* neighbourafter, Point begining,
                FractalisingVeinLineSegment parent, int colorARGB, bool isleft)
            {
                FractalisationParentalLevel = parent.FractalisationParentalLevel - 1;
                SideBranchesCount = parent.SideBranchesCount;
                ChildParentRatio = parent.ChildParentRatio;
                ChildrenDeclinationRad = parent.ChildrenDeclinationRad;
                //parent = parentFVLSPtr;
                neighbourBefore = neighbourbefore;
                neighbourAfter = neighbourafter;
                x1 = begining.X;
                y1 = begining.Y;
                Point end;
                if (isleft) end = new Point(x1, y1).Add(new Vector2(parent.x2 - parent.x1, parent.y2 - parent.y1).Part(*ChildParentRatio).Rotated(*ChildrenDeclinationRad));
                else end = new Point(x1, y1).Add(new Vector2(parent.x2 - parent.x1, parent.y2 - parent.y1).Part(*ChildParentRatio).Rotated(-*ChildrenDeclinationRad));
                x2 = end.X;
                y2 = end.Y;
                //FractalisingVeinLineSegment par = Marshal.PtrToStructure<FractalisingVeinLineSegment>(parentFVLSPtr);
                Thickness = parent.Thickness / *ChildParentRatio;
                left = parent.left;
                right = parent.right;
                _color = colorARGB;

                firstChild = null;

                if (FractalisationParentalLevel > 0 && *SideBranchesCount > 0 && (left || right))
                {
                    FractalisingVeinLineSegment[] children = new FractalisingVeinLineSegment[*SideBranchesCount * ((left && right) ? 2 : 1)];
                    fixed (FractalisingVeinLineSegment* ptr = children)
                    {
                        //FractalisingVeinLineSegment* p = children;
                        FractalisingVeinLineSegment* p = ptr + 1;
                        Point[] points = new LineSegment(new Point(x1, y1), new Point(x2, y2)).PartPoints(*SideBranchesCount, false);
                        //FractalisingVeinLineSegment* one = null;
                        //FractalisingVeinLineSegment newChild;
                        if (left && !right)
                        {
                            if (*SideBranchesCount > 1) *p = new FractalisingVeinLineSegment(null, p + 1, points[0], this, _color, true);
                            else *p = new FractalisingVeinLineSegment(null, null, points[0], this, _color, true);
                            p++;
                        }
                        else if (!left && right)
                        {
                            if (*SideBranchesCount > 1) *p = new FractalisingVeinLineSegment(null, p + 1, points[0], this, _color, false);
                            else *p = new FractalisingVeinLineSegment(null, null, points[0], this, _color, false);
                            p++;
                        }
                        else
                        {
                            if (*SideBranchesCount > 1)
                            {
                                *p = new FractalisingVeinLineSegment(null, p + 1, points[0], this, _color, true);
                                p++;
                                *p = new FractalisingVeinLineSegment(null, p + 1, points[0], this, _color, false);
                            }
                            else
                            {
                                *p = new FractalisingVeinLineSegment(null, p + 1, points[0], this, _color, true);
                                p++;
                                *p = new FractalisingVeinLineSegment(null, null, points[0], this, _color, false);
                            }
                            p++;
                        }                        
                        
                        if (*SideBranchesCount > 1)
                        {
                            for (int i = 1; i < *SideBranchesCount - 1; i++)
                            {
                                //FractalisingVeinLineSegment newChild;
                                *p = new FractalisingVeinLineSegment(p - 1, p + 1, points[i], this, _color, true);
                                p++;
                                *p = new FractalisingVeinLineSegment(p - 1, p + 1, points[i], this, _color, false);
                                p++;
                                //one = &newChild;
                            }

                            *p = new FractalisingVeinLineSegment(p - 1, p + 1, points[points.Length - 1], this, _color, true);
                            p++;
                            *p = new FractalisingVeinLineSegment(p - 1, null, points[points.Length - 1], this, _color, false);
                            p++;
                        }
                    }
                }

                //if (neighbourBefore != null) neighbourbefore->neighbourAfter = this;
            }
                     

            //public IntPtr parent { get; private set; }
            public unsafe FractalisingVeinLineSegment* neighbourBefore { get; private set; }
            public unsafe FractalisingVeinLineSegment* neighbourAfter { get; set; }
            public unsafe FractalisingVeinLineSegment* firstChild { get; private set; }
            //private int _fractalisationLevel;
            /// <summary>
            /// Determines how many child levels are under this.
            /// </summary>
            public int FractalisationParentalLevel { get; private set; }
            public unsafe int* SideBranchesCount { get; private set; }
            public unsafe float* ChildParentRatio { get; private set; }
            public unsafe float* ChildrenDeclinationRad { get; private set; }
            public float Thickness { get; private set; }

            private int _color;
            public Color VeinColor
            {
                get
                {
                    return Color.FromArgb(_color);
                }
                set
                {                    
                    SetColor(value.ToArgb());                
                }
            }

            private int x1, y1, x2, y2;

            private bool left, right;

            public unsafe void DrawToGraphics(ref Graphics gr)
            {
                // implementovat barvu
                if (neighbourAfter != null) neighbourAfter->DrawToGraphics(ref gr);
                if (firstChild != null) firstChild->DrawToGraphics(ref gr);
            }

            private unsafe void SetColor(int argb)
            {
                _color = argb;
                if (firstChild != null) firstChild->SetColor(argb);
                if (neighbourAfter != null) neighbourAfter->SetColor(argb);
            }
        }
        */

        public struct Curve
        {
            public Curve(Point[] curvePoints)
            {
                CurvePoints = curvePoints;
                Tension = 0.5F;
            }
            public Curve(Point[] curvePoints, float tension)
            {
                CurvePoints = curvePoints;
                Tension = tension;
            }

            public Point[] CurvePoints { get; set; }
            public float Tension { get; set; }

            public Point[] GetDrawnPixelsPoints() => Geometry.CurveDrawnPoints(CurvePoints, Tension);
            public float GetLength()
            {
                float res = 0;
                Point[] _this = this.GetDrawnPixelsPoints();
                if (_this.Length > 1)
                {
                    for (int i = 0; i < _this.Length - 1; i++)
                    {
                        res += PointsDistance(_this[i], _this[i + 1]);
                    }
                }
                return res;
            }
            public Point PartPoint(float Part) => CurvePartPoints(Tension, CurvePoints, Part)[0];
            /// <summary>
            /// Returns Part of the curve where input point is located.
            /// </summary>
            /// <param name="pointContained"></param>
            /// <returns>If the curve doesn't contain the input point, returns -1.</returns>
            public float PartPointLocation(Point pointContained)
            {
                float res = -1;
                Point[] curpnts = this.GetDrawnPixelsPoints();
                for (int i = 0; i < curpnts.Length; i++)
                {
                    if (curpnts[i] == pointContained)
                    {
                        res = (float)(i + 1) / (float)curpnts.Length;
                        break;
                    }
                }
                return res;
            }

            public Line LineFromPointOn(Point PointContained)
            {
                Point[] _points = this.GetDrawnPixelsPoints();
                if (_points.Contains<Point>(PointContained))
                {
                    float part = this.PartPointLocation(PointContained);
                    Point nextp;
                    int index;
                    try
                    {
                        index = Convert.ToInt32(part * _points.Length) - 1;
                        nextp = _points[index + 3];
                        return new Line(PointContained, nextp);
                    }
                    catch
                    {
                        index = Convert.ToInt32(part * _points.Length - 1);
                        nextp = _points[index - 3];
                        return new Line(nextp, PointContained);
                    }                    
                }
                else throw new Exception("Line doesn't contain input point.");
            }
        }
    }

    internal static partial class Generals
    {
        public static Geometry.LineSegment[] Join(this Geometry.LineSegment[] a, Geometry.LineSegment[] b)
        {
            Geometry.LineSegment[] res = new Geometry.LineSegment[a.Length + b.Length];
            for (int i = 0; i < res.Length; i++)
            {
                if (i < a.Length) res[i] = a[i];
                else res[i] = b[i - a.Length];
            }
            return res;
        }
    }
}



// IntPtr operace
/*
     // Create a point struct.
        Point p;
        p.x = 1;
        p.y = 1;

        Console.WriteLine("The value of first point is " + p.x + " and " + p.y + ".");

        // Initialize unmanged memory to hold the struct.
        IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(p));

        try
        {

            // Copy the struct to unmanaged memory.
            Marshal.StructureToPtr(p, pnt, false);

            // Create another point.
            Point anotherP;

            // Set this Point to the value of the 
            // Point in unmanaged memory. 
            anotherP = (Point)Marshal.PtrToStructure(pnt, typeof(Point));

            Console.WriteLine("The value of new point is " + anotherP.x + " and " + anotherP.y + ".");

        }
        finally
        {
            // Free the unmanaged memory.
            Marshal.FreeHGlobal(pnt);
        }

*/
