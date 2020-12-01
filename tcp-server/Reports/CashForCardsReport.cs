
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tcp_server
{
    public class CashForCardsReport
    {
        public List<CashForCardReport> cashForCardReports { get; set; }
        public decimal TotalSold { get; set; }
        public int TotalSoldCount { get; set; }

        public CashForCardsReport()
        {

        }
        public CashForCardsReport(List<CashForCardReport> cashForCardReports,
            decimal TotalSold,
            int TotalSoldCount)
        {
            this.cashForCardReports = cashForCardReports;
            this.TotalSold = TotalSold;
            this.TotalSoldCount = TotalSoldCount;
        }
        public class CashForCardReport
        {
            public int cardId { get; set; }
            public string clientName { get; set; }
            public int cashierid { get; set; }
            public string cashierName { get; set; }
            public int cashierRegisterId { get; set; }
            public decimal cardPrice { get; set; }
            public DateTime date { get; set; }
            public CashForCardReport()
            {

            }
            public CashForCardReport(
                int cardId,
                string clientName,
                int cashierid,
                string cashierName,
                int cashierRegisterId,
                decimal cardPrice,
                DateTime date
                )
            {
                this.cardId = cardId;
                this.clientName = clientName;
                this.cashierid = cashierid;
                this.cashierName = cashierName;
                this.cashierRegisterId = cashierRegisterId;
                this.cardPrice = cardPrice;
                this.date = date;
            }
        }
        public void GetCashforCardsReport(DateTime fromDate, DateTime toDate)
        {

            SqlConn sqlConn = new SqlConn();
            cashForCardReports = new List<CashForCardReport>();
            List<TransactionCashRegister> transactionsCashRegister = new List<TransactionCashRegister>();
            transactionsCashRegister = sqlConn.selectTransactionCashRegister("transactions_cashiermashine",
                           " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '" + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            if (transactionsCashRegister != null && transactionsCashRegister.Count > 0)
            {
                TotalSold = 0;
                TotalSoldCount = 0;
                foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
                {
                    if (transactionCashRegister.operation == 22)
                    {
                        Card card = new Card();
                        card = sqlConn.selectCard("cards", "card_id='" + transactionCashRegister.cardId + "'");
                        if (card != null && card.id > 0)
                        {

                            cashForCardReports.Add(new CashForCardReport(transactionCashRegister.cardId,
                                card.cardParentName,
                                transactionCashRegister.cashier_id,
                                transactionCashRegister.cashier_name,
                                transactionCashRegister.cashier_register_id,
                                transactionCashRegister.summ,
                                transactionCashRegister.date));
                            TotalSold += transactionCashRegister.summ;
                            TotalSoldCount++;
                        }
                    }
                }
            }
        }
    }
}