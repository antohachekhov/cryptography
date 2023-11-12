using System;
using System.Collections;
using System.Text;
using System.Linq;
using System.Numerics;

namespace PSZI_lr1_v2
{
    public class EncoderClass
    {
        public static Encoding enc = Encoding.ASCII;
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

        public static string BitArrayToHexString(BitArray bits)
        {
            StringBuilder sb = new StringBuilder();
            byte[] bytes = BitArrayToByteArray(bits);
            foreach (byte b in bytes)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0')).Append(' ');

            string hexStr = sb.ToString();
            return hexStr;
        }

        public static BitArray StringToBitArray(string str)
        {
            return ByteArrayToBitArray(StringToByteArray(str));
        }

        public static string BitArrayToString(BitArray bits)
        {
            return ByteArrayToString(BitArrayToByteArray(bits));
        }

        public static long ByteArrayToLong(byte[] byteArray)
        {
            byte[] byteArrayToLong = new byte[byteCountLong];
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
            BitArray bits = BitArrayFunctions.ReverseOnlyValuesInBytes(bitArray);
            byte[] bytes = new byte[Convert.ToInt32(Math.Ceiling(bits.Count / 8.0))];
            bits.CopyTo(bytes, 0);
            return bytes;
        }

        public static byte[] BitArrayToByteArray2(BitArray bitArray)
        {
            BitArray bits = bitArray;
            byte[] bytes = new byte[Convert.ToInt32(Math.Ceiling(bits.Count / 8.0))];
            bits.CopyTo(bytes, 0);
            Array.Reverse(bytes);
            // Младший бит в старшей позиции
            return bytes;
        }

        public static BitArray ByteArrayToBitArray(byte[] bytes)
        {
            BitArray bits = new BitArray(bytes);

            bits = BitArrayFunctions.ReverseOnlyValuesInBytes(bits);

            return bits;
        }

        public static BitArray ByteArrayToBitArray2(byte[] bytes)
        {
            Array.Reverse(bytes);
            BitArray bits = new BitArray(bytes);
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

        public static BitArray IntToBitArrayLength4(int value)
        {
            BitArray bits = new BitArray(4);

            for(int i = bits.Length - 1; i >=0 ; i--)
            {
                bits[i] = ((int)(value / Math.Pow(2,i)) == 1) ? true : false;

                double v = value / Math.Pow(2, i);
                value -= (int)((int)v * Math.Pow(2, i));
            }

            
            return bits.ReverseAll(); ;
        }


        public static BitArray BigIntegerToBitArray(BigInteger bigInteger)
        {
            return ByteArrayToBitArray2(BigIntegerToByteArray(bigInteger));
        }

        private static byte[] BigIntegerToByteArray(BigInteger bigInteger)
        {
            byte[] byteArray = bigInteger.ToByteArray();

            Array.Reverse(byteArray); // Обратный переворот младший бит стоит в последней позиции

            // Удаление лишних нулевых элементов
            int countOfNullBytes = 0;
            foreach (byte b in byteArray)
            {
                if (b == 0x0) countOfNullBytes++;
                else break;
            }
            byte[] byteArrayWithoutNull = new byte[byteArray.Length - countOfNullBytes];
            Array.Copy(byteArray, countOfNullBytes, byteArrayWithoutNull, 0, byteArray.Length - countOfNullBytes);

            // Младший бит стоит в старшей позиции
            return byteArrayWithoutNull;
        }

        public static BigInteger BitArrayToBigInteger(BitArray bitArray)
        {
            return ByteArrayToBigInteger(BitArrayToByteArray2(bitArray));
        }

        private static BigInteger ByteArrayToBigInteger(byte[] byteArray)
        {

            Array.Reverse(byteArray); // Младший бит числа должен находиться в нулевой позиции
            BigInteger number = new BigInteger(byteArray);
            return number;
        }

    }
}
