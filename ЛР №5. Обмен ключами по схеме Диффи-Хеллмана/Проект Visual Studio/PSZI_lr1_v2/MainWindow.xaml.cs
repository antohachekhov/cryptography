using System;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
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

        // сгенерировать простое число
        private void ButtonGeneratePN_Click(object sender, RoutedEventArgs e)
        {
            int t = Convert.ToInt32(TextBoxT.Text);
            int n = Convert.ToInt32(TextBoxN1.Text);

            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();
            NumberWithNumIters result = program.generateSimpleNumberByN(n, t);
            //останавливаем счётчик
            stopwatch.Stop();

            long time = stopwatch.ElapsedMilliseconds;
            TextBoxPN.Text = Convert.ToString(result.number); // простое число
            TextBoxIN.Text = Convert.ToString(result.numIters); // количество итераций
            TextBoxTime.Text = Convert.ToString(time) + " мс"; // время
        }

        // вывод простых чисел из диапазона
        private void ButtonShowPN_Click(object sender, RoutedEventArgs e)
        {

            ListBoxPrimeNumbers.Items.Clear();
            int t = Convert.ToInt32(TextBoxT.Text);
            BigInteger min = BigInteger.Parse(TextBoxMin.Text);
            BigInteger max = BigInteger.Parse(TextBoxMax.Text);

            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();
            List<BigInteger> result = program.generateAllSimpleNumbersFromRange(min, max, t);
            //останавливаем счётчик
            stopwatch.Stop();

            long time = stopwatch.ElapsedMilliseconds;

            for (int i = 0, j; i < result.Count; i++)
            {
                j = i + 1;
                ListBoxPrimeNumbers.Items.Add(j + "\t" + result[i].ToString());
            }

            TextBoxTime2.Text = Convert.ToString(time) + " мс";
        }

        // получить первообразные корни
        private void ButtonGetPR_Click(object sender, RoutedEventArgs e)
        {
            ListBoxPrimitiveRoots.Items.Clear();
            int t = Convert.ToInt32(TextBoxT.Text);
            BigInteger number = BigInteger.Parse(TextBoxN2.Text);

            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();
            List<BigInteger> result = program.getPrimitiveRoots(number, t);
            //останавливаем счётчик
            stopwatch.Stop();
            long time = stopwatch.ElapsedMilliseconds;

            long sumTime = time;

            for (int i = 0; i < result.Count; i++)
            {
                int j = i + 1;
                ListBoxPrimitiveRoots.Items.Add(j + "\t" + result[i].ToString());
            }

            TextBoxTime3.Text = Convert.ToString(sumTime) + " мс";
        }

        // обменять ключи
        private void ButtonSendKeys_Click(object sender, RoutedEventArgs e)
        {
            int n = Convert.ToInt32(TextBoxN3.Text);
            int g = Convert.ToInt32(TextBoxG.Text);

            if (g >= n)
            {
                MessageBox.Show("g должно быть меньше, чем n");
                return;
            }

            BigInteger xa = BigInteger.Parse(TextBoxXA.Text);
            BigInteger xb = BigInteger.Parse(TextBoxXB.Text);
            
            TextBoxYA.Text = program.generateY(g, xa, n).ToString();
            TextBoxYB.Text = program.generateY(g, xb, n).ToString();
            TextBlock2Step.Text = "2) Вычисление чисел";


            int ya = Convert.ToInt32(TextBoxYA.Text);
            int yb = Convert.ToInt32(TextBoxYB.Text);

            TextBoxKA.Text = program.generateY(yb, xa, n).ToString();
            TextBoxKB.Text = program.generateY(ya, xb, n).ToString();
            TextBlock3Step.Text = "3) Вычисление секретного ключа";

        }

        private void ButtonRandomN_Click(object sender, RoutedEventArgs e)
        {
            int t = Convert.ToInt32(TextBoxT.Text);
           // BitArray n = (BitArray)program.random.Next(2, 8 * EncoderClass.byteCountLong);
            //TextBoxN3.Text = EncoderClass.BitArrayToString(n);
        }

        private void ButtonRandomX_Click(object sender, RoutedEventArgs e)
        {
            int t = Convert.ToInt32(TextBoxT.Text);
            int n = Convert.ToInt32(TextBoxN3.Text);

            NumberWithNumIters xa = program.generateSimpleNumberByN(n, t);
            NumberWithNumIters xb = program.generateSimpleNumberByN(n, t);

            TextBoxXA.Text = Convert.ToString(xa.number);
            TextBoxXB.Text = Convert.ToString(xb.number);
        }
    }
}
