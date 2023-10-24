using NUnit.Framework;
using PSZI_lr1_v2;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using TestProject2.PSZI_lr1_v2;

namespace TestProject2
{



    namespace PSZI_lr1_v2
    {
        public class GeneratorKey2
        {
            public const int keyLength = 56;
            const int byteLength = 8;


            // Количество чисел - 56
            int[,] G = {
            { 57, 49, 41, 33, 25, 17, 09 },
            { 01, 58, 50, 42, 34, 26, 18 },
            { 10, 02, 59, 51, 43, 35, 27 },
            { 19, 11, 03, 60, 52, 44, 36 },
            { 63, 55, 47, 39, 31, 23, 15 },
            { 07, 62, 54, 46, 38, 30, 22 },
            { 14, 06, 61, 53, 45, 37, 29 },
            { 21, 13, 05, 28, 20, 12, 04 }
        };

            BitArray C_0, D_0;

            int[] countShiftMatrix = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

            int[,] H = {
            {14,    17,  11,  24,  1 ,  5 , 3 ,  28,  15,  6 ,  21,  10,  23,  19,  12,  4 },
            {26,    8 ,  16,  7 ,  27,  20, 13,  2 ,  41,  52,  31,  37,  47,  55,  30,  40 },
            {51,    45,  33,  48,  44,  49, 39,  56,  34,  53,  46,  42,  50,  36,  29,  32 }
        };


            void cicleLeftShift(BitArray bitArray, int countShift)
            {
                BitArray firstBits = new BitArray(countShift);

                // Сохраняем первые биты
                for (int i = 0; i < firstBits.Length; i++)
                {
                    firstBits[i] = bitArray[i];
                }

                for (int i = 0; i < bitArray.Length - countShift; i++)
                {
                    bitArray[i] = bitArray[i + countShift];
                }

                // Заносим первые биты в последние биты
                for (int i = 0; i < firstBits.Length; i++)
                {
                    bitArray[i + bitArray.Length - countShift] = firstBits[i];
                }
            }


            // Генерация ключа
            public BitArray GenerateKey(int i)
            {
                Console.WriteLine("Генерируем подключ...");

                int countSumShift = 0;
                for (int j = 0; j < i; j++)
                {
                    countSumShift += countShiftMatrix[j];
                }

                BitArray Ci = new BitArray(C_0);
                BitArray Di = new BitArray(D_0);

                cicleLeftShift(Ci, countSumShift);
                cicleLeftShift(Di, countSumShift);

                Ci = BitArrayFunctions.Append(Ci, Di);


                BitArray belowKey = new BitArray(H.Length);
                for (int j = 0; j < H.Length; j++)
                {
                    int iG = j / H.GetLength(1);
                    int jG = j % H.GetLength(1);
                    belowKey[j] = Ci[H[iG, jG] - 1];
                }

                return belowKey;
            }

            public static BitArray ExtendedKey(BitArray key)
            {
                BitArray extendedKey = new BitArray(0);

                bool[] byteKey = new bool[byteLength];

                for (int i = 0, j = 0; i < key.Length; i++, j++)
                {
                    byteKey[j] = key[i];

                    // Когда заполнены 7 бит байта
                    if (j == byteLength - 2)
                    {
                        // Заполнение последнего бита байта
                        int countOnes = byteKey.Count(x => x == true);
                        if (countOnes % 2 == 0)
                        {
                            byteKey[byteLength - 1] = !byteKey[byteLength - 1];
                        }

                        // Добавление байта в расширенный ключ
                        extendedKey = BitArrayFunctions.Append(extendedKey, new BitArray(byteKey));
                        j = -1;
                    }
                }

                return extendedKey;
            }

            public GeneratorKey2(BitArray generalKey)
            {

                if (generalKey.Length != keyLength)
                {
                    throw new Exception("");
                }

                Console.WriteLine("Расширение ключа...");
                BitArray extendedKey = ExtendedKey(generalKey);

                Console.WriteLine("Вычисление C_0 и D_0");
                // Генерация C_0 и D_0 
                int lengthCAndD = extendedKey.Length / 2;
                C_0 = new BitArray(lengthCAndD);
                D_0 = new BitArray(lengthCAndD);
                int countColumnsG = G.GetLength(1);
                for (int i = 0; i < G.Length / 2; i++)
                {
                    int iG = i / countColumnsG;
                    int jG = i % countColumnsG;

                    C_0[i] = extendedKey[G[iG, jG] - 1];


                    int iG2 = iG + G.GetLength(0) / 2;
                    int jG2 = jG;
                    D_0[i] = extendedKey[G[iG2, jG2] - 1];
                }
            }


        }
    }

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            GeneratorKey generatorKey = new(new System.Collections.BitArray(0));
            Assert.Pass();
        }
    }
}