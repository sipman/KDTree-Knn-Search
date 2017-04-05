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
                new FeatureVector(new int[]{3,8}),
                new FeatureVector(new int[]{2,6}),
                new FeatureVector(new int[]{4,1}),

            });

            categories.AddCategory("Hus", new FeatureVector[]
            {
                new FeatureVector(new int[]{5,2}),
                new FeatureVector(new int[]{6,11}),
                new FeatureVector(new int[]{8,15}),
                new FeatureVector(new int[]{7,14}),

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
                new FeatureVector(new int[]{4,6}),
                new FeatureVector(new int[]{2,3}),
                new FeatureVector(new int[]{1,9})
            };

            Console.WriteLine(KDTree.Search(unkown));


        }

    }


}