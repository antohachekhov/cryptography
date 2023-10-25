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
    }

    public class Program
    {
        public BitArray originalText;
        public BitArray key1, key2, key3;
        public BitArray key;
        public BitArray vi;
        public BitArray cipherText;
        public BitArray InitializationVector;
        public GeneratorKey generatorKey;
        public GeneratorKey[] generatorKeys;
        public PCBC encryptorByPCBC;
        public int lengthBlock = 64;
        public BitArray[] belowKeys;
        public long timeOfEncoding;

        // Чтение текста из файла
        public void ReadOriginalText(string filename)
        {
            Console.WriteLine("Читаем текст из файла...");
            originalText = EncoderClass.StringToBitArray(readFromFile(filename));
            Console.WriteLine("Текст = " + "\'" + originalText + "\'");
        }

        public BitArray ReadKey(string filename)
        {
            Console.WriteLine("Читаем ключ из файла...");
            BitArray key = EncoderClass.StringToBitArray(readFromFile(filename));
            Console.WriteLine("Ключ = " + "\'" + key + "\'");
            return key;
        }

        public void ReadVI(string filename)
        {
            Console.WriteLine("Читаем вектор инициализации из файла...");
            vi = EncoderClass.StringToBitArray(readFromFile(filename));
            Console.WriteLine("Вектор инициализации = " + "\'" + vi + "\'");
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
            encryptorByPCBC = new PCBC(generatorKeys, InitializationVector);
        }

        //public void FillKeys()
        //{
        //    generatorKey = new GeneratorKey(key);

        //    belowKeys = new BitArray[16];

        //    for (int i = 0; i < 16; i++)
        //        belowKeys[i] = generatorKey.GenerateKey(i);
        //}

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

        public BitArray GetPadding(int length)
        {
            return new BitArray(length);
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

        //public int[] searchAvalancheEffect(BitArray X, int index, ModeChooseAvalanche chooseAvalanche)
        //{
        //    encryptorByDES = new EncryptByDES(generatorKey);

        //    BitArray originalTextFalse = new BitArray(X);
        //    BitArray originalTextTrue = new BitArray(X);

        //    BitArray keyFalse = new BitArray(key);
        //    BitArray keyTrue = new BitArray(key);
        //    if (chooseAvalanche == ModeChooseAvalanche.originalText)
        //    {
        //        originalTextFalse.Set(index, false);
        //        originalTextTrue.Set(index, true);
        //    }
        //    else
        //    {
        //        keyFalse.Set(index, false);
        //        keyTrue.Set(index, true);
        //    }

        //    GeneratorKey generatorFalse = new GeneratorKey(keyFalse);
        //    GeneratorKey generatorTrue = new GeneratorKey(keyTrue);


        //    return encryptorByDES.searchAvalancheEffect(originalTextTrue, originalTextFalse, generatorTrue, generatorFalse);
        //}

        public void Decryption()
        {
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
