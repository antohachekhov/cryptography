using PSZI_lr1_v2;
using System;
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace PSZI_lr1
{
    class GeneratorKey
    {
        public BitArray keyToBitArray;
        public BitArray startShiftRegister;

        // Начальный и конечный символы из ASCII,
        // символы между которыми будут использоваться для генерации ключа
        int[] startEndASCII = { Convert.ToInt32('!'), Convert.ToInt32('~') };

        // Максимальное число, полученное из 10 бит для скремблеров
        public const int maxBeginValueLFSR = 1023;

        Random rand = new Random();


        public int generateRandomValue(int start, int end)
        {
            int rnd = rand.Next(start, end);
            return rnd;
        }

        public BitArray generateRandomKey(int length)
        {
            byte[] key = new byte[length];

            for (int i = 0; i < length; i++)
                key[i] = (byte)generateRandomValue(startEndASCII[0], startEndASCII[1]);


            Console.WriteLine(String.Join(",", key));
            return EncoderClass.ByteArrayToBitArray(key);
        }



        // Генерация ключа
        public BitArray GenerateKey(ModeGenKey command, BitArray originalTextToBitArray)
        {
            Console.WriteLine("Генерируем ключ...");

            if (command == ModeGenKey.random)
                keyToBitArray = generateRandomKey(EncoderClass.BitArrayToByteArray(originalTextToBitArray).Length);
            else if (command <= ModeGenKey.LFSR2)
            {
                // Проверка входного значения стартового значения сдвигового регистра
                if (startShiftRegister.Length > 10)
                    throw new Exception("Стартовое значение генератора должно занимать максимум 10 бит");

                
                long startShiftRegisterToLong = EncoderClass.ByteArrayToLong(EncoderClass.BitArrayToByteArray(startShiftRegister));
                Console.WriteLine("Стартовое значение: " + startShiftRegisterToLong);
                LFSR lfsr = new LFSR((int)command);
                keyToBitArray = lfsr.generatePRV(originalTextToBitArray.Length, startShiftRegisterToLong);
            }
            else
            {
                throw new Exception("Нет такой команды генерации кода");
            }

            Console.WriteLine("Сгенерированный ключ: " + String.Join(",", keyToBitArray));
            return keyToBitArray;
        }

        public GeneratorKey()
        {
            this.startShiftRegister = new BitArray(10);

            // Заполнение начального значения сдвигового регистра
            // случайным образом
            for (int i = 0; i < 10; i++)
                this.startShiftRegister.Set(i, generateRandomValue(0, 1) == 1 ? true : false);
        }

        public GeneratorKey(BitArray startShiftRegister)
        {
            this.startShiftRegister = new BitArray(startShiftRegister);
        }
    }
}
