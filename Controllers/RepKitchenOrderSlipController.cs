using Mono.Cecil;
using PrintProcessor.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace PrintProcessor.Controllers
{
    public class RepKitchenOrderSlipController
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
        private string _companyName = "";
        private string _address = "";
        private string _tin = "";
        private string _serialNo = "";
        private string _machineNo = "";
        private string _receipfooter = "";
        private string _orTitle = "";
        private string _printerType = "";
        private string _invoicefooter = "";
        private Boolean _showCustomerInfo = false;

        // =============
        // Print Receipt
        // =============
        public void PrintKitchenOrderSlip(int salesId, int terminalId, string type, string printerName, List<SysGeneralSettingsModel> generalSettingsList)
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
                    _showCustomerInfo = generalSettingsList[i].ShowCustomerInfo;
                }

                if (useDefaultPrinter) this.GetDefaultPrinter();

                PrinterSettings ps = new PrinterSettings
                {
                    PrinterName = _printer
                };

                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(PrintKitchenOrderSlipPage);
                pd.PrinterSettings = ps;
                pd.Print();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

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
        public void PrintKitchenOrderSlipPage(object sender, PrintPageEventArgs e)
        {
            // ============
            // Data Context
            // ============
            Data.EasyRestaurantDBDataContext db = new Data.EasyRestaurantDBDataContext();

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
            //    x = 5; y = 5;
            //    width = 245.0F; height = 0F;
            //}
            //else if (Modules.SysCurrentModule.GetCurrentSettings().PrinterType == "Thermal Printer")
            //{
            //    x = 5; y = 5;
            //    width = 260.0F; height = 0F;
            //}
            //else
           // {
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
           // var systemCurrent = Modules.SysCurrentModule.GetCurrentSettings();

            // =================
            // Sales Order Title
            // =================
            String officialReceiptTitle = "O R D E R   S L I P";
            graphics.DrawString(officialReceiptTitle, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            y += graphics.MeasureString(officialReceiptTitle, fontArial8Regular).Height;

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

                if (_showCustomerInfo == true)
                {
                    // ==============
                    // Customer Line
                    // ==============
                    var customerLines = from d in db.TrnSales where d.Id == _salesId select d;
                    if (customerLines.Any())
                    {
                        var customer = from d in customerLines
                                       where d.CustomerId == d.MstCustomer.Id
                                       select d;
                        if (customer.Any())
                        {
                            //Customer Name
                            String customerNameLabel = "\nCustomer Name";
                            String customerName = "\n" + customer.FirstOrDefault().MstCustomer.Customer;
                            graphics.DrawString(customerNameLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                            graphics.DrawString(customerName, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                            y += graphics.MeasureString(customerName, fontArial8Regular).Height;
                            //Birthday
                            String customerBdayLabel = "Birthday";
                            String customerBday = Convert.ToDateTime(customer.FirstOrDefault().MstCustomer.Birthday).ToShortDateString();
                            graphics.DrawString(customerBdayLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                            graphics.DrawString(customerBday, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                            y += graphics.MeasureString(customerBday, fontArial8Regular).Height;
                            //Age
                            String customerAgeLabel = "Age";
                            String customerAge = Convert.ToString(customer.FirstOrDefault().MstCustomer.Age);
                            graphics.DrawString(customerAgeLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                            graphics.DrawString(customerAge, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                            y += graphics.MeasureString(customerAge, fontArial8Regular).Height;
                            //Gender
                            String customerGenderLabel = "Gender";
                            String customerGender = customer.FirstOrDefault().MstCustomer.Gender;
                            graphics.DrawString(customerGenderLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                            graphics.DrawString(customerGender, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                            y += graphics.MeasureString(customerGender, fontArial8Regular).Height;
                            //Contact Number
                            String customerContactNumberLabel = "Contact Number";
                            String customerContactNumber = customer.FirstOrDefault().MstCustomer.ContactNumber;
                            graphics.DrawString(customerContactNumberLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                            graphics.DrawString(customerContactNumber, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                            y += graphics.MeasureString(customerContactNumber, fontArial8Regular).Height;
                            //Email
                            String customerEmailLabel = "Email Address";
                            String customerEmail = customer.FirstOrDefault().MstCustomer.EmailAddress;
                            graphics.DrawString(customerEmailLabel, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                            graphics.DrawString(customerEmail, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                            y += graphics.MeasureString(customerEmail, fontArial8Regular).Height;

                            // ========
                            // Customer Line
                            // ========
                            Point customerLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                            Point customerLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                            graphics.DrawLine(blackPen, customerLineFirstPoint, customerLineSecondPoint);
                        }
                    }
                }

                // ==========
                // Sales Line
                // ==========
                Decimal totalAmount = 0;
                Decimal totalNumberOfItems = 0;
                String tableLabel = "";
                if (sales.FirstOrDefault().MstTable.TableCode != "Walk-in" && sales.FirstOrDefault().MstTable.TableCode != "Delivery")
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
                String qty = "\nQUANTITY";
                graphics.DrawString(itemLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                graphics.DrawString(qty, fontArial8Regular, drawBrush, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                y += graphics.MeasureString(itemLabel, fontArial8Regular).Height + 5.0F;

                var salesLines = from d in db.TrnSalesLines
                                 where d.SalesId == _salesId
                                 //for pooling
                                 
                                 //&& d.MstItem.DefaultKitchenReport == kitchenReport
                                 
                                 //&& d.IsPrinted == false
                                 select d;
                //foreach (DataGridViewRow row in _orderPrintTable.Rows)
                //{
                    foreach (var SL in salesLines.OrderBy(d => d.ItemHeaderId))
                    {
                        //if (Convert.ToInt32(row.Cells[0].Value) == SL.Id && Convert.ToBoolean(row.Cells[3].Value) == true)
                        //{
                            totalNumberOfItems += 1;

                            totalAmount += SL.Amount;
                            if (SL.Preparation == "NA")
                            {
                                SL.Preparation = "";
                            }

                            var equalItemId = from s in db.MstItems
                                              where s.Id == SL.ItemId
                                              select s;

                            String itemData = "";
                            String qtyData = "";
                            if (equalItemId.FirstOrDefault().Category == "Add-On")
                            {
                                itemData = SL.MstItem.ItemDescription;
                                qtyData = SL.Quantity.ToString("N2", CultureInfo.InvariantCulture);

                                RectangleF itemDataRectangle = new RectangleF
                                {
                                    X = 20,
                                    Y = y,
                                    Size = new Size(170, ((int)graphics.MeasureString(itemData, fontArial8Regular, 170, StringFormat.GenericTypographic).Height)),
                                    Width = width
                                };

                                graphics.DrawString(itemData, fontArial8Regular, Brushes.Black, itemDataRectangle, drawFormatLeft);
                                graphics.DrawString(qtyData, fontArial8Regular, Brushes.Black, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                                y += itemDataRectangle.Size.Height + 3.0F;
                            }
                            else if (equalItemId.FirstOrDefault().Category == "Item Modifier")
                            {
                                itemData = SL.MstItem.ItemDescription;

                                RectangleF itemDataRectangle = new RectangleF
                                {
                                    X = 20,
                                    Y = y,
                                    Size = new Size(170, ((int)graphics.MeasureString(itemData, fontArial8Regular, 170, StringFormat.GenericTypographic).Height)),
                                    Width = width
                                };

                                graphics.DrawString(itemData, fontArial8Regular, Brushes.Black, itemDataRectangle, drawFormatLeft);
                                graphics.DrawString(qtyData, fontArial8Regular, Brushes.Black, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                                y += itemDataRectangle.Size.Height + 3.0F;
                            }
                            else
                            {
                                itemData =  SL.MstItem.ItemDescription;
                                qtyData = SL.Quantity.ToString("N2", CultureInfo.InvariantCulture);

                                RectangleF itemDataRectangle = new RectangleF
                                {
                                    X = x,
                                    Y = y,
                                    Size = new Size(170, ((int)graphics.MeasureString(itemData, fontArial8Regular, 170, StringFormat.GenericTypographic).Height)),
                                    Width = width
                                };

                                graphics.DrawString(itemData, fontArial8Regular, Brushes.Black, itemDataRectangle, drawFormatLeft);
                                graphics.DrawString(qtyData, fontArial8Regular, Brushes.Black, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                                y += itemDataRectangle.Size.Height + 3.0F;
                            }
                            
                            //String itemAmountData = (salesLine.Amount + salesLine.DiscountAmount).ToString("#,##0.00");

                            //graphics.DrawString(qtyData, fontArial8Regular, Brushes.Black, new RectangleF(x, y, 245.0F, height), drawFormatRight);
                            //if (Modules.SysCurrentModule.GetCurrentSettings().PrinterType == "Dot Matrix Printer")
                            //{
                            //    graphics.DrawString(SL.Preparation, fontArial8Regular, drawBrush, new RectangleF(x + 150, y, 100, height), drawFormatLeft);
                            //}
                            //else
                            //{
                            //    graphics.DrawString(SL.Preparation, fontArial8Regular, drawBrush, new RectangleF(x + 150, y, 100, height), drawFormatLeft);
                            //}
                            //if (SL.Preparation.Length > 60)
                            //{
                            //    y += itemDataRectangle.Size.Height + 30.0F;
                            //}
                            //else if (SL.Preparation.Length > 50)
                            //{
                            //    y += itemDataRectangle.Size.Height + 20.0F; //need to minus
                            //}
                            //else if (SL.Preparation.Length > 40)
                            //{
                            //    y += itemDataRectangle.Size.Height + 12.0F;
                            //}
                            //else
                            //{
                            //    y += itemDataRectangle.Size.Height + 3.0F;
                            //}
                        //}
                    }
               // }

                // ========
                // 2nd Line
                // ========
                Point secondLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                Point secondLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                graphics.DrawLine(blackPen, secondLineFirstPoint, secondLineSecondPoint);

                // ============
                // Total Amount
                // ============
                //String totalSalesLabel = "\nTotal Amount";
                //String totalSalesAmount = "\n" + totalAmount.ToString("#,##0.00");
                //graphics.DrawString(totalSalesLabel, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                //graphics.DrawString(totalSalesAmount, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatRight);
                //y += graphics.MeasureString(totalSalesAmount, fontArial8Regular).Height;

                //String totalNumberOfItemsLabel = "Total No. of Item(s)";
                //String totalNumberOfItemsQuantity = totalNumberOfItems.ToString("#,##0.00");
                //graphics.DrawString(totalNumberOfItemsLabel, fontArial8Regular, drawBrush, new RectangleF(x, y + 5, width, height), drawFormatLeft);
                //graphics.DrawString(totalNumberOfItemsQuantity, fontArial8Regular, drawBrush, new RectangleF(x, y + 5, width, height), drawFormatRight);
                //y += graphics.MeasureString(totalNumberOfItemsQuantity, fontArial8Regular).Height;

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

                //// ========
                //// 6th Line
                //// ========
                //Point sixthLineFirstPoint = new Point(0, Convert.ToInt32(y) + 5);
                //Point sixthLineSecondPoint = new Point(500, Convert.ToInt32(y) + 5);
                //graphics.DrawLine(blackPen, sixthLineFirstPoint, sixthLineSecondPoint);

                //String salesInvoiceFooter = "\n" + systemCurrent.InvoiceFooter;
                //graphics.DrawString(salesInvoiceFooter, fontArial8Regular, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
                //y += graphics.MeasureString(salesInvoiceFooter, fontArial8Regular).Height;

            }
            if (_printerType == "Dot Matrix Printer")
            {
                String space = "\n\n\n\n\n\n\n\n\n\n.";
                graphics.DrawString(space, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            }
            else
            {
                String space = "\n\n\n.";
                graphics.DrawString(space, fontArial8Bold, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
            }

            var salesLinesNotPrinted = from d in db.TrnSalesLines
                                       where d.SalesId == _salesId
                                       
                                       //&& d.MstItem.DefaultKitchenReport == kitchenReport
                                       && d.IsPrinted == false
                                       select d;

            if (salesLinesNotPrinted.Any())
            {
                foreach (var salesLine in salesLinesNotPrinted)
                {
                    var updateSalesLines = salesLine;
                    updateSalesLines.IsPrinted = true;
                    db.SubmitChanges();
                }
            }
        }
    }

}
