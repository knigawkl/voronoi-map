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
                ColorAreas();
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

        private enum MapObjects
        {
            Blank = 0, KeyPoint, ContourPoint, CustomObject
        }

        private void ColorAreas()
        {
            int[,] mapPixels = new int[600, 600];
            AddKeyPointsToMapPixels(ref mapPixels);
            AddContourPointsToMapPixels(ref mapPixels);
            AddCustomObjectsToMapPixels(ref mapPixels);

            map.KeyPoints[0].BrushColor = Brushes.Green;
            map.KeyPoints[1].BrushColor = Brushes.HotPink;
            map.KeyPoints[2].BrushColor = Brushes.Thistle;


            /*
            foreach (var kp in map.KeyPoints)
            {
                //int r = (int)(kp.X * 0.4);
                //int g = (int)(kp.Y * 0.4);
                //int b = 123;
                //Random r = new Random();
                //kp.BrushColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(kp.X), (byte)r.Next(kp.Y), (byte)r.Next(kp.X)));//contourPointColor;//new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
            }
            */
            foreach (var kp in map.KeyPoints)
            {
                OutputTxt.AppendText(kp.BrushColor.ToString() + "  ");
            }
            
            for (int row = 0; row < 600; row++)
            {
                for (int column = 0; column < 600; column++)
                {
                    SolidColorBrush color = FindClosestKeyPoint(row, column).BrushColor;
                    DrawTransparentPoint(color, new System.Windows.Point(row, column));
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

        private void AddKeyPointsToMapPixels(ref int[,] mapPixels)
        {
            foreach (var kp in map.KeyPoints)
            {
                mapPixels[kp.X, kp.Y] = (int)MapObjects.KeyPoint;
            }
        }

        private void AddContourPointsToMapPixels(ref int[,] mapPixels)
        {
            foreach (var cp in map.ContourPoints)
            {
                mapPixels[cp.X, cp.Y] = (int)MapObjects.ContourPoint;
            }
        }

        private void AddCustomObjectsToMapPixels(ref int[,] mapPixels)
        {
            foreach (var co in map.CustomObjects)
            {
                mapPixels[co.X, co.Y] = (int)MapObjects.CustomObject;
            }
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
                Opacity = .20
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

        private void DrawLine(Brush color, Point src, Point end)
        {
            Line line = new Line()
            {
                Stroke = color,
                StrokeThickness = 2,
                X1 = src.X,
                Y1 = src.Y,
                X2 = end.X,
                Y2 = end.Y
            };
            Map.Children.Add(line);
        }
    }
}
