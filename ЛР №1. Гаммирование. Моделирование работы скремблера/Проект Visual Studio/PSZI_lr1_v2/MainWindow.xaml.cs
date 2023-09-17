using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            program.GenerateKey(command);
            writeKeyToWindow(program.key);
            writeShiftToWindow(program.startshift);
        }

        // Чтение ключа из файла
        private void ButtonReadKey_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                program.ReadKey(openFileDialog.FileName);
                writeKeyToWindow(program.key);
            }
            program.ReadScr("startShiftRegister.txt");
            writeShiftToWindow(program.startshift);
        }

        // Изменение оригинального текста 
        private void TextBoxOriginalTextContentCC_TextChanged(object sender, RoutedEventArgs e)
        {
            string text = TextBoxOriginalTextContentCC.Text;
            Program.writeToFile("originalText.txt", text);
            TextBoxOriginalTextContentCC2.Text = Program.toBin(text);
            TextBoxOriginalTextContentCC16.Text = Program.toHex(text);
        }

        // Изменение ключа
        private void TextBoxKeyCC_TextChanged(object sender, RoutedEventArgs e)
        {
            string key = TextBoxKeyCC.Text;
            Program.writeToFile("key.txt", key);
            TextBoxKeyCC2.Text = Program.toBin(key);
            TextBoxKeyCC16.Text = Program.toHex(key);
        }

        // Изменение начального значения скремблера
        private void TextBoxScrCC_TextChanged(object sender, RoutedEventArgs e)
        {
            string scr = TextBoxScrCC.Text;
            Program.writeToFile("startShiftRegister.txt", scr);
            TextBoxScrCC2.Text = Program.toBin(scr);
            TextBoxScrCC16.Text = Program.toHex(scr);
        }

        // Изменение шифротекста
        private void TextBoxCipherTextCC_TextChanged(object sender, RoutedEventArgs e)
        {
            string cipher = TextBoxCipherTextCC.Text;
            Program.writeToFile("cipherText.txt", cipher);
            TextBoxCipherTextCC2.Text = Program.toBin(cipher);
            TextBoxCipherTextCC16.Text = Program.toHex(cipher);
        }

        // Проверка сбалансированности
        private void ButtonCheckBalance_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxKeyCC.Text;
            TextBoxKeyBal.Text = program.calcBalance(key).ToString();
        }

        // Проверка цикличности
        private void ButtonCheckCycle_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxKeyCC.Text;
            List<double> cl = program.calcСyclicality(key);
            string clT = "";
            for (int i = 0; i < cl.Count; i++)
                clT = clT + cl[i] + " ";
            TextBoxKeyCycle.Text = clT;
        }

        // Проверка корреляции
        private void ButtonCheckCorrelation_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxKeyCC.Text;
            string startShiftRegister = Program.readFromFile("startShiftRegister.txt");
            TextBoxKeyCorr.Text = program.calcСorrelation(key, startShiftRegister).ToString();
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
                writeOriginToWindow(program.originalText);
                //writeOriginToWindow("S");
            }
        }

        // Открытие шифротекста из файла
        private void ButtonCipherText_Click(object sender, RoutedEventArgs e)
        {
            //string original = TextBoxOriginalTextContentCC.Text;
            program.cipherText = CipherXOR.encryptText(program.originalText, program.key);
            Program.writeToFile("cipherText.txt", program.cipherText);
            writeCipherToWindow(program.cipherText);
        }

        // Исследование скремблера
        private void ButtonResearchScrambler_Click(object sender, RoutedEventArgs e)
        {
            ModeGenKey command = chooseModToGenKey1;
            string key = TextBoxBeginKeyCC.Text;
            TextBoxTScr.Text = program.calcPeriod(command, key).ToString();
            TextBoxX2.Text = program.calcChiSquare(key).ToString();
        }

        // Запись ключа в текстовые окна
        public void writeKeyToWindow(string key)
        {
            TextBoxKeyCC.Text = key;
            TextBoxKeyCC2.Text = Program.toBin(key);
            TextBoxKeyCC16.Text = Program.toHex(key);
        }

        // Запись начального значения скремблера в текстовые окна
        public void writeShiftToWindow(string reg)
        {
            TextBoxScrCC.Text = reg;
            TextBoxScrCC2.Text = Program.toBin(reg);
            TextBoxScrCC16.Text = Program.toHex(reg);
        }

        // Запись текста в тектовые окна
        public void writeOriginToWindow(string origin)
        {
            TextBoxOriginalTextContentCC.Text = origin;
            TextBoxOriginalTextContentCC2.Text = Program.toBin(origin);
            TextBoxOriginalTextContentCC16.Text = Program.toHex(origin);
        }

        // Запись шифротекста в текстовые окна
        public void writeCipherToWindow(string cipher)
        {
            TextBoxCipherTextCC.Text = cipher.ToString();
            TextBoxCipherTextCC2.Text = Program.toBin(cipher);
            TextBoxCipherTextCC16.Text = Program.toHex(cipher).ToString();
        }
    }
}
