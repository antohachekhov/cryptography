using System;
using System.Windows;
using Microsoft.Win32;
using System.Collections;
using System.Windows.Controls;
using System.Linq;
using System.Numerics;

namespace PSZI_lr1_v2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Program program = null;

        public MainWindow()
        {
            program = new Program();
            InitializeComponent();
        }

        public void ButtonCountRSA_Click(object sender, RoutedEventArgs e)
        {
            BigInteger hashValue = BigInteger.Parse(TextBoxHash.Text);

            // Генерация ключей RSA
            keysRSA keys = GeneratorKeysRSA.generateKeys(BigInteger.Parse("680564733841876926927715055360727278773"), BigInteger.Parse("295734403997374903219419662932896015067"));

            // Вывод открытого ключа:
            TextBoxOpenKey.Text = "(" + keys.openKey.Item1.ToString() + "," + keys.openKey.Item2.ToString() + ")";


            // Генерация цифровой подписи
            EncryptByRSA.edsRSA eds = EncryptByRSA.GenerateEDS(hashValue, keys.closeKey);


            string message = TextBoxMsg.Text;
            TextBoxEDSRSA.Text = "<" + message + "," + eds.eds.ToString() + ">";
        }

        public void ButtonCheckRSA_Click(object sender, RoutedEventArgs ev)
        {
            // Ввод электронного документа
            string[] strings = TextBoxEDSRSA.Text.Split('<', ',', '>');

            // Parse message
            string message = "";
            BigInteger m = 0;
            try
            {
                m = BigInteger.Parse(strings[1]);
            }
            catch
            {
                message = strings[1];
            }

            if (message != "")
                m = program.generateHashValue(message);

            BigInteger s = BigInteger.Parse(strings[2]);

            EncryptByRSA.edsRSA eds = new EncryptByRSA.edsRSA(s);


            // Ввод открытого ключа

            strings = TextBoxOpenKey.Text.Split('(', ',', ')');



            BigInteger e = BigInteger.Parse(strings[1]);
            BigInteger n = BigInteger.Parse(strings[2]);
            keysRSA keys = new keysRSA(e, 0, n);

            bool check = EncryptByRSA.CheckEDS(m, eds, keys.openKey);

            TextBoxEDSRSATrue.Text = check ? "Подлинная" : "Неподлинная";
        }
        public void ButtonCountEG_Click(object sender, RoutedEventArgs e)
        {
            BigInteger hashValue = BigInteger.Parse(TextBoxHash.Text);
            BigInteger p = BigInteger.Parse(TextBoxEGp.Text);
            BigInteger g = BigInteger.Parse(TextBoxEGg.Text);
            BigInteger x = BigInteger.Parse(TextBoxEGx.Text);
            BigInteger k = BigInteger.Parse(TextBoxEGk.Text);


            if (g >= p || x >= p)
            {
                MessageBox.Show("Проверьте правильность введенных чисел! g < p && x < p", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            keysElgamal keys = GeneratorKeysElgamal.generateKeys(p, g, x);
            // Вывод открытого ключа:
            TextBoxOpenKey.Text = "(" + keys.openKey.y.ToString() + "," + keys.openKey.g.ToString() + "," + keys.openKey.p.ToString() + ")";

            EncryptByElgamal.edsElgamal eds = EncryptByElgamal.GenerateEDS(hashValue, keys, k);

            string message = TextBoxMsg.Text;

            TextBoxEDSEG.Text = "<" + message + "," + "(" + eds.eds.a.ToString() + ";" + eds.eds.b.ToString() + ")" + ">";
        }
        public void ButtonCheckEG_Click(object sender, RoutedEventArgs e)
        {
            // Ввод электронного документа
            string[] strings = TextBoxEDSEG.Text.Split('<', ',', '>');

            // Parse message
            string message = "";
            BigInteger m = 0;
            try
            {
                m = BigInteger.Parse(strings[1]);
            }
            catch
            {
                message = strings[1];
            }

            if (message != "")
                m = program.generateHashValue(message);

            strings = strings[2].Split('(', ';', ')');
            BigInteger a = BigInteger.Parse(strings[1]);
            BigInteger b = BigInteger.Parse(strings[2]);

            // Ввод открытого ключа
            strings = TextBoxOpenKey.Text.Split('(', ',', ')');
            BigInteger y = BigInteger.Parse(strings[1]);
            BigInteger g = BigInteger.Parse(strings[2]);
            BigInteger p = BigInteger.Parse(strings[3]);

            EncryptByElgamal.edsElgamal eds = new EncryptByElgamal.edsElgamal(a, b);

            keysElgamal keys = new keysElgamal(y, g, p, 0);

            bool check = EncryptByElgamal.CheckEDS(m, keys, eds);

            TextBoxEDSEGTrue.Text = check ? "Подлинная" : "Неподлинная";
        }

        public BigInteger RandomGenerate()
        {
            BigInteger[] arr = new BigInteger[]
            {
                BigInteger.Parse("680564733841876926927715055360727278773"),
                BigInteger.Parse("295734403997374903219419662932896015067"),
                BigInteger.Parse("340282366920938463471724603615805326309"),
                BigInteger.Parse("582763727518307894417093596483818754517"),
                BigInteger.Parse("910282376920938463463568564727192087897"),
                BigInteger.Parse("828013009170620080387581665656564811753"),
                BigInteger.Parse("850093028839980204918799989614299326233"),
                BigInteger.Parse("282898886447483866258689260311414793389"),
                BigInteger.Parse("501429714320794494411675879087544625713"),
                BigInteger.Parse("490462918295251921316534706924024850613"),
                BigInteger.Parse("148462460474284711355933156222025664063"),
                BigInteger.Parse("632121803788957602408189975765888484859"),
                BigInteger.Parse("437439071908070065173606047197212447347"),
                BigInteger.Parse("703561419676555233452828219966184075211"),
                BigInteger.Parse("292713950364172170383564885648055469889"),
                BigInteger.Parse("578018259923088227806680514049621249249"),
                BigInteger.Parse("856884970121640316723697223110615336323"),
                BigInteger.Parse("978817388248000043294238098479319430089"),
                BigInteger.Parse("834811222397903132340707132019551765009"),
                BigInteger.Parse("675384074886636627272130607192595914811"),
                BigInteger.Parse("249498771347972223262240032751403730959"),
                BigInteger.Parse("121196625664180396992286699569047469611")
            };

            int i = program.random.Next(arr.Length);

            return arr[i];
        }

        private void ButtonCountHash_Click(object sender, RoutedEventArgs e)
        {
            string message = TextBoxMsg.Text;

            BigInteger hashValue = program.generateHashValue(message);

            TextBoxHash.Text = hashValue.ToString();

            //Console.WriteLine("Схема-1: " + EncoderClass.BitArrayToString(hash));
            //hash = HashFunctions.schema2(blocks, rand);
            //Console.WriteLine("Схема-2: " + EncoderClass.BitArrayToString(hash));
            //hash = HashFunctions.schema3(blocks, rand);
            //Console.WriteLine("Схема-3: " + EncoderClass.BitArrayToString(hash));
            //hash = HashFunctions.schema4(blocks, rand);
            //Console.WriteLine("Схема-4: " + EncoderClass.BitArrayToString(hash));
            //
            //BitArray rand2 = program.GenerateRandomBitArray(8);
            //hash = HashFunctions.PBGV(blocks, rand, rand2);
            //Console.WriteLine("Схема-PBGV: " + EncoderClass.BitArrayToString(hash));
            //hash = HashFunctions.QG(blocks, rand, rand2);
            //Console.WriteLine("Схема-QG: " + EncoderClass.BitArrayToString(hash));
        }
    }
}
