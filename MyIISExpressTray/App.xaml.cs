using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Ninject;
using IISExpressProxy.Services;
using IISExpressProxy.Models;

namespace IISExpressProxy
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private IKernel _kernel;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			_kernel = new StandardKernel();
			_kernel.Bind<IApplicationHostConfig>().ToConstant(new ApplicationHostConfig(XMLConfigSettingsService.Instance, "ConfigPath"));
			_kernel.Bind<IApplicationHost>().To<ApplicationHost>();
			_kernel.Bind<IMainViewModel>().To<MainVM>();
			_kernel.Bind<IAppBrowser>().To<AppBrowser>();
			_kernel.Bind<IApplicationProxy>().To<AppProxy>();
			_kernel.Bind<IApplicationManager>().To<ApplicationManager>();
			_kernel.Bind<ISettingsService>().ToConstant(XMLConfigSettingsService.Instance);


			this.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
			this.MainWindow = new MainWindow();
			this.MainWindow.DataContext = _kernel.Get<IMainViewModel>();
			this.MainWindow.Show();
		}
	}
}
