using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PSZI_lr1_v2
{
    public struct keysElgamal
    {
        /// <summary>
        /// (y,g,p)
        /// </summary>
        public (BigInteger y, BigInteger g, BigInteger p) openKey;

        /// <summary>
        /// (x)
        /// </summary>
        public BigInteger closeKey;

        public keysElgamal(BigInteger y, BigInteger g, BigInteger p, BigInteger x) : this()
        {
            openKey = (y, g, p);
            closeKey = x;
        }
    }



    public class GeneratorKeysElgamal
    {

        public static BigInteger powMod(BigInteger a, BigInteger e, BigInteger m)
        {
            BigInteger r = 1;
            while (e > 0)
            {
                if ((e & 1) == 0)
                {
                    e = e >> 1;
                    a = (a * a) % m;
                }
                else
                {
                    e = e - 1;
                    r = (r * a) % m;
                }
            }

            return r;
        }

        public static keysElgamal generateKeys(BigInteger p, BigInteger g, BigInteger x)
        {
            BigInteger y = powMod(g, x, p);


            return new keysElgamal(y, g, p, x);
        }
    }
}
