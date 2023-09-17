using System;
using System.Text;

namespace PSZI_lr1_v2
{
    class EncoderClass
    {
        public static Encoding enc = Encoding.GetEncoding(1251);
        public static int byteCountLong = 8;
        public static int byteCountInt = 4;

        public static byte[] StringToByteArray(string str)
        {
            return enc.GetBytes(str);
        }

        public static byte[] IntToByteArray(int number)
        {
            byte[] byteArray = BitConverter.GetBytes(number);
            Array.Reverse(byteArray);
            return byteArray;
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            string str = enc.GetString(bytes);
            str = str.Trim('\0');
            return str;
        }

        public static string StringtoBin(string str)
        {
            string cc2 = "";
            for (int i = 0; i < str.Length; i++)
                cc2 += Convert.ToString(str[i], 2);
            return cc2;
        }

        public static string StringtoBin(string str, int length)
        {
            long number = EncoderClass.ByteArrayToLong(EncoderClass.StringToByteArray(str));
            string cc2 = Convert.ToString(number, 2);

            if (cc2.Length < length)
            {
                for (int i = 0; i < length - cc2.Length; )
                    cc2 = '0' + cc2;
            }


            return cc2;
        }
        public static string StringtoHex(string str)
        {
            string cc16 = "";
            for (int i = 0; i < str.Length; i++)
                cc16 += Convert.ToString(str[i], 16);
            return cc16;
        }

        internal static long ByteArrayToLong(byte[] byteArray)
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

        internal static byte[] LongToByteArray(long number)
        {
            byte[] byteArray = BitConverter.GetBytes(number);
            Array.Reverse(byteArray);
            return byteArray;
        }

        internal static int ByteArrayToInt(byte[] byteArray)
        {
            byte[] byteArrayToInt = new byte[EncoderClass.byteCountInt];
            Array.Reverse(byteArray);
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArrayToInt[i] = byteArray[i];
            }
            int number = BitConverter.ToInt32(byteArrayToInt, 0);
            return number;
        }
    }
}
