using PSZI_lr1_v2;
using System;
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace PSZI_lr1_v2
{
    class GeneratorRandomKey
    {
        public BitArray keyToBitArray;

        // Начальный и конечный символы из ASCII,
        // символы между которыми будут использоваться для генерации ключа
        int[] startEndASCII = { Convert.ToInt32('!'), Convert.ToInt32('~') };

       
        Random rand = new Random();


        public int generateRandomValue(int start, int end)
        {
            int rnd = rand.Next(start, end);
            return rnd;
        }

        public BitArray generateRandomKey(int length)
        {
            length = length / 8;
            byte[] key = new byte[length];

            for (int i = 0; i < length; i++)
                key[i] = (byte)generateRandomValue(startEndASCII[0], startEndASCII[1]);

            return EncoderClass.ByteArrayToBitArray(key);
        }

        // Генерация ключа
        // length - количество битов
        public BitArray GenerateRandomKey(int length)
        {          
            keyToBitArray = generateRandomKey(length);
            return keyToBitArray;
        }
    }
}
