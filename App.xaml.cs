using System.Windows;
using AppointmentSystem.ViewModels;

namespace AppointmentSystem
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Set the main window data context
            var mainWindow = this.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.DataContext = new MainViewModel();
            }
        }
    }
}
