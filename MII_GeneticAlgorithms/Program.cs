using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MII_GeneticAlgorithms
{
    class Program
    {
        static void Main(string[] args)
        {
            int k = 10;
            GeneticAlgorithm geneticAlgorithm  = new GeneticAlgorithm(4,k,10000,
                new int[] { k * 50, k * 50, k * 50, k * 50}
                //new int[] { 0, 50, 100, 150 }
                );
            geneticAlgorithm.GenerateProducts(50);
            //geneticAlgorithm.GenerateProductsWithNorm(10);
            geneticAlgorithm.Population = geneticAlgorithm.GeneratePopulation(100);


            Console.WriteLine(
                "best: " + geneticAlgorithm.GetElementDeviation(geneticAlgorithm.SelectBest(1)[0]));
            int prev = geneticAlgorithm.GetElementDeviation(geneticAlgorithm.SelectBest(1)[0]);
            for (int i = 0; i < 1_000; i++)
            {
                geneticAlgorithm.Evolve();
                //if(i%100==0)
                if (geneticAlgorithm.GetElementDeviation(geneticAlgorithm.SelectBest(1)[0]) != prev)
                {
                    Console.WriteLine(
                        "best: " + geneticAlgorithm.GetElementDeviation(geneticAlgorithm.SelectBest(1)[0]));
                    prev=geneticAlgorithm.GetElementDeviation(geneticAlgorithm.SelectBest(1)[0]);
                }
            }

            

            var t = geneticAlgorithm.GetElementFeatures(geneticAlgorithm.SelectBest(1)[0]);

            Console.WriteLine(
                "best: " + geneticAlgorithm.GetElementDeviation(geneticAlgorithm.SelectBest(1)[0]));
            Console.WriteLine(string.Join(" ", t));
            Console.WriteLine(geneticAlgorithm.SelectBest(1)[0]);

            Console.WriteLine();
            Console.WriteLine("Bust:");

            geneticAlgorithm.Bust();

            Console.WriteLine();
            Console.WriteLine("Bust2:");
            geneticAlgorithm.Bust2();

            Console.ReadKey();
        }

        public static Random random = new Random();
    }
}
