﻿using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace PSZI_lr1_v2
{
    public class EncryptByRSA
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

        public struct edsRSA
        {
            public BigInteger eds;
            public edsRSA(BigInteger s) : this()
            {
                this.eds = s;
            }
        }
        public static edsRSA GenerateEDS(BigInteger message, (BigInteger d, BigInteger n) closeKey)
        {
            BigInteger s = powMod(message, closeKey.d, closeKey.n);

            return new edsRSA(s);
        }

        public static bool CheckEDS(BigInteger hashMessage, edsRSA EDS, (BigInteger e, BigInteger n) openKey)
        {
            BigInteger right = powMod(EDS.eds, openKey.e, openKey.n);

            return hashMessage == right;
        }
    }
}
