using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if(lastP != null)
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

        public static TextWithAvalanche Decrypte(PCBC PCBCTrue, PCBC PCBCFalse,
                                                 BitArray lastCTrue, BitArray lastCFalse,
                                                 BitArray tempCTrue, BitArray tempCFalse,
                                                 BitArray lastPTrue, BitArray lastPFalse)
        {
            BitArray C_i_1True, C_i_1False;

            if (lastCTrue != null)
                C_i_1True = lastCTrue;
            else
                C_i_1True = PCBCTrue.C0;

            if (lastCFalse != null)
                C_i_1False = lastCFalse;
            else
                C_i_1False = PCBCFalse.C0;

            TextWithAvalanche newCWithAvalanche = EDE.DecrypteWithAvalanche(tempCTrue, tempCFalse, PCBCTrue.generatorKeys, PCBCFalse.generatorKeys);

            newCWithAvalanche.textTrue.Xor(C_i_1True);
            newCWithAvalanche.textFalse.Xor(C_i_1False);

            if (lastP != null)
            {
                lastPTrue.Xor(lastP);
            }

            return newC;
        }

        public static TextWithAvalanche Encrypte(PCBC pCBCTrue, PCBC pCBCFalse,
                                                   BitArray lastPTrue, BitArray lastPFalse,
                                                   BitArray tempPTrue, BitArray tempPFalse,
                                                   BitArray lastCTrue, BitArray lastCFalse)
        {
            
        }
