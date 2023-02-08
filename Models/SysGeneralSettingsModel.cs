using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintProcessor.Models
{
    public  class SysGeneralSettingsModel
    {
        public String CompanyName { get; set; }
        public String Address { get; set; }
        public String ContactNo { get; set; }
        public String CompanyImage { get; set; }
        public String TIN { get; set; }
        public String AccreditationNo { get; set; }
        public String SerialNo { get; set; }
        public String PermitNo { get; set; }
        public String MachineNo { get; set; }
        public String DeclareRate { get; set; }
        public String ReceiptFooter { get; set; }
        public String WithdrawalFooter { get; set; }
        public String InvoiceFooter { get; set; }
        public String LicenseCode { get; set; }
        public String TenantOf { get; set; }
        public String CurrentUserId { get; set; }
        public String CurrentUserName { get; set; }
        public String CurrentVersion { get; set; }
        public String CurrentDeveloper { get; set; }
        public String CurrentSupport { get; set; }
        public String CurrentPeriodId { get; set; }
        public String CurrentDate { get; set; }
        public String TerminalId { get; set; }
        public String WalkinCustomerId { get; set; }
        public String DefaultDiscountId { get; set; }
        public String ReturnSupplierId { get; set; }
        public String WithdrawalPrintTitle { get; set; }
        public String ORPrintTitle { get; set; }
        public Boolean IsTenderPrint { get; set; }
        public Boolean IsBarcodeQuantityAlwaysOne { get; set; }
        public Boolean WithCustomerDisplay { get; set; }
        public String CustomerDisplayPort { get; set; }
        public String CustomerDisplayBaudRate { get; set; }
        public String CustomerDisplayFirstLineMessage { get; set; }
        public String CustomerDisplayIfCounterClosedMessage { get; set; }
        public String CollectionReport { get; set; }
        public String ZReadingFooter { get; set; }
        public String EasypayAPIURL { get; set; }
        public String EasypayDefaultUsername { get; set; }
        public String EasypayDefaultPassword { get; set; }
        public String EasypayMotherCardNumber { get; set; }
        public String ActivateAuditTrail { get; set; }
        public String FacepayImagePath { get; set; }
        public String POSType { get; set; }
        public Boolean AllowNegativeInventory { get; set; }
        public Boolean IsLoginDate { get; set; }
        public Boolean EnableEasyShopIntegration { get; set; }
        public Boolean PromptLoginSales { get; set; }
        public String PrinterType { get; set; }
        public Boolean SwipeLogin { get; set; }
        public Boolean DateLogin { get; set; }
        public Boolean HideSalesAmount { get; set; }
        public Boolean HideStockInPriceAndCost { get; set; }
        public Boolean HideSalesItemDetail { get; set; }
        public Boolean HideSalesItemListBarcode { get; set; }
        public Boolean HideSalesItemListItemCode { get; set; }
        public Boolean LockAutoSales { get; set; }
        public Boolean ShowCustomerInfo { get; set; }
        public Boolean ChoosePrinter { get; set; }
        public Boolean IsTriggeredQuantity { get; set; }
        public Boolean EnableEditPrice { get; set; }
        public Boolean PrinterReady { get; set; }
        public String SalesOrderPrinterType { get; set; }
        public String TenantName { get; set; }
        public String TenantCode { get; set; }
        public String SalesType { get; set; }
        public String IPAddress { get; set; }
        public String ServerIP { get; set; }
        public String TenantID { get; set; }
        public String FlashNotes { get; set; }
        public String LastUserId { get; set; }
        public Boolean IsOverRide { get; set; }
        public String RLCServerIP { get; set; }
        public String EODPerformedDate { get; set; }
        public Boolean ShowServiceCharge { get; set; }
        public String CompanyLogoPath { get; set; }
        public Boolean EnableParkingSystem { get; set; }
        public Boolean AllowTenderZero { get; set; }
        public Boolean EnableDTRFeature { get; set; }
        public Boolean DisableDate { get; set; }
        public Boolean ExcludeZeroPrices { get; set; }
        public Boolean AutoStartIntegration { get; set; }
        public Boolean EnablePartialBillCollection { get; set; }
        public String CurrencyId { get; set; }
        public Boolean HideSalesDetail { get; set; }
        public Boolean WithEasyRestaurant { get; set; }
        public Boolean PromptPreviousEOD { get; set; }
        public String RestaurantToken { get; set; }
        public String RestaurantDomain { get; set; }
        public String LipadTenant { get; set; }
        public String LipadDomain { get; set; }
        public String LipadToken { get; set; }
        public String SMTenant { get; set; }
        public String SMSalesType { get; set; }
        public String SMTerminalId { get; set; }
        public String TransactionType { get; set; }
        public Boolean IsLock { get; set; }
    }
}
