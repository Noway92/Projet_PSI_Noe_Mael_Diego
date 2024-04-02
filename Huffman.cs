using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Projet_PSI_Noe_Mael_Diego
{
    public class Huffman
    {
        private List<Noeud> element;
        private MyImage image;

        public Huffman(MyImage image, Dictionary<Pixel, BitArray> dico)
        {
            //Création du constructeur : ces boucles permettent de calculer les fréquences d'apparition des Pixel
            List<Noeud> element = new List<Noeud>();
            this.image = image;
            for(int i=0;i<image.Hauteur;i++)
            {
                for(int j=0;j<image.Largeur;j++)
                {
                    bool test = true;
                    int x;
                    for (x=0; x<element.Count && test==true;x++)
                    {
                        if (element[x].Pixel == image.RGB[i,j])
                        {
                            test = false;
                        }
                    }                 
                    
                    if(test == true)
                    {
                        Noeud a = new Noeud(image.RGB[i, j], 1);
                        element.Add(a);
                    }
                    else
                    {
                        element[x-1].Fréquence = element[x-1].Fréquence + 1;
                    }
                }
            }
            this.element = element;
            Noeud arbre = CreerArbre();
            BitArray bitArray = new BitArray(0);
            //On va creer un dico contenant les Pixel pour clés et des codes binaires pour valeurs associées
            coder_pixel(arbre, dico, bitArray);
        }

        /// <summary>
        /// Trier tous les éléments en fonction de leur fréquence
        /// </summary>
        public void trier_tout()
        { 
            for(int i=0;i<element.Count;i++)
            {
                int min = i;
                for(int j=i;j<element.Count;j++)
                {
                    if (element[j].Fréquence<element[min].Fréquence)
                    {
                        min = j;
                    }
                }
                Noeud inter = element[i];
                element[i] = element[min];
                element[min] = inter;              

            }
        }

        /// <summary>
        /// Pour remettre le nouvel élément à sa place lors de la création de l'arbre
        /// </summary>
        /// <param name="a"></param>
        public void trier_1_élément(Noeud a)
        {


            //Insert a pour la dernière occurence de la boucle dans la création de l'arbre
            if(element.Count==0)
            {
                element.Add(a);
            }
            else
            {
                bool test = false;
                for (int i = 0; i < element.Count && test == false; i++)
                {

                    if (element[i].Fréquence > a.Fréquence)
                    {
                        element.Insert(i, a);
                        test = true;
                    }
                }
                // cette condition permet d'insérer notre élément si il possède la plus grande fréquence de la liste
                if(test == false)
                {
                    element.Add(a);
                }

            }
            


        }

        public Noeud CreerArbre()
        {
            trier_tout();
            List<Noeud> arbre = new List<Noeud>();
            while(element.Count >=2)
            {
                // On crée un noeud avec l'autre constructeur pour commencer à crée les noeuds de l'arbre
                Noeud a = new Noeud(element[0].Fréquence + element[1].Fréquence, element[0], element[1]);
                //Supprime le Noeud 1
                element.Remove(element[0]);
                //Supprime le Noeud 2 qui est devenu le 1
                element.Remove(element[0]);
                trier_1_élément(a);
            }

            return element[0];
        }

        public void coder_pixel(Noeud actuel, Dictionary<Pixel, BitArray> dico,BitArray bitArray)
        {

            bool newBitG = false;;
            bool newBitD = true;
            //Va ajouter la nouvel valeur de bit array pour aller à gauche et à droite
            BitArray newBitArrayG = new BitArray(bitArray.Length + 1);
            BitArray newBitArrayD = new BitArray(bitArray.Length + 1);
            for (int i = 0; i < bitArray.Length; i++)
            {
                newBitArrayG[i] = bitArray[i];
                newBitArrayD[i] = bitArray[i];
            }
            //Ajoute un bit = 1 lorsque que l'on va à droite dans l'abre
            newBitArrayG[bitArray.Length] = newBitG;
            //Ajoute un bit = 0 lorsque que l'on va à gauche dans l'abre
            newBitArrayD[bitArray.Length] = newBitD;
            //Sachant que les noeuds en bas de chaque arbre ne possède pas de noeud gauche et noeud droit, on pourra jouer la dessus pour la récursivité
            if (actuel.Gauche.Gauche==null)
            {
                dico.Add(actuel.Gauche.Pixel, newBitArrayG);
            }
            else
            {
                coder_pixel(actuel.Gauche, dico, newBitArrayG);
            }
            if (actuel.Droit.Droit == null)
            {
                dico.Add(actuel.Droit.Pixel, newBitArrayD);
            }
            else
            {
                coder_pixel(actuel.Droit, dico, newBitArrayD);
            }

        }
 

    }

}
