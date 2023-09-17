using PSZI_lr1_v2;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace PSZI_lr1
{
    class GeneratorKey
    {
        public byte[] key;
        public string startShiftRegister;

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

        public byte[] generateRandomKey(int length)
        {
            byte[] key = new byte[length];

            for (int i = 0; i < length; i++)
                key[i] = (byte)generateRandomValue(startEndASCII[0], startEndASCII[1]);


            Console.WriteLine(String.Join(",", key));
            return key;
        }



        // Генерация ключа
        public byte[] GenerateKey(ModeGenKey command, string originalText)
        {
            Console.WriteLine("Генерируем ключ...");

            if (command == ModeGenKey.random)
                key = generateRandomKey(originalText.Length);
            else if (command <= ModeGenKey.LFSR2)
            {
                uint startShiftRegisterToInt;
                // Проверка входного значения стартового значения сдвигового регистра
                if (this.startShiftRegister == "")
                {
                    startShiftRegisterToInt = (uint)generateRandomValue(0, maxBeginValueLFSR);
                    this.startShiftRegister = EncoderClass.ByteArrayToString(EncoderClass.UintToByteArray(startShiftRegisterToInt));
                }
                else
                {
                    startShiftRegisterToInt = EncoderClass.ByteArrayToUint(EncoderClass.StringToByteArray(this.startShiftRegister));
                }

                if (startShiftRegisterToInt > maxBeginValueLFSR)
                    throw new Exception("Стартовое значение генератора должно занимать максимум 10 бит");
                Console.WriteLine("Стартовое значение: " + startShiftRegisterToInt);
                LFSR lfsr = new LFSR((int)command);
                key = lfsr.generatePRV(EncoderClass.StringtoBin(originalText, originalText.Length).Length, startShiftRegisterToInt);
            }
            else
            {
                throw new Exception("Нет такой команды генерации кода");
            }

            Console.WriteLine("Сгенерированный ключ: " + String.Join(",", key));
            return key;
        }

        public GeneratorKey()
        {
            this.startShiftRegister = "";
        }

        public GeneratorKey(string startShiftRegister)
        {
            this.startShiftRegister = startShiftRegister;
        }
    }
}
