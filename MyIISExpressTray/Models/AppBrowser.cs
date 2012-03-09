using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IISExpressProxy.Models
{
	class AppBrowser : IAppBrowser
	{
		public void ViewApp(string url)
		{
			System.Diagnostics.Process.Start(url);
		}
	}
}
