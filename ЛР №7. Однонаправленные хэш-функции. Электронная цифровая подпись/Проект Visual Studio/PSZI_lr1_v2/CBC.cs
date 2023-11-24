using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSZI_lr1_v2
{
    public class CBC
    {
        public GeneratorDESKey generatorKeys;
        public BitArray C0;
        EncryptByDES des;

        public CBC(GeneratorDESKey generatorKeys, BitArray IV)
        {
            this.generatorKeys = generatorKeys;
            des = new EncryptByDES(this.generatorKeys);

            //generatorKeys.CopyTo(this.generatorKeys, 0);
            C0 = new BitArray(IV);
        }

        public BitArray Encrypte(BitArray tempP, BitArray lastC)
        {

            BitArray C_i_1;

            if (lastC != null)
                C_i_1 = lastC;
            else
                C_i_1 = C0;

            BitArray newP = new BitArray(tempP);

            newP.Xor(C_i_1);

            return des.Encrypte(newP); ;
        }

        public BitArray Decrypte(BitArray lastC, BitArray tempC, BitArray lastP)
        {

            BitArray C_i_1;

            if (lastC != null)
                C_i_1 = lastC;
            else
                C_i_1 = C0;

            BitArray newC = des.Decrypte(tempC);

            newC.Xor(C_i_1);

            return newC;
        }
    }
}