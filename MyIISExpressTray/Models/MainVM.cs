using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using IISExpressProxy.Services;

namespace IISExpressProxy.Models
{
	class MainVM : BaseVM, IMainViewModel
	{		
		private string _appConfigPath;
		private bool _autoBrowse;
		private string _selectedSite;
		private string _selectedPool;
		private string _cmdText = "Host";
		private int _selectedExternalPort;

		private readonly IApplicationManager _appManager;
		private readonly ISettingsService _settingsService;
		public MainVM(IApplicationManager appManager, ISettingsService settingsSvc)
		{
			_appManager = appManager;
			_settingsService = settingsSvc;

			int portNum = 1;
			if (Int32.TryParse(settingsSvc.Get("LastExternalPort"), out portNum))
				_selectedExternalPort = portNum;
			else
				_selectedExternalPort = 1;

			string lastSite = settingsSvc.Get("LastSiteName");
			if (!String.IsNullOrEmpty(lastSite) && AvailableSites.Contains(lastSite))
				_selectedSite = lastSite;
			else
				_selectedSite = AvailableSites.Count > 0 ? AvailableSites[0] : null;
			 
			string lastPool = settingsSvc.Get("LastPoolName");
			if (!String.IsNullOrEmpty(lastPool) && AvailableSites.Contains(lastPool))
				_selectedPool = lastPool;
			else
				_selectedPool = AvailablePools.Count > 0 ? AvailablePools[0] : null;

			bool autoBrowse = true;
			if (Boolean.TryParse(_settingsService.Get("LastAutoBrowse"), out autoBrowse))
				_autoBrowse = autoBrowse;
			else
				_autoBrowse = true;

			AppConfigPath = _settingsService.Get("ConfigPath");
		}

		#region Properties
		
		public List<string> AvailableSites
		{
			get
			{
				return _appManager.AvailableApplications;
			}
		}

		public List<string> AvailablePools
		{
			get
			{
				return _appManager.AvailableApplicationPools;
			}
		}

		public string SelectedSite
		{
			get
			{
				return _selectedSite;
			}
			set
			{
				_settingsService.Set("LastSiteName", value);
				_selectedSite = value;
				NotifiyPropertyChanged("SelectedSite");
			}
		}

		public string SelectedPool
		{
			get
			{
				return _selectedPool;
			}
			set
			{
				_settingsService.Set("LastPoolName", value);
				_selectedPool = value;
				NotifiyPropertyChanged("SelectedPool");
			}
		}

		public string CurrentCommandText
		{
			get
			{
				return _cmdText;
			}
			set
			{
				_cmdText = value;
				NotifiyPropertyChanged("CurrentCommandText");
			}
		}

		public bool AutoBrowse
		{
			get
			{
				return _autoBrowse;
			}
			set
			{
				_settingsService.Set("LastAutoBrowse", value.ToString());
				_autoBrowse = value;
				NotifiyPropertyChanged("AutoBrowse");
			}
		}

		public string AppConfigPath
		{
			get
			{
				return _appConfigPath;
			}
			set
			{
				_settingsService.Set("ConfigPath", value);
				_appConfigPath = value;
				NotifiyPropertyChanged("AppConfigPath");
			}
		}

		[Range(1, 65535)]
		public int SelectedExternalPort
		{
			get
			{
				return _selectedExternalPort;
			}
			set
			{
				_settingsService.Set("LastExternalPort", value.ToString());
				_selectedExternalPort = value;
				NotifiyPropertyChanged("SelectedExternalPort");
			}
		}
		#endregion


		#region Commands


		private ICommand _exCommand;
		public ICommand ExCommand
		{
			get
			{
				if (_exCommand == null)
					_exCommand = new Cmd(p => ExecuteEx(p), p => CanExecuteEx());

				return _exCommand;
			}
		}

		private bool CanExecuteEx()
		{
			return !String.IsNullOrEmpty(SelectedSite) && !String.IsNullOrEmpty(SelectedPool) && SelectedExternalPort >= 1 && SelectedExternalPort <= 65535;
		}

		private bool _isHosted = false;
		private void ExecuteEx(object param)
		{
			if (!_isHosted)
			{
				_appManager.Host(SelectedSite, SelectedPool, SelectedExternalPort, AutoBrowse);
				_isHosted = true;
				CurrentCommandText = "Stop Host";
			}
			else
			{
				_appManager.EndCurrentHosting();
				_isHosted = false;
				CurrentCommandText = "Host";
			}//end if
		}
		#endregion
		
	}
}
