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
        private double cornerRoundness = 0.14;
        private bool isGridActive = false;
        private string drawType = "Start/Stop";
        private string drawState = "OFF";   // First_ON, ON, First_OFF, OFF
        private readonly FileHandling fileHandling = new FileHandling();
        private List<Point> arrowPoints = new List<Point>();
        private Color[,] buttonsColors = new Color[,] {  { Color.FromRgb(0, 0, 0), Color.FromRgb(170, 170, 170) },
                                            { Color.FromRgb(63, 63, 63), Color.FromRgb(255, 255, 255) },
                                            { Color.FromRgb(204, 0, 0), Color.FromRgb(229, 123, 123) },
                                            { Color.FromRgb(255, 86, 0), Color.FromRgb(255, 170, 127) },
                                            { Color.FromRgb(255, 153, 0), Color.FromRgb(255, 204, 127) },
                                            { Color.FromRgb(255, 230, 0), Color.FromRgb(255, 242, 127) },
                                            { Color.FromRgb(130, 255, 0), Color.FromRgb(192, 255, 127) },
                                            { Color.FromRgb(0, 204, 88), Color.FromRgb(127, 229, 171) },
                                            { Color.FromRgb(0, 206, 255), Color.FromRgb(127, 230, 255) },
                                            { Color.FromRgb(10, 84, 245), Color.FromRgb(132, 169, 250) },
                                            { Color.FromRgb(143, 0, 214), Color.FromRgb(199, 127, 234) },
                                            { Color.FromRgb(230, 0, 172), Color.FromRgb(242, 127, 213) } };
        private int customColorsCounter = 0;
        private string arrowType;
        private List<int> undoList = new List<int>();   //0 - element; 1 - arrow
        private List<int> redoList = new List<int>();
        private List<GraphicElementModel> redoGEM = new List<GraphicElementModel>();
        private List<ArrowsModel> redoArrow = new List<ArrowsModel>();


        public MainWindow()
        {
            InitializeComponent();
            currentTool.Text = "Current tool: " + drawType;
            btnStartStop.IsEnabled = false;
            btnFinishArrow.IsEnabled = false;

            fileHandling.OpenConfiguration();
            ColorTopRow();
            UpdateElements();
        }

        #region ColorButtons
        private void ColorTopRow()
        {
            btnColorRow1Column1.Background = new SolidColorBrush(buttonsColors[0, 0]);
            btnColorRow1Column2.Background = new SolidColorBrush(buttonsColors[1, 0]);
            btnColorRow1Column3.Background = new SolidColorBrush(buttonsColors[2, 0]);
            btnColorRow1Column4.Background = new SolidColorBrush(buttonsColors[3, 0]);
            btnColorRow1Column5.Background = new SolidColorBrush(buttonsColors[4, 0]);
            btnColorRow1Column6.Background = new SolidColorBrush(buttonsColors[5, 0]);
            btnColorRow1Column7.Background = new SolidColorBrush(buttonsColors[6, 0]);
            btnColorRow1Column8.Background = new SolidColorBrush(buttonsColors[7, 0]);
            btnColorRow1Column9.Background = new SolidColorBrush(buttonsColors[8, 0]);
            btnColorRow1Column10.Background = new SolidColorBrush(buttonsColors[9, 0]);
            btnColorRow1Column11.Background = new SolidColorBrush(buttonsColors[10, 0]);
            btnColorRow1Column12.Background = new SolidColorBrush(buttonsColors[11, 0]);

            btnColorRow2Column1.Background = new SolidColorBrush(buttonsColors[0, 1]);
            btnColorRow2Column2.Background = new SolidColorBrush(buttonsColors[1, 1]);
            btnColorRow2Column3.Background = new SolidColorBrush(buttonsColors[2, 1]);
            btnColorRow2Column4.Background = new SolidColorBrush(buttonsColors[3, 1]);
            btnColorRow2Column5.Background = new SolidColorBrush(buttonsColors[4, 1]);
            btnColorRow2Column6.Background = new SolidColorBrush(buttonsColors[5, 1]);
            btnColorRow2Column7.Background = new SolidColorBrush(buttonsColors[6, 1]);
            btnColorRow2Column8.Background = new SolidColorBrush(buttonsColors[7, 1]);
            btnColorRow2Column9.Background = new SolidColorBrush(buttonsColors[8, 1]);
            btnColorRow2Column10.Background = new SolidColorBrush(buttonsColors[9, 1]);
            btnColorRow2Column11.Background = new SolidColorBrush(buttonsColors[10, 1]);
            btnColorRow2Column12.Background = new SolidColorBrush(buttonsColors[11, 1]);
        }
        private void btnColorRow3Column1_Click(object sender, RoutedEventArgs e) {}
        private void btnColorRow3Column2_Click(object sender, RoutedEventArgs e) {}

        private void btnColorAll_Click(object sender, RoutedEventArgs e) 
        {
            var test = (sender as Button).Name;
            string[] parts = test.Split("wCn".ToCharArray());
            if (Convert.ToInt32(parts[3]) <= 2) color = buttonsColors[Convert.ToInt32(parts[5]) - 1, Convert.ToInt32(parts[3]) - 1];
            else color = fileHandling.customButtonsColors[Convert.ToInt32(parts[5]) - 1];
            switch (drawType)
            {
                case "Start/Stop":
                    fileHandling.toolColor[0] = color;
                    break;
                case "Input/Output":
                    fileHandling.toolColor[1] = color;
                    break;
                case "Process":
                    fileHandling.toolColor[2] = color;
                    break;
                case "Decision":
                    fileHandling.toolColor[3] = color;
                    break;
            }
            btnColorActive.Background = new SolidColorBrush(color);
            UpdateElements();
        }

        private void btnColorPicker_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.AllowFullOpen = true;
            colorDialog.ShowHelp = false;
            colorDialog.AnyColor = true;

            colorDialog.ShowDialog();

            System.Drawing.Color c = colorDialog.Color;
            color = Color.FromRgb(c.R, c.G, c.B);

            fileHandling.customButtonsColors[customColorsCounter] = color;
            customColorsCounter++;
            if (customColorsCounter == 12) customColorsCounter = 0;
            UpdateElements();
        }
        #endregion

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

        private void MenuItem_File_OpenConfiguration(object sender, RoutedEventArgs e)
        {
            fileHandling.OpenConfiguration();
            UpdateElements();
        }

        private void MenuItem_File_SaveConfiguration(object sender, RoutedEventArgs e)
        {
            fileHandling.SaveConfiguration();
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
            if (undoList.Any())
            {
                redoList.Add(undoList.Last());
                if (undoList.Last() == 0)
                {
                    redoGEM.Add(fileHandling.gems[fileHandling.gems.Count - 1]);
                    fileHandling.gems.RemoveAt(fileHandling.gems.Count - 1);
                }
                else 
                {
                    redoArrow.Add(fileHandling.arrows[fileHandling.arrows.Count - 1]);
                    fileHandling.arrows.RemoveAt(fileHandling.arrows.Count - 1);
                }
                undoList.RemoveAt(undoList.Count - 1);
            }
            UpdateElements();          
        }

        private void MenuItem_Redo_Click(object sender, RoutedEventArgs e)
        {
            if (redoList.Any())
            {
                undoList.Add(redoList.Last());
                if (redoList.Last() == 0)
                {
                    fileHandling.gems.Add(redoGEM[redoGEM.Count - 1]);
                    redoGEM.RemoveAt(redoGEM.Count - 1);
                }
                else
                {
                    fileHandling.arrows.Add(redoArrow[redoArrow.Count - 1]);
                    redoArrow.RemoveAt(redoArrow.Count - 1);
                }
                redoList.RemoveAt(redoList.Count - 1);
            }
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
            GraphicElementModel testStartStop = new GraphicElementModel(0, "Start/Stop", "test", new Point(1, 1), 6, 2, fileHandling.toolColor[0], 1);
            GraphicElementModel testIO = new GraphicElementModel(1, "Input/Output", "test", new Point(9, 1), 6, 2, fileHandling.toolColor[1], 1);
            GraphicElementModel testProcess = new GraphicElementModel(2, "Process", "test", new Point(1, 5), 6, 2, fileHandling.toolColor[2], 1);
            GraphicElementModel testDecision = new GraphicElementModel(3, "Decision", "test", new Point(9, 5), 6, 2, fileHandling.toolColor[3], 1);

            fileHandling.gems.Add(testStartStop);
            fileHandling.gems.Add(testIO);
            fileHandling.gems.Add(testProcess);
            fileHandling.gems.Add(testDecision);

            List<Point> testList = new List<Point>();
            testList.Add(new Point(4, 3));
            testList.Add(new Point(4, 5));
            ArrowsModel testArrow = new ArrowsModel(testList, "Solid Line");

            List<Point> testList2 = new List<Point>();
            testList2.Add(new Point(12, 3));
            testList2.Add(new Point(12, 4));
            testList2.Add(new Point(4, 4));
            ArrowsModel testArrow2 = new ArrowsModel(testList2, "Dashed Line");

            List<Point> testList3 = new List<Point>();
            testList3.Add(new Point(12, 4));
            testList3.Add(new Point(12, 5));
            ArrowsModel testArrow3 = new ArrowsModel(testList3, "Solid Line");

            fileHandling.arrows.Add(testArrow2);
            fileHandling.arrows.Add(testArrow);
            fileHandling.arrows.Add(testArrow3);

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
            color = fileHandling.toolColor[0];
            ButtonControll(sender.ToString());
            btnStartStop.IsEnabled = false;
            UpdateElements();
        }
        private void btnIO_Click(object sender, RoutedEventArgs e)
        {
            color = fileHandling.toolColor[1];
            ButtonControll(sender.ToString());
            btnIO.IsEnabled = false;
            UpdateElements();
        }
        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            color = fileHandling.toolColor[2];
            ButtonControll(sender.ToString());
            btnProcess.IsEnabled = false;
            UpdateElements();
        }
        private void btnDecision_Click(object sender, RoutedEventArgs e)
        {
            color = fileHandling.toolColor[3];
            ButtonControll(sender.ToString());
            btnDecision.IsEnabled = false;
            UpdateElements();
        }
        private void btnSolidLine_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnSolidLine.IsEnabled = false;
            btnFinishArrow.IsEnabled = true;
        }
        private void btnDashedLine_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnDashedLine.IsEnabled = false;
            btnFinishArrow.IsEnabled = true;
        }
        private void btnFinishArrow_Click(object sender, RoutedEventArgs e)
        {
            arrowType = drawType;
            ButtonControll(sender.ToString());
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
            btnEllipse.IsEnabled = true;

            buttonName.Trim();
            string[] parts = buttonName.Split(":".ToCharArray());

            drawType = parts[1].Trim();
            currentTool.Text = "Current tool: " + drawType;

            if (drawType != "Solid Line" && drawType != "Dashed Line") ArrowCreation();
        }
        #endregion
            
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                fileHandling.width = Convert.ToDouble(txtWidth.Text);
                txtWidth.Text = "";
            }
            catch {}

            try
            {
                fileHandling.height = Convert.ToDouble(txtHeight.Text);
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

                if (drawType != "Solid Line" && drawType != "Dashed Line")
                {
                    startPoint = new Point(Convert.ToInt32(pntStart.X / fileHandling.scale - fileHandling.width / 2), Convert.ToInt32(pntStart.Y / fileHandling.scale - fileHandling.height / 2));
                    if (pntEnd != null) startPoint = new Point(Convert.ToInt32(pntEnd.X / fileHandling.scale - fileHandling.width / 2), Convert.ToInt32(pntEnd.Y / fileHandling.scale - fileHandling.height / 2));


                    Path path = new Path();
                    path.Stroke = new SolidColorBrush(blueprintColor);
                    path.StrokeThickness = 2;

                    switch (drawType)
                    {
                        case "Start/Stop":
                            Rect myRect = new Rect(startPoint.X * fileHandling.scale, startPoint.Y * fileHandling.scale, fileHandling.width * fileHandling.scale, fileHandling.height * fileHandling.scale);

                            path.Data = new RectangleGeometry(myRect, cornerRoundness * fileHandling.scale * fileHandling.height, cornerRoundness * fileHandling.scale * fileHandling.height);
                            break;

                        case "Input/Output":
                            PathFigure myPathFigure = new PathFigure();
                            myPathFigure.StartPoint = new Point(Convert.ToInt32(startPoint.X + (fileHandling.width / 7)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale);
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (fileHandling.width)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale),
                                    true /* IsStroked */ ));
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (fileHandling.width / 7 * 6)) * fileHandling.scale, Convert.ToInt32(startPoint.Y + (fileHandling.height)) * fileHandling.scale),
                                    true /* IsStroked */ ));
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X) * fileHandling.scale, Convert.ToInt32(startPoint.Y + (fileHandling.height)) * fileHandling.scale),
                                    true /* IsStroked */ ));
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (fileHandling.width / 7)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale),
                                    true /* IsStroked */ ));

                            PathGeometry myPathGeometry = new PathGeometry();
                            myPathGeometry.Figures.Add(myPathFigure);
                            path.Data = myPathGeometry;

                            break;

                        case "Process":
                            Rect myRect2 = new Rect(startPoint.X * fileHandling.scale, startPoint.Y * fileHandling.scale, fileHandling.width * fileHandling.scale, fileHandling.height * fileHandling.scale);

                            path.Data = new RectangleGeometry(myRect2);
                            break;

                        case "Decision":
                            PathFigure myPathFigure2 = new PathFigure();
                            myPathFigure2.StartPoint = new Point(Convert.ToInt32(startPoint.X + (fileHandling.width / 2)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale);
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + fileHandling.width) * fileHandling.scale, Convert.ToInt32((startPoint.Y * 2) + fileHandling.height) * fileHandling.scale / 2),
                                    true /* IsStroked */ ));
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (fileHandling.width / 2)) * fileHandling.scale, Convert.ToInt32(startPoint.Y + fileHandling.height) * fileHandling.scale),
                                    true /* IsStroked */ ));
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X) * fileHandling.scale, Convert.ToInt32((startPoint.Y * 2) + fileHandling.height) * fileHandling.scale / 2),
                                    true /* IsStroked */ ));
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (fileHandling.width / 2)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale),
                                    true /* IsStroked */ ));

                            PathGeometry myPathGeometry2 = new PathGeometry();
                            myPathGeometry2.Figures.Add(myPathFigure2);

                            path.Data = myPathGeometry2;
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
                    line.X1 = startPoint.X;
                    line.Y1 = startPoint.Y;
                    line.X2 = endPoint.X;
                    line.Y2 = endPoint.Y;
                    if(drawType == "Dashed Line") line.StrokeDashArray = new DoubleCollection() { 2, 2 };
                    line.StrokeThickness = 2;
                    
                    DrawBoard.Children.Add(line);  
                }


                if (drawState == "First_ON") drawState = "ON";
                else if (drawState == "First_OFF")
                {
                    #region Saving new elements
                    if (drawType != "Solid Line" && drawType != "Dashed Line")
                    {
                        GraphicElementModel element = new GraphicElementModel(fileHandling.gems.Count, drawType, "Test", startPoint, fileHandling.width, fileHandling.height, color, 2);    //TODO: stroke
                        fileHandling.gems.Add(element);
                        undoList.Add(0);
                        redoList.Clear();
                        redoGEM.Clear();
                        redoArrow.Clear();
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
                currentTool.Text = "Elements count: " + Convert.ToString(DrawBoard.Children.Count);
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
                        myPathFigure.StartPoint = new Point(Convert.ToInt32(gem.ElementStartingLocation.X + (gem.ElementWidth / 7)) * fileHandling.scale, Convert.ToInt32(gem.ElementStartingLocation.Y) * fileHandling.scale);
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
                PathGeometry myPathGeometryLine = new PathGeometry();
                myPathGeometryLine.Figures.Add(myPathFigureLine);
                if (arrowsModel.arrowType == "Dashed Line") path.StrokeDashArray = new DoubleCollection() { 2, 2 };
                path.Data = myPathGeometryLine;

                DrawBoard.Children.Add(path);



                Path path2 = new Path();
                path2.Stroke = Brushes.Black;
                path2.StrokeThickness = 2;

                PathFigure myPathFigureLineTip = new PathFigure();
                myPathFigureLineTip.StartPoint = new Point(arrowsModel.points.Last().X * fileHandling.scale, arrowsModel.points.Last().Y * fileHandling.scale);

                Point absCoordinates = new Point(arrowsModel.points.Last().X - arrowsModel.points[arrowsModel.points.Count()-2].X, arrowsModel.points.Last().Y - arrowsModel.points[arrowsModel.points.Count() - 2].Y);
                double angle =  Math.Atan2(absCoordinates.Y, absCoordinates.X) * 180.0 / Math.PI;
                Point arrowHeadLeft = new Point((arrowsModel.points.Last().X + (Math.Cos((angle + 135) * Math.PI / 180) / 8)) * fileHandling.scale, (arrowsModel.points.Last().Y + (Math.Sin((angle + 135) * Math.PI / 180) / 8)) * fileHandling.scale);
                Point arrowHeadRight = new Point((arrowsModel.points.Last().X + (Math.Cos((angle + 225) * Math.PI / 180) / 8)) * fileHandling.scale, (arrowsModel.points.Last().Y + (Math.Sin((angle + 225) * Math.PI / 180) / 8)) * fileHandling.scale);

                myPathFigureLineTip.Segments.Add(new LineSegment(arrowHeadLeft, true));
                myPathFigureLineTip.Segments.Add(new LineSegment(new Point(arrowsModel.points.Last().X * fileHandling.scale, arrowsModel.points.Last().Y * fileHandling.scale), true));
                myPathFigureLineTip.Segments.Add(new LineSegment(arrowHeadRight, true));
                myPathFigureLineTip.Segments.Add(new LineSegment(new Point(arrowsModel.points.Last().X * fileHandling.scale, arrowsModel.points.Last().Y * fileHandling.scale), true));

                PathGeometry myPathGeometryLineTip = new PathGeometry();
                myPathGeometryLineTip.Figures.Add(myPathFigureLineTip);
                path2.Data = myPathGeometryLineTip;
                
                DrawBoard.Children.Add(path2);
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

                if (drawType == "Dashed Line") path.StrokeDashArray = new DoubleCollection() { 2, 2 };
                path.Data = myPathGeometryLine;

                DrawBoard.Children.Add(path);
            }

            txtCurrentWidth.Text = "  Current width: " + fileHandling.width.ToString();
            txtCurrentHeight.Text = "  Current height: " + fileHandling.height.ToString();
            txtCurrentScale.Text = "  Current scale: " + fileHandling.scale.ToString();
            txtCurrentGridItems.Text = "  Current number: " + (fileHandling.gridItemsPerCM * 12).ToString() + "; each crate: " + 1 / fileHandling.gridItemsPerCM + "cm";

            btnColorRow3Column1.Background = new SolidColorBrush(fileHandling.customButtonsColors[0]);
            btnColorRow3Column2.Background = new SolidColorBrush(fileHandling.customButtonsColors[1]);
            btnColorRow3Column3.Background = new SolidColorBrush(fileHandling.customButtonsColors[2]);
            btnColorRow3Column4.Background = new SolidColorBrush(fileHandling.customButtonsColors[3]);
            btnColorRow3Column5.Background = new SolidColorBrush(fileHandling.customButtonsColors[4]);
            btnColorRow3Column6.Background = new SolidColorBrush(fileHandling.customButtonsColors[5]);
            btnColorRow3Column7.Background = new SolidColorBrush(fileHandling.customButtonsColors[6]);
            btnColorRow3Column8.Background = new SolidColorBrush(fileHandling.customButtonsColors[7]);
            btnColorRow3Column9.Background = new SolidColorBrush(fileHandling.customButtonsColors[8]);
            btnColorRow3Column10.Background = new SolidColorBrush(fileHandling.customButtonsColors[9]);
            btnColorRow3Column11.Background = new SolidColorBrush(fileHandling.customButtonsColors[10]);
            btnColorRow3Column12.Background = new SolidColorBrush(fileHandling.customButtonsColors[11]);

            btnColorActive.Background = new SolidColorBrush(color);
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
            if (arrowType == "Dashed Line") element.arrowType = "Dashed Line";
            fileHandling.arrows.Add(element);

            undoList.Add(1);
            redoList.Clear();
            redoGEM.Clear();
            redoArrow.Clear();

            arrowPoints.Clear();
            UpdateElements();
        }
    }
}


