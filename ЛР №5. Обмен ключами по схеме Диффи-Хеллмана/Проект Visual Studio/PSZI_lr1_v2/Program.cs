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

        public static BitArray Add10(BitArray bitArray)
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

            return bitArray;
        }

        public static BitArray Add1(BitArray bitArray)
        {
            bool flagAdded = false;


            for (int i = 0; !flagAdded && i < bitArray.Length; i++)
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

            return bitArray;
        }
    }


    public struct SimpleValueWithNumItersAndTime
    {
        public BitArray trueSimpleValue;
        public int numIters;
        public long time;

        public SimpleValueWithNumItersAndTime(BitArray trueSimpleValue, int numIters, long time) : this()
        {
            this.trueSimpleValue = trueSimpleValue;
            this.numIters = numIters;
            this.time = time;
        }
    }

    public struct ValuesWithTime
    {
        public List<BitArray> values;
        public long time;

        public ValuesWithTime(List<BitArray> trueSimpleValues, long time) : this()
        {
            this.values = trueSimpleValues;
            this.time = time;
        }
    }

    public class Program
    {

        // Количество бит в числе
        public int n;

        // Количество проверок в тесте Рабина-Миллера
        public int t;

        List<BitArray> simple3_2000Numbers;

        // первообразный корень n
        public int g;



        // Генерирует простое число указанной размерности
        public SimpleValueWithNumItersAndTime generateSimpleNumberByN()
        {
            // Задаем smallerNumber и largerNumber чтобы не писать два раза один и тот же алгоритм в функции
            BitArray smallerNumber = new BitArray(n);
            smallerNumber[n - 1] = true;

            BitArray largerNumber = new BitArray(n + 1);
            largerNumber[n] = true;

            return generateSimpleNumberFromRange(smallerNumber, largerNumber);
        }

        // Генерирует простое число от нижней границы до верхней, если простого числа нет, то возвращает null
        // Диапазон не включает числа smallerNumber и largerNumber.
        public SimpleValueWithNumItersAndTime generateSimpleNumberFromRange(BitArray smallerNumber, BitArray largerNumber)
        {
            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();
            BitArray trueSimpleValue = null;

            BitArray simpleNumber = new BitArray(smallerNumber);
            ulong simpleNumberULong = EncoderClass.BitArrayToUlong(simpleNumber);
            ulong largerNumberULong = EncoderClass.BitArrayToUlong(largerNumber);

            simpleNumber = BitArrayFunctions.Add1(simpleNumber);
            // Делаем число нечетным
            simpleNumber[0] = true;
            int i = 0;
            for (; simpleNumberULong < largerNumberULong; i++, simpleNumber = BitArrayFunctions.Add10(simpleNumber))
            {
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

                        if (!flagDivision2) simple3_2000Numbers.Add(EncoderClass.UlongToBitArray(simpleNumber2ULong));
                    }
                }

                bool flagDivision = true;
                simpleNumberULong = EncoderClass.BitArrayToUlong(simpleNumber);
                Console.WriteLine(simpleNumberULong);
                for (int j = 0; flagDivision && j < simple3_2000Numbers.Count; j++)
                {
                    ulong simple3_2000Number = EncoderClass.BitArrayToUlong(simple3_2000Numbers[j]);

                    if (simpleNumberULong >= simple3_2000Number)
                        break;

                    if (simpleNumberULong % simple3_2000Number == 0)
                    {
                        flagDivision = false;
                    }
                }

                if (!flagDivision)
                {
                    Console.WriteLine("Число " + simpleNumberULong + "не прошло проверку на деление");
                    continue;
                }

                // Делаем тесты Рабина-Миллера
                bool flagTestMillerRabin = testRabinMiller(simpleNumberULong);


                if (flagTestMillerRabin)
                {

                    trueSimpleValue = new BitArray(simpleNumber);
                    break;
                }
                else
                {
                    Console.WriteLine("Число " + simpleNumberULong + "не прошло проверку на тесте");
                }

            }
            //останавливаем счётчик
            stopwatch.Stop();
            long time = stopwatch.ElapsedMilliseconds;

            return new SimpleValueWithNumItersAndTime(trueSimpleValue, i, time);
        }

        public bool testRabinMiller(ulong number)
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
                int a = rnd.Next(2, (int)number - 2);

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


        public void addSampleFactorAndDecreaseNumber(List<BitArray> sampleFactors, ref ulong numberUlong, ulong sampleNumberUlong)
        {
            if (numberUlong % sampleNumberUlong == 0)
            {
                sampleFactors.Add(EncoderClass.UlongToBitArray(sampleNumberUlong));

                // Уменьшаем number пока оно не перестанет иметь множитель 2
                while (numberUlong % sampleNumberUlong == 0)
                    numberUlong /= sampleNumberUlong;
            }
        }

        public List<BitArray> getSampleFactors(ulong numberUlong)
        {
            List<BitArray> sampleFactors = new List<BitArray>();


            ulong sampleNumberUlong = 2;

            // Убираем четный множитель
            addSampleFactorAndDecreaseNumber(sampleFactors, ref numberUlong, sampleNumberUlong);


            BitArray number = EncoderClass.UlongToBitArray(numberUlong + 1);    // Крайняя граница на 1 больше чем число
            int maxIters = 1000;
            BitArray sampleNumber;
            for (int k = 0; k < maxIters && numberUlong != 1; k++)
            {
                sampleNumber = EncoderClass.UlongToBitArray(sampleNumberUlong);
                // Поиск простого числа < number
                SimpleValueWithNumItersAndTime result = generateSimpleNumberFromRange(sampleNumber, number);

                sampleNumberUlong = EncoderClass.BitArrayToUlong(result.trueSimpleValue);

                addSampleFactorAndDecreaseNumber(sampleFactors, ref numberUlong, sampleNumberUlong);
            }

            if (numberUlong != 1)
            {
                Console.WriteLine("Число n = " + numberUlong + " не нашло простого делителя");
            }

            return sampleFactors;
        }

        public ValuesWithTime getPrimitiveRoots(ulong numberUlong)
        {
            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();

            // TODO: подсчет фи(n)
            // TODO: Если n - простое то мы делаем то то
            // TODO: иначе то то


            // Нахождение простых множителей
            List<BitArray> sampleFactors = getSampleFactors(numberUlong);   // TODO: сюда надо впихнуть результат функции фи(n)

            int maxCountPrimitiveRoots = 100;
            List<BitArray> primitiveRoots = new List<BitArray>();

            for (ulong primitiveRootUlong = 2; primitiveRoots.Count != maxCountPrimitiveRoots; primitiveRootUlong++)
            {
                bool isPrimitiveRoot = true;
                foreach (BitArray sampleRoot in sampleFactors)
                {
                    ulong sampleRootUlong = EncoderClass.BitArrayToUlong(sampleRoot);

                    if (Math.Pow(primitiveRootUlong, numberUlong /*TODO: Тут должен быть результат фи(n) */ / sampleRootUlong) % numberUlong == 1)
                        isPrimitiveRoot = false;
                }

                if (isPrimitiveRoot)
                    primitiveRoots.Add(EncoderClass.UlongToBitArray(primitiveRootUlong));

            }
            //останавливаем счётчик
            stopwatch.Stop();
            long time = stopwatch.ElapsedMilliseconds;
            return new ValuesWithTime(primitiveRoots, time);
        }

        public ValuesWithTime generateAllSimpleNumbersFromRange(BitArray smallerNumber, BitArray largerNumber)
        {
            List<BitArray> simpleNumbers = new List<BitArray>();

            long sumTime = 0;

            while (true)
            {
                SimpleValueWithNumItersAndTime result = generateSimpleNumberFromRange(smallerNumber, largerNumber);

                if (result.trueSimpleValue == null)
                    break;

                sumTime += result.time;
                simpleNumbers.Add(result.trueSimpleValue);
                smallerNumber = result.trueSimpleValue;
            }

            return new ValuesWithTime(simpleNumbers, sumTime);
        }


    }
}
