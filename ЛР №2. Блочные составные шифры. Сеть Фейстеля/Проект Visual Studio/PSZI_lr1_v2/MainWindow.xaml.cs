﻿using System;
using System.Windows;
using Microsoft.Win32;
using PSZI_lr1;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace PSZI_lr1_v2
{
    enum ModeGenKey
    {
        one,
        two
    }
    enum ModeGenFunc
    {
        one,
        two
    }

    enum ModeChooseAvalanche
    {
        originalText,
        key
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Program program = null;
        ModeGenKey chooseModToGenKey = ModeGenKey.one;
        ModeGenFunc chooseModToFunc = ModeGenFunc.one;
        ModeChooseAvalanche chooseAvalanche = ModeChooseAvalanche.originalText;

        public MainWindow()
        {
            InitializeComponent();
            CheckBoxKeyN1.IsChecked = true;
            CheckBoxOF1.IsChecked = true;
            CheckBoxOriginalText.IsChecked = true;
            program = new Program();
        }

        // Выбран первый способ генерации подключей
        private void ChooseKeyN1(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey = ModeGenKey.one;
            CheckBoxKeyN2.IsChecked = false;
        }

        // Выбран второй способ генерации подключей
        private void ChooseKeyN2(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey = ModeGenKey.two;
            CheckBoxKeyN1.IsChecked = false;
        }

        // Выбрана первая образующая функция
        private void ChooseOF1(object sender, RoutedEventArgs e)
        {
            chooseModToFunc = ModeGenFunc.one;
            CheckBoxOF2.IsChecked = false;
        }

        // Выбрана вторая образующая функция
        private void ChooseOF2(object sender, RoutedEventArgs e)
        {
            chooseModToFunc = ModeGenFunc.two;
            CheckBoxOF1.IsChecked = false;
        }

        // Генерация ключа
        private void ButtonGenKey_Click(object sender, RoutedEventArgs e)
        {
            ModeGenKey commandWay = chooseModToGenKey;
            program.GenerateKey(commandWay);
            program.countRounds = Convert.ToInt32(TextBoxRound.Text);
            writeKeyToWindow();
        }

        // Чтение ключа из файла
        private void ButtonReadKey_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                program.ReadKey(openFileDialog.FileName);
                writeKeyToWindow();
            }
        }

        // Изменение оригинального текста 
        private void TextBoxOriginalTextContentCC_TextChanged(object sender, RoutedEventArgs e)
        {
            if (TextBoxOriginalTextContentCC.Text == "")
                return;
            string text = TextBoxOriginalTextContentCC.Text;
            program.originalText = EncoderClass.StringToBitArray(text);
            Program.writeToFile("originalText.txt", text);
        }

        private void TextBoxOriginalTextContentCC16_TextChanged(object sender, RoutedEventArgs e)
        {
            if (TextBoxOriginalTextContentCC16.Text == "")
                return;
            string text = TextBoxOriginalTextContentCC16.Text;
            string binString = EncoderClass.HexStringToBinString(text);
            program.originalText = EncoderClass.BinStringToBitArray(binString);
            Program.writeToFile("originalText.txt", EncoderClass.BitArrayToString(program.originalText));
        }

        // Изменение ключа
        private void TextBoxKeyCC_TextChanged(object sender, RoutedEventArgs e)
        {
            if (TextBoxKeyCC.Text == "")
                return;
            string key = TextBoxKeyCC.Text;
            program.key = EncoderClass.StringToBitArray(key);
            Program.writeToFile("key.txt", key);
            writeKeyToWindow();
        }

        private void TextBoxKeyCC16_TextChanged(object sender, RoutedEventArgs e)
        {
            if (TextBoxKeyCC16.Text == "")
                return;
            string key = TextBoxKeyCC16.Text;
            string binString = EncoderClass.HexStringToBinString(key);
            program.key = EncoderClass.BinStringToBitArray(binString);
            Program.writeToFile("key.txt", EncoderClass.BitArrayToString(program.key));
            writeKeyToWindow();
        }

        // Изменение шифротекста
        private void TextBoxCipherTextCC_TextChanged(object sender, RoutedEventArgs e)
        {
            string cipher = TextBoxCipherTextCC.Text;
            program.cipherText = EncoderClass.StringToBitArray(cipher);
            Program.writeToFile("cipherText.txt", cipher);
            TextBoxCipherTextCC16.Text = EncoderClass.BitArraytoHexString(program.cipherText);
        }

        // Открытие файла с текстом по ссылке
        private void ButtonOpenOriginalFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                FilenameOriginalText.Text = openFileDialog.FileName;
                program.ReadOriginalText(openFileDialog.FileName);
                writeOriginToWindow();
            }
        }

        // Открытие файла с ключом по ссылке
        private void ButtonOpenKey_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                FilenameKey.Text = openFileDialog.FileName;
                program.ReadKey(openFileDialog.FileName);
                writeKeyToWindow();
            }
        }

        // Шифрование
        private void ButtonCipherText_Click(object sender, RoutedEventArgs e)
        {
            decimal d;
            if (decimal.TryParse(TextBoxRound.Text, out d))
            {
                program.countRounds = (int)d;
                program.GenerateKey(chooseModToGenKey);
                program.GenerateEncryptor(chooseModToFunc);
                program.Encryption();
                Program.writeToFile("cipherText.txt", EncoderClass.BitArrayToString(program.cipherText));
                writeCipherToWindow();
            }
            else
            {
                //invalid
                MessageBox.Show("Please enter a valid number of rounds");
                return;
            }
        }


        // Шифрование
        private void ButtonDecipherText_Click(object sender, RoutedEventArgs e)
        {
            decimal d;
            if (decimal.TryParse(TextBoxRound.Text, out d))
            {
                program.countRounds = (int)d;
                program.GenerateKey(chooseModToGenKey);
                program.GenerateEncryptor(chooseModToFunc);
                program.Decryption();
                Program.writeToFile("originalText.txt", EncoderClass.BitArrayToString(program.cipherText));
                writeCipherToWindow();
            }
            else
            {
                //invalid
                MessageBox.Show("Please enter a valid number of rounds");
                return;
            }
        }


        // Подача бита от 1 до 64
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
            decimal countRounds;
            int[] a;
            if (decimal.TryParse(TextBoxRound.Text, out countRounds))
            {
                program.countRounds = (int)countRounds;
                program.GenerateKey(chooseModToGenKey);
                program.GenerateEncryptor(chooseModToFunc);
                a = program.searchAvalancheEffect(Convert.ToInt32(TextBoxChangeBit.Text), chooseAvalanche, chooseModToGenKey);
                TextBoxExitCC.Text = String.Join(" ", a);
            }
            else
            {
                //invalid
                MessageBox.Show("Please enter a valid number of rounds");
                return;
            }

            string fileNameRounds = @".\rounds.txt";
            string fileNameCount = @".\countChanges.txt";

            int[] rounds = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            using (var sr = new StreamWriter(fileNameRounds))
            {
                sr.Write(String.Join("\n", rounds));
            }

            using (var sr = new StreamWriter(fileNameCount))
            {
                sr.Write(String.Join("\n", a));
            }

            Process.Start("..\\..\\..\\..\\main.exe");


        }


        // Запись ключа в текстовые окна
        public void writeKeyToWindow()
        {
            TextBoxKeyCC.Text = EncoderClass.BitArrayToString(program.key);
            TextBoxKeyCC16.Text = EncoderClass.BitArraytoHexString(program.key);
        }

        // Запись текста в тектовые окна
        public void writeOriginToWindow()
        {
            TextBoxOriginalTextContentCC.Text = EncoderClass.BitArrayToString(program.originalText);
            TextBoxOriginalTextContentCC16.Text = EncoderClass.BitArraytoHexString(program.originalText);
        }

        // Запись шифротекста в текстовые окна
        public void writeCipherToWindow()
        {
            TextBoxCipherTextCC.Text = EncoderClass.BitArrayToString(program.cipherText);
            TextBoxCipherTextCC16.Text = EncoderClass.BitArraytoHexString(program.cipherText);
        }

        private void TextBoxOriginalTextContentCC_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxOriginalTextContentCC.TextChanged += TextBoxOriginalTextContentCC_TextChanged;
        }

        private void TextBoxOriginalTextContentCC_LostFocus(object sender, RoutedEventArgs e)
        {
            writeOriginToWindow();
            TextBoxOriginalTextContentCC.TextChanged -= TextBoxOriginalTextContentCC_TextChanged;
        }
        private void TextBoxOriginalTextContentCC16_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxOriginalTextContentCC16.TextChanged += TextBoxOriginalTextContentCC16_TextChanged;
        }

        private void TextBoxOriginalTextContentCC16_LostFocus(object sender, RoutedEventArgs e)
        {
            writeOriginToWindow();
            TextBoxOriginalTextContentCC16.TextChanged -= TextBoxOriginalTextContentCC16_TextChanged;
        }

        private void ChooseOriginalText(object sender, RoutedEventArgs e)
        {
            
            chooseAvalanche = ModeChooseAvalanche.originalText;
            CheckBoxKey.IsChecked = false;
        }

        private void ChooseKey(object sender, RoutedEventArgs e)
        {
            CheckBoxOriginalText.IsChecked = false;
            chooseAvalanche = ModeChooseAvalanche.key;
        }
    }
}
