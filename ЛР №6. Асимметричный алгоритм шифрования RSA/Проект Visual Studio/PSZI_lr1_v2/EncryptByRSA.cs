using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace PSZI_lr1_v2
{
    public class EncryptByRSA
    {
        public static BigInteger generateY(BigInteger m, BigInteger pow, BigInteger n)
        {
            BigInteger powBigInt = m;

            BigInteger n1 = n - 1;
            BigInteger r = pow % n1;

            for (BigInteger i = 0; i < r - 1; i++)
            {
                powBigInt *= m;
            }

            return powBigInt % n;
        }

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


        public static BigInteger mul_mod(BigInteger a, BigInteger b, BigInteger m)
        {
            BigInteger x;
            BigInteger c;
            BigInteger r;
            if (a >= m) a %= m;
            if (b >= m) b %= m;
            x = a;
            c = x * b / m;
            r = (a * b - c * m) % m;
            return r < 0 ? r + m : r;
        }


        public static BigInteger pow_mod(BigInteger a, BigInteger b, BigInteger m)
        {
            BigInteger r = m == 1 ? 0 : 1;
            while (b > 0)
            {
                if ((b & 1) == 1) r = mul_mod(r, a, m);
                b = b >> 1;
                a = mul_mod(a, a, m);
            }
            return r;
        }

        public static List<BigInteger> setBigIntegersFromString(BitArray message, int length)
        {
            List<BigInteger> ms = new List<BigInteger>();

            for (int iMessage = 0; iMessage < message.Length;)
            {
                BitArray binM = new BitArray(length);
                for (int iM = 0; iM < length && iMessage < message.Length; iM++, iMessage++)
                {
                    binM[iM] = message[iMessage];
                }

                ms.Add(EncoderClass.BitArrayToBigInteger(binM));
            }

            return ms;
        }

        public static BitArray setStringFromBigInteger(List<BigInteger> ms, int length, (BigInteger, BigInteger) key)
        {
            BitArray cs = new BitArray(0);

            for (int i = 0; i < ms.Count; i++)
            {
                BigInteger c = powMod(ms[i], key.Item1, key.Item2);
                BitArray binC = new BitArray(length);
                BitArray binCFromBigInt = EncoderClass.BigIntegerToBitArray(c);

                for (int j = 0; j < length && j < binCFromBigInt.Length; j++)
                {
                    binC[j] = binCFromBigInt[j];
                }

                cs = cs.Append(binC);
            }
            return cs;
        }


        public static BitArray Encrypte(BitArray message, (BigInteger, BigInteger) openKey)
        {
            // Разделение на блоки
            int maxMLength = (int)BigInteger.Log(openKey.Item2, 2);

            List<BigInteger> ms1 = setBigIntegersFromString(message, maxMLength);

            // Вычисление c
            int maxCLength = (int)BigInteger.Log(openKey.Item2, 2);
            maxCLength += BigInteger.Pow(2, maxCLength) == openKey.Item2 ? 0 : 1;

            BitArray cs1 = setStringFromBigInteger(ms1, maxCLength, openKey);

            return cs1;
        }

        public static BitArray Decrypte(BitArray cipherText, (BigInteger, BigInteger) closeKey)
        {
            // Разделение на блоки
            int maxCLength = (int)BigInteger.Log(closeKey.Item2, 2);
            maxCLength += BigInteger.Pow(2, maxCLength) == closeKey.Item2 ? 0 : 1;

            List<BigInteger> cs = setBigIntegersFromString(cipherText, maxCLength);

            // Вычисление c
            int maxMLength = (int)BigInteger.Log(closeKey.Item2, 2);

            BitArray ms = setStringFromBigInteger(cs, maxMLength, closeKey);

            return ms;
        }
    }
}
