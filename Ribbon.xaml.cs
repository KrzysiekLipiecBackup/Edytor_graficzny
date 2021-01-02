using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Edytor_graficzny
{
    /// <summary>
    /// Interaction logic for Ribbon.xaml
    /// </summary>
    public partial class Ribbon : Window
    {
        public Ribbon()
        {
            InitializeComponent();
        }

        private void btnColorPicker_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            // Keeps the user from selecting a custom color.
            MyDialog.AllowFullOpen = false;
            // Allows the user to get help. (The default is false.)

            MyDialog.ShowHelp = true;
            // Sets the initial color select to the current text color.
            MyDialog.ShowDialog();
            
            System.Drawing.Color c = MyDialog.Color;
            System.Windows.Media.Color d = new Color();

            d.A = c.A;
            d.R = c.R;
            d.G = c.G;
            d.B = c.B;

            //MyDialog.Color = textBox1.ForeColor;

            //// Update the text box color if the user clicks OK 
            //if (MyDialog.ShowDialog() == DialogResult.OK)
            //    textBox1.ForeColor = MyDialog.Color;
        }
    }
}
