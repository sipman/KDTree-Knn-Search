namespace KD_Tree
{
    public class FeatureVector
    {
        public int[] point;
        public string category;
        public double smallestDistance = double.MaxValue;

        public FeatureVector(int[] point)
        {
            this.point = point;
        }
    }
}