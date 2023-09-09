using PSZI_lr1_v2;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace PSZI_lr1
{
    class GeneratorKey
    {
        public string key;
        public string startShiftRegister;

        // Начальный и конечный символы из ASCII,
        // символы между которыми будут использоваться для генерации ключа
        int[] startEndASCII = { Convert.ToInt32('!'), Convert.ToInt32('~') };

        // Максимальное число, полученное из 9 бит для скремблеров
        const int maxBeginValueLFSR = 511;

        Random rand = new Random();


        public int generateRandomValue(int start, int end)
        {
            return rand.Next(start, end);
        }

        public string generateRandomKey(int length)
        {
            string key = "";

            for (int i = 0; i < length; i++)
            {
                key += Convert.ToChar(generateRandomValue(startEndASCII[0], startEndASCII[1]));
            }

            return key;
        }


        // Генерация ключа
        public void GenerateKey(ModeGenKey command, int length)
        {
            Console.WriteLine("Генерируем ключ...");
            long startShiftRegisterLong = generateRandomValue(0, maxBeginValueLFSR);
            if (command == ModeGenKey.random)
                key = generateRandomKey(length);
            else if (command <= ModeGenKey.LFSR2)
            {
                LFSR lfsr = new LFSR((int)command);
                key = lfsr.generatePRV(length, startShiftRegisterLong);
                startShiftRegister = Convert.ToString(startShiftRegisterLong);
            }
            else
            {
                throw new Exception("Нет такой команды генерации кода");
            }
            
            Console.WriteLine("Ключ = " + "\'" + key + "\'");
            Console.WriteLine("Начальное значение скремблера = " + "\'" + startShiftRegister + "\'");
        }
    }
}
