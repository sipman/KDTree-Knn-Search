using System.Collections;
using System.Collections.Generic;

namespace KD_Tree
{
    public class KDLeaf
    {
        public double MedianValue;
        public int Index;
        public int[] Path;
        public KDLeaf Left;
        public KDLeaf Right;
        public KDLeaf Parent;
        public KDNode Median;
        public List<KDNode> Nodes = null;


    }
}