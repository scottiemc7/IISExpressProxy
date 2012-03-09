using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IISExpressProxy.Models
{
	interface IApplicationHost : IDisposable
	{
		List<string> AvailableApplications { get; }
		List<string> AvailablePools { get; }

		void HostApplication(string appPoolName, string siteName, int port);
		void EndCurrentApplication();
	}
}
