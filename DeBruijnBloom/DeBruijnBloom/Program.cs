using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DeBruijnBloom
{
    internal class Program
    {
        static String TestKmer(int k, String read)
        {
            Random rnd = new Random();
            char[] bases = new char[] { 'A', 'C', 'T', 'G' };
            int n = read.Length - k;
            BloomFilter tester = new BloomFilter(k, n*30);
            HashSet<String> kmers = new HashSet<string>();
            for (int i = 0; i < n; i++)
            {
                kmers.Add(read.Substring(i, k));
                if(i == 0 || i == n-1)
                {
                    tester.addKmer(read.Substring(i, k), true);
                }
                else
                {
                    tester.addKmer(read.Substring(i, k), false);
                }
                
            }
            String ret = k.ToString() + "\t";
            Thread.Sleep(3000);
            ret += testMembSpeed(k, n, read, tester, 1000000) + "\t";
            Thread.Sleep(3000);
            ret += testFWDSpeed(k, n, read, tester, 1000000) + "\t";
            Thread.Sleep(3000);
            ret += testBWDSpeed(k, n, read, tester, 1000000) + "\t";
            Thread.Sleep(3000);
            ret += falsePositveTest(k, tester, kmers, 1000000);
            return ret;
        }
        static double testMembSpeed(int k, int n, string read, BloomFilter filter, int trials)
        {
            double speed = 0;
            int actualTrials = trials;
            Random rnd = new Random();
            for (int i = 0; i<trials; i++)
            {
                String substring = read.Substring(rnd.Next(n-1), k);
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                int ret = filter.memb(substring);
                stopwatch.Stop();
                if (ret == -1)
                {
                    Console.WriteLine("Error");
                    Thread.Sleep(2000);
                }
                if(stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L)) > 10000)
                {
                    actualTrials--;
                }
                else
                {
                    speed += stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
                }
                
                
            }
            return speed/actualTrials;
        }
        static double testFWDSpeed(int k, int n, string read, BloomFilter filter, int trials)
        {
            double speed = 0;
            int actualTrials = trials;
            Random rnd = new Random();
            for (int i = 0; i < trials; i++)
            {
                int r = rnd.Next(n - 1);
                String substring = read.Substring(r, k);
                char nextChar = read[r + k];
                int hash = filter.memb(substring);
                if (hash == -1)
                {
                    Console.WriteLine("Hash Error");
                    Thread.Sleep(2000);
                }
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                int ret = filter.fwd(substring, hash, nextChar);
                stopwatch.Stop();
                if (ret == -1)
                {
                    Console.WriteLine("Error");
                    Thread.Sleep(2000);
                }
                if (stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L)) > 10000)
                {
                    actualTrials--;
                }
                else
                {
                    speed += stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
                }

            }
            return speed / actualTrials;
        }
        static double testBWDSpeed(int k, int n, string read, BloomFilter filter, int trials)
        {
            double speed = 0;
            int actualTrials = trials;
            Random rnd = new Random();
            for (int i = 0; i < trials; i++)
            {
                int r = rnd.Next(1, n - 1);
                String substring = read.Substring(r, k);
                char nextChar = read[r-1];
                int hash = filter.memb(substring);
                if (hash == -1)
                {
                    Console.WriteLine("Hash Error");
                    Thread.Sleep(2000);
                }
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                int ret = filter.bwd(substring, hash, nextChar);
                stopwatch.Stop();
                if (ret == -1)
                {
                    Console.WriteLine("Error");
                    Thread.Sleep(2000);
                }
                if (stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L)) > 10000)
                {
                    actualTrials--;
                }
                else
                {
                    speed += stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
                }

            }
            return speed / actualTrials;
        }

        static double falsePositveTest(int k, BloomFilter filter, HashSet<String> kmers, int trials)
        {
            double positives = 0;
            char[] bases = new char[] { 'A', 'C', 'T', 'G' };
            Random rnd = new Random();
            for (int i = 0; i < trials; i++)
            {
                string kmer = "";
                for (int j = 0; j < k; j++)
                {
                    kmer = kmer + bases[rnd.Next(bases.Length)];
                }
                while (kmers.Contains(kmer))
                {
                    kmer = "";
                    for (int j = 0; j < k; j++)
                    {
                        kmer = kmer + bases[rnd.Next(bases.Length)];
                    }
                }
                int ret = filter.memb(kmer);
                if (ret != -1)
                {
                    positives++;
                }

            }
            return positives / trials;
        }
        static void Main(string[] args)
        {
            String[] fileArray = new string[] { "hiv-1" };
            for (int i = 0; i<fileArray.Length; i++)
            {
                String fileString = "";
                for (int k = 50; k < 161; k++)
                {
                    String input = File.ReadAllText(fileArray[i] + "_input.txt");
                    input = Regex.Replace(input, @"\s+", "");
                    String tempOutput = TestKmer(k, input);
                    fileString = fileString + tempOutput + "\n";

                    
                }
                File.WriteAllText(fileArray[i]+"_output.txt", fileString);
            }

        }
    }
}
