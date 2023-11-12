using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSZI_lr1_v2
{
    public static class GeneratorRandomKey
    {
        static Random random = new Random();
        static int[] startEndASCII = { Convert.ToInt32('!'), Convert.ToInt32('~') };

        public static int generateRandomValue(int start, int end)
        {
            int rnd = random.Next(start, end);
            return rnd;
        }

        public static BitArray generateRandomKey(int length)
        {
            byte[] key = new byte[length];

            for (int i = 0; i < length; i++)
                key[i] = (byte)generateRandomValue(startEndASCII[0], startEndASCII[1]);

            return EncoderClass.ByteArrayToBitArray(key);
        }

    }
}
