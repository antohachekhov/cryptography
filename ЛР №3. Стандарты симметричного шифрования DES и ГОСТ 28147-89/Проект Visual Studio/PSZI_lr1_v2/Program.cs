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
    }

    class Program
    {
        public BitArray originalText;
        public BitArray key;
        public BitArray cipherText;
        public GeneratorKey generatorKey;
        public EncryptByDES encryptorByDES;
        public int countRounds;
        public int lengthBlock = 64;
        public string[] belowKeys = new string[16];

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
        public void GenerateKey()
        {
            generatorKey = new GeneratorKey(key);
        }


        // Определение обьекта, который будет шифровать
        public void GenerateEncryptor()
        {
            //encryptorByFeistelNetwork = new EncryptorByFeistelNetwork(command);
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


        public BitArray[] DividingTextIntoBlocks(BitArray text)
        {

            BitArray[] originalTextBlocks = new BitArray[text.Length / lengthBlock];

            // Разделение текста на блоки по 64 бита
            for (int i = 0; i < text.Length / lengthBlock; i++)
            {
                BitArray bitArray64 = new BitArray(lengthBlock);

                // Подсчет левой части
                for (int j = 0; j < lengthBlock; j++)
                    bitArray64[j] = text[i * lengthBlock + j];

                originalTextBlocks[i] = new BitArray(bitArray64);
            }

            return originalTextBlocks;
        }

        public void Encryption()
        {
            BitArray[] originalTextBlocks = DividingTextIntoBlocks(originalText);
            BitArray cipherText = new BitArray(0);

            // Шифруем каждый блок
            for (int iBlock = 0; iBlock < originalTextBlocks.Length; iBlock++)
            {
                BitArrayFunctions.Append(cipherText, encryptorByDES.Encrypte(originalTextBlocks[iBlock]));
            }

            this.cipherText = cipherText;
        }




        public int[] searchAvalancheEffect(int index, ModeChooseAvalanche chooseAvalanche)
        {

            BitArray originalTextFalse = new BitArray(originalText);
            BitArray originalTextTrue = new BitArray(originalText);

            BitArray keyFalse = new BitArray(key);
            BitArray keyTrue = new BitArray(key);
            if (chooseAvalanche == ModeChooseAvalanche.originalText)
            {
                originalTextFalse.Set(index, false);
                originalTextTrue.Set(index, true);
            }
            else
            {
                keyFalse.Set(index, false);
                keyTrue.Set(index, true);
            }

            GeneratorKey generatorFalse = new GeneratorKey(keyFalse);
            GeneratorKey generatorTrue = new GeneratorKey(keyTrue);


            return encryptorByDES.searchAvalancheEffect(originalTextTrue, originalTextFalse, generatorTrue, generatorFalse);
        }

        internal void Decryption()
        {
            BitArray[] cipherTextBlocks = DividingTextIntoBlocks(cipherText);
            BitArray originalText = new BitArray(0);

            // Шифруем каждый блок
            for (int iBlock = 0; iBlock < cipherTextBlocks.Length; iBlock++)
            {
                BitArrayFunctions.Append(cipherText, encryptorByDES.Decrypte(cipherTextBlocks[iBlock]));
            }

            this.originalText = originalText;
        }
    }
}
