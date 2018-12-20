using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MII_GeneticAlgorithms
{
    public class Chromosome
    {
        public int[] Gens { get; set; }

        public Chromosome(int[] Gens)
        {
            this.Gens = Gens;
        }

        public Chromosome(int GensNum)
        {
            Gens = new int[GensNum];
        }

        public Chromosome(string s)
        {
            Gens = s.Select(n => Convert.ToInt32(n+"")).ToArray();
        }

        public override string ToString()
        {
            return string.Join("", Gens);
        }

        public Chromosome()
        {
            
        }
        
    }
}
