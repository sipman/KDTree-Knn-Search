using System;
using System.Collections.Generic;
using System.Linq;

namespace KD_Tree
{
    public class Category
    {
        public string label = String.Empty;
        public List<Feature> features;

        public Category(string label, Feature [] features)
        {
            this.label = label;
            this.features = features.ToList();
        }

    }
}