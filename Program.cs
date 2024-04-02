using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlTypes;
using System.Collections;

namespace Projet_PSI_Noe_Mael_Diego
{
    public class Program
    {
        
        static void Main(string[] args)
        { 
            // Nous permet de travailler sur toutes les images
            string reponse;
            do
            {
                Console.WriteLine("Sur quelles images voulez-vous travailler \nlena, coco, lac ou Test?");
                reponse = Console.ReadLine();
            } while (reponse != "lena" && reponse != "coco" && reponse != "lac" && reponse != "Test");            
            MyImage Test = new MyImage(reponse);
            Console.Clear();
            
            // Nous permettra de réaliser toutes les différentes actions sur notre image
            int choix = -1;
            do
            {
                Console.WriteLine("Si vous voulez faire une rotation taper 1\nSi vous voulez faire un agrandissement taper 2\nSi vous voulez faire une convolution taper 3\nSi vous voulez voir une fractale taper 4\nSi vous voulez voir de la stegenographie taper 5\nSi vous voulez compresser votre image avec Huffman taper 6\nSi vous voulez jouer au jeu des couleurs taper 7");
                try
                {
                    choix = Convert.ToInt32(Console.ReadLine());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }while(choix != 1 && choix!=2 && choix!=3 && choix!=4 && choix!=5 && choix!=6 && choix!=7);
            Console.Clear();
            switch (choix)
            {
                // ROTATION
                case 1:
                    
                    // l'angle peut prendre toutes les valeurs de réel que l'on veut, même négative
                    double angle = -1;
                    bool test;
                    do
                    {
                        test = true;
                        Console.WriteLine("Donner l'angle de rotation de votre image en degré : ");
                        try
                        {
                            angle = Convert.ToDouble(Console.ReadLine());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            test = false;
                        } 
                    } while(test==false);
                    Test.Rotation(angle);
                    break;
                //AGRANDISSEMENT
                case 2:
                    double coef = -1;
                    do
                    {
                        Console.WriteLine("Donner le coefficient d'agrandissement : ");
                        try
                        {
                            coef = Convert.ToDouble(Console.ReadLine());
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    } while (coef <= 0);
                    Test.Agrandissement(coef);
                    break;
                // CONVOLUTION
                case 3:
                    int conv = -1;
                    do
                    {
                        Console.WriteLine("Donner le type de convolution que vous voulez faire :\nUn repoussage : écrivez 0\nUn flou : écrivez 1\nUn renforcement des bords : écrivez 2\nUne détection des bords : écrivez 3");
                        try
                        {
                            conv = Convert.ToInt32(Console.ReadLine());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    } while (conv != 0 && conv != 1 && conv != 2 && conv != 3);
                    Test.Convolution(conv);
                    break;
                //FRACTAL
                case 4:
                    Test.Fractal();
                    break;
                //STEGANOGRAPHIE
                case 5:
                    Test.Steganographie(reponse);
                    Test.From_Image_To_File("Essai");
                    Process.Start("Essai.bmp");
                    
                    // On attend 5 secondes pour pouvoir montrer l'image cacher
                    DateTime attendre = DateTime.Now;
                    while ((DateTime.Now - attendre).TotalSeconds < 5)
                    {

                    }
                    Test.trouver();
                    break;
                //HUFFMAN
                case 6:
                    
                    Dictionary<Pixel, BitArray> dico = new Dictionary<Pixel, BitArray>();
                    Huffman huffman = new Huffman(Test, dico);
                    Test.décompresser(dico, Test.compresser(dico));
                    break;
                //INNOVATION
                case 7:
                    MyImage Image = new MyImage("Test");
                    Image.jeu_des_couleurs();
                    break;

            } 
            //Inutilse d'afficher l'image lors de l'innovation
            if(choix!=7)
            {
                Test.From_Image_To_File("Essai");
                Process.Start("Essai.bmp");
            }                                           
            Console.ReadKey();


        }
    }
}
