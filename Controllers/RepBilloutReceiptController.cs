using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;

namespace OfficialReceiptApp.Controllers
{
    public class RepBilloutReceiptController
	{
		// ============
		// Data Context
		// ============
		Data.EasyRestaurantDBDataContext db = new Data.EasyRestaurantDBDataContext();

		// ================
		// Global Variables
		// ================
		private int _salesId = 0;
		private int _collectionId = 0;
		private int _terminalId = 0;
		private string _type = "";
		private string _printer = "";
		private bool useDefaultPrinter = Boolean.Parse(ConfigurationManager.AppSettings["useDefaultPrinter"].ToString());

		// =============
		// Print Receipt
		// =============
		public void PrintBillReceipt(int salesId, int terminalId, string type, string printerName)
        {
            try
            {
				_salesId = salesId;
				_terminalId = terminalId;
				_type = type;
				_printer = printerName;

				if (useDefaultPrinter) this.GetDefaultPrinter();

				PrinterSettings ps = new PrinterSettings
                {
                    PrinterName = _printer
                };

                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(PrintBillReceiptPage);
                pd.PrinterSettings = ps;
                pd.Print();

            }
            catch(Exception ex)
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
        public void PrintBillReceiptPage(object sender, PrintPageEventArgs e)
        {
            Font fontArial12Bold = new Font("Arial", 12, FontStyle.Bold);
            Font fontArial12Regular = new Font("Arial", 12, FontStyle.Regular);
            Font fontArial11Bold = new Font("Arial", 11, FontStyle.Bold);
            Font fontArial11Regular = new Font("Arial", 11, FontStyle.Regular);
            Font fontArial8Bold = new Font("Arial", 8, FontStyle.Bold);
            Font fontArial8Regular = new Font("Arial", 8, FontStyle.Regular);
            Font fontArial10Bold = new Font("Arial", 10, FontStyle.Bold);

            // ==================
            // Alignment Settings
            // ==================
            StringFormat drawFormatCenter = new StringFormat { Alignment = StringAlignment.Center };
            StringFormat drawFormatLeft = new StringFormat { Alignment = StringAlignment.Near };
            StringFormat drawFormatRight = new StringFormat { Alignment = StringAlignment.Far };

            float x, y;
            float width, height;
            //if (Modules.SysCurrentModule.GetCurrentSettings().PrinterType == "Dot Matrix Printer")
            //{
             //   x = 5; y = 5;
               // width = 245.0F; height = 0F;
            //}
            //else
            //{
                x = 5; y = 5;
                width = 260.0F; height = 0F;
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
            String companyName = "ABC Company";

            float adjustStringName = 1;
            if (companyName.Length > 43)
            {
                adjustStringName = 2;
            }

            graphics.DrawString(companyName, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += (graphics.MeasureString(companyName, fontArial8Regular).Height * adjustStringName);

            // ===============
            // Company Address
            // ===============

            String companyAddress = "Keppel";

            float adjustStringAddress = 1;
            if (companyAddress.Length > 25)
            {
                adjustStringAddress = 2;
            }
            graphics.DrawString(companyAddress, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += (graphics.MeasureString(companyAddress, fontArial8Regular).Height * adjustStringAddress);

            // ==========
            // TIN Number
            // ==========
            String TINNumber = "1234";
            graphics.DrawString("TIN: " + TINNumber, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += graphics.MeasureString(companyAddress, fontArial8Regular).Height;

            // =============
            // Serial Number
            // =============
            String serialNo = "1234";
            graphics.DrawString("SN: " + serialNo, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += graphics.MeasureString(companyAddress, fontArial8Regular).Height;

            // ==============
            // Machine Number
            // ==============
            String machineNo = "1234";
            graphics.DrawString("MIN: " + machineNo, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += graphics.MeasureString(companyAddress, fontArial8Regular).Height;

            // ======================
            // Official Receipt Title
            // ======================
            //String officialReceiptTitle = systemCurrent.ORPrintTitle ;
            String officialReceiptTitle = "B I L L";
            graphics.DrawString(officialReceiptTitle, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += graphics.MeasureString(officialReceiptTitle, fontArial8Regular).Height;

            // ========
            // 1st Line
            // ========
            Point firstLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
            Point firstLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
            graphics.DrawLine(blackPen, firstLineFirstPoint, firstLineSecondPoint);

            // ==========
            // Sales Line
            // ==========
            Decimal totalGrossSales = 0;
            Decimal totalSales = 0;
            Decimal totalDiscount = 0;
            Decimal totalVATSales = 0;
            Decimal totalVATAmount = 0;
            Decimal totalNonVATSales = 0;
            //Decimal totalVATExclusive = 0;
            Decimal totalVATExempt = 0;
            Decimal totalVATZeroRated = 0;
            Decimal totalNumberOfItems = 0;
            Decimal totalServiceCharge = 0;
            String discountGiven = "";
            Decimal lessVAT = 0;
            var sales = from d in db.TrnSales where d.Id == _salesId select d;

            String tableLabel = "";
            if (sales.FirstOrDefault().MstTable.TableCode != "Walk-In" && sales.FirstOrDefault().MstTable.TableCode != "Delivery")
            {
                tableLabel = "\nTable No.:";

            }
            else
            {
                tableLabel = "\nOrder Type:";

            }
            graphics.DrawString(tableLabel + sales.FirstOrDefault().MstTable.TableCode, fontArial10Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
            y += graphics.MeasureString(tableLabel, fontArial8Regular).Height;

            String itemLabel = "\nITEM";
            String amountLabel = "\nAMOUNT";
            graphics.DrawString(itemLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
            graphics.DrawString(amountLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
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
                                               s.PriceSplitPercentage,
                                               s.TrnSale,
                                               s.MstDiscount.Discount
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
                                               TaxAmount = g.Sum(a => a.TaxAmount),
                                               g.Key.TrnSale.ServiceChargeAmount,
                                               g.Key.Discount,
                                               g.Key.TrnSale.Pax,
                                               g.Key.TrnSale.DiscountedPax
                                           };

                if (salesLineGroupbyItem.Any())
                {
                    foreach (var salesLine in salesLineGroupbyItem.ToList())
                    {
                        totalNumberOfItems += 1;
                        totalGrossSales += salesLine.Price * salesLine.Quantity;
                        totalSales += salesLine.Amount;
                        totalDiscount += salesLine.DiscountAmount;
                        totalServiceCharge = salesLine.ServiceChargeAmount;
                        discountGiven = salesLine.Discount;
                        if (salesLine.MstTax.Code == "VAT")
                        {
                            totalVATSales += (salesLine.Price * salesLine.Quantity) - ((salesLine.Price * salesLine.Quantity) / (1 + (salesLine.MstItem.MstTax1.Rate / 100)) * (salesLine.MstItem.MstTax1.Rate / 100));
                            totalVATAmount += salesLine.TaxAmount;
                        }
                        else if (salesLine.MstTax.Code == "NONVAT")
                        {
                            totalNonVATSales += salesLine.Price * salesLine.Quantity;
                        }
                        else if (salesLine.MstTax.Code == "EXEMPTVAT")
                        {
                            if (salesLine.MstItem.MstTax1.Rate > 0)
                            {
                                totalVATExempt += ((((salesLine.Price * salesLine.Quantity) / salesLine.Pax.GetValueOrDefault()) * salesLine.DiscountedPax.GetValueOrDefault()) / (1 + (salesLine.MstItem.MstTax1.Rate / 100)));
                                lessVAT += ((((salesLine.Price * salesLine.Quantity) / salesLine.Pax.GetValueOrDefault()) * salesLine.DiscountedPax.GetValueOrDefault()) / (1 + (salesLine.MstItem.MstTax1.Rate / 100)) * (salesLine.MstItem.MstTax1.Rate / 100));
                            }
                            else
                            {
                                totalVATExempt += salesLine.Price * salesLine.Quantity;
                            }
                        }
                        else if (salesLine.MstTax.Code == "ZEROVAT")
                        {
                            totalVATZeroRated += salesLine.Amount;
                        }

                        String itemData = salesLine.ItemDescription + "\n" + salesLine.Quantity.ToString("#,##0.00") + " " + salesLine.Unit + " @ " + salesLine.Price.ToString("#,##0.00");
                        String itemAmountData = (salesLine.Price * salesLine.Quantity).ToString("#,##0.00");
                        RectangleF itemDataRectangle = new RectangleF
                        {
                            X = x,
                            Y = y,
                            Size = new Size(150, ((int)graphics.MeasureString(itemData, fontArial8Regular, 150, StringFormat.GenericTypographic).Height))
                        };
                        graphics.DrawString(itemData, fontArial8Regular, Brushes.Black, itemDataRectangle, drawFormatLeft);
                        //if (Modules.SysCurrentModule.GetCurrentSettings().PrinterType == "Dot Matrix Printer")
                        //{
                        //    graphics.DrawString(itemAmountData, fontArial8Regular, drawBrush, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                        //}
                        //else
                        //{
                            graphics.DrawString(itemAmountData, fontArial8Regular, drawBrush, new RectangleF(x, y, 250.0F, height), drawFormatRight);
                        //}
                        y += itemDataRectangle.Size.Height + 3.0F;
                    }
                }
            }

            // ========
            // 2nd Line
            // ========
            Point secondLineFirstPoint = new Point(0, Convert.ToInt32(y) + 10);
            Point secondLineSecondPoint = new Point(500, Convert.ToInt32(y) + 10);
            graphics.DrawLine(blackPen, secondLineFirstPoint, secondLineSecondPoint);

            Decimal totalAmountDue = totalSales + totalServiceCharge;

            String totalSalesLabel = "\nSub-total Amount";
            String totalSalesAmount = "\n" + totalGrossSales.ToString("#,##0.00");
            graphics.DrawString(totalSalesLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
            graphics.DrawString(totalSalesAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
            y += graphics.MeasureString(totalSalesAmount, fontArial8Regular).Height;

            String serviceChargeLabel = "Service Charge";
            String totalServiceChargeAmount = totalServiceCharge.ToString("#,##0.00");
            graphics.DrawString(serviceChargeLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
            graphics.DrawString(totalServiceChargeAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
            y += graphics.MeasureString(totalServiceChargeAmount, fontArial8Regular).Height;

            String lessVATLabel = "LESS: VAT";
            String totalLessVATAmount = lessVAT.ToString("#,##0.00");
            graphics.DrawString(lessVATLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
            graphics.DrawString(totalLessVATAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
            y += graphics.MeasureString(totalLessVATAmount, fontArial8Regular).Height;

            if (discountGiven != "Zero Discount")
            {
                String DiscountLabel = "Discount Given";
                String Discount = discountGiven;
                graphics.DrawString(DiscountLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                graphics.DrawString(Discount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                y += graphics.MeasureString(Discount, fontArial8Regular).Height;
            }
            String totalDiscountLabel = "LESS: Discount";
            String totalDiscountAmount = totalDiscount.ToString("#,##0.00");
            graphics.DrawString(totalDiscountLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
            graphics.DrawString(totalDiscountAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
            y += graphics.MeasureString(totalDiscountAmount, fontArial8Regular).Height;

            String netSalesLabel = "Total Amount Due";
            String netSalesAmount = totalAmountDue.ToString("#,##0.00");
            graphics.DrawString(netSalesLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
            graphics.DrawString(netSalesAmount, fontArial12Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
            y += graphics.MeasureString(netSalesAmount, fontArial12Regular).Height;

            String totalNumberOfItemsLabel = "Total No. of Item(s)\n\n";
            String totalNumberOfItemsQuantity = totalNumberOfItems.ToString("#,##0.00") + "\n\n";
            graphics.DrawString(totalNumberOfItemsLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
            graphics.DrawString(totalNumberOfItemsQuantity, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
            y += graphics.MeasureString(totalNumberOfItemsQuantity, fontArial8Regular).Height;
            // ========
            // 3rd Line
            // ========
            Point thirdLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
            Point thirdLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
            graphics.DrawLine(blackPen, thirdLineFirstPoint, thirdLineSecondPoint);


            String receiptFooter = "\n" + "Receipt Footer";
            graphics.DrawString(receiptFooter, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += graphics.MeasureString(receiptFooter, fontArial8Regular).Height;
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
