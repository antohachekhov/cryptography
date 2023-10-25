using System;
using System.Windows;
using Microsoft.Win32;
using PSZI_lr1;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace PSZI_lr1_v2
{

    enum ModeChooseAvalanche
    {
        originalText,
        key,
        vi, 
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



        // Генерация ключа
        private void ButtonGenKey_Click(object sender, RoutedEventArgs e)
        {
            program.GenerateKey();
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

        // Чтение вектора инициализации из файла
        private void ButtonReadVI_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                program.ReadVI(openFileDialog.FileName);
                writeVIToWindow();
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
        }

        private void TextBoxKeyCC16_TextChanged(object sender, RoutedEventArgs e)
        {
            if (TextBoxKeyCC16.Text == "")
                return;
            string key = TextBoxKeyCC16.Text;
            string binString = EncoderClass.HexStringToBinString(key);
            program.key = EncoderClass.BinStringToBitArray(binString);
            Program.writeToFile("key.txt", EncoderClass.BitArrayToString(program.key));
        }

        // Изменение вектора инициализации
        private void TextBoxVICC_TextChanged(object sender, RoutedEventArgs e)
        {
            if (TextBoxVICC.Text == "")
                return;
            string vi = TextBoxVICC.Text;
            program.vi = EncoderClass.StringToBitArray(vi);
            Program.writeToFile("vi.txt", vi);
        }

        private void TextBoxVICC16_TextChanged(object sender, RoutedEventArgs e)
        {
            if (TextBoxVICC16.Text == "")
                return;
            string vi = TextBoxVICC16.Text;
            string binString = EncoderClass.HexStringToBinString(vi);
            program.vi = EncoderClass.BinStringToBitArray(binString);
            Program.writeToFile("vi.txt", EncoderClass.BitArrayToString(program.vi));
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

        // Открытие файла с вектором инициализации по ссылке
        private void ButtonOpenVI_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                FilenameVI.Text = openFileDialog.FileName;
                program.ReadVI(openFileDialog.FileName);
                writeVIToWindow();
            }
        }

        // Шифрование
        private void ButtonCipherText_Click(object sender, RoutedEventArgs e)
        {
            program.Encryption();
            Program.writeToFile("cipherText.txt", EncoderClass.BitArrayToString(program.cipherText));
            writeCipherToWindow();
            timeOfEncoding.Text = program.timeOfEncoding.ToString() + " мс";
        }


        // Шифрование
        private void ButtonDecipherText_Click(object sender, RoutedEventArgs e)
        {

            program.Decryption();
            Program.writeToFile("originalText.txt", EncoderClass.BitArrayToString(program.cipherText));
            writeCipherToWindow();

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
            int index = Convert.ToInt32(TextBoxChangeBit.Text) - 1;
            BitArray[] Xs = program.DividingTextIntoBlocks(program.originalText);
            BitArray Xfalse = new BitArray(Xs[0]);
            BitArray Xtrue = new BitArray(Xs[0]);

            BitArray keyFalse = new BitArray(GeneratorKey.ExtendedKey(program.key));
            BitArray keyTrue = new BitArray(GeneratorKey.ExtendedKey(program.key));

            int[,] MDepFalse;
            int[,] MDisFalse;

            int[,] MDepTrue;
            int[,] MDisTrue;

            if (chooseAvalanche == ModeChooseAvalanche.originalText)
            {

            }
            else if (chooseAvalanche == ModeChooseAvalanche.key)
            {

            }
            else if (chooseAvalanche == ModeChooseAvalanche.vi)
            {

            }
            else if (chooseAvalanche == ModeChooseAvalanche.cipherText)
            {

            }

            MDepFalse = program.matrixDependence(Xfalse, keyFalse);
            MDisFalse = program.matrixDistances(Xfalse, keyFalse);

            MDepTrue = program.matrixDependence(Xtrue, keyTrue);
            MDisTrue = program.matrixDistances(Xtrue, keyTrue);


            for (int i =0; i < MDisFalse.GetLength(0); i++)
            {
                Console.WriteLine("{" + "\t");
                for (int j = 0; j < MDisFalse.GetLength(1); j++)
                {
                    Console.Write(MDisFalse[i, j] + "\t");
                }
                Console.Write("}" + "\t");
            }

            TextBoxMeanBitFalse.Text = program.criteria1(MDisFalse).ToString();
            TextBoxStFullFalse.Text = program.criteria2(MDepFalse).ToString();
            TextBoxStLavEffFalse.Text = program.criteria3(MDisFalse).ToString();
            TextBoxStStrongFalse.Text = program.criteria4(MDepFalse).ToString();

            TextBoxMeanBitTrue.Text = program.criteria1(MDisTrue).ToString();
            TextBoxStFullTrue.Text = program.criteria2(MDepTrue).ToString();
            TextBoxStLavEffTrue.Text = program.criteria3(MDisTrue).ToString();
            TextBoxStStrongTrue.Text = program.criteria4(MDepTrue).ToString();



            int[] countBits = program.searchAvalancheEffect(Xfalse, index, chooseAvalanche);

            string fileNameRounds = @".\rounds.txt";
            string fileNameCount = @".\countChanges.txt";

            int[] rounds = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            using (var sr = new StreamWriter(fileNameRounds))
            {
                sr.Write(String.Join("\n", rounds));
            }

            using (var sr = new StreamWriter(fileNameCount))
            {
                sr.Write(String.Join("\n", countBits));
            }

            Process.Start("..\\..\\..\\..\\main.exe");


        }


        // Запись ключа в текстовые окна
        public void writeKeyToWindow()
        {
            TextBoxKeyCC.Text = EncoderClass.BitArrayToString(program.key);
            TextBoxKeyCC16.Text = EncoderClass.BitArraytoHexString(program.key);
        }

        // Запись вектор инициализации в текстовые окна
        public void writeVIToWindow()
        {
            TextBoxVICC.Text = EncoderClass.BitArrayToString(program.vi);
            TextBoxVICC16.Text = EncoderClass.BitArraytoHexString(program.vi);
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
            CheckBoxVI.IsChecked = false;
            CheckBoxCipherText.IsChecked = false;
        }

        private void ChooseKey(object sender, RoutedEventArgs e)
        {
            CheckBoxOriginalText.IsChecked = false;
            chooseAvalanche = ModeChooseAvalanche.key;
            CheckBoxVI.IsChecked = false;
            CheckBoxCipherText.IsChecked = false;
        }

        private void ChooseVI(object sender, RoutedEventArgs e)
        {
            CheckBoxOriginalText.IsChecked = false;
            CheckBoxKey.IsChecked = false;
            chooseAvalanche = ModeChooseAvalanche.vi;
            CheckBoxCipherText.IsChecked = false;
        }

        private void ChooseCipherText(object sender, RoutedEventArgs e)
        {
            CheckBoxOriginalText.IsChecked = false;
            CheckBoxKey.IsChecked = false;
            CheckBoxVI.IsChecked = false;
            chooseAvalanche = ModeChooseAvalanche.cipherText;
        }

        private void ButtonGenerateBelowKeys_Click(object sender, RoutedEventArgs e)
        {

            program.FillKeys();
            
        }


        private void ButtonShowBelowKeys_Click(object sender, RoutedEventArgs e)
        {
            if (StackPanelBelowKeys.Visibility == Visibility.Collapsed)
            {

                StackPanelBelowKeys.Visibility = Visibility.Visible;
                ButtonShowBelowKeys.Content = "Закрыть подключи";
                int j = 0;
                for (int i = 0; i < 16; i++)
                {
                    j = i + 1;
                    if (i < 8)
                        LB1.Items.Add(j + "\t" + EncoderClass.BitArraytoHexString(program.belowKeys[i]));
                    else
                        LB2.Items.Add(j + "\t" + EncoderClass.BitArraytoHexString(program.belowKeys[i]));
                }
            }
            else
            {
                StackPanelBelowKeys.Visibility = Visibility.Collapsed;
                ButtonShowBelowKeys.Content = "Посмотреть подключи";
                LB1.Items.Clear();
                LB2.Items.Clear();
            }
        }

        private void TextBoxKeyCC_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxKeyCC.TextChanged += TextBoxKeyCC_TextChanged;
        }

        private void TextBoxKeyCC_LostFocus(object sender, RoutedEventArgs e)
        {
            writeKeyToWindow();
            TextBoxKeyCC.TextChanged -= TextBoxKeyCC_TextChanged;
        }
        private void TextBoxKeyCC16_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxKeyCC16.TextChanged += TextBoxKeyCC16_TextChanged;
        }

        private void TextBoxKeyCC16_LostFocus(object sender, RoutedEventArgs e)
        {
            writeKeyToWindow();
            TextBoxKeyCC16.TextChanged -= TextBoxKeyCC16_TextChanged;
        }

        // вектор инициализации
        private void TextBoxVICC_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxVICC.TextChanged += TextBoxVICC_TextChanged;
        }

        private void TextBoxVICC_LostFocus(object sender, RoutedEventArgs e)
        {
            writeVIToWindow();
            TextBoxVICC.TextChanged -= TextBoxVICC_TextChanged;
        }
        private void TextBoxVICC16_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxVICC16.TextChanged += TextBoxVICC16_TextChanged;
        }

        private void TextBoxVICC16_LostFocus(object sender, RoutedEventArgs e)
        {
            writeVIToWindow();
            TextBoxVICC16.TextChanged -= TextBoxVICC16_TextChanged;
        }

        private void ButtonGenerateKey_Click(object sender, RoutedEventArgs e)
        {
            // генерация ключа рандом
        }
    }
}
