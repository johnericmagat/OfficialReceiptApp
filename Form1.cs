using Newtonsoft.Json;
using OfficialReceiptApp.Controllers;
using OfficialReceiptApp.Models;
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
			string textFile = @"D:\HII\Projects\EASY_RESTAURANT\_Documents\_Test\logs\2023120.txt";

			string text = File.ReadAllText(textFile);

			//string[] lines = File.ReadAllLines(textFile);
			//foreach (string line in lines)
			//	Console.WriteLine(line);

			RepOfficialReceiptModel deserializedOR = JsonConvert.DeserializeObject<RepOfficialReceiptModel>(text);

			RepOfficialReceiptController repOfficialReceiptController = new RepOfficialReceiptController();
			repOfficialReceiptController.PrintOfficialReceipt(deserializedOR.SalesId, deserializedOR.CollectionId, false, "");
		}
	}
}
