using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.Drawing.Drawing2D;
//using static AlgoNature.Components.Maths;

namespace AlgoNature.Components
{
    internal static partial class Geometry
    {
        public static float DEG(float rad) => (float)(rad * 180 / Math.PI);
        public static float DEG(IConvertible rad) => (float)(Convert.ToDouble(rad) * 180 / Math.PI);
        public static float RAD(float deg) => (float)(deg * Math.PI / 180);
        public static float RAD(int deg) => (float)(deg * Math.PI / 180);
        public static float RAD(IConvertible deg) => (float)(Convert.ToDouble(deg) * Math.PI / 180);

        public static float BasicAngleDEG(float deg)
        {
            float res = deg % 360;
            if (res < 0) res += 360;
            return res;
        }
        public static float BasicAngleRAD(float rad)
        {
            float res = rad % (float)(2 * Math.PI);
            if (res < 0) res += (float)(2 * Math.PI);
            return res;
        }

        public static Vector2 Rotated(this Vector2 vector, float AngleRad)
            => new Vector2((float)(vector.X * Math.Cos(AngleRad) - vector.Y * Math.Sin(AngleRad)),
                           (float)(vector.X * Math.Sin(AngleRad) + vector.Y * Math.Cos(AngleRad)));
        /*{
            //Vector2 unitVector1 = vector / vector.Length();
            //var RotationMatrix = Matrix3x2.CreateRotationZ(Radians);
            var rotVect = Matrix3x2.CreateRotation(AngleRad);
            Vector2 transformed = Vector2.Transform(vector, rotVect);
            return transformed;
        }*/
        public static Vector2 Vector2FromPoints(Point p1, Point p2) => new Vector2(p2.X - p1.X, p2.Y - p1.Y);
        public static Vector2 Part(this Vector2 vector, float ratio) => vector.ToUnitVector() * vector.Length() * ratio;

        public static Point Add(this Point point, Vector2 vector) => new Point(point.X + (int)vector.X, point.Y + (int)vector.Y);
        public static Point Add(this Point point, Vector2 directionVector, float Length) => point.Add(directionVector.ToUnitVector() * Length);
        public static Point Add(this Point point, Point other) => new Point(point.X + other.X, point.Y + other.Y);
        public static Point Substract(this Point point, Point other) => new Point(point.X - other.X, point.Y - other.Y);
        public static Point ToPoint(this PointF point) => new Point((int)point.X, (int)point.Y);

