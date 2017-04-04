using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
namespace KD_Tree
{
    public class KDtree
    {
        private List<KDNode> tree;
        private Categories categories;

        public KDtree(Categories categories)
        {
            tree = new List<KDNode>();
            this.categories = categories;
            foreach (Category category in categories.ToArray())
            {
                AddNode(category);
            }

        }

        private void AddNode(Category category)
        {
            foreach (FeatureVector vector in category.features.ToArray())
            {
                AddNode(category.label, vector.point);
            }
        }

        private void AddNode(string label, int [] point)
        {
          tree.Add(new KDNode(label, point));

        }



        private List<KDNode> GetNN(int[] point)
        {
            return GetNN(3, tree, point, 0);
        }

        private List<KDNode> GetNN(int k, int[] point)
        {
            return GetNN(k, tree, point, 0);
        }

        private List<KDNode> GetNN(int k, List<KDNode> leaf, int[] point, int index)
        {
            int numOfNodesInLeaf = leaf.Count;
            if (numOfNodesInLeaf <= k)
            {
                return leaf;
            }
            List<KDNode> newLeaf = new List<KDNode>();
            leaf.Sort((first, second)=>first.point[index].CompareTo(second.point[index]));
            int median = (numOfNodesInLeaf / 2);
            int medianValue = leaf[median].point[index];
            if (point[index] < medianValue )
            {
                newLeaf = leaf.FindAll(e => e.point[index] < medianValue);
            }
            else
            {
                newLeaf = leaf.FindAll(e => e.point[index] >= medianValue);
            }
            if (index + 1 < point.Length)
            {
                index++;
            }
            else
            {
                index = 0;
            }

            return GetNN(k, newLeaf, point, index);
        }


        public void Search(FeatureVector[] vectors)
        {
            Search(3, vectors);
        }

        public void Search(int k, FeatureVector[] vectors)
        {

            foreach (FeatureVector vector in vectors)
            {
                List<KDNode> kNN = GetNN(k, vector.point);
                RecordVotes(kNN.ToArray());
            }
        }

        private void RecordVotes(KDNode[] kNN)
        {
            foreach (KDNode node in kNN)
            {
                categories.CastVote(node.label);
            }
        }

    }

}