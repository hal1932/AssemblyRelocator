using AssemblyRelocator.Models;
using HlLib.CommandLine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AssemblyRelocator
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static string TargetAssembly { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var (options, args) = ParseCommandLine();
            TargetAssembly = args[1];

            if (options.NoWindow)
            {
            }
            else
            {
                new Bootstrapper().Run();
            }
        }

        private (CmdOptions options, string[] args) ParseCommandLine()
        {
            var parser = new CommandLineOptions<CmdOptions>();

            string[] args;
            var options = parser.Parse(Environment.GetCommandLineArgs(), out args);

            return (options: options, args: args);
        }
    }
}
