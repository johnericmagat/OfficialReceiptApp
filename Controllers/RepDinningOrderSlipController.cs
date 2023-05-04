using PrintProcessor.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace PrintProcessor.Controllers
{
    public class RepDinningOrderSlipController
	{
		// ============
		// Data Context
		// ============
		Data.EasyRestaurantDBDataContext db = new Data.EasyRestaurantDBDataContext();

		// ================
		// Global Variables
		// ================
		private int _salesId = 0;
		private int _terminalId = 0;
		private string _type = "";
		private string _printer = "";
		private bool useDefaultPrinter = Boolean.Parse(ConfigurationManager.AppSettings["useDefaultPrinter"].ToString());
        private string _companyName = "";
        private string _address = "";
        private string _tin = "";
        private string _serialNo = "";
        private string _machineNo = "";
        private string _receipfooter = "";
        private string _orTitle = "";
        private string _printerType = "";
        private string _invoicefooter = "";

        // =============
        // Print Receipt
        // =============
        public void PrintDinningOrderSlip(int salesId, int terminalId, string type, string printerName, List<SysGeneralSettingsModel> generalSettingsList)
        {
            try
            {
				_salesId = salesId;
				_terminalId = terminalId;
				_type = type;
				_printer = printerName;

                for (int i = 0; i < generalSettingsList.Count(); i++)
                {
                    _companyName = generalSettingsList[i].CompanyName;
                    _address = generalSettingsList[i].Address;
                    _tin = generalSettingsList[i].TIN;
                    _serialNo = generalSettingsList[i].SerialNo;
                    _machineNo = generalSettingsList[i].MachineNo;
                    _receipfooter = generalSettingsList[i].ReceiptFooter;
                    _orTitle = generalSettingsList[i].ORPrintTitle;
                    _printerType = generalSettingsList[i].PrinterType;
                    _invoicefooter = generalSettingsList[i].InvoiceFooter;
                }

                if (useDefaultPrinter) this.GetDefaultPrinter();

				PrinterSettings ps = new PrinterSettings
                {
                    PrinterName = _printer
                };

                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(PrintDinningOrderSlipPage);
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

		// ==========
		// Print Page
		// ==========
		public void PrintDinningOrderSlipPage(object sender, PrintPageEventArgs e)
        {
         
            // =============
            // Font Settings
            // =============
            Font fontArial12Bold = new Font("Arial", 12, FontStyle.Bold);
            Font fontArial12Regular = new Font("Arial", 12, FontStyle.Regular);
            Font fontArial11Bold = new Font("Arial", 11, FontStyle.Bold);
            Font fontArial11Regular = new Font("Arial", 11, FontStyle.Regular);
            Font fontArial8Bold = new Font("Arial", 8, FontStyle.Bold);
            Font fontArial8Regular = new Font("Arial", 8, FontStyle.Regular);
            Font fontArial9Bold = new Font("Arial", 9, FontStyle.Bold);
            Font fontArial9Regular = new Font("Arial", 9, FontStyle.Regular);
            Font fontArial10Bold = new Font("Arial", 10, FontStyle.Bold);
            Font fontArial10Regular = new Font("Arial", 10, FontStyle.Regular);
            Font fontArial7Bold = new Font("Arial", 7, FontStyle.Bold);
            Font fontArial7Regular = new Font("Arial", 7, FontStyle.Regular);

            // ==================
            // Alignment Settings
            // ==================
            StringFormat drawFormatCenter = new StringFormat { Alignment = StringAlignment.Center };
            StringFormat drawFormatLeft = new StringFormat { Alignment = StringAlignment.Near };
            StringFormat drawFormatRight = new StringFormat { Alignment = StringAlignment.Far };

            float x, y;
            float width, height;
            // if (Modules.SysCurrentModule.GetCurrentSettings().PrinterType == "Dot Matrix Printer")
            //{
            //    x = 5; y = 5;
            //    width = 245.0F; height = 0F;
            //}
            //else if (Modules.SysCurrentModule.GetCurrentSettings().PrinterType == "Thermal Printer")
            //{
              //  x = 5; y = 5;
              //  width = 260.0F; height = 0F;
            //}
            //else
            //{
                x = 5; y = 5;
                width = 250.0F; height = 0F;
            //}
            // ==============
            // Tools Settings
            // ==============
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            Pen blackPen = new Pen(Color.Black, 1);
            Pen whitePen = new Pen(Color.White, 1);

            // ========
            // Graphics
            // ========
            Graphics graphics = e.Graphics;

            // ==============
            // System Current
            // ==============
            //var systemCurrent = Modules.SysCurrentModule.GetCurrentSettings();

            // ============
            // Company Name
            // ============
            String companyName = _companyName;
            float adjustStringName = 1;
            if (companyName.Length > 43)
            {
                adjustStringName = 3;
            }

            graphics.DrawString(companyName, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += graphics.MeasureString(companyName, fontArial8Regular).Height * adjustStringName;

            // ===============
            // Company Address
            // ===============

            String companyAddress = _address;

            float adjuctHeight = 1;
            if (companyAddress.Length > 43)
            {
                adjuctHeight = 3;
            }

            graphics.DrawString(companyAddress, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += (graphics.MeasureString(companyAddress, fontArial8Regular, Convert.ToInt32(width)).Height);

            // ==========
            // TIN Number
            // ==========
            String TINNumber = _tin;
            graphics.DrawString("TIN: " + TINNumber, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += graphics.MeasureString(companyAddress, fontArial8Regular, Convert.ToInt32(width)).Height;

            // =============
            // Serial Number
            // =============
            String serialNo = _serialNo;
            graphics.DrawString("SN: " + serialNo, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += graphics.MeasureString(companyAddress, fontArial8Regular, Convert.ToInt32(width)).Height;

            // ==============
            // Machine Number
            // ==============
            String machineNo = _machineNo;
            graphics.DrawString("MIN: " + machineNo, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += graphics.MeasureString(companyAddress, fontArial8Regular, Convert.ToInt32(width)).Height;

            // =================
            // Sales Order Title
            // =================
            String officialReceiptTitle = "O R D E R   S L I P";
            graphics.DrawString(officialReceiptTitle, fontArial10Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += graphics.MeasureString(officialReceiptTitle, fontArial10Regular, Convert.ToInt32(width)).Height;

            // ============
            // Sales Header
            // ============
            var sales = from d in db.TrnSales
                        where d.Id == _salesId
                        select d;

            if (sales.Any())
            {
                String terminalText = "Terminal: " + sales.FirstOrDefault().MstTerminal.Terminal;
                graphics.DrawString(terminalText, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
                y += graphics.MeasureString(terminalText, fontArial8Regular).Height;

                String collectionNumberText = sales.FirstOrDefault().SalesNumber;
                graphics.DrawString(collectionNumberText, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
                y += graphics.MeasureString(collectionNumberText, fontArial8Regular).Height;

                String collectionDateText = sales.FirstOrDefault().SalesDate.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
                graphics.DrawString(collectionDateText, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
                y += graphics.MeasureString(collectionDateText, fontArial8Regular).Height;

                String collectionTimeText = sales.FirstOrDefault().UpdateDateTime.ToString("H:mm:ss", CultureInfo.InvariantCulture);
                graphics.DrawString(collectionTimeText, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
                y += graphics.MeasureString(collectionTimeText, fontArial8Regular).Height;

                // ========
                // 1st Line
                // ========
                Point firstLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                Point firstLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                graphics.DrawLine(blackPen, firstLineFirstPoint, firstLineSecondPoint);
                if (_type == "58mm Printer")
                {
                    // ==========
                    // Sales Line
                    // ==========
                    Decimal totalAmount = 0;
                    Decimal totalNumberOfItems = 0;

                    String itemLabel = "\nITEM";
                    String amountLabel = "\nAMOUNT";
                    graphics.DrawString(itemLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    graphics.DrawString(amountLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, 240.0F, height), drawFormatRight);
                    y += graphics.MeasureString(itemLabel, fontArial8Regular).Height + 5.0F;

                    var salesLines = from d in db.TrnSalesLines where d.SalesId == _salesId select d;
                    if (salesLines.Any())
                    {
                        var salesLineGroupbyItem = from s in salesLines
                                                   group s by new
                                                   {
                                                       s.SalesId,
                                                       s.ItemId,
                                                       s.MstItem,
                                                       s.UnitId,
                                                       s.MstUnit,
                                                       s.NetPrice,
                                                       s.Price,
                                                       s.TaxId,
                                                       s.MstTax,
                                                       s.DiscountId,
                                                       s.DiscountRate,
                                                       s.SalesAccountId,
                                                       s.AssetAccountId,
                                                       s.CostAccountId,
                                                       s.TaxAccountId,
                                                       s.SalesLineTimeStamp,
                                                       s.UserId,
                                                       s.Preparation,
                                                       s.Price1,
                                                       s.Price2,
                                                       s.Price2LessTax,
                                                       s.PriceSplitPercentage
                                                   } into g
                                                   select new
                                                   {
                                                       g.Key.ItemId,
                                                       g.Key.MstItem,
                                                       g.Key.MstItem.ItemDescription,
                                                       g.Key.MstUnit.Unit,
                                                       g.Key.Price,
                                                       g.Key.NetPrice,
                                                       g.Key.DiscountId,
                                                       g.Key.DiscountRate,
                                                       g.Key.TaxId,
                                                       g.Key.MstTax,
                                                       g.Key.MstTax.Tax,
                                                       Amount = g.Sum(a => a.Amount),
                                                       Quantity = g.Sum(a => a.Quantity),
                                                       DiscountAmount = g.Sum(a => a.DiscountAmount * a.Quantity),
                                                       TaxAmount = g.Sum(a => a.TaxAmount)
                                                   };

                        if (salesLineGroupbyItem.Any())
                        {
                            foreach (var salesLine in salesLines.OrderBy(d => d.ItemHeaderId))
                            {
                                totalNumberOfItems += salesLine.Quantity;

                                totalAmount += salesLine.Amount;

                                var equalItemId = from s in db.MstItems
                                                  where s.Id == salesLine.ItemId
                                                  select s;

                                String itemData = "";
                                String itemAmountData = "";

                                if (equalItemId.FirstOrDefault().Category == "Add-On")
                                {
                                    itemData = "     " + salesLine.MstItem.ItemDescription + "\n" + "      " + salesLine.Quantity.ToString("N2", CultureInfo.InvariantCulture) + " @ " + salesLine.Price.ToString("#,##0.00");
                                    itemAmountData = "\n" + (salesLine.Amount + salesLine.DiscountAmount).ToString("N2", CultureInfo.InvariantCulture);
                                }
                                else if (equalItemId.FirstOrDefault().Category == "Item Modifier")
                                {
                                    itemData = "     " + salesLine.MstItem.ItemDescription;
                                }
                                else
                                {
                                    itemData = salesLine.MstItem.ItemDescription + "\n" + salesLine.Quantity.ToString("N2", CultureInfo.InvariantCulture) + " @ " + salesLine.Price.ToString("#,##0.00");
                                    itemAmountData = "\n" + (salesLine.Amount + salesLine.DiscountAmount).ToString("N2", CultureInfo.InvariantCulture);
                                }

                                RectangleF itemDataRectangle = new RectangleF
                                {
                                    X = x,
                                    Y = y,
                                    Size = new Size(150, ((int)graphics.MeasureString(itemData, fontArial8Regular, 150, StringFormat.GenericTypographic).Height)),
                                    Width = width
                                };
                                graphics.DrawString(itemData, fontArial8Regular, Brushes.Black, itemDataRectangle, drawFormatLeft);
                                graphics.DrawString(itemAmountData, fontArial8Regular, drawBrush, new RectangleF(x, y, 240.0F, height), drawFormatRight);
                                y += itemDataRectangle.Size.Height + 3.0F;
                            }
                        }
                    }

                    // ========
                    // 2nd Line
                    // ========
                    Point secondLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                    Point secondLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                    graphics.DrawLine(blackPen, secondLineFirstPoint, secondLineSecondPoint);

                    // ============
                    // Total Amount
                    // ============
                    String totalSalesLabel = "\nTotal Amount";
                    String totalSalesAmount = "\n" + totalAmount.ToString("N2", CultureInfo.InvariantCulture);
                    graphics.DrawString(totalSalesLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    graphics.DrawString(totalSalesAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, 240.0F, height), drawFormatRight);
                    y += graphics.MeasureString(totalSalesAmount, fontArial8Regular).Height;

                    String totalNumberOfItemsLabel = "Total No. of Item(s)";
                    String totalNumberOfItemsQuantity = totalNumberOfItems.ToString("N2", CultureInfo.InvariantCulture);
                    graphics.DrawString(totalNumberOfItemsLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    graphics.DrawString(totalNumberOfItemsQuantity, fontArial8Regular, drawBrush, new RectangleF(x, y, 240.0F, height), drawFormatRight);
                    y += graphics.MeasureString(totalNumberOfItemsQuantity, fontArial8Regular).Height;

                    // ========
                    // 3rd Line
                    // ========
                    Point thirdLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                    Point thirdLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                    graphics.DrawLine(blackPen, thirdLineFirstPoint, thirdLineSecondPoint);

                    String remarks = "\nRemarks: \n\n " + salesLines.FirstOrDefault().TrnSale.Remarks;
                    graphics.DrawString(remarks, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    y += graphics.MeasureString(remarks, fontArial8Regular).Height;

                    //// ========
                    //// 4th Line
                    //// ========
                    //Point forththLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                    //Point forthLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                    //graphics.DrawLine(blackPen, forththLineFirstPoint, forthLineSecondPoint);

                    //String orderNumber = "\nOrder Number: \n\n " + salesLines.FirstOrDefault().TrnSale.SalesNumber;
                    //graphics.DrawString(orderNumber, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    //y += graphics.MeasureString(orderNumber, fontArial8Regular).Height;

                    // ========
                    // 5th Line
                    // ========
                    Point fifthLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                    Point fifthLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                    graphics.DrawLine(blackPen, fifthLineFirstPoint, fifthLineSecondPoint);

                    // =======
                    // Cashier
                    // =======
                    String cashier = sales.FirstOrDefault().MstUser5.FullName;

                    String cashierLabel = "\nTeller";
                    String cashierUserData = "\n" + cashier;
                    graphics.DrawString(cashierLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    graphics.DrawString(cashierUserData, fontArial8Regular, drawBrush, new RectangleF(x, y, 240.0F, height), drawFormatRight);
                    y += graphics.MeasureString(cashierUserData, fontArial8Regular).Height;

                    // ========
                    // 6th Line
                    // ========
                    Point sixthLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                    Point sixthLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                    graphics.DrawLine(blackPen, sixthLineFirstPoint, sixthLineSecondPoint);

                    String salesInvoiceFooter = "\n" + "Footer";
                    graphics.DrawString(salesInvoiceFooter, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
                    y += graphics.MeasureString(salesInvoiceFooter, fontArial8Regular).Height;
                }
                else
                {
                    //// ==============
                    //// Customer Line
                    //// ==============
                    //var customerLines = from d in db.TrnSales where d.Id == trnSalesId select d;
                    //if (customerLines.Any())
                    //{
                    //    var customer = from d in customerLines
                    //                   where d.CustomerId == d.MstCustomer.Id
                    //                   select d;
                    //    if (customer.Any())
                    //    {
                    //        //Customer Name
                    //        String customerNameLabel = "\nCustomer Name";
                    //        String customerName = "\n" + customer.FirstOrDefault().MstCustomer.Customer;
                    //        graphics.DrawString(customerNameLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    //        graphics.DrawString(customerName, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                    //        y += graphics.MeasureString(customerName, fontArial8Regular).Height;
                    //        //Birthday
                    //        String customerBdayLabel = "Birthday";
                    //        String customerBday = Convert.ToDateTime(customer.FirstOrDefault().MstCustomer.Birthday).ToShortDateString();
                    //        graphics.DrawString(customerBdayLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    //        graphics.DrawString(customerBday, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                    //        y += graphics.MeasureString(customerBday, fontArial8Regular).Height;
                    //        //Age
                    //        String customerAgeLabel = "Age";
                    //        String customerAge = Convert.ToString(customer.FirstOrDefault().MstCustomer.Age);
                    //        graphics.DrawString(customerAgeLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    //        graphics.DrawString(customerAge, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                    //        y += graphics.MeasureString(customerAge, fontArial8Regular).Height;
                    //        //Gender
                    //        String customerGenderLabel = "Gender";
                    //        String customerGender = customer.FirstOrDefault().MstCustomer.Gender;
                    //        graphics.DrawString(customerGenderLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    //        graphics.DrawString(customerGender, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                    //        y += graphics.MeasureString(customerGender, fontArial8Regular).Height;
                    //        //Contact Number
                    //        String customerContactNumberLabel = "Contact Number";
                    //        String customerContactNumber = customer.FirstOrDefault().MstCustomer.ContactNumber;
                    //        graphics.DrawString(customerContactNumberLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    //        graphics.DrawString(customerContactNumber, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                    //        y += graphics.MeasureString(customerContactNumber, fontArial8Regular).Height;
                    //        //Email
                    //        String customerEmailLabel = "Email Address";
                    //        String customerEmail = customer.FirstOrDefault().MstCustomer.EmailAddress;
                    //        graphics.DrawString(customerEmailLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    //        graphics.DrawString(customerEmail, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                    //        y += graphics.MeasureString(customerEmail, fontArial8Regular).Height;

                    //        // ========
                    //        // Customer Line
                    //        // ========
                    //        Point customerLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                    //        Point customerLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                    //        graphics.DrawLine(blackPen, customerLineFirstPoint, customerLineSecondPoint);
                    //    }
                    //}

                    // ==========
                    // Sales Line
                    // ==========
                    Decimal totalAmount = 0;
                    Decimal totalNumberOfItems = 0;

                    String itemLabel = "\nITEM";
                    String amountLabel = "\nAMOUNT";
                    graphics.DrawString(itemLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    graphics.DrawString(amountLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                    y += graphics.MeasureString(itemLabel, fontArial8Regular).Height + 5.0F;

                    var salesLines = from d in db.TrnSalesLines where d.SalesId == _salesId select d;
                    if (salesLines.Any())
                    {
                        var salesLineGroupbyItem = from s in salesLines
                                                   group s by new
                                                   {
                                                       s.SalesId,
                                                       s.ItemId,
                                                       s.MstItem,
                                                       s.UnitId,
                                                       s.MstUnit,
                                                       s.NetPrice,
                                                       s.Price,
                                                       s.TaxId,
                                                       s.MstTax,
                                                       s.DiscountId,
                                                       s.DiscountRate,
                                                       s.SalesAccountId,
                                                       s.AssetAccountId,
                                                       s.CostAccountId,
                                                       s.TaxAccountId,
                                                       s.SalesLineTimeStamp,
                                                       s.UserId,
                                                       s.Preparation,
                                                       s.Price1,
                                                       s.Price2,
                                                       s.Price2LessTax,
                                                       s.PriceSplitPercentage
                                                   } into g
                                                   select new
                                                   {
                                                       g.Key.ItemId,
                                                       g.Key.MstItem,
                                                       g.Key.MstItem.ItemDescription,
                                                       g.Key.MstUnit.Unit,
                                                       g.Key.Price,
                                                       g.Key.NetPrice,
                                                       g.Key.DiscountId,
                                                       g.Key.DiscountRate,
                                                       g.Key.TaxId,
                                                       g.Key.MstTax,
                                                       g.Key.MstTax.Tax,
                                                       Amount = g.Sum(a => a.Amount),
                                                       Quantity = g.Sum(a => a.Quantity),
                                                       DiscountAmount = g.Sum(a => a.DiscountAmount * a.Quantity),
                                                       TaxAmount = g.Sum(a => a.TaxAmount)
                                                   };

                        if (salesLineGroupbyItem.Any())
                        {
                            foreach (var salesLine in salesLines.OrderBy(d => d.ItemHeaderId))
                            {
                                totalNumberOfItems += 1;

                                totalAmount += salesLine.Amount;

                                var equalItemId = from s in db.MstItems
                                                  where s.Id == salesLine.ItemId
                                                  select s;

                                String itemData = "";
                                String itemAmountData = "";

                                if (equalItemId.FirstOrDefault().Category == "Add-On")
                                {
                                    itemData = "     " + salesLine.MstItem.ItemDescription + "\n" + "      " + salesLine.Quantity.ToString("N2", CultureInfo.InvariantCulture) + " @ " + salesLine.Price.ToString("#,##0.00");
                                    itemAmountData = "\n" + (salesLine.Amount + salesLine.DiscountAmount).ToString("N2", CultureInfo.InvariantCulture);
                                }
                                else if (equalItemId.FirstOrDefault().Category == "Item Modifier")
                                {
                                    itemData = "     " + salesLine.MstItem.ItemDescription;
                                }
                                else
                                {
                                    itemData = salesLine.MstItem.ItemDescription + "\n" + salesLine.Quantity.ToString("N2", CultureInfo.InvariantCulture) + " @ " + salesLine.Price.ToString("#,##0.00");
                                    itemAmountData = "\n" + (salesLine.Amount + salesLine.DiscountAmount).ToString("N2", CultureInfo.InvariantCulture);
                                }

                                RectangleF itemDataRectangle = new RectangleF
                                {
                                    X = x,
                                    Y = y,
                                    Size = new Size(150, ((int)graphics.MeasureString(itemData, fontArial8Regular, 150, StringFormat.GenericTypographic).Height)),
                                    Width = width
                                };
                                graphics.DrawString(itemData, fontArial8Regular, Brushes.Black, itemDataRectangle, drawFormatLeft);
                                //if (Modules.SysCurrentModule.GetCurrentSettings().PrinterType == "Dot Matrix Printer")
                                //{
                                //    graphics.DrawString(itemAmountData, fontArial8Regular, drawBrush, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                                //}
                                //else
                                //{
                                    graphics.DrawString(itemAmountData, fontArial8Regular, drawBrush, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                                //}
                                y += itemDataRectangle.Size.Height + 3.0F;
                            }
                        }
                    }

                    // ========
                    // 2nd Line
                    // ========
                    Point secondLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                    Point secondLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                    graphics.DrawLine(blackPen, secondLineFirstPoint, secondLineSecondPoint);

                    // ============
                    // Total Amount
                    // ============
                    String totalSalesLabel = "\nTotal Amount";
                    String totalSalesAmount = "\n" + totalAmount.ToString("N2", CultureInfo.InvariantCulture);
                    graphics.DrawString(totalSalesLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    graphics.DrawString(totalSalesAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                    y += graphics.MeasureString(totalSalesAmount, fontArial8Regular).Height;

                    String totalNumberOfItemsLabel = "Total No. of Item(s)";
                    String totalNumberOfItemsQuantity = totalNumberOfItems.ToString("N2", CultureInfo.InvariantCulture);
                    graphics.DrawString(totalNumberOfItemsLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    graphics.DrawString(totalNumberOfItemsQuantity, fontArial8Regular, drawBrush, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                    y += graphics.MeasureString(totalNumberOfItemsQuantity, fontArial8Regular).Height;

                    // ========
                    // 3rd Line
                    // ========
                    Point thirdLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                    Point thirdLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                    graphics.DrawLine(blackPen, thirdLineFirstPoint, thirdLineSecondPoint);

                    String remarks = "\nRemarks: \n\n " + salesLines.FirstOrDefault().TrnSale.Remarks;
                    graphics.DrawString(remarks, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    y += graphics.MeasureString(remarks, fontArial8Regular).Height;

                    //// ========
                    //// 4th Line
                    //// ========
                    //Point forththLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                    //Point forthLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                    //graphics.DrawLine(blackPen, forththLineFirstPoint, forthLineSecondPoint);

                    //String orderNumber = "\nOrder Number: \n\n " + salesLines.FirstOrDefault().TrnSale.SalesNumber;
                    //graphics.DrawString(orderNumber, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    //y += graphics.MeasureString(orderNumber, fontArial8Regular).Height;

                    // ========
                    // 5th Line
                    // ========
                    Point fifthLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                    Point fifthLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                    graphics.DrawLine(blackPen, fifthLineFirstPoint, fifthLineSecondPoint);

                    // =======
                    // Cashier
                    // =======
                    String cashier = sales.FirstOrDefault().MstUser5.FullName;

                    String cashierLabel = "\nTeller";
                    String cashierUserData = "\n" + cashier;
                    graphics.DrawString(cashierLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    graphics.DrawString(cashierUserData, fontArial8Regular, drawBrush, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                    y += graphics.MeasureString(cashierUserData, fontArial8Regular).Height;

                    // ========
                    // 6th Line
                    // ========
                    Point sixthLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                    Point sixthLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                    graphics.DrawLine(blackPen, sixthLineFirstPoint, sixthLineSecondPoint);

                    String salesInvoiceFooter = "\n" + _invoicefooter;
                    graphics.DrawString(salesInvoiceFooter, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
                    y += graphics.MeasureString(salesInvoiceFooter, fontArial8Regular).Height + 10F;

                    //Zen.Barcode.CodeQrBarcodeDraw codeQrBarcodeDraw = Zen.Barcode.BarcodeDrawFactory.CodeQr;
                    //PictureBox pictureBox = new PictureBox();
                    //pictureBox.Size = new Size(100, 100);
                    //pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    //pictureBox.Image = codeQrBarcodeDraw.Draw(sales.FirstOrDefault().SalesNumber, 50);

                    //Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
                    //pictureBox.DrawToBitmap(bmp, new Rectangle(0, 0, pictureBox.Width, pictureBox.Height));
                    //graphics.DrawImage(bmp, x + 70, y + 15);
                    //y += bmp.Height + 7;
                }
            }

            //if (Modules.SysCurrentModule.GetCurrentSettings().PrinterType == "Dot Matrix Printer")
            //{
              //  String space = "\n\n\n\n\n\n\n\n\n\n.";
              //  graphics.DrawString(space, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            //}
            //else
            //{
                String space = "\n\n\n.";
                graphics.DrawString(space, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            //}
        }
    }
}
