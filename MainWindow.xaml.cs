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
        private int iter = 0;
        private Point pntStart;
        private Point pntEnd;
        private Color color = Color.FromRgb(255, 180, 180);
        private double width = 6, height = 2;
        private double cornerRoundness = 0.14;
        private bool isGridActive = false;
        private string drawType = "Start/Stop";
        private string drawState = "OFF";   // First_ON, ON, First_OFF, OFF
        private readonly Random randomNumber = new Random();
        private readonly Src.FileHandling fileHandling = new Src.FileHandling();
        private List<Point> arrowPoints = new List<Point>();


        public MainWindow()
        {
            InitializeComponent();
            currentTool.Text = "Current tool: " + drawType;
            btnStartStop.IsEnabled = false;
            btnFinishArrow.IsEnabled = false;

            UpdateElements();
            TextUpdate();
        }

        private void TextUpdate()
        {
            txtCurrentWidth.Text = "  Current width: " + width.ToString();
            txtCurrentHeight.Text = "  Current height: " + height.ToString();
            txtCurrentScale.Text = "  Current scale: " + fileHandling.scale.ToString();
            txtCurrentGridItems.Text = "  Current number: " + (fileHandling.gridItemsPerCM * 12).ToString() + "; each crate: " + 1/fileHandling.gridItemsPerCM + "cm";
        }

        private void MenuItem_File_NewClick(object sender, RoutedEventArgs e)
        {
            fileHandling.NewFile();
        }

        private void MenuItem_File_OpenClick(object sender, RoutedEventArgs e)
        {
            fileHandling.gems.Clear();
            fileHandling.OpenFile(fileHandling.gems);
            UpdateElements();
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
            if (fileHandling.gems.Count > 0) fileHandling.gems.RemoveAt(fileHandling.gems.Count - 1);
            UpdateElements();
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
            isGridActive = true;
            GraphicElementModel testStartStop = new GraphicElementModel(0, "Start/Stop", "test", new Point(1, 1), 6, 2, color, 1);
            GraphicElementModel testIO = new GraphicElementModel(1, "Input/Output", "test", new Point(9, 1), 6, 2, color, 1);
            GraphicElementModel testProcess = new GraphicElementModel(2, "Process", "test", new Point(1, 5), 6, 2, color, 1);
            GraphicElementModel testDecision = new GraphicElementModel(3, "Decision", "test", new Point(9, 5), 6, 2, color, 1);

            fileHandling.gems.Add(testStartStop);
            fileHandling.gems.Add(testIO);
            fileHandling.gems.Add(testProcess);
            fileHandling.gems.Add(testDecision);

            List<Point> testList = new List<Point>();
            testList.Add(new Point(4, 3));
            testList.Add(new Point(4, 5));
            ArrowsModel testArrow = new ArrowsModel(testList, "test");

            List<Point> testList2 = new List<Point>();
            testList2.Add(new Point(12, 3));
            testList2.Add(new Point(12, 4));
            testList2.Add(new Point(4, 4));
            ArrowsModel testArrow2 = new ArrowsModel(testList2, "test");

            //List<Point> testList3 = new List<Point>();
            //testList3.Add(new Point(12, 4));
            //testList3.Add(new Point(12, 5));
            //ArrowsModel testArrow3 = new ArrowsModel(testList3, "test");

            fileHandling.arrows.Add(testArrow2);
            fileHandling.arrows.Add(testArrow);
            //fileHandling.arrows.Add(testArrow3);

            UpdateElements();
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

        #region Tools buttons handling
        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnStartStop.IsEnabled = false;
        }
        private void btnIO_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnIO.IsEnabled = false;
        }
        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnProcess.IsEnabled = false;
        }
        private void btnDecision_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnDecision.IsEnabled = false;
        }
        private void btnSolidLine_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnSolidLine.IsEnabled = false;
        }
        private void btnDashedLine_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnDashedLine.IsEnabled = false;
        }
        private void btnDottedLine_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnDottedLine.IsEnabled = false;
        }
        private void btnFinishArrow_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            ArrowCreation();
            btnFinishArrow.IsEnabled = false;
        }
        private void btnEllipse_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnEllipse.IsEnabled = false;
        }

        private void ButtonControll(string buttonName)
        {
            btnStartStop.IsEnabled = true;
            btnIO.IsEnabled = true;
            btnProcess.IsEnabled = true;
            btnDecision.IsEnabled = true;
            btnSolidLine.IsEnabled = true;
            btnDashedLine.IsEnabled = true;
            btnDottedLine.IsEnabled = true;
            btnEllipse.IsEnabled = true;

            buttonName.Trim();
            string[] parts = buttonName.Split(":".ToCharArray());

            drawType = parts[1].Trim();
            currentTool.Text = "Current tool: " + drawType;
        }
        #endregion

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

            try
            {
                fileHandling.gridItemsPerCM = Convert.ToDouble(txtGridItems.Text)/12;
                txtCurrentGridItems.Text = "";
            }
            catch { }

            UpdateElements();
            TextUpdate();
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
            Path borderLine = new Path();
            borderLine.Stroke = Brushes.Black;
            borderLine.Opacity = 0.3;
            borderLine.StrokeThickness = 2;
            LineGeometry lineBorderLine = new LineGeometry(new Point(fileHandling.gridItemsPerCM * 12 * fileHandling.scale, 0), new Point(fileHandling.gridItemsPerCM * 12 * fileHandling.scale, DrawBoard.ActualHeight));
            borderLine.Data = lineBorderLine;
            DrawBoard.Children.Add(borderLine);
        }
        #endregion

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            fileHandling.gems.Clear();
            fileHandling.arrows.Clear();
            arrowPoints.Clear();
            UpdateElements();
        }


        private void Drawnado()
        {
            Point startPoint;
            Point endPoint;
            if (drawState != "OFF")
            {
                if (drawState == "ON" || drawState == "First_OFF")
                {
                    if (DrawBoard.Children.Count != 0)
                    {
                        DrawBoard.Children.RemoveAt(DrawBoard.Children.Count - 1);
                    }
                }
                Color blueprintColor = Color.FromRgb(120, 120, 255);

                if (drawType != "Solid Line")
                {
                    startPoint = new Point(Convert.ToInt32(pntStart.X / fileHandling.scale - width / 2), Convert.ToInt32(pntStart.Y / fileHandling.scale - height / 2));
                    if (pntEnd != null) startPoint = new Point(Convert.ToInt32(pntEnd.X / fileHandling.scale - width / 2), Convert.ToInt32(pntEnd.Y / fileHandling.scale - height / 2));


                    Path path = new Path();
                    path.Stroke = new SolidColorBrush(blueprintColor);
                    path.StrokeThickness = 2;

                    switch (drawType)
                    {
                        case "Start/Stop":
                            Rect myRect = new Rect(startPoint.X * fileHandling.scale, startPoint.Y * fileHandling.scale, width * fileHandling.scale, height * fileHandling.scale);

                            path.Data = new RectangleGeometry(myRect, cornerRoundness * fileHandling.scale * height, cornerRoundness * fileHandling.scale * height);
                            break;

                        case "Input/Output":
                            PathFigure myPathFigure = new PathFigure();
                            myPathFigure.StartPoint = new Point(Convert.ToInt32(startPoint.X + (width / 7)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale);
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (width)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale),
                                    true /* IsStroked */ ));
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (width / 7 * 6)) * fileHandling.scale, Convert.ToInt32(startPoint.Y + (height)) * fileHandling.scale),
                                    true /* IsStroked */ ));
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X) * fileHandling.scale, Convert.ToInt32(startPoint.Y + (height)) * fileHandling.scale),
                                    true /* IsStroked */ ));
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (width / 7)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale),
                                    true /* IsStroked */ ));

                            PathGeometry myPathGeometry = new PathGeometry();
                            myPathGeometry.Figures.Add(myPathFigure);

                            path.Data = myPathGeometry;
                            break;

                        case "Process":
                            Rect myRect2 = new Rect(startPoint.X * fileHandling.scale, startPoint.Y * fileHandling.scale, width * fileHandling.scale, height * fileHandling.scale);

                            path.Data = new RectangleGeometry(myRect2);
                            break;

                        case "Decision":
                            PathFigure myPathFigure2 = new PathFigure();
                            myPathFigure2.StartPoint = new Point(Convert.ToInt32(startPoint.X + (width / 2)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale);
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + width) * fileHandling.scale, Convert.ToInt32((startPoint.Y * 2) + height) * fileHandling.scale / 2),
                                    true /* IsStroked */ ));
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (width / 2)) * fileHandling.scale, Convert.ToInt32(startPoint.Y + height) * fileHandling.scale),
                                    true /* IsStroked */ ));
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X) * fileHandling.scale, Convert.ToInt32((startPoint.Y * 2) + height) * fileHandling.scale / 2),
                                    true /* IsStroked */ ));
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (width / 2)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale),
                                    true /* IsStroked */ ));

                            PathGeometry myPathGeometry2 = new PathGeometry();
                            myPathGeometry2.Figures.Add(myPathFigure2);

                            path.Data = myPathGeometry2;
                            break;

                        case "Solid Line":
                            PathFigure myPathFigure3 = new PathFigure();
                            myPathFigure3.StartPoint = new Point(Convert.ToInt32(startPoint.X + (width / 7)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale);
                            myPathFigure3.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (width)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale),
                                    true /* IsStroked */ ));
                            myPathFigure3.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (width / 7 * 6)) * fileHandling.scale, Convert.ToInt32(startPoint.Y + (height)) * fileHandling.scale),
                                    true /* IsStroked */ ));
                            myPathFigure3.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X) * fileHandling.scale, Convert.ToInt32(startPoint.Y + (height)) * fileHandling.scale),
                                    true /* IsStroked */ ));
                            myPathFigure3.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (width / 7)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale),
                                    true /* IsStroked */ ));

                            PathGeometry myPathGeometry3 = new PathGeometry();
                            myPathGeometry3.Figures.Add(myPathFigure3);

                            path.Data = myPathGeometry3;
                            break;

                        case "Ellipse":
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
                    DrawBoard.Children.Add(path);
                }
                else
                {
                    btnFinishArrow.IsEnabled = true;
                    if (!arrowPoints.Any()) startPoint = new Point(Convert.ToInt32(pntStart.X / fileHandling.scale) * fileHandling.scale, Convert.ToInt32(pntStart.Y / fileHandling.scale) * fileHandling.scale);
                    else startPoint = new Point(arrowPoints.Last().X * fileHandling.scale, arrowPoints.Last().Y * fileHandling.scale);
                    

                    endPoint = new Point(Convert.ToInt32(pntStart.X / fileHandling.scale) * fileHandling.scale, Convert.ToInt32(pntStart.Y / fileHandling.scale) * fileHandling.scale);
                    if (pntEnd != null) endPoint = new Point(Convert.ToInt32(pntEnd.X / fileHandling.scale) * fileHandling.scale, Convert.ToInt32(pntEnd.Y / fileHandling.scale) * fileHandling.scale);

                    Line line = new Line();
                    line.Stroke = new SolidColorBrush(blueprintColor);
                    //line.Stroke = System.Windows.Media.Brushes.Black;
                    line.X1 = startPoint.X;
                    line.Y1 = startPoint.Y;
                    line.X2 = endPoint.X;
                    line.Y2 = endPoint.Y;
                    line.StrokeThickness = 2;
                    
                    DrawBoard.Children.Add(line);  
                }


                if (drawState == "First_ON") drawState = "ON";
                else if (drawState == "First_OFF")
                {
                    #region Saving new elements
                    if (drawType != "Solid Line")
                    {
                        GraphicElementModel element = new GraphicElementModel(fileHandling.gems.Count, drawType, "Test", startPoint, width, height, color, 2);    //TODO: stroke
                        fileHandling.gems.Add(element);
                        
                    }
                    else
                    {
                        if (!arrowPoints.Any()) arrowPoints.Add(new Point(startPoint.X / fileHandling.scale, startPoint.Y / fileHandling.scale));
                        arrowPoints.Add(new Point(endPoint.X / fileHandling.scale, endPoint.Y / fileHandling.scale));
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
                Path path = new Path();
                path.Stroke = Brushes.Black;
                path.Fill = new SolidColorBrush(gem.ElementColor);
                path.StrokeThickness = 2;

                switch (gem.ElementType)
                {
                    case "Start/Stop":
                        Rect myRect = new Rect(gem.ElementStartingLocation.X * fileHandling.scale, gem.ElementStartingLocation.Y * fileHandling.scale, gem.ElementWidth * fileHandling.scale, gem.ElementHeight * fileHandling.scale);

                        path.Data = new RectangleGeometry(myRect, cornerRoundness * fileHandling.scale * gem.ElementHeight, cornerRoundness * fileHandling.scale * gem.ElementHeight);
                        break;

                    case "Input/Output":
                        PathFigure myPathFigure = new PathFigure();
                        myPathFigure.StartPoint = new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (width / 7)) * fileHandling.scale, Convert.ToInt32(gem.ElementStartingLocation.Y) * fileHandling.scale);
                        myPathFigure.Segments.Add(
                            new LineSegment(
                                new Point(Convert.ToInt32(gem.ElementStartingLocation.X + gem.ElementWidth) * fileHandling.scale, Convert.ToInt32(gem.ElementStartingLocation.Y) * fileHandling.scale),
                                true /* IsStroked */ ));
                        myPathFigure.Segments.Add(
                            new LineSegment(
                                new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 7 * 6)) * fileHandling.scale, Convert.ToInt32(gem.ElementStartingLocation.Y + gem.ElementHeight) * fileHandling.scale),
                                true /* IsStroked */ ));
                        myPathFigure.Segments.Add(
                            new LineSegment(
                                new Point(Convert.ToInt32(gem.ElementStartingLocation.X) * fileHandling.scale, Convert.ToInt32(gem.ElementStartingLocation.Y + gem.ElementHeight) * fileHandling.scale),
                                true /* IsStroked */ ));
                        myPathFigure.Segments.Add(
                            new LineSegment(
                                new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 7)) * fileHandling.scale, Convert.ToInt32(gem.ElementStartingLocation.Y) * fileHandling.scale),
                                true /* IsStroked */ ));

                        PathGeometry myPathGeometry = new PathGeometry();
                        myPathGeometry.Figures.Add(myPathFigure);

                        path.Data = myPathGeometry;
                        break;

                    case "Process":
                        Rect myRect2 = new Rect(gem.ElementStartingLocation.X * fileHandling.scale, gem.ElementStartingLocation.Y * fileHandling.scale, gem.ElementWidth * fileHandling.scale, gem.ElementHeight * fileHandling.scale);

                        path.Data = new RectangleGeometry(myRect2);
                        break;

                    case "Decision":
                        PathFigure myPathFigure2 = new PathFigure();
                        myPathFigure2.StartPoint = new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 2)) * fileHandling.scale, Convert.ToInt32(gem.ElementStartingLocation.Y) * fileHandling.scale);
                        myPathFigure2.Segments.Add(
                            new LineSegment(
                                new Point(Convert.ToInt32(gem.ElementStartingLocation.X + gem.ElementWidth) * fileHandling.scale, Convert.ToInt32((gem.ElementStartingLocation.Y * 2) + gem.ElementHeight) * fileHandling.scale / 2),
                                true /* IsStroked */ ));
                        myPathFigure2.Segments.Add(
                            new LineSegment(
                                new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 2)) * fileHandling.scale, Convert.ToInt32(gem.ElementStartingLocation.Y + gem.ElementHeight) * fileHandling.scale),
                                true /* IsStroked */ ));
                        myPathFigure2.Segments.Add(
                            new LineSegment(
                                new Point(Convert.ToInt32(gem.ElementStartingLocation.X) * fileHandling.scale, Convert.ToInt32((gem.ElementStartingLocation.Y * 2) + gem.ElementHeight) * fileHandling.scale / 2),
                                true /* IsStroked */ ));
                        myPathFigure2.Segments.Add(
                            new LineSegment(
                                new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 2)) * fileHandling.scale, Convert.ToInt32(gem.ElementStartingLocation.Y) * fileHandling.scale),
                                true /* IsStroked */ ));

                        PathGeometry myPathGeometry2 = new PathGeometry();
                        myPathGeometry2.Figures.Add(myPathFigure2);

                        path.Data = myPathGeometry2;
                        break;
                }

                DrawBoard.Children.Add(path);
            }

            foreach (ArrowsModel arrowsModel in fileHandling.arrows.ToList())
            {
                Path path = new Path();
                path.Stroke = Brushes.Black;
                path.StrokeThickness = 2;

                PathFigure myPathFigureLine = new PathFigure();
                myPathFigureLine.StartPoint = new Point(arrowsModel.points[0].X * fileHandling.scale, arrowsModel.points[0].Y * fileHandling.scale);

                foreach (var _point in arrowsModel.points)
                {
                    if (_point != arrowsModel.points[0])
                    {
                        myPathFigureLine.Segments.Add(new LineSegment(new Point(_point.X * fileHandling.scale, _point.Y * fileHandling.scale), true));
                    }
                }

                Point absCoordinates = new Point(arrowsModel.points.Last().X - arrowsModel.points[arrowsModel.points.Count()-2].X, arrowsModel.points.Last().Y - arrowsModel.points[arrowsModel.points.Count() - 2].Y);
                double angle =  Math.Atan2(absCoordinates.Y, absCoordinates.X) * 180.0 / Math.PI;
                Point arrowHeadLeft = new Point((arrowsModel.points.Last().X + (Math.Cos((angle + 135) * Math.PI / 180) / 8)) * fileHandling.scale, (arrowsModel.points.Last().Y + (Math.Sin((angle + 135) * Math.PI / 180) / 8)) * fileHandling.scale);
                Point arrowHeadRight = new Point((arrowsModel.points.Last().X + (Math.Cos((angle + 225) * Math.PI / 180) / 8)) * fileHandling.scale, (arrowsModel.points.Last().Y + (Math.Sin((angle + 225) * Math.PI / 180) / 8)) * fileHandling.scale);

                myPathFigureLine.Segments.Add(new LineSegment(arrowHeadLeft, true));
                myPathFigureLine.Segments.Add(new LineSegment(new Point(arrowsModel.points.Last().X * fileHandling.scale, arrowsModel.points.Last().Y * fileHandling.scale), true));
                myPathFigureLine.Segments.Add(new LineSegment(arrowHeadRight, true));
                myPathFigureLine.Segments.Add(new LineSegment(new Point(arrowsModel.points.Last().X * fileHandling.scale, arrowsModel.points.Last().Y * fileHandling.scale), true));

                PathGeometry myPathGeometryLine = new PathGeometry();
                myPathGeometryLine.Figures.Add(myPathFigureLine);

                path.Data = myPathGeometryLine;

                DrawBoard.Children.Add(path);
            }

            if(arrowPoints.Any())
            {
                Path path = new Path();
                path.Stroke = Brushes.Black;
                path.StrokeThickness = 2;

                PathFigure myPathFigureLine = new PathFigure();
                myPathFigureLine.StartPoint = new Point(arrowPoints[0].X * fileHandling.scale, arrowPoints[0].Y * fileHandling.scale);

                foreach (var _point in arrowPoints)
                {
                    if (_point != arrowPoints[0])
                    {
                        myPathFigureLine.Segments.Add(new LineSegment(new Point(_point.X * fileHandling.scale, _point.Y * fileHandling.scale), true));
                    }
                }
                PathGeometry myPathGeometryLine = new PathGeometry();
                myPathGeometryLine.Figures.Add(myPathFigureLine);

                path.Data = myPathGeometryLine;

                DrawBoard.Children.Add(path);
            }
        }
        #endregion

        private void ArrowCreation()
        {
            List<Point> WHY_ARE_THIS_NEEDED = new List<Point>();
            foreach (Point _point in arrowPoints)
            {
                WHY_ARE_THIS_NEEDED.Add(_point);
            }

            ArrowsModel element = new ArrowsModel(WHY_ARE_THIS_NEEDED, "Solid Line");
            fileHandling.arrows.Add(element);
            arrowPoints.Clear();
            UpdateElements();
        }

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