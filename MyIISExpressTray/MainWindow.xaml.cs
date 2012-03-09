using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Forms;

namespace IISExpressProxy
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private System.Windows.Forms.NotifyIcon _icon;
		private WindowState _lastState = WindowState.Normal;
		public MainWindow()
		{
			InitializeComponent();

			System.Drawing.Icon i = new System.Drawing.Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("IISExpressProxy.Images.Gear_XP.ico"));
			_icon = new System.Windows.Forms.NotifyIcon() { Icon = i, Visible = true };
			_icon.DoubleClick += new EventHandler(_icon_DoubleClick);
		}

		~MainWindow()
		{
			if(_icon != null)
				_icon.Dispose();
		}

		void _icon_DoubleClick(object sender, EventArgs e)
		{
			if (IsVisible)
				this.Hide();
			else
			{
				this.Show();
				this.WindowState = _lastState;
			}//end if
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			_icon.Visible = false;
			_icon.Dispose();
			_icon = null;
		}
		
		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);

			if (!this.IsLoaded)
				return;

			if (this.WindowState == System.Windows.WindowState.Minimized)
				_icon_DoubleClick(this, e);//Hide
			else
				_lastState = this.WindowState;
		}

		private void ButtonEditPath_Click(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				return;

			this.ButtonEditPath.Text = dlg.SelectedPath;
		}
	}
}
