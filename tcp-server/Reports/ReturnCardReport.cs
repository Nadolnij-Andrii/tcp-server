using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class ReturnCardReport
    {   
        public List<ReturnCard> returnedCards { get; set; }
        public void GetRetunCardReport(DateTime fromDate, DateTime toDate)
        {
            SqlConn conn = new SqlConn();
            List<TransactionCashRegister> transactionsCashRegister = conn.selectTransactionCashRegister("transactions_cashiermashine",
                       " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                       + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            this.returnedCards = new List<ReturnCard>();
            foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
            {
                if ((int)transactionCashRegister.operation == 2 && (decimal)transactionCashRegister.summ != 0)
                {
                    returnedCards.Add(
                        new ReturnCard(
                            transactionCashRegister.cardId,
                            transactionCashRegister.date,
                            (decimal)transactionCashRegister.summ,
                            0,
                            (string)transactionCashRegister.cashier_name
                            )
                        );
                        
                }
                if ((int)transactionCashRegister.operation == 20 && (decimal)transactionCashRegister.summ != 0)
                {
                    returnedCards.Add(
                        new ReturnCard(
                            transactionCashRegister.cardId,
                            transactionCashRegister.date,
                            0,
                            (decimal)transactionCashRegister.summ,
                            (string)transactionCashRegister.cashier_name
                            )
                        );

                }
            }
        }
        public class ReturnCard
        {
            public int CardId { get; set; }
            public DateTime Date { get; set; }
            public decimal Summ { get; set; }
            public decimal CardPrice { get; set; }
            public string CashierName { get; set; }

            public ReturnCard(
                int CardId,
                DateTime Date,
                decimal Summ,
                decimal CardPrice,
                string CashierName
                )
            {
                this.CardId = CardId;
                this.Date = Date;
                this.Summ = Summ;
                this.CardPrice = CardPrice;
                this.CashierName = CashierName;
            }

        }
    }
}
