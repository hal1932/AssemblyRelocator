using Microsoft.Practices.Prism.MefExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AssemblyRelocator
{
    class Bootstrapper : MefBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return new Views.Shell();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            (Application.Current.MainWindow = Shell as Window).Show();
        }
    }
}
