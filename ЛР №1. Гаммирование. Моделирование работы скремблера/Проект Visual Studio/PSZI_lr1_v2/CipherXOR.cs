
using PSZI_lr1_v2;
using System;
using System.Collections;

namespace PSZI_lr1
{

    class CipherXOR
    {
        internal static BitArray encryptText(BitArray originalText, BitArray key)
        {
            BitArray cipherText = new BitArray(originalText);
            Console.WriteLine(EncoderClass.BitArrayToString(originalText));
            Console.WriteLine(EncoderClass.BitArrayToString(key));
            Console.WriteLine(EncoderClass.BitArrayToString(cipherText));
            return cipherText.Xor(key);
        }
    }
}