        public static Point CentralPoint(Point a, Point b) => new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        public static Point PartPointBetween(Point p1, Point p2, float part) => p1.Add(new Vector2(p2.X - p1.X, p2.Y - p1.Y) * part);
        public static Point[] CurvePointsPartPointsNotAccurate(Point[] CurvePoints, params float[] Parts)
        {
            Point[] res = new Point[Parts.Length];

            float TotalLength = 0;
            for (int i = 0; i < Parts.Length - 1; i++)
            {
                TotalLength += new Vector2(CurvePoints[i + 1].X - CurvePoints[i].X, CurvePoints[i + 1].Y - CurvePoints[i].Y).Length();
            }

            for (int i = 0; i < Parts.Length; i++)
            {
                if (Parts[i] > 1 || Parts[i] < 0) throw new Exception("Part cannot be out of curve - the value must be between 0 and 1!");
                float lngth = TotalLength * Parts[i];
                int j = 0;
                Vector2 vect;
                while ((vect = new Vector2(CurvePoints[j + 1].X - CurvePoints[j].X, CurvePoints[j + 1].Y - CurvePoints[j].Y)).Length() < lngth)
                {
                    lngth -= vect.Length();
                    j++;
                }
                res[i] = CurvePoints[j].Add(lngth * vect);
            }

            return res;
        }
        //public static Point[] CurveWithTensionPartPoints(Point[] CurvPoints, float Tension, params float[] Parts)
        //{
        //    Point[] res = new Point[Parts.Length];
        //    GraphicsPath path = new GraphicsPath();
        //    path.AddCurve(CurvPoints, Tension);
        //    PointF[] points = path.PathPoints;
        //    for (int i = 0; i < Parts.Length; i++)
        //    {
        //        res[i] = points[Convert.ToInt32(points.Length * Parts[i]) - 1].ToPoint();
        //    }
        //    return res;
        //}
        //public static Point[] CurvePartPoints(Point[] CurvPoints, params float[] Parts)
        //{
        //    Point[] res = new Point[Parts.Length];
        //    Point[] points = CurvePoints(CurvPoints);
        //    for (int i = 0; i < Parts.Length; i++)
        //    {
        //        res[i] = points[Convert.ToInt32(points.Length * Parts[i]) - 1];
        //    }
        //    return res;
        //}
        public static Point[] CurvePartPoints(float Tension, Point[] CurvPoints, params float[] Parts)
        {
            Point[] res = new Point[Parts.Length];
            Point[] points = CurveDrawnPoints(CurvPoints, Tension);
            for (int i = 0; i < Parts.Length; i++)
            {
                try { res[i] = points[Convert.ToInt32(points.Length * Parts[i])]; }
                catch { res[i] = points[Convert.ToInt32(points.Length * Parts[i]) - 1]; }
            }
            return res;
        }
        public static Point[] Moved(this Point[] points, int xMove, int yMove)
        {
            Point[] res = new Point[points.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = points[i].Add(new Point(xMove, yMove));
            }
            return res;
        }
        public static Point[] CurveDrawnPoints(Point[] CurvPoints, float Tension)
        {
            List<Point> res = new List<Point>();
            
            int xMin = 0;
            int xMax = 0;
            int yMin = 0;
            int yMax = 0;
            foreach (Point p in CurvPoints)
            {
                //Point p = CurvePoints[i];
                //Point vect = p.Substract(_centerPoint);
                //Point tensioned = _centerPoint.Add((1 + (0.1F * Tension)) * new Vector2(vect.X, vect.Y));
                if (p.X < xMin) xMin = p.X;
                if (p.X > xMax) xMax = p.X;
                if (p.Y < yMin) yMin = p.Y;
                if (p.Y > yMax) yMax = p.Y;
            }           

            // posun
            int posunX = (xMin < 0) ? -xMin : 0;
            int posunY = (yMin < 0) ? -yMin : 0;

            int Width = xMax + posunX + 1; // +1 index vs. délka
            Width += (int)(Width * Tension * 2); // +rezerva
            int Height = yMax + posunY + 1;
            Height += (int)(Height * Tension * 2);

            GraphicsPath path = new GraphicsPath();
            Point[] points = CurvPoints.Moved(posunX, posunY);
            path.AddCurve(points, Tension);
            //PointF[] rectPoints = path.PathPoints;       // Původně tam bylo tohle

            var bmp = new Bitmap(Width, Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawPath(Pens.AliceBlue, path);

                Point P1, P2;
                for (int i = 0; i < points.Length - 1; i++)
                {
                    P1 = points[i];
                    P2 = points[i + 1];
                    int width = P1.X - P2.X; 
                    int height = P1.Y - P2.Y;
                    // +1 index vs. délka
                    width += (width >= 0) ? 1 : -1;
                    height += (height >= 0) ? 1 : -1;

                    if (Math.Abs(width) <= Math.Abs(height))
                    {
                        if (height <= 0)
                        {
                            for (int y = P1.Y; y <= P2.Y; y++)
                            {
                                if (width <= 0)
                                {
                                    for (int x = P1.X; x <= P2.X; x++)
                                    {
                                        if (bmp.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x - posunX, y - posunY))) res.Add(new Point(x - posunX, y - posunY));
                                    }
                                }
                                else
                                {
                                    for (int x = P1.X; x >= P2.X; x--)
                                    {
                                        if (bmp.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x - posunX, y - posunY))) res.Add(new Point(x - posunX, y - posunY));
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int y = P1.Y; y >= P2.Y; y--)
                            {
                                if (width <= 0)
                                {
                                    for (int x = P1.X; x <= P2.X; x++)
                                    {
                                        if (bmp.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x - posunX, y - posunY))) res.Add(new Point(x - posunX, y - posunY));
                                    }
                                }
                                else
                                {
                                    for (int x = P1.X; x >= P2.X; x--)
                                    {
                                        if (bmp.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x - posunX, y - posunY))) res.Add(new Point(x - posunX, y - posunY));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (width <= 0)
                        {
                            for (int x = P1.X; x <= P2.X; x++)
                            {
                                if (height <= 0)
                                {
                                    for (int y = P1.Y; y <= P2.Y; y++)
                                    {
                                        if (bmp.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x - posunX, y - posunY))) res.Add(new Point(x - posunX, y - posunY));
                                    }
                                }
                                else
                                {
                                    for (int y = P1.Y; y >= P2.Y; y--)
                                    {
                                        if (bmp.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x - posunX, y - posunY))) res.Add(new Point(x - posunX, y - posunY));
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int x = P1.X; x >= P2.X; x--)
                            {
                                if (height <= 0)
                                {
                                    for (int y = P1.Y; y <= P2.Y; y++)
                                    {
                                        if (bmp.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x - posunX, y - posunY))) res.Add(new Point(x - posunX, y - posunY));
                                    }
                                }
                                else
                                {
                                    for (int y = P1.Y; y >= P2.Y; y--)
                                    {
                                        if (bmp.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb()) if (!res.Contains(new Point(x - posunX, y - posunY))) res.Add(new Point(x - posunX, y - posunY));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            bmp.Dispose();
            //for (int i = 0; i < Parts.Length; i++)
            //{
            //    res[i] = points[(int)(points.Length * Parts[i]) - 1].ToPoint();
            //}
            if (res.Count == 0)
            {
                return CurvPoints;
            }
            else
            {
                return res.ToArray().Moved(-posunX, -posunY);
            }
        }

        public static PointF IntersectWithLine(this Line line1, Line line2)
        {
            float[] parms = line1.Parameters.Union(line2.Parameters);
            return Maths.SolveLinear2VarBasicEquationSet(parms[0], parms[1], -parms[2],
                                                         parms[3], parms[4], -parms[5]);
        }
              
        /// <summary>
        /// A method for intersecting a Line with a Curve
        /// </summary>
        /// <param name="line">Input Line</param>
        /// <param name="curvePoints">Leading points of the curve to be intersected</param>
        /// <param name="tension">Tension of the curve</param>
        /// <returns>If returns [-1, -1], line doesn't intersect the path.</returns>
        public static Point IntersectWithCurve(this Line line, Point[] curvePoints, float tension)
        {
            //GraphicsPath gp = new GraphicsPath();
            //gp.AddCurve(curvePoints, tension);
            Point res = new Point(-1, -1);
            //Bitmap bmp = new Bitmap(regionSize.Width, regionSize.Height);
            //Graphics g = Graphics.FromImage(bmp);
            //g.DrawCurve(new Pen(Color.AliceBlue), curvePoints, tension);
            float[] parameters = line.Parameters;
            Point[] CurPts = CurveDrawnPoints(curvePoints, tension);
            
            //foreach (Point p in CurPts)
            //{
            //    if (Math.Round(parameters[0] * p.X + parameters[1] * p.Y + parameters[2], 0, MidpointRounding.AwayFromZero) == 0 /*||
            //        (int)(parameters[0] * p.X + parameters[1] * p.Y + parameters[2] - ((parameters[0] * p.X + parameters[1] * p.Y + parameters[2]) % 10)) == 0*/)
            //    {
            //        res = p;
            //        break;
            //    }
            //}
            //if (res == new Point(-1, -1))
            //{
                Point p = CurPts[0];
                Point q = CurPts[CurPts.Length - 1];
                //bool sgn1 = (parameters[0] * p.X + parameters[1] * p.Y + parameters[2] >= 0);
                //bool sgn2 = (parameters[0] * q.X + parameters[1] * q.Y + parameters[2] >= 0);
                //Point p1, p2;
                //if (sgn1 != sgn2)
                //{
                    //double[] results = new double[CurPts.Length];
                    double res1, res2;
                    //float d1, d2;
                    for (int i = 0; i < CurPts.Length - 1; i++)
                    {
                        p = CurPts[i];
                        q = CurPts[i + 1];
                        /*results[i] =*/ res1 = parameters[0] * p.X + parameters[1] * p.Y + parameters[2];
                        res2 = parameters[0] * q.X + parameters[1] * q.Y + parameters[2];
                        if ((Maths.Sgn(res1) != Maths.Sgn(res2)))
                        {
                            res = line.Intersect(new Line(p, q));
                            break;
                        }
                    }
                //}
            //}
            //for (int x = 0; x < regionSize.Width; x++)
            //{
            //    for (int y = 0; y < regionSize.Height; y++)
            //    {
            //        if (bmp.GetPixel(x, y).ToArgb() == Color.AliceBlue.ToArgb())
            //        {
            //            if (parameters[0] * x + parameters[1] * y + parameters[2] == 0)
            //            {
            //                res = new Point(x, y);
            //                break;
            //            }
            //        }
            //    }
            //}
            //g.Dispose();
            //bmp.Dispose();
            return res;
        }

        public static Point IntersectWithCurve(this Line line, Point[] curvePoints, float tension, Point pointNotToIntersectWith)
        {            
            Point res = new Point(-1, -1);
            
            float[] parameters = line.Parameters;
            Point[] CurPts = CurveDrawnPoints(curvePoints, tension);

            // Pokud je na listu, první body, jež jsou ve středu lisu, budou odebrány
            
                List<Point> cp = CurPts.ToList();
                Point p0 = CurPts[0];
                while (cp[0] == p0) cp.RemoveAt(0);
                CurPts = cp.ToArray();
            
            
            Point p = CurPts[0];
            Point q = CurPts[CurPts.Length - 1];
            
            double res1, res2;
            //float d1, d2;
            for (int i = 0; i < CurPts.Length - 1; i++)
            {
                p = CurPts[i];
                q = CurPts[i + 1];
                /*results[i] =*/
                res1 = parameters[0] * p.X + parameters[1] * p.Y + parameters[2];
                res2 = parameters[0] * q.X + parameters[1] * q.Y + parameters[2];
                if ((Maths.Sgn(res1) != Maths.Sgn(res2)))
                {
                    res = line.Intersect(new Line(p, q));
                    if (res != pointNotToIntersectWith) break;
                }
            }
            
            return res;
        }

        //public static Point Intersect(this GraphicsPath path, Line line, out bool Intersects)
        //{
        //    PointF res = new Point(-1, -1);
        //    PointF[] points = path.PathPoints;
        //    float[] parameters = line.Parameters;
        //    foreach (PointF p in points)
        //    {
        //        if (parameters[0] * p.X + parameters[1] * p.Y + parameters[2] == 0)
        //        {
        //            res = p;
        //            break;
        //        }
        //    }
        //    Intersects = !(res == new PointF(-1, -1));
        //    return res.ToPoint();
        //}
        //public static Point Intersect(this Line line, GraphicsPath path, out bool Intersects)
        //{
        //    PointF res = new Point(-1, -1);
        //    PointF[] points = path.PathPoints;
        //    float[] parameters = line.Parameters;
        //    foreach (PointF p in points)
        //    {
        //        if (parameters[0] * p.X + parameters[1] * p.Y + parameters[2] == 0)
        //        {
        //            res = p;
        //            break;
        //        }
        //    }
        //    Intersects = !(res == new PointF(-1, -1));
        //    return res.ToPoint();
        //}
        //public static Point IntersectWithCurve(this Line line, Point[] CurvPoints, float Tension, out bool Intersects)
        //{
        //    PointF res = new Point(-1, -1);
        //    GraphicsPath path = new GraphicsPath();
        //    path.AddCurve(CurvPoints, Tension);
        //    PointF[] points = path.PathPoints;
        //    float[] parameters = line.Parameters;
        //    foreach (PointF p in points)
        //    {
        //        if (parameters[0] * p.X + parameters[1] * p.Y + parameters[2] == 0)
        //        {
        //            res = p;
        //            break;
        //        }
        //    }
        //    Intersects = !(res == new PointF(-1, -1));
        //    return res.ToPoint();
        //}

        public static LineSegment[] RotatedAndRescaledVeinsLineSegments(this LineSegment[] originalLineSegments, LineSegment originalCenterLineSegment, LineSegment newLineSegment)
        {
            LineSegment[] res = new LineSegment[originalLineSegments.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = originalLineSegments[i].RotatedAndRescaled(originalCenterLineSegment, newLineSegment);
            }
            return res;
        }
        public static LineSegment[] RotatedAndRescaledVeinsLineSegments(this LineSegment[] originalLineSegments, Curve originalCenterCurve, LineSegment newLineSegment)
        {
            LineSegment[] _orig = originalLineSegments.LineSegmentsAdjacentToCurveByOriginPointAlignedToLine(originalCenterCurve);
            LineSegment _center = new LineSegment(originalCenterCurve.CurvePoints[0], originalCenterCurve.CurvePoints[originalCenterCurve.CurvePoints.Length - 1]);
            return _orig.RotatedAndRescaledVeinsLineSegments(_center, newLineSegment);
        }
        private static LineSegment[] LineSegmentsAdjacentToCurveByOriginPointAlignedToLine(this LineSegment[] originals, Curve originalCntr)
        {
            LineSegment[] res = new LineSegment[originals.Length];
            LineSegment centerLine = new LineSegment(originalCntr.CurvePoints[0], originalCntr.CurvePoints[originalCntr.CurvePoints.Length - 1]);
            Point newp;
            for (int i = 0; i < res.Length; i++)
            {
                newp = centerLine.PartPoint(originalCntr.PartPointLocation(originals[i].OriginPoint));
                res[i] = originals[i].MovedFromOriginPointToNewOriginPoint(newp);
            }
            return res;
        }
        private static LineSegment RotatedAndRescaled(this LineSegment segment, LineSegment origCntrLineSegment, LineSegment newCntrLineSegment)
        {
            float ratio = newCntrLineSegment.Length / origCntrLineSegment.Length;
            Point newOrigPoint = newCntrLineSegment.PartPoint(origCntrLineSegment.PartPointLocation(segment.OriginPoint));
            float angle = origCntrLineSegment.AngleBetween(segment);
            return new LineSegment(newOrigPoint, segment.DirectionVector.Rotated(angle), ratio * segment.Length);
        }


        public static Vector2 ToUnitVector(this Vector2 directionVector) => directionVector / directionVector.Length();

        public static void DrawPartLine(this Graphics graphics, Pen pen, Point p1, Point p2, float part)
        {
            graphics.DrawLine(pen, p1, PartPointBetween(p1, p2, part));
        }
        public static void DrawPartLine(this Graphics graphics, Pen pen, int x1, int y1, int x2, int y2, float part)
            => graphics.DrawPartLine(pen, new Point(x1, y1), new Point(x2, y2), part);
        public static void DrawLineSegments(this Graphics graphics, Pen pen, LineSegment[] lineSegments)
        {
            foreach (LineSegment segment in lineSegments)
            {
                graphics.DrawLine(pen, segment.OriginPoint, segment.EndPoint);
            }
        }
        public static void DrawPartLineSegment(this Graphics graphics, Pen pen, LineSegment lineSegment, float part)
            => graphics.DrawPartLine(pen, lineSegment.OriginPoint, lineSegment.EndPoint, part);
        public static void DrawPartLineSegments(this Graphics graphics, Pen pen, LineSegment[] lineSegments, float part)
        {
            foreach (LineSegment segment in lineSegments)
            {
                graphics.DrawPartLine(pen, segment.OriginPoint, segment.EndPoint, part);
            }
        }

        public static float PointsDistance(this Point p1, Point p2) => Convert.ToSingle(Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2)));

        public static float AngleBetween(this Vector2 v1, Vector2 v2) => Convert.ToSingle(Math.Acos(v1.ScalarProduct(v2) / (v1.Length() * v2.Length())));
        public static float ScalarProduct(this Vector2 v1, Vector2 v2) => (v1.X * v2.X) + (v1.Y * v2.Y);

        /*public static Vector2 Times(this Vector2 vect, dynamic number)
        {
            float num = Convert.ToSingle(number);
            return new Vector2(vect.X * num, vect.Y * num);
        }*/
            
    }
}
