using PSZI_lr1_v2;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;

namespace PSZI_lr1
{
    class Program
    {
        public string originalText;
        public string key;
        public string cipherText;

        const string fileNameStartText = "originalText.txt";
        const string fileNameCipherText = "cipherText.txt";
        const string fileNameKey = "key.txt";

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
            Console.WriteLine("Ключ = " + "\'" + originalText + "\'");
        }
        

        // Генерация ключа
        public void GenerateKey(ModeGenKey command)
        {
            GeneratorKey generatorKey = new GeneratorKey();
            generatorKey.GenerateKey(command, originalText.Length);
            key = generatorKey.key;
            writeToFile(fileNameKey, key);
            Console.WriteLine("Ключ = " + "\'" + key + "\'");
        }


        public string toBin(string str)
        {
            string cc2 = "";
            for (int i = 0; i < str.Length; i++)
                cc2 += Convert.ToString(str[i], 2);
            return cc2;
        }
        public string toHex(string str)
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

            double relativeNumberOfOnes = KeyToBit.Count(x => x == '1') / KeyToBit.Length;
            double relativeNumberOfZeros = KeyToBit.Count(x => x == '0') / KeyToBit.Length;

            double ratio = 0.0;

            if (relativeNumberOfOnes != relativeNumberOfZeros)
            {
                ratio = Math.Abs(relativeNumberOfOnes - relativeNumberOfZeros);
            }
            return ratio;
        }

        /*public int calcFirstСycleLengthInBin(string key)
        {

        }*/

        /*public double calcСorrelation(string key)
        {
            long keyToNumber = Convert.ToInt32(key);

            long shiftKey = keyToNumber >> calcFirstСycleLengthInBin(key);

            return calcBalance(Convert.ToString(shiftKey ^ keyToNumber));
        }*/

    }
}
