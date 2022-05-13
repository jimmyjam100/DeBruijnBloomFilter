using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeBruijnBloom
{
    internal class RollingHash
    {
        private int stringSize;
        private int mod;
        private int p;
        private int[] multArray;
        //private int[] modInverseArray;
        private int inverseP;
        public RollingHash(int stringSize, int mod, int p)
        {
            this.stringSize = stringSize;
            this.mod = mod;
            this.p = p;
            this.multArray = new int[stringSize];
            //this.modInverseArray = new int[mod];
            int curMult = 1;
            for (int i = 0; i < multArray.Length; i++)
            {
                if (curMult > mod)
                {
                    curMult = curMult % mod;
                }
                multArray[i] = curMult;
                curMult = curMult * p;
            }
            //for (int i = 0; i < mod; i++)
            //{
            //modInverseArray[i] = modInverse(i, mod);
            //}
            inverseP = modInverse(p, mod);

        }
        static int modInverse(int a, int m)
        {
            for (int x = 1; x < m; x++)
                if (((a % m) * (x % m)) % m == 1)
                    return x;
            return 1;
        }
        public int hash(string s)
        {
            if (s.Length != stringSize)
            {
                return -1;
            }
            int ret = 0;
            for (int i = 0; i < s.Length; i++)
            {
                ret = (int)(((long)ret + charToNumb(s[i])*(long)multArray[i])%mod);
            }
            return ret;
        }

        public int fwd(int hash, string s, char c)
        {
            return (int)((((long)(hash+mod)-charToNumb(s[0]))*(long)inverseP+charToNumb(c)*(long)multArray[stringSize-1])%mod);
        }
        public int bwd(int hash, string s, char c) 
        {
            return (int)(((long)((long)(mod*4+hash) - charToNumb(s[stringSize - 1]) * multArray[stringSize - 1]) * (long)p + charToNumb(c))%mod);    
        }

        private int charToNumb(char c)
        {
            switch (c)
            {
                case 'A':
                    return 0;
                case 'C':
                    return 1;
                case 'T':
                    return 2;
                case 'G':
                    return 3;
            }
            return -1;
        }

    }
}
