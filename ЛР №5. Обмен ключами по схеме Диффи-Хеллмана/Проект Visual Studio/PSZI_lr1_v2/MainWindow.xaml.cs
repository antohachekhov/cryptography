using System;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using System.Collections.Generic;

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
            ulong min = Convert.ToUInt64(TextBoxMin.Text);
            ulong max = Convert.ToUInt64(TextBoxMax.Text);

            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();
            List<ulong> result = program.generateAllSimpleNumbersFromRange(min, max, t);
            //останавливаем счётчик
            stopwatch.Stop();

            long time = stopwatch.ElapsedMilliseconds;

            for (int i = 0, j; i < result.Count; i++)
            {
                j = i + 1;
                ListBoxPrimeNumbers.Items.Add(j + "\t" + result[i]);
            }

            TextBoxTime2.Text = Convert.ToString(time) + " мс";
        }

        // получить первообразные корни
        private void ButtonGetPR_Click(object sender, RoutedEventArgs e)
        {
            ListBoxPrimitiveRoots.Items.Clear();
            int t = Convert.ToInt32(TextBoxT.Text);
            ulong number = Convert.ToUInt64(TextBoxN2.Text);

            Stopwatch stopwatch = new Stopwatch();
            //засекаем время начала операции
            stopwatch.Start();
            List<ulong> result = program.getPrimitiveRoots(number, t);
            //останавливаем счётчик
            stopwatch.Stop();
            long time = stopwatch.ElapsedMilliseconds;

            long sumTime = time;

            for (int i = 0; i < result.Count; i++)
            {
                int j = i + 1;
                ListBoxPrimitiveRoots.Items.Add(j + "\t" + result[i]);
            }

            TextBoxTime3.Text = Convert.ToString(sumTime) + " мс";
        }

        // обменять ключи
        private void ButtonSendKeys_Click(object sender, RoutedEventArgs e)
        {
            ulong n = Convert.ToUInt64(TextBoxN3.Text);
            ulong g = Convert.ToUInt64(TextBoxG.Text);

            if (g >= n)
            {
                MessageBox.Show("g должно быть меньше, чем n");
                return;
            }

            ulong xa = Convert.ToUInt64(TextBoxXA.Text);
            ulong xb = Convert.ToUInt64(TextBoxXB.Text);
            
            TextBoxYA.Text = Convert.ToString(program.generateY(g, xa, n));
            TextBoxYB.Text = Convert.ToString(program.generateY(g, xb, n));
            TextBlock2Step.Text = "2) Вычисление чисел";


            ulong ya = Convert.ToUInt64(TextBoxYA.Text);
            ulong yb = Convert.ToUInt64(TextBoxYB.Text);

            TextBoxKA.Text = Convert.ToString(program.generateY(ya, xa, n));
            TextBoxKB.Text = Convert.ToString(program.generateY(yb, xb, n));
            TextBlock3Step.Text = "3) Вычисление секретного ключа";

        }

        private void ButtonRandomN_Click(object sender, RoutedEventArgs e)
        {
            int t = Convert.ToInt32(TextBoxT.Text);
            ulong n = (ulong)program.random.Next(2, 8 * EncoderClass.byteCountLong);
            TextBoxN3.Text = Convert.ToString(n);
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
