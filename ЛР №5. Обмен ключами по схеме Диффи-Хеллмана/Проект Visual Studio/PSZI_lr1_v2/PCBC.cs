using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PSZI_lr1_v2.Program;

namespace PSZI_lr1_v2
{
    public class PCBC
    {
        public GeneratorKey[] generatorKeys;
        public BitArray C0;

        public PCBC(GeneratorKey[] generatorKeys, BitArray IV)
        {
            this.generatorKeys = new GeneratorKey[3];


            generatorKeys.CopyTo(this.generatorKeys, 0);
            C0 = new BitArray(IV);
        }

        public BitArray Encrypte(BitArray lastP, BitArray tempP, BitArray lastC)
        {

            BitArray C_i_1;

            if (lastC != null)
                C_i_1 = lastC;
            else
                C_i_1 = C0;

            BitArray newP = new BitArray(tempP);

            newP.Xor(C_i_1);

            if (lastP != null)
            {
                newP.Xor(lastP);
            }

            return EDE.Encrypte(newP, generatorKeys[0], generatorKeys[1], generatorKeys[2]);
        }

        public BitArray Decrypte(BitArray lastC, BitArray tempC, BitArray lastP)
        {

            BitArray C_i_1;

            if (lastC != null)
                C_i_1 = lastC;
            else
                C_i_1 = C0;

            BitArray newC = EDE.Decrypte(tempC, generatorKeys[0], generatorKeys[1], generatorKeys[2]);

            newC.Xor(C_i_1);

            if (lastP != null)
            {
                newC.Xor(lastP);
            }

            return newC;
        }
    }
}