using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;


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
                bitArray = BitArrayFunctions.Append(bitArray, bitArrayInByte);
            }
            return bitArray;
        }

        public static int CountXor1(BitArray first, BitArray second)
        {
            BitArray xor = new BitArray(first);
            xor.Xor(second);

            int count = 0;
            for(int i =0; i < xor.Length; i++)
            {
                if (xor[i] == true)
                    count++;
            }

            return count;
        }
    }

    public class Program
    {
        public BitArray originalText;
        public BitArray key1, key2, key3;
        public BitArray iv;
        public BitArray cipherText;
        public GeneratorKey generatorKey;
        public GeneratorKey[] generatorKeys;
        public PCBC encryptorByPCBC;
        public int lengthBlock = 64;
        public int lengthKey = 56;
        public BitArray[] belowKeys;
        public long timeOfEncoding;
        public ModeChoosePadding modePadding;

        public BitArray GenerateRandomBitArray(int length)
        {
            return (new GeneratorRandomKey()).generateRandomKey(length);
        }

        // Определение обьекта, который будет генерировать ключи
        public void GenerateKey()
        {
            GeneratorKey generatorKey1 = new GeneratorKey(key1);
            GeneratorKey generatorKey2 = new GeneratorKey(key2);
            GeneratorKey generatorKey3 = new GeneratorKey(key3);

            generatorKeys = new GeneratorKey[3] { generatorKey1, generatorKey2, generatorKey3 };
        }


        // Определение обьекта, который будет шифровать
        public void GenerateEncryptor()
        {
            encryptorByPCBC = new PCBC(generatorKeys, iv);
        }

        public static string ReadFromFile(string fileName)
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

        public BitArray GetPadding(int length)
        {
            if (modePadding == ModeChoosePadding.zeros)
                return new BitArray(length);
            else if (modePadding == ModeChoosePadding.ones)
                return GenerateTrueBitArray(length);
            else
                return GenerateRandomBitArray(length);
        }

        private BitArray GenerateTrueBitArray(int length)
        {
            BitArray bitArray = EncoderClass.IntToBitArrayLength4(length / 8);

            BitArray newPadding = new BitArray(length);

            for(int i = 0; i < newPadding.Length;  )
            {
                for (int j = 0; j < bitArray.Length && i < newPadding.Length; j++, i++)
                {
                    newPadding[i] = bitArray[j];
                }
            }

            return newPadding;

        }

        public BitArray[] DividingTextIntoBlocks(BitArray text)
        {
            int countBlocks = text.Length / lengthBlock;

            if (text.Length % lengthBlock != 0)
                countBlocks++;

            BitArray[] originalTextBlocks = new BitArray[countBlocks];

            // Разделение текста на блоки по 64 бита
            for (int i = 0; i < text.Length / lengthBlock; i++)
            {
                BitArray bitArray64 = new BitArray(lengthBlock);

                // Подсчет левой части
                for (int j = 0; j < lengthBlock; j++)
                    bitArray64[j] = text[i * lengthBlock + j];

                originalTextBlocks[i] = new BitArray(bitArray64);
            }

            // Добавление padding
            if(text.Length % lengthBlock != 0)
            {
                int lengthMiniBlock = text.Length % lengthBlock;
                BitArray bitArray64 = new BitArray(lengthMiniBlock);
                for (int j = 0; j < lengthMiniBlock; j++)
                    bitArray64[j] = text[text.Length - lengthMiniBlock + j];

                bitArray64 = BitArrayFunctions.Append(bitArray64, GetPadding(lengthBlock - lengthMiniBlock));
                originalTextBlocks[countBlocks - 1] = new BitArray(bitArray64);
            }

            return originalTextBlocks;
        }

        public void Encryption()
        {
            GenerateEncryptor();
            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();

            BitArray[] originalTextBlocks = DividingTextIntoBlocks(originalText);
            BitArray cipherText = new BitArray(0);
            BitArray cipherTextLastBlock = encryptorByPCBC.Encrypte(null, originalTextBlocks[0], null);

            // Шифруем каждый блок
            cipherText = BitArrayFunctions.Append(cipherText, cipherTextLastBlock);

            for (int iBlock = 1; iBlock < originalTextBlocks.Length; iBlock++)
            {
                cipherTextLastBlock = encryptorByPCBC.Encrypte(originalTextBlocks[iBlock - 1], originalTextBlocks[iBlock], cipherTextLastBlock);
                cipherText = BitArrayFunctions.Append(cipherText, cipherTextLastBlock);
            }

            //останавливаем счётчик
            stopwatch.Stop();
            timeOfEncoding = stopwatch.ElapsedMilliseconds;

            this.cipherText = cipherText;
        }

        // Количество ненулевых элементов в множестве
        public int Hemming(BitArray x)
        {
            int count = 0;
            for (int i = 0; i < x.Length; i++) if (x[i] == true) count++;
            return count;
        }

        // Матрица расстояний
        public int[,] matrixDistances(BitArray X, BitArray key)
        {
            // Создадим класс EncryptorByFeistelNetwork для доступа к func
            EncryptByDES encryptByDes2 = new EncryptByDES(new GeneratorKey(key));
            BitArray Y = encryptByDes2.Encrypte(X);

            int n = X.Length;
            int m = Y.Length;

            List<BitArray>[,] listY = new List<BitArray>[n, m];

            int[,] MDist = new int[n, m];

            for(int i = 0; i < n; i++)
            {
                // Считаем Xi
                BitArray Xi = new BitArray(X);
                Xi[i] = X[i] != true;

                BitArray Yi = encryptByDes2.Encrypte(Xi);

                for (int j = 0; j < m; j++)
                {
                    BitArray YiXorY = new BitArray(Yi);
                    YiXorY.Xor(Y);
                    if (Hemming(YiXorY) == j + 1)
                        MDist[i, j] = 1;
                }
            }

            return MDist;
        }


        // Матрица зависимостей
        public int[,] matrixDependence(BitArray X, BitArray key)
        {
            Console.WriteLine(EncoderClass.BitArrayToBinString(X));
            // Создадим класс EncryptorByFeistelNetwork для доступа к func
            EncryptByDES encryptByDes2 = new EncryptByDES(new GeneratorKey(key));
            BitArray Y = encryptByDes2.Encrypte(X);

            int n = X.Length;
            int m = Y.Length;

            // Матрица зависимостей
            int[,] MDep = new int[n, m];

            for (int i = 0; i < n; i++)
            {
                // Считаем Xi
                BitArray Xi = new BitArray(X);
                Xi[i] = (X[i] == true) ? false : true;

                BitArray Yi = encryptByDes2.Encrypte(Xi);

                for (int j = 0; j < m; j++)
                {
                    if (Yi[j] != Y[j])
                        MDep[i, j] = 1;
                }
            }

            Console.WriteLine(EncoderClass.BitArrayToBinString(Y));

            return MDep;
        }

        public double criteria1(int[,] MDist)
        {
            double result = 0.0;
            int n = MDist.GetLength(0);
            int m = MDist.GetLength(1);
            int sizeU = DividingTextIntoBlocks(originalText).Length;
            for(int i = 0; i < n; i++)
            {
                double result2 = 0.0;
                for(int j = 0; j < m; j++)
                {
                    result2 += (j + 1) * MDist[i, j];
                }
                result += result2 / sizeU;
            }
            result /= n;
            Console.WriteLine("d1 = " + result.ToString());
            return result;
        }

        public double criteria2(int[,] MDep)
        {
            double d = 0.0;
            int n = MDep.GetLength(0);
            int m = MDep.GetLength(1);

            double k = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (MDep[i, j] == 0) k++;
                }
            }

            d = 1 - (k / (n * m));

            Console.WriteLine("d2 = " + d.ToString());

            return d;
        }

        public double criteria3(int[,] MDist)
        {
            double result = 0.0;
            int n = MDist.GetLength(0);
            int m = MDist.GetLength(1);
            int sizeU = DividingTextIntoBlocks(originalText).Length;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    result += 2 * (j + 1) * MDist[i, j] - m;
                }

                result = Math.Abs(result);
            }
            result *= sizeU / (n * m);
            result = 1.0 - result;

            Console.WriteLine("d3 = " + result.ToString());
            return result;
        }

        public double criteria4(int[,] MDep)
        {
            double d = 0.0;
            int n = MDep.GetLength(0);
            int m = MDep.GetLength(1);

            double k = 0;
            int sizeU = DividingTextIntoBlocks(originalText).Length;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    k += (2 * MDep[i,j] / sizeU) - 1.0;
                }

                k = Math.Abs(k);
            }

            d = 1 - k / (n * m);

            Console.WriteLine("d4 = " + d.ToString());

            return d;
        }

        public struct TextWithAvalanche
        {
            public BitArray textTrue;
            public BitArray textFalse;
            public int[] avalancheEffect;
        }

        /// <summary>
        /// <returns>Массив, в котором хранится количество изменившихся битов для каждого блока открытого/зашифрованного текста</returns>
        /// </summary>
        public int[] searchAvalancheEffectForPCBC(BitArray textTrue, BitArray textFalse,
                                                    BitArray keyTrue, BitArray keyFalse,
                                                    BitArray ivTrue, BitArray ivFalse,
                                                    bool isEncrypteOrDecrypte)
        {
            GeneratorKey generatorFalse = new GeneratorKey(keyFalse);
            GeneratorKey generatorTrue = new GeneratorKey(keyTrue);

            PCBC PCBCTrue = new PCBC(new GeneratorKey[3]{ generatorTrue, generatorTrue, generatorTrue}, ivTrue);
            PCBC PCBCFalse = new PCBC(new GeneratorKey[3] { generatorFalse, generatorFalse, generatorFalse }, ivFalse);

            BitArray[] textBlocksTrue = DividingTextIntoBlocks(textTrue);
            BitArray[] textBlocksFalse = DividingTextIntoBlocks(textFalse);

            int[] avalanchesEffects = new int[textBlocksTrue.Length];

            if (isEncrypteOrDecrypte)
            {
                BitArray cipherTextLastBlockTrue = PCBCTrue.Encrypte(null, textBlocksTrue[0], null);
                BitArray cipherTextLastBlockFalse = PCBCFalse.Encrypte(null, textBlocksFalse[0], null);

                avalanchesEffects[0] = BitArrayFunctions.CountXor1(cipherTextLastBlockTrue, cipherTextLastBlockFalse);

                for (int iBlock = 1; iBlock < textBlocksTrue.Length; iBlock++)
                {
                    cipherTextLastBlockTrue = PCBCTrue.Encrypte(textBlocksTrue[iBlock - 1], textBlocksTrue[iBlock], cipherTextLastBlockTrue);
                    cipherTextLastBlockFalse = PCBCFalse.Encrypte(textBlocksFalse[iBlock - 1], textBlocksFalse[iBlock], cipherTextLastBlockFalse);

                    avalanchesEffects[iBlock] = BitArrayFunctions.CountXor1(cipherTextLastBlockTrue, cipherTextLastBlockFalse);
                }
            }
            else
            {
                BitArray originalTextLastBlockTrue = PCBCTrue.Decrypte(null, textBlocksTrue[0], null);
                BitArray originalTextLastBlockFalse = PCBCFalse.Decrypte(null, textBlocksFalse[0], null);

                avalanchesEffects[0] = BitArrayFunctions.CountXor1(originalTextLastBlockTrue, originalTextLastBlockFalse);
                
                for (int iBlock = 1; iBlock < textBlocksTrue.Length; iBlock++)
                {
                    originalTextLastBlockTrue = PCBCTrue.Decrypte(textBlocksTrue[iBlock - 1], textBlocksTrue[iBlock], originalTextLastBlockTrue);
                    originalTextLastBlockFalse = PCBCFalse.Decrypte(textBlocksFalse[iBlock - 1], textBlocksFalse[iBlock], originalTextLastBlockFalse);

                    avalanchesEffects[iBlock] = BitArrayFunctions.CountXor1(originalTextLastBlockTrue, originalTextLastBlockFalse);
                }

            }


            return avalanchesEffects;
        }

        /// <summary>
        /// <returns>Массив, в котором хранится количество изменившихся битов для каждого блока открытого/зашифрованного текста</returns>
        /// </summary>
        public int[] searchAvalancheEffectForEDE(BitArray textTrue, BitArray textFalse,
                                                    BitArray key1True, BitArray key1False,
                                                    BitArray key2, 
                                                    BitArray key3,
                                                    bool isEncrypteOrDecrypte)
        {
            GeneratorKey generatorKey1False = new GeneratorKey(key1False);
            GeneratorKey generatorKey1True = new GeneratorKey(key1True);
            GeneratorKey generatorKey2 = new GeneratorKey(key2);
            GeneratorKey generatorKey3 = new GeneratorKey(key3);

            BitArray[] textBlocksTrue = DividingTextIntoBlocks(textTrue);
            BitArray[] textBlocksFalse = DividingTextIntoBlocks(textFalse);

            int[] avalanchesEffects = new int[textBlocksTrue.Length];

            if (isEncrypteOrDecrypte)
            {
                for (int iBlock = 0; iBlock < textBlocksTrue.Length; iBlock++)
                {
                    BitArray cipherTextLastBlockTrue = EDE.Encrypte(textBlocksTrue[iBlock], generatorKey1True, generatorKey2, generatorKey3);
                    BitArray cipherTextLastBlockFalse = EDE.Encrypte(textBlocksFalse[iBlock], generatorKey1False, generatorKey2, generatorKey3);

                    avalanchesEffects[iBlock] = BitArrayFunctions.CountXor1(cipherTextLastBlockTrue, cipherTextLastBlockFalse);
                }
            }
            else
            {
                for (int iBlock = 0; iBlock < textBlocksTrue.Length; iBlock++)
                {
                    BitArray cipherTextLastBlockTrue = EDE.Decrypte(textBlocksTrue[iBlock], generatorKey1True, generatorKey2, generatorKey3);
                    BitArray cipherTextLastBlockFalse = EDE.Decrypte(textBlocksFalse[iBlock], generatorKey1False, generatorKey2, generatorKey3);

                    avalanchesEffects[iBlock] = BitArrayFunctions.CountXor1(cipherTextLastBlockTrue, cipherTextLastBlockFalse);
                }

            }


            return avalanchesEffects;
        }

        public void Decryption()
        {
            GenerateEncryptor();
            BitArray[] cipherTextBlocks = DividingTextIntoBlocks(this.originalText);
            BitArray originalText = new BitArray(0);
            BitArray originalTextLastBlock = encryptorByPCBC.Decrypte(null, cipherTextBlocks[0], null);
            originalText = BitArrayFunctions.Append(originalText, originalTextLastBlock);
            // Шифруем каждый блок
            for (int iBlock = 1; iBlock < cipherTextBlocks.Length; iBlock++)
            {
                originalTextLastBlock = encryptorByPCBC.Decrypte(cipherTextBlocks[iBlock - 1], cipherTextBlocks[iBlock], originalTextLastBlock);
                originalText = BitArrayFunctions.Append(originalText, originalTextLastBlock);
            }

            this.cipherText = originalText;
        }

    }
}
