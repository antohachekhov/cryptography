using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace PSZI_lr1
{
    class Program
    {
        public string originalText;
        public string key;
        public static string cipherText;


        const string fileNameStartText = "originalText.txt";
        const string fileNameCipherText = "cipherText.txt";
        const string fileNameKey = "key.txt";

        static void Start()
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
        }


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
        public void GenerateKey(int command) // command = 0 - случайно, 1 - 1й скремблер, 2 - 2й скремблер
        {
            Console.WriteLine("Генерируем ключ...");
            if (command == 0)
            {
                key = CipherXOR.generateKey(originalText.Length);
            }
            else
            {
                if (command > 2 || command < 1)
                {
                    throw new Exception("Неизвестная ошибка");
                }
                else
                {
                    LFSR lfsr = new LFSR(command);
                    long startShiftRegister = 0; // ХЗ ЧТО ЭТО
                    key = Convert.ToString(lfsr.generatePRV(originalText.Length, startShiftRegister));
                }
            }
            
            writeToFile(fileNameKey, key);
            Console.WriteLine("Ключ = " + "\'" + key + "\'");
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



    }
}
