using System.Windows;
using System.IO;
using Smallifier.Model.ConsoleHelper;

namespace Smallifier
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string[] args = e.Args;
            bool isValidPath = args.Length > 0 ? File.Exists(args[args.Length - 1]) : false;

            if (isValidPath)
            {
                ConsoleAllocator.ShowConsoleWindow();
                MainConsole app = new MainConsole(args);
                app.start();
            }
            else
            {
                new MainWindow().ShowDialog();
            }
            this.Shutdown();
        }
    }
}
