using PSZI_lr1;
using System.Collections;

namespace PSZI_lr1_v2
{
    public class EncryptByDES
    {
        GeneratorKey generatorKey;
        EncryptorByFeistelNetwork encryptorByFeistelNetwork;
        int originalTextLength = 64;
        int countRounds = 16;

        public EncryptByDES(GeneratorKey generatorKey)
        {
            this.generatorKey = generatorKey;
            encryptorByFeistelNetwork = new EncryptorByFeistelNetwork();
        }

        public int[,] IP = {
            {58, 50, 42, 34, 26, 18, 10, 02 },
            {60, 52, 44, 36, 28, 20, 12, 04 },
            {62, 54, 46, 38, 30, 22, 14, 06 },
            {64, 56, 48, 40, 32, 24, 16, 08 },
            {57, 49, 41, 33, 25, 17, 09, 01 },
            {59, 51, 43, 35, 27, 19, 11, 03 },
            {61, 53, 45, 37, 29, 21, 13, 05 },
            {63, 55, 47, 39, 31, 23, 15, 07 }
        };

        public int[,] IP_1 = {
            {40, 08, 48, 16, 56, 24, 64, 32 },
            {39, 07, 47, 15, 55, 23, 63, 31 },
            {38, 06, 46, 14, 54, 22, 62, 30 },
            {37, 05, 45, 13, 53, 21, 61, 29 },
            {36, 04, 44, 12, 52, 20, 60, 28 },
            {35, 03, 43, 11, 51, 19, 59, 27 },
            {34, 02, 42, 10, 50, 18, 58, 26 },
            {33, 01, 41, 09, 49, 17, 57, 25 }
        };

        public void RearrangementIP(ref BitArray text, int[,] IPtemp)
        {
            BitArray rearrangementText = new BitArray(text);
            int countIPcolumns = IPtemp.GetLength(1);
            for (int i = 0; i < text.Length; i++)
            {
                int IG = i / countIPcolumns;
                int JG = i % countIPcolumns;
                text[i] = rearrangementText[IPtemp[IG, JG] - 1];
            }
        }

        public BitArray[] DivideTextIntoTwoParts(BitArray bitArray)
        {
            // Подготовка данных для раундов
            BitArray firstPartText = new BitArray(bitArray.Length / 2);
            BitArray secondPartText = new BitArray(bitArray.Length / 2);

            for (int i = 0; i < bitArray.Length / 2; i++)
            {
                firstPartText[i] = bitArray[i];
                secondPartText[i] = bitArray[i + bitArray.Length / 2];
            }

            BitArray[] parts = new BitArray[2] { firstPartText, secondPartText };
            return parts;
        }

        public BitArray Encrypte(BitArray originalText)
        {
            // IP
            RearrangementIP(ref originalText, IP);

            BitArray[] parts = DivideTextIntoTwoParts(originalText);
            int i = 0;
            dataToEncryption data = new dataToEncryption(parts[0], parts[1], generatorKey.GenerateKey(i));

            for (;i < countRounds; i++, data.partKey = generatorKey.GenerateKey(i))
            {
                // Сеть Фейсбула
                data = encryptorByFeistelNetwork.Encrypte(data);
            }

            BitArray cipherText = new BitArray(0);
            cipherText.Append(data.secondPartText);
            cipherText.Append(data.firstPartText);
            // IP^-1
            RearrangementIP(ref cipherText, IP_1);
            return cipherText;
        }

        public BitArray Decrypte(BitArray cipherText)
        {
            // IP
            RearrangementIP(ref cipherText, IP);

            // Подготовка данных для раундов
            BitArray[] parts = DivideTextIntoTwoParts(cipherText);


            dataToEncryption data = new dataToEncryption(parts[0], parts[1], generatorKey.GenerateKey(15));

            for (int i = countRounds - 1; i >= 0; i--, data.partKey = generatorKey.GenerateKey(i))
            {
                // Сеть Фейсбула
                data = encryptorByFeistelNetwork.Decrypte(data);
            }


            BitArray originalText = new BitArray(0);
            BitArrayFunctions.Append(originalText, data.secondPartText);
            BitArrayFunctions.Append(originalText, data.firstPartText);
            // IP^-1
            RearrangementIP(ref originalText, IP_1);
            return originalText;
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

        public int[] searchAvalancheEffect(BitArray originalTextTrue, BitArray originalTextFalse, GeneratorKey generatorKeyTrue, GeneratorKey generatorKeyFalse)
        {

            // IP
            RearrangementIP(ref originalTextTrue, IP);
            RearrangementIP(ref originalTextFalse, IP);

            // Подготовка данных для раундов

            BitArray[] partsTrue = DivideTextIntoTwoParts(originalTextTrue);
            BitArray[] partsFalse = DivideTextIntoTwoParts(originalTextFalse);


            int i = 0;
            BitArray partKeyTrue = generatorKeyTrue.GenerateKey(i);
            BitArray partKeyFalse = generatorKeyFalse.GenerateKey(i);

            dataToEncryption dataTrue = new dataToEncryption(partsTrue[0], partsTrue[1], partKeyTrue);
            dataToEncryption dataFalse = new dataToEncryption(partsFalse[0], partsFalse[1], partKeyFalse);

            int[] countChangedBitsArray = new int[16];
            for (; i < 16; i++, dataFalse.partKey = generatorKeyFalse.GenerateKey(i), dataTrue.partKey = generatorKeyTrue.GenerateKey(i))
            {
                dataFalse = encryptorByFeistelNetwork.Encrypte(dataFalse);
                dataTrue = encryptorByFeistelNetwork.Encrypte(dataTrue);
                BitArray cipherTextFalse = BitArrayFunctions.Append(dataFalse.firstPartText, dataFalse.secondPartText);
                BitArray cipherTextTrue = BitArrayFunctions.Append(dataTrue.firstPartText, dataTrue.secondPartText);

                countChangedBitsArray[i] = countChangedBits(cipherTextFalse, cipherTextTrue);
            }

            return countChangedBitsArray;
        }

    }
}