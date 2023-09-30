using PSZI_lr1_v2;
using System;
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace PSZI_lr1
{
    class GeneratorKey
    {
        public BitArray generalKey;
        public ModeGenKey command;
        const int partKeyLengthFirst = 32;
        const int startScramblerLength = 8;


        BitArray generateFirstPartKey(int length, int i)
        {
            BitArray newKey = new BitArray(length);
            int j = 0;
            for (; j < this.generalKey.Length - i && j < newKey.Length; j++)
                newKey[newKey.Length - j - 1] = this.generalKey[this.generalKey.Length - j - i - 1];

            for (int k = 0; j < newKey.Length; j++, k++)
                newKey[j] = newKey[k];

            return newKey;
        }

        BitArray generateSecondPartKey(int i)
        {
            BitArray startShiftRegister = generateFirstPartKey(startScramblerLength, i);

            long startShiftRegisterLong = EncoderClass.ByteArrayToLong(EncoderClass.BitArrayToByteArray(startShiftRegister));
            LFSR lfsr = new LFSR(true);
            BitArray newKey = lfsr.generatePRV(partKeyLengthFirst, startShiftRegisterLong);

            return newKey;
        }



        // Генерация ключа
        public BitArray GenerateKey(int i)
        {
            Console.WriteLine("Генерируем ключ...");
            BitArray partKey;
            if (command == ModeGenKey.one)
            // для i-го раунда подключом является цепочка из 32 подряд идущих бит заданного ключа, которая начинается с бита номер,
            // продолжается до последнего бита ключа и при его достижении циклически повторяется, начиная с 1 бита;
            {
                partKey = generateFirstPartKey(partKeyLengthFirst, i);
            }
            else if (command == ModeGenKey.two)
            {
                // для i-го раунда, начиная с бита номер i, берётся цепочка из 8 подряд идущих бит ключа,
                // которая является начальным значением для скремблера вида;
                // подключом является сгенерированная этим скремблером последовательность из 32 бит;

                partKey = generateSecondPartKey(i);
            }
            else
            {
                throw new Exception("Нет такой команды генерации кода");
            }

            Console.WriteLine("Сгенерированный ключ: " + String.Join(",", partKey));
            return partKey;
        }

        public GeneratorKey(ModeGenKey command, BitArray generalKey)
        {
            this.command = command;
            this.generalKey = generalKey;
        }


    }
}
