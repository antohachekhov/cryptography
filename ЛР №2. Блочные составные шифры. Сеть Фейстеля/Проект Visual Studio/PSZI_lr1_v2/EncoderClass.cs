using System;
using System.Collections;
using System.Text;
using System.Linq;
using PSZI_lr1;

namespace PSZI_lr1_v2
{
    class EncoderClass
    {
        public static Encoding enc = Encoding.UTF8;
        public static int byteCountLong = 8;
        public static int byteCountInt = 4;

        public static byte[] StringToByteArray(string str)
        {
            byte[] bytes = enc.GetBytes(str);

            // переворачиваем байты для того чтобы хранить их в BitArray в правильном порядке
            bytes.Reverse();
            return bytes;
        }
        
        public static string HexStringToBinString(string hexstring)
        {
            hexstring = hexstring.Replace(" ", "");
            string binString = String.Join(String.Empty,
                               hexstring.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
            char[] charArray = binString.ToCharArray();
            
            return new string(charArray);
        }


        public static string ByteArrayToString(byte[] bytes)
        {
            // переворачиваем байты для того чтобы хранить их в BitArray в правильном порядке
            bytes.Reverse();
            string str = enc.GetString(bytes);
            str = str.Trim('\0');
            return str;
        }

        public static string BitArrayToBinString(BitArray bits)
        {
            string binaryStr = "";
            foreach (bool bit in bits)
            {
                binaryStr += bit ? '1' : '0';
            }
            return binaryStr;
        }

        public static string BitArraytoHexString(BitArray bits)
        {
            StringBuilder sb = new StringBuilder();
            byte[] bytes = EncoderClass.BitArrayToByteArray(bits);
            foreach (byte b in bytes)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0')).Append(' ');

            string hexStr = sb.ToString();
            return hexStr;
        }

        public static BitArray StringToBitArray(string str)
        {
            return EncoderClass.ByteArrayToBitArray(EncoderClass.StringToByteArray(str));
        }

        public static string BitArrayToString(BitArray bits)
        {
            return EncoderClass.ByteArrayToString(EncoderClass.BitArrayToByteArray(bits));
        }

        public static long ByteArrayToLong(byte[] byteArray)
        {
            byte[] byteArrayToLong = new byte[EncoderClass.byteCountLong];
            Array.Reverse(byteArray);
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArrayToLong[i] = byteArray[i];
            }
            long number = BitConverter.ToInt64(byteArrayToLong, 0);
            return number;
        }

        public static byte[] LongToByteArray(long number)
        {
            byte[] byteArray = BitConverter.GetBytes(number);
            Array.Reverse(byteArray);
            return byteArray;
        }

        public static byte[] BitArrayToByteArray(BitArray bitArray)
        {
            BitArray bits = BitArrayFuctions.ReverseOnlyValuesInBytes(bitArray);
            byte[] bytes = new byte[Convert.ToInt32(Math.Ceiling(bits.Count / 8.0))];
            bits.CopyTo(bytes, 0);
            return bytes;
        }

        public static BitArray ByteArrayToBitArray(byte[] bytes)
        {
            BitArray bits = new BitArray(bytes);

            bits = BitArrayFuctions.ReverseOnlyValuesInBytes(bits);

            return bits;
        }

        internal static BitArray BinStringToBitArray(string str)
        {
            BitArray bits = new BitArray(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                bits.Set(i, (str[i] == '1') ? true : false);
            }
            return bits;
        }


    }
}
