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
                Stroke = Brushes.Black,
                StrokeThickness = 5
            };

            Map.Children.Add(rec);
            Canvas.SetTop(rec, start.Y);
            Canvas.SetLeft(rec, start.X);

        }
    }
}
