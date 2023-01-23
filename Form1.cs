using Newtonsoft.Json;
using OfficialReceiptApp.Controllers;
using OfficialReceiptApp.Models;
using SharpCompress.Common;
using Squirrel;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

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
		string textFileLocation = ConfigurationManager.AppSettings["textFileLocation"].ToString();
		Timer timer;
		bool isBusy = false;

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

		public void InitTimer()
		{
			timer = new Timer();
			timer.Tick += new EventHandler(timer_Tick);
			timer.Interval = 1000;
			timer.Start();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			this.PrintOR();
		}

		private void PrintOR()
		{
			DirectoryInfo info = new DirectoryInfo(textFileLocation);
			FileInfo[] files = info.GetFiles("*.txt");

			foreach (FileInfo file in files)
			{
				string text = File.ReadAllText(Path.Combine(textFileLocation, file.Name));
				RepOfficialReceiptModel deserializedOR = JsonConvert.DeserializeObject<RepOfficialReceiptModel>(text);

				RepOfficialReceiptController repOfficialReceiptController = new RepOfficialReceiptController();
				repOfficialReceiptController.PrintOfficialReceipt(deserializedOR.SalesId, deserializedOR.CollectionId, false, "");

				if (File.Exists(Path.Combine(textFileLocation, file.Name))) File.Delete(Path.Combine(textFileLocation, file.Name));
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			InitTimer();
		}
	}
}
