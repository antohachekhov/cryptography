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


    enum ModeChooseMethod
    {
        PCBC,
        EDE
    }

    public enum ModeChoosePadding
    {
        zeros,
        ones,
        random
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Program program = null;
        ModeChooseAvalanche chooseAvalanche = ModeChooseAvalanche.originalText;
        ModeChooseMethod chooseMethod = ModeChooseMethod.PCBC;
        ModeChoosePadding choosePadding = ModeChoosePadding.zeros;

        public MainWindow()
        {
            program = new Program();
            InitializeComponent();
            CheckBoxOriginalText.IsChecked = true;
            CheckBoxPCBC.IsChecked = true;
            CheckBoxZerosPadding.IsChecked = true;
            
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

        public void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            BitArray originalTextFalse = new BitArray(program.originalText);
            BitArray originalTextTrue = new BitArray(program.originalText);

            BitArray key1False = new BitArray(GeneratorKey.ExtendedKey(program.key1));
            BitArray key1True = new BitArray(GeneratorKey.ExtendedKey(program.key1));

            BitArray key2 = new BitArray(GeneratorKey.ExtendedKey(program.key2));
            BitArray key3 = new BitArray(GeneratorKey.ExtendedKey(program.key3));

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

                int[] countBits;
                if (chooseMethod == ModeChooseMethod.PCBC)
                    // Количество измененных бит в каждом из блоков
                    countBits = program.searchAvalancheEffectForPCBC(originalTextTrue, originalTextFalse, 
                        key1False, key1True, ivFalse, ivTrue, isEncryptOrDecryptFlag);
                else
                    // Количество измененных бит в каждом из блоков
                    countBits = program.searchAvalancheEffectForEDE(originalTextTrue, originalTextFalse,
                        key1False, key1True, key2,  key3, isEncryptOrDecryptFlag);

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

            for (int i = 0; i < countOfBlocks; i++)
            {
                string fileNameCount = @".\countChanges" + Convert.ToString(i + 1) + @".txt";
                using (var sr = new StreamWriter(fileNameCount))
                {
                    sr.Write(String.Join("\n", countsOfChanges[i]));
                }
            }

            if (chooseMethod == ModeChooseMethod.PCBC)
            {
                Process.Start("..\\..\\..\\..\\main3.exe");
            }
            else
            {
                Process.Start("..\\..\\..\\..\\main1.exe");
            }
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
            chooseAvalanche = ModeChooseAvalanche.key;
            CheckBoxOriginalText.IsChecked = false;
            CheckBoxIV.IsChecked = false;
            CheckBoxCipherText.IsChecked = false;
        }

        private void ChooseIV(object sender, RoutedEventArgs e)
        {
            chooseAvalanche = ModeChooseAvalanche.iv;
            CheckBoxOriginalText.IsChecked = false;
            CheckBoxKey.IsChecked = false;
            CheckBoxCipherText.IsChecked = false;
        }

        private void ChooseCipherText(object sender, RoutedEventArgs e)
        {
            chooseAvalanche = ModeChooseAvalanche.cipherText;
            CheckBoxOriginalText.IsChecked = false;
            CheckBoxKey.IsChecked = false;
            CheckBoxIV.IsChecked = false;
        }

        private void ChooseMethodPCBC(object sender, RoutedEventArgs e)
        {
            chooseMethod = ModeChooseMethod.PCBC;
            CheckBoxEDE.IsChecked = false;
        }

        private void ChooseMethodEDE(object sender, RoutedEventArgs e)
        {
            chooseMethod = ModeChooseMethod.EDE;
            CheckBoxPCBC.IsChecked = false;
        }

        private void ChooseZerosPadding(object sender, RoutedEventArgs e)
        {
            choosePadding = ModeChoosePadding.zeros;
            CheckBoxOnesPadding.IsChecked = false;
            CheckBoxRandomPadding.IsChecked = false;

            program.modePadding = choosePadding;
        }

        private void ChooseOnesPadding(object sender, RoutedEventArgs e)
        {
            choosePadding = ModeChoosePadding.ones;
            CheckBoxZerosPadding.IsChecked = false;
            CheckBoxRandomPadding.IsChecked = false;

            program.modePadding = choosePadding;
        }

        private void ChooseRandomPadding(object sender, RoutedEventArgs e)
        {
            choosePadding = ModeChoosePadding.random;
            CheckBoxZerosPadding.IsChecked = false;
            CheckBoxOnesPadding.IsChecked = false;

            program.modePadding = choosePadding;
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
            Button button = sender as Button;
            StackPanel stackPanel = (button?.Parent) as StackPanel;

            string name = stackPanel.Name;

            int length = 0;

            if (name.Contains("Key1") || name.Contains("Key2") || name.Contains("Key3"))
                length = program.lengthKey; // биты
            else if (name.Contains("IV"))
                length = program.lengthBlock;

            BitArray randomBitArray = program.GenerateRandomBitArray(length);

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
    }
}
