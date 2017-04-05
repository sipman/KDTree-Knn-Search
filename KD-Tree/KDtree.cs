using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
namespace KD_Tree
{
    public class KDtree
    {
        private List<KDNode> nodes;
        private Categories categories;
        private List<KDLeaf> tree;

        public KDtree(Categories categories)
        {
            nodes = new List<KDNode>();
            tree = new List<KDLeaf>();
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
          nodes.Add(new KDNode(label, point));

        }

        private List<KDNode> BestBinFirst(int[] point)
        {

            return BestBinFirst(3, point);
        }

        private List<KDNode> BestBinFirst(int k, int[] point)
        {
            List<int> path = new List<int>();
            path.Add(-1);
            return BestBinFirst(k, nodes, point, 0, path);
        }

        private List<KDNode> BestBinFirst(int k, List<KDNode> nodes, int[] point, int index, List<int> path)
        {
            int numOfNodesInLeaf = nodes.Count;
            if (numOfNodesInLeaf <= k)
            {
                return nodes;
            }

            KDLeaf newLeaf = (tree.Exists(e => e.Path.SequenceEqual(path.ToArray())))
                                        ? tree.Find(e => e.Path.SequenceEqual(path.ToArray()))
                                        : BuildLeafInTree(nodes, index, path);

            List<KDNode> newNodes = new List<KDNode>();
            if (point[index] < newLeaf.Median && newLeaf.Left != null)
            {
                newNodes = newLeaf.Left.Nodes.ToList();
            }else if (point[index] >= newLeaf.Median && newLeaf.Right != null)
            {
                newNodes = newLeaf.Right.Nodes.ToList();
            }else if (point[index] < newLeaf.Median && newLeaf.Left == null)
            {
                newNodes = nodes.FindAll(e => e.point[index] < newLeaf.Median);
            }else if (point[index] >= newLeaf.Median && newLeaf.Right == null)
            {
                newNodes = nodes.FindAll(e => e.point[index] >= newLeaf.Median);
            }

            int nextStep = (point[index] < newLeaf.Median) ? 0 : 1;
            path.Add(nextStep);

            return BestBinFirst(k, newNodes, point, ((index + 1 < point.Length) ? index+1 : index = 0), path);
        }

        private KDLeaf BuildLeafInTree(List<KDNode> leaf, int index, List<int> path)
        {
            int numOfNodesInLeaf = leaf.Count;
            leaf.Sort((first, second)=>first.point[index].CompareTo(second.point[index]));
            int median = (numOfNodesInLeaf / 2);
            int medianValue = leaf[median].point[index];

            KDLeaf newLeaf = new KDLeaf();
            newLeaf.Median = medianValue;
            newLeaf.Index = index;
            newLeaf.Nodes = leaf.ToArray();
            newLeaf.Path = path.ToArray();

            int[] parentPath = path.Take(path.Count-1).ToArray();
            newLeaf.Parent = (path.Count > 0) ? tree.Find(e => e.Path.SequenceEqual(parentPath)) : null;
            if(path.Count > 1){
                if (path.Last() == 0)
                {
                    newLeaf.Parent.Left = newLeaf;
                }
                else
                {
                    newLeaf.Parent.Right = newLeaf;
                }
            }
            tree.Add(newLeaf);

            return newLeaf;
        }

        public string Search(FeatureVector[] vectors)
        {
            return Search(3, vectors);
        }

        public string Search(int k, FeatureVector[] vectors)
        {

            foreach (FeatureVector vector in vectors)
            {
                List<KDNode> kNN = BestBinFirst(k, vector.point);
                DetermineCloset(kNN.ToArray(), vector);
                //Recrawl kd-tree in x-seconds for better hits
            }
            string result = SelectResult(vectors);

            return result;
        }

        private void DetermineCloset(KDNode[] kNN, FeatureVector vector)
        {
            foreach (KDNode node in kNN)
            {
                int sum = 0;
                for (int i = 0; i < node.point.Length; i++)
                {
                    sum += (node.point[i] - vector.point[i])*(node.point[i] - vector.point[i]);
                }
                double distance = Math.Sqrt(sum);
                if (vector.smallestDistance > distance)
                {
                    vector.smallestDistance = distance;
                    vector.category = node.label;
                }
            }
        }

        private string SelectResult(FeatureVector[] vectors)
        {
            return (from vector in vectors
                group vector by vector.category
                into grp
                orderby grp.Count() descending
                select grp.Key).First();
        }

    }

}