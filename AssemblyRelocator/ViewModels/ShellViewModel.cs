using AssemblyRelocator.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace AssemblyRelocator.ViewModels
{
    class ShellViewModel : BindableBase
    {
        #region AssemblyPath
        private string _AssemblyPath = App.TargetAssembly;
        public string AssemblyPath
        {
            get { return _AssemblyPath; }
            set { SetProperty(ref _AssemblyPath, value); }
        }
        #endregion

        #region ReferencedAssemblies
        private AssemblyReference[] _ReferencedAssemblies;
        public AssemblyReference[] ReferencedAssemblies
        {
            get { return _ReferencedAssemblies; }
            set { SetProperty(ref _ReferencedAssemblies, value); }
        }
        #endregion

        #region ReloadCommand
        private ICommand _ReloadCommand;
        public ICommand ReloadCommand
        {
            get { return _ReloadCommand ?? (_ReloadCommand = new DelegateCommand(async () => await ReloadAsync())); }
        }
        #endregion

        #region OpenCommand
        private ICommand _OpenCommand;
        public ICommand OpenCommand
        {
            get { return _OpenCommand ?? (_OpenCommand = new DelegateCommand<IList>(Open)); }
        }
        #endregion

        #region CopyPathCommand
        private ICommand _CopyPathCommand;
        public ICommand CopyPathCommand
        {
            get { return _CopyPathCommand ?? (_CopyPathCommand = new DelegateCommand<IList>(CopyPath)); }
        }
        #endregion

        #region OpenWithILSpyCommand
        private ICommand _OpenWithILSpyCommand;
        public ICommand OpenWithILSpyCommand
        {
            get { return _OpenWithILSpyCommand ?? (_OpenWithILSpyCommand = new DelegateCommand<IList>(OpenWithILSpy)); }
        }
        #endregion

        #region RelocateCommand
        private ICommand _RelocateCommand;
        public ICommand RelocateCommand
        {
            get { return _RelocateCommand ?? (_RelocateCommand = new DelegateCommand<IList>(Relocate)); }
        }
        #endregion

        public ShellViewModel()
        {
            InitializeAsync().ConfigureAwait(false);
        }

        private async Task InitializeAsync()
        {
            await ReloadAsync();
        }

        private async Task ReloadAsync()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            {
                var resolver = new AssemblyReferenceResolver();
                var references = await resolver.ResolveReferences(AssemblyPath);

                references = references.OrderBy(x => x.Location).ToArray();

                Application.Current.Dispatcher.Invoke(() => ReferencedAssemblies = references);
            }
            Mouse.OverrideCursor = null;
        }

        private void Open(IList items)
        {
            var directories = GetPaths(items).Select(x => Path.GetDirectoryName(x));
            foreach (var dir in directories)
            {
                Process.Start(dir);
            }
        }

        private void CopyPath(IList items)
        {
            var paths = items.Cast<AssemblyReference>()
                .Select(x => x.Location)
                .Where(x => File.Exists(x))
                .Distinct()
                .ToArray();
            Clipboard.SetText(string.Join(Environment.NewLine, paths));
        }

        private void OpenWithILSpy(IList items)
        {
            var paths = GetPaths(items);
            if (ILSpyProxy.Open(paths) == null)
            {
                MessageBox.Show("設定ファイルに書いてある ILSpy.exe を起動できません");
            }
        }

        private void Relocate(IList items)
        {
            var dialog = new CommonOpenFileDialog()
            {
                DefaultDirectory = Path.GetDirectoryName(AssemblyPath),
                EnsurePathExists = true,
                IsFolderPicker = true,
                Multiselect = false,
                Title = "フォルダを選んでください",
            };
            var result = dialog.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
            {
                return;
            }

            Console.WriteLine(dialog.FileName);
       }

        private string[] GetPaths(IList items)
        {
            string[] paths;
            if (items == null)
            {
                paths = new[] { AssemblyPath };
            }
            else
            {
                paths = items.Cast<AssemblyReference>()
                    .Select(x => x.Location)
                    .Where(x => File.Exists(x))
                    .Distinct()
                    .ToArray();
            }
            return paths;
        }
    }
}
