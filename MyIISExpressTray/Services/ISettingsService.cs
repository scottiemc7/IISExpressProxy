using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IISExpressProxy.Services
{
	interface ISettingsService
	{
		string Get(string settingName);
		string GetDecrypted(string settingName);
		void Set(string settingName, string value);
		void SetEncrypted(string settingName, string value);
	}
}
