using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using IISExpressProxy.Services;

namespace IISExpressProxy.Models
{
	class ApplicationHostConfig : IApplicationHostConfig
	{
		//private const string CONFIGFILENAME = "applicationhost.config";
		//private const string IISROOTDIRNAME = "IISExpress";
		//private const string IISCONFIGDIRNAME = "config";

		private readonly string _configFileSetting;
		private readonly ISettingsService _settings;
		public ApplicationHostConfig(ISettingsService settingsSvc, string configFilePathSettingName)
		{
			_configFileSetting = configFilePathSettingName;
			_settings = settingsSvc;
		}

		public List<string> GetApplicationPools()
		{
			List<string> ret = new List<string>();
			XmlDocument configFile = GetConfigDoc();
			if (configFile == null)
				return ret;

			//Read app pools
			foreach (XmlNode xmlNode in configFile.SelectNodes("/configuration/system.applicationHost/applicationPools/add"))
				ret.Add(xmlNode.Attributes["name"].Value);

			return ret;
		}

		public List<string> GetAvailableSites()
		{
			List<string> ret = new List<string>();
			XmlDocument configFile = GetConfigDoc();
			if (configFile == null)
				return ret;

			//Read available sites
			foreach (XmlNode xmlNode in configFile.SelectNodes("/configuration/system.applicationHost/sites/site"))
				ret.Add(xmlNode.Attributes["name"].Value);

			return ret;
		}

		public string CreateConfigDocWithSingleSite(string siteName, string poolName, int port)
		{
			XmlDocument configFile = GetConfigDoc();

			//Find pool node
			XmlNode poolNode = configFile.SelectSingleNode(String.Format("/configuration/system.applicationHost/applicationPools/add[@name='{0}']", poolName));
			if(poolNode == null)
				throw new ArgumentException("Unknown application pool name", "poolName");

			//Remove all sites except for one specified
			XmlNode rootNode = configFile.SelectSingleNode("/configuration/system.applicationHost/sites");
			foreach (XmlNode xmlNode in configFile.SelectNodes("/configuration/system.applicationHost/sites/site"))
			{
				if (String.Compare(xmlNode.Attributes["name"].Value, siteName, true) != 0)
					rootNode.RemoveChild(xmlNode);
				else
				{
					xmlNode.SelectSingleNode("application").Attributes["applicationPool"].Value = poolName;
					xmlNode.SelectSingleNode("bindings/binding[@protocol='http']").Attributes["bindingInformation"].Value = String.Format("*:{0}:", port);
				}//end if
			}//end foreach

			string path = String.Format("{0}\\{1}\\{2}.xml", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyIISExpressTray", Guid.NewGuid());
			configFile.Save(path);

			return path;
		}

		private XmlDocument GetConfigDoc()
		{
			//List<string> ret = new List<string>();
			//string path = String.Format("{0}\\{1}\\{2}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), IISROOTDIRNAME, IISCONFIGDIRNAME);
			//if (!Directory.Exists(path))
			//    return null;

			//Open config file
			XmlDocument configFile = new XmlDocument();
			string path = _settings.Get(_configFileSetting);
			if (!String.IsNullOrEmpty(path) && !path.EndsWith("applicationhost.config"))
				path += "\\applicationhost.config";

			if (File.Exists(path))
				configFile.Load(path);

			return configFile;
		}
	}
}
