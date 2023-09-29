using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PSZI_lr1
{
    class LFSR
    {
        int num_polinomial;

        // Генерация псевдослучайной последовательности
        public BitArray generatePRV(int lengthInBit, long startShiftRegister)
        {
            BitArray bitArray = new BitArray(lengthInBit);
            long shiftRegister = startShiftRegister;

            for (int i = 0; i < lengthInBit; i++)
            {
                bitArray[i] = getLowBitAndShift(ref shiftRegister);
            }
            Console.WriteLine("Сгенерированный ключ в двоичке: " + String.Join(", ", bitArray));
            return bitArray;
        }

        public int calcPeriod(long startShiftRegister)
        {
            long shiftRegister = startShiftRegister;

            // Сохраняем последний для нахождения по нему 
            // расстояния между повторяющимися элементами
            long lastShiftRegister = shiftRegister;

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
            Console.WriteLine("set count = " + set.Count);
            return setLength - ind;
        }

        private bool getLowBitAndShift(ref long shiftRegister)
        {
            bool lowBit = (shiftRegister & 0x1) == 1 ? true : false;
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
