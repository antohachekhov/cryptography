using PSZI_lr1_v2;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace PSZI_lr1
{
    class GeneratorKey
    {
        public byte[] key;
        public int? startShiftRegister;

        // Начальный и конечный символы из ASCII,
        // символы между которыми будут использоваться для генерации ключа
        int[] startEndASCII = { Convert.ToInt32('!'), Convert.ToInt32('~') };

        // Максимальное число, полученное из 9 бит для скремблеров
        public const int maxBeginValueLFSR = 511;

        Random rand = new Random();


        public int generateRandomValue(int start, int end)
        {
            return rand.Next(start, end);
        }

        public byte[] generateRandomKey(int length)
        {
            byte[] key = new byte[length];

            for (int i = 0; i < length; i++)
                rand.NextBytes(key);

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
                // Проверка входного значения стартового значения сдвигового регистра
                if(this.startShiftRegister == null)
                    startShiftRegister = generateRandomValue(0, maxBeginValueLFSR);
                else if (startShiftRegister > maxBeginValueLFSR)
                        throw new Exception("Стартовое значение генератора должно занимать максимум 10 бит");



                LFSR lfsr = new LFSR((int)command);
                key = lfsr.generatePRV(EncoderClass.StringtoBin(originalText).Length, (int)startShiftRegister);
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
            this.startShiftRegister = null;
        }

        public GeneratorKey(string startShiftRegister)
        {
            this.startShiftRegister = EncoderClass.ByteArrayToInt(EncoderClass.StringToByteArray(startShiftRegister));
        }
    }
}
