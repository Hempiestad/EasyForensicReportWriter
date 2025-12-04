using System.Windows;

namespace EasyForensicReportWriter
{
    public partial class App : Application
    {
    }

    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var app = new App();
            app.InitializeComponent();
            app.Run(new MainWindow());
        }
    }
}