using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSZI_lr1
{
    class LFSR
    {
        int num_polinomial;

        public long generatePRV(int length, long startShiftRegister)
        {
            long shiftRegister = startShiftRegister;
            long value = 0;
            for(int i = 0; i < length; i++)
            {
                value *= 2;
                value += Convert.ToInt32(getLowBitAndShift(ref shiftRegister));
            }

            return value;
        }

        private bool getLowBitAndShift(ref long shiftRegister)
        {
            bool lowBit = Convert.ToBoolean((shiftRegister >> 1) & 0x1);
            if (num_polinomial == 1)
                shiftRegister = ((shiftRegister >> 10 ^ shiftRegister >> 5 ^ shiftRegister >> 4 ^ shiftRegister >> 2 ^ shiftRegister) & 0x1) << 7 | shiftRegister >> 1;
            else
                shiftRegister = ((shiftRegister >> 10 ^ shiftRegister >> 7) & 0x1) << 7 | shiftRegister >> 1;

            return lowBit;
        }


        public LFSR(int num_polinomial)
        {
            this.num_polinomial = num_polinomial;
        }

    }
}
