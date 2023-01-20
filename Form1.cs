using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Squirrel;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace OfficialReceiptApp
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			Task.Run(() => CheckAndApplyUpdate()).GetAwaiter().GetResult();

			Assembly assembly = Assembly.GetExecutingAssembly();
			FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

			string versionNumber = fileVersionInfo.FileVersion.ToString();
			this.Text += $"Official Receipt v.{versionNumber}";
		}

		string fileServerLocation = ConfigurationManager.AppSettings["fileServerLocation"].ToString();

		private async Task CheckAndApplyUpdate()
		{
			try
			{
				bool updated = false;
				using (var updateManager = new UpdateManager(fileServerLocation))
				{
					var updateInfo = await updateManager.CheckForUpdate();
					if (updateInfo.ReleasesToApply != null &&
						updateInfo.ReleasesToApply.Count > 0)
					{
						var releaseEntry = await updateManager.UpdateApp();
						updated = true;
					}
				}
				if (updated)
				{
					UpdateManager.RestartApp("OfficialReceiptApp.exe");
				}
			}
			catch
			{
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
		}
	}
}
