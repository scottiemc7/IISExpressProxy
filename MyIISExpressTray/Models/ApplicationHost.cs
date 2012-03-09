using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security;
using System.IO;

namespace IISExpressProxy.Models
{
	class ApplicationHost : IApplicationHost, IDisposable
	{
		private readonly IApplicationHostConfig _config;
		private Process _currentProcess;
		private string _currentConfig;
		public ApplicationHost(IApplicationHostConfig config)
		{
			_config = config;
		}

		public void HostApplication(string appPoolName, string siteName, int port)
		{
			if(!AvailableApplications.Contains(siteName) || ! AvailablePools.Contains(appPoolName))
				throw new ArgumentException("Unknown application or pool name");

			if(port < 1 || port > 65535)
				throw new ArgumentException("Invalid port number. Must be between 1 and 65535.", "port");

			_currentConfig = _config.CreateConfigDocWithSingleSite(siteName, appPoolName, port);

			var psi = new ProcessStartInfo
			{
				FileName = String.Format("{0}\\{1}\\{2}", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "IIS Express", "iisexpress.exe"),								
				Domain = Environment.MachineName,
				Arguments = String.Format("/site:{0} /config:\"{1}\"", siteName, _currentConfig),				
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = false
			};

			_currentProcess = Process.Start(psi);			
		}

		public void EndCurrentApplication()
		{
			if (_currentProcess != null && !_currentProcess.HasExited)
			{
				_currentProcess.Kill();
				_currentProcess.Dispose();
				_currentProcess = null;
			}//end if

			if (!String.IsNullOrEmpty(_currentConfig) && File.Exists(_currentConfig))
				File.Delete(_currentConfig);
			_currentConfig = null;
		}

		private List<string> _availableApps;
		public List<string> AvailableApplications
		{
			get
			{
				if (_availableApps == null)
					_availableApps = _config.GetAvailableSites();

				return _availableApps;
			}
		}

		private List<string> _availablePools;
		public List<string> AvailablePools
		{
			get
			{
				if (_availablePools == null)
					_availablePools = _config.GetApplicationPools();

				return _availablePools;
			}
		}

		public void Dispose()
		{
			EndCurrentApplication();

			GC.SuppressFinalize(this);
		}
	}
}
