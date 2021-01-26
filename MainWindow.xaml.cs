using Edytor_graficzny.Models;
using Edytor_graficzny.Src;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Edytor_graficzny
{
    public partial class MainWindow : Window
    {
        private Point pntStart;
        private Point pntEnd;
        private Color color = Color.FromRgb(255, 180, 180);
        private bool isGridActive = false;
        private bool selected = false;
        DrawType previousDrawType = DrawType.StartStop;
        DrawType currentDrawType = DrawType.StartStop;
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
        private List<int> undoList = new List<int>();   //0 - element; 1 - arrow
        private List<int> redoList = new List<int>();
        private List<GraphicElementModel> redoGEM = new List<GraphicElementModel>();
        private List<ArrowsModel> redoArrow = new List<ArrowsModel>();

        public MainWindow()
        {
            InitializeComponent();
            currentTool.Text = "Current tool: " + currentDrawType;
            btnStartStop.IsEnabled = false;
            btnFinish.IsEnabled = false;

            fileHandling.OpenConfiguration();
            ColorTopRow();
            UpdateElements(true);
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

        private void btnColorAll_Click(object sender, RoutedEventArgs e) 
        {
            var test = (sender as Button).Name;
            string[] parts = test.Split("wCn".ToCharArray());
            if (Convert.ToInt32(parts[3]) <= 2) color = buttonsColors[Convert.ToInt32(parts[5]) - 1, Convert.ToInt32(parts[3]) - 1];
            else color = fileHandling.customButtonsColors[Convert.ToInt32(parts[5]) - 1];
            switch (currentDrawType)
            {
                case DrawType.StartStop:
                    fileHandling.toolColor[0] = color;
                    break;
                case DrawType.InputOutput:
                    fileHandling.toolColor[1] = color;
                    break;
                case DrawType.Process:
                    fileHandling.toolColor[2] = color;
                    break;
                case DrawType.Decision:
                    fileHandling.toolColor[3] = color;
                    break;
            }
            btnColorActive.Background = new SolidColorBrush(color);
            UpdateElements(true);
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
            UpdateElements(true);
        }
        #endregion

        #region MenuButtons
        private void MenuItem_File_OpenClick(object sender, RoutedEventArgs e)
        {
            fileHandling.gems.Clear();
            fileHandling.OpenFile(fileHandling.gems);
            UpdateElements(true);
        }
        private void MenuItem_File_SaveClick(object sender, RoutedEventArgs e)
        {
            fileHandling.SaveFile();
        }
        private void MenuItem_File_OpenConfiguration(object sender, RoutedEventArgs e)
        {
            fileHandling.OpenConfiguration();
            UpdateElements(true);
        }
        private void MenuItem_File_SaveConfiguration(object sender, RoutedEventArgs e)
        {
            fileHandling.SaveConfiguration();
        }
        private void MenuItem_File_ExitClick(object sender, RoutedEventArgs e)
        {
            fileHandling.SaveConfiguration();
            Application.Current.Shutdown();
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
            UpdateElements(true);
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
            UpdateElements(true);
        }
        private void MenuItem_btnTest_Click(object sender, RoutedEventArgs e)
        {
            isGridActive = true;
            GraphicElementModel testStartStop = new GraphicElementModel(0, "StartStop", "test", new Point(1, 1), 6, 2, fileHandling.toolColor[0], 1);
            GraphicElementModel testIO = new GraphicElementModel(1, "InputOutput", "test", new Point(9, 1), 6, 2, fileHandling.toolColor[1], 1);
            GraphicElementModel testProcess = new GraphicElementModel(2, "Process", "test", new Point(1, 5), 6, 2, fileHandling.toolColor[2], 1);
            GraphicElementModel testDecision = new GraphicElementModel(3, "Decision", "test", new Point(9, 5), 6, 2, fileHandling.toolColor[3], 1);

            fileHandling.gems.Add(testStartStop);
            fileHandling.gems.Add(testIO);
            fileHandling.gems.Add(testProcess);
            fileHandling.gems.Add(testDecision);

            List<Point> testList = new List<Point>();
            testList.Add(new Point(4, 3));
            testList.Add(new Point(4, 5));
            ArrowsModel testArrow = new ArrowsModel(testList, "SolidLine");

            List<Point> testList2 = new List<Point>();
            testList2.Add(new Point(12, 3));
            testList2.Add(new Point(12, 4));
            testList2.Add(new Point(4, 4));
            ArrowsModel testArrow2 = new ArrowsModel(testList2, "DashedLine");

            List<Point> testList3 = new List<Point>();
            testList3.Add(new Point(12, 4));
            testList3.Add(new Point(12, 5));
            ArrowsModel testArrow3 = new ArrowsModel(testList3, "SolidLine");

            fileHandling.arrows.Add(testArrow2);
            fileHandling.arrows.Add(testArrow);
            fileHandling.arrows.Add(testArrow3);

            UpdateElements(true);
        }
        #endregion

        #region MouseButtonsAndMove
        private void DrawBoard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition((UIElement)sender);
            List<GraphicElementModel> reverseGems = new List<GraphicElementModel>(fileHandling.gems);
            reverseGems.Reverse();
            if (selected) selected = false;

            foreach (GraphicElementModel rG in reverseGems)
            {
                if (pt.X >= rG.ElementStartingLocation.X * Variables.scale && pt.X <= (rG.ElementStartingLocation.X + rG.ElementWidth) * Variables.scale)
                {
                    if (pt.Y >= rG.ElementStartingLocation.Y * Variables.scale && pt.Y <= (rG.ElementStartingLocation.Y + rG.ElementHeight) * Variables.scale)
                    {
                        selected = true;
                        fileHandling.gems.Remove(rG);
                        fileHandling.gems.Add(rG);

                        UpdateElements(true);
                        DrawBoard.Children.Add(ElementsDrawing.DrawingHelpers(pt, rG));
                        foreach (Rectangle rectangle in ElementsDrawing.SelectedElement(rG.ElementStartingLocation, rG.ElementWidth, rG.ElementHeight))
                        {
                            DrawBoard.Children.Add(rectangle);
                        }

                        break;
                    }
                }
            }

            if (!selected)
            {
                pntStart = e.GetPosition(DrawBoard);
                if (drawState == "First_OFF" || drawState == "OFF") drawState = "First_ON";
                Drawnado();
            }
        }

        private void DrawBoard_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (drawState == "First_ON" || drawState == "ON") drawState = "First_OFF";
            Drawnado();
        }

        private void DrawBoard_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        }

        private void DrawBoard_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
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
        #endregion

        #region Tools buttons handling
        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            color = fileHandling.toolColor[0];
            ButtonControll(sender.ToString());
            btnStartStop.IsEnabled = false;
            UpdateElements(true);
        }
        private void btnIO_Click(object sender, RoutedEventArgs e)
        {
            color = fileHandling.toolColor[1];
            ButtonControll(sender.ToString());
            btnIO.IsEnabled = false;
            UpdateElements(true);
        }
        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            color = fileHandling.toolColor[2];
            ButtonControll(sender.ToString());
            btnProcess.IsEnabled = false;
            UpdateElements(true);
        }
        private void btnDecision_Click(object sender, RoutedEventArgs e)
        {
            color = fileHandling.toolColor[3];
            ButtonControll(sender.ToString());
            btnDecision.IsEnabled = false;
            UpdateElements(true);
        }
        private void btnSolidLine_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnSolidLine.IsEnabled = false;
            btnFinish.IsEnabled = true;
        }
        private void btnDashedLine_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnDashedLine.IsEnabled = false;
            btnFinish.IsEnabled = true;
        }
        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            ButtonControll(sender.ToString());
            btnFinish.IsEnabled = false;
        }

        private void ButtonControll(string buttonName)
        {
            btnStartStop.IsEnabled = true;
            btnIO.IsEnabled = true;
            btnProcess.IsEnabled = true;
            btnDecision.IsEnabled = true;
            btnSolidLine.IsEnabled = true;
            btnDashedLine.IsEnabled = true;
            previousDrawType = currentDrawType;

            buttonName.Trim();
            string[] parts = buttonName.Split(":".ToCharArray());
            switch (parts[1].Trim())
            {
                case "Start/Stop":
                    currentDrawType = DrawType.StartStop;
                    break;
                case "Input/Output":
                    currentDrawType = DrawType.InputOutput;
                    break;
                case "Process":
                    currentDrawType = DrawType.Process;
                    break;
                case "Decision":
                    currentDrawType = DrawType.Decision;
                    break;
                case "Solid Line":
                    currentDrawType = DrawType.SolidLine;
                    break;
                case "Dashed Line":
                    currentDrawType = DrawType.DashedLine;
                    break;
                case "Finish":
                    currentDrawType = DrawType.None;
                    break;
                default:
                    currentDrawType = DrawType.None;
                    break;
            }
            currentTool.Text = "Current tool: " + currentDrawType;

            if (currentDrawType != DrawType.SolidLine && currentDrawType != DrawType.DashedLine && (previousDrawType == DrawType.SolidLine || previousDrawType == DrawType.DashedLine)) ArrowCreation();
        }
        #endregion

        #region Others Buttons
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Variables.width = Convert.ToDouble(txtWidth.Text);
                txtWidth.Text = "";
            }
            catch { }

            try
            {
                Variables.height = Convert.ToDouble(txtHeight.Text);
                txtHeight.Text = "";
            }
            catch { }

            try
            {
                Variables.scale = Convert.ToDouble(txtScale.Text);
                txtScale.Text = "";
            }
            catch { }

            try
            {
                Variables.gridItemsPerCM = Convert.ToDouble(txtGridItems.Text) / 12;
                txtCurrentGridItems.Text = "";
            }
            catch { }

            try
            {
                Variables.fontSize = Convert.ToInt32(txtFontSize.Text);
                txtCurrentFontSize.Text = "";
            }
            catch { }

            UpdateElements(true);
        }

        #region Grid Drawing
        private void btnGrid_Click(object sender, RoutedEventArgs e)
        {
            isGridActive = !isGridActive;
            UpdateElements(true);
        }

        private void GridDrawing()
        {
            for (int i = 0; i < (DrawBoard.ActualHeight - 1) / Variables.scale; i++)
            {
                Path grid = new Path();
                grid.Stroke = Brushes.Black;
                grid.Opacity = 0.3;
                grid.StrokeThickness = 0.5;
                LineGeometry lineGeometry = new LineGeometry(new Point(0, i * Variables.scale), new Point(DrawBoard.ActualWidth, i * Variables.scale));
                grid.Data = lineGeometry;
                DrawBoard.Children.Add(grid);
            }
            for (int i = 0; i < (DrawBoard.ActualWidth - 1) / Variables.scale; i++)
            {
                Path grid = new Path();
                grid.Stroke = Brushes.Black;
                grid.Opacity = 0.3;
                grid.StrokeThickness = 0.5;
                LineGeometry lineGeometry = new LineGeometry(new Point(i * Variables.scale, 0), new Point(i * Variables.scale, DrawBoard.ActualHeight));
                grid.Data = lineGeometry;
                DrawBoard.Children.Add(grid);
            }
            Path borderLine = new Path();
            borderLine.Stroke = Brushes.Black;
            borderLine.Opacity = 0.3;
            borderLine.StrokeThickness = 2;
            LineGeometry lineBorderLine = new LineGeometry(new Point(Variables.gridItemsPerCM * 12 * Variables.scale, 0), new Point(Variables.gridItemsPerCM * 12 * Variables.scale, DrawBoard.ActualHeight));
            borderLine.Data = lineBorderLine;
            DrawBoard.Children.Add(borderLine);
        }
        #endregion

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            fileHandling.gems.Clear();
            fileHandling.arrows.Clear();
            arrowPoints.Clear();
            undoList.Clear();
            redoList.Clear();
            redoGEM.Clear();
            redoArrow.Clear();
            UpdateElements(true);
        }

        private void btnDynamicFontSize_Click(object sender, RoutedEventArgs e)
        {
            Variables.isDynamicFontActive = !Variables.isDynamicFontActive;
            UpdateElements(true);
        }
        #endregion

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

                if (currentDrawType != DrawType.SolidLine && currentDrawType != DrawType.DashedLine)
                {
                    startPoint = new Point(Convert.ToInt32(pntStart.X / Variables.scale - Variables.width / 2), Convert.ToInt32(pntStart.Y / Variables.scale - Variables.height / 2));
                    if (pntEnd != null) startPoint = new Point(Convert.ToInt32(pntEnd.X / Variables.scale - Variables.width / 2), Convert.ToInt32(pntEnd.Y / Variables.scale - Variables.height / 2));


                    Path path = new Path();
                    path.Stroke = new SolidColorBrush(blueprintColor);
                    path.StrokeThickness = 2;

                    switch (currentDrawType)
                    {
                        case DrawType.StartStop:
                            Rect myRect = new Rect(startPoint.X * Variables.scale, startPoint.Y * Variables.scale, Variables.width * Variables.scale, Variables.height * Variables.scale);

                            path.Data = new RectangleGeometry(myRect, Variables.cornerRoundness * Variables.scale * Variables.height, Variables.cornerRoundness * Variables.scale * Variables.height);
                            break;

                        case DrawType.InputOutput:
                            PathFigure myPathFigure = new PathFigure();
                            myPathFigure.StartPoint = new Point(Convert.ToInt32(startPoint.X + (Variables.width / 7)) * Variables.scale, Convert.ToInt32(startPoint.Y) * Variables.scale);
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (Variables.width)) * Variables.scale, Convert.ToInt32(startPoint.Y) * Variables.scale),
                                    true /* IsStroked */ ));
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (Variables.width / 7 * 6)) * Variables.scale, Convert.ToInt32(startPoint.Y + (Variables.height)) * Variables.scale),
                                    true /* IsStroked */ ));
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X) * Variables.scale, Convert.ToInt32(startPoint.Y + (Variables.height)) * Variables.scale),
                                    true /* IsStroked */ ));
                            myPathFigure.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (Variables.width / 7)) * Variables.scale, Convert.ToInt32(startPoint.Y) * Variables.scale),
                                    true /* IsStroked */ ));

                            PathGeometry myPathGeometry = new PathGeometry();
                            myPathGeometry.Figures.Add(myPathFigure);
                            path.Data = myPathGeometry;

                            break;

                        case DrawType.Process:
                            Rect myRect2 = new Rect(startPoint.X * Variables.scale, startPoint.Y * Variables.scale, Variables.width * Variables.scale, Variables.height * Variables.scale);

                            path.Data = new RectangleGeometry(myRect2);
                            break;

                        case DrawType.Decision:
                            PathFigure myPathFigure2 = new PathFigure();
                            myPathFigure2.StartPoint = new Point(Convert.ToInt32(startPoint.X + (Variables.width / 2)) * Variables.scale, Convert.ToInt32(startPoint.Y) * Variables.scale);
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + Variables.width) * Variables.scale, Convert.ToInt32((startPoint.Y * 2) + Variables.height) * Variables.scale / 2),
                                    true /* IsStroked */ ));
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (Variables.width / 2)) * Variables.scale, Convert.ToInt32(startPoint.Y + Variables.height) * Variables.scale),
                                    true /* IsStroked */ ));
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X) * Variables.scale, Convert.ToInt32((startPoint.Y * 2) + Variables.height) * Variables.scale / 2),
                                    true /* IsStroked */ ));
                            myPathFigure2.Segments.Add(
                                new LineSegment(
                                    new Point(Convert.ToInt32(startPoint.X + (Variables.width / 2)) * Variables.scale, Convert.ToInt32(startPoint.Y) * Variables.scale),
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
                    btnFinish.IsEnabled = true;
                    if (!arrowPoints.Any()) startPoint = new Point(Convert.ToInt32(pntStart.X / Variables.scale) * Variables.scale, Convert.ToInt32(pntStart.Y / Variables.scale) * Variables.scale);
                    else startPoint = new Point(arrowPoints.Last().X * Variables.scale, arrowPoints.Last().Y * Variables.scale);
                    

                    endPoint = new Point(Convert.ToInt32(pntStart.X / Variables.scale) * Variables.scale, Convert.ToInt32(pntStart.Y / Variables.scale) * Variables.scale);
                    if (pntEnd != null) endPoint = new Point(Convert.ToInt32(pntEnd.X / Variables.scale) * Variables.scale, Convert.ToInt32(pntEnd.Y / Variables.scale) * Variables.scale);

                    Line line = new Line();
                    line.Stroke = new SolidColorBrush(blueprintColor);
                    line.X1 = startPoint.X;
                    line.Y1 = startPoint.Y;
                    line.X2 = endPoint.X;
                    line.Y2 = endPoint.Y;
                    if(currentDrawType == DrawType.DashedLine) line.StrokeDashArray = new DoubleCollection() { 2, 2 };
                    line.StrokeThickness = 2;
                    
                    DrawBoard.Children.Add(line);  
                }


                if (drawState == "First_ON") drawState = "ON";
                else if (drawState == "First_OFF")
                {
                    #region Saving new elements
                    if (currentDrawType != DrawType.SolidLine && currentDrawType != DrawType.DashedLine)
                    {
                        GraphicElementModel element = new GraphicElementModel(fileHandling.gems.Count, currentDrawType.ToString(), "A bit longer text", startPoint, Variables.width, Variables.height, color, 2);    //TODO: stroke
                        fileHandling.gems.Add(element);
                        undoList.Add(0);
                        redoList.Clear();
                        redoGEM.Clear();
                        redoArrow.Clear();
                    }
                    else
                    {
                        if (!arrowPoints.Any()) arrowPoints.Add(new Point(startPoint.X / Variables.scale, startPoint.Y / Variables.scale));
                        arrowPoints.Add(new Point(endPoint.X / Variables.scale, endPoint.Y / Variables.scale));
                    }
                    #endregion

                    UpdateElements(true);

                    drawState = "OFF";
                }
                currentTool.Text = "Elements count: " + Convert.ToString(DrawBoard.Children.Count);
            }
        }


        #region Clear, then drawing grid -> elements -> arrows -> text
        private void UpdateElements(bool textDrawing)
        {
            DrawBoard.Children.Clear();
            if (isGridActive) GridDrawing();

            foreach (GraphicElementModel gem in fileHandling.gems.ToList())
            {
                DrawBoard.Children.Add(ElementsDrawing.UpdateGEMS(gem));
                DrawBoard.Children.Add(ElementsDrawing.UpdateTXT(gem));
            }

            foreach (ArrowsModel arrowsModel in fileHandling.arrows.ToList())
            {
                DrawBoard.Children.Add(ElementsDrawing.UpdateArrow(arrowsModel));
                DrawBoard.Children.Add(ElementsDrawing.UpdateArrowHead(arrowsModel));
            }

            if(arrowPoints.Any())
            {
                bool dashed = false;
                if (currentDrawType == DrawType.DashedLine) dashed = true;
                DrawBoard.Children.Add(ElementsDrawing.UpdateNewArrow(arrowPoints, dashed));
            }

            txtCurrentWidth.Text = "  Current width: " + Variables.width.ToString();
            txtCurrentHeight.Text = "  Current height: " + Variables.height.ToString();
            txtCurrentScale.Text = "  Current scale: " + Variables.scale.ToString();
            txtCurrentGridItems.Text = "  Current number: " + (Variables.gridItemsPerCM * 12).ToString() + "; each crate: " + 1 / Variables.gridItemsPerCM + "cm";
            if (Variables.isDynamicFontActive) txtCurrentFontSize.Text = "  Dynamic font is Active";
            else txtCurrentFontSize.Text = "  Current font: " + Variables.fontSize.ToString();

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
            ArrowsModel element = new ArrowsModel(WHY_ARE_THIS_NEEDED, "SolidLine");
            if (previousDrawType == DrawType.DashedLine) element.arrowType = "DashedLine";
            fileHandling.arrows.Add(element);

            undoList.Add(1);
            redoList.Clear();
            redoGEM.Clear();
            redoArrow.Clear();
            arrowPoints.Clear();

            UpdateElements(true);
        }
    }
}

public enum DrawType
{
    StartStop,
    InputOutput,
    Process,
    Decision,
    SolidLine,
    DashedLine,
    None
}