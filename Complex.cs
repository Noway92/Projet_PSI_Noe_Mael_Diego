using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_PSI_Noe_Mael_Diego
{
    public class Complex
    {
        private double re;
        private double im;

        public Complex(double re, double im)
        {
            this.re = re;
            this.im = im;
        }

        public double Module()
        {
            return Math.Sqrt(re*re + im*im);

        }
        /// <summary>
        /// Multiplication pour nombres complexes
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public Complex Multiplication(Complex a)
        {
            Complex z = new Complex(0, 0);
            z.re = re*a.re - im*a.im;
            z.im = a.im * re + im * a.re;
            return z;
        }
        /// <summary>
        /// Addition pour nombres complexes
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>

        public Complex Addition(Complex a)
        {
            Complex z = new Complex(0, 0);
            z.re = re+ a.re;
            z.im = a.im + im;
            return z;
        }

        public string toString()
        {
            string rep = re + "+" + im + "i";
            return rep;
        }
        /// <summary>
        /// permet de direcement tester une égalité entre complexe comme suit : Complex a== Complex b 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Complex a, Complex b)
        {
            if (a.re == b.re && a.im == b.im)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// permet de direcement tester une inégalité entre complexe comme suit : Complex a!= Complex b 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Complex a, Complex b)
        {
            if (a.re == b.re && a.im == b.im)
            {
                return false;
            }
            return true;
        }

        public double Re
        {
            get { return this.re; }
            set { this.re = value; }
        }

        public double Im
        {
            get { return this.im; }
            set { this.im = value; }
        }

    }

}
