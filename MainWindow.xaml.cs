using Edytor_graficzny.Models;
using Edytor_graficzny.Src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

//…more using statements


namespace Edytor_graficzny
{
    public partial class MainWindow : Window
    {
        //private List<GraphicElementModel> gems = new List<GraphicElementModel>();
        private int iter = 0;
        private Point pntStart;
        private Point pntEnd;
        private Color color = Color.FromRgb(255, 180, 180);
        private double width = 6, height = 2;
        private double cornerRoundness = 0.14;
        private bool isGridActive = false;
        private string drawType = "line";
        private string drawState = "OFF";   // First_ON, ON, First_OFF, OFF
        private readonly Random randomNumber = new Random();
        private readonly Src.FileHandling fileHandling = new Src.FileHandling();


        public MainWindow()
        {
            InitializeComponent();
            currentTool.Text = "Current tool: " + drawType;
            btnStartStop.IsEnabled = true;
            btnLine.IsEnabled = false;
            btnEllipse.IsEnabled = true;

            ConstantUpdate();
        }

        private void ConstantUpdate()
        {

            txtCurrentWidth.Text = "  Current width: " + width.ToString();
            txtCurrentHeight.Text = "  Current height: " + height.ToString();
            txtCurrentScale.Text = "  Current scale: " + fileHandling.scale.ToString();
        }

        private void MenuItem_File_NewClick(object sender, RoutedEventArgs e)
        {
            fileHandling.NewFile();
        }

        private void MenuItem_File_OpenClick(object sender, RoutedEventArgs e)
        {
            fileHandling.gems.Clear();
            DrawBoard.Children.Clear();
            fileHandling.OpenFile(fileHandling.gems);
            double sizeOfElementsX = 0;
            double sizeOfElementsY = 0;
            foreach (var gem in fileHandling.gems)
            {
                if (gem.ElementStartingLocation.X + gem.ElementWidth > sizeOfElementsX)
                {
                    sizeOfElementsX = gem.ElementStartingLocation.X + gem.ElementWidth;
                }
                if (gem.ElementStartingLocation.Y + gem.ElementHeight > sizeOfElementsY)
                {
                    sizeOfElementsY = gem.ElementStartingLocation.Y + gem.ElementHeight;
                }
            }
            fileHandling.scale = (DrawBoard.ActualWidth / sizeOfElementsX);
            if ((DrawBoard.ActualHeight / sizeOfElementsY) < fileHandling.scale)
            {
                fileHandling.scale = (DrawBoard.ActualHeight / sizeOfElementsY);
            }


            string tempDrawType = drawType;
            foreach (var gem in fileHandling.gems)
            {
                drawType = gem.ElementType;
                pntStart.X = (gem.ElementStartingLocation.X - gem.ElementWidth) * fileHandling.scale;
                pntStart.Y = (sizeOfElementsY - gem.ElementStartingLocation.Y - gem.ElementHeight) * fileHandling.scale;
                pntEnd.X = (gem.ElementStartingLocation.X + gem.ElementWidth) * fileHandling.scale;
                pntEnd.Y = (sizeOfElementsY - gem.ElementStartingLocation.Y + gem.ElementHeight) * fileHandling.scale;
                drawState = "First_ON";
                Drawnado();
                drawState = "OFF";
            }
            drawType = tempDrawType;
        }

        private void MenuItem_File_SaveClick(object sender, RoutedEventArgs e)
        {
            fileHandling.SaveFile();
        }
        private void MenuItem_Sandbox_Click(object sender, RoutedEventArgs e)
        {
            Sandbox s = new Sandbox();
            s.Top = 50;
            s.Left = 50;
            s.Show();
        }

        private void MenuItem_Ribbon_Click(object sender, RoutedEventArgs e)
        {
            Ribbon r = new Ribbon();
            r.Top = 50;
            r.Left = 50;
            r.Show();
        }

        private void MenuItem_Undo_Click(object sender, RoutedEventArgs e)
        {
            if (DrawBoard.Children.Count > 0) DrawBoard.Children.RemoveAt(DrawBoard.Children.Count - 1);
        }

        private void MenuItem_btnNewLineTest_Click(object sender, RoutedEventArgs e)
        {
            Line myLine = new Line();
            myLine.Stroke = Brushes.LightSteelBlue;
            myLine.X1 = 1 + iter * 50;
            myLine.X2 = 50 + iter * 50;
            myLine.Y1 = 1 + iter * 50;
            myLine.Y2 = 50 + iter * 50;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 5;
            DrawBoard.Children.Add(myLine);
            iter++;
        }

