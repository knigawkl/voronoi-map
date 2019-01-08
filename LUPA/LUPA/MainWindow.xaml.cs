using Microsoft.Win32;
using System;
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
        System.Windows.Point position;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Draws a Key/Contour Point (depends on which radio button is checked)
        /// </summary>
        private void Map_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            position = e.GetPosition(this);
            if (KeyPointBtn.IsChecked == true)
            {
                DrawPoint(Brushes.IndianRed, position);
                AddKeyPoint(position);
            }
            else if (ContourPointBtn.IsChecked == true)
            {
                DrawPoint(Brushes.LightSeaGreen, position);
                AddContourPoint(position);
            }
        }

        private void Map_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Shape clickedShape)
            {
                if (clickedShape.Stroke == Brushes.IndianRed && KeyPointBtn.IsChecked == true)
                {
                    Map.Children.Remove(clickedShape);
                    //tu usunac z naszego map
                    
                }
                if (clickedShape.Stroke == Brushes.LightSeaGreen && ContourPointBtn.IsChecked == true)
                {
                    Map.Children.Remove(clickedShape);
                }
            }
        }
        
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "TXT Files (*.txt)|*.txt",
            };

            bool? result = ofd.ShowDialog();

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

        private void KeyPointBtn_Checked(object sender, RoutedEventArgs e)
        {
            KeyPointBtn.Background = Brushes.IndianRed;
            ContourPointBtn.Background = Brushes.Azure;
        }

        private void ContourBtn_Checked(object sender, RoutedEventArgs e)
        {
            ContourPointBtn.Background = Brushes.LightSeaGreen;
            KeyPointBtn.Background = Brushes.Azure;
        }

        private void DrawPoint(Brush color, System.Windows.Point position)
        {           
            var rec = new Rectangle()
            {
                StrokeThickness = 5,
                Stroke = color
            };
            Map.Children.Add(rec);
            Canvas.SetTop(rec, position.Y - TopToolbar.ActualHeight);
            Canvas.SetLeft(rec, position.X);
        }

        private void AddKeyPoint(System.Windows.Point position)
        {
            map.KeyPoints.Add(new KeyPoint(position.X, position.Y, "defaultKeyPointName"));
        }

        private void AddContourPoint(System.Windows.Point position)
        {
            map.ContourPoints.Add(new Point(position.X, position.Y));
        }
    }
}
