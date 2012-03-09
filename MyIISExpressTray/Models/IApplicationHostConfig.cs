using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IISExpressProxy.Models
{
	interface IApplicationHostConfig
	{
		List<string> GetApplicationPools();
		List<string> GetAvailableSites();
		string CreateConfigDocWithSingleSite(string siteName, string poolName, int port);
	}
}
