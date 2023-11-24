using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Numerics;

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

        public static BitArray Remove1FromPose(BitArray bitArray, int pos)
        {
            if (bitArray[pos])
            {
                bitArray[pos] = false;
            }
            else
            {
                bool oneFounded = false;
                int posOne;
                for (posOne = pos; !oneFounded && posOne < bitArray.Length; posOne++)
                {
                    if (bitArray[posOne])
                        oneFounded = true;
                }
                if (oneFounded)
                {
                    for (int i = pos; i < posOne; i++)
                        bitArray[i] = !bitArray[i];
                }
                else
                {
                    Console.WriteLine("Error");
                }
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

        public static BitArray Pow(BitArray bitArray, BigInteger pow)
        {
            BitArray newBitArray = new BitArray(1, true);

            for (BigInteger i = 0; i < pow; i++)// TODO: сделать через двоичную степень
            {
                newBitArray = Mult(newBitArray, bitArray);
            }

            return newBitArray;
        }

        public static BitArray Diff(BitArray bitArray1, BitArray bitArray2)
        {
            BitArray newBitArray = new BitArray(bitArray1.Length);
            for (int i = 0; i < bitArray1.Length; i++)
            {
                if (i < bitArray2.Length)
                {
                    if (bitArray1[i])
                    {
                        newBitArray[i] = !bitArray2[i];
                    }
                    else
                    {
                        if (bitArray2[i])
                        {
                            bitArray1 = Remove1FromPose(bitArray1, i);
                            newBitArray[i] = true;
                        }
                    }
                }
                else
                {
                    newBitArray[i] = bitArray1[i];
                }
            }

            bool oneFounded = false;
            int j;
            for (j = newBitArray.Length - 1; j >= 0; j--)
            {
                if (newBitArray[j])
                    break;
            }

            if (j != newBitArray.Length - 1)
            {
                BitArray copiedBitArray = new BitArray(j + 1);
                for (int k = 0; k < j + 1; k++)
                    copiedBitArray[k] = newBitArray[k];
                newBitArray = copiedBitArray;
            }

            return newBitArray;
        }


        public static BitArray Div(BitArray bitArray1, BitArray bitArray2)
        {
            BitArray newBitArray = new BitArray(1);


            return newBitArray;
        }

        public static bool Less(BitArray bitArray1, BitArray bitArray2)
        {
            bool isLess = false;
            bool isLengthLess = bitArray1.Length < bitArray2.Length;    

            BitArray largerBitArray = isLengthLess ? bitArray2 : bitArray1;
            BitArray smallerBitArray = isLengthLess ? bitArray1 : bitArray2;
            int i = largerBitArray.Length - 1;
            for (; i >= smallerBitArray.Length; i--)
            {
                if(largerBitArray[i] && isLengthLess)
                {
                    return true;
                }
            }

            for (; i >= 0 && bitArray1[i] == bitArray2[i]; i--);

            if (i != -1 && bitArray2[i] == true)
                return true;

            return isLess;
        }


        public static BitArray Mod(BitArray bitArray1, BitArray bitArray2)
        {
            BitArray newBitArray = new BitArray(1, true);

            //for (ulong i = 0; i < pow; i++)
            {
           //     newBitArray = Mult(newBitArray, bitArray);
            }

            return newBitArray;
        }
    }

    public struct NumberWithNumIters
    {
        public BigInteger number;
        public ulong numIters;

        public NumberWithNumIters(BigInteger number, ulong numIters) : this()
        {
            this.number = number;
            this.numIters = numIters;
        }
    }

    public struct FactorWithPow
    {
        public BigInteger number;
        public int pow;

        public FactorWithPow(BigInteger number, int pow) : this()
        {
            this.number = number;
            this.pow = pow;
        }
    }

    public class Program
    {
        List<ulong> simple3_2000Numbers;

        public Random random = new Random();

        public bool checkNumberIsSimple(BigInteger numberBitArray, int t)
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

            for (int j = 0; j < simple3_2000Numbers.Count; j++)
            {
                BigInteger simple3_2000NumberBigInteger = new BigInteger(simple3_2000Numbers[j]);

                if (simple3_2000NumberBigInteger >= numberBitArray)
                    break;

                if (numberBitArray % simple3_2000NumberBigInteger == 0)
                {
                    Console.WriteLine("Число " + numberBitArray + "не прошло проверку на деление");
                    return false;
                }
            }

            // Делаем тесты Рабина-Миллера
            bool flagTestMillerRabin = testRabinMiller(numberBitArray, t);


            if (!flagTestMillerRabin)
            {
                Console.WriteLine("Число " + numberBitArray + "не прошло проверку на тесте");
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

            BitArray largerNumber = new BitArray(n, true);

            byte[] randomBytes = new byte[EncoderClass.byteCountLong];
            BigInteger randomNumber;
            ulong numIters = 0;
            for (; ; numIters++)
            {
                random.NextBytes(randomBytes);

                BitArray randomBits = BitArrayFunctions.GenerateRandomBitArray(random, smallerNumber, largerNumber);
                randomBits[0] = true;
                randomBits[n - 1] = true;
                randomNumber = EncoderClass.BitArrayToBigInteger(randomBits);

                if (checkNumberIsSimple(randomNumber, t))
                    break;
            }

            return new NumberWithNumIters(randomNumber, numIters);
        }

        // Генерирует простое число от нижней границы до верхней, если простого числа нет, то возвращает null
        // Диапазон не включает числа smallerNumber и largerNumber.
        public NumberWithNumIters generateSimpleNumberFromRange(BigInteger smallerNumber, BigInteger largerNumber, int t)
        {
            BigInteger trueSimpleNumber = 0;
            BigInteger simpleNumber = smallerNumber + 1; // Границы не входят в диапазон

            // Делаем число нечетным
            if (simpleNumber % 2 == 0)
                simpleNumber++;

            ulong numIters = 0;
            for (; simpleNumber < largerNumber ;
                numIters++, simpleNumber += 2)
            {
                if (checkNumberIsSimple(simpleNumber, t))
                {
                    trueSimpleNumber = simpleNumber;
                    break;
                }

            }

            return new NumberWithNumIters(trueSimpleNumber, numIters);
        }

        public bool testRabinMiller(BigInteger number, int t)
        {
            if(number != 0 || number != 1 || number != 2 || number != 3)  // Если число равно 0 || 1 || 2 || 3
                return true;

            if (number % 2 == 0)
                return false;

            BigInteger m = number - 1;
            int b = 0;

            while (m % 2 == 0)
            {
                m /= 2;
                b += 1;
            }

            // m = (p - 1) / 2^b

            BitArray bitArrayFrom2 = new BitArray(new bool[2] {false, true});
            for (int i = 0; i < t; i++)
            {
                // 1 шаг
                Random rnd = new Random();

               
                BitArray a = BitArrayFunctions.GenerateRandomBitArray(random,
                    bitArrayFrom2, EncoderClass.BigIntegerToBitArray(number - 2));

                // 2 шаг
                BigInteger z = EncoderClass.BitArrayToBigInteger(BitArrayFunctions.Pow(a, m)) % number;

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
                        z = BigInteger.Pow(z, 2) % number;
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


        public void addSampleFactorAndDecreaseNumber(List<FactorWithPow> sampleFactors,
            ref BigInteger number, BigInteger sampleNumber)
        {
            if (number % sampleNumber == 0)
            {
                // Уменьшаем number пока оно не перестанет иметь множитель
                int pow = 0;
                for (; number % sampleNumber == 0; pow++)
                {
                    number /= sampleNumber;
                }

              sampleFactors.Add(new FactorWithPow(sampleNumber, pow));
            }
        }

        public List<FactorWithPow> getSampleFactors(BigInteger number, int t)
        {
            List<FactorWithPow> sampleFactorsWithPows = new List<FactorWithPow>();

            BigInteger sampleNumberUlong = 2;

            // Убираем четный множитель
            addSampleFactorAndDecreaseNumber(sampleFactorsWithPows, ref number, sampleNumberUlong);

            int maxIters = 1000;
            for (int k = 0; k < maxIters && number != 1; k++)
            {
                // Поиск простого числа < number
                NumberWithNumIters result = generateSimpleNumberFromRange(sampleNumberUlong,
                   number + 1, t);

                Console.WriteLine(result.number);
                sampleNumberUlong = result.number;

                if (sampleNumberUlong == 0)
                    break;
                addSampleFactorAndDecreaseNumber(sampleFactorsWithPows, ref number, sampleNumberUlong);
            }

            if (number != 1)
            {
                Console.WriteLine("Число n = " + number + " не нашло простого делителя");
            }

            return sampleFactorsWithPows;
        }

        public List<BigInteger> getPrimitiveRoots(BigInteger number, int t)
        {
            List<FactorWithPow> sampleFactorsWithPowsFromN = getSampleFactors(number, t);

            BigInteger phi = 1;
            foreach (FactorWithPow sampleFactorWithPow in sampleFactorsWithPowsFromN)
            {
                phi *= sampleFactorWithPow.number - 1;
                phi *= BigInteger.Pow(sampleFactorWithPow.number, sampleFactorWithPow.pow - 1);
            }

            // Нахождение простых множителей
            List<FactorWithPow> sampleFactorsWithPowsFromPhi = getSampleFactors(phi, t);

            int maxCountPrimitiveRoots = 100;
            List<BigInteger> primitiveRoots = new List<BigInteger>();

            for (BigInteger primitiveRootUlong = 2; primitiveRoots.Count != maxCountPrimitiveRoots; primitiveRootUlong++)
            {
                bool isPrimitiveRoot = true;
                foreach (FactorWithPow sampleRoot in sampleFactorsWithPowsFromPhi)
                {
                    if (BigInteger.Pow(primitiveRootUlong, (int)(phi / sampleRoot.number)) % number == 1)
                        isPrimitiveRoot = false;
                }

                if (isPrimitiveRoot)
                {
                    if(BigInteger.Pow(primitiveRootUlong, (int)phi) % number == 1)
                        primitiveRoots.Add(primitiveRootUlong);
                }
                    
            }

             return primitiveRoots;
        }

        public List<BigInteger> generateAllSimpleNumbersFromRange(BigInteger smallerNumber, BigInteger largerNumber, int t)
        {
            List<BigInteger> simpleNumbers = new List<BigInteger>();

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

        public int generateY(int g, BigInteger xa, int n)
        {
            BigInteger powBigInt = g;

            int n1 = n - 1;
            int r = (int)(xa % n1);

            for (int i = 0; i < r - 1; i++)
            {
                powBigInt *= g;
            }

            
            return (int)(powBigInt % n);
        }

        public int generateY2(ulong g, BigInteger xa, int n)
        {
            

            BitArray gb = EncoderClass.UlongToBitArray(g);

            BigInteger pow = EncoderClass.BitArrayToBigInteger(BitArrayFunctions.Pow(gb, xa));

            return (int)(pow % n);
        }
    }
}
