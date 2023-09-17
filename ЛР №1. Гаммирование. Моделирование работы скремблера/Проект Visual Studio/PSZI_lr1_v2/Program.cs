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

        /*static void Start()
        {
            // Чтение текста из файла
            Console.WriteLine("Читаем текст из файла...");
            string originalText = readFromFile(fileNameStartText);
            Console.WriteLine("Текст = " + "\'" + originalText + "\'");

            // Генерация ключа
            Console.WriteLine("Генерируем ключ...");
            string key = CipherXOR.generateKey(originalText.Length);
            writeToFile(fileNameKey, key);
            Console.WriteLine("Ключ = " + "\'" + key + "\'");

            // Шифрование текста
            Console.WriteLine("Шифруем текст...");
            string cipherText = CipherXOR.encryptText(originalText, key);
            writeToFile(fileNameKey, cipherText);
            Console.WriteLine("Зашифрованный текст = " + "\'" + cipherText + "\'");

            Console.WriteLine("Hello World!");
        }*/


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
            generatorKey.GenerateKey(command, originalText);
            key = generatorKey.key;
            startshift = generatorKey.startShiftRegister;
            writeToFile(fileNameKey, key);
            writeToFile(fileNameStartShiftRegister, startshift);
            Console.WriteLine("Ключ = " + "\'" + key + "\'");
        }

        public static long toNum(string str)
        {
            byte[] asciiBytes = Encoding.ASCII.GetBytes(str);
            long num = 0;
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                num += Convert.ToInt32(asciiBytes[i]);
                Console.WriteLine("charValue:" + num);
            }
                        
            return num;
        }
        public static string toBin(string str)
        {
            string cc2 = "";
            for (int i = 0; i < str.Length; i++)
                cc2 += Convert.ToString(str[i], 2);
            return cc2;
        }
        public static string toHex(string str)
        {
            string cc16 = "";
            for (int i = 0; i < str.Length; i++)
                cc16 += Convert.ToString(str[i], 16);
            return cc16;
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
            string KeyToBit = toBin(key);

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
            long startShiftRegisterLong = toNum(startShiftRegister);
            LFSR lfsr = new LFSR((int)command);
            int period = lfsr.calcPeriod(startShiftRegisterLong);
            return period;
        }


        public static int calcFirstСycleLengthInBin(string key)
        {
            string KeyToBit = toBin(key);
            char firstBit = KeyToBit[0];
            int sizeOfFirstCicle = 1;
            for (int i = 1; i < KeyToBit.Length && KeyToBit[i] == firstBit; i++, sizeOfFirstCicle++) ;

            return sizeOfFirstCicle;
        }

        public double calcChiSquare(string key)
        {
            string KeyToBit = toBin(key);

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
            string KeyToBit = toBin(key);
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
            long keyToNumber = toNum(key);

            generatorKey.GenerateKey(ModeGenKey.LFSR1, key + " ");

            long shiftKey = toNum(generatorKey.key);
            Console.WriteLine("shiftKey:" + shiftKey);

            Console.WriteLine("keyToNumber:" + keyToNumber);

            shiftKey = shiftKey >> calcFirstСycleLengthInBin(key);

            Console.WriteLine("Корреляция:" + calcBalance(Convert.ToString(shiftKey ^ keyToNumber)));
            Console.WriteLine("shiftKey:" + shiftKey);
            return calcBalance(Convert.ToString(shiftKey ^ keyToNumber));
        }

    }
}
