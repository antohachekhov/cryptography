using System;
using System.Collections;
using System.Linq;

namespace PSZI_lr1_v2
{
    public class GeneratorDESKey
    {
        public static int keyLength = 56;
        const int byteLength = 8;


        // Количество чисел - 56
        static int[,] G = {
            { 57, 49, 41, 33, 25, 17, 09 },
            { 01, 58, 50, 42, 34, 26, 18 },
            { 10, 02, 59, 51, 43, 35, 27 },
            { 19, 11, 03, 60, 52, 44, 36 },
            { 63, 55, 47, 39, 31, 23, 15 },
            { 07, 62, 54, 46, 38, 30, 22 },
            { 14, 06, 61, 53, 45, 37, 29 },
            { 21, 13, 05, 28, 20, 12, 04 }
        };

        public BitArray C_0, D_0;

        int[] countShiftMatrix = { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        int[,] H = {
            {14,    17,  11,  24,  1 ,  5 , 3 ,  28,  15,  6 ,  21,  10,  23,  19,  12,  4 },
            {26,    8 ,  16,  7 ,  27,  20, 13,  2 ,  41,  52,  31,  37,  47,  55,  30,  40 },
            {51,    45,  33,  48,  44,  49, 39,  56,  34,  53,  46,  42,  50,  36,  29,  32 }
        };


        public void cicleLeftShift(ref BitArray bitArray, int countShift)
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
            //
            //Console.WriteLine("Генерируем подключ...");
            //

            int countSumShift = 0;
            for (int j = 0; j < i; j++)
            {
                countSumShift += countShiftMatrix[j];
            }
            
            BitArray Ci = new BitArray(C_0);
            BitArray Di = new BitArray(D_0);

            cicleLeftShift(ref Ci, countSumShift);
            cicleLeftShift(ref Di, countSumShift);

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

        public static void DivideKeyToC0AndD0(GeneratorDESKey generatorKey, BitArray key)
        {
            int lengthCAndD = G.Length / 2;
            generatorKey.C_0 = new BitArray(lengthCAndD);
            generatorKey.D_0 = new BitArray(lengthCAndD);

            int countColumnsG = G.GetLength(1);
            int countRowsG = G.GetLength(0);
            for (int i = 0; i < lengthCAndD; i++)
            {
                int iG = i / countColumnsG;
                int jG = i % countColumnsG;

                generatorKey.C_0[i] = key[G[iG, jG] - 1];


                int iG2 = iG + countRowsG / 2;
                int jG2 = jG;
                generatorKey.D_0[i] = key[G[iG2, jG2] - 1];
            }
        }

        public GeneratorDESKey(BitArray generalKey)
        {

            if (generalKey.Length != keyLength)
            {
                //MessageBox.Show("Начальный ключ должен состоять из 7 байт (56 бит)");
                
            }

            //
            //Console.WriteLine("Расширение ключа...");
            //
            BitArray extendedKey = new BitArray(generalKey);
            if(extendedKey.Length != 64)
                extendedKey = ExtendedKey(generalKey);

            //Console.WriteLine("Вычисление C_0 и D_0");
            // Генерация C_0 и D_0 
            DivideKeyToC0AndD0(this, extendedKey);
        }


    }
}
