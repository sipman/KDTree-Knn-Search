using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Globalization;
using System.Security.Policy;

namespace KD_Tree
{
    /// <summary>
    /// This class contains everything needed to do KD Tree logic.
    /// It has two search options either euclidean distance or median determined.
    /// </summary>
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

        /// <summary>
        /// This is a wrapper that allow a bit of abstraction.
        /// This absraction allows a whole categorie to be added to the tree.
        /// </summary>
        /// <param name="category">A categorie that needs to be added to the tree</param>
        private void AddNode(Category category)
        {
            foreach (Feature vector in category.features.ToArray())
            {
                AddNode(category.label, vector.descr);
            }
        }

        /// <summary>
        /// This function adds an node to the tree.
        /// </summary>
        /// <param name="label">label of categorie</param>
        /// <param name="descr">the descr in the R^n</param>
        private void AddNode(string label, double[] descr)
        {
            nodes.Add(new KDNode(label, descr));

        }

        /// <summary>
        /// A wrapper for MedianDeterminedSearch allowing ease of use.
        /// </summary>
        /// <param name="descr">The search descr</param>
        /// <returns>A Nearest Neighborhood for further evaluation</returns>
        private List<KDNode> MedianDeterminedSearch(double[] descr)
        {

            return MedianDeterminedSearch(3, descr);
        }

        /// <summary>
        /// A wrapper for MedianDeterminedSearch allowing ease of use.
        /// </summary>
        /// <param name="k">The desired size of the Nearest Neighborhood</param>
        /// <param name="descr">The search descr</param>
        /// <returns>A Nearest Neighborhood for further evaluation</returns>
        private List<KDNode> MedianDeterminedSearch(int k, double[] descr)
        {
            List<int> path = new List<int>();
            path.Add(-1);
            return MedianDeterminedSearch(k, nodes, descr, 0, path);
        }

        /// <summary>
        /// This is the search function that evaluates on the median and the current search descr index.
        /// It works by comparing the leaf median to the search descr index, if the search descr index
        /// is lower than median it "goes" left else it "goes" right in the tree.
        /// Look up KD-Tree for more information.
        ///
        /// It calls it self recursively, taking one leaf at a time.
        /// </summary>
        /// <param name="k">The desired size of the Nearest Neighborhood</param>
        /// <param name="nodes">The list of nodes that needs to be evaluated</param>
        /// <param name="descr">The search descr</param>
        /// <param name="index">The current index that needs to be evaluated</param>
        /// <param name="path">The path to the current leaf in question</param>
        /// <returns>A Nearest Neighborhood for further evaluation</returns>
        private List<KDNode> MedianDeterminedSearch(int k, List<KDNode> nodes, double[] descr, int index, List<int> path)
        {
            //Console.WriteLine("Hit #1");
            int numOfNodesInLeaf = nodes.Count;
            if (numOfNodesInLeaf <= k)
            {
                BbfPath = path.ToArray();
                return nodes;
            }
            //Console.WriteLine("Hit #2");
            KDLeaf newLeaf = (tree.Exists(e => e.Path.Length == path.Count && e.Path.SequenceEqual(path.ToArray())))
                ? tree.Find(e => e.Path.Length == path.Count && e.Path.SequenceEqual(path.ToArray()))
                : BuildLeafInTree(nodes, index, path);
            //Console.WriteLine("Hit #3");

            List<KDNode> newNodes = new List<KDNode>();

            if (descr[index] < newLeaf.MedianValue && newLeaf.Left != null)
            {
                //Console.WriteLine("Hit #4.1");
                newNodes = newLeaf.Left.Nodes;
            }else if (descr[index] >= newLeaf.MedianValue && newLeaf.Right != null)
            {
                //Console.WriteLine("Hit #4.2");
                newNodes = newLeaf.Right.Nodes;
            }else if (descr[index] < newLeaf.MedianValue && newLeaf.Left == null)
            {
                //Console.WriteLine("Hit #4.3");
                newNodes = nodes.FindAll(e => e.descr[index] < newLeaf.MedianValue);
            }else if (descr[index] >= newLeaf.MedianValue && newLeaf.Right == null)
            {
                //Console.WriteLine("Hit #4.4");
                newNodes = nodes.FindAll(e => e.descr[index] >= newLeaf.MedianValue);
            }
            //Console.WriteLine("Hit #5");
            int nextStep = (descr[index] < newLeaf.MedianValue) ? 0 : 1;
            path.Add(nextStep);
            //Console.WriteLine("Hit #6");
            return MedianDeterminedSearch(k, newNodes, descr, ((index + 1 < descr.Length) ? index+1 : index = 0), path);
        }

