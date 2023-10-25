using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSZI_lr1_v2
{
    internal class EDE
    {
        static BitArray encrypt(BitArray inputText, BitArray key1, BitArray key2, BitArray key3)
        {
            EncryptByDES des1 = new EncryptByDES(new GeneratorKey(key1));
            EncryptByDES des2 = new EncryptByDES(new GeneratorKey(key2));
            EncryptByDES des3 = new EncryptByDES(new GeneratorKey(key3));

            BitArray tempResult = des1.Encrypte(inputText);
            tempResult = des2.Decrypte(tempResult);
            tempResult = des3.Encrypte(tempResult);
            return tempResult;
        }
    }
}
