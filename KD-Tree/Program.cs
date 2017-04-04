using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Drawing;

namespace KD_Tree
{
    internal class Program
    {
        public static void Main(string[] args)
        {

            Categories categories = new Categories();

            categories.AddCategory("Kat", new FeatureVector[]
            {
                new FeatureVector(new int[]{1,9}),
                new FeatureVector(new int[]{1,8}),
                new FeatureVector(new int[]{2,6}),
                new FeatureVector(new int[]{2,1}),

            });

            categories.AddCategory("Hus", new FeatureVector[]
            {
                new FeatureVector(new int[]{11,67}),
                new FeatureVector(new int[]{12,84}),
                new FeatureVector(new int[]{11,22}),
                new FeatureVector(new int[]{13,19}),

            });


            categories.AddCategory("Flaske", new FeatureVector[]
            {
                new FeatureVector(new int[]{76,45}),
                new FeatureVector(new int[]{88,76}),
                new FeatureVector(new int[]{34,123}),
                new FeatureVector(new int[]{22,43}),

            });

            KDtree KDTree = new KDtree(categories);


            FeatureVector[] unkown =
            {
                new FeatureVector(new int[]{2,3}),
                new FeatureVector(new int[]{4,22}),
                new FeatureVector(new int[]{2,3}),
                new FeatureVector(new int[]{1,90})
            };

            KDTree.Search(unkown);


            Console.WriteLine("Kat: "+categories.GetCategory("Kat").votes);
            Console.WriteLine("Hus: "+categories.GetCategory("Hus").votes);
            Console.WriteLine("Flaske: "+categories.GetCategory("Flaske").votes);
        }

    }


}