using PSZI_lr1_v2;
using System;
using System.Collections;

namespace PSZI_lr1
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


    class EncryptorByFeistelNetwork
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
        BitArray E(BitArray seq)
        {
            BitArray result = new BitArray(48);
            result[0] = seq[31];
            for(int i = 1, indexFrom = 0; i < 47; i++)
            {
                if(i % 6 == 0)
                {
                    indexFrom -= 2;
                }
                result[i] = seq[indexFrom];
            }
            result[47] = seq[0];
            return result;
        }


        BitArray S(BitArray B)
        {
            BitArray result = new BitArray(4);
            BitArray strBit = new BitArray(2) { B[0], B[5]};
            int str = (int) EncoderClass.ByteArrayToLong(EncoderClass.BitArrayToByteArray(strBit));
            BitArray colBit = new BitArray(4) { B[1], B[2], B[3], B[4] };
            int col = (int)EncoderClass.ByteArrayToLong(EncoderClass.BitArrayToByteArray(colBit));
            int resultInt = 
        }


        /// <summary> 
        /// Функция шифрования
        /// </summary>
        /// <param name="RightPart">32-битная последовательность R полученная на прошлой итерации</param>
        /// <param name="key"> 48-битный ключ K(i), который является результатом преобразования 64-битного ключа K</param>
        /// <returns></returns>
        BitArray func(BitArray RightPart, BitArray key)
        {
            // E – расширение 32 - битной последовательности до 48 - битной
            BitArray resE = E(RightPart);
            resE.Xor(key);

            BitArray B1 = new BitArray(6);
            BitArray B2 = new BitArray(6);
            BitArray B3 = new BitArray(6);
            BitArray B4 = new BitArray(6);
            BitArray B5 = new BitArray(6);
            BitArray B6 = new BitArray(6);
            BitArray B7 = new BitArray(6);
            BitArray B8 = new BitArray(6);
            int indexInResE = 0;
            for(int i = 0; i < 6; i++, indexInResE++)
            {
                B1[i] = resE[indexInResE];
            }
            for (int i = 0; i < 6; i++, indexInResE++)
            {
                B2[i] = resE[indexInResE];
            }
            for (int i = 0; i < 6; i++, indexInResE++)
            {
                B3[i] = resE[indexInResE];
            }
            for (int i = 0; i < 6; i++, indexInResE++)
            {
                B4[i] = resE[indexInResE];
            }
            for (int i = 0; i < 6; i++, indexInResE++)
            {
                B5[i] = resE[indexInResE];
            }
            for (int i = 0; i < 6; i++, indexInResE++)
            {
                B6[i] = resE[indexInResE];
            }
            for (int i = 0; i < 6; i++, indexInResE++)
            {
                B7[i] = resE[indexInResE];
            }
            for (int i = 0; i < 6; i++, indexInResE++)
            {
                B8[i] = resE[indexInResE];
            }


        }

        public dataToEncryption Encrypte(dataToEncryption data)
        {
            BitArray firstPartText = new BitArray(data.firstPartText);
            data.firstPartText = func(data.firstPartText, data.partKey).Xor(data.secondPartText);

            data.secondPartText = firstPartText;
            return data;
        }

        public EncryptorByFeistelNetwork()
        {
        }

        internal dataToEncryption Decrypte(dataToEncryption data)
        {
            BitArray secondPartText = new BitArray(data.secondPartText);
            data.secondPartText = func(data.secondPartText, data.partKey).Xor(data.firstPartText);

            data.firstPartText = secondPartText;
            return data;
        }
    }
}
