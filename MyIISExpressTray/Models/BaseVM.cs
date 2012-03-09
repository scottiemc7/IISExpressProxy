using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace IISExpressProxy.Models
{
	abstract class BaseVM : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifiyPropertyChanged(string propName)
		{
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}
	}
}