//case "Solid Line":
//    PathFigure myPathFigure3 = new PathFigure();
//    myPathFigure3.StartPoint = new Point(Convert.ToInt32(startPoint.X + (fileHandling.width / 7)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale);
//    myPathFigure3.Segments.Add(
//        new LineSegment(
//            new Point(Convert.ToInt32(startPoint.X + (fileHandling.width)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale),
//            true /* IsStroked */ ));
//    myPathFigure3.Segments.Add(
//        new LineSegment(
//            new Point(Convert.ToInt32(startPoint.X + (fileHandling.width / 7 * 6)) * fileHandling.scale, Convert.ToInt32(startPoint.Y + (fileHandling.height)) * fileHandling.scale),
//            true /* IsStroked */ ));
//    myPathFigure3.Segments.Add(
//        new LineSegment(
//            new Point(Convert.ToInt32(startPoint.X) * fileHandling.scale, Convert.ToInt32(startPoint.Y + (fileHandling.height)) * fileHandling.scale),
//            true /* IsStroked */ ));
//    myPathFigure3.Segments.Add(
//        new LineSegment(
//            new Point(Convert.ToInt32(startPoint.X + (fileHandling.width / 7)) * fileHandling.scale, Convert.ToInt32(startPoint.Y) * fileHandling.scale),
//            true /* IsStroked */ ));

//    PathGeometry myPathGeometry3 = new PathGeometry();
//    myPathGeometry3.Figures.Add(myPathFigure3);

//    path.Data = myPathGeometry3;
//    break;

//case "Ellipse":
//    SolidColorBrush mySolidColorBrush = new SolidColorBrush
//    {
//        Color = Color.FromArgb(255, this.color.R, this.color.G, this.color.B)
//    };

//    Ellipse myEllipse = new Ellipse
//    {
//        Fill = mySolidColorBrush,
//        StrokeThickness = 2,
//        Stroke = Brushes.Black,
//        Width = Math.Abs(pntStart.X - pntEnd.X),
//        Height = Math.Abs(pntStart.Y - pntEnd.Y)
//    };

//    DrawBoard.Children.Add(myEllipse);

//    Canvas.SetLeft(myEllipse, (pntStart.X < pntEnd.X) ? pntStart.X : pntEnd.X);
//    Canvas.SetTop(myEllipse, (pntStart.Y < pntEnd.Y) ? pntStart.Y : pntEnd.Y);
//    break;