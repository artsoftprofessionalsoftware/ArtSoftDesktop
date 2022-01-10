using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArtSoftDesktop
{
    class Program
    {
        public static string initProfile = null;

        [STAThread]
        static void Main(string[] args)
        {
            Application app = new Application();
            app.StartupUri = new Uri("MainWindow.xaml", System.UriKind.Relative);
            if (args.Length > 0)
            {
                for(int i = 0; i < args.Length; i += 2)
                {
                    string arg = args[i];
                    if(arg == "--profile" || arg == "-p")
                    {
                        string val = args[i + 1];
                        initProfile = val;
                        break;
                    }
                }
            }
            app.Run();
        }
    }
}
