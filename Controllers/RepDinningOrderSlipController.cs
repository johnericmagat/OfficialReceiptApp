using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficialReceiptApp.Controllers
{
	public class RepDinningOrderSlipController
	{
        Data.EasyRestaurantDBDataContext db = new Data.EasyRestaurantDBDataContext();

        // ================
        // Global Variables
        // ================
        private int _salesId = 0;
        private string _type = "";
        private string _printer = "";

        public void PrintBill(int salesId, string type, string printerName)
        {
            try
            {
                _salesId = salesId;
                _type = type;
                _printer = printerName;

                this.GetDefaultPrinter();

                PrinterSettings ps = new PrinterSettings
                {
                    PrinterName = _printer
                };

                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(PrintOrderSlipPage);
                pd.PrinterSettings = ps;
                pd.Print();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        // ===============
        // Default Printer
        // ===============
        private void GetDefaultPrinter()
        {
            try
            {
                PrinterSettings settings = new PrinterSettings();
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    settings.PrinterName = printer;
                    if (settings.IsDefaultPrinter)
                        _printer = printer;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void PrintOrderSlipPage(object sender, PrintPageEventArgs e)
        {

        }

    }
}
