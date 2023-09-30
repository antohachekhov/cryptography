using PSZI_lr1_v2;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows;

namespace PSZI_lr1
{
    public static class BitArrayFuctions
    {
        public static BitArray Append(this BitArray current, BitArray after)
        {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }
    }

    class Program
    {
        public BitArray originalText;
        public BitArray key;
        public BitArray cipherText;
        public GeneratorKey generatorKey;
        public EncryptorByFeistelNetwork encryptorByFeistelNetwork;
        public int countRounds;

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
            key = EncoderClass.StringToBitArray(readFromFile(filename));
            Console.WriteLine("Ключ = " + "\'" + key + "\'");
        }

        // Определение обьекта, который будет генерировать ключи
        // command - способ генерации подключа
        public void GenerateKey(ModeGenKey command)
        {
            generatorKey = new GeneratorKey(command, key);
        }


        // Определение обьекта, который будет шифровать
        // command - способ генерации образующей
        public void GenerateEncryptor(ModeGenFunc command)
        {
            encryptorByFeistelNetwork = new EncryptorByFeistelNetwork(command);
        }

        public void Encryption()
        {
            int lenghtOfBlock = originalText.Length / 2;
            BitArray firstBlockOfText = new BitArray(lenghtOfBlock);
            BitArray secondBlockOfText = new BitArray(lenghtOfBlock);

            if (lenghtOfBlock + lenghtOfBlock != originalText.Length)
                MessageBox.Show("Длины не совпадают! Никита был не прав, а Саша неправильно перевела текст", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            for (int j = 0; j < lenghtOfBlock; j++)
                firstBlockOfText[j] = originalText[j];

            for (int j = 0; j < lenghtOfBlock; j++)
                secondBlockOfText[j] = originalText[lenghtOfBlock + j];

            int i = 0;
            BitArray partKey = generatorKey.GenerateKey(0);

            dataToEncryption data = new dataToEncryption(firstBlockOfText, secondBlockOfText, partKey);

            for (; i < countRounds; i++, data.partKey = generatorKey.GenerateKey(i))
            {
                Console.WriteLine("Левая часть: " + EncoderClass.BitArraytoHexString(data.firstPartText));
                Console.WriteLine("Правая часть: " + EncoderClass.BitArraytoHexString(data.secondPartText));
                Console.WriteLine("Ключ: " + EncoderClass.BitArraytoHexString(data.partKey));

                data = encryptorByFeistelNetwork.Encrypte(data);


            }

            Console.WriteLine("Левая часть: " + EncoderClass.BitArraytoHexString(data.firstPartText));
            Console.WriteLine("Правая часть: " + EncoderClass.BitArraytoHexString(data.secondPartText));
            Console.WriteLine("Ключ: " + EncoderClass.BitArraytoHexString(data.partKey));
            cipherText = BitArrayFuctions.Append(data.firstPartText, data.secondPartText);
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
