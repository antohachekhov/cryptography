using System.Numerics;

namespace PSZI_lr1_v2
{
    public struct keys
    {
        /// <summary>
        /// (e,n)
        /// </summary>
        public (BigInteger, BigInteger) openKey;

        /// <summary>
        /// (d,n)
        /// </summary>
        public (BigInteger, BigInteger) closeKey;

        public keys(BigInteger e, BigInteger d, BigInteger n) : this()
        {
            openKey = (e, n);
            closeKey = (d, n);
        }
    }

    public struct d_i_j
    {
        public BigInteger d;
        public BigInteger i;
        public BigInteger j;

        public d_i_j(BigInteger d, BigInteger i, BigInteger j) : this()
        {
            this.d = d;
            this.i = i;
            this.j = j;
        }
    }

    public static class GeneratorKeysRSA
    {
        public static keys generateKeys(BigInteger p,BigInteger q)
        {
            BigInteger n = p * q;

            BigInteger phi = (p - 1) * (q - 1);

            BigInteger e = 2;
            for (; e < phi; e++)
            {

                if(calcDByEuclideanAlg(phi, e).d == 1)
                    break;
            }

            BigInteger d = calcDByEuclideanAlg(phi, e).j;

            if (d < 0) d += phi;

            return new keys(e, d, n);
        }


        private static d_i_j calcDByEuclideanAlg(BigInteger m, BigInteger n)
        {
            if (n == 0)
                return new d_i_j(m, 1, 0);
            else
            {
                d_i_j result = calcDByEuclideanAlg(n, m % n);
                return new d_i_j(result.d, result.j, result.i - result.j * (m / n));
            }
        }

    }
}
