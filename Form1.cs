using Newtonsoft.Json;
using PrintProcessor.Controllers;
using PrintProcessor.Models;
using Squirrel;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace PrintProcessor
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			this.FormBorderStyle = FormBorderStyle.None;
			this.ShowInTaskbar = false;
			this.Load += new EventHandler(Form1_Load);

			Task.Run(() => CheckAndApplyUpdate()).GetAwaiter().GetResult();

			Assembly assembly = Assembly.GetExecutingAssembly();
			FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

			string versionNumber = fileVersionInfo.FileVersion.ToString();
			this.Text += $"Official Receipt v.{versionNumber}";
		}

		string installerLocation = ConfigurationManager.AppSettings["installerLocation"].ToString();
		string textFileLocation = ConfigurationManager.AppSettings["textFileLocation"].ToString();

		DispatcherTimer dispatcherTimer;
		BackgroundWorker backgroundWorker;

		private async Task CheckAndApplyUpdate()
		{
			try
			{
				bool updated = false;
				using (var updateManager = new UpdateManager(installerLocation))
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

		private void Print()
		{
			if (!Directory.Exists(textFileLocation)) Directory.CreateDirectory(textFileLocation);

			DirectoryInfo info = new DirectoryInfo(textFileLocation);
			FileInfo[] files = info.GetFiles("*.txt");

			foreach (FileInfo file in files)
			{
				string text = File.ReadAllText(Path.Combine(textFileLocation, file.Name));
				RepTextFileModel deserializedJson = JsonConvert.DeserializeObject<RepTextFileModel>(text);

				if (deserializedJson.Type == "OR")
				{
					RepOfficialReceiptController repOfficialReceiptController = new RepOfficialReceiptController();
					repOfficialReceiptController.PrintOfficialReceipt(deserializedJson.SalesId, deserializedJson.CollectionId, deserializedJson.TerminalId, deserializedJson.Type, deserializedJson.Printer, false);
				}
				else if (deserializedJson.Type == "BR")
				{
					RepBilloutReceiptController repBilloutReceiptController = new RepBilloutReceiptController();
					repBilloutReceiptController.PrintBillReceipt(deserializedJson.SalesId, deserializedJson.TerminalId, deserializedJson.Type, deserializedJson.Printer);
				}
				else
				{
					//print KOS
					//print DOS

					RepDinningOrderSlipController repDinningOrderSlipController = new RepDinningOrderSlipController();
					repDinningOrderSlipController.PrintDOS(deserializedJson.SalesId, deserializedJson.TerminalId, deserializedJson.Type, deserializedJson.Printer);
				}

				if (File.Exists(Path.Combine(textFileLocation, file.Name))) File.Delete(Path.Combine(textFileLocation, file.Name));
			}
		}

		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
			if (!backgroundWorker.IsBusy)
			{
				backgroundWorker.WorkerReportsProgress = true;
				backgroundWorker.DoWork += (obj, ea) => this.Print();
				backgroundWorker.RunWorkerAsync();
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.Size = new Size(0, 0);
			dispatcherTimer = new DispatcherTimer();
			backgroundWorker = new BackgroundWorker();

			dispatcherTimer.Tick += dispatcherTimer_Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			dispatcherTimer.Start();
		}
	}
}
