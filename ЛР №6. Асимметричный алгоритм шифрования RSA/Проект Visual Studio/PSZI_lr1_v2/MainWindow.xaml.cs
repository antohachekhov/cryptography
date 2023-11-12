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
                BigInteger n = BigInteger.Parse(partTexts[3]);

                program.keys.openKey = (e, n);
            }
            else if (name.Contains("CloseKey"))
            {
                string[] partTexts = text.Split('(', ',', ')');

                BigInteger d = BigInteger.Parse(partTexts[1]);
                BigInteger n = BigInteger.Parse(partTexts[3]);

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
                BigInteger.Parse("680564733841876926927715055360727278377"),
                BigInteger.Parse("340282366920938463471724603615805326309"),
                BigInteger.Parse("340282366920938463471724603615805326673"),
                BigInteger.Parse("910282376920938463463568564727192087897"),
                BigInteger.Parse("910282376920938463463568564727192087757"),
            };

            int i = program.random.Next(arr.Length);

            return arr[i];
        }

    }
}
