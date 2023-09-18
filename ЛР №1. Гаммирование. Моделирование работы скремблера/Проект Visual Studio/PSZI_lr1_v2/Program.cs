using PSZI_lr1_v2;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;

namespace PSZI_lr1
{
    class Program
    {
        public BitArray originalText;
        public BitArray key;
        public BitArray startshift;
        public BitArray cipherText;
        public GeneratorKey generatorKey = new GeneratorKey();

        const string fileNameStartText = "originalText.txt";
        const string fileNameCipherText = "cipherText.txt";
        const string fileNameKey = "key.txt";
        const string fileNameStartShiftRegister = "startShiftRegister.txt";


        // Чтение текста из файла
        public void ReadOriginalText(string filename)
        {
            Console.WriteLine("Читаем текст из файла...");
            originalText = EncoderClass.StringToBitArray(readFromFile(filename));
            Console.WriteLine("Текст = " + "\'" + originalText + "\'");
        }

        public void ReadKey(string filename)
        {
            Console.WriteLine("Читаем ключ из файла...");
            key = EncoderClass.StringToBitArray(filename);
            Console.WriteLine("Ключ = " + "\'" + key + "\'");
        }

        public void ReadScr(string filename)
        {
            Console.WriteLine("Читаем начальное значение скремблера из файла...");
            startshift = EncoderClass.StringToBitArray(readFromFile(filename));
            Console.WriteLine("Начальное значение скремблера = " + "\'" + startshift + "\'");
        }

        // Генерация ключа
        public void GenerateKey(ModeGenKey command)
        {
            key = generatorKey.GenerateKey(command, originalText);

            startshift = generatorKey.startShiftRegister;
            // Вывод ключа
            writeToFile(fileNameKey, EncoderClass.BitArrayToString(key));

            // Вывод стартового значения сдвигового регистра
            writeToFile(fileNameStartShiftRegister, EncoderClass.BitArrayToString(generatorKey.startShiftRegister));
        }

        public static string readFromFile(string fileName)
        {
            string text = "";
            try
            {
                // Открываем файл для чтения текста 
                using (var sr = new StreamReader(fileName))
                {
                    text = sr.ReadToEnd();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Файл " + e.Message.ToString() + " не удалось прочитать.");
            }

            return text;
        }


        public static void writeToFile(string fileName, string text)
        {
            try
            {
                // Открытие файла для записи данных
                using (var sr = new StreamWriter(fileName))
                {

                    sr.Write(text);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("В файл " + e.Message.ToString() + " невозможно записать данные");
            }
        }

        public double calcBalance(BitArray keyToBitArray)
        {
            string KeyToBit = EncoderClass.BitArraytoBinString(keyToBitArray);

            double relativeNumberOfOnes = (double)KeyToBit.Count(x => x == '1') / KeyToBit.Length;
            double relativeNumberOfZeros = (double)KeyToBit.Count(x => x == '0') / KeyToBit.Length;

            double ratio = 0.0;

            if (relativeNumberOfOnes != relativeNumberOfZeros)
            {
                ratio = Math.Abs(relativeNumberOfOnes - relativeNumberOfZeros);
            }
            Console.WriteLine("ratio:" + ratio);
            return ratio;
        }


        public double calcPeriod(ModeGenKey command, string startShiftRegister)
        {
            long startShiftRegisterInt = EncoderClass.ByteArrayToLong(EncoderClass.StringToByteArray(startShiftRegister));
            LFSR lfsr = new LFSR((int)command);
            int period = lfsr.calcPeriod(startShiftRegisterInt);
            return period;
        }


        public int calcFirstСycleLengthInBin(BitArray keyToBitArray)
        {
            bool firstBit = keyToBitArray.Get(0);
            int sizeOfFirstCicle = 1;
            for (int i = 1; i < keyToBitArray.Length && keyToBitArray.Get(i) == firstBit; i++, sizeOfFirstCicle++) ;

            return sizeOfFirstCicle;
        }

        public double calcChiSquare(BitArray keyToBitArray)
        {
            string KeyToBit = EncoderClass.BitArraytoBinString(keyToBitArray);

            double relativeNumberOfOnes = (double)KeyToBit.Count(x => x == '1') / KeyToBit.Length;
            double relativeNumberOfZeros = (double)KeyToBit.Count(x => x == '0') / KeyToBit.Length;

            double chi = Math.Pow(relativeNumberOfOnes - 0.5, 2) / 0.5 + Math.Pow(relativeNumberOfZeros - 0.5, 2) / 0.5;

            return chi;
        }

        public string[] regexSplit(string str, string regexStr)
        {
            Regex regex = new Regex(regexStr);
            string[] substrings = regex.Split(str);
            return substrings;
        }

        public List<double> calcСyclicality(BitArray keyToBitArray)
        {
            string KeyToBit = EncoderClass.BitArraytoBinString(keyToBitArray);
            // Определение длин циклов с 1
            string[] strs1 = regexSplit(KeyToBit, "0+").Where(s => !string.IsNullOrEmpty(s)).ToArray();
            // Определение длин циклов с 0
            string[] strs0 = regexSplit(KeyToBit, "1+").Where(s => !string.IsNullOrEmpty(s)).ToArray();

            // Общее количество циклов в последовательности
            int nCicles = strs1.Length + strs0.Length;
            int tempNCicles = 0;

            List<double> relativeCountCicles = new List<double>();

            int countCicles = 1;
            // Поиск количества циклов, пока циклы не закончатся
            for (int lengthCicle = 1; tempNCicles != nCicles; lengthCicle++)
            {
                countCicles = strs1.Count(x => x.Length == lengthCicle);
                countCicles += strs0.Count(x => x.Length == lengthCicle);
                tempNCicles += countCicles;

                relativeCountCicles.Add((double)countCicles / nCicles);
            }
            Console.WriteLine(string.Join(", ", relativeCountCicles));
            return relativeCountCicles;
        }

        public double calcСorrelation(ModeGenKey command, BitArray keyToBitArray, string startShiftRegister)
        {
            Console.WriteLine("Входной ключ: " + String.Join(", ", keyToBitArray));

            int shiftLengthInBit = calcFirstСycleLengthInBin(keyToBitArray);

            // Генерация такого же ключа только с дополнительными символами
            BitArray newKeyToBitArray = new BitArray(keyToBitArray.Length + shiftLengthInBit);


            newKeyToBitArray = generatorKey.GenerateKey(command, newKeyToBitArray);
            Console.WriteLine("Сгенерированный ключ: " + String.Join(", ", newKeyToBitArray));

            // Осуществляем циклический сдвиг 
            bool[] shiftKeyToBool = new bool[keyToBitArray.Length];

            for(int i = 0; i < keyToBitArray.Count; i++)
            {
                shiftKeyToBool[i] = newKeyToBitArray.Get(i + shiftLengthInBit);
            }

            BitArray shiftKeyToBitArray = new BitArray(shiftKeyToBool);

            // XOR
            BitArray xorKeysToBitArray = keyToBitArray.Xor(shiftKeyToBitArray);
            Console.WriteLine("Разница ключей:" + String.Join(", ", xorKeysToBitArray));

            Console.WriteLine("Корреляция:" + calcBalance(xorKeysToBitArray));
            return calcBalance(xorKeysToBitArray);
        }

    }
}
