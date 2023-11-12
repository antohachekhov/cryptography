using System;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
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

        private void setValueToProgramBitArray(string name, string text)
        {
            if (name.Contains("OriginalText"))
                program.originalText = EncoderClass.StringToBitArray(text);
            else if (name.Contains("SampleP"))
                program.p = EncoderClass.BigIntegerToBitArray(BigInteger.Parse(text));
            else if (name.Contains("SampleQ"))
                program.q = EncoderClass.BigIntegerToBitArray(BigInteger.Parse(text));
            else if (name.Contains("OpenKey"))
            {
                string[] partTexts = text.Split('(', ',', ')');

                BigInteger e = BigInteger.Parse(partTexts[1]);
                BigInteger n = BigInteger.Parse(partTexts[2]);

                program.keys.openKey = (e, n);
            }
            else if (name.Contains("CloseKey"))
            {
                string[] partTexts = text.Split('(', ',', ')');

                BigInteger d = BigInteger.Parse(partTexts[1]);
                BigInteger n = BigInteger.Parse(partTexts[2]);

                program.keys.openKey = (d, n);
            }
            else if (name.Contains("CipherK"))
                program.cipherKey = EncoderClass.StringToBitArray(text);
        }

        private void writeToWindow(StackPanel stackPanel)
        {

            string name = stackPanel.Name;
            string text = "";
            string text16 = "";
            if (name.Contains("OriginalText"))
            {
                text = EncoderClass.BitArrayToString(program.originalText);
                text16 = EncoderClass.BitArrayToHexString(program.originalText);
            }
            else if (name.Contains("SampleP"))
                text = Convert.ToString(EncoderClass.BitArrayToBigInteger(program.p));
            else if (name.Contains("SampleQ"))
                text = Convert.ToString(EncoderClass.BitArrayToBigInteger(program.q));
            else if (name.Contains("OpenKey"))
                text = "(" + Convert.ToString(program.keys.openKey.Item1) + "," + Convert.ToString(program.keys.openKey.Item2) + ")";
            else if (name.Contains("CloseKey"))
                text = "(" + Convert.ToString(program.keys.closeKey.Item1) + "," + Convert.ToString(program.keys.closeKey.Item2) + ")";
            else if (name.Contains("CipherK"))
                text = EncoderClass.BitArrayToString(program.cipherKey);
            else if (name.Contains("CipherText"))
            {
                text = EncoderClass.BitArrayToString(program.cipherText);
                text16 = EncoderClass.BitArrayToHexString(program.cipherText);
            }
            stackPanel.Children.OfType<TextBox>().First().Text = text;

            if (stackPanel.Children.OfType<TextBox>().Count() > 1)
            {
                stackPanel.Children.OfType<TextBox>().Last().Text = text16;
            }
        }

        private void TextBoxSampleNumbers_TextChanged(object sender, RoutedEventArgs e)
        {
            // Определение инициатора
            TextBox textBoxCC = sender as TextBox;
            StackPanel stackPanel = textBoxCC?.Parent as StackPanel;

            // Запись в переменную
            setValueToProgramBitArray(stackPanel.Name, textBoxCC.Text);

            // Запись в файл
            Program.writeToFile(stackPanel.Name + ".txt", textBoxCC.Text);
        }

        // Изменение текста в сс с записью в 16 сс
        private void TextBoxCC_TextChanged(object sender, RoutedEventArgs e)
        {
            // Определение инициатора
            TextBox textBoxCC = sender as TextBox;
            StackPanel stackPanel = textBoxCC?.Parent as StackPanel;

            BitArray bitArrayFromText = EncoderClass.StringToBitArray(textBoxCC.Text);

            // Изменение значений в cc 16
            TextBox textBoxCC16 = stackPanel.Children.OfType<TextBox>().Last();
            textBoxCC16.Text = EncoderClass.BitArrayToHexString(bitArrayFromText);

            // Запись в переменную
            setValueToProgramBitArray(stackPanel.Name, textBoxCC.Text);

            // Запись в файл
            Program.writeToFile(stackPanel.Name + ".txt", textBoxCC.Text);
        }

        // Изменение текста в 16 сс с записью в сс
        private void TextBoxCC16_TextChanged(object sender, RoutedEventArgs e)
        {
            // Определение инициатора
            TextBox textBoxCC16 = sender as TextBox;
            StackPanel stackPanel = textBoxCC16?.Parent as StackPanel;

            BitArray bitArrayFromText = EncoderClass.BinStringToBitArray(EncoderClass.HexStringToBinString(textBoxCC16.Text));

            // Изменение значений в cc 16
            TextBox textBoxCC = stackPanel.Children.OfType<TextBox>().First();
            textBoxCC.Text = EncoderClass.BitArrayToString(bitArrayFromText);

            // Запись в переменную
            setValueToProgramBitArray(stackPanel.Name, textBoxCC.Text);

            // Запись в файл
            Program.writeToFile(stackPanel.Name + ".txt", textBoxCC.Text);
        }


        // Шифрование
        private void ButtonEncrypte_Click(object sender, RoutedEventArgs e)
        {
            program.Encryption();
            Program.writeToFile("cipherText.txt", EncoderClass.BitArrayToString(program.cipherText));
            Program.writeToFile("cipherKey.txt", EncoderClass.BitArrayToString(program.cipherKey));
            writeToWindow(StackPanelCipherTextOutput);
            writeToWindow(StackPanelCipherKeyOutput);
        }

        // Дешифрование
        private void ButtonDecrypte_Click(object sender, RoutedEventArgs e)
        {
            program.Decryption();
            Program.writeToFile("originalText.txt", EncoderClass.BitArrayToString(program.originalText));
            writeToWindow(StackPanelOriginalTextOutput);
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            // Определение инициатора события
            Button button = sender as Button;

            // Определение родительского элемента
            StackPanel stackPanel = (button?.Parent) as StackPanel;

            // Определение дочернего элемента текст бокса
            TextBox textBox = stackPanel.Children.OfType<TextBox>().FirstOrDefault();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                textBox.Text = openFileDialog.FileName;
                string text = Program.ReadFromFile(openFileDialog.FileName);

                // Запись в переменную
                setValueToProgramBitArray(stackPanel.Name, text);

                writeToWindow((stackPanel.Parent as StackPanel).Children.OfType<StackPanel>().Last());
            }
        }

        private void ButtonGenerateSampleNumber_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            StackPanel stackPanel = (button?.Parent) as StackPanel;

            string name = stackPanel.Name;

            BigInteger randomSampleNumber = program.generateSimpleNumberByN(130, BigInteger.Parse("10000000000000000000000000000"));

            string randomBitArray = this.RandomGenerate().ToString();
            // Запись в переменную
            setValueToProgramBitArray(name, randomBitArray);

            writeToWindow((stackPanel.Parent as StackPanel).Children.OfType<StackPanel>().Last());
        }

        private void ButtonGenerateSampleNumber2_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            StackPanel stackPanel = (button?.Parent) as StackPanel;

            string name = stackPanel.Name;

            BigInteger randomSampleNumber = program.generateSimpleNumberFromRange(BigInteger.Parse("10000000000000000000000000000"), BigInteger.Parse("10000000000000000000000000000000"), 1000);
            string randomBitArray = randomSampleNumber.ToString();
            // Запись в переменную
            setValueToProgramBitArray(name, randomBitArray);

            writeToWindow((stackPanel.Parent as StackPanel).Children.OfType<StackPanel>().Last());
        }

        private void TextBoxCC16_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.TextChanged += TextBoxCC16_TextChanged;
        }

        private void TextBoxCC_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.TextChanged += TextBoxCC_TextChanged;
        }

        private void TextBoxCC_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            writeToWindow(textBox?.Parent as StackPanel);
            // Вывод на экран
            textBox.TextChanged -= TextBoxCC_TextChanged;
        }

        private void TextBoxCC16_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            writeToWindow(textBox?.Parent as StackPanel);
            // Вывод на экран
            textBox.TextChanged -= TextBoxCC16_TextChanged;
        }

        private void ButtonGenerateKeys_Click(object sender, RoutedEventArgs e)
        {
            program.GenerateRSAKeys();

            Program.writeToFile("openKey.txt", "(" + Convert.ToString(program.keys.openKey.Item1) + "," + Convert.ToString(program.keys.openKey.Item2) + ")");
            Program.writeToFile("closeKey.txt", "(" + Convert.ToString(program.keys.closeKey.Item1) + "," + Convert.ToString(program.keys.closeKey.Item2) + ")");


            writeToWindow(StackPanelOpenKeyOutput);
            writeToWindow(StackPanelCloseKeyOutput);

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
