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

            TextBoxPN.Text = Convert.ToString(EncoderClass.ByteArrayToUlong(EncoderClass.BitArrayToByteArray(program.generateSimpleNumberByN()))); // простое число
            TextBoxIN.Text = ""; // количество итераций
            TextBoxTime.Text = ""; // время
        }

        // вывод простых чисел из диапазона
        private void ButtonShowPN_Click(object sender, RoutedEventArgs e)
        {
            ulong min = Convert.ToUInt64(TextBoxMin);
            ulong max = Convert.ToUInt64(TextBoxMax);
            int j = 0;


            BitArray smallerNumber = EncoderClass.ByteArrayToBitArray(EncoderClass.UlongToByteArray(min));
            BitArray largerNumber = EncoderClass.ByteArrayToBitArray(EncoderClass.UlongToByteArray(max));

            List<BitArray> simpleNumbers = new List<BitArray>();

            while(simpleNumbers.Count == 0 || simpleNumbers[simpleNumbers.Count - 1] != null)
            {
                simpleNumbers.Add(program.generateSimpleNumberFromRange(smallerNumber, largerNumber));
            }


            for (int i = 0; i < simpleNumbers.Count; i++)
            {
                j = i + 1;
                ListBoxPrimeNumbers.Items.Add(j + "\t" + simpleNumbers[i]);
            }
        }

        // получить первообразные корни
        private void ButtonGetPR_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 1; i++)
            {
                int j = i + 1;
                ListBoxPrimitiveRoots.Items.Add(j + "\t" + "");
            }
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
