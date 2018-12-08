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
        Map map;
        System.Windows.Point position;

        public MainWindow()
        {
            InitializeComponent();
            map = new Map();
        }

        /// <summary>
        /// Draws a Key/Contour Point (depends on which radio button is checked)
        /// </summary>
        private void Map_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            position = e.GetPosition(this);
            position.Y -= TopToolbar.ActualHeight;
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

        /// <summary>
        /// Removes a Key/Contour Point (depends on which radio button is checked)
        /// </summary>
        private void Map_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Rectangle clickedShape)
            {
                if (clickedShape.Stroke == Brushes.IndianRed && KeyPointBtn.IsChecked == true)
                {
                    Map.Children.Remove(clickedShape);   
                }
                if (clickedShape.Stroke == Brushes.LightSeaGreen && ContourPointBtn.IsChecked == true)
                {
                    Map.Children.Remove(clickedShape);
                }
                RemovePointFromDataContainer(clickedShape);
            }
        }

        private void RemovePointFromDataContainer(Rectangle clickedShape)
        {
            double x = Canvas.GetLeft(clickedShape);
            double y = Canvas.GetTop(clickedShape) + TopToolbar.ActualHeight;
            map.KeyPoints.RemoveAll(o => o.X == x && o.Y == y);
            map.ContourPoints.RemoveAll(o => o.X == x && o.Y == y);
        }

        private void DrawPoint(Brush color, System.Windows.Point position)
        {           
            var rec = new Rectangle()
            {
                StrokeThickness = 5,
                Stroke = color
            };
            Map.Children.Add(rec);
            Canvas.SetTop(rec, position.Y);
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
                Map.Children.Clear();
                map = Parser.ParseFile(ofd.FileName);
                DrawMap();
            }
        }

        private void DrawMap()
        {
            DrawContourPoints();
            DrawKeyPoints();
        }

        private void DrawContourPoints()
        {
            System.Windows.Point position = new System.Windows.Point();
            foreach (var cp in map.ContourPoints)
            {               
                position.X = cp.X;
                position.Y = cp.Y;
                DrawPoint(Brushes.LightSeaGreen, position);
            }
        }

        private void DrawKeyPoints()
        {
            System.Windows.Point position = new System.Windows.Point();
            foreach (var kp in map.KeyPoints)
            {
                position.X = kp.X;
                position.Y = kp.Y;
                DrawPoint(Brushes.IndianRed, position);
            }
        }

        private void ChangeBackground_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "PNG Files (*.png)|*.png",
            };

            bool? result = ofd.ShowDialog();
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
