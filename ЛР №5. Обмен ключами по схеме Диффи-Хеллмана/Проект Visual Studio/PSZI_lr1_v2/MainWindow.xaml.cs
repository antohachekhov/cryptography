using System;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Controls;
using System.Linq;
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
            program.t = Convert.ToInt32(TextBoxT.Text);
            program.n = Convert.ToInt32(TextBoxN.Text);

            SimpleValueWithNumItersAndTime result = program.generateSimpleNumberByN();

            TextBoxPN.Text = Convert.ToString(EncoderClass.BitArrayToUlong(result.trueSimpleValue)); // простое число
            TextBoxIN.Text = Convert.ToString(result.numIters); // количество итераций
            TextBoxTime.Text = Convert.ToString(result.time) + " мс"; // время
        }

        // вывод простых чисел из диапазона
        private void ButtonShowPN_Click(object sender, RoutedEventArgs e)
        {

            ListBoxPrimeNumbers.Items.Clear();
            program.t = Convert.ToInt32(TextBoxT.Text);
            ulong min = Convert.ToUInt64(TextBoxMin.Text);
            ulong max = Convert.ToUInt64(TextBoxMax.Text);

            BitArray smallerNumber = EncoderClass.UlongToBitArray(min);
            BitArray largerNumber = EncoderClass.UlongToBitArray(max);

            ValuesWithTime result = program.generateAllSimpleNumbersFromRange(smallerNumber, largerNumber);

            List<BitArray> simpleNumbers = result.values;

            long sumTime = result.time;

            for (int i = 0, j; i < simpleNumbers.Count; i++)
            {
                j = i + 1;
                ListBoxPrimeNumbers.Items.Add(j + "\t" + EncoderClass.BitArrayToUlong(simpleNumbers[i]));
            }

            TextBoxTime2.Text = Convert.ToString(sumTime);
        }

        // получить первообразные корни
        private void ButtonGetPR_Click(object sender, RoutedEventArgs e)
        {
            ListBoxPrimitiveRoots.Items.Clear();
            program.t = Convert.ToInt32(TextBoxT.Text);
            ulong number = Convert.ToUInt64(TextBoxInputN.Text);

            ValuesWithTime result = program.getPrimitiveRoots(number);

            List<BitArray> primitiveNumbers = result.values;

            long sumTime = result.time;

            for (int i = 0; i < primitiveNumbers.Count; i++)
            {
                int j = i + 1;
                ListBoxPrimitiveRoots.Items.Add(j + "\t" + EncoderClass.BitArrayToUlong(primitiveNumbers[i]));
            }

            TextBoxTime3.Text = Convert.ToString(sumTime);
        }

        // обменять ключи
        private void ButtonChangeKeys_Click(object sender, RoutedEventArgs e)
        {
            program.n = Convert.ToInt32(TextBoxChangeN.Text);
            program.g = Convert.ToInt32(TextBoxChangeG.Text);

            TextBoxXA.Text = "";
            TextBoxXB.Text = "";
            TextBoxYA.Text = "";
            TextBoxYB.Text = "";
            TextBoxKA.Text = "";
            TextBoxKB.Text = "";

        }
    }
}
