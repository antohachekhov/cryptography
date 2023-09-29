using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PSZI_lr1
{
    class LFSR
    {
        bool forKey;

        // Генерация псевдослучайной последовательности
        public BitArray generatePRV(int lengthInBit, long startShiftRegister)
        {
            BitArray bitArray = new BitArray(lengthInBit);
            long shiftRegister = startShiftRegister;

            for (int i = 0; i < lengthInBit; i++)
            {
                bitArray[i] = getLowBitAndShift(ref shiftRegister);
            }
            Console.WriteLine("Сгенерированная последовательность в двоичке: " + String.Join(", ", bitArray));
            return bitArray;
        }

        private bool getLowBitAndShift(ref long shiftRegister)
        {
            bool lowBit = (shiftRegister & 0x1) == 1 ? true : false;
            shiftRegister = ((shiftRegister >> 1 ^ shiftRegister) & 0x1);
            if (forKey)
                shiftRegister = shiftRegister << 7 | shiftRegister >> 1;
            else
                shiftRegister = shiftRegister << 15 | shiftRegister >> 1;

            return lowBit;
        }


        public LFSR(bool forKey)
        {
            this.forKey = forKey;
        }

    }
}
