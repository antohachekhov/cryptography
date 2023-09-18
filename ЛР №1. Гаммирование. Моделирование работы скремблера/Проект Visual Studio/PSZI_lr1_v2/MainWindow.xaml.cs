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
        random,
        LFSR1,
        LFSR2
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Program program = null;
        ModeGenKey chooseModToGenKey = ModeGenKey.random;
        ModeGenKey chooseModToGenKey1 = ModeGenKey.random;

        public MainWindow()
        {
            InitializeComponent();
            program = new Program();
        }
        
        // Выбран случайный ключ
        private void ChooseRandom(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey = ModeGenKey.random;
            CheckBoxLFSR1.IsChecked = false;
            CheckBoxLFSR2.IsChecked = false;
        }

        // Выбран первый скремблер
        private void ChooseLFSR1(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey = ModeGenKey.LFSR1;
            CheckBoxRandom.IsChecked = false;
            CheckBoxLFSR2.IsChecked = false;
        }

        // Выбран второй скремблер
        private void ChooseLFSR2(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey = ModeGenKey.LFSR2;
            CheckBoxLFSR1.IsChecked = false;
            CheckBoxRandom.IsChecked = false;
        }

        // Выбран первый скремблер в исследовании
        private void ChooseLFSR11(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey1 = ModeGenKey.LFSR1;
            CheckBoxScr2.IsChecked = false;
        }

        // Выбран второй скремблер в исследовании
        private void ChooseLFSR21(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey1 = ModeGenKey.LFSR2;
            CheckBoxScr1.IsChecked = false;
        }

        // Генерация ключа
        private void ButtonGenKey_Click(object sender, RoutedEventArgs e)
        {
            ModeGenKey command = chooseModToGenKey;
            program.GenerateKey(command, EncoderClass.StringToBitArray(TextBoxScrCC.Text));
            writeKeyToWindow();
            if (command != ModeGenKey.random)
                writeShiftToWindow();
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
            program.ReadScr("startShiftRegister.txt");
            writeShiftToWindow();
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
            string text = TextBoxOriginalTextContentCC2.Text;
            program.originalText = EncoderClass.BinStringToBitArray(text);
            Program.writeToFile("originalText.txt", text);
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
            string key = TextBoxKeyCC.Text;
            program.key = EncoderClass.BinStringToBitArray(key);
            Program.writeToFile("key.txt", key);
            writeKeyToWindow();
        }

        // Изменение начального значения скремблера
        private void TextBoxScrCC_TextChanged(object sender, RoutedEventArgs e)
        {
            string scr = TextBoxScrCC.Text;
            program.startshift = EncoderClass.StringToBitArray(scr);
            Program.writeToFile("startShiftRegister.txt", scr);
            writeShiftToWindow();
        }

        private void TextBoxScrCC2_TextChanged(object sender, RoutedEventArgs e)
        {
            string scr = TextBoxScrCC.Text;
            program.startshift = EncoderClass.BinStringToBitArray(scr);
            Program.writeToFile("startShiftRegister.txt", scr);
            writeShiftToWindow();
        }

        // Изменение шифротекста
        private void TextBoxCipherTextCC_TextChanged(object sender, RoutedEventArgs e)
        {
            string cipher = TextBoxCipherTextCC.Text;
            program.cipherText = EncoderClass.StringToBitArray(cipher);
            Program.writeToFile("cipherText.txt", cipher);
            TextBoxCipherTextCC2.Text = EncoderClass.BitArrayToBinString(program.cipherText);
            TextBoxCipherTextCC16.Text = EncoderClass.BitArraytoHexString(program.cipherText);
        }

        // Проверка сбалансированности
        private void ButtonCheckBalance_Click(object sender, RoutedEventArgs e)
        {
            TextBoxKeyBal.Text = program.calcBalance(program.key).ToString();
        }

        // Проверка цикличности
        private void ButtonCheckCycle_Click(object sender, RoutedEventArgs e)
        {
            List<double> cl = program.calcСyclicality(program.key);
            string clT = "";
            for (int i = 0; i < cl.Count; i++)
                clT = clT + cl[i] + " ";
            TextBoxKeyCycle.Text = clT;
        }

        // Проверка корреляции
        private void ButtonCheckCorrelation_Click(object sender, RoutedEventArgs e)
        {
            TextBoxKeyCorr.Text = program.calcСorrelation(chooseModToGenKey, program.key).ToString();
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

        // Открытие шифротекста из файла
        private void ButtonCipherText_Click(object sender, RoutedEventArgs e)
        {
            program.cipherText = CipherXOR.encryptText(program.originalText, program.key);
            Program.writeToFile("cipherText.txt", EncoderClass.BitArrayToString(program.cipherText));
            writeCipherToWindow();
        }

        // Исследование скремблера
        private void ButtonResearchScrambler_Click(object sender, RoutedEventArgs e)
        {
            ModeGenKey command = chooseModToGenKey1;
            BitArray keyToBitArray = EncoderClass.StringToBitArray(TextBoxBeginKeyCC.Text);
            TextBoxTScr.Text = program.calcPeriod(command, keyToBitArray).ToString();
            TextBoxX2.Text = program.calcChiSquare(keyToBitArray).ToString();
        }

        // Запись ключа в текстовые окна
        public void writeKeyToWindow()
        {
            TextBoxKeyCC.Text = EncoderClass.BitArrayToString(program.key);
            TextBoxKeyCC2.Text = EncoderClass.BitArrayToBinString(program.key);
            TextBoxKeyCC16.Text = EncoderClass.BitArraytoHexString(program.key);
        }

        // Запись начального значения скремблера в текстовые окна
        public void writeShiftToWindow()
        {
            TextBoxScrCC.Text = EncoderClass.BitArrayToString(program.startshift);
            TextBoxScrCC2.Text = EncoderClass.BitArrayToBinString(program.startshift);
            TextBoxScrCC16.Text = EncoderClass.BitArraytoHexString(program.startshift);
        }

        // Запись текста в тектовые окна
        public void writeOriginToWindow()
        {
            TextBoxOriginalTextContentCC.Text = EncoderClass.BitArrayToString(program.originalText);
            TextBoxOriginalTextContentCC2.Text = EncoderClass.BitArrayToBinString(program.originalText);
            TextBoxOriginalTextContentCC16.Text = EncoderClass.BitArraytoHexString(program.originalText);
        }

        // Запись шифротекста в текстовые окна
        public void writeCipherToWindow()
        {
            TextBoxCipherTextCC.Text = EncoderClass.BitArrayToString(program.cipherText);
            TextBoxCipherTextCC2.Text = EncoderClass.BitArrayToBinString(program.cipherText);
            TextBoxCipherTextCC16.Text = EncoderClass.BitArraytoHexString(program.cipherText);
        }
    }
}
