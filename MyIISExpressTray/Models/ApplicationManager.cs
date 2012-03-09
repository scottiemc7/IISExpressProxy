using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Net.NetworkInformation;
using System.Net;

namespace IISExpressProxy.Models
{
	class ApplicationManager : IApplicationManager
	{
		private readonly IApplicationHost _appHost;
		private readonly IAppBrowser _appBrowser;
		private readonly IApplicationProxy _appProxy;

		private ApplicationManager() { }
		public ApplicationManager(IApplicationHost host, IAppBrowser browser, IApplicationProxy proxy)
		{
			_appHost = host;
			_appBrowser = browser;
			_appProxy = proxy;
		}

		public bool IsHosting { get; private set; }

		public List<string> AvailableApplicationPools
		{
			get { return _appHost.AvailablePools; }
		}

		public List<string> AvailableApplications
		{
			get { return _appHost.AvailableApplications; }
		}


		public void Host(string applicationName, string applicationPool, int externalPort, bool autoBrowseTo)
		{
			if (IsHosting)
				return;

			IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
			List<int> openPorts = new List<int>();
			foreach (TcpConnectionInformation tcpConnectionInformation in properties.GetActiveTcpConnections())
				openPorts.Add(tcpConnectionInformation.LocalEndPoint.Port);
			foreach (IPEndPoint endpoint in properties.GetActiveTcpListeners())
				openPorts.Add(endpoint.Port);
			foreach (IPEndPoint endpoint in properties.GetActiveUdpListeners())
				openPorts.Add(endpoint.Port);

			openPorts.Sort();

			//Find first available port (not guaranteed)
			int internalPort = -1;
			for (int i = 3000; i <= 65535; i++)
			{
				if (!openPorts.Contains(i))
				{
					internalPort = i;
					break;
				}//end if
			}//end for

			if (internalPort == -1)
				throw new Exception("Unable to find open port");

			//Set up proxy, host app, browse to if appropriate
			_appProxy.Start(internalPort, externalPort);
			_appHost.HostApplication(applicationPool, applicationName, internalPort);
			if (autoBrowseTo)
				_appBrowser.ViewApp(BrowsableURL(externalPort));
			IsHosting = true;
		}

	

		private string BrowsableURL(int port)
		{
			return String.Format("http://{0}:{1}", "localhost", port);
		}

		public void EndCurrentHosting()
		{
			if (!IsHosting)
				return;

			_appProxy.End();
			_appHost.EndCurrentApplication();

			IsHosting = false;
		}

		public void Dispose()
		{
			_appHost.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
