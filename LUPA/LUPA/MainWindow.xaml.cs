using Microsoft.Win32;
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

namespace LUPA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Point start;

        private void Map_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "TXT Files (*.txt)|*.txt",
            };
            ofd.ShowDialog();

            Parser p = new Parser();
            var contour = p.ParseContour(ofd.FileName);

            foreach (var contourPoint in contour)
            {
                start = new Point(contourPoint.X, contourPoint.Y);

                var rec = new Rectangle()
                {
                    Stroke = Brushes.IndianRed,
                    StrokeThickness = 5
                };

                Map.Children.Add(rec);
                Canvas.SetTop(rec, start.Y);
                Canvas.SetLeft(rec, start.X);
            }

            Line li = new Line()
            {
                Stroke = Brushes.IndianRed,
                StrokeThickness = 2,
                X1 = 40,
                Y1 = 150,
                X2 = 40,
                Y2 = 206
            };
            Map.Children.Add(li);

            Line li1 = new Line()
            {
                Stroke = Brushes.IndianRed,
                StrokeThickness = 2,
                X1 = 40,
                Y1 = 150,
                X2 = 402,
                Y2 = 50
            };
            Map.Children.Add(li1);
            
            Line li2 = new Line()
            {
                Stroke = Brushes.IndianRed,
                StrokeThickness = 2,
                X1 = 40,
                Y1 = 206,
                X2 = 205,
                Y2 = 505.5
            };
            Map.Children.Add(li2);

            Line li3 = new Line()
            {
                Stroke = Brushes.IndianRed,
                StrokeThickness = 2,
                X1 = 402,
                Y1 = 50,
                X2 = 40,
                Y2 = 150
            };
            Map.Children.Add(li3);

            Line li4 = new Line()
            {
                Stroke = Brushes.IndianRed,
                StrokeThickness = 2,
                X1 = 205,
                Y1 = 505.5,
                X2 = 402,
                Y2 = 560
            };
            Map.Children.Add(li4);

            Line li5 = new Line()
            {
                Stroke = Brushes.IndianRed,
                StrokeThickness = 2,
                X1 = 402,
                Y1 = 560,
                X2 = 550,
                Y2 = 300
            };
            Map.Children.Add(li5);

            Line li6 = new Line()
            {
                Stroke = Brushes.IndianRed,
                StrokeThickness = 2,
                X1 = 550,
                Y1 = 300,
                X2 = 402,
                Y2 = 50
            };
            Map.Children.Add(li6);
        }

        private void ChangeBackground_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "PNG Files (*.png)|*.png",
            };
            ofd.ShowDialog();

            ImageBrush ib = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(ofd.FileName, UriKind.Relative))
            };
            Map.Background = ib;
        }
    }
}
