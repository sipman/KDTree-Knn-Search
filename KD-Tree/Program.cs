using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;

namespace KD_Tree
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            Categories categories = new Categories();

            string[] files = Directory.GetFiles("../../Database/");

            foreach (string file in files)
            {
                if(!file.Contains(".DS_Store")){
                    categories.AddCategory(Path.GetFileNameWithoutExtension(file),Persist.Load(file));
                }
            }

            /*categories.AddCategory("Kat", new Feature[]
            {
                new Feature(new double[]{1,9}),
                new Feature(new double[]{3,8}),
                new Feature(new double[]{2,6}),
                new Feature(new double[]{4,1}),

            });

            categories.AddCategory("Hus", new Feature[]
            {
                new Feature(new double[]{5,2}),
                new Feature(new double[]{6,11}),
                new Feature(new double[]{8,15}),
                new Feature(new double[]{7,14}),

            });


            categories.AddCategory("Flaske", new Feature[]
            {
                new Feature(new double[]{76,45}),
                new Feature(new double[]{88,76}),
                new Feature(new double[]{34,123}),
                new Feature(new double[]{22,43}),

            });*/

            KDtree KDTree = new KDtree(categories);


            Feature[] unkown = Persist.Load("../../Database/n03063599_1688.sift");

        /*    foreach(double descr in unkown[0].descr){
                Console.WriteLine(descr);
            }*/
            Console.WriteLine("Start Search...");
            Console.WriteLine(KDTree.MedianSearch(unkown));
            //expected: n03063599_1688.sift

        }

    }


}