
using System;

namespace PSZI_lr1
{

    class CipherXOR
    {
        const int numASCIIsymbols = 127;

        public static string generateKey(int length)
        {
            // 33-126 включительно
            var randChar = new Random();

            string key = "";

            for (int i = 0; i < length; i++)
            {
                key += Convert.ToChar(randChar.Next(0, numASCIIsymbols));
            }

            return key;

        }

        public static string encryptText(string text, string key)
        {
           string ciphertext = ""; 

           for (int i = 0; i < text.Length; i++)
            {
                ciphertext += Convert.ToChar(text[i]  ^  key[i]);
            }
            return ciphertext;
        }


    }
}
