using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IISExpressProxy.Models
{
	interface IAppBrowser
	{
		void ViewApp(string url);
	}
}
