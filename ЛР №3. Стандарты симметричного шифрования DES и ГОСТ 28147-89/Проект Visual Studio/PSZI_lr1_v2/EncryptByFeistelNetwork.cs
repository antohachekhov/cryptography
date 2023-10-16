using System;
using System.Collections;

namespace PSZI_lr1_v2
{
    public struct dataToEncryption
    {
        public BitArray firstPartText;
        public BitArray secondPartText;
        public BitArray partKey;

        public dataToEncryption(BitArray firstPartText, BitArray secondPartText, BitArray partKey)
        {
            this.firstPartText = firstPartText;
            this.secondPartText = secondPartText;
            this.partKey = partKey;
        }
    }


    public class EncryptorByFeistelNetwork
    {
        
        public int[,] S1 = new int[4, 16] { 
            {14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
            {0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
            {4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
            {15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 } };

        public int[,] S2 = new int[4, 16] { 
            {15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
            {3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5},
            {0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
            {13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 } };

        public int[,] S3 = new int[4, 16] { 
            {10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
            {13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1},
            {13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7},
            {1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12} };

        public int[,] S4 = new int[4, 16] { 
            {7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
            {13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9},
            {10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4},
            {3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14} };

        public int[,] S5 = new int[4, 16] { 
            {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
            {14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6},
            {4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14},
            {11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3} };

        public int[,] S6 = new int[4, 16] {
            {12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
            {10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8},
            {9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6},
            {4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13} };

        public int[,] S7 = new int[4, 16] {
           {4 ,11, 2 ,14, 15, 0, 8, 13, 3 ,12, 9, 7 ,5 ,10, 6, 1 },
           {13, 0, 11, 7, 4 ,9 ,1 ,10 ,14 ,3 ,5 ,12 ,2 ,15, 8, 6 },
           {1 ,4 ,11 ,13, 12, 3, 7, 14, 10,15, 6, 8 ,0 ,5 ,9 ,2  },
           { 6 ,11, 13, 8, 1 ,4 ,10, 7 ,9,   5, 0, 15,14, 2, 3, 12} };

        public int[,] S8 = new int[4, 16] {
            {13, 2 , 8 , 4, 6 ,15, 11, 1, 10,  9, 3, 14,5 ,0 ,12, 7},
            {1 , 15, 13, 8, 10, 3, 7 ,4 ,12 , 5 ,6 ,11 ,0 ,14, 9, 2},
            {7 , 11, 4 , 1, 9 ,12, 14, 2, 0 , 6 ,10, 13,15, 3, 5, 8},
            {2 , 1 , 14, 7, 4 ,10, 8 ,13, 15, 12, 9, 0 ,3 ,5 ,6 ,11 } };


        /// <summary> 
        /// Функция расширения из 32-битной последовательности в 48-битную
        /// </summary>
        /// <param name="seq">32-битная последовательность</param>
        /// <returns>48-битная последовательность</returns>
        public BitArray E(BitArray seq)
        {
            BitArray result = new BitArray(48);
            result[0] = seq[seq.Length - 1];
            for(int i = 1, indexFrom = 0; i < result.Length - 1; i++, indexFrom++)
            {
                if(i % 6 == 0)
                {
                    indexFrom -= 2;
                }
                result[i] = seq[indexFrom];
            }
            result[result.Length - 1] = seq[0];
            return result;
        }


        public BitArray S(int[,] tableS, BitArray B)
        {
            BitArray strBit = new BitArray(new bool[] { false, false, false, false, false, false, B[0], B[5] });
            int str = (int) EncoderClass.ByteArrayToLong(EncoderClass.BitArrayToByteArray(strBit));
            BitArray colBit = new BitArray(new bool[] { false, false, false, false, B[1], B[2], B[3], B[4] });
            int col = (int) EncoderClass.ByteArrayToLong(EncoderClass.BitArrayToByteArray(colBit));
            int resultInt = tableS[str, col];
            BitArray result = new BitArray(EncoderClass.IntToBitArrayLength4(resultInt));
            return result;
        }

        public BitArray P(BitArray seq)
        {
            BitArray result = new BitArray(32);
            int[] indexes = new int[32] { 16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26,
                5, 18, 31, 10, 2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22, 11, 4, 25 };
            for(int i = 0; i < result.Length; i++)
            {
                result[i] = seq[indexes[i] - 1];
            }
            return result;
        }


        /// <summary> 
        /// Функция шифрования
        /// </summary>
        /// <param name="rightPart">32-битная последовательность R полученная на прошлой итерации</param>
        /// <param name="key"> 48-битный ключ K(i), который является результатом преобразования 64-битного ключа K</param>
        /// <returns></returns>
        public BitArray func(BitArray rightPart, BitArray key)
        {
            BitArray resE;
            // E – расширение 32 - битной последовательности до 48 - битной
            if (rightPart.Length != key.Length)
                resE = E(rightPart);
            else
                resE = new BitArray(rightPart);
            resE.Xor(key);

            BitArray[] B = new BitArray[8];

            int indexInResE = 0;
            for (int i = 0; i < B.Length; i++)
            {
                B[i] = new BitArray(6);
                for (int j = 0; j < 6; j++, indexInResE++) B[i][j] = resE[indexInResE];
            }


            BitArray resS = new BitArray(S(S1, B[0]));
            resS = BitArrayFunctions.Append(resS, S(S2, B[1]));
            resS = BitArrayFunctions.Append(resS, S(S3, B[2]));
            resS = BitArrayFunctions.Append(resS, S(S4, B[3]));
            resS = BitArrayFunctions.Append(resS, S(S5, B[4]));
            resS = BitArrayFunctions.Append(resS, S(S6, B[5]));
            resS = BitArrayFunctions.Append(resS, S(S7, B[6]));
            resS = BitArrayFunctions.Append(resS, S(S8, B[7]));

            return P(resS);
        }

        public dataToEncryption Decrypte(dataToEncryption data)
        {
            BitArray firstPartText = new BitArray(data.firstPartText);
            data.firstPartText = func(data.firstPartText, data.partKey).Xor(data.secondPartText);

            data.secondPartText = firstPartText;
            return data;
        }

        public EncryptorByFeistelNetwork()
        {
        }

        public dataToEncryption Encrypte(dataToEncryption data)
        {
            BitArray secondPartText = new BitArray(data.secondPartText);
            data.secondPartText = func(data.secondPartText, data.partKey).Xor(data.firstPartText);

            data.firstPartText = secondPartText;
            return data;
        }
    }
}
