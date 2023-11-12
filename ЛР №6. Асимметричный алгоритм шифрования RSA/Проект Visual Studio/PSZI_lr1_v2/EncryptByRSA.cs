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

            for (int i = 0; i < r - 1; i++)
            {
                powBigInt *= m;
            }

            for (int i = 0; i > r + 1; i--)
            {
                powBigInt /= m;
            }

            return powBigInt % n;
        }

        public static BitArray Encrypte(BitArray message, (BigInteger, BigInteger) openKey)
        {
            // Разделение на блоки
            int maxMLength = (int)BigInteger.Log(openKey.Item2, 2);

            List<BigInteger> ms = new List<BigInteger>();

            for(int iMessage = 0; iMessage < message.Length;)
            {
                BitArray binM = new BitArray(maxMLength);
                for (int iM = 0; iM < maxMLength && iMessage < message.Length; iM++, iMessage++)
                {
                    binM[iM] = message[iMessage];
                }

                ms.Add(EncoderClass.BitArrayToBigInteger(binM));
            }

            // Вычисление c
            int maxCLength = (int)BigInteger.Log(openKey.Item2, 2);
            maxCLength += BigInteger.Pow(2, maxCLength) == openKey.Item2 ? 0 : 1;
            BitArray cs = new BitArray(0);


            for (int i = 0; i < ms.Count; i++)
            {
                BigInteger c = generateY(ms[i], openKey.Item1, openKey.Item2);
                BitArray binC = new BitArray(maxCLength);
                BitArray binCFromBigInt = EncoderClass.BigIntegerToBitArray(c);

                for(int j =0; j < maxCLength && j < binCFromBigInt.Length; j++)
                {
                    binC[j] = binCFromBigInt[j];
                }

                cs = cs.Append(binC);
            }

            return cs;
        }

        public static BitArray Decrypte(BitArray cipherText, (BigInteger, BigInteger) closeKey)
        {
            // Разделение на блоки
            int maxCLength = (int)BigInteger.Log(closeKey.Item2, 2);
            maxCLength += BigInteger.Pow(2, maxCLength) == closeKey.Item2 ? 0 : 1;

            List<BigInteger> cs = new List<BigInteger>();

            for (int iCipherText = 0; iCipherText < cipherText.Length;)
            {
                BitArray binC = new BitArray(maxCLength);
                for (int iC = 0; iC < maxCLength; iC++, iCipherText++)
                {
                    binC[iC] = cipherText[iCipherText];
                }

                cs.Add(EncoderClass.BitArrayToBigInteger(binC));
            }

            // Вычисление c
            int maxMLength = (int)BigInteger.Log(closeKey.Item2, 2);
            BitArray ms = new BitArray(0);

            for (int i = 0; i < cs.Count; i++)
            {
                BigInteger m = generateY(cs[i], closeKey.Item1, closeKey.Item2);
                BitArray binM = new BitArray(maxMLength);
                BitArray binMFromBigInt = EncoderClass.BigIntegerToBitArray(m);

                for (int j = 0; j < maxMLength && j < binMFromBigInt.Length; j++)
                {
                    binM[j] = binMFromBigInt[j];
                }
                
                ms = ms.Append(binM);
            }

            return ms;
        }
    }
}
