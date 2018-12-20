using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MII_GeneticAlgorithms
{
    class GeneticAlgorithm
    {
        public int ProductFeaturesCount { get; set; }
        public Product[] Products { get; set; }
        public List<Chromosome> Population { get; set; }
        public int ProductNumberInChromosome { get; set; }

        public int PriceLimit { get; set; }
        public int[] FeaturesLimits { get; set; }

        public GeneticAlgorithm(int productFeaturesCount, int productNumberInChromosome, int priceLimit, int[] featuresLimits)
        {
            ProductFeaturesCount = productFeaturesCount;
            ProductNumberInChromosome = productNumberInChromosome;
            PriceLimit = priceLimit;
            FeaturesLimits = featuresLimits;
        }

        public void GenerateProducts(int elementsCount) { 
            Products = new Product[elementsCount];

            for (int i = 0; i < elementsCount; i++)
            {
                Products[i] = new Product(i,ProductFeaturesCount);
            }

        }

        public void GenerateProductsWithNorm(int elementsCount) { 
            Products = new Product[elementsCount];

            for (int i = 0; i < 5; i++)
            {
                Products[i] = new Product(i,ProductFeaturesCount,6);
            }
            for (int i = 5; i < elementsCount; i++)
            {
                Products[i] = new Product(i,ProductFeaturesCount);
            }

        }

        public List<Chromosome> GeneratePopulation(int populationSize)
        {
            List<Chromosome> population = new List<Chromosome>();
            for (int i = 0; i < populationSize; i++)
            {
                List<int> productIndexes = new List<int>();
                for (int j = 0; j < Products.Length; j++)
                {
                    productIndexes.Add(j);
                }

                int [] gens = new int[Products.Length];
                for (int j = 0; j < ProductNumberInChromosome; j++)
                {
                    int genInd = Program.random.Next(0,productIndexes.Count);
                    gens[productIndexes[genInd]] = 1;
                    productIndexes.RemoveAt(genInd);
                }

                Chromosome c = new Chromosome(gens);
                //if (c.ToString() == "0011110111")
                //{
                //    Console.WriteLine("kek");
                //}
                population.Add(c);
            }

            return population;
        }

        public double GetElementFitness(Chromosome chromosome)
        {
            List<Product> products = GetProductsFromElement(chromosome);
            int productsPrice = products.Sum(p => p.Price);

            if (productsPrice > PriceLimit)
                return 0;

            int[] featuresSums = new int[ProductFeaturesCount];
            for (int i = 0; i < featuresSums.Length; i++)
            {
                featuresSums[i] = products.Sum(p => p.features[i]);
            }

            int deviationSum = 0;
            for (int i = 0; i < featuresSums.Length; i++)
            {
                deviationSum += Math.Abs(featuresSums[i] - FeaturesLimits[i]);
            }

            return deviationSum < 1 ? 1 : 1.0 / deviationSum;
        }

        public int GetElementDeviation(Chromosome chromosome)
        {
            List<Product> products = GetProductsFromElement(chromosome);
            int[] featuresSums = new int[ProductFeaturesCount];
            for (int i = 0; i < featuresSums.Length; i++)
            {
                featuresSums[i] = products.Sum(p => p.features[i]);
            }

            int deviationSum = 0;
            for (int i = 0; i < featuresSums.Length; i++)
            {
                deviationSum += Math.Abs(featuresSums[i] - FeaturesLimits[i]);
            }

            return deviationSum;
        }

        public int[] GetElementFeatures(Chromosome chromosome)
        {
            List<Product> products = GetProductsFromElement(chromosome);
            int[] features = new int[ProductFeaturesCount];
            for (int i = 0; i < features.Length; i++)
            {
                features[i] = products.Sum(p => p.features[i]);
            }

            return features;
        }

        public List<Product> GetProductsFromElement(Chromosome chromosome)
        {
            List<Product> products = new List<Product>();
            for (int i = 0; i < Products.Length; i++)
            {
                if (chromosome.Gens[i]==1)
                {
                    products.Add(Products[i]);
                }
            }

            return products;
        }

        public List<Chromosome> SelectBest(int count)
        {
            List<ChromosomeWithFitness> chromosomeWithFitness = new List<ChromosomeWithFitness>();
            for (int i = 0; i < Population.Count; i++)
            {
                chromosomeWithFitness.Add(new ChromosomeWithFitness{Chromosome = Population[i],Fitness = GetElementDeviation(Population[i])});
            }
            chromosomeWithFitness.Sort((y, x) => y.Fitness.CompareTo(x.Fitness));

            List<Chromosome> chromosomes = new List<Chromosome>();
            for (int i = 0; i < Math.Min(count, chromosomeWithFitness.Count); i++)
            {
                chromosomes.Add(chromosomeWithFitness[i].Chromosome);
            }

            return chromosomes;
        }

        public List<Chromosome> SelectRoulette(int count)
        {
            List<double> props = new List<double>();
            for (int i = 0; i < Population.Count; i++)
            {
                props.Add(GetElementFitness(Population[i]));
            }

            double propsSum = props.Sum();

            List<Chromosome> chosen = new List<Chromosome>();
            for (int i = 0; i < count; i++)
            {
                double pick = Program.random.NextDouble() * (propsSum);
                double current = 0;

                for (int j = 0; j < Population.Count; j++)
                {
                    current += props[j];
                    if (current > pick)
                    {
                        chosen.Add(Population[j]);
                        break;
                    }
                }
            }

            return chosen;


        }

        public Chromosome MutateElement(Chromosome chromosome)
        {
            List<int> ones = new List<int>();
            List<int> zeros = new List<int>();

            for (int i = 0; i < chromosome.Gens.Length; i++)
            {
                if(chromosome.Gens[i]==1)
                    ones.Add(i);
                else
                    zeros.Add(i);
            }

            int one_r_i = Program.random.Next(0, ones.Count );
            zeros.Add(ones[one_r_i]);
            ones.RemoveAt(one_r_i);

            int zero_r_i = Program.random.Next(0, zeros.Count);
            ones.Add(zeros[zero_r_i]);
            zeros.RemoveAt(zero_r_i);

            Chromosome newChromosome = new Chromosome(Products.Length);
            for (int i = 0; i < ones.Count; i++)
            {
                newChromosome.Gens[ones[i]] = 1;
            }

            return newChromosome;
        }

        public List<Chromosome> GetMutatedSet(int count)
        {
            List<Chromosome> mutatedSet = new List<Chromosome>();
            for (int i = 0; i < count; i++)
            {
                mutatedSet.Add(MutateElement(Population[Program.random.Next(0, Population.Count )]));
            }


            return mutatedSet;
        }

        public Chromosome CrossoverElements(Chromosome element1, Chromosome element2)
        {
            List<int> ones = new List<int>();

            for (int i = 0; i < element1.Gens.Length; i++)
            {
                if (element1.Gens[i] == 1)
                    ones.Add(i);
            }

            for (int i = 0; i < element2.Gens.Length; i++)
            {
                if (element2.Gens[i] == 1 && !ones.Contains(i))
                    ones.Add(i);
            }

            Chromosome newChromosome = new Chromosome(Products.Length);

            for (int i = 0; i < ProductNumberInChromosome; i++)
            {
                int rIndex = Program.random.Next(0, ones.Count );
                newChromosome.Gens[ones[rIndex]] = 1;
                ones.RemoveAt(rIndex);
            }

            return newChromosome;

        }

        public List<Chromosome> GetCrossoverSet(int count)
        {
            List<Chromosome> crossoverSet = new List<Chromosome>();
            for (int i = 0; i < count; i++)
            {
                crossoverSet.Add(CrossoverElements(
                        Population[Program.random.Next(0, Population.Count )],
                        Population[Program.random.Next(0, Population.Count )]
                    ));
            }

            return crossoverSet;
        }

        public void KillDuplicates()
        {
            Population = Population
                .GroupBy(p => p.ToString())
                .Select(group => group.First()).ToList();
        }

        public void Evolve()
        {
            List<Chromosome> newPopulation = new List<Chromosome>();

            newPopulation.AddRange(SelectRoulette(20));
            newPopulation.AddRange(SelectBest(20));
            newPopulation.AddRange(GetCrossoverSet(20));
            newPopulation.AddRange(GetMutatedSet(20));
            newPopulation.AddRange(GeneratePopulation(100));

            Population = newPopulation;

            KillDuplicates();
        }

        public void Bust()
        {
            Chromosome best = new Chromosome();
            int dev = 10000;

            Console.WriteLine("count: " + Math.Pow(2, Products.Length));

            string bestS = "";
            for (int i = 0; i < Math.Pow(2,Products.Length); i++)
            {
                string binary = IntToBinaryString(i);
                if (OneNumbers(binary) == ProductNumberInChromosome)
                {
                    Chromosome current = new Chromosome(binary);
                    int dev_current = GetElementDeviation(current);
                    if (dev_current < dev)
                    {
                        dev = dev_current;
                        best = current;
                        bestS = binary;
                    }
                }

                //if (i % 10_000_000 == 0)
                //{
                //    Console.WriteLine(i);
                //}
            }

            Console.WriteLine("best deviation: "+dev);
            Console.WriteLine("best deviation: "+bestS);
            Console.WriteLine(string.Join(" ", GetElementFeatures(best)));


            //Console.WriteLine(IntToBinaryString((int) (Math.Pow(2, Products.Length)-1)));
        }
        public void Bust2()
        {
            //Chromosome best = new Chromosome();
            //int dev = 10000;

            //Console.WriteLine("count: " + Math.Pow(2, Products.Length));

            //for (int i = 0; i < Math.Pow(2,Products.Length); i++)
            //{
            //    string binary = IntToBinaryString(i);
            //    if (OneNumbers(binary) == ProductNumberInChromosome)
            //    {
            //        Chromosome current = new Chromosome(binary);
            //        int dev_current = GetElementDeviation(current);
            //        if (dev_current < dev)
            //        {
            //            dev = dev_current;
            //            best = current;
            //        }
            //    }


            //}

            //Console.WriteLine("best deviation: "+dev);
            //Console.WriteLine(string.Join(" ", GetElementFeatures(best)));

            Chromosome best = new Chromosome();
            int dev = 10000;

            string str = new string('1',ProductNumberInChromosome)+ new string('0', Products.Length-ProductNumberInChromosome);
            var per = new Permutations(str);
            var list = per.GetPermutationsList();
            list = list.Distinct().ToList();

            foreach (var l in list)
            {
                Chromosome current = new Chromosome(l);
                int dev_current = GetElementDeviation(current);
                if (dev_current < dev)
                {
                    dev = dev_current;
                    best = current;
                }
                //Console.WriteLine(l);
            }

            Console.WriteLine("best deviation: " + dev);
            Console.WriteLine(string.Join(" ", GetElementFeatures(best)));
            //Console.WriteLine(IntToBinaryString((int) (Math.Pow(2, Products.Length)-1)));
        }

        public int OneNumbers(string str)
        {
            int count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '1')
                {
                    count++;
                }
            }

            return count;
        }

        public string IntToBinaryString(int k)
        {
            return Convert.ToString(k, 2).PadLeft(Products.Length, '0');
        }

        public int StringToInt(string s)
        {
            var reversedBits = s.Reverse().ToArray();
            var num = 0;
            for (var power = 0; power < reversedBits.Count(); power++)
            {
                var currentBit = reversedBits[power];
                if (currentBit == '1')
                {
                    var currentNum = (int)Math.Pow(2, power);
                    num += currentNum;
                }
            }

            return num;
        }



        public class Permutations
        {
            private List<string> _permutationsList;
            private String _str;

            private void AddToList(char[] a, bool repeat = true)
            {
                var bufer = new StringBuilder("");
                for (int i = 0; i < a.Count(); i++)
                {
                    bufer.Append(a[i]);
                }
                if (repeat || !_permutationsList.Contains(bufer.ToString()))
                {
                    _permutationsList.Add(bufer.ToString());
                }

            }

            private void RecPermutation(char[] a, int n, bool repeat = true)
            {
                for (var i = 0; i < n; i++)
                {
                    var temp = a[n - 1];
                    for (var j = n - 1; j > 0; j--)
                    {
                        a[j] = a[j - 1];
                    }
                    a[0] = temp;
                    if (i < n - 1) AddToList(a, repeat);
                    if (n > 0) RecPermutation(a, n - 1, repeat);
                }
            }

            public Permutations()
            {
                _str = "";
            }

            public Permutations(String str)
            {
                _str = str;
            }
            public String PermutationStr
            {
                get
                {
                    return _str;
                }
                set
                {
                    _str = value;
                }
            }
            public List<string> GetPermutationsList(bool repeat = true)
            {
                _permutationsList = new List<string> { _str };
                RecPermutation(_str.ToArray(), _str.Length, repeat);
                return _permutationsList;
            }
            public List<string> GetPermutationsSortList(bool repeat = true)
            {
                GetPermutationsList(repeat).Sort();
                return _permutationsList;
            }

        }
    }

    struct ChromosomeWithFitness
    {
        public Chromosome Chromosome { get; set; }
        public double Fitness { get; set; }
    }
}
