using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;


namespace PSZI_lr1_v2
{
    public static class BitArrayFunctions
    {
        public static BitArray Append(this BitArray current, BitArray after)
        {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        public static BitArray ReverseAll(this BitArray current)
        {
            var bools = new bool[current.Count];
            for (int i = 0; i < current.Count; i++)
            {
                bools[i] = current.Get(current.Count - i - 1);
            }
            return new BitArray(bools);
        }

        public static BitArray ReverseOnlyValuesInBytes(this BitArray current)
        {
            BitArray bitArray = new BitArray(0);
            for (int i = 0; i < current.Count / 8; i++)
            {
                bool[] bools = new bool[8];
                for (int j = 0; j < 8; j++)
                {
                    bools[j] = current.Get((i + 1) * 8 - j - 1);
                }
                BitArray bitArrayInByte = new BitArray(bools);
                bitArray = BitArrayFunctions.Append(bitArray, bitArrayInByte);
            }
            return bitArray;
        }

        public static int CountXor1(BitArray first, BitArray second)
        {
            BitArray xor = new BitArray(first);
            xor.Xor(second);

            int count = 0;
            for (int i = 0; i < xor.Length; i++)
            {
                if (xor[i] == true)
                    count++;
            }

            return count;
        }

        public static void Add10(ref BitArray bitArray)
        {
            bool flagAdded = false;


            for (int i = 1; !flagAdded && i < bitArray.Length; i++)
            {

                bitArray[i] = !bitArray[i];
                if (bitArray[i] == true)
                {
                    flagAdded = true;
                }
            }

            // Если идет переход на следующий разряд
            if (!flagAdded)
            {
                bitArray = bitArray.Append(new BitArray(1, true));
            }
        }
    }

    public class Program
    {

        // Количество бит в числе
        public int n;

        // Количество проверок в тесте Рабина-Миллера
        public int t;

        List<BitArray> simple3_2000Numbers;

        public int g;

        // Генерирует простое число указанной размерности
        public BitArray generateSimpleNumberByN()
        {
            // Задаем smallerNumber и largerNumber чтобы не писать два раза один и тот же алгоритм в функции
            BitArray smallerNumber = new BitArray(n - 1, true);
            BitArray largerNumber = new BitArray(n + 1);

            return generateSimpleNumberFromRange(smallerNumber, largerNumber);
        }

        // Генерирует простое число от нижней границы до верхней, если простого числа нет, то возвращает null
        // Диапазон не включает числа smallerNumber и largerNumber.
        public BitArray generateSimpleNumberFromRange(BitArray smallerNumber, BitArray largerNumber)
        {
            BitArray trueSimpleValue = null;

            BitArray simpleNumber = new BitArray(smallerNumber);




            for (int i = 0; simpleNumber != largerNumber; i++)
            {
                // Генерация простого числа
                // Выбор случайного числа p 
                BitArrayFunctions.Add10(ref simpleNumber);

                // Делаем число нечетным
                simpleNumber[0] = true;

                // Проверка на делимость

                // Подсчет simple3_2000Numbers
                if (simple3_2000Numbers == null)
                {
                    simple3_2000Numbers = new List<BitArray>();
                    for (ulong simpleNumber2ULong = 2; simpleNumber2ULong < 2000; simpleNumber2ULong++)
                    {
                        bool flagDivision2 = false;
                        for (ulong divNumber = 2; divNumber < simpleNumber2ULong; divNumber++)
                        {
                            if (simpleNumber2ULong % divNumber == 0 && simpleNumber2ULong != divNumber)
                            {
                                flagDivision2 = true;
                                break;
                            }
                        }

                        if (!flagDivision2) simple3_2000Numbers.Add(EncoderClass.ByteArrayToBitArray(EncoderClass.UlongToByteArray(simpleNumber2ULong)));
                    }
                }

                bool flagDivision = true;
                ulong simpleNumerULong = EncoderClass.ByteArrayToUlong(EncoderClass.BitArrayToByteArray(simpleNumber));
                Console.WriteLine(simpleNumerULong);
                for (int j = 0; flagDivision && j < simple3_2000Numbers.Count; j++)
                {
                    ulong simple3_2000Number = EncoderClass.ByteArrayToUlong(EncoderClass.BitArrayToByteArray(simple3_2000Numbers[j]));

                    if (simpleNumerULong % simple3_2000Number != 0)
                    {
                        flagDivision = false;
                    }
                }

                if (!flagDivision)
                {
                    continue;
                }

                bool flagTestMillerRabin = false;
                // Делаем тесты Рабина-Миллера
                for (int numTests = 0; numTests < t; numTests++)
                {

                }

                if (!flagTestMillerRabin)
                {
                    trueSimpleValue = new BitArray(simpleNumber);
                    break;
                }

            }

            return trueSimpleValue;
        }

    }
}
