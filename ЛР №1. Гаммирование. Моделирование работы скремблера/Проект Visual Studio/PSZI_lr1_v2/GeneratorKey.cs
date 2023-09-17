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
        public const int maxBeginValueLFSR = 511;

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
        public void GenerateKey(ModeGenKey command, string originalText)
        {
            Console.WriteLine("Генерируем ключ...");

            
            if (command == ModeGenKey.random)
                key = generateRandomKey(originalText.Length);
            else if (command <= ModeGenKey.LFSR2)
            {
                long startShiftRegisterLong = 0;
                if (this.startShiftRegister != "")
                {
                    startShiftRegisterLong = Program.toNum(this.startShiftRegister);
                    if (startShiftRegisterLong > maxBeginValueLFSR)
                        throw new Exception("Стартовое значение генератора должно занимать максимум 10 бит");
                }
                else
                {
                    startShiftRegisterLong = generateRandomValue(0, maxBeginValueLFSR);
                }
                LFSR lfsr = new LFSR((int)command);
                key = lfsr.generatePRV(Program.toBin(originalText).Length, startShiftRegisterLong);
                startShiftRegister = Convert.ToString(startShiftRegisterLong);
            }
            else
            {
                throw new Exception("Нет такой команды генерации кода");
            }
            
            Console.WriteLine("Ключ = " + "\'" + key + "\'");
            Console.WriteLine("Начальное значение скремблера = " + "\'" + startShiftRegister + "\'");
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
