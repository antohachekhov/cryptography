
using System;

namespace PSZI_lr1
{

    class CipherXOR
    {

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
