using Edytor_graficzny.Models;
using Edytor_graficzny.Src;
using System;
using System.Collections.Generic;
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
        private List<GraphicElementModel> gems = new List<GraphicElementModel>();
        private int iter = 0;
        private Point pntStart;
        private Point pntEnd;
        private Byte colorRed = 255;
        private Byte colorGreen = 255;
        private Byte colorBlue = 255;
        private string drawType = "line";
        private string drawState = "OFF";   // First_ON, ON, First_OFF, OFF
        private readonly Random randomNumber = new Random();
        private readonly Src.FileHandling fileHandling = new Src.FileHandling();


        public MainWindow()
        {
            InitializeComponent();
            currentTool.Text = "Current tool: " + drawType;
            btnPen.IsEnabled = true;
            btnLine.IsEnabled = false;
            btnEllipse.IsEnabled = true;
        }

        private void MenuItem_File_NewClick(object sender, RoutedEventArgs e)
        {
            fileHandling.NewFile();
        }

        private void MenuItem_File_OpenClick(object sender, RoutedEventArgs e)
        {
            gems.Clear();
            DrawBoard.Children.Clear();
            fileHandling.OpenFile(gems);
            double sizeOfElementsX = 0;
            double sizeOfElementsY = 0;
            double scale = 0f;
            foreach (var gem in gems)
            {
                if (gem.startX + gem.width > sizeOfElementsX)
                {
                    sizeOfElementsX = gem.startX + gem.width;
                }
                if (gem.startY + gem.height > sizeOfElementsY)
                {
                    sizeOfElementsY = gem.startY + gem.height;
                }
            }
            scale = (DrawBoard.ActualWidth / sizeOfElementsX);
            if ((DrawBoard.ActualHeight / sizeOfElementsY) < scale)
            {
                scale = (DrawBoard.ActualHeight / sizeOfElementsY);
            }


            string tempDrawType = drawType;
            foreach (var gem in gems)
            {
                drawType = gem.name;
                pntStart.X = (gem.startX - gem.width) * scale;                            
                //pntStart.Y = DrawBoard.ActualHeight - ((gem.startY - (gem.height)) * scale);
                pntStart.Y = (sizeOfElementsY - gem.startY - gem.height) * scale;
                pntEnd.X = (gem.startX + gem.width) * scale;
                pntEnd.Y = (sizeOfElementsY - gem.startY + gem.height) * scale;
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

        private void DrawBoard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            pntStart = e.GetPosition(DrawBoard);
            if (drawState == "First_OFF" || drawState == "OFF") drawState = "First_ON";
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

        private void btnPen_Click(object sender, RoutedEventArgs e)
        {
            EnableAllButtons();
            btnPen.IsEnabled = false;
            drawType = "pen";
            currentTool.Text = "Current tool: " + drawType;
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

        private void EnableAllButtons()
        {
            btnPen.IsEnabled = true;
            btnLine.IsEnabled = true;
            btnEllipse.IsEnabled = true;
        }

        private void Drawnado()
        {
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
                    case "pen":
                        break;
                    case "line":
                        Line myLine = new Line();
                        myLine.Stroke = Brushes.DarkGray;
                        myLine.X1 = pntStart.X;
                        myLine.X2 = pntEnd.X;
                        myLine.Y1 = pntStart.Y;
                        myLine.Y2 = pntEnd.Y;
                        myLine.StrokeThickness = 2;

                        DrawBoard.Children.Add(myLine);
                        break;
                    case "ellipse":
                        Ellipse myEllipse = new Ellipse();
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        mySolidColorBrush.Color = Color.FromArgb(255, colorRed, colorGreen, colorBlue);
                        myEllipse.Fill = mySolidColorBrush;
                        myEllipse.StrokeThickness = 2;
                        myEllipse.Stroke = Brushes.Black;
                        myEllipse.Width = Math.Abs(pntStart.X - pntEnd.X);
                        myEllipse.Height = Math.Abs(pntStart.Y - pntEnd.Y);

                        DrawBoard.Children.Add(myEllipse);

                        Canvas.SetLeft(myEllipse, (pntStart.X < pntEnd.X) ? pntStart.X : pntEnd.X);
                        Canvas.SetTop(myEllipse, (pntStart.Y < pntEnd.Y) ? pntStart.Y : pntEnd.Y);
                        break;
                    default:
                        Console.WriteLine("TODO - add error message");
                        break;
                }

                if (drawState == "First_ON") drawState = "ON";
                else if (drawState == "First_OFF") drawState = "OFF";
                currentTool.Text = Convert.ToString(DrawBoard.Children.Count);
            }
        }

        private void btnColorBlack_Click(object sender, RoutedEventArgs e)
        {
            colorRed = 0;
            colorGreen = 0;
            colorBlue = 0;
        }

        private void btnColorRed_Click(object sender, RoutedEventArgs e)
        {
            colorRed = 237;
            colorGreen = 36;
            colorBlue = 28;
        }

        private void btnColorGreen_Click(object sender, RoutedEventArgs e)
        {
            colorRed = 47;
            colorGreen = 215;
            colorBlue = 97;
        }

        private void btnColorBlue_Click(object sender, RoutedEventArgs e)
        {
            colorRed = 0;
            colorGreen = 162;
            colorBlue = 232;
        }
    }
}