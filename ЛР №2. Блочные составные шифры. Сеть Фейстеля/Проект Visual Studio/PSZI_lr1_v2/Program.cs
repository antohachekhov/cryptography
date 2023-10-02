﻿using PSZI_lr1_v2;
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
                bitArray = BitArrayFuctions.Append(bitArray, bitArrayInByte);
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

        public int countChangedBits(BitArray one, BitArray two)
        {
            BitArray bitChangeArray = new BitArray(one);
            bitChangeArray.Xor(two);

            int count = 0;

            for (int i = 0; i < bitChangeArray.Count; i++)
            {
                if (bitChangeArray.Get(i))
                    count++;
            }

            return count;

        }


        public int[] searchAvalancheEffect(int index)
        {
            BitArray originalTextFalse = new BitArray(this.originalText);
            BitArray originalTextTrue = new BitArray(this.originalText);

            int lenghtOfBlock = originalText.Length / 2;
            BitArray firstBlockOfTextFalse = new BitArray(lenghtOfBlock);
            BitArray secondBlockOfTextFalse = new BitArray(lenghtOfBlock);

            if (lenghtOfBlock + lenghtOfBlock != originalText.Length)
                MessageBox.Show("Длины не совпадают! Никита был не прав, а Саша неправильно перевела текст", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            for (int j = 0; j < lenghtOfBlock; j++)
                firstBlockOfTextFalse[j] = originalText[j];

            for (int j = 0; j < lenghtOfBlock; j++)
                secondBlockOfTextFalse[j] = originalText[lenghtOfBlock + j];

            BitArray firstBlockOfTextTrue = new BitArray(firstBlockOfTextFalse);
            BitArray secondBlockOfTextTrue = new BitArray(secondBlockOfTextFalse);

            firstBlockOfTextFalse.Set(index, false);
            firstBlockOfTextTrue.Set(index, true);
            secondBlockOfTextFalse.Set(index, false);
            secondBlockOfTextTrue.Set(index, true);

            int i = 0;
            BitArray partKey = generatorKey.GenerateKey(i);

            dataToEncryption dataFalse = new dataToEncryption(firstBlockOfTextFalse, secondBlockOfTextFalse, partKey);
            dataToEncryption dataTrue = new dataToEncryption(firstBlockOfTextTrue, secondBlockOfTextTrue, partKey);

            int[] countChangedBitsArray = new int[countRounds];
            for (; i < countRounds; i++, dataFalse.partKey = generatorKey.GenerateKey(i), dataTrue.partKey = generatorKey.GenerateKey(i))
            {
                dataFalse = encryptorByFeistelNetwork.Encrypte(dataFalse);
                dataTrue = encryptorByFeistelNetwork.Encrypte(dataTrue);
                BitArray cipherTextFalse = BitArrayFuctions.Append(dataFalse.firstPartText, dataFalse.secondPartText);
                BitArray cipherTextTrue = BitArrayFuctions.Append(dataTrue.firstPartText, dataTrue.secondPartText);
                countChangedBitsArray[i] = countChangedBits(cipherTextFalse, cipherTextTrue);
            }

            return countChangedBitsArray;
        }
    }
}
