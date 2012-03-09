using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace IISExpressProxy.Models
{
	interface IMainViewModel
	{
		int SelectedExternalPort { get; set; }
		List<string> AvailableSites { get; }
		List<string> AvailablePools { get; }

		string SelectedSite { get; set; }
		string SelectedPool { get; set; }
		ICommand ExCommand { get; }
	}
}
