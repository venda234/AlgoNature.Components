using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoNature.Components
{
    class Maths
    {
        public static int Sgn(int number)
        {
            //if (number != 0) return number > 0 ? 1 : -1;
            //else return 0;
            return number.CompareTo(0);
        }
        public static int Sgn(float number)
        {
            //if (number != 0) return number > 0 ? 1 : -1;
            //else return 0;
            return number.CompareTo(0);
        }
        public static int Sgn(double number)
        {
            //if (number != 0) return number > 0 ? 1 : -1;
            //else return 0;
            return number.CompareTo(0);
        }

        public static PointF SolveLinear2VarBasicEquationSet(float k_a, float k_b, float k_c, float k_d, float k_e, float k_f)
        {
            //double k_a = Convert.ToDouble(a_box.Text);
            //double k_b = Convert.ToDouble(b_box.Text);
            //double k_c = Convert.ToDouble(c_box.Text);
            //double k_d = Convert.ToDouble(d_box.Text);
            //double k_e = Convert.ToDouble(e_box.Text);
            //double k_f = Convert.ToDouble(f_box.Text);
            float x;
            float y;
            //Brush barva_spatne = new SolidColorBrush(Colors.Red);
            //Brush barva_spravne = new SolidColorBrush(Colors.Green);

            try
            {
                x = (k_c * k_e - k_b * k_f) / (k_a * k_e - k_b * k_d);
                y = (k_c - k_a * x) / k_b; //-(k_c * k_d - k_a * k_f) / (k_b * k_e - k_b * k_d);
                //x = Math.Round(x, 5);
                //y = Math.Round(y, 5);
                //Solution_Text.Text = "x je " + Convert.ToString(x) + " a y je " + Convert.ToString(y) + " .";
                //Solution_Text.Foreground = barva_spravne;
            }
            catch
            {
                //Solution_Text.Text = "Nemá Řešení!";
                //Solution_Text.Foreground = barva_spatne;
                throw new Exception("Doesn't interect!");
            }

            return new PointF(x, y);
        }
    }
}
