using System;
using System.Drawing;
namespace KD_Tree
{
    public class KDNode
    {
        public double[] descr;
        public string label;

        public KDNode(string label, double[] descr)
        {

            this.descr = descr;
            this.label = label;
        }
    }
}