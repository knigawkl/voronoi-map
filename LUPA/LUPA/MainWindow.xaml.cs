using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LUPA.DataContainers;
using System.Collections.Generic;
using System.Linq;

namespace LUPA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly SolidColorBrush keyPointColor = Brushes.IndianRed;
        public readonly SolidColorBrush contourPointColor = Brushes.LightSeaGreen;
        public readonly SolidColorBrush customObjectColor = Brushes.Gold;
        public readonly SolidColorBrush areaLinesColor = Brushes.Black;
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
            OutputTxt.Text = "";
            position = e.GetPosition(this);
            position.X = (int)position.X;
            position.Y = (int)position.Y;
            position.Y -= TopToolbar.ActualHeight;
            if (KeyPointBtn.IsChecked == true)
            {
                DrawPoint(keyPointColor, position);
                AddKeyPoint(position);
                OutputTxt.Text = "Dodano punkt kluczowy (" + position.X + "; " + position.Y + ")";
            }
            else if (ContourPointBtn.IsChecked == true)
            {
                if (map.ContourPoints.Count < 3)
                {
                    OutputTxt.Text = "Przed dodaniem punktu konturu wczytaj plik wejściowy zawierający co najmniej 3 punkty konturu";
                }
                else
                {
                    DeleteCurrentContour();
                    DrawPoint(contourPointColor, position);
                    AddContourPoint(position);
                    DrawContourLinesInOrder();
                    OutputTxt.Text = "Dodano punkt konturu (" + position.X + "; " + position.Y + ")";
                }
            }
        }

        /// <summary>
        /// Removes a Key/Contour Point (depends on which radio button is checked)
        /// </summary>
        private void Map_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            OutputTxt.Text = "";
            if (e.OriginalSource is Rectangle clickedShape)
            {
                if (clickedShape.Stroke == keyPointColor && KeyPointBtn.IsChecked == true)
                {
                    Map.Children.Remove(clickedShape);
                    OutputTxt.Text = "Usunięto punkt kluczowy (" + Canvas.GetLeft(clickedShape) + "; " + Canvas.GetTop(clickedShape) + ")";
                }
                if (clickedShape.Stroke == contourPointColor && ContourPointBtn.IsChecked == true && map.ContourPoints.Count < 4)
                {
                    OutputTxt.Text = "Nieudane usuwanie: muszą pozostać co najmniej 3 punkty konturu";
                }
                else if (clickedShape.Stroke == contourPointColor && ContourPointBtn.IsChecked == true)
                {
                    Map.Children.Remove(clickedShape);
                    RemovePointFromDataContainer(clickedShape);
                    DeleteCurrentContour();
                    DrawContourLinesInOrder();
                    OutputTxt.Text = "Usunięto punkt konturu (" + Canvas.GetLeft(clickedShape) + "; " + Canvas.GetTop(clickedShape) + ")";
                }
            }
        }

        private void DeleteCurrentContour()
        {
            var contourLines = Map.Children.OfType<Line>().ToList();
            foreach (var line in contourLines)
            {
                Map.Children.Remove(line);
            }
        }

        private void RemovePointFromDataContainer(Rectangle clickedShape)
        {
            double x = Canvas.GetLeft(clickedShape);
            double y = Canvas.GetTop(clickedShape);
            map.KeyPoints.RemoveAll(o => o.X == x && o.Y == y);
            map.ContourPoints.RemoveAll(o => o.X == x && o.Y == y);
        }

        private void AddKeyPoint(System.Windows.Point position)
        {
            map.KeyPoints.Add(new KeyPoint((int)position.X, (int)position.Y, "defaultKeyPointName"));
            DrawMap();
        }

        private void AddContourPoint(System.Windows.Point position)
        {
            var distances = new List<double>();
            double dist;
            foreach (var cp in map.ContourPoints)
            {
                dist = Util.Mathematics.CalculateDistBetweenPoints(position, new System.Windows.Point(cp.X, cp.Y));
                distances.Add(dist);
            }
            distances.Sort();
            Point firstPt, scndPt;
            firstPt = FindDistantPoint(position, distances[0]);
            scndPt = FindDistantPoint(position, distances[1]);
            int firstIndex = map.ContourPoints.IndexOf(firstPt);
            int scndIndex = map.ContourPoints.IndexOf(scndPt);
            if (firstIndex == 0 && scndIndex == 1)
            {
                map.ContourPoints.Insert(scndIndex, new Point(position.X, position.Y));
            }
            else if (firstIndex == 0 && scndIndex == map.ContourPoints.Count - 1)
            {
                map.ContourPoints.Add(new Point(position.X, position.Y));
            }
            else if (firstIndex < scndIndex)
            {
                map.ContourPoints.Insert(scndIndex, new Point(position.X, position.Y));
            }
            else if (firstIndex > scndIndex)
            {
                map.ContourPoints.Insert(firstIndex, new Point(position.X, position.Y));
            }
        }

        private Point FindDistantPoint(System.Windows.Point position, double distance)
        {
            Point point = new Point(0, 0);
            double dist;
            foreach (var cp in map.ContourPoints)
            {
                dist = Util.Mathematics.CalculateDistBetweenPoints(position, cp);
                if (dist == distance)
                {
                    return cp;
                }
            }
            return point;
        }

        private KeyPoint FindDistantPointFromKeyPoint(Point position, double distance)
        {
            KeyPoint point = new KeyPoint(0, 0, "");
            double dist;
            foreach (var kp in map.KeyPoints)
            {
                dist = Util.Mathematics.CalculateDistBetweenPoints(position, kp);
                if (dist == distance)
                {
                    return kp;
                }
            }
            return point;
        }

        private void KeyPointBtn_Checked(object sender, RoutedEventArgs e)
        {
            KeyPointBtn.Background = keyPointColor;
            ContourPointBtn.Background = Brushes.Azure;
        }

        private void ContourBtn_Checked(object sender, RoutedEventArgs e)
        {
            ContourPointBtn.Background = contourPointColor;
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
                map = Parser.ParseFile(ofd.FileName, out List<string> feedback);
                DrawMap();
                DrawAreaLines();
            }
        }

        private void DrawMap()
        {
            DrawContourPoints();
            DrawKeyPoints();
            DrawCustomObjects();
            DrawContourLinesInOrder();
        }

        private void DrawContourPoints()
        {
            System.Windows.Point position = new System.Windows.Point();
            foreach (var cp in map.ContourPoints)
            {
                position.X = cp.X;
                position.Y = cp.Y;
                DrawPoint(contourPointColor, position);
            }
        }

        private void DrawKeyPoints()
        {
            System.Windows.Point position = new System.Windows.Point();
            foreach (var kp in map.KeyPoints)
            {
                position.X = kp.X;
                position.Y = kp.Y;
                DrawPoint(keyPointColor, position);
            }
        }

        private void DrawCustomObjects()
        {
            System.Windows.Point position = new System.Windows.Point();
            foreach (var co in map.CustomObjects)
            {
                position.X = co.X;
                position.Y = co.Y;
                DrawPoint(customObjectColor, position);
            }
        }

        private void DrawContourLinesInOrder()
        {
            double srcPtX, srcPtY, endPtX, endPtY;
            for (int i = 0; i < map.ContourPoints.Count - 1; i++)
            {
                srcPtX = map.ContourPoints[i].X;
                srcPtY = map.ContourPoints[i].Y;
                endPtX = map.ContourPoints[i + 1].X;
                endPtY = map.ContourPoints[i + 1].Y;
                DrawLine(contourPointColor, srcPtX, srcPtY, endPtX, endPtY);
            }
            DrawLastContourLine();
        }

        private void DrawLastContourLine()
        {
            double srcPtX, srcPtY, endPtX, endPtY;
            int totalContourPointsNum = map.ContourPoints.Count - 1;
            srcPtX = map.ContourPoints[0].X;
            srcPtY = map.ContourPoints[0].Y;
            endPtX = map.ContourPoints[totalContourPointsNum].X;
            endPtY = map.ContourPoints[totalContourPointsNum].Y;
            DrawLine(contourPointColor, srcPtX, srcPtY, endPtX, endPtY);
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
            OutputTxt.Text = "Zmieniono tło na: " + ofd.FileName.ToString();
        }

        private void DrawAreaLines()
        {
            for (int i = 0; i < map.KeyPoints.Count; i++)
            {
                map.KeyPoints[i].Id = i;
            }

            int[,] identificators = new int[600, 600];

            for (int row = 0; row < Map.ActualHeight; row++)
            {
                for (int column = 0; column < Map.ActualWidth; column++)
                {
                    identificators[row, column] = FindClosestKeyPoint(row, column).Id;
                }
            }

            for (int row = 1; row < 599; row++)
            {
                for (int column = 1; column < 599; column++)
                {
                    if(!Util.Mathematics.IsPointInPolygon(map.ContourPoints, new Point(row, column)))
                    {
                        break;
                    }

                    if (identificators[row, column] != identificators[row - 1, column] 
                        || identificators[row, column] != identificators[row + 1, column]
                        || identificators[row, column] != identificators[row, column - 1] 
                        || identificators[row, column] != identificators[row + 1, column])
                    {
                        DrawTransparentPoint(areaLinesColor, new System.Windows.Point(row, column));
                    }
                }
            }
        }

        private KeyPoint FindClosestKeyPoint(int x, int y)
        {
            var distances = new List<double>();
            double dist;
            foreach (var kp in map.KeyPoints)
            {
                dist = Util.Mathematics.CalculateDistBetweenPoints(new Point(x, y), kp);
                distances.Add(dist);
            }
            distances.Sort();
            return FindDistantPointFromKeyPoint(new Point(x, y), distances[0]);

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

        private void DrawTransparentPoint(Brush color, System.Windows.Point position)
        {
            var rec = new Rectangle()
            {
                StrokeThickness = 1,
                Stroke = color,
                Opacity = .80
            };
            Map.Children.Add(rec);
            Canvas.SetTop(rec, position.Y);
            Canvas.SetLeft(rec, position.X);
        }

        private void DrawLine(Brush color, double srcPtX, double srcPtY, double endPtX, double endPtY)
        {
            Line line = new Line()
            {
                Stroke = color,
                StrokeThickness = 2,
                X1 = srcPtX,
                Y1 = srcPtY,
                X2 = endPtX,
                Y2 = endPtY
            };
            Map.Children.Add(line);
        }
    }
}
