using System;
using System.Collections.Generic;
using System.Windows;


namespace Edytor_graficzny
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			// Create the startup window
			MainWindow wnd = new MainWindow();
			// Do stuff here, e.g. to the window
			if (e.Args.Length == 1)
				MessageBox.Show("Found Secret #1\n\nAdded Parameter: \n\n" + e.Args[0]);
			wnd.Title = "Hello World";
			// Show the window
			wnd.Show();
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
			e.Handled = true;
		}
	}
}