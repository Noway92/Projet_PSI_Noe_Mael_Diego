using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using static System.Net.WebRequestMethods;
using System.Diagnostics;


namespace Projet_PSI_Noe_Mael_Diego
{
    public class MyImage
    {
        private string type_image;
        private int taille_fichier;
        private int taille_offset;
        private int largeur;
        private int hauteur;
        private int nombre_bits_couleur;
        private Pixel[,] Rgb;

        public MyImage(string myfile)
        {
            // Lit le fichier BitMap et le écrit entièrement ce fichier dans un tableau de byte
            byte[] Image = System.IO.File.ReadAllBytes(myfile + ".bmp");
            // Va chercher les 2 premières lettre (BM)
            string a = Convert.ToChar(Image[0]) + "";
            string b = Convert.ToChar(Image[1]) + "";
            this.type_image = a + b;

            // Toute les fonctions ci dessous vont juste convertir les bytes en int pour récuppérer les informations utiles du HEADER
            byte[] inter = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                inter[i] = Image[i + 2];
            }
            this.taille_fichier = Convertir_Endian_To_Int(inter);

            for (int i = 0; i < 4; i++)
            {
                inter[i] = Image[i + 14];
            }
            this.taille_offset = Convertir_Endian_To_Int(inter);

            for (int i = 0; i < 4; i++)
            {
                inter[i] = Image[i + 18];
            }
            this.largeur = Convertir_Endian_To_Int(inter);

            for (int i = 0; i < 4; i++)
            {
                inter[i] = Image[i + 22];
            }
            this.hauteur = Convertir_Endian_To_Int(inter);

            byte[] inter2 = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                inter2[i] = Image[i + 28];
            }
            this.nombre_bits_couleur = Convertir_Endian_To_Int(inter2);

