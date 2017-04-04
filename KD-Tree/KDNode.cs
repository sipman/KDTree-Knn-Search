using System;
using System.Drawing;
namespace KD_Tree
{
    public class KDNode
    {
        public int[] point;
        public string label;
        public KDNode(string label, int [] point)
        {

            this.point = point;
            this.label = label;
        }

        public string ToString()
        {
            string rtr = "{ Point: (";
            bool first = true;
            foreach (int i in point)
            {
                if(!first){
                 rtr += ", ";
                }
                rtr += i.ToString();
                first = false;
            }
            rtr += "),";
            rtr += " Object: "+label+" }";
            return rtr;
        }
    }
}