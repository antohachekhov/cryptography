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
                bitArray = Append(bitArray, bitArrayInByte);
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

        public static BitArray Sum(BitArray bitArray1, BitArray bitArray2)
        {
            BitArray newBitArray = new BitArray(bitArray1);

            for (int i = 0; i < bitArray2.Length; i++)
            {
                if (bitArray2[i] == true)
                    newBitArray = Add1ToPos(newBitArray, i);
            }
            return newBitArray;
        }

        public static BitArray Add1ToPos(BitArray bitArray, int pos)
        {
            bool flagAdded = false;


            for (int i = pos; !flagAdded && i < bitArray.Length; i++)
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
                int dopLength = pos - bitArray.Length > 0 ? pos - bitArray.Length + 1 : 1;

                BitArray dopBitArray = new BitArray(dopLength);
                dopBitArray[dopLength - 1] = true;
                bitArray = bitArray.Append(dopBitArray);
            }

            return bitArray;
        }


        public static BitArray GetBitArrayWithLengthFromBitArray(BitArray bitArray, int maxLength)
        {
            BitArray returned = new BitArray(maxLength);
            for (int i = 0; i < maxLength && i < bitArray.Length; i++)
            {
                returned[i] = bitArray[i];
            }

            return returned;
        }

        public static BitArray GenerateRandomBitArray(Random random, BitArray startBitArray, BitArray endBitArray)
        {
            byte[] randomBytes = new byte[EncoderClass.byteCountLong];

            random.NextBytes(randomBytes);

            BitArray randomBits = GetBitArrayWithLengthFromBitArray(EncoderClass.ByteArrayToBitArray(randomBytes), endBitArray.Length);
            return randomBits;
        }

        public static BitArray Mult(BitArray bitArray1, BitArray bitArray2)
        {
            BitArray newBitArray = new BitArray(0);

            for (int i = 0; i < bitArray1.Length; i++)
            {
                if(bitArray1[i] == true)
                {
                    BitArray zeroBitArray = new BitArray(i);
                    newBitArray = Sum(newBitArray, Append(zeroBitArray, bitArray2));
                }
            }
            return newBitArray;
        }

        public static BitArray Pow(BitArray bitArray, ulong pow)
        {
            BitArray newBitArray = new BitArray(1, true);

            for (ulong i = 0; i < pow; i++)
            {
                newBitArray = Mult(newBitArray, bitArray);
            }

            return newBitArray;
        }

    }


    public struct NumberWithNumIters
    {
        public ulong number;
        public ulong numIters;

        public NumberWithNumIters(ulong number, ulong numIters) : this()
        {
            this.number = number;
            this.numIters = numIters;
        }
    }

    public struct FactorWithPow
    {
        public ulong number;
        public ulong pow;

        public FactorWithPow(ulong number, ulong pow) : this()
        {
            this.number = number;
            this.pow = pow;
        }
    }

    public class Program
    {
        List<ulong> simple3_2000Numbers;

        public Random random = new Random();

        public bool checkNumberIsSimple(ulong number, int t)
        {
            // Проверка на делимость
            // Подсчет simple3_2000Numbers
            if (simple3_2000Numbers == null)
            {
                simple3_2000Numbers = new List<ulong>();
                for (ulong simpleNumber = 2; simpleNumber < 2000; simpleNumber++)
                {
                    bool flagDivision = false;
                    for (ulong divNumber = 2; divNumber < simpleNumber; divNumber++)
                    {
                        if (simpleNumber % divNumber == 0 && simpleNumber != divNumber)
                        {
                            flagDivision = true;
                            break;
                        }
                    }

                    if (!flagDivision) simple3_2000Numbers.Add(simpleNumber);
                }
            }

            for (int j = 0; j < simple3_2000Numbers.Count && simple3_2000Numbers[j] < number; j++)
            {
                if (number % simple3_2000Numbers[j] == 0)
                {
                    Console.WriteLine("Число " + number + "не прошло проверку на деление");
                    return false;
                }
            }

            // Делаем тесты Рабина-Миллера
            bool flagTestMillerRabin = testRabinMiller(number, t);


            if (!flagTestMillerRabin)
            {
                Console.WriteLine("Число " + number + "не прошло проверку на тесте");
                return false;
            }

            return true;
        }

        // Генерирует простое число указанной размерности
        public NumberWithNumIters generateSimpleNumberByN(int n, int t)
        {

            // Задаем smallerNumber и largerNumber чтобы не писать два раза один и тот же алгоритм в функции
            BitArray smallerNumber = new BitArray(n);
            smallerNumber[n - 1] = true;

            BitArray largerNumber = new BitArray(n + 1);
            largerNumber[n] = true;

            ulong randomNumber;

            byte[] randomBytes = new byte[EncoderClass.byteCountLong];

            ulong numIters = 0;
            for (; ; numIters++)
            {
                random.NextBytes(randomBytes);

                BitArray randomBits = BitArrayFunctions.GenerateRandomBitArray(random, smallerNumber, largerNumber);
                randomBits[0] = true;
                randomBits[n - 1] = true;
                randomNumber = EncoderClass.BitArrayToUlong(randomBits);

                if (checkNumberIsSimple(randomNumber, t))
                    break;
            }

            return new NumberWithNumIters(randomNumber, numIters);
        }

        // Генерирует простое число от нижней границы до верхней, если простого числа нет, то возвращает null
        // Диапазон не включает числа smallerNumber и largerNumber.
        public NumberWithNumIters generateSimpleNumberFromRange(ulong smallerNumber, ulong largerNumber, int t)
        {
            ulong trueSimpleNumber = 0;
            ulong simpleNumber = smallerNumber + 1; // Границы не входят в диапазон

            // Делаем число нечетным
            if (simpleNumber % 2 == 0)
                simpleNumber++;

            ulong numIters = 0;
            for (; simpleNumber < largerNumber; numIters++, simpleNumber += 2)
            {
                if (checkNumberIsSimple(simpleNumber, t))
                {
                    trueSimpleNumber = simpleNumber;
                    break;
                }

            }

            return new NumberWithNumIters(trueSimpleNumber, numIters);
        }

        public bool testRabinMiller(ulong number, int t)
        {
            if (number == 1 || number == 3)
            {
                return true;
            }

            if (number < 2 || number % 2 == 0)
                return false;

            ulong m = number - 1;
            int b = 0;

            while (m % 2 == 0)
            {
                m /= 2;
                b += 1;
            }

            // m = (p - 1) / 2^b

            for (int i = 0; i < t; i++)
            {
                // 1 шаг
                Random rnd = new Random();
                BitArray randomBitArray = BitArrayFunctions.GenerateRandomBitArray(random,
                    EncoderClass.UlongToBitArray(2), EncoderClass.UlongToBitArray(number - 2));
                ulong a = EncoderClass.BitArrayToUlong(randomBitArray);

                // 2 шаг
                ulong z = (ulong)Math.Pow(a, m) % number;

                // 3 шаг
                if (z == 1 || z == number - 1)
                {
                    continue;
                }

                int j = 0;

                while (j < b)
                {
                    // 4 шаг
                    if (j > 0 && z == 1)
                    {
                        return false;
                    }

                    // 5 шаг
                    j++;
                    if (j < b && z < number - 1)
                    {
                        z = (ulong)(Math.Pow(z, 2) % number);
                        continue;
                    }

                    if (z == number - 1)
                    {
                        return true;
                    }
                }


                // 6 шаг
                if (j == b && z != number - 1)
                {
                    return false;
                }
            }

            return true;

        }


        public void addSampleFactorAndDecreaseNumber(List<FactorWithPow> sampleFactors, ref ulong numberUlong, ulong sampleNumberUlong)
        {
            if (numberUlong % sampleNumberUlong == 0)
            {
                // Уменьшаем number пока оно не перестанет иметь множитель
                ulong pow = 0;
                for (; numberUlong % sampleNumberUlong == 0; pow++)
                    numberUlong /= sampleNumberUlong;

                sampleFactors.Add(new FactorWithPow(sampleNumberUlong, pow));
            }
        }

        public List<FactorWithPow> getSampleFactors(ulong number, int t)
        {
            List<FactorWithPow> sampleFactorsWithPows = new List<FactorWithPow>();

            ulong sampleNumberUlong = 2;

            // Убираем четный множитель
            addSampleFactorAndDecreaseNumber(sampleFactorsWithPows, ref number, sampleNumberUlong);


            int maxIters = 1000;
            for (int k = 0; k < maxIters && number != 1; k++)
            {
                // Поиск простого числа < number
                NumberWithNumIters result = generateSimpleNumberFromRange(sampleNumberUlong, number + 1, t);

                Console.WriteLine(result.number);
                sampleNumberUlong = result.number;
                addSampleFactorAndDecreaseNumber(sampleFactorsWithPows, ref number, sampleNumberUlong);
            }

            if (number != 1)
            {
                Console.WriteLine("Число n = " + number + " не нашло простого делителя");
            }

            return sampleFactorsWithPows;
        }

        public List<ulong> getPrimitiveRoots(ulong numberUlong, int t)
        {
            List<FactorWithPow> sampleFactorsWithPowsFromN = getSampleFactors(numberUlong, t);

            ulong phi = 1;
            foreach (FactorWithPow sampleFactorWithPow in sampleFactorsWithPowsFromN)
            {
                phi *= (sampleFactorWithPow.number - 1) * (ulong)Math.Pow(sampleFactorWithPow.number, sampleFactorWithPow.pow - 1);
            }

            // Нахождение простых множителей
            List<FactorWithPow> sampleFactorsWithPowsFromPhi = getSampleFactors(phi, t);

            int maxCountPrimitiveRoots = 100;
            List<ulong> primitiveRoots = new List<ulong>();

            for (ulong primitiveRootUlong = 2; primitiveRoots.Count != maxCountPrimitiveRoots; primitiveRootUlong++)
            {
                bool isPrimitiveRoot = true;
                foreach (FactorWithPow sampleRoot in sampleFactorsWithPowsFromPhi)
                {
                    if (Math.Pow(primitiveRootUlong, phi / sampleRoot.number) % numberUlong == 1)
                        isPrimitiveRoot = false;
                }

                if (isPrimitiveRoot)
                    primitiveRoots.Add(primitiveRootUlong);
            }

            return primitiveRoots;
        }

        public List<ulong> generateAllSimpleNumbersFromRange(ulong smallerNumber, ulong largerNumber, int t)
        {
            List<ulong> simpleNumbers = new List<ulong>();

            while (true)
            {
                NumberWithNumIters result = generateSimpleNumberFromRange(smallerNumber, largerNumber, t);

                if (result.number == 0)
                    break;

                simpleNumbers.Add(result.number);
                smallerNumber = result.number;
            }

            return simpleNumbers;
        }

        public ulong generateY(ulong g, ulong xa, ulong n)
        {
            BitArray powBits = BitArrayFunctions.Pow(EncoderClass.UlongToBitArray(g), xa);


            // Я ЗАЕБАЛАСЬ
            // Крч тут идет переполнение, т.к. g^большое простое число = точно не больщое число, а сверх большое число
            // Мне было лень писать деление и разность, но похоже тут не обойтись
            // И еще надо подумать как вывести на экран, похоже цифрами(
            return EncoderClass.BitArrayToUlong(powBits) % n;
        }
    }
}
