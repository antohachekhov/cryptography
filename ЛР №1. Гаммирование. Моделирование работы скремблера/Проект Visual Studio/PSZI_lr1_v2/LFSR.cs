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

        // Генерация псевдослучайной последовательности
        public byte[] generatePRV(int lengthInBit, int startShiftRegister)
        {
            int shiftRegister = startShiftRegister;
            int value = 0;

            for (int i = 0; i < lengthInBit; i++)
            {
                value = value << 1 | getLowBitAndShift(ref shiftRegister);
            }

            return PSZI_lr1_v2.EncoderClass.IntToByteArray(value);
        }

        public int calcPeriod(int startShiftRegister)
        {
            int shiftRegister = startShiftRegister;

            // Сохраняем последний для нахождения по нему 
            // расстояния между повторяющимися элементами
            int lastShiftRegister = shiftRegister;

            // Создаем коллекцию, хранящую только уникальные значения
            SortedSet<long> set = new SortedSet<long>();


            int setLength = set.Count;
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
            return setLength - ind;
        }

        private int getLowBitAndShift(ref int shiftRegister)
        {
            int lowBit = shiftRegister & 0x1;
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
