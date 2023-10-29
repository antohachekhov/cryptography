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
            byte[] expectedArray = new byte[] { 0x7, 0x5b, 0xcd, 0x15 }; // 0x15, 0xcd, 0x5b, 0x7

            byte[] actualArray = EncoderClass.UlongToByteArray(number);

            CollectionAssert.AreEquivalent(expectedArray, actualArray);
        }

        [Test]
        public void ByteToULong()
        {
            byte[] array = new byte[] { 0x7, 0x5b, 0xcd, 0x15 };

            ulong expectedNumber = 123456789;

            ulong actualNumber = EncoderClass.ByteArrayToUlong(array);

            Assert.AreEqual(expectedNumber, actualNumber);
        }
    }
}