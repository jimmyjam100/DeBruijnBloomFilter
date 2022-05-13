using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeBruijnBloom
{
    internal class BloomFilter
    {
        char[] letters;
        int arraySize;
        bool[] array;
        RollingHash hasher;
        HashSet<int> edges;

        public BloomFilter(int k, int arraySize)
        {
            this.arraySize = arraySize;
            array = new bool[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                array[i] = false;
            }
            hasher = new RollingHash(k, 50047111, 4);
            edges = new HashSet<int>();
            letters = new char[]{ 'A', 'C', 'T', 'G'};
        }

        public void addKmer(string kmer, bool edge)
        {
            int hash = hasher.hash(kmer);
            array[hash % arraySize] = true;
            if (edge)
            {
                edges.Add(hash);
            }
        }
        private bool simpleMemb(string s)
        {
            return array[hasher.hash(s) % arraySize];
        }
        
        private bool simpleFwd(string s, int hash, char c)
        {
            return array[hasher.fwd(hash, s, c) % arraySize];
        }
        private bool simpleBwd(string s, int hash, char c)
        {
            return array[hasher.bwd(hash, s, c) % arraySize];
        }
        private bool anyFwd(string s, int hash)
        {
            for (int i = 0; i < 4; i++)
            {
                if(simpleFwd(s, hash, letters[i]))
                {
                    return true;
                }
            }
            return false;
        }
        private bool anyBwd(string s, int hash)
        {
            for (int i = 0; i < 4; i++)
            {
                if (simpleBwd(s, hash, letters[i]))
                {
                    return true;
                }
            }
            return false;
        }
        public int memb(string kmer)
        {
            int hash = hasher.hash(kmer);
            if (simpleMemb(hash))
            {
                bool f = anyFwd(kmer, hash);
                bool b = anyBwd(kmer, hash);
                if (f && b)
                {
                    return hash;
                }
                if ((f || b) & edges.Contains(hash))
                {
                    return hash;
                }
            }
            return -1;
        }
        private bool simpleMemb(int hash)
        {
            return array[hash % arraySize];
        }
        public int memb(string kmer, int hash)
        {
            if (simpleMemb(hash))
            {
                bool f = anyFwd(kmer, hash);
                bool b = anyBwd(kmer, hash);
                if (f && b)
                {
                    return hash;
                }
                if ((f || b) & edges.Contains(hash))
                {
                    return hash;
                }
            }
            return -1;
        }
        public int bwd(string kmer, int hash, char c)
        {
            int newHash = hasher.bwd(hash, kmer, c);
            string newkmer = c + kmer.Remove(kmer.Length - 1, 1) ;

            return memb(newkmer, newHash);
        }
        public int fwd(string kmer, int hash, char c)
        {
            int newHash = hasher.fwd(hash, kmer, c);
            string newkmer = kmer.Remove(0, 1) + c;
            return memb(newkmer, newHash);
        }


    }
}
