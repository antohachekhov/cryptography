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

        public string generatePRV(int lengthInBit, long startShiftRegister)
        {
            long shiftRegister = startShiftRegister;
            long value = 0;

            for (int i = 0; i < lengthInBit; i++)
            {
                value = value << 1 | getLowBitAndShift(ref shiftRegister);
            }
            Console.WriteLine("Int:" + value);
            string b = "";
            //while (value > 0)
            //{
                //char charValue = Convert.ToChar(value & 0xFF);
                //Console.WriteLine("charValue:" + charValue + " = " + (value & 0xFF));
                //b = charValue + b;
                //value >>= 7;
                
            //}
            byte[] ii = BitConverter.GetBytes(value);
            b = Encoding.ASCII.GetString(ii);
            Console.WriteLine("ii = " + ii);

            return b;
        }

        public int calcPeriod(long startShiftRegister)
        {
            long shiftRegister = startShiftRegister;
            HashSet<long> set = new HashSet<long>();
            int setLength = set.Count;
            long lastShiftRegister = shiftRegister;
            set.Add(shiftRegister);
            while (setLength != set.Count)
            {
                setLength = set.Count;
                getLowBitAndShift(ref shiftRegister);
                lastShiftRegister = shiftRegister;
                if (!set.Contains(shiftRegister))
                    set.Add(shiftRegister);
            }

            int ind = Array.IndexOf(set.ToArray(), lastShiftRegister);
            Console.WriteLine(ind);
            return setLength - ind;
        }

        private long getLowBitAndShift(ref long shiftRegister)
        {
            long lowBit = shiftRegister & 0x1;
            if (num_polinomial == 1)
                shiftRegister = ((shiftRegister >> 10 ^ shiftRegister >> 5 ^ shiftRegister >> 4 ^ shiftRegister >> 2 ^ shiftRegister) & 0x1) << 9 | shiftRegister >> 1;
            else
                shiftRegister = ((shiftRegister >> 10 ^ shiftRegister >> 7) & 0x1) << 9 | shiftRegister >> 1;

            return lowBit;
        }


        public LFSR(int num_polinomial)
        {

            this.num_polinomial = num_polinomial;
        }

    }
}
