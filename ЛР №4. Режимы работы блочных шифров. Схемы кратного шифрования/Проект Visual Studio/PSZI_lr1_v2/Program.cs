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
using System.Diagnostics;
using System.Security.Cryptography;


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

    class Program
    {
        public BitArray originalText;
        public BitArray key;
        public BitArray vi;
        public BitArray cipherText;
        public GeneratorKey generatorKey;
        public EncryptByDES encryptorByDES;
        public int countRounds;
        public int lengthBlock = 64;
        public BitArray[] belowKeys;
        public long timeOfEncoding;

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

        public void ReadVI(string filename)
        {
            Console.WriteLine("Читаем вектор инициализации из файла...");
            vi = EncoderClass.StringToBitArray(readFromFile(filename));
            Console.WriteLine("Вектор инициализации = " + "\'" + vi + "\'");
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

        public void FillKeys()
        {
            generatorKey = new GeneratorKey(key);

            belowKeys = new BitArray[16];

            for (int i = 0; i < 16; i++)
                belowKeys[i] = generatorKey.GenerateKey(i);
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
            encryptorByDES = new EncryptByDES(generatorKey);

            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();

            BitArray[] originalTextBlocks = DividingTextIntoBlocks(originalText);
            BitArray cipherText = new BitArray(0);

            // Шифруем каждый блок
            for (int iBlock = 0; iBlock < originalTextBlocks.Length; iBlock++)
            {
                cipherText = BitArrayFunctions.Append(cipherText, encryptorByDES.Encrypte(originalTextBlocks[iBlock]));
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

        // Матрица расстояний c входным Зашифрованным текстом
        public int[,] matrixDistances(BitArray X, BitArray key, BitArray Y)
        {
            // Создадим класс EncryptorByFeistelNetwork для доступа к func
            EncryptByDES encryptByDes2 = new EncryptByDES(new GeneratorKey(key));

            int n = X.Length;
            int m = Y.Length;

            List<BitArray>[,] listY = new List<BitArray>[n, m];

            int[,] MDist = new int[n, m];

            for (int i = 0; i < n; i++)
            {
                // Считаем Xi
                BitArray Xi = new BitArray(X);
                Xi[i] = X[i] != true;

                BitArray Yi = encryptByDes2.Encrypte(Xi); // ПРОБЛЕМА!!! ---------------------------------------------------

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


        // Матрица зависимостей с входным Зашифрованным текстом
        public int[,] matrixDependence(BitArray X, BitArray key, BitArray Y)
        {
            //Console.WriteLine(EncoderClass.BitArrayToBinString(X));

            // Создадим класс EncryptorByFeistelNetwork для доступа к func
            EncryptByDES encryptByDes2 = new EncryptByDES(new GeneratorKey(key));

            int n = X.Length;
            int m = Y.Length;

            // Матрица зависимостей
            int[,] MDep = new int[n, m];

            for (int i = 0; i < n; i++)
            {
                // Считаем Xi
                BitArray Xi = new BitArray(X);
                Xi[i] = (X[i] == true) ? false : true;

                BitArray Yi = encryptByDes2.Encrypte(Xi); // ПРОБЛЕМА!!! ---------------------------------------------------

                for (int j = 0; j < m; j++)
                {
                    if (Yi[j] != Y[j])
                        MDep[i, j] = 1;
                }
            }

            // Console.WriteLine(EncoderClass.BitArrayToBinString(Y));

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

        public int[] searchAvalancheEffect(BitArray X, int index, ModeChooseAvalanche chooseAvalanche)
        {
            encryptorByDES = new EncryptByDES(generatorKey);

            BitArray originalTextFalse = new BitArray(X);
            BitArray originalTextTrue = new BitArray(X);

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
            BitArray[] cipherTextBlocks = DividingTextIntoBlocks(this.originalText);
            BitArray originalText = new BitArray(0);

            // Шифруем каждый блок
            for (int iBlock = 0; iBlock < cipherTextBlocks.Length; iBlock++)
            {
                originalText = BitArrayFunctions.Append(originalText, encryptorByDES.Decrypte(cipherTextBlocks[iBlock]));
            }

            this.cipherText = originalText;
        }
    }
}
