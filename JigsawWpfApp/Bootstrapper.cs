using JigsawWpfApp.Views;
using System.Windows;
using Prism.Modularity;
using Autofac;
using Prism.Autofac;

using JigsawWpfApp.Configs;
using JigsawWpfApp.ViewModels;

namespace JigsawWpfApp
{
    class Bootstrapper : AutofacBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
            Application.Current.Exit += delegate
            {
                Config.Instance.SaveToJson();
            };
        }

        protected override void ConfigureModuleCatalog()
        {
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;
            //moduleCatalog.AddModule(typeof(YOUR_MODULE));
        }
    }
}
