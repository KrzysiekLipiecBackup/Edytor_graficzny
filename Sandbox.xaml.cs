using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Edytor_graficzny
{
    /// <summary>
    /// Interaction logic for Sandbox.xaml
    /// </summary>
    public partial class Sandbox : Window
    {
        public Sandbox()
        {
			InitializeComponent();
			//TextBlock tb = new TextBlock();
			//tb.TextWrapping = TextWrapping.Wrap;
			//tb.Margin = new Thickness(10);
			//tb.Inlines.Add("An example on ");
			//tb.Inlines.Add(new Run("the TextBlock control ") { FontWeight = FontWeights.Bold });
			//tb.Inlines.Add("using ");
			//tb.Inlines.Add(new Run("inline ") { FontStyle = FontStyles.Italic });
			//tb.Inlines.Add(new Run("text formatting ") { Foreground = Brushes.Blue });
			//tb.Inlines.Add("from ");
			//tb.Inlines.Add(new Run("Code-Behind") { TextDecorations = TextDecorations.Underline });
			//tb.Inlines.Add(".");
			//this.Content = tb;
		}


		private void btnClickMe_Click(object sender, RoutedEventArgs e)
		{
			lbResult.Items.Add(pnlMain.FindResource("strPanel").ToString());
			lbResult.Items.Add(this.FindResource("strWindow").ToString());
			lbResult.Items.Add(Application.Current.FindResource("strApp").ToString());
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			string s = null;
			try
			{
				s.Trim();
			}
			catch (Exception ex)
			{
				MessageBox.Show("A handled exception just occurred: " + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
			s.Trim();
		}

		private void ReadFile_Click(object sender, RoutedEventArgs e)
		{
			using (StreamReader sr = File.OpenText(@"D:\Programowanie\Visual Studio C# WPF\Edytor graficzny\Res\Text\StartText.txt"))
            {
				string s = "";
				while ((s = sr.ReadLine()) != null)
				{
					lbResult.Items.Add(s);
				}
			}
		}

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
		}


		private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			txtStatus.Text = "Selection starts at character #" + textBox.SelectionStart + Environment.NewLine;
			txtStatus.Text += "Selection is " + textBox.SelectionLength + " character(s) long" + Environment.NewLine;
			txtStatus.Text += "Selected text: '" + textBox.SelectedText + "'";
		}
		private void cbAllFeatures_CheckedChanged(object sender, RoutedEventArgs e)
		{
			bool newVal = (cbAllFeatures.IsChecked == true);
			cbFeatureAbc.IsChecked = newVal;
			cbFeatureXyz.IsChecked = newVal;
			cbFeatureWww.IsChecked = newVal;
		}

		private void cbFeature_CheckedChanged(object sender, RoutedEventArgs e)
		{
			cbAllFeatures.IsChecked = null;
			if ((cbFeatureAbc.IsChecked == true) && (cbFeatureXyz.IsChecked == true) && (cbFeatureWww.IsChecked == true))
				cbAllFeatures.IsChecked = true;
			if ((cbFeatureAbc.IsChecked == false) && (cbFeatureXyz.IsChecked == false) && (cbFeatureWww.IsChecked == false))
				cbAllFeatures.IsChecked = false;
		}

		private void BtnLoadFromFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				Uri fileUri = new Uri(openFileDialog.FileName);
				imgDynamic.Source = new BitmapImage(fileUri);
			}
		}
		private void BtnLoadFromResource_Click(object sender, RoutedEventArgs e)
		{
			//Uri resourceUri = new Uri(new Uri(Directory.GetCurrentDirectory(), UriKind.Absolute), new Uri(@"/Res/Images/watermark.ico", UriKind.Relative));
			//imgDynamic.Source = new BitmapImage(resourceUri);

			//imgDynamic.Source = new BitmapImage(new Uri(@"/Res/Images/watermark.ico", UriKind.Relative));

			//imgDynamic.Source = new BitmapImage(new Uri("ms-appx:///Res/Images/watermark.ico"));
		}

        private void btnMainWindow_Click(object sender, RoutedEventArgs e)
        {
			MainWindow mW = new MainWindow();
			mW.Show();
			this.Close();
        }

        private void lbResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}