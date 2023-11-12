using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PSZI_lr1_v2
{
    public class EncryptByElgamal
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

        public struct edsElgamal {
            public (BigInteger a, BigInteger b) eds;
            public edsElgamal(BigInteger a, BigInteger b) : this()
            {
                this.eds = (a, b);
            }
        }

        public static BigInteger getInverseElem(BigInteger d, BigInteger f)
        {
            BigInteger[] x = new BigInteger[3] { 1, 0, f };
            BigInteger[] y = new BigInteger[3] { 0, 1, d };
            BigInteger result;
            BigInteger q = 0;
            BigInteger[] t;
            while (true)
            {
                if (y[2] == 0)
                {
                    result = 0;
                    break;
                }
                if (y[2] == 1)
                {
                    result = y[1];
                    break;
                }
                q = x[2] / y[2];
                t = new BigInteger[3];
                for (int i = 0; i < 3; i++)
                {
                    t[i] = x[i] - q * y[i];
                    x[i] = y[i];
                    y[i] = t[i];
                }
            }
            if (result < 0) result += f;
            return result;
        }

        public static edsElgamal GenerateEDS(BigInteger message, keysElgamal keys, BigInteger k)
        {
            BigInteger a = powMod(keys.openKey.g, k, keys.openKey.p);

            BigInteger k_1 = getInverseElem(k, keys.openKey.p - 1);

            BigInteger preMod = k_1 * (message - keys.closeKey * a);

            while(preMod < 0)
            {
                preMod += keys.openKey.p - 1;
            }

            BigInteger b = preMod % (keys.openKey.p - 1);

            return new edsElgamal(a, b);
        }

        public static BigInteger pow(BigInteger a, BigInteger pow)
        {
            BigInteger b = 1;
            for(int i = 0; i < pow; i++)
            {
                b *= a;
            }
            return b;
        }

        public static bool CheckEDS(BigInteger message, keysElgamal keys, edsElgamal EDS)
        {
            //BigInteger left = powMod(keys.openKey.y, EDS.eds.a, keys.openKey.p) * powMod(EDS.eds.a, EDS.eds.b, keys.openKey.p);
            BigInteger right = powMod(keys.openKey.g, message, keys.openKey.p) ;

            BigInteger left = pow(keys.openKey.y, EDS.eds.a) * pow(EDS.eds.a, EDS.eds.b) % keys.openKey.p;
            return left == right;
        }


    }
}
