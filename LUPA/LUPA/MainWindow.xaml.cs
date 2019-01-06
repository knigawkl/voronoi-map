using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LUPA.DataContainers;

namespace LUPA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Map map = new Map();
        System.Windows.Point start;

        public MainWindow()
        {
            InitializeComponent();
        }
      
        private void Map_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(this);

            var rec = new Rectangle()
            {
                Stroke = Brushes.BlueViolet,
                StrokeThickness = 5
            };

            Map.Children.Add(rec);
            Canvas.SetTop(rec, start.Y - TopToolbar.ActualHeight);
            Canvas.SetLeft(rec, start.X);
        }
        /*
        private void Map_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
        */
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "TXT Files (*.txt)|*.txt",
            };

            Nullable<bool> result = ofd.ShowDialog();

            if (result == true)
            {
                map = Parser.ParseFile(ofd.FileName);
            }
        }
        
        private void ChangeBackground_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "PNG Files (*.png)|*.png",
            };

            Nullable<bool> result = ofd.ShowDialog();
            if (result == true)
            {
                ImageBrush ib = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(ofd.FileName, UriKind.Relative))
                };
                Map.Background = ib;
            }
        }
    }
}
