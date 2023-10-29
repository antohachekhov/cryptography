using System;
using System.Windows;
using Microsoft.Win32;
using PSZI_lr1;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Controls;
using System.Linq;

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
    }
}
