using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AlgoNature.Components
{
    internal static partial class Generals
    {
        public const string DEFAULT_LOCALE_KEY = "en-US";

        public const double Phi = 1.618033988749894848204586834365638117720309179805762862135448622705260462818902449707207204189391137484754088075386891752;
        public const double GoldenAngleRad = 2.3999632297286533222315555066336;
        public const double LNPhi = 0.48121182505960344749775891342437;        

        public static long UniqueID(this object obj)
        {
            return obj.GetHashCode() + obj.GetType().GetHashCode();
        }

        public static Point[] ReversedPointsWithoutFirst(this Point[] pointArray)
        {
            Point[] res = new Point[pointArray.Length - 1];
            
            for (int i = pointArray.Length - 2; i >= 0; i--)
            {
                res[pointArray.Length - 2 - i] = pointArray[i];
            }

            return res;
        }
        public static Point[] Union(this Point[] a, Point[] b)
        {
            Point[] res = new Point[a.Length + b.Length];
            for (int i = 0; i < res.Length; i++)
            {
                if (i < a.Length) res[i] = a[i];
                else res[i] = b[i - a.Length];
            }
            return res;
        }

        public static float[] Union(this float[] a, float[] b)
        {
            float[] res = new float[a.Length + b.Length];
            for (int i = 0; i < res.Length; i++)
            {
                if (i < a.Length) res[i] = a[i];
                else res[i] = b[i - a.Length];
            }
            return res;
        }

        //public static Line

        public static Color CombineColors(params Color[] aColors)
        {
            //Color result = new Color();
            int a = 0, r = 0, g = 0, b = 0;
            foreach (Color c in aColors)
            {
                a += c.A;
                r += c.R;
                g += c.G;
                b += c.B;
            }
            a /= aColors.Length;
            r /= aColors.Length;
            g /= aColors.Length;
            b /= aColors.Length;
            return Color.FromArgb(a, r, g, b);
        }
        public static Color CombineColors(Color[] aColors, float[] Intensities)
        {
            //Color result = new Color();
            int a = 0, r = 0, g = 0, b = 0;
            float totalIntens = 0;
            for (int i = 0; i < aColors.Length; i++)
            {
                float intensity = 0;
                try { intensity += Intensities[i]; }
                catch { }
                a += (int)(aColors[i].A * intensity);
                r += (int)(aColors[i].R * intensity);
                g += (int)(aColors[i].G * intensity);
                b += (int)(aColors[i].B * intensity);
                totalIntens += intensity;
            }
            a = (int)(a / totalIntens);
            r = (int)(r / totalIntens);
            g = (int)(g / totalIntens);
            b = (int)(b / totalIntens);
            return Color.FromArgb(a, r, g, b);
        }
        public static Color CombineWith(this Color color, Color other, float othersIntensity) 
            => CombineColors(new Color[] { color, other }, new float[] { 1, othersIntensity });

        public static Color DecombineTwoColors(Color resultedColor, Color combinationColor, float intensityOrig, float intensityComb)
        {
            int a = (int)((2 * resultedColor.A - combinationColor.A * intensityComb) / intensityOrig);
            if (a < 0) a = 0;
            int r = (int)((2 * resultedColor.R - combinationColor.R * intensityComb) / intensityOrig);
            if (r < 0) r = 0;
            int g = (int)((2 * resultedColor.G - combinationColor.G * intensityComb) / intensityOrig);
            if (g < 0) g = 0;
            int b = (int)((2 * resultedColor.B - combinationColor.B * intensityComb) / intensityOrig);
            if (b < 0) b = 0;
            // Ošetřit přesně proti záporným hodnotám
            return Color.FromArgb(a, r, g, b);
        }
        public static Color DecombineWith(this Color resColor, Color colorCombinedWith, float othersIntensity)
            => DecombineTwoColors(resColor, colorCombinedWith, othersIntensity, 1);

        //public Bitmap ToBitmap(this Graphics graphics, int width, int height)
        //{
        //    Bitmap bmp = new Bitmap(width, height, graphics);
        //    Image img = Image.

        //}


        ///// <summary>
        ///// Converts float* to float, because there's no definition in C#.
        ///// </summary>
        ///// <param name="num">float* to be converted to float</param>
        ///// <returns></returns>
        //public static float ToFloat(this float num) => num;

        ///// <summary>
        ///// Converts double* to double, because there's no definition in C#.
        ///// </summary>
        ///// <param name="num">double* to be converted to float</param>
        ///// <returns></returns>
        //public static double ToDouble(this double num) => num;

        public static bool Implies(this bool a, bool b)
        {
            if (a && !b) return false;
            else return true;
        }

        public static Point ToPoint(this Vector2 vect)
            => new Point((int)vect.X, (int)vect.Y);
    }
}
