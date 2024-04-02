using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_PSI_Noe_Mael_Diego
{
    public class Noeud
    {
        private Pixel pixel;
        private int fréquence;
        private Noeud gauche;
        private Noeud droit;

        /// <summary>
        /// Noeud utile pour creer la liste initiale
        /// </summary>
        /// <param name="pixel"></param>
        /// <param name="fréquence"></param>
        public Noeud(Pixel pixel, int fréquence)
        {
            this.pixel = pixel;
            this.fréquence = fréquence;
            this.Droit = null;
            this.Gauche = null;
            
        }

        /// <summary>
        /// Noeud utile pour creer l'abre 
        /// </summary>
        /// <param name="fréquence"></param>
        /// <param name="gauche"></param>
        /// <param name="droit"></param>
        public Noeud(int fréquence,Noeud gauche,Noeud droit)
        {
            this.pixel = null;
            this.fréquence = fréquence;
            this.gauche = gauche;
            this.droit = droit;

        }


        public Pixel Pixel
        {
            get { return this.pixel; }
            set { this.pixel = value; }
        }

        public int Fréquence
        {
            get { return this.fréquence; }
            set { this.fréquence = value; }
        }

        public Noeud Gauche
        {
            get { return this.gauche; }
            set { this.gauche=value; }
        }

        public Noeud Droit
        {
            get { return this.droit;}
            set { this.droit=value; }
        }
    }
}
