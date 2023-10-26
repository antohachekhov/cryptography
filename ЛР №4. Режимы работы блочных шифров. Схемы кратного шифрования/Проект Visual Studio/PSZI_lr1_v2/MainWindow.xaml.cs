using System;
using System.Windows;
using Microsoft.Win32;
using PSZI_lr1;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Controls;
using System.Linq;

namespace PSZI_lr1_v2
{

    enum ModeChooseAvalanche
    {
        originalText,
        key,
        iv, 
        cipherText
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Program program = null;
        ModeChooseAvalanche chooseAvalanche = ModeChooseAvalanche.originalText;

        public MainWindow()
        {
            InitializeComponent();
            CheckBoxOriginalText.IsChecked = true;
            program = new Program();
        }

        private void setValueToProgramBitArray(string name, BitArray value)
        {
            if (name.Contains("OriginalText"))
                program.originalText = value;
            else if (name.Contains("Key1"))
                program.key1 = value;
            else if (name.Contains("Key2"))
                program.key2 = value;
            else if (name.Contains("Key3"))
                program.key3 = value;
            else if (name.Contains("IV"))
                program.iv = value;
        }

        private void writeToWindow(StackPanel stackPanel)
        {

            string name = stackPanel.Name;
            BitArray value = new BitArray(0);
            if (name.Contains("OriginalText"))
                value = program.originalText;
            else if (name.Contains("Key1"))
                value = program.key1;
            else if (name.Contains("Key2"))
                value = program.key2;
            else if (name.Contains("Key3"))
                value = program.key3;
            else if (name.Contains("IV"))
                value = program.iv;
            else if (name.Contains("CipherText"))
                value = program.cipherText;

            string textCC = EncoderClass.BitArrayToString(value);
            string textCC16 = EncoderClass.BitArraytoHexString(value);
            stackPanel.Children.OfType<TextBox>().First().Text = textCC;
            stackPanel.Children.OfType<TextBox>().Last().Text = textCC16;
        }

        // Изменение ключа
        private void TextBoxCC_TextChanged(object sender, RoutedEventArgs e)
        {
            // Определение инициатора
            TextBox textBoxCC = sender as TextBox;
            StackPanel stackPanel = textBoxCC?.Parent as StackPanel;

            BitArray bitArrayFromText = EncoderClass.StringToBitArray(textBoxCC.Text);

            // Изменение значений в cc 16
            TextBox textBoxCC16 = stackPanel.Children.OfType<TextBox>().Last();
            textBoxCC16.Text = EncoderClass.BitArraytoHexString(bitArrayFromText);

            // Запись в переменную
            setValueToProgramBitArray(stackPanel.Name, bitArrayFromText);

            // Запись в файл
            Program.writeToFile(stackPanel.Name + ".txt", textBoxCC.Text);
        }

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
            setValueToProgramBitArray(stackPanel.Name, bitArrayFromText);

            // Запись в файл
            Program.writeToFile(stackPanel.Name + ".txt", textBoxCC.Text);
        }

        
        // Шифрование
        private void ButtonEncrypte_Click(object sender, RoutedEventArgs e)
        {
            program.GenerateKey();
            program.Encryption();
            Program.writeToFile("cipherText.txt", EncoderClass.BitArrayToString(program.cipherText));
            writeToWindow(StackPanelCipherTextOutput);
            timeOfEncoding.Text = program.timeOfEncoding.ToString() + " мс";
        }

        // Дешифрование
        private void ButtonDecrypte_Click(object sender, RoutedEventArgs e)
        {
            program.GenerateKey();
            program.Decryption();
            Program.writeToFile("originalText.txt", EncoderClass.BitArrayToString(program.cipherText));
            writeToWindow(StackPanelCipherTextOutput);
        }

        public void TextBoxChangeBit_TextChanged(object sender, RoutedEventArgs e)
        {
            decimal d;
            if (decimal.TryParse(TextBoxChangeBit.Text, out d))
            {
                if (d > 0 && d < 65)
                {

                }
                else
                {
                    TextBoxChangeBit.Text = "1";
                    throw new Exception();

                }

            }
            else
            {
                //invalid
                MessageBox.Show("Please enter a valid number of rounds");
                return;
            }
        }

        public void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            BitArray originalTextFalse = new BitArray(program.originalText);
            BitArray originalTextTrue = new BitArray(program.originalText);

            BitArray key1False = new BitArray(GeneratorKey.ExtendedKey(program.key1));
            BitArray key1True = new BitArray(GeneratorKey.ExtendedKey(program.key1));

            BitArray ivFalse = new BitArray(program.iv);
            BitArray ivTrue = new BitArray(program.iv);

