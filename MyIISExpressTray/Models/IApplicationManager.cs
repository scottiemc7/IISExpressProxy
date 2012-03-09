using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IISExpressProxy.Models
{
	interface IApplicationManager : IDisposable
	{
		void EndCurrentHosting();
		void Host(string applicationName, string applicationPool, int externalPort, bool autoBrowseTo);
		
		List<string> AvailableApplications { get; }
		List<string> AvailableApplicationPools { get; }
		bool IsHosting { get; }
	}
}
