using System;
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
        private Byte colorRed;
        private Byte colorGreen;
        private Byte colorBlue;
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
            fileHandling.OpenFile();
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

            colorRed = Convert.ToByte(randomNumber.Next(50, 255));
            colorBlue = Convert.ToByte(randomNumber.Next(50, 255));
            colorGreen = Convert.ToByte(randomNumber.Next(50, 255));

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
                        myLine.Stroke = Brushes.SteelBlue;
                        myLine.X1 = pntStart.X;
                        myLine.X2 = pntEnd.X;
                        myLine.Y1 = pntStart.Y;
                        myLine.Y2 = pntEnd.Y;
                        myLine.StrokeThickness = 5;

                        DrawBoard.Children.Add(myLine);
                        break;
                    case "ellipse":
                        Ellipse myEllipse = new Ellipse();
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                        mySolidColorBrush.Color = Color.FromArgb(colorRed, colorBlue, colorGreen, 0);
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
    }
}