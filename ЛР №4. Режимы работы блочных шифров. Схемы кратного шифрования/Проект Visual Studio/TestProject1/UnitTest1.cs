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
        public void Rearrangement_IP()
        {
            EncryptByDES encryptByDES = new EncryptByDES(null);

            bool[] textBool = { true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false};

            BitArray text = new BitArray(textBool);

            int[,] IP ={ { 9, 10, 11, 12, 13, 14, 15, 16},
                       {1, 2, 3, 4, 5, 6 , 7, 8},
                       {25, 26, 27, 28, 29, 30, 31, 32},
                       {17, 18, 19, 20 ,21 , 22, 23, 24},
                       {41, 42, 43, 44, 45, 46, 47, 48},
                       {33, 34, 35, 36 ,37 , 38, 39, 40},
                       {57, 58, 59, 60, 61, 62, 63,64},
                       { 49, 50, 51, 52 ,53 , 54, 55,56 } };


            bool[] expectedBool = { false, false, false, false, false ,false , false, false,
                                    true, true, true, true, true, true, true, true,
                                    false, false, false, false, false ,false , false, false,
                                    true, true, true, true, true, true, true, true,
                                    false, false, false, false, false ,false , false, false,
                                    true, true, true, true, true, true, true, true,
                                    false, false, false, false, false ,false , false, false,
                                    true, true, true, true, true, true, true, true,};

            encryptByDES.RearrangementIP(ref text, IP);

            CollectionAssert.AreEqual(new BitArray(expectedBool), text);
         
        }

        [Test]
        public void Rearrangement_IP_with_IP_1()
        {
            EncryptByDES encryptByDES = new EncryptByDES(null);

            bool[] textBool = { true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false};

            BitArray text = new BitArray(textBool);

            int[,] IP = encryptByDES.IP;
            int[,] IP_1 = encryptByDES.IP_1;


            encryptByDES.RearrangementIP(ref text, IP);
            encryptByDES.RearrangementIP(ref text, IP_1);

            CollectionAssert.AreEqual(new BitArray(textBool), text);

        }


        [Test]
        public void DivideTextIntoTwoParts()
        {
            EncryptByDES encryptByDES = new EncryptByDES(null);

            bool[] textBool = { true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                false, false, false, false, false ,false , false, false,
                                false, false, false, false, false ,false , false, false,
                                false, false, false, false, false ,false , false, false};

            BitArray text = new BitArray(textBool);

            BitArray[] expected = new BitArray[]
            {
                new BitArray(new bool[]{ true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true }
                                ),
                new BitArray(new bool[]{ false, false, false, false, false ,false , false, false,
                                false, false, false, false, false ,false , false, false,
                                false, false, false, false, false ,false , false, false,
                                false, false, false, false, false ,false , false, false })
            };
            BitArray[] parts = encryptByDES.DivideTextIntoTwoParts(text);

            CollectionAssert.AreEqual(expected, parts);
        }


        [Test]
        public void EncrypteWithDecrypte()
        {
            BitArray key = new BitArray(GeneratorKey.keyLength);
            GeneratorKey generatorKey = new GeneratorKey(key);
            EncryptByDES encryptByDES = new EncryptByDES(generatorKey);

            bool[] textBool = { true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false};

            BitArray text = new BitArray(textBool);

            BitArray cipherText = encryptByDES.Encrypte(text);
            BitArray originalText = encryptByDES.Decrypte(cipherText);
            CollectionAssert.AreEqual(text, originalText);
        }


        [Test]
        public void ExtendedKeyOdd()
        {
            bool[] keyBool = { true, true, true, true, true, true, true};

            BitArray key = new BitArray(keyBool);

            BitArray expected = new BitArray(new bool[] { true, true, true, true, true, true, true, false });

            BitArray newKey = GeneratorKey.ExtendedKey(key);
            CollectionAssert.AreEqual(expected, newKey);
        }

        [Test]
        public void ExtendedKeyEven()
        {
            bool[] keyBool = { true, true, true, true, true, true, false };

            BitArray key = new BitArray(keyBool);

            BitArray expected = new BitArray(new bool[] { true, true, true, true, true, true, false, true });

            BitArray newKey = GeneratorKey.ExtendedKey(key);
            CollectionAssert.AreEqual(expected, newKey);
        }

        [Test]
        public void DivideKeyToC0()
        {
            bool[] keyBool = { true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false};

            BitArray key = new BitArray(keyBool);

            BitArray expectedC0 = new BitArray(new bool[] { false, true, false, true, false, true, false,
                                                            true, false, true, false, true, false, true,
                                                            false, true, false, true, false, true, false,
                                                            true, false, true, false, true, false, true});

            GeneratorKey generatorKey = new GeneratorKey(new BitArray(56));
            GeneratorKey.DivideKeyToC0AndD0(generatorKey, key);
            CollectionAssert.AreEqual(expectedC0, generatorKey.C_0);
        }

        [Test]
        public void DivideKeyToD0()
        {
            bool[] keyBool = { true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false};

            BitArray key = new BitArray(keyBool);

            BitArray expectedD0 = new BitArray(new bool[] { false, true, false, true, false, true, false,
                                                            true, false, true, false, true, false, true,
                                                            false, true, false, true, false, true, false,
                                                            true, false, true, false, true, false, true});
            GeneratorKey generatorKey = new GeneratorKey(new BitArray(56));
            GeneratorKey.DivideKeyToC0AndD0(generatorKey, key);
            CollectionAssert.AreEqual(expectedD0, generatorKey.D_0);
        }

        [Test]
        public void cicleLeftShift()
        {
            bool[] CBool = { true, true, true, true, true, true, true, true,
                             false, false, false, false, false ,false , false, false,
                             true, true, true, true, true, true, true, true,
                             false, false, false, false};

            BitArray C = new BitArray(CBool);

            BitArray expectedC = new BitArray(new bool[] 
            { true, true, true, true, true, true, true,
              false, false, false, false, false ,false , false, false,
              true, true, true, true, true, true, true, true,
              false, false, false, false, true});
            GeneratorKey generatorKey = new GeneratorKey(new BitArray(56));
            generatorKey.cicleLeftShift(ref C, 1);
            CollectionAssert.AreEqual(expectedC, C);
        }


        [Test]
        public void E()
        {
            EncryptorByFeistelNetwork encryptorByFeistelNetwork = new EncryptorByFeistelNetwork();

            bool[] seqBool = {  true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false};

            BitArray seq = new BitArray(seqBool);

            BitArray ESeq = encryptorByFeistelNetwork.E(seq);

            BitArray expectedSeq = new BitArray(new bool[]
            {false, true, true, true, true, true,
             true, true, true, true, true, false,
             true, false, false, false, false, false,
             false, false, false, false, false, true,
             false, true, true, true, true, true,
             true, true, true, true, true, false,
             true, false, false, false, false, false,
             false, false, false, false, false, true
            });

            CollectionAssert.AreEqual(expectedSeq, ESeq);
        }

        [Test]
        public void S_true()
        {
            EncryptorByFeistelNetwork encryptorByFeistelNetwork = new EncryptorByFeistelNetwork();

            bool[] BBool = {  true, true, true, true, true, true};

            BitArray B = new BitArray(BBool);

            BitArray result = encryptorByFeistelNetwork.S(encryptorByFeistelNetwork.S1, B);

            BitArray expectedResult = new BitArray(new bool[]
            { true, true, false, true
            });

            CollectionAssert.AreEqual(expectedResult, result);
        }

        [Test]
        public void S_in_Method()
        {
            EncryptorByFeistelNetwork encryptorByFeistelNetwork = new EncryptorByFeistelNetwork();

            bool[] BBool = { false, true, true, false, true, true };

            BitArray B = new BitArray(BBool);

            BitArray result = encryptorByFeistelNetwork.S(encryptorByFeistelNetwork.S1, B);

            BitArray expectedResult = new BitArray(new bool[]
            { false, true, false, true
            });

            CollectionAssert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Rearrangement_P()
        {
            EncryptByDES encryptByDES = new EncryptByDES(null);

            bool[] textBool = { true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false};

            BitArray text = new BitArray(textBool);

            int[,] P ={ { 16, 7, 20, 21, 29, 12, 28, 17 },
                { 1, 15, 23, 26, 5, 18, 31, 10 },
                { 2, 8, 24, 14, 32, 27, 3, 9 },
                { 19, 13, 30, 6, 22, 11, 4, 25 } };

            EncryptorByFeistelNetwork encryptorByFeistelNetwork = new EncryptorByFeistelNetwork();
            BitArray result = encryptorByFeistelNetwork.P(text);

            encryptByDES.RearrangementIP(ref text, P);

            CollectionAssert.AreEqual(text, result);

        }


        [Test]
        public void Rearrangement_P_2()
        {
            

            bool[] textBool = { true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false,
                                true, true, true, true, true, true, true, true,
                                false, false, false, false, false ,false , false, false};

            BitArray text = new BitArray(textBool);

            BitArray newText = new BitArray(new bool[]
            { false, true, true, true, false, false, false, true,
              true, false, true, false, true, true, false, false,
              true, true, true, false, false, false, true, false,
              true, false, false, true, true, false, true, false
            });

            EncryptorByFeistelNetwork encryptorByFeistelNetwork = new EncryptorByFeistelNetwork();
            BitArray result = encryptorByFeistelNetwork.P(text);

            CollectionAssert.AreEqual(newText, result);

        }


        [Test]
        public void func()
        {
            BitArray key = new BitArray(48);

            bool[] textBool = { true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true};

            BitArray rightPart = new BitArray(textBool);

            BitArray newText = new BitArray(new bool[]
            { true, true, false, true, true, false, false, true,
              true, true, false, false, true, true, true, false,
              false, false, true, true, true, true, false, true,
              true, true, false, false, true, false, true, true,
            });

            EncryptorByFeistelNetwork encryptorByFeistelNetwork = new EncryptorByFeistelNetwork();
            BitArray result = encryptorByFeistelNetwork.func(rightPart, key);

            CollectionAssert.AreEqual(encryptorByFeistelNetwork.P(newText), result);

        }

        [Test]
        public void EncrypteFeibul()
        {
            BitArray key = new BitArray(48);

            bool[] textBool = { true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true};

            BitArray rightPart = new BitArray(textBool);

            dataToEncryption data = new dataToEncryption(rightPart, rightPart, key);
            EncryptorByFeistelNetwork encryptorByFeistelNetwork = new EncryptorByFeistelNetwork();
            BitArray newText = encryptorByFeistelNetwork.P(new BitArray(new bool[]
            { !true, !true, !false, !true, !true, !false, !false,! true,
              !true, !true, !false, !false,! true,! true, !true, !false,
              !false,!false,! true, !true, !true, !true,  !false,! true,
              !true, !true, !false, !false,! true,! false,! true,! true,
            }));

            dataToEncryption expectedResult = new dataToEncryption(rightPart, newText, key);

            
            dataToEncryption result = encryptorByFeistelNetwork.Encrypte(data);

            CollectionAssert.AreEqual(expectedResult.firstPartText, result.firstPartText);
            CollectionAssert.AreEqual(expectedResult.secondPartText, result.secondPartText);
            CollectionAssert.AreEqual(expectedResult.partKey, result.partKey);

        }

        [Test]
        public void EncrypteWithDecripteFeibul()
        {
            BitArray key = new BitArray(48);

            bool[] textBool = { true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true,
                                true, true, true, true, true, true, true, true};

            BitArray rightPart = new BitArray(textBool);

            dataToEncryption data = new dataToEncryption(rightPart, rightPart, key);
            EncryptorByFeistelNetwork encryptorByFeistelNetwork = new EncryptorByFeistelNetwork();

            dataToEncryption result = encryptorByFeistelNetwork.Decrypte(encryptorByFeistelNetwork.Encrypte(data));

            CollectionAssert.AreEqual(data.firstPartText, result.firstPartText);
            CollectionAssert.AreEqual(data.secondPartText, result.secondPartText);
            CollectionAssert.AreEqual(data.partKey, result.partKey);

        }


    }
}