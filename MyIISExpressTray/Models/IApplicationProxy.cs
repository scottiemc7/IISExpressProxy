using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IISExpressProxy.Models
{
	interface IApplicationProxy : IDisposable
	{
		void End();
		void Start(int internalPort, int externalPort);
	}
}
