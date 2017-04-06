using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Security.Policy;

namespace KD_Tree
{
    public class KDtree
    {
        private List<KDNode> nodes;
        private Categories categories;
        private List<KDLeaf> tree;
        private int[] BbfPath;

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
                BbfPath = path.ToArray();
                return nodes;
            }

            KDLeaf newLeaf = (tree.Exists(e => e.Path.Length == path.Count && e.Path.SequenceEqual(path.ToArray())))
                                        ? tree.Find(e => e.Path.Length == path.Count && e.Path.SequenceEqual(path.ToArray()))
                                        : BuildLeafInTree(nodes, index, path);


            List<KDNode> newNodes = new List<KDNode>();
            if (point[index] < newLeaf.MedianValue && newLeaf.Left != null)
            {
                newNodes = newLeaf.Left.Nodes.ToList();
            }else if (point[index] >= newLeaf.MedianValue && newLeaf.Right != null)
            {
                newNodes = newLeaf.Right.Nodes.ToList();
            }else if (point[index] < newLeaf.MedianValue && newLeaf.Left == null)
            {
                newNodes = nodes.FindAll(e => e.point[index] < newLeaf.MedianValue);
            }else if (point[index] >= newLeaf.MedianValue && newLeaf.Right == null)
            {
                newNodes = nodes.FindAll(e => e.point[index] >= newLeaf.MedianValue);
            }

            int nextStep = (point[index] < newLeaf.MedianValue) ? 0 : 1;
            path.Add(nextStep);

            return BestBinFirst(k, newNodes, point, ((index + 1 < point.Length) ? index+1 : index = 0), path);
        }


        private KDLeaf BuildLeafInTree(List<KDNode> remainingNodes, int index, List<int> path)
        {
            int numOfNodesInLeaf = remainingNodes.Count;

            remainingNodes.Sort((first, second)=>first.point[index].CompareTo(second.point[index]));

            int median = numOfNodesInLeaf / 2;

            //median--;

            int medianValue = remainingNodes[median].point[index];

            KDLeaf newLeaf = new KDLeaf();
            newLeaf.MedianValue = medianValue;
            newLeaf.Median = remainingNodes[median];
            newLeaf.Index = index;
            newLeaf.Nodes = remainingNodes.ToArray();
            newLeaf.Path = path.ToArray();

            int[] parentPath = path.Take(path.Count-1).ToArray();
            newLeaf.Parent = (path.Count > 0) ? tree.Find(e => e.Path.Length == parentPath.Length && e.Path.SequenceEqual(parentPath)) : null;
            if(path.Count > 1){
                if (path.Last() == 0)
                {
                    if(newLeaf.Parent.Left != null ) Console.WriteLine(newLeaf.Parent.Left.Median);
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

        private bool NNTreeSearch(FeatureVector vector)
        {
            List<int> path = new List<int>();
            path.Add(-1);
            return NNTreeSearch(vector, path, nodes.Count);
        }

        private bool NNTreeSearch(FeatureVector vector, List<int> path, int nodesLeft)
        {
            if (tree.Count <= 0)
            {
                BuildLeafInTree(nodes, 0, path);
            }

            int[] parentPath = path.Take(path.Count-1).ToArray();


            if (nodesLeft <= 2)
            {
                KDLeaf parent = tree.Find(e => e.Path.SequenceEqual(parentPath));
                int index = (parent.Index + 1 < vector.point.Length) ? parent.Index + 1 : 0;
                List<KDNode> parentNodes = parent.Nodes.ToList();
                List<KDNode> nodes = parentNodes.FindAll(e => e.point[index] < parent.MedianValue);
                foreach (KDNode node in nodes)
                {
                    double distance = CalculateDistance(node.point, vector.point, vector.point.Length);
                    if (distance < vector.smallestDistance)
                    {
                        vector.smallestDistance = distance;
                        vector.category = node.label;
                    }

                }
                return true;
            }

            double distanceToLeaf;
            double distanceToLeft;
            KDLeaf currentLeaf;

            if (tree.Exists(e => e.Path.SequenceEqual(path.ToArray())))
            {
                currentLeaf = tree.Find(e => e.Path.SequenceEqual(path.ToArray()));
            }
            else
            {
                KDLeaf parent = tree.Find(e => e.Path.SequenceEqual(parentPath));
                List<KDNode> parentNodes = parent.Nodes.ToList();


                List<KDNode> remainingNodes;
                if (vector.point[parent.Index] < parent.MedianValue)
                {

                    remainingNodes = nodes.FindAll(e => e.point[parent.Index] < parent.MedianValue);
                }else
                {
                    remainingNodes = nodes.FindAll(e => e.point[parent.Index] >= parent.MedianValue);
                }

                currentLeaf = BuildLeafInTree(remainingNodes, (parent.Index +1 < vector.point.Length) ? parent.Index + 1 : 0, path);
            }

            if (currentLeaf.Left == default(KDLeaf))
            {
                int indexLeft = currentLeaf.Index;
                if (indexLeft + 1 < vector.point.Length)
                {
                    indexLeft += 1;
                }
                else
                {
                    indexLeft = 0;
                }
                List<KDNode> remainingNodesForLeftSide = nodes.FindAll(e => e.point[currentLeaf.Index] < currentLeaf.MedianValue);
                List<int> leftPath = path.ToList();
                leftPath.Add(0);

                    BuildLeafInTree(remainingNodesForLeftSide, indexLeft, leftPath);

            }

            distanceToLeaf = CalculateDistance(currentLeaf.Median.point, vector.point, vector.point.Length);

            distanceToLeft = CalculateDistance(currentLeaf.Left.Median.point, vector.point, vector.point.Length);

            if (distanceToLeaf < vector.smallestDistance)
            {
                vector.smallestDistance = distanceToLeaf;
                vector.category = currentLeaf.Median.label;
            }
            int remainingTotalNodes = new int();
            if (distanceToLeaf > distanceToLeft)
            {
                path.Add(0);
                remainingTotalNodes = currentLeaf.Nodes.Count(e => e.point[currentLeaf.Index] < currentLeaf.MedianValue);
            }
            else
            {
                path.Add(1);
                remainingTotalNodes = currentLeaf.Nodes.Count(e => e.point[currentLeaf.Index] >= currentLeaf.MedianValue);
            }
            if (remainingTotalNodes <= 1)
            {
                return true;
            }
            return NNTreeSearch(vector, path, remainingTotalNodes);
        }


        private double CalculateDistance(int[] point1, int[] point2, int length)
        {
            int sum = 0;
            for (int i = 0; i < length; i++)
            {
                sum += (point1[i] - point2[i])*(point1[i] - point2[i]);
            }
            return Math.Sqrt(sum);
        }

        public string Search(FeatureVector[] vectors)
        {
            return Search(3, vectors);
        }

        public string Search(int k, FeatureVector[] vectors)
        {

            foreach (FeatureVector vector in vectors)
            {
                /*
                List<KDNode> kNN = BestBinFirst(k, vector.point);
                DetermineCloset(kNN.ToArray(), vector);
                */
                NNTreeSearch(vector);


            }
            string result = SelectResult(vectors);

            return result;
        }

        private void DetermineCloset(KDNode[] kNN, FeatureVector vector)
        {
            foreach (KDNode node in kNN)
            {

                double distance = CalculateDistance(node.point, vector.point, node.point.Length);
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