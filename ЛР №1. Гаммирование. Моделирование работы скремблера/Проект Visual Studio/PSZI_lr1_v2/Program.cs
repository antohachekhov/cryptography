using PSZI_lr1_v2;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PSZI_lr1
{
    class Program
    {
        public string originalText;
        public string key;
        public string startshift;
        public string cipherText;
        public GeneratorKey generatorKey = new GeneratorKey();

        const string fileNameStartText = "originalText.txt";
        const string fileNameCipherText = "cipherText.txt";
        const string fileNameKey = "key.txt";
        const string fileNameStartShiftRegister = "startShiftRegister.txt";


        // Чтение текста из файла
        public void ReadOriginalText(string filename)
        {
            Console.WriteLine("Читаем текст из файла...");
            originalText = readFromFile(filename);
            Console.WriteLine("Текст = " + "\'" + originalText + "\'");
        }

        public void ReadKey(string filename)
        {
            Console.WriteLine("Читаем ключ из файла...");
            key = readFromFile(filename);
            Console.WriteLine("Ключ = " + "\'" + key + "\'");
        }

        public void ReadScr(string filename)
        {
            Console.WriteLine("Читаем начальное значение скремблера из файла...");
            string scr = readFromFile(filename);
            Console.WriteLine("Начальное значение скремблера = " + "\'" + scr + "\'");
        }

        // Генерация ключа
        public void GenerateKey(ModeGenKey command)
        {
            byte[] keyToByte = generatorKey.GenerateKey(command, originalText);

            key = EncoderClass.ByteArrayToString(keyToByte);
            startshift = generatorKey.startShiftRegister;
            // Вывод ключа
            writeToFile(fileNameKey, key);

            // Вывод стартового значения сдвигового регистра
            writeToFile(fileNameStartShiftRegister,generatorKey.startShiftRegister);
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


        // + вызов на кнопку вывода баланса

        public double calcBalance(string key)
        {
            string KeyToBit = EncoderClass.StringtoBin(key, originalText.Length);

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
            uint startShiftRegisterInt = EncoderClass.ByteArrayToUint(EncoderClass.StringToByteArray(startShiftRegister));
            LFSR lfsr = new LFSR((int)command);
            int period = lfsr.calcPeriod(startShiftRegisterInt);
            return period;
        }


        public int calcFirstСycleLengthInBin(string key)
        {
            string KeyToBit = EncoderClass.StringtoBin(key, originalText.Length);
            char firstBit = KeyToBit[0];
            int sizeOfFirstCicle = 1;
            for (int i = 1; i < KeyToBit.Length && KeyToBit[i] == firstBit; i++, sizeOfFirstCicle++) ;

            return sizeOfFirstCicle;
        }

        public double calcChiSquare(string key)
        {
            string KeyToBit = EncoderClass.StringtoBin(key, originalText.Length);

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

        public List<double> calcСyclicality(string key)
        {
            string KeyToBit = EncoderClass.StringtoBin(key, originalText.Length);
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

        public double calcСorrelation(string key, string startShiftRegister)
        {
            byte[] keyToByte = EncoderClass.StringToByteArray(key);
            Console.WriteLine("Входной ключ: " + String.Join(", ", keyToByte));

            uint keyToUint = EncoderClass.ByteArrayToUint(keyToByte);
            Console.WriteLine("Входной ключ: " + keyToUint);

            int shiftLengthInBit = calcFirstСycleLengthInBin(key);

            string newKey = key;
            // Генерация такого же ключа только с дополнительными символами
            for (int i = 0; i < shiftLengthInBit / 8; i++)
            {
                newKey += " ";
            }
            byte[] shiftKeyByte = generatorKey.GenerateKey(ModeGenKey.LFSR1, newKey);
            uint shiftKeyToUint = EncoderClass.ByteArrayToUint(shiftKeyByte);
            Console.WriteLine("Сгенерированный ключ: " + shiftKeyToUint);

            // Осуществляем циклический сдвиг 
            shiftKeyToUint = shiftKeyToUint >> shiftLengthInBit;

            // XOR
            uint xorKeysToUint = shiftKeyToUint ^ keyToUint;
            string xorKeys = EncoderClass.ByteArrayToString(EncoderClass.UintToByteArray(xorKeysToUint));
            Console.WriteLine("Разница ключей:" + xorKeysToUint);
            Console.WriteLine("Разница ключей:" + xorKeys);

            Console.WriteLine("Корреляция:" + calcBalance(xorKeys));
            return calcBalance(xorKeys);
        }

    }
}
