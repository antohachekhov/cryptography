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
        
        private void ChooseRandom(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey = ModeGenKey.random;
            CheckBoxLFSR1.IsChecked = false;
            CheckBoxLFSR2.IsChecked = false;
        }

        private void ChooseLFSR1(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey = ModeGenKey.LFSR1;
            CheckBoxRandom.IsChecked = false;
            CheckBoxLFSR2.IsChecked = false;
        }

        private void ChooseLFSR2(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey = ModeGenKey.LFSR2;
            CheckBoxLFSR1.IsChecked = false;
            CheckBoxRandom.IsChecked = false;
        }

        private void ChooseLFSR11(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey1 = ModeGenKey.LFSR1;
            CheckBoxScr2.IsChecked = false;
        }

        private void ChooseLFSR21(object sender, RoutedEventArgs e)
        {
            chooseModToGenKey1 = ModeGenKey.LFSR2;
            CheckBoxScr1.IsChecked = false;
        }

        // генерация ключа
        private void ButtonGenKey_Click(object sender, RoutedEventArgs e)
        {
            ModeGenKey command = chooseModToGenKey; 
            program.GenerateKey(command);
            writeKeyToWindow(program.key);
        }

        // чтение ключа из файла
        private void ButtonReadKey_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                program.ReadKey(openFileDialog.FileName);
                writeKeyToWindow(program.key);
            }
        }

        // проверка сбалансированности
        private void ButtonCheckBalance_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxKeyCC.Text;
            program.calcBalance(key);
            // ВЫВОД НА ЭКРАН
        }

        // проверка цикличности
        private void ButtonCheckCycle_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxKeyCC.Text;
            program.calcСyclicality(key);
            // ВЫВОД НА ЭКРАН
        }

        // проверка корреляции
        private void ButtonCheckCorrelation_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxKeyCC.Text;
            string startShiftRegister = Program.readFromFile("startShiftRegister.txt");
            program.calcСorrelation(key, startShiftRegister);
            // ВЫВОД НА ЭКРАН
        }

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

        private void ButtonCipherText_Click(object sender, RoutedEventArgs e)
        {
            //string original = TextBoxOriginalTextContentCC.Text;
            program.cipherText = CipherXOR.encryptText(program.originalText, program.key);
            Program.writeToFile("cipherText.txt", program.cipherText);
            writeCipherToWindow(program.cipherText);
        }

        private void ButtonResearchScrambler_Click(object sender, RoutedEventArgs e)
        {
            ModeGenKey command = chooseModToGenKey1;
            string key = TextBoxBeginKeyCC.Text;
            TextBoxTScr.Text = program.calcPeriod(command, key).ToString();
            TextBoxX2.Text = program.calcChiSquare(key).ToString();
        }

        public void writeKeyToWindow(string key)
        {
            TextBoxKeyCC.Text = key;
            TextBoxKeyCC2.Text = EncoderClass.StringtoBin(key);
            TextBoxKeyCC16.Text = EncoderClass.StringtoHex(key);
        }
        public void writeOriginToWindow(string origin)
        {
            TextBoxOriginalTextContentCC.Text = origin;
            TextBoxOriginalTextContentCC2.Text = EncoderClass.StringtoBin(origin);
            TextBoxOriginalTextContentCC16.Text = EncoderClass.StringtoHex(origin);
        }
        public void writeCipherToWindow(string cipher)
        {
            TextBoxCipherTextCC.Text = cipher.ToString();
            TextBoxCipherTextCC2.Text = EncoderClass.StringtoBin(cipher);
            TextBoxCipherTextCC16.Text = EncoderClass.StringtoHex(cipher);
        }
    }
}
