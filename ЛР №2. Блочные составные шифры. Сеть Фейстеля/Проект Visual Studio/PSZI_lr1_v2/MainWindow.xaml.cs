using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using PSZI_lr1;

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


    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Program program = null;
        ModeGenKey chooseModToGenKey = ModeGenKey.one;
        ModeGenFunc chooseModToFunc = ModeGenFunc.one;

        public MainWindow()
        {
            InitializeComponent();
            CheckBoxKeyN1.IsChecked = true;
            CheckBoxOF1.IsChecked = true;
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
            string text = TextBoxOriginalTextContentCC.Text;
            program.originalText = EncoderClass.StringToBitArray(text);
            Program.writeToFile("originalText.txt", text);
            writeOriginToWindow();
        }

        private void TextBoxOriginalTextContentCC2_TextChanged(object sender, RoutedEventArgs e)
        {
            //string text = TextBoxOriginalTextContentCC2.Text;
            //program.originalText = EncoderClass.BinStringToBitArray(text);
            Program.writeToFile("originalText.txt", EncoderClass.BitArrayToString(program.originalText));
            writeOriginToWindow();
        }

        // Изменение ключа
        private void TextBoxKeyCC_TextChanged(object sender, RoutedEventArgs e)
        {
            string key = TextBoxKeyCC.Text;
            program.key = EncoderClass.StringToBitArray(key);
            Program.writeToFile("key.txt", key);
            writeKeyToWindow();
        }

        private void TextBoxKeyCC2_TextChanged(object sender, RoutedEventArgs e)
        {
            //string key = TextBoxKeyCC2.Text;
            //program.key = EncoderClass.BinStringToBitArray(key);
            Program.writeToFile("key.txt", EncoderClass.BitArrayToString(program.key));
            writeKeyToWindow();
        }

        // Изменение шифротекста
        private void TextBoxCipherTextCC_TextChanged(object sender, RoutedEventArgs e)
        {
            string cipher = TextBoxCipherTextCC.Text;
            program.cipherText = EncoderClass.StringToBitArray(cipher);
            Program.writeToFile("cipherText.txt", cipher);
            //TextBoxCipherTextCC2.Text = EncoderClass.BitArrayToBinString(program.cipherText);
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
                FilenameOriginalText.Text = openFileDialog.FileName;
                program.ReadOriginalText(openFileDialog.FileName);
                writeOriginToWindow();
            }
        }

        // Открытие шифротекста из файла
        private void ButtonCipherText_Click(object sender, RoutedEventArgs e)
        {
            program.cipherText = CipherXOR.encryptText(program.originalText, program.key);
            Program.writeToFile("cipherText.txt", EncoderClass.BitArrayToString(program.cipherText));
            writeCipherToWindow();
        }

        // Запись ключа в текстовые окна
        public void writeKeyToWindow()
        {
            TextBoxKeyCC.Text = EncoderClass.BitArrayToString(program.key);
            //TextBoxKeyCC2.Text = EncoderClass.BitArrayToBinString(program.key);
            TextBoxKeyCC16.Text = EncoderClass.BitArraytoHexString(program.key);
        }

        // Запись текста в тектовые окна
        public void writeOriginToWindow()
        {
            TextBoxOriginalTextContentCC.Text = EncoderClass.BitArrayToString(program.originalText);
            //TextBoxOriginalTextContentCC2.Text = EncoderClass.BitArrayToBinString(program.originalText);
            TextBoxOriginalTextContentCC16.Text = EncoderClass.BitArraytoHexString(program.originalText);
        }

        // Запись шифротекста в текстовые окна
        public void writeCipherToWindow()
        {
            TextBoxCipherTextCC.Text = EncoderClass.BitArrayToString(program.cipherText);
            //TextBoxCipherTextCC2.Text = EncoderClass.BitArrayToBinString(program.cipherText);
            TextBoxCipherTextCC16.Text = EncoderClass.BitArraytoHexString(program.cipherText);
        }
    }
}