            //Passage du tableau de bytes en tableau Pixel
            Pixel[] tabPixel = new Pixel[largeur * hauteur];
            int compteur = 0;
            for (int i = 0; i < Image.Length - 54; i = i + 3)
            {
                tabPixel[compteur] = new Pixel(Image[i + 54], Image[i + 1 + 54], Image[i + 2 + 54]);
                compteur++;
            }
            //Passage du tableau de Pixel en Matrice de Pixel
            compteur = 0;
            Rgb = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    Rgb[i, j] = tabPixel[compteur];
                    compteur++;
                }
            }
        }

        public int TailleFichier
        {
            get { return this.taille_fichier; }
            set { this.taille_fichier = value; }
        }
        public int Hauteur
        {
            get { return this.hauteur; }
            set { this.hauteur = value; }
        }
        public int Largeur
        {
            get { return this.largeur; }
            set { this.largeur = value; }
        }
        public Pixel[,] RGB
        {
            get { return this.Rgb; }
            set { this.Rgb = value; }
        }

        /// <summary>
        /// Convertit du litlle endian vers un entier
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public int Convertir_Endian_To_Int(byte[] tab)
        {
            int a = 0;
            int b = 1;
            for (int i = 0; i < tab.Length; i++)
            {
                a = a + tab[i] * b;
                b = b * 256;
            }
            return a;

        }

        /// <summary>
        /// Permet de reconvertir les attributs de la classe MyImage en fichier BitMap
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>

        public void From_Image_To_File(string file)
        {
            // Création d'un tableau qui va contenir toutes les informations du fichier BitMap que l'on voudra creer
            byte[] tab = new byte[largeur * hauteur * 3 + 54];
            //On travaille dans notre cas toujours sur un fichier BitMap
            tab[0] = 66;
            tab[1] = 77;
            //On va juste convertir tous les entiers en tableau de bytes que l'on introduit ensuite dans "tab"
            byte[] inter = Convertir_Int_To_Little_Endian(taille_fichier, 4);
            for (int i = 0; i < 4; i++)
            {
                tab[i + 2] = inter[i];
            }
            for (int i = 0; i < 8; i++)
            {
                tab[i + 6] = 0;
            }
            inter = Convertir_Int_To_Little_Endian(taille_offset, 4);
            for (int i = 0; i < 4; i++)
            {
                tab[14 + i] = inter[i];
            }
            inter = Convertir_Int_To_Little_Endian(largeur, 4);
            for (int i = 0; i < 4; i++)
            {
                tab[18 + i] = inter[i];
            }
            inter = Convertir_Int_To_Little_Endian(hauteur, 4);
            for (int i = 0; i < 4; i++)
            {
                tab[22 + i] = inter[i];
            }
            tab[26] = 1;
            tab[27] = 0;
            inter = Convertir_Int_To_Little_Endian(nombre_bits_couleur, 2);
            for (int i = 0; i < 2; i++)
            {
                tab[28 + i] = inter[i];
            }
            for (int i = 0; i < 24; i++)
            {
                tab[30 + i] = 0;
            }
            //Passage de la matrice de Pixel en tableau de Pixel
            Pixel[] tab_pixel = new Pixel[largeur * hauteur];
            int compteur = 0;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    tab_pixel[compteur] = RGB[i, j];
                    compteur++;
                }
            }
            compteur = 0;
            for (int i = 0; i < largeur * hauteur * 3; i = i + 3)
            {
                tab[54 + i] = tab_pixel[compteur].Red;
                tab[54 + i + 1] = tab_pixel[compteur].Green;
                tab[54 + i + 2] = tab_pixel[compteur].Blue;
                compteur++;
            }
            //Création du fichier BitMap
            BinaryWriter bw = new BinaryWriter(System.IO.File.Create(file + ".bmp"));
            for (int i = 0; i < tab.Length; i++)
            {
                bw.Write(tab[i]);
            }

            bw.Close();
        }
        /// <summary>
        /// Conversion direct en litlle endian
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public byte[] Convertir_Int_To_Little_Endian(int val, int nb_octets)
        {

            //En initialisant le tableau toutes les valeurs sont déjà 0
            byte[] tab = new byte[nb_octets];
            float a = float.Parse(val + "");
            for (int i = 0; i < tab.Length && a != 0; i++)
            {
                tab[i] = (byte)(a % 256);
                a = float.Parse((byte)(a / 256) + "");
            }
            return tab;
        }
        /// <summary>
        /// Permet de faire la rotation d'une image en donnant l'angle en degré
        /// </summary>
        /// <param name="angle"></param>
        public void Rotation(double angle)
        {
            double angleRadians = angle * Math.PI / 180.0;

            //Utile pour définir les coordonnées du repère de l'image de base
            double centreX = largeur / 2;
            double centreY = hauteur / 2;

            // Calcule de la nouvelle hauteur et de la nouvelle largeur
            int new_hauteur = (int)(Math.Abs(Math.Sin(angleRadians) * largeur) + Math.Abs(Math.Cos(angleRadians) * hauteur));
            int new_largeur = (int)(Math.Abs(Math.Sin(angleRadians) * hauteur) + Math.Abs(Math.Cos(angleRadians) * largeur));

            //Ces nouvelles valeurs doivent forcément être multiple de 4
            if (new_hauteur % 4 != 0)
            {
                new_hauteur += 4 - (new_hauteur % 4);
            }
            if (new_largeur % 4 != 0)
            {
                new_largeur += 4 - (new_largeur % 4);
            }

            //On crée une matrice que l'on va remplir pour creer la nouvelle de matrice de Pixel
            Pixel[,] tab = new Pixel[new_hauteur, new_largeur];
            //Utile pour définir les coordonnées du nouveau repère de l'image après rotation
            double new_centreX = new_largeur / 2;
            double new_centreY = new_hauteur / 2;

            // On fait dans le sens opposé de la logique, on prend la matrice d'arrivée et on cherche son antécédent si il existe
            for (int i = 0; i < new_hauteur; i++)
            {
                for (int j = 0; j < new_largeur; j++)
                {
                    // On calcule les coordonnées antécédentes des coordonnées actuelles i et j
                    int x = (int)Math.Round((Math.Cos(angleRadians) * (j - new_centreX)) - (Math.Sin(angleRadians) * (i - new_centreY)) + centreX);
                    int y = (int)Math.Round((Math.Sin(angleRadians) * (j - new_centreX)) + (Math.Cos(angleRadians) * (i - new_centreY)) + centreY);
                    if (x >= 0 && x < largeur && y >= 0 && y < hauteur)
                    {
                        tab[i, j] = Rgb[y, x];
                    }
                    // Quand l'image tourne il y aura forcément l'apparition de Pixel noir
                    else
                    {
                        tab[i, j] = new Pixel(0, 0, 0);
                    }

                }
            }
            // On modifie les valeurs qui ont été changés lors du changement de matrice de pixel
            hauteur = new_hauteur;
            largeur = new_largeur;
            taille_fichier = new_hauteur * new_largeur * 3 + 54;
            Rgb = tab;
        }

        public void Agrandissement(double agrandissement)
        {
            while (agrandissement == 0)
            {
                Console.WriteLine("Il est impossible d'utiliser 0 en coefficient d'agrandissement");
                agrandissement = Convert.ToDouble(Console.ReadLine());
            }
            int new_hauteur = (int)(hauteur * agrandissement);
            int new_largeur = (int)(largeur * agrandissement);

            // même logique que pour la rotation, on veut des multiples de 4

            if (new_hauteur % 4 != 0)
            {
                new_hauteur += 4 - (new_hauteur % 4);
            }
            if (new_largeur % 4 != 0)
            {
                new_largeur += 4 - (new_largeur % 4);
            }
            //On crée une matrice que l'on va remplir pour creer la nouvelle de matrice de Pixel
            Pixel[,] tab = new Pixel[new_hauteur, new_largeur];
            for (int i = 0; i < new_hauteur; i++)
            {
                for (int j = 0; j < new_largeur; j++)
                {
                    // On calcule les coordonnées des antécédents
                    int y = (int)(i / agrandissement);
                    int x = (int)(j / agrandissement);
                    if (x >= 0 && x < largeur && y < hauteur && y >= 0)
                    {
                        tab[i, j] = Rgb[y, x];
                    }
                    else
                    {
                        tab[i, j] = new Pixel(0, 0, 0);
                    }


                }
            }
            // On modifie les valeurs qui ont été changés lors du changement de matrice de pixel
            hauteur = new_hauteur;
            largeur = new_largeur;
            taille_fichier = new_hauteur * new_largeur * 3 + 54;
            Rgb = tab;

        }
        /// <summary>
        /// Cette fonction permet de convertir des float en byte  
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public byte Float_To_Byte(float a)
        {
            if (a < 0)
            {
                a = 0;
            }
            if (a > 255)
            {
                a = 255;
            }
            return Convert.ToByte((int)a);
        }

        public void Convolution(int possibilité)
        {
            int[,] tab = new int[3, 3];
            switch (possibilité)
            {
                //Repoussage
                case 0:
                    tab = new int[3, 3] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
                    break;
                //Flou
                case 1:
                    tab = new int[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
                    break;
                //Renforcement des bords
                case 2:
                    tab = new int[3, 3] { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
                    break;
                // détection des bords
                case 3:
                    tab = new int[3, 3] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
                    break;
            }
            //On crée une matrice que l'on va remplir pour creer la nouvelle de matrice de Pixel
            Pixel[,] resultat = new Pixel[hauteur, largeur];

            // On veut parcourir chaque pixel de la matrice pour appliquer le filtre
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    float red = 0;
                    float blue = 0;
                    float green = 0;
                    //Compteur utile pour connaitre la ligne du pixel de la matrice de flou que l'on va devoir utiliser
                    int compteur1 = 0;
                    // Compteur utile pour savoir par combien l'on va devoir diviser au final
                    int compteur = 0;
                    for (int x = i - 1; x <= i + 1; x++)
                    {
                        //Compteur utile pour connaitre la colonne du pixel de la matrice de flou que l'on va devoir utiliser
                        int compteur2 = 0;
                        for (int y = j - 1; y <= j + 1; y++)
                        {
                            //On applique le filtre seulement si les Pixels sont dans la matrice
                            if (x >= 0 && x < hauteur && y >= 0 && y < largeur)
                            {
                                red += (Rgb[x, y].Red) * tab[compteur1, compteur2];
                                green += (Rgb[x, y].Green) * tab[compteur1, compteur2];
                                blue += (Rgb[x, y].Blue) * tab[compteur1, compteur2];
                                compteur += tab[compteur1, compteur2];
                            }

                            compteur2++;
                        }
                        compteur1++;
                    }

                    if (compteur == 0)
                    {
                        compteur = 1;
                    }
                    //On divise par le nombre d'occurence que l'on a réalisé
                    red = red / compteur;
                    green = green / compteur;
                    blue = blue / compteur;


                    resultat[i, j] = new Pixel(Float_To_Byte(red), Float_To_Byte(green), Float_To_Byte(blue));
                }
            }
            Rgb = resultat;
        }

        public void Fractal()
        {
            //On crée une matrice que l'on va remplir pour creer la nouvelle de matrice de Pixel
            Pixel[,] resultat = new Pixel[hauteur, largeur];
            //Nombre à partir du quel on considère que l'on est en +infini
            int iteration_max = 50;
            int choix = -1;
            do
            {
                Console.WriteLine("Tapez 1, 2 ou 3 pour obtenir différentes fractales");
                try
                {
                    choix = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (choix != 1 && choix != 2 && choix != 3);

            Complex c = new Complex(0,0);
            switch(choix)
            {
                case 1:
                    c = new Complex(-0.8, 0.156); //celle de base
                    break;
                case 2:
                    c = new Complex(0.39, 0.2); //elle est stylée
                    break;
                case 3:
                    c = new Complex(-0.8,0.2) ;  
                    break;


            }

            
            //Parcours de chaque Pixel de la matrice
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    //On initialise Z0 : le complexe à utiliser dans la formule pour la suite
                    double reel = (i - hauteur / 2) / (0.5 * hauteur);
                    double imaginaire = (j - largeur / 2) / (0.5 * largeur);

                    Complex z = new Complex(reel, imaginaire);
                    int n = 0;
                    //On fait la suite en utilisant la formule Zn+1 = Zn^2 + c
                    do
                    {
                        z = z.Multiplication(z);
                        z = z.Addition(c);
                        n++;

                    } while (n < iteration_max && z.Module() <= 2);
                    
                    // Si le module de z n'a jamais dépassé 2
                    if (n == iteration_max)
                    {
                        //Noir
                        resultat[i,j] = new Pixel(0, 0, 0);
                    }
                    else
                    {
                        //On met une autre couleur (cacul pour faire un dégrader sympa)
                        double t = (double)n / iteration_max;
                        int r = (int)(10 * (1 - t) * Math.Pow(t, 3) * 255);
                        int g = (int)(15 * Math.Pow(t, 2) * Math.Pow(1 - t, 2) * 255);
                        int b = (int)(8.5 * Math.Pow(1 - t, 3) * t * 255);
                        resultat[i,j] = new Pixel((byte)r, (byte)g, (byte)b);
                        //resultat[i, j] = new Pixel(255, 255, 255);
                    }

                }
            }
            Rgb = resultat;
        }   
        
        /// <summary>
        /// On cache une image dans une image
        /// </summary>
        public void Steganographie(string image_de_base)
        {
                       
            string reponse;
            do
            {
                Console.WriteLine("Quelle image voulez vous cachez dans "+image_de_base+" ? \nlena, coco, lac ou Test?");
                reponse = Console.ReadLine();
            } while (reponse != "lena" && reponse != "coco" && reponse != "lac" && reponse != "Test");         

          
            // On va créé 2 instance de my image pour pouvoir comparer les 2 images et voir si l'on peut ou pas cacher une image dans l'autre
            MyImage image_initiale = new MyImage(image_de_base);
            MyImage image_cachée = new MyImage(reponse);

            double agrandissement_hauteur = image_cachée.hauteur / image_initiale.hauteur;
            double agrandissement_largeur = image_cachée.largeur / image_initiale.largeur;

            //On calcule le max des 2
            double agrandissement_max = agrandissement_hauteur;
            if(agrandissement_largeur>agrandissement_hauteur)
            {
                agrandissement_max = agrandissement_largeur; 
            }

            // on augmente la taille de l'image initiale pour pouvoir cacher toute l'image que l'utilisateur a demandé si l'image demandé est trop grande
            if(agrandissement_max>1)
            {
                image_initiale.Agrandissement(agrandissement_max);
                hauteur = image_initiale.Hauteur;
                largeur = image_initiale.Largeur;
                taille_fichier = hauteur * largeur * 3 + 54;
                Rgb = image_initiale.RGB;
            }

            //On crée une matrice que l'on va remplir pour creer la nouvelle de matrice de Pixel
            Pixel[,] resultat = new Pixel[hauteur, largeur];
            byte r = 0;
            byte v = 0;
            byte b = 0;
            for (int i=0;i<hauteur;i++)
            {
                
                for(int j =0;j<largeur;j++)
                {
                    r = 0;
                    v = 0;
                    b = 0;
                    // On prend les binaires fort de l'image de base
                    int[] image_de_base_rouge = binairefort(Rgb[i,j].Red);
                    int[] image_de_base_vert = binairefort(Rgb[i, j].Green);
                    int[] image_de_base_bleu = binairefort(Rgb[i, j].Blue);

                    
                    int[] image_cachée_rouge = new int[4] { 0, 0, 0, 0};
                    int[] image_cachée_vert = new int[4] { 0, 0, 0, 0 };
                    int[] image_cachée_bleu = new int[4] { 0, 0, 0, 0 };
                    
                    if (i<image_cachée.Hauteur && j<image_cachée.Largeur)
                    {
                        //On prend les binaires forts de l'image que l'on veut cacher
                        image_cachée_rouge = binairefort(image_cachée.Rgb[i, j].Red);
                        image_cachée_vert = binairefort(image_cachée.Rgb[i, j].Green);
                        image_cachée_bleu = binairefort(image_cachée.Rgb[i, j].Blue);
                    }
                    // On calcule la nouvelle valeur du Pixel
                    for(int h =0;h<8;h++)
                    {
                        //On remplace les bits faibles par les bits de poids fort de l'image cachée
                        if(h<4)
                        {
                            r += Convert.ToByte(image_cachée_rouge[3 - h] * Math.Pow(2, h));
                            v += Convert.ToByte(image_cachée_vert[3 - h] * Math.Pow(2, h));
                            b += Convert.ToByte(image_cachée_bleu[3 - h] * Math.Pow(2, h));
                        }
                        //On remplace les bits forts par les bits de poids fort de l'image initiale
                        else
                        {
                            r += Convert.ToByte(image_de_base_rouge[7 - h] * Math.Pow(2, h));
                            v += Convert.ToByte(image_de_base_vert[7 - h] * Math.Pow(2, h));
                            b += Convert.ToByte(image_de_base_bleu[7 - h] * Math.Pow(2, h));
                        }
                        
                    }
                    resultat[i, j] = new Pixel(r, v, b);
                }
                
            }
            Rgb = resultat;

        }

        /// <summary>
        /// Prendre les 4 bits les plus forts
        /// </summary>
        /// <param name="valeur"></param>
        /// <returns></returns>
        public int[] binairefort(int valeur)
        {
            //On va mettre dans un tableau les 4 bits les plus forts des 8 de bases
            int[] tab = new int [4];
            for (int i = 0; i < 8; i++)
            {
                if(i>=4)
                {
                    
                    tab[7-i] = valeur % 2;
                }
                valeur = valeur / 2;
            }
            return tab;
        }

        /// <summary>
        /// Prendre les 4 bits les plus faibles
        /// </summary>
        /// <param name="valeur"></param>
        /// <returns></returns>
        public int[] binairefaible(int valeur)
        {
            //On va mettre dans un tableau les 4 bits les plus faibles des 8 de bases
            int[] tab = new int[4];
            for (int i = 0; i < 4; i++)
            {
                
                tab[3 - i] = valeur % 2;
                valeur = valeur / 2;
            }
            return tab;
        }

        /// <summary>
        /// Retourne l'image cachée dérrière l'image de base
        /// </summary>
        /// <returns></returns>
        public void trouver()
        {
            //On crée une matrice que l'on va remplir pour creer la nouvelle de matrice de Pixel
            Pixel[,] image_cachée = new Pixel[hauteur, largeur];
            byte r_cachée = 0;
            byte v_cachée = 0;
            byte b_cachée = 0;
            for(int i =0;i<hauteur;i++)
            {
                for(int j =0;j<largeur;j++)
                {
                    // On vient chercher les bits de poids faibles de l'image 
                    r_cachée = 0;
                    v_cachée = 0;
                    b_cachée = 0;
                    int[] image_rouge = binairefaible(Rgb[i, j].Red);
                    int[] image_vert = binairefaible(Rgb[i, j].Green);
                    int[] image_bleu = binairefaible(Rgb[i, j].Blue);
                    //On trouve l'image cachée en transformant les bits de poids faibles en bits de poids forts
                    for(int h=4;h<8;h++)
                    {
                        r_cachée += Convert.ToByte(image_rouge[7 - h] * Math.Pow(2, h));
                        v_cachée += Convert.ToByte(image_vert[7 - h] * Math.Pow(2, h));
                        b_cachée += Convert.ToByte(image_bleu[7 - h] * Math.Pow(2, h));
                    }
                    image_cachée[i,j]=new Pixel(r_cachée,v_cachée,b_cachée);
                }
            }

            Rgb = image_cachée;                            
        }

        public BitArray[,] compresser(Dictionary<Pixel, BitArray> dico)
        {
            //Va creer la matrice de BitArray remplaçant la matrice de Pixel après algorithme d'huffman
            BitArray[,] Rgb_compressée = new BitArray[hauteur, largeur];
            for (int i=0;i<hauteur;i++)
            {
                for(int j=0;j<largeur;j++)
                {
                    /*
                    Autre méthode
                    BitArray value;
                    //On vient chercher la valeur de la clé associé à Rgb[i,j]
                    dico.TryGetValue(Rgb[i, j], out value);
                    Rgb_compressée[i, j] = value;

                    */

                    //On vient remplacer chaque Pixel par son équivalent en bit après huffman
                    foreach(KeyValuePair<Pixel, BitArray> kvp in dico)
                    {
                        if(kvp.Key == Rgb[i,j])
                        {
                            Rgb_compressée[i,j] = kvp.Value;
                        }
                    }
                    
                }
            }

            //On rentre toutes cette matrice sur un fichier binnaire
            BinaryWriter bw = new BinaryWriter(System.IO.File.Create("Compressé"));
            for (int i = 0; i < Rgb_compressée.GetLength(0); i++)
            {
                for(int j=0; j<Rgb_compressée.GetLength(1);j++)
                {
                    //On écrit chaque bit dans notre fichier
                    foreach(bool x in Rgb_compressée[i,j])
                    {
                        bw.Write(x);
                    }                

                }               
            }
            bw.Close();
            return Rgb_compressée;
        }

        public void décompresser(Dictionary<Pixel, BitArray> dico, BitArray[,] compressé)
        {
            //BitArray Image = System.IO.File.ReadAllBytes("Compressé");
            
            for (int i=0;i<hauteur;i++)
            {
                for(int j =0;j<largeur;j++)
                {
                    foreach (KeyValuePair<Pixel, BitArray> kvp in dico)
                    {
                        if (kvp.Value == compressé[i,j])
                        {
                            Rgb[i,j] = kvp.Key;
                        }
                    }
                }
            }

        }

        public void jeu_des_couleurs()
        {
            Console.WriteLine("Selectionner le carré de couleur différente en donnant son numéro");
            bool stop = false;
            // 15 niveau différent
            int niveau = 1;
            do
            {
                //Initialement notre matrice est une matrice de taille 3*3                
                hauteur = 3;
                largeur = 3;
                taille_fichier = 81;
                Rgb = new Pixel[3, 3];
                Console.WriteLine("Niveau : " + niveau);
                //On va choisir la couleur et le pixel de couleur différente de manière aléatoire
                Random r = new Random();
                int choix = r.Next(1, 10);
                int couleur = r.Next(0, 3);
                int compteur_couleur_différente = 1;
                //On remplit les pixels avec leurs bonne couleurs
                for(int i =0;i<3;i++)
                {
                    for(int j =0;j<3;j++)
                    {   
                        //Pixel différent
                        if(choix==compteur_couleur_différente)
                        {
                            switch (couleur)
                            {
                                case 0:
                                    Rgb[i, j] = new Pixel((byte)(235+niveau), 0, 0);
                                    break;
                                case 1:
                                    Rgb[i, j] = new Pixel(0, (byte)(235+niveau), 0);
                                    break;
                                case 2:
                                    Rgb[i, j] = new Pixel(0, 0, (byte)(235+niveau));
                                    break;
                            }

                        }
                        //Pixel basique
                        else
                        {
                            switch (couleur)
                            {
                                case 0:
                                    Rgb[i, j] = new Pixel(255, 0, 0);
                                    break;
                                case 1:
                                    Rgb[i, j] = new Pixel(0,255, 0);
                                    break;
                                case 2:
                                    Rgb[i, j] = new Pixel(0, 0, 255);
                                    break;
                            }

                        }
                        compteur_couleur_différente++;
                       
                    }
                }
                // On veut une image de plus grande taille
                Agrandissement(100);
                From_Image_To_File("Essai");
                Process.Start("Essai.bmp");
                //Intéraction avec l'utilisateur
                int rep = -1;
                do
                {
                    try
                    {
                        Console.WriteLine("donner le numéro du Pixel différent entre 1 et 9:\nle carré en haut à droite est le numéro 1, celui à sa droite le numéro 2 ..., le numéro 9 est en haut à droite");
                        rep = Convert.ToInt32(Console.ReadLine());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                } while (rep <= 0 || rep > 9);
                

                if(rep==choix)
                {
                    niveau++;
                    //On arrête la dernière couleur à un byte près par rapport aux autres couleurs
                    if(niveau==20)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.Clear();
                        // On attend 1 seconde pour pouvoir laisser l'écran bleu
                        DateTime attendre = DateTime.Now;
                        while ((DateTime.Now - attendre).TotalSeconds < 2)
                        {

                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Clear();
                        Console.WriteLine("Vous avez fini le jeu BRAVO!!!");
                        stop = true;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Clear();
                        // On attend 1 seconde pour pouvoir laisser l'écran vert
                        DateTime attendre = DateTime.Now;
                        while ((DateTime.Now - attendre).TotalSeconds < 2)
                        {

                        }
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Clear();
                    }
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Clear();
                    // On attend 1 seconde pour pouvoir laisser l'écran rouge
                    DateTime attendre = DateTime.Now;
                    while ((DateTime.Now - attendre).TotalSeconds < 2)
                    {

                    }
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Clear();
                    Console.WriteLine("Fin du jeu");
                    stop = true;
                }               


            } while (stop == false);
            


        }

    }
}
