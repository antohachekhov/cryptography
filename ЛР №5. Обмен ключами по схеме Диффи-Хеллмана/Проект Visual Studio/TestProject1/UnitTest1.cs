using NUnit.Framework;
using PSZI_lr1_v2;
using System.Collections;
using System.Collections.Generic;

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


        [Test]
        public void GetSampleFactorsFrom2()
        {
            ulong number = 2;

            List<FactorWithPow> expectedSampleFactors = new List<FactorWithPow>();
            expectedSampleFactors.Add(new FactorWithPow(2, 1));

            Program program = new Program();
            int t = 5;
            List<FactorWithPow> actualNumber = program.getSampleFactors(number, t);

            CollectionAssert.AreEqual(expectedSampleFactors, actualNumber);
        }

        [Test]
        public void GetSampleFactorsFrom1024()
        {
            ulong number = 1024;

            List<FactorWithPow> expectedSampleFactors = new List<FactorWithPow>();
            expectedSampleFactors.Add(new FactorWithPow(2, 10));

            Program program = new Program();
            int t = 5;
            List<FactorWithPow> actualNumber = program.getSampleFactors(number, t);

            CollectionAssert.AreEqual(expectedSampleFactors, actualNumber);
        }

        [Test]
        public void GetSampleFactorsFrom41()
        {
            ulong number = 41;

            List<FactorWithPow> expectedSampleFactors = new List<FactorWithPow>();
            expectedSampleFactors.Add(new FactorWithPow(41, 1));

            Program program = new Program();
            int t = 5;
            List<FactorWithPow> actualNumber = program.getSampleFactors(number, t);

            CollectionAssert.AreEqual(expectedSampleFactors, actualNumber);
        }

        [Test]
        public void GetSampleFactorsFromNumber()
        {
            ulong number = 40;

            List<FactorWithPow> expectedSampleFactors = new List<FactorWithPow>();
            expectedSampleFactors.Add(new FactorWithPow(2, 3));
            expectedSampleFactors.Add(new FactorWithPow(5, 1));

            Program program = new Program();
            int t = 5;
            List<FactorWithPow> actualNumber = program.getSampleFactors(number, t);

            CollectionAssert.AreEqual(expectedSampleFactors, actualNumber);
        }


        [Test]
        public void Sum1()
        {
            ulong number1 = 40;
            ulong number2 = 40;

            BitArray bitArray1 = EncoderClass.UlongToBitArray(number1);
            BitArray bitArray2 = EncoderClass.UlongToBitArray(number2);

            ulong expectedNumber = 80;
            
            ulong actualNumber = EncoderClass.BitArrayToUlong(BitArrayFunctions.Sum(bitArray1, bitArray2));

            Assert.AreEqual(expectedNumber, actualNumber);
        }


        [Test]
        public void Sum2()
        {
            ulong number1 = 40;
            ulong number2 = 2048;

            BitArray bitArray1 = EncoderClass.UlongToBitArray(number1);
            BitArray bitArray2 = EncoderClass.UlongToBitArray(number2);

            ulong expectedNumber = 2088;

            ulong actualNumber = EncoderClass.BitArrayToUlong(BitArrayFunctions.Sum(bitArray1, bitArray2));

            Assert.AreEqual(expectedNumber, actualNumber);
        }

        [Test]
        public void Sum3()
        {
            ulong number1 = 40;
            ulong number2 = 180000;

            BitArray bitArray1 = EncoderClass.UlongToBitArray(number1);
            BitArray bitArray2 = EncoderClass.UlongToBitArray(number2);

            ulong expectedNumber = 180040;

            ulong actualNumber = EncoderClass.BitArrayToUlong(BitArrayFunctions.Sum(bitArray1, bitArray2));

            Assert.AreEqual(expectedNumber, actualNumber);
        }


        [Test]
        public void Mult1()
        {
            ulong number1 = 1;
            ulong number2 = 40;

            BitArray bitArray1 = EncoderClass.UlongToBitArray(number1);
            BitArray bitArray2 = EncoderClass.UlongToBitArray(number2);

            ulong expectedNumber = 40;

            ulong actualNumber = EncoderClass.BitArrayToUlong(BitArrayFunctions.Mult(bitArray1, bitArray2));

            Assert.AreEqual(expectedNumber, actualNumber);
        }


        [Test]
        public void Mult2()
        {
            ulong number1 = 40;
            ulong number2 = 40;

            BitArray bitArray1 = EncoderClass.UlongToBitArray(number1);
            BitArray bitArray2 = EncoderClass.UlongToBitArray(number2);

            ulong expectedNumber = 1600;

            ulong actualNumber = EncoderClass.BitArrayToUlong(BitArrayFunctions.Mult(bitArray1, bitArray2));

            Assert.AreEqual(expectedNumber, actualNumber);
        }

        [Test]
        public void Mult3()
        {
            ulong number1 = 40;
            ulong number2 = 180000;

            BitArray bitArray1 = EncoderClass.UlongToBitArray(number1);
            BitArray bitArray2 = EncoderClass.UlongToBitArray(number2);

            ulong expectedNumber = 7200000;

            ulong actualNumber = EncoderClass.BitArrayToUlong(BitArrayFunctions.Mult(bitArray1, bitArray2));

            Assert.AreEqual(expectedNumber, actualNumber);
        }

        [Test]
        public void Pow1()
        {
            ulong number = 40;
            ulong pow = 1;

            BitArray bitArray = EncoderClass.UlongToBitArray(number);

            ulong expectedNumber = 40;

            ulong actualNumber = EncoderClass.BitArrayToUlong(BitArrayFunctions.Pow(bitArray, pow));

            Assert.AreEqual(expectedNumber, actualNumber);
        }

        [Test]
        public void Pow2()
        {
            ulong number = 40;
            ulong pow = 2;

            BitArray bitArray = EncoderClass.UlongToBitArray(number);

            ulong expectedNumber = 1600;

            ulong actualNumber = EncoderClass.BitArrayToUlong(BitArrayFunctions.Pow(bitArray, pow));


            Assert.AreEqual(expectedNumber, actualNumber);
        }
    }
}