        /// <summary>
        /// This function builds a Leaf in the tree and saves it, so no Leaf will be computed twice.
        /// </summary>
        /// <param name="remainingNodes">The amount of nodes remaining for this leaf.</param>
        /// <param name="index">The index this leaf needs to evaluate, if MedianDeterminedSearch is choosen</param>
        /// <param name="path">The path to this exact leaf</param>
        /// <returns>The new leaf</returns>
        private KDLeaf BuildLeafInTree(List<KDNode> remainingNodes, int index, List<int> path)
        {
            if (remainingNodes.Count == 0)
            {
                return new KDLeaf();
            }
            int numOfNodesInLeaf = remainingNodes.Count;

            remainingNodes.Sort((first, second)=>first.descr[index].CompareTo(second.descr[index]));

            int median = numOfNodesInLeaf / 2;

            //median--;

            double medianValue = remainingNodes[median].descr[index];

            KDLeaf newLeaf = new KDLeaf();
            newLeaf.MedianValue = medianValue;
            newLeaf.Median = remainingNodes[median];
            newLeaf.Index = index;
            newLeaf.Nodes = remainingNodes;
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


        /// <summary>
        /// This is just a wrapper for ease of use.
        /// </summary>
        /// <param name="vector">The vector in question</param>
        /// <returns>True, always...</returns>
        private bool EuclideanDistanceSearch(Feature vector)
        {
            List<int> path = new List<int>();
            path.Add(-1);
            return EuclideanDistanceSearch(vector, path, nodes.Count);
        }

        /// <summary>
        /// This is the euclidean distance search, it work kinda like MedianDeterminedSearch but instead of
        /// evaluating a specific search descr index to the median. It evaluates the distance between the leaf's
        /// median descr and the search descr.
        ///
        /// It calls it self recursively, taking one leaf at a time.
        /// </summary>
        /// <param name="vector">The vector in question</param>
        /// <param name="path">The path to this exact leaf</param>
        /// <param name="nodesLeft">The amount of nodes left for this exact leaf</param>
        /// <returns>True, always...</returns>
        private bool EuclideanDistanceSearch(Feature vector, List<int> path, int nodesLeft)
        {
            if (tree.Count <= 0)
            {
                BuildLeafInTree(nodes, 0, path);
            }

            int[] parentPath = path.Take(path.Count - 1).ToArray();


            if (nodesLeft <= 2)
            {
                KDLeaf parent = tree.Find(e => e.Path.SequenceEqual(parentPath));
                int index = (parent.Index + 1 < vector.descr.Length) ? parent.Index + 1 : 0;
                List<KDNode> parentNodes = parent.Nodes;
                List<KDNode> nodes = parentNodes.FindAll(e => e.descr[index] < parent.MedianValue);
                foreach (KDNode node in nodes)
                {
                    double distance = CalculateDistance(node.descr, vector.descr, vector.descr.Length);
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
                if (vector.descr[parent.Index] < parent.MedianValue)
                {

                    remainingNodes = nodes.FindAll(e => e.descr[parent.Index] < parent.MedianValue);
                }
                else
                {
                    remainingNodes = nodes.FindAll(e => e.descr[parent.Index] >= parent.MedianValue);
                }

                currentLeaf = BuildLeafInTree(remainingNodes,
                    (parent.Index + 1 < vector.descr.Length) ? parent.Index + 1 : 0, path);
            }

            if (currentLeaf.Left == default(KDLeaf))
            {
                int indexLeft = currentLeaf.Index;
                if (indexLeft + 1 < vector.descr.Length)
                {
                    indexLeft += 1;
                }
                else
                {
                    indexLeft = 0;
                }
                List<KDNode> remainingNodesForLeftSide =
                    nodes.FindAll(e => e.descr[currentLeaf.Index] < currentLeaf.MedianValue);
                List<int> leftPath = path.ToList();
                leftPath.Add(0);

                BuildLeafInTree(remainingNodesForLeftSide, indexLeft, leftPath);

            }

            distanceToLeaf = CalculateDistance(currentLeaf.Median.descr, vector.descr, vector.descr.Length);
            if (currentLeaf.Left != default(KDLeaf))
            {
                distanceToLeft = CalculateDistance(currentLeaf.Left.Median.descr, vector.descr, vector.descr.Length);
            }
            else
            {
                distanceToLeft = double.MaxValue;
            }
            if (distanceToLeaf < vector.smallestDistance)
            {
                vector.smallestDistance = distanceToLeaf;
                vector.category = currentLeaf.Median.label;
            }
            int remainingTotalNodes = new int();
            if (distanceToLeaf > distanceToLeft)
            {
                path.Add(0);
                remainingTotalNodes = currentLeaf.Nodes.Count(e => e.descr[currentLeaf.Index] < currentLeaf.MedianValue);
            }
            else
            {
                path.Add(1);
                remainingTotalNodes = currentLeaf.Nodes.Count(e => e.descr[currentLeaf.Index] >= currentLeaf.MedianValue);
            }
            if (remainingTotalNodes <= 1)
            {
                return true;
            }
            return EuclideanDistanceSearch(vector, path, remainingTotalNodes);
        }

        /// <summary>
        /// This function calculates the euclidean distance between to descrs in R^n
        /// </summary>
        /// <param name="descr1">First descr</param>
        /// <param name="descr2">Second descr</param>
        /// <param name="length">Length of index's desired for calculation</param>
        /// <returns>Distance between the descrs</returns>
        private double CalculateDistance(double[] descr1, double[] descr2, int length)
        {
            double sum = 0;
            for (int i = 0; i < length; i++)
            {
                sum += (descr1[i] - descr2[i])*(descr1[i] - descr2[i]);
            }
            return Math.Sqrt(sum);
        }

        /// <summary>
        /// This function determines which descr is closest in a nearest neighborhood
        /// </summary>
        /// <param name="kNN">The neighborhood</param>
        /// <param name="vector">The vector in question</param>
        private void DetermineCloset(KDNode[] kNN, Feature vector)
        {
            foreach (KDNode node in kNN)
            {

                double distance = CalculateDistance(node.descr, vector.descr, node.descr.Length);
                if (vector.smallestDistance > distance)
                {
                    vector.smallestDistance = distance;
                    vector.category = node.label;
                }
            }
        }

        /// <summary>
        /// This function evaluates all the vectors in question and finds the most dominating result label
        /// </summary>
        /// <param name="vectors">The vectors in question</param>
        /// <returns>The most dominating result label</returns>
        private string SelectResult(Feature[] vectors)
        {
            return (from vector in vectors
                group vector by vector.category
                into grp
                orderby grp.Count() descending
                select grp.Key).First();
        }

        /// <summary>
        /// This is just a wrapper for ease of use
        /// </summary>
        /// <param name="vectors">The vectors in question</param>
        /// <returns>Result label</returns>
        public string MedianSearch(Feature[] vectors)
        {
            return MedianSearch(3, vectors);
        }

        /// <summary>
        /// This is performs a Search based upon the mediant.
        /// </summary>
        /// <param name="k">The desired nearest neaighborhood</param>
        /// <param name="vectors">The vectors in question</param>
        /// <returns>Result label</returns>
        public string MedianSearch(int k, Feature[] vectors)
        {
            int i = 1;
            int total = vectors.Length;
            foreach (Feature vector in vectors)
            {
               if(i % 50 == 0){
                Console.Clear();
                Console.WriteLine("Searching...");
                Console.WriteLine(i+"/"+total);
               }

                List<KDNode> kNN = MedianDeterminedSearch(k, vector.descr);
                DetermineCloset(kNN.ToArray(), vector);

                i++;
            }

            Console.WriteLine(i);
            string result = SelectResult(vectors);

            return result;
        }

        /// <summary>
        /// This performs the Euclidean distance search.
        /// </summary>
        /// <param name="vectors">The vectors in question</param>
        /// <returns>Result label</returns>
        public string EuclideanSearch(Feature[] vectors)
        {
            int i = 1;
            int total = vectors.Length;
            foreach (Feature vector in vectors)
            {
                Console.Clear();
                Console.WriteLine("Searching...");
                Console.WriteLine(i+"/"+total);


                EuclideanDistanceSearch(vector);
                i++;
            }

            string result = SelectResult(vectors);

            return result;
        }

    }

}