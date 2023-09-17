using System;
using System.Text;

namespace PSZI_lr1_v2
{
    class EncoderClass
    {
        static Encoding enc = Encoding.ASCII;

        public static byte[] StringToByteArray(string str)
        {
            return enc.GetBytes(str);
        }

        public static byte[] IntToByteArray(int number)
        {
            return BitConverter.GetBytes(number);
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            return enc.GetString(bytes);
        }

        public static string StringtoBin(string str)
        {
            string cc2 = "";
            for (int i = 0; i < str.Length; i++)
                cc2 += Convert.ToString(str[i], 2);
            return cc2;
        }
        public static string StringtoHex(string str)
        {
            string cc16 = "";
            for (int i = 0; i < str.Length; i++)
                cc16 += Convert.ToString(str[i], 16);
            return cc16;
        }

        internal static uint ByteArrayToUint(byte[] keyToByte)
        {
            return BitConverter.ToUInt32(keyToByte, 0);
        }

        internal static byte[] UintToByteArray(uint number)
        {
            return BitConverter.GetBytes(number);
        }

        internal static int ByteArrayToInt(byte[] keyToByte)
        {
            return BitConverter.ToInt32(keyToByte, 0);
        }
    }
}
