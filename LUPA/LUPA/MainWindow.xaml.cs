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
                Stroke = Brushes.IndianRed,
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
        }

        const double ScaleRate = 1.1;
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                st.ScaleX *= ScaleRate;
                st.ScaleY *= ScaleRate;
            }
            else
            {
                st.ScaleX /= ScaleRate;
                st.ScaleY /= ScaleRate;
            }
        }
    }
}
