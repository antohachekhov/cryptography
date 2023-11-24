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

            BigInteger message = BigInteger.Parse(TextBoxMsg.Text);
            keysRSA keys = GeneratorKeysRSA.generateKeys(BigInteger.Parse("680564733841876926927715055360727278773"), BigInteger.Parse("295734403997374903219419662932896015067"));

            EncryptByRSA.edsRSA eds = EncryptByRSA.GenerateEDS(message, keys.closeKey);

            TextBoxEDSRSA.Text = "(" + eds.eds.m.ToString() + "," + eds.eds.s.ToString() + ")";
        }
        public void ButtonCheckRSA_Click(object sender, RoutedEventArgs ev)
        {
            string[] strings = TextBoxEDSRSA.Text.Split('(', ',', ')');



            BigInteger m = BigInteger.Parse(strings[1]);
            BigInteger s = BigInteger.Parse(strings[2]);

            EncryptByRSA.edsRSA eds = new EncryptByRSA.edsRSA(m, s);

            //BigInteger e = 7;
            //BigInteger n = 77;
            keysRSA keys = GeneratorKeysRSA.generateKeys(BigInteger.Parse("680564733841876926927715055360727278773"), BigInteger.Parse("295734403997374903219419662932896015067"));

            bool check = EncryptByRSA.CheckEDS(eds, keys.openKey);
            //bool check = EncryptByRSA.CheckEDS(eds, (e, n));

            TextBoxEDSRSATrue.Text = check ? "Подлинная" : "Неподлинная";
        }
        public void ButtonCountEG_Click(object sender, RoutedEventArgs e)
        {
            BigInteger m = BigInteger.Parse(TextBoxMsg.Text);
            BigInteger p = 23;
            BigInteger g = 5;
            BigInteger x = 3;
            BigInteger k = 13;

            keysElgamal keys = GeneratorKeysElgamal.generateKeys(p, g, x);
            EncryptByElgamal.edsElgamal eds = EncryptByElgamal.GenerateEDS(m, keys, k);

            TextBoxEDSEG.Text = "(" + eds.eds.a.ToString() + ',' + eds.eds.b.ToString() + ')';
        }
        public void ButtonCheckEG_Click(object sender, RoutedEventArgs e)
        {
            BigInteger m = BigInteger.Parse(TextBoxMsg.Text);
            BigInteger p = 23;
            BigInteger g = 5;
            BigInteger x = 3;
            keysElgamal keys = GeneratorKeysElgamal.generateKeys(p, g, x);

            string[] strings = TextBoxEDSEG.Text.Split('(', ',', ')');



            BigInteger a = BigInteger.Parse(strings[1]);
            BigInteger b = BigInteger.Parse(strings[2]);

            EncryptByElgamal.edsElgamal eds = new EncryptByElgamal.edsElgamal(a, b);

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

    }
}
