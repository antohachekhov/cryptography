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
        public int lengthBlock = 64;

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


        public BitArray[,] DividingTextIntoBlocks(BitArray text)
        {

            BitArray[,] originalTextBlocks = new BitArray[text.Length / lengthBlock, 2];

            // Разделение текста на блоки по 64 бита
            for (int i = 0; i < text.Length / lengthBlock; i++)
            {
                BitArray bitArray32 = new BitArray(lengthBlock / 2);

                // Подсчет левой части
                for (int j = 0; j < lengthBlock / 2; j++)
                    bitArray32.Set(j, i * lengthBlock + j < text.Length ? text.Get(i * lengthBlock + j) : false);
                originalTextBlocks[i, 0] = new BitArray(bitArray32);

                // Подсчет правой части
                for (int j = lengthBlock / 2; j < lengthBlock; j++)
                    bitArray32.Set(j - lengthBlock / 2, i * lengthBlock + j < text.Length ? text.Get(i * lengthBlock + j) : false);
                originalTextBlocks[i, 1] = new BitArray(bitArray32);

            }

            return originalTextBlocks;
        }

        public void Encryption()
        {

            BitArray[,] originalTextBlocks = DividingTextIntoBlocks(originalText);
            cipherText = new BitArray(0);

            // Шифруем каждый блок
            for (int iBlock = 0; iBlock < originalTextBlocks.GetLength(0); iBlock++)
            {
                int i = 0;
                BitArray partKey = generatorKey.GenerateKey(0);

                dataToEncryption data = new dataToEncryption(originalTextBlocks[iBlock, 0], originalTextBlocks[iBlock, 1], partKey);

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
                cipherText = BitArrayFuctions.Append(cipherText, data.firstPartText);
                cipherText = BitArrayFuctions.Append(cipherText, data.secondPartText);

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


        public int[] searchAvalancheEffect(int index, ModeChooseAvalanche chooseAvalanche, ModeGenKey modeGenKey)
        {
            int[] countChangedBitsArray;
            if (chooseAvalanche == ModeChooseAvalanche.originalText)
            {
                BitArray[,] originalTextBlocksFalse;
                BitArray[,] originalTextBlocksTrue;
                BitArray originalTextFalse = new BitArray(this.originalText);
                BitArray originalTextTrue = new BitArray(this.originalText);
                originalTextFalse.Set(index, false);
                originalTextTrue.Set(index, true);

                originalTextBlocksFalse = DividingTextIntoBlocks(originalTextFalse);
                originalTextBlocksTrue = DividingTextIntoBlocks(originalTextTrue);

                int i = 0;
                BitArray partKey = generatorKey.GenerateKey(i);

                dataToEncryption dataFalse = new dataToEncryption(originalTextBlocksFalse[0,0], originalTextBlocksFalse[0, 1], partKey);
                dataToEncryption dataTrue = new dataToEncryption(originalTextBlocksTrue[0, 0], originalTextBlocksTrue[0, 1], partKey);

                countChangedBitsArray = new int[countRounds];
                for (; i < countRounds; i++, dataFalse.partKey = generatorKey.GenerateKey(i), dataTrue.partKey = generatorKey.GenerateKey(i))
                {
                    dataFalse = encryptorByFeistelNetwork.Encrypte(dataFalse);
                    dataTrue = encryptorByFeistelNetwork.Encrypte(dataTrue);
                    BitArray cipherTextFalse = BitArrayFuctions.Append(dataFalse.firstPartText, dataFalse.secondPartText);
                    BitArray cipherTextTrue = BitArrayFuctions.Append(dataTrue.firstPartText, dataTrue.secondPartText);

                    Console.WriteLine("cipherTextFalse = " + EncoderClass.BitArraytoHexString(cipherTextFalse));
                    Console.WriteLine("cipherTextTrue = " + EncoderClass.BitArraytoHexString(cipherTextTrue));
                    Console.WriteLine("partKey = " + EncoderClass.BitArraytoHexString(dataTrue.partKey));

                    countChangedBitsArray[i] = countChangedBits(cipherTextFalse, cipherTextTrue);
                }
            }
            else
            {
                BitArray[,] originalTextBlocks = DividingTextIntoBlocks(originalText);

                BitArray keyFalse = new BitArray(key);
                BitArray keyTrue = new BitArray(key);
                keyFalse.Set(index, false);
                keyTrue.Set(index, true);


                GeneratorKey generatorFalse = new GeneratorKey(modeGenKey, keyFalse);
                GeneratorKey generatorTrue = new GeneratorKey(modeGenKey, keyTrue);


                int i = 0;
                BitArray partKeyFalse = generatorFalse.GenerateKey(i);
                BitArray partKeyTrue = generatorTrue.GenerateKey(i);

                dataToEncryption dataFalse = new dataToEncryption(originalTextBlocks[0, 0], originalTextBlocks[0, 1], partKeyFalse);
                dataToEncryption dataTrue = new dataToEncryption(originalTextBlocks[0, 0], originalTextBlocks[0, 1], partKeyTrue);

                countChangedBitsArray = new int[countRounds];
                for (; i < countRounds; i++, dataFalse.partKey = generatorFalse.GenerateKey(i), dataTrue.partKey = generatorTrue.GenerateKey(i))
                {
                    dataFalse = encryptorByFeistelNetwork.Encrypte(dataFalse);
                    dataTrue = encryptorByFeistelNetwork.Encrypte(dataTrue);
                    BitArray cipherTextFalse = BitArrayFuctions.Append(dataFalse.firstPartText, dataFalse.secondPartText);
                    BitArray cipherTextTrue = BitArrayFuctions.Append(dataTrue.firstPartText, dataTrue.secondPartText);

                    Console.WriteLine("cipherTextFalse = " + EncoderClass.BitArraytoHexString(cipherTextFalse));
                    Console.WriteLine("cipherTextTrue = " + EncoderClass.BitArraytoHexString(cipherTextTrue));
                    Console.WriteLine("partKey = " + EncoderClass.BitArraytoHexString(dataTrue.partKey));

                    countChangedBitsArray[i] = countChangedBits(cipherTextFalse, cipherTextTrue);
                }
            }

            

            return countChangedBitsArray;
        }

        internal void Decryption()
        {
            BitArray[,] originalTextBlocks = DividingTextIntoBlocks(originalText);
            cipherText = new BitArray(0);

            // Шифруем каждый блок
            for (int iBlock = 0; iBlock < originalTextBlocks.GetLength(0); iBlock++)
            {
                int i = 0;
                BitArray partKey = generatorKey.GenerateKey(0);

                dataToEncryption data = new dataToEncryption(originalTextBlocks[iBlock, 0], originalTextBlocks[iBlock, 1], partKey);

                for (; i < countRounds; i++, data.partKey = generatorKey.GenerateKey(i))
                {
                    Console.WriteLine("Левая часть: " + EncoderClass.BitArraytoHexString(data.firstPartText));
                    Console.WriteLine("Правая часть: " + EncoderClass.BitArraytoHexString(data.secondPartText));
                    Console.WriteLine("Ключ: " + EncoderClass.BitArraytoHexString(data.partKey));

                    data = encryptorByFeistelNetwork.Decrypte(data);
                }

                Console.WriteLine("Левая часть: " + EncoderClass.BitArraytoHexString(data.firstPartText));
                Console.WriteLine("Правая часть: " + EncoderClass.BitArraytoHexString(data.secondPartText));
                Console.WriteLine("Ключ: " + EncoderClass.BitArraytoHexString(data.partKey));
                cipherText = BitArrayFuctions.Append(cipherText, data.firstPartText);
                cipherText = BitArrayFuctions.Append(cipherText, data.secondPartText);

            }
        }
    }
}
