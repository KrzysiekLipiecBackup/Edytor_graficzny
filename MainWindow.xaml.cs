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
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int iter = 0;
        private Point pnt_start;
        private Point pnt_end;
        private Byte color_red;
        private Byte color_green;
        private Byte color_blue;
        private string draw_type = "line";
        private string draw_state = "OFF";   // First_ON, ON, First_OFF, OFF
        private Random rnd = new Random();


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
            pnt_start = e.GetPosition(DrawBoard);

            color_red = Convert.ToByte(rnd.Next(50, 255));
            color_blue = Convert.ToByte(rnd.Next(50, 255));
            color_green = Convert.ToByte(rnd.Next(50, 255));

            if (draw_state == "First_OFF" || draw_state == "OFF") draw_state = "First_ON";
        }

        private void DrawBoard_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                if (draw_state == "First_ON" || draw_state == "ON") draw_state = "First_OFF";
            }
            pnt_end = e.GetPosition(DrawBoard);
            Drawnado();
        }

        private void btnPen_Click(object sender, RoutedEventArgs e)
        {
            draw_type = "pen";
            currentTool.Text = "Current tool: " + draw_type;
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

        private void Drawnado()
        {
            switch (draw_type)
            {
                case "pen":
                    Draw_Pen();
                    break;
                case "line":
                    Draw_Line();
                    break;
                case "ellipse":
                    Draw_Ellipse();
                    break;
                default:
                    Console.WriteLine("!!!Used Default draw_type (Pen)");
                    Draw_Pen();
                    break;
            }
            currentTool.Text = Convert.ToString(DrawBoard.Children.Count);
        }

        private void Draw_Pen()
        {

        }
        private void Draw_Line()
        {
            if (draw_state != "OFF")    //draw_state First_Off
            {
                if (draw_state == "ON" || draw_state == "First_OFF")
                {
                    DrawBoard.Children.RemoveAt(DrawBoard.Children.Count - 1);
                }

                Line myLine = new Line();
                myLine.Stroke = Brushes.SteelBlue;
                myLine.X1 = pnt_start.X;
                myLine.X2 = pnt_end.X;
                myLine.Y1 = pnt_start.Y;
                myLine.Y2 = pnt_end.Y;
                myLine.StrokeThickness = 5;

                DrawBoard.Children.Add(myLine);

                if (draw_state == "First_ON") draw_state = "ON";
                else if(draw_state == "First_OFF") draw_state = "OFF";
            }
         }

        private void Draw_Ellipse()
        {
            if (draw_state != "OFF")
            {
                if (draw_state == "ON" || draw_state == "First_OFF")
                {
                    DrawBoard.Children.RemoveAt(DrawBoard.Children.Count - 1);
                }

                Ellipse myEllipse = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(color_red, color_blue, color_green, 0);
                myEllipse.Fill = mySolidColorBrush;
                myEllipse.StrokeThickness = 2;
                myEllipse.Stroke = Brushes.Black;
                myEllipse.Width = Math.Abs(pnt_start.X - pnt_end.X);
                myEllipse.Height = Math.Abs(pnt_start.Y - pnt_end.Y);

                DrawBoard.Children.Add(myEllipse);
                
                Canvas.SetLeft(myEllipse, (pnt_start.X < pnt_end.X) ? pnt_start.X : pnt_end.X);
                Canvas.SetTop(myEllipse, (pnt_start.Y < pnt_end.Y) ? pnt_start.Y : pnt_end.Y);

                if (draw_state == "First_ON") draw_state = "ON";
                else if(draw_state == "First_OFF") draw_state = "OFF";
            }
        }
    }


}