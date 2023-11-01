using NUnit.Framework;
using PSZI_lr1_v2;
using System.Collections;

namespace TestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void UlongToByte()
        {
            ulong number = 123456789;
            byte[] expectedArray = new byte[] { 0x07, 0x5b, 0xcd, 0x15 };

            byte[] actualArray = EncoderClass.UlongToByteArray(number);

            CollectionAssert.AreEqual(expectedArray, actualArray);
        }

        [Test]
        public void ByteToULong()
        {
            byte[] array = new byte[] { 0x07, 0x5b, 0xcd, 0x15 };

            ulong expectedNumber = 123456789;

            ulong actualNumber = EncoderClass.ByteArrayToUlong(array);

            Assert.AreEqual(expectedNumber, actualNumber);
        }


        [Test]
        public void UlongToBitArray()
        {
            ulong number = 123456789;

            bool[] expectedArray = new bool[] { true, false, true, false, true, false, false, false,
                                                true, false, true, true, false, false, true, true,
                                                true, true, false, true, true, false, true, false,
                                                true, true, true, false, false, false, false, false}; 


            BitArray actualArray = EncoderClass.UlongToBitArray(number);

            CollectionAssert.AreEqual(new BitArray(expectedArray), actualArray);
        }

        [Test]
        public void BitArrayToULong()
        {
            bool[] array = new bool[] { true, false, true, false, true, false, false, false,
                                                   true, false, true, true, false, false, true, true,
                                                   true, true, false, true, true, false, true, false,
                                                   true, true, true, false, false, false, false, false};

            ulong expectedNumber = 123456789;

            ulong actualNumber = EncoderClass.BitArrayToUlong(new BitArray(array));

            Assert.AreEqual(expectedNumber, actualNumber);
        }



        [Test]
        public void BitArray4ToULong()
        {
            bool[] array = new bool[] { true, true, true, true };

            ulong expectedNumber = 15;

            ulong actualNumber = EncoderClass.BitArrayToUlong(new BitArray(array));

            Assert.AreEqual(expectedNumber, actualNumber);
        }
    }
}