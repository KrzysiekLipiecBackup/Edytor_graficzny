using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//…more using statements


namespace Edytor_graficzny
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int iter = 0;
        private Point pnt_start;
        private Point pnt_end;
        private string draw_type = "line";
        public MainWindow()
        {
            InitializeComponent();
            currentTool.Text = "Current tool: " + draw_type;
        }

        private void MenuItem_Sandbox_Click(object sender, RoutedEventArgs e)
        {
            Sandbox s = new Sandbox();
            s.Top = 50;
            s.Left = 50;
            s.Show();
        }

        private void MenuItem_btnNewLineTest_Click(object sender, RoutedEventArgs e)
        {
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
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
            pnt_start = e.GetPosition(DrawBoard);
        }

        private void DrawBoard_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            pnt_end = e.GetPosition(DrawBoard);
            switch (draw_type)
            {
                case "line":
                    Draw_Line();
                    break;
                case "ellipse":
                    Draw_Ellipse();
                    break;
                default:
                    Console.WriteLine("!!!Used Default draw_type");
                    Draw_Line();
                    break;
            }
        }

        private void btnLine_Click(object sender, RoutedEventArgs e)
        {
            draw_type = "line";
            currentTool.Text = "Current tool: " + draw_type;
        }

        private void btnEllipse_Click(object sender, RoutedEventArgs e)
        {
            draw_type = "ellipse";
            currentTool.Text = "Current tool: " + draw_type;
        }

        private void Draw_Line()
        {
            Line myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.SteelBlue;
            myLine.X1 = pnt_start.X;
            myLine.X2 = pnt_end.X;
            myLine.Y1 = pnt_start.Y;
            myLine.Y2 = pnt_end.Y;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 5;
            DrawBoard.Children.Add(myLine);
        }

        private void Draw_Ellipse()
        {
            Ellipse myEllipse = new Ellipse();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 2;
            myEllipse.Stroke = Brushes.Black;
            myEllipse.Width = Math.Abs(pnt_start.X - pnt_end.X);
            myEllipse.Height = Math.Abs(pnt_start.Y - pnt_end.Y); ;         
            DrawBoard.Children.Add(myEllipse);
            Canvas.SetLeft(myEllipse, (pnt_start.X < pnt_end.X) ? pnt_start.X : pnt_end.X);
            Canvas.SetTop(myEllipse, (pnt_start.Y < pnt_end.Y) ? pnt_start.Y : pnt_end.Y);
        }

    }


}