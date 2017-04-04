## Synopsis

Just a simple dummy project that creates a KD-Tree K-Nearest-Neighbor Search. It works by crawling the KD-Tree to the first bin with k or less number of entities in the returning leaf. Then it votes alá K-Nearest-Neighbor on the categories.

## Code Example
In this example we create 3 categories which consists of 4-featurevectores.
After that it feeds this categoires object to the KDtree object, thereby allowing
KDTree-crawl.

After that a unknow object is defined and the KDTree is searching for this unkown.

At the end we coutput the different votes, from the search.

 ```cs
            Categories categories = new Categories();

            categories.AddCategory("Kat", new FeatureVector[]
            {
                new FeatureVector(new int[]{1,9}),
                new FeatureVector(new int[]{1,8}),
                new FeatureVector(new int[]{2,6}),
                new FeatureVector(new int[]{2,1}),

            });

            categories.AddCategory("Hus", new FeatureVector[]
            {
                new FeatureVector(new int[]{11,67}),
                new FeatureVector(new int[]{12,84}),
                new FeatureVector(new int[]{11,22}),
                new FeatureVector(new int[]{13,19}),

            });


            categories.AddCategory("Flaske", new FeatureVector[]
            {
                new FeatureVector(new int[]{76,45}),
                new FeatureVector(new int[]{88,76}),
                new FeatureVector(new int[]{34,123}),
                new FeatureVector(new int[]{22,43}),

            });

            KDtree KDTree = new KDtree(categories);


            FeatureVector[] unkown =
            {
                new FeatureVector(new int[]{2,3}),
                new FeatureVector(new int[]{4,22}),
                new FeatureVector(new int[]{2,3}),
                new FeatureVector(new int[]{1,90})
            };

            KDTree.Search(unkown);


            Console.WriteLine("Kat: "+categories.GetCategory("Kat").votes);
            Console.WriteLine("Hus: "+categories.GetCategory("Hus").votes);
            Console.WriteLine("Flaske: "+categories.GetCategory("Flaske").votes);
```

## Motivation

Kinda just wanted to get some insights to KDTree.