            bool isEncryptOrDecryptFlag = true; // true - если шифрование, false - если дешифровка

            // Подсчет длины зависимостей
            int count = 0;
            if (chooseAvalanche == ModeChooseAvalanche.originalText || chooseAvalanche == ModeChooseAvalanche.cipherText)
                count = program.originalText.Length;
            else if (chooseAvalanche == ModeChooseAvalanche.key)
                count = program.key1.Length;
            else if(chooseAvalanche == ModeChooseAvalanche.iv)
                count = program.iv.Length;




            
            int countOfBlocks = program.DividingTextIntoBlocks(program.originalText).Length; // вообще всегда должно быть 3
            
            // Инициализация осей
            int[][] countsOfChanges = new int[countOfBlocks][]; // оси Y

            for (int i = 0; i < countOfBlocks; i++)
            {
                countsOfChanges[i] = new int[count];
            }

            int[] positions = new int[count];   // ось X

            // Заполнение зависимостей
            for (int index = 0; index < count; index++)
            {
                
                // Замена бита
                if (chooseAvalanche == ModeChooseAvalanche.originalText)
                {
                    isEncryptOrDecryptFlag = true;
                    originalTextFalse[index] = false;
                    originalTextTrue[index] = true;

                }
                // Изменение бита в ключе будет происходить только в первом ключе
                else if (chooseAvalanche == ModeChooseAvalanche.key)
                {
                    isEncryptOrDecryptFlag = true;
                    key1False[index] = false;
                    key1True[index] = true;
                }
                else if (chooseAvalanche == ModeChooseAvalanche.iv)
                {
                    isEncryptOrDecryptFlag = true;
                    ivFalse[index] = false;
                    ivTrue[index] = true;
                }
                else if (chooseAvalanche == ModeChooseAvalanche.cipherText)
                {
                    isEncryptOrDecryptFlag = false;
                    originalTextFalse[index] = false;
                    originalTextTrue[index] = true;
                }

                // Количество измененных бит в каждом из блоков
                int[] countBits = program.searchAvalancheEffectForPCBC(originalTextTrue, originalTextFalse, 
                    key1False, key1True, ivFalse, ivTrue, isEncryptOrDecryptFlag);

                // Запись в оси
                positions[index] = index;
                for (int i = 0; i < countOfBlocks; i++)
                {
                    countsOfChanges[i][index] = countBits[i];
                }
            }

            string fileNameRounds = @".\rounds.txt";

            using (var sr = new StreamWriter(fileNameRounds))
            {
                sr.Write(String.Join("\n", positions));
            }

            for(int i = 0; i < countOfBlocks; i++)
            {
                string fileNameCount = @".\countChanges" + Convert.ToString(i + 1) + @".txt";
                using (var sr = new StreamWriter(fileNameCount))
                {
                    sr.Write(String.Join("\n", countsOfChanges[i]));
                }
            }

            Process.Start("..\\..\\..\\..\\main.exe");
        }

        private void ChooseOriginalText(object sender, RoutedEventArgs e)
        {

            chooseAvalanche = ModeChooseAvalanche.originalText;
            CheckBoxKey.IsChecked = false;
            CheckBoxIV.IsChecked = false;
            CheckBoxCipherText.IsChecked = false;
        }

        private void ChooseKey(object sender, RoutedEventArgs e)
        {
            CheckBoxOriginalText.IsChecked = false;
            chooseAvalanche = ModeChooseAvalanche.key;
            CheckBoxIV.IsChecked = false;
            CheckBoxCipherText.IsChecked = false;
        }

        private void ChooseIV(object sender, RoutedEventArgs e)
        {
            CheckBoxOriginalText.IsChecked = false;
            CheckBoxKey.IsChecked = false;
            chooseAvalanche = ModeChooseAvalanche.iv;
            CheckBoxCipherText.IsChecked = false;
        }

        private void ChooseCipherText(object sender, RoutedEventArgs e)
        {
            CheckBoxOriginalText.IsChecked = false;
            CheckBoxKey.IsChecked = false;
            CheckBoxIV.IsChecked = false;
            chooseAvalanche = ModeChooseAvalanche.cipherText;
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
                BitArray bitArrayFromText = EncoderClass.StringToBitArray(text);

                // Запись в переменную
                setValueToProgramBitArray(stackPanel.Name, bitArrayFromText);

                writeToWindow((stackPanel.Parent as StackPanel).Children.OfType<StackPanel>().Last());
            }
        }

        

        private void ButtonRandomGenerate_Click(object sender, RoutedEventArgs e)
        {

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

        private void ChoosePCBC(object sender, RoutedEventArgs e)
        {

        }

        private void ChooseEDE(object sender, RoutedEventArgs e)
        {

        }
    }
}
