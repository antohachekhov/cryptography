using PSZI_lr1_v2;
using System;
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using System.IO;

namespace PSZI_lr1
{
    public struct dataToEncryption
    {
        public BitArray firstPartText;
        public BitArray secondPartText;
        public BitArray partKey;

        public dataToEncryption(BitArray firstPartText, BitArray secondPartText, BitArray partKey)
        {
            this.firstPartText = firstPartText;
            this.secondPartText = secondPartText;
            this.partKey = partKey;
        }
    }

    class EncryptorByFeistelNetwork
    {
        ModeGenFunc command;

        BitArray func(BitArray partText, BitArray partKey)
        {
            BitArray newPartText;

            if (command == ModeGenFunc.one)
            {
                newPartText = new BitArray(partKey);
            }
            else
            {
                // функция имеет вид ( ) ( ), где() – левая часть шифруемого блока,
                // на которую посредством операции XOR была наложена 32 - битная последовательность, сгенерированная 16 разрядным скремблером вида
                BitArray leftPartText = new BitArray(partKey.Length);
                for (int i = 0; i < partKey.Length && i < partText.Length; i++)
                    leftPartText[i] = partText[i];
                LFSR lfsr = new LFSR(false);
                BitArray sequence = lfsr.generatePRV(partKey.Length, 100);
                leftPartText.Xor(sequence);
                newPartText = new BitArray(leftPartText);
            }

            return newPartText;
        }

        public dataToEncryption Encrypte(dataToEncryption data)
        {
            BitArray firstPartText = new BitArray(data.firstPartText);
            data.firstPartText = func(data.firstPartText, data.partKey).And(data.secondPartText);

            data.secondPartText = firstPartText;
            return data;
        }

        public EncryptorByFeistelNetwork(ModeGenFunc command)
        {
            this.command = command;
        }
    }
}
