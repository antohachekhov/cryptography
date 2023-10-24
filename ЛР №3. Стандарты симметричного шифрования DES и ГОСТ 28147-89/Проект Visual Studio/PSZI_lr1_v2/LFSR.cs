using PSZI_lr1_v2;
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
            Console.WriteLine("Сгенерированная последовательность в двоичке: " + EncoderClass.BitArrayToBinString(bitArray));
            return bitArray;
        }

        private bool getLowBitAndShift(ref long shiftRegister)
        {
            bool lowBit = (shiftRegister & 0x1) == 1 ? true : false;
            if (forKey)
                shiftRegister = ((shiftRegister >> 1 ^ shiftRegister) & 0x1) << 7 | shiftRegister >> 1;
            else
                shiftRegister = ((shiftRegister >> 14 ^ shiftRegister >> 1 ^ shiftRegister) & 0x1) << 15 | shiftRegister >> 1;

            //Console.WriteLine("shiftRegister = " + shiftRegister);
            return lowBit;
        }


        public LFSR(bool forKey)
        {
            this.forKey = forKey;
        }

    }
}
