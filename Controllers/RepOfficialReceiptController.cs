using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;

namespace PrintProcessor.Controllers
{
	public class RepOfficialReceiptController
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
		private bool _isReprinted = false;
		private bool useDefaultPrinter = Boolean.Parse(ConfigurationManager.AppSettings["useDefaultPrinter"].ToString());

		// =============
		// Print Receipt
		// =============
		public void PrintOfficialReceipt(int salesId, int collectionId, int terminalId, string type, string printerName, bool isReprint)
		{
			try
			{
				_salesId = salesId;
				_collectionId = collectionId;
				_terminalId = terminalId;
				_type = type;
				_printer = printerName;
				_isReprinted = isReprint;
				
				if (useDefaultPrinter) this.GetDefaultPrinter();

				PrinterSettings ps = new PrinterSettings
				{
					PrinterName = _printer
				};

				PrintDocument pd = new PrintDocument();
				pd.PrintPage += new PrintPageEventHandler(PrintOfficialReceiptPage);
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
		public void PrintOfficialReceiptPage(object sender, PrintPageEventArgs e)
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

			x = 5; y = 5;
			width = 170.0F; height = 0F;

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
				adjustStringName = 3;
			}

			graphics.DrawString(companyName, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
			y += (graphics.MeasureString(companyName, fontArial8Regular).Height * adjustStringName);

			// ===============
			// Company Address
			// ===============

			String companyAddress = "Unit 1001B 10/F Keppel Center, Cebu Business Park, Cebu City, 6000, Philippines";

			// float adjustStringAddress = 1;
			//if (companyAddress.Length > 40)
			//{
			//  adjustStringAddress = 2;
			//}
			//else if (companyAddress.Length > 75)
			//{
			//  adjustStringAddress = 3;
			//}
			//else if (companyAddress.Length > 110)
			//{
			//  adjustStringAddress = 5;
			//}
			//else if (companyAddress.Length > 150)
			//{
			//adjustStringAddress = 7;
			//}

			float adjustStringAddress = 1;

			if (companyAddress.Length <= 40)
			{
				adjustStringAddress = 1;
			}
			else if (companyAddress.Length <= 75)
			{
				adjustStringAddress = 2;
			}
			else if (companyAddress.Length <= 110)
			{
				adjustStringAddress = 3;
			}
			else if (companyAddress.Length <= 150)
			{
				adjustStringAddress = 4;
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
			float adjustStringSerialNo = 1;
			if (companyAddress.Length > 43)
			{
				adjustStringSerialNo = 3;
			}
			String serialNo = "5678";
			graphics.DrawString("SN: " + serialNo, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
			y += graphics.MeasureString(companyAddress, fontArial8Regular).Height;

			// ==============
			// Machine Number
			// ==============
			String machineNo = "9101";
			graphics.DrawString("MIN: " + machineNo, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
			y += graphics.MeasureString(companyAddress, fontArial8Regular).Height;

			// ======================
			// Official Receipt Title
			// ======================
			String officialReceiptTitle = "OFFICIAL RECEIPT";
			graphics.DrawString(officialReceiptTitle, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
			y += graphics.MeasureString(officialReceiptTitle, fontArial8Regular).Height;

			//var sales = from d in db.TrnSales where d.Id == trnSalesId select d;
			// ==============
			// Customer Name
			// ==============

			//String customerName = sales.Any() == true ? sales.FirstOrDefault().MstCustomer.Customer : "";
			//graphics.DrawString("MIN: " + machineNo, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
			//y += graphics.MeasureString(companyAddress, fontArial8Regular).Height;

			// =================
			// Collection Header
			// =================
			var collections = from d in db.TrnCollections where d.Id == _collectionId select d;
			if (collections.Any())
			{
				String collectionNumberText = collections.FirstOrDefault().CollectionNumber;
				graphics.DrawString(collectionNumberText, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
				y += graphics.MeasureString(collectionNumberText, fontArial8Regular).Height;

				String collectionDateText = collections.FirstOrDefault().CollectionDate.ToString("MM-dd-yyyy", CultureInfo.InvariantCulture);
				graphics.DrawString(collectionDateText, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
				y += graphics.MeasureString(collectionDateText, fontArial8Regular).Height;

				String collectionTimeText = collections.FirstOrDefault().UpdateDateTime.ToString("H:mm:ss", CultureInfo.InvariantCulture);
				graphics.DrawString(collectionTimeText, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
				y += graphics.MeasureString(collectionTimeText, fontArial8Regular).Height;

				if (_isReprinted)
				{
					graphics.DrawString("REPRINTED", fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
					y += graphics.MeasureString("REPRINTED", fontArial8Regular).Height;
				}

				// ==========
				// Sales Line
				// ==========
				Decimal totalNetGrossSales = 0;
				Decimal totalSales = 0;
				Decimal totalDiscount = 0;
				Decimal change = 0;
				Decimal totalVATSales = 0;
				Decimal totalVATAmount = 0;
				Decimal totalNonVATSales = 0;
				//Decimal totalVATExclusive = 0;
				Decimal subtotalVATExempt = 0;
				Decimal totalVATZeroRated = 0;
				Decimal totalNumberOfItems = 0;
				String discountGiven = "";
				Decimal totalGrossSales = 0;
				Decimal lessVAT = 0;
				Decimal totalServiceCharge = 0;
				Decimal divider = 1.12M;
				Decimal totalVATExempt = 0;

				var sales = from d in db.TrnSales where d.Id == _salesId select d;

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
												   s.ItemId,
												   s.MstItem,
												   s.UnitId,
												   s.MstUnit,
												   s.NetPrice,
												   s.Price,
												   s.TaxId,
												   s.MstTax,
												   s.MstDiscount,
												   s.DiscountId,
												   s.DiscountRate,
												   s.SalesAccountId,
												   s.AssetAccountId,
												   s.CostAccountId,
												   s.TaxAccountId,
												   s.UserId,
												   s.Preparation,
												   s.Price1,
												   s.Price2,
												   s.Price2LessTax,
												   s.PriceSplitPercentage,
												   s.TrnSale,
												   s.ServiceCharge
											   } into g
											   select new
											   {
												   g.Key.ItemId,
												   g.Key.MstItem,
												   g.Key.MstItem.ItemDescription,
												   g.Key.MstUnit.Unit,
												   g.Key.Price,
												   g.Key.NetPrice,
												   g.Key.MstDiscount,
												   g.Key.DiscountId,
												   g.Key.DiscountRate,
												   g.Key.TaxId,
												   g.Key.MstTax,
												   g.Key.MstTax.Tax,
												   Amount = g.Sum(a => a.Amount),
												   Quantity = g.Sum(a => a.Quantity),
												   DiscountAmount = g.Sum(a => a.DiscountAmount * a.Quantity),
												   TaxAmount = g.Sum(a => a.TaxAmount),
												   g.Key.TrnSale.DiscountedPax,
												   g.Key.TrnSale.Pax,
												   ServiceCharge = g.Sum(a => a.ServiceCharge)
											   };

					if (salesLineGroupbyItem.Any())
					{
						var SalesDiscount = from d in salesLineGroupbyItem
											where d.MstDiscount.Discount != "Zero Discount"
											select d;

						if (SalesDiscount.Any())
						{
							discountGiven = SalesDiscount.FirstOrDefault().MstDiscount.Discount;
						}
						else
						{
							discountGiven = "Zero Discount";
						}

						foreach (var salesLine in salesLineGroupbyItem.ToList())
						{
							totalNumberOfItems += salesLine.Quantity;
							totalNetGrossSales += salesLine.Price * salesLine.Quantity;
							totalGrossSales += salesLine.Price * salesLine.Quantity;
							totalSales += salesLine.Amount;
							totalDiscount += salesLine.DiscountAmount;
							totalServiceCharge += salesLine.ServiceCharge;
							if (salesLine.MstTax.Code == "VAT")
							{
								totalVATSales += ((salesLine.Price * salesLine.Quantity) - salesLine.DiscountAmount) / (1 + (salesLine.MstItem.MstTax1.Rate / 100));
								totalVATAmount += (((salesLine.Price * salesLine.Quantity) - salesLine.DiscountAmount) / (1 + (salesLine.MstItem.MstTax1.Rate / 100)) * (salesLine.MstItem.MstTax1.Rate / 100));
							}
							else if (salesLine.MstTax.Code == "NONVAT")
							{
								totalNonVATSales += (salesLine.Price * salesLine.Quantity) - salesLine.DiscountAmount;
							}
							else if (salesLine.MstTax.Code == "EXEMPTVAT")
							{
								if (salesLine.MstItem.MstTax1.Rate > 0)
								{
									if (salesLine.Pax.GetValueOrDefault() > 1)
									{
										subtotalVATExempt += ((salesLine.Price * salesLine.Quantity) / salesLine.Pax.GetValueOrDefault()) * salesLine.DiscountedPax.GetValueOrDefault();
										lessVAT += ((((salesLine.Price * salesLine.Quantity) / salesLine.Pax.GetValueOrDefault()) * salesLine.DiscountedPax.GetValueOrDefault()) / (1 + (salesLine.MstItem.MstTax1.Rate / 100)) * (salesLine.MstItem.MstTax1.Rate / 100));
										totalVATSales += (((salesLine.Price * salesLine.Quantity) / salesLine.Pax.GetValueOrDefault()) * (salesLine.Pax.GetValueOrDefault() - salesLine.DiscountedPax.GetValueOrDefault())) / (1 + (salesLine.MstItem.MstTax1.Rate / 100));
										totalVATAmount += ((((salesLine.Price * salesLine.Quantity) / salesLine.Pax.GetValueOrDefault()) * (salesLine.Pax.GetValueOrDefault() - salesLine.DiscountedPax.GetValueOrDefault())) / (1 + (salesLine.MstItem.MstTax1.Rate / 100)) * (salesLine.MstItem.MstTax1.Rate / 100));
									}
									else
									{
										subtotalVATExempt += salesLine.Price * salesLine.Quantity;
										lessVAT += ((((salesLine.Price * salesLine.Quantity) / salesLine.Pax.GetValueOrDefault()) * salesLine.DiscountedPax.GetValueOrDefault()) / (1 + (salesLine.MstItem.MstTax1.Rate / 100)) * (salesLine.MstItem.MstTax1.Rate / 100));
									}
								}
								else
								{
									subtotalVATExempt += salesLine.Price * salesLine.Quantity;
								}
							}
							else if (salesLine.MstTax.Code == "ZEROVAT")
							{
								totalVATZeroRated += (salesLine.Price * salesLine.Quantity) - salesLine.DiscountAmount;
							}

							String itemData = salesLine.ItemDescription + "\n" + salesLine.Quantity.ToString() + " " + salesLine.Unit + " @ " + salesLine.Price.ToString();
							Decimal itemAmountData = salesLine.Price * salesLine.Quantity;
							RectangleF itemDataRectangle = new RectangleF
							{
								X = x,
								Y = y,
								Size = new Size(150, ((int)graphics.MeasureString(itemData, fontArial8Regular, 150, StringFormat.GenericDefault).Height))
							};
							graphics.DrawString(itemData, fontArial8Regular, Brushes.Black, itemDataRectangle, drawFormatLeft);

							graphics.DrawString(itemAmountData.ToString("#,##0.00"), fontArial8Regular, drawBrush, new RectangleF(x, y, 250.0F, height), drawFormatRight);

							y += itemDataRectangle.Size.Height + 3.0F;
						}

						if (subtotalVATExempt != 0)
						{
							totalVATExempt += subtotalVATExempt / divider;
						}
					}
				}

				// ========
				// 2nd Line
				// ========
				Point secondLineFirstPoint = new Point(0, Convert.ToInt32(y) + 10);
				Point secondLineSecondPoint = new Point(500, Convert.ToInt32(y) + 10);
				graphics.DrawLine(blackPen, secondLineFirstPoint, secondLineSecondPoint);

				String totalNetSalesAmount = totalSales.ToString("#,##0.00");
				String totalSCAmount = totalServiceCharge.ToString("#,##0.00");

				//Decimal totalAmountDue = Convert.ToDecimal(totalNetSalesAmount) + Convert.ToDecimal(totalSCAmount);

				// ==============================
				// Total Sales and Total Discount
				// ==============================
				//String totalNetSalesLabel = "\nTotal Net Sales";
				//String totalNetSalesAmount = "\n" + totalNetGrossSales.ToString("#,##0.00");
				//graphics.DrawString(totalNetSalesLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				//graphics.DrawString(totalNetSalesAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
				//y += graphics.MeasureString(totalNetSalesAmount, fontArial8Regular).Height;

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

				//if (totalVATExempt > 0)
				//{
				//    String ExemptVATLabel = "\nVAT-Exempt Sales";
				//    String totalVATExemptAmount = "\n" + totalVATExempt.ToString("#,##0.00");
				//    graphics.DrawString(ExemptVATLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				//    graphics.DrawString(totalVATExemptAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
				//    y += graphics.MeasureString(totalVATExemptAmount, fontArial8Regular).Height;
				//}

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

				//Decimal totalAmountDue = (Convert.ToDecimal(totalGrossSales.ToString("#,##0.00")) - Convert.ToDecimal(totalDiscount.ToString("#,##0.00")) - Convert.ToDecimal(lessVAT.ToString("#,##0.00"))) + Convert.ToDecimal(totalServiceCharge.ToString("#,##0.00"));
				Decimal totalAmountDue = totalSales + totalServiceCharge;
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
				Point thirdLineFirstPoint = new Point(0, Convert.ToInt32(y) - 7);
				Point thirdLineSecondPoint = new Point(500, Convert.ToInt32(y) - 7);
				graphics.DrawLine(blackPen, thirdLineFirstPoint, thirdLineSecondPoint);

				// ================
				// Collection Lines
				// ================
				var collectionLines = from d in db.TrnCollectionLines where d.CollectionId == collections.FirstOrDefault().Id select d;
				if (collectionLines.Any())
				{
					foreach (var collectionLine in collectionLines)
					{
						String collectionLineLabel = collectionLine.MstPayType.PayType;
						String collectionLineAmount = collectionLine.Amount.ToString("#,##0.00");

						graphics.DrawString(collectionLineLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
						graphics.DrawString(collectionLineAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
						y += graphics.MeasureString(collectionLineAmount, fontArial8Regular).Height;
					}
				}

				// ======
				// Change
				// ======
				change = collections.FirstOrDefault().ChangeAmount;

				String changelabel = "Change";
				String changeAmount = change.ToString("#,##0.00");
				graphics.DrawString(changelabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				graphics.DrawString(changeAmount, fontArial12Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
				y += graphics.MeasureString(changeAmount, fontArial8Regular).Height;

				// ========
				// 4th Line
				// ========
				Point forthLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
				Point forthLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
				graphics.DrawLine(blackPen, forthLineFirstPoint, forthLineSecondPoint);

				// ============
				// VAT Analysis
				// ============
				String vatAnalysisLabel = "\nVAT ANALYSIS";
				graphics.DrawString(vatAnalysisLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				y += graphics.MeasureString(vatAnalysisLabel, fontArial8Regular).Height + +5.0F;

				String vatSalesLabel = "VAT Sales";
				String totalVatSalesAmount = totalVATSales.ToString("#,##0.00");
				graphics.DrawString(vatSalesLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				graphics.DrawString(totalVatSalesAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
				y += graphics.MeasureString(totalVatSalesAmount, fontArial8Regular).Height;

				String totalVATAmountLabel = "VAT Amount";
				String totalVatAmount = totalVATAmount.ToString("#,##0.00");
				graphics.DrawString(totalVATAmountLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				graphics.DrawString(totalVatAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
				y += graphics.MeasureString(totalVatAmount, fontArial8Regular).Height;

				String totalNonVATSalesLabel = "Non-VAT";
				String totalNonVatAmount = totalNonVATSales.ToString("#,##0.00");
				graphics.DrawString(totalNonVATSalesLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				graphics.DrawString(totalNonVatAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
				y += graphics.MeasureString(totalNonVatAmount, fontArial8Regular).Height;

				//String totalVATExclusiveLabel = "VAT Exclusive";
				//String totaltotalVATExclusiveAmount = totalVATExclusive.ToString("#,##0.00");
				//graphics.DrawString(totalVATExclusiveLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				//graphics.DrawString(totaltotalVATExclusiveAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
				//y += graphics.MeasureString(totaltotalVATExclusiveAmount, fontArial8Regular).Height;

				String totalVATExemptLabel = "VAT Exempt";
				String totaltotalVATExemptAmount = totalVATExempt.ToString("#,##0.00");
				graphics.DrawString(totalVATExemptLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				graphics.DrawString(totaltotalVATExemptAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
				y += graphics.MeasureString(totaltotalVATExemptAmount, fontArial8Regular).Height;

				String totalVATZeroRatedLabel = "VAT Zero Rated";
				String totalVatZeroRatedAmount = totalVATZeroRated.ToString("#,##0.00");
				graphics.DrawString(totalVATZeroRatedLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				graphics.DrawString(totalVatZeroRatedAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
				y += graphics.MeasureString(totalVatZeroRatedAmount, fontArial8Regular).Height;

				// ========
				// 6th Line
				// ========
				Point sixthLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
				Point sixthLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
				graphics.DrawLine(blackPen, sixthLineFirstPoint, sixthLineSecondPoint);

				// =======
				// Cashier
				// =======

				var user = from d in db.MstUsers
						   where d.Id == sales.FirstOrDefault().SalesAgent
						   select d;

				String salesAgent = user.FirstOrDefault().FullName;
				String cashier = collections.FirstOrDefault().MstUser3.FullName;

				String cashierLabel = "\nCashier";
				String cashierUserData = "\n" + cashier;
				graphics.DrawString(cashierLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				graphics.DrawString(cashierUserData, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
				y += graphics.MeasureString(cashierUserData, fontArial8Regular).Height - 10.0F;

				String salesAgentLabel = "\nSales";
				String agent = "\n" + salesAgent;
				graphics.DrawString(salesAgentLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				graphics.DrawString(agent, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
				y += graphics.MeasureString(agent, fontArial8Regular).Height - 5.0F;

				//var user = from d in db.MstUsers
				//           where d.Id == sales.FirstOrDefault().SalesAgent
				//           select d;

				//String salesAgent = user.FirstOrDefault().UserName;

				//String salesAgentLabel = "Sales Agent";
				//String salesAgentData = "\n" + salesAgent;

				// ========
				// 7th Line
				// ========
				Point seventhLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
				Point seventhLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
				graphics.DrawLine(blackPen, seventhLineFirstPoint, seventhLineSecondPoint);

				// ========
				// 9th Line
				// ========
				Point ninethLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
				Point ninethLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
				graphics.DrawLine(blackPen, ninethLineFirstPoint, ninethLineSecondPoint);

				String remarks = "\nRemarks: \n\n " + collections.FirstOrDefault().TrnSale.Remarks;
				graphics.DrawString(remarks, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
				y += graphics.MeasureString(remarks, fontArial8Regular).Height;

				//// =========
				//// 10th Line
				//// =========
				Point tenthLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
				Point tenthLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
				graphics.DrawLine(blackPen, tenthLineFirstPoint, tenthLineSecondPoint);
				foreach (var collectionLine in collectionLines)
				{
					if (collectionLine.MstPayType.PayTypeCode == "CREDITCARD")
					{
						String CCnumber = collectionLine.CreditCardNumber;
						String CCHolder = collectionLine.CreditCardHolderName;
						String creditCardHolder = "\nCredit Card Holder:" + CCHolder;
						graphics.DrawString(creditCardHolder, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
						y += graphics.MeasureString(creditCardHolder, fontArial8Regular).Height;

						String newCC = new string('*', CCnumber.Length - 4) + CCnumber.Substring(CCnumber.Length - 4);

						String creditCardNumber = "Credit Card Number:" + newCC;
						graphics.DrawString(creditCardNumber, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
						y += graphics.MeasureString(creditCardNumber, fontArial8Regular).Height;

						String creditCardType = "Credit Card Type:" + collectionLine.CreditCardType;
						graphics.DrawString(creditCardType, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
						y += graphics.MeasureString(creditCardType, fontArial8Regular).Height;

						String creditCardBank = "Credit Card Bank:" + collectionLine.CreditCardBank;
						graphics.DrawString(creditCardBank, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
						y += graphics.MeasureString(creditCardNumber, fontArial8Regular).Height;
					}
					else if (collectionLine.OtherInformation != "NA" || collectionLine.OtherInformation != "")
					{
						String OtherInfo = "Other Information:" + "\n";
						graphics.DrawString(OtherInfo, fontArial8Regular, drawBrush, new RectangleF(x, y + 6, width, height), drawFormatLeft);
						y += graphics.MeasureString(OtherInfo, fontArial8Regular).Height;
						String OtherInfoData = collectionLine.MstPayType.PayType + " " + collectionLine.OtherInformation;
						graphics.DrawString(OtherInfoData, fontArial8Regular, drawBrush, new RectangleF(x, y + 6, width, height), drawFormatLeft);
						y += graphics.MeasureString(OtherInfoData, fontArial8Regular).Height;
					}
					else
					{
					}
				}
				if (collections.FirstOrDefault().TrnCollectionLines.FirstOrDefault().OtherInformation != "NA" || collections.FirstOrDefault().TrnCollectionLines.FirstOrDefault().OtherInformation != "")
				{
					// =========
					// 12th Line
					// =========
					Point twelvethLineFirstPoint = new Point(0, Convert.ToInt32(y) + 18);
					Point twelvethLineSecondPoint = new Point(500, Convert.ToInt32(y) + 18);
					graphics.DrawLine(blackPen, twelvethLineFirstPoint, twelvethLineSecondPoint);

					String receiptFooter = "\n" + "Receipt Footer";
					graphics.DrawString(receiptFooter, fontArial8Regular, drawBrush, new RectangleF(x, y + 5, width, height), drawFormatCenter);
					y += graphics.MeasureString(receiptFooter, fontArial8Regular).Height;
				}
				else
				{
					// =========
					// 12th Line
					// =========
					Point twelvethLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
					Point twelvethLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
					graphics.DrawLine(blackPen, twelvethLineFirstPoint, twelvethLineSecondPoint);

					String receiptFooter = "\n" + "Footer";
					graphics.DrawString(receiptFooter, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
					y += graphics.MeasureString(receiptFooter, fontArial8Regular).Height;
				}
			}

			String space = "\nThank you and come again!";
			graphics.DrawString(space, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
		}
	}
}
