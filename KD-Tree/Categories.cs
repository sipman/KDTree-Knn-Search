using System.Collections.Generic;
using System.Data;

namespace KD_Tree
{
    public class Categories
    {
        public List<Category> categories;

        public Categories()
        {
            this.categories = new List<Category>();
        }

        public void AddCategory(string label, Feature[] features)
        {
            if (categories.Exists(e => e.label == label))
            {
                throw new DuplicateNameException("The category is already created!");
            }
            categories.Add(new Category(label, features));
        }

        public Category GetCategory(string label)
        {
            return categories.Find(e => e.label == label);
        }

        public Category[] ToArray()
        {
            return categories.ToArray();
        }

    }
}