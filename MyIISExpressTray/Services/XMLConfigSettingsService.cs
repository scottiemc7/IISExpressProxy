using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;

namespace IISExpressProxy.Services
{
	class XMLConfigSettingsService : EncryptingSettingsService
	{
		private readonly string XMLPATH = String.Format("{0}\\{1}\\{2}.xml", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyIISExpressTray", "config");
		private readonly XmlDocument _settingsDoc;
		private XMLConfigSettingsService()
		{
			string dirPath = Path.GetDirectoryName(XMLPATH);
			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);

			_settingsDoc = new XmlDocument();
			if (!File.Exists(XMLPATH))
			{
				_settingsDoc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration></configuration>");
				_settingsDoc.Save(XMLPATH);
			}
			else
				_settingsDoc.Load(XMLPATH);
		}

		private static ISettingsService _instance;
		public static ISettingsService Instance
		{
			get
			{
				if (_instance == null)
					_instance = new XMLConfigSettingsService();

				return _instance;
			}
		}

		public override string Get(string settingName)
		{
			XmlNode node = _settingsDoc.SelectSingleNode(String.Format("/configuration/setting[@name='{0}']", settingName));
			if (node != null)
				return node.InnerText;
			else
				return null;
		}

		public override void Set(string settingName, string value)
		{
			XmlNode node = _settingsDoc.SelectSingleNode(String.Format("/configuration/setting[@name='{0}']", settingName));
			if (node != null)
				node.InnerText = value;
			else
			{
				XmlNode root = _settingsDoc.SelectSingleNode("/configuration");
				XmlNode setting = _settingsDoc.CreateElement("setting");
				XmlAttribute attribute = _settingsDoc.CreateAttribute("name");
				attribute.Value = settingName;
				setting.InnerText = value;
				setting.Attributes.Append(attribute);
				root.AppendChild(setting);
			}//end if

			_settingsDoc.Save(XMLPATH);
		}
	}
}
