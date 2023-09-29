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
            BitArray newPartText = new BitArray(partText.Length);

            if (command == ModeGenFunc.one)
            {
                newPartText = partKey;
            }
            else
            {
                // ещё больше магии               
                // Доделать
            }

            return newPartText;
        }

        public dataToEncryption Encrypte(dataToEncryption data)
        {
            BitArray firstPartText = new BitArray(data.firstPartText);
            data.firstPartText = func(data.firstPartText, data.partKey).Xor(data.secondPartText);

            data.secondPartText = firstPartText;
            return data;
        }

        public EncryptorByFeistelNetwork(ModeGenFunc command)
        {
            this.command = command;
        }
    }
}
