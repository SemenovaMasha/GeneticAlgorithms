using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MII_GeneticAlgorithms
{
    class Product
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int[] features { get; set; }

        public Product(int productNumber, int featureCount)
        {
            Name = "Product №" + productNumber;
            Price = Program.random.Next(100, 3000);

            features = new int[featureCount];
            for (int i = 0; i < featureCount; i++)
            {
                features[i] = Program.random.Next(0, 100);
            }
        }
        public Product(int productNumber, int featureCount, int k)
        {
            Name = "Product №" + productNumber;
            Price = Program.random.Next(100, 1000);

            features = new int[featureCount];
            for (int i = 0; i < featureCount; i++)
            {
                features[i] = 10 * i;
            }
        }

    }
}
