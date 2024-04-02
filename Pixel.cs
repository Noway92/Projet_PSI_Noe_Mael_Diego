using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_PSI_Noe_Mael_Diego
{
    public class Pixel
    {
        private byte red;
        private byte green;
        private byte blue;

        public Pixel(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }

        public byte Red
        {
            get { return this.red; }
            set { this.red = value; }
        }
        public byte Green
        {
            get { return this.green; }
            set { this.green = value; }
        }
        public byte Blue
        {
            get { return this.blue; }
            set { this.blue = value; }
        }
        public string toString()
        {
            string a = "red : " + red + "\ngreen : " + green + "\nblue : " + blue;
            return a;
        }
        /// <summary>
        /// permet de direcement tester une égalité entre Pixel comme suit : Pixel a == Pixel b 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator== (Pixel a, Pixel b)
        {
            if (a.blue == b.blue && a.red == b.red && a.green==b.green)
            {
                return true;
            }
            return false;

        }
        /// <summary>
        /// permet de direcement tester une inégalité entre Pixel comme suit : Pixel a != Pixel b 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator!= (Pixel a, Pixel b)
        {
            if (a.blue == b.blue && a.red == b.red && a.green == b.green)
            {
                return false;
            }
            return true;

        }
        

    }
}