        private void MenuItem_btnTest_Click(object sender, RoutedEventArgs e)
        {
            Point testPoint = new Point(0, 0);
            Color testColor = Color.FromRgb(255, 255, 255);
            GraphicElementModel testA = new GraphicElementModel(fileHandling.gems.Count, "startStop", "Start", testPoint, 3, 1, testColor, 1);
            fileHandling.gems.Add(testA);
        }

        private void DrawBoard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            pntStart = e.GetPosition(DrawBoard);
            if (drawState == "First_OFF" || drawState == "OFF") drawState = "First_ON";
            Drawnado();
        }

        private void DrawBoard_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (drawState == "First_ON" || drawState == "ON") drawState = "First_OFF";
            Drawnado();
        }

        private void DrawBoard_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                if (drawState == "First_ON" || drawState == "ON") drawState = "First_OFF";
            }
            pntEnd = e.GetPosition(DrawBoard);
            Drawnado();
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            EnableAllButtons();
            drawType = "startStop";
            currentTool.Text = "Current tool: " + drawType;

            //double startX = 1, startY = 1;

            //Rect myRect = new Rect(startX * scale, startY * scale, width * scale, height * scale);
            //RectangleGeometry test2 = new RectangleGeometry(myRect, cornerRoundness * scale * height, cornerRoundness * scale * height);

            //Path startStop = new Path();
            //startStop.Stroke = Brushes.Black;
            //startStop.StrokeThickness = 2;
            //startStop.Data = test2;

            //DrawBoard.Children.Add(startStop);
        }

        private void btnLine_Click(object sender, RoutedEventArgs e)
        {
            EnableAllButtons();
            btnLine.IsEnabled = false;
            drawType = "line";
            currentTool.Text = "Current tool: " + drawType;
        }

        private void btnEllipse_Click(object sender, RoutedEventArgs e)
        {
            EnableAllButtons();
            btnEllipse.IsEnabled = false;
            drawType = "ellipse";
            currentTool.Text = "Current tool: " + drawType;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                width = Convert.ToDouble(txtWidth.Text);
                txtWidth.Text = "";
            }
            catch {}

            try
            {
                height = Convert.ToDouble(txtHeight.Text);
                txtHeight.Text = "";
            }
            catch { }

            try
            {
                fileHandling.scale = Convert.ToDouble(txtScale.Text);
                txtScale.Text = "";
            }
            catch { }
        }

        #region Grid Drawing
        private void btnGrid_Click(object sender, RoutedEventArgs e)
        {
            isGridActive = !isGridActive;
            UpdateElements();
        }

        private void GridDrawing()
        {
            for (int i = 0; i < (DrawBoard.ActualHeight - 1) / fileHandling.scale; i++)
            {
                Path grid = new Path();
                grid.Stroke = Brushes.Black;
                grid.Opacity = 0.3;
                grid.StrokeThickness = 0.5;
                LineGeometry lineGeometry = new LineGeometry(new Point(0, i * fileHandling.scale), new Point(DrawBoard.ActualWidth, i * fileHandling.scale));
                grid.Data = lineGeometry;
                DrawBoard.Children.Add(grid);
            }
            for (int i = 0; i < (DrawBoard.ActualWidth - 1) / fileHandling.scale; i++)
            {
                Path grid = new Path();
                grid.Stroke = Brushes.Black;
                grid.Opacity = 0.3;
                grid.StrokeThickness = 0.5;
                LineGeometry lineGeometry = new LineGeometry(new Point(i * fileHandling.scale, 0), new Point(i * fileHandling.scale, DrawBoard.ActualHeight));
                grid.Data = lineGeometry;
                DrawBoard.Children.Add(grid);
            }
        }
        #endregion
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            fileHandling.gems.Clear();
            UpdateElements();
        }

        private void EnableAllButtons()
        {
            btnStartStop.IsEnabled = true;
            btnLine.IsEnabled = true;
            btnEllipse.IsEnabled = true;
        }

        private void Drawnado()
        {
            Point startPoint;
            if (drawState != "OFF")
            {
                if (drawState == "ON" || drawState == "First_OFF")
                {
                    if (DrawBoard.Children.Count != 0)
                    {
                        DrawBoard.Children.RemoveAt(DrawBoard.Children.Count - 1);
                    }
                }

                switch (drawType)
                {
                    case "startStop":
                        startPoint = new Point(Convert.ToInt32(pntStart.X / fileHandling.scale - width / 2) * fileHandling.scale, Convert.ToInt32(pntStart.Y / fileHandling.scale - height / 2) * fileHandling.scale);
                        if (pntEnd != null) startPoint = new Point(Convert.ToInt32(pntEnd.X / fileHandling.scale - width / 2) * fileHandling.scale, Convert.ToInt32(pntEnd.Y / fileHandling.scale - height / 2) * fileHandling.scale);
                        
                        Color blueprintColor = Color.FromRgb(120, 120, 255);

                        Rect myRect = new Rect(startPoint.X, startPoint.Y, width * fileHandling.scale, height * fileHandling.scale);

                        Path startStop = new Path();
                        startStop.Stroke = new SolidColorBrush(blueprintColor);
                        startStop.StrokeThickness = 2;
                        startStop.Data = new RectangleGeometry(myRect, cornerRoundness * fileHandling.scale * height, cornerRoundness * fileHandling.scale * height);

                        DrawBoard.Children.Add(startStop);
                        break;

                    case "line":
                        Line myLine = new Line
                        {
                            Stroke = Brushes.DarkGray,
                            X1 = pntStart.X,
                            X2 = pntEnd.X,
                            Y1 = pntStart.Y,
                            Y2 = pntEnd.Y,
                            StrokeThickness = 2
                        };

                        DrawBoard.Children.Add(myLine);
                        break;

                    case "ellipse":
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush
                        {
                            Color = Color.FromArgb(255, this.color.R, this.color.G, this.color.B)
                        };

                        Ellipse myEllipse = new Ellipse
                        {
                            Fill = mySolidColorBrush,
                            StrokeThickness = 2,
                            Stroke = Brushes.Black,
                            Width = Math.Abs(pntStart.X - pntEnd.X),
                            Height = Math.Abs(pntStart.Y - pntEnd.Y)
                        };

                        DrawBoard.Children.Add(myEllipse);

                        Canvas.SetLeft(myEllipse, (pntStart.X < pntEnd.X) ? pntStart.X : pntEnd.X);
                        Canvas.SetTop(myEllipse, (pntStart.Y < pntEnd.Y) ? pntStart.Y : pntEnd.Y);
                        break;
                    default:
                        Console.WriteLine("TODO - add error message");
                        break;
                }

                if (drawState == "First_ON") drawState = "ON";
                else if (drawState == "First_OFF")
                {

                    #region Saving new element to gems
                    switch (drawType)
                    {
                        case "startStop":
                            {
                                GraphicElementModel element = new GraphicElementModel(fileHandling.gems.Count, "startStop", "Start", startPoint, width, height, color, 2);    //TODO: stroke //TODO: replace color with color picker
                                fileHandling.gems.Add(element);

                                break;
                            }
                    }
                    #endregion

                    UpdateElements();

                    drawState = "OFF";
                }
                currentTool.Text = Convert.ToString(DrawBoard.Children.Count);
                mouse_x.Text = Convert.ToString(pntEnd.X);
                mouse_y.Text = Convert.ToString(pntEnd.Y);
                gemsNumber.Text = Convert.ToString(fileHandling.gems.Count);
            }
        }


        #region Clear, then drawing grid -> elements -> arrows -> text
        private void UpdateElements()
        {
            DrawBoard.Children.Clear();
            if (isGridActive) GridDrawing();
            foreach (GraphicElementModel gem in fileHandling.gems.ToList())
            {
                switch (gem.ElementType)
                {
                    case "startStop":
                        {
                            Rect myRect = new Rect(gem.ElementStartingLocation.X, gem.ElementStartingLocation.Y, gem.ElementWidth * fileHandling.scale, gem.ElementHeight * fileHandling.scale);

                            Path startStop = new Path();
                            startStop.Stroke = Brushes.Black;
                            startStop.Fill = new SolidColorBrush(color);
                            startStop.StrokeThickness = gem.ElementStroke;
                            startStop.Data = new RectangleGeometry(myRect, cornerRoundness * fileHandling.scale * gem.ElementHeight, cornerRoundness * fileHandling.scale * gem.ElementHeight);


                            DrawBoard.Children.Add(startStop);
                            break;
                        }
                }
            }
        }
        #endregion

        private void btnColorBlack_Click(object sender, RoutedEventArgs e)
        {
            color = Color.FromRgb(0, 0, 0);
        }

        private void btnColorRed_Click(object sender, RoutedEventArgs e)
        {
            color = Color.FromRgb(237, 36, 28);
        }

        private void btnColorGreen_Click(object sender, RoutedEventArgs e)
        {
            color = Color.FromRgb(47, 215, 97);
        }

        private void btnColorBlue_Click(object sender, RoutedEventArgs e)
        {
            color = Color.FromRgb(0, 162, 232);
        }
    }
}