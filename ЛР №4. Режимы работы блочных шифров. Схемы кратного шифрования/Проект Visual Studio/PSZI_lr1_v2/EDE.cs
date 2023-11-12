using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSZI_lr1_v2
{
    public class EDE
    {
        public static BitArray Encrypte(BitArray inputText, GeneratorKey key1, GeneratorKey key2, GeneratorKey key3)
        {
            EncryptByDES des1 = new EncryptByDES(key1);
            EncryptByDES des2 = new EncryptByDES(key2);
            EncryptByDES des3 = new EncryptByDES(key3);

            BitArray tempResult = des1.Encrypte(inputText);
            tempResult = des2.Decrypte(tempResult);
            tempResult = des3.Encrypte(tempResult);
            return tempResult;
        }

        public static BitArray Decrypte(BitArray inputCipherText, GeneratorKey key1, GeneratorKey key2, GeneratorKey key3)
        {
            EncryptByDES des1 = new EncryptByDES(key3);
            EncryptByDES des2 = new EncryptByDES(key2);
            EncryptByDES des3 = new EncryptByDES(key1);

            BitArray tempResult = des1.Decrypte(inputCipherText);
            tempResult = des2.Encrypte(tempResult);
            tempResult = des3.Decrypte(tempResult);
            return tempResult;
        }
    }
}
