using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class ReplacementCardReport
    {
        public List<ReplacementCard> replacementCards = new List<ReplacementCard>();
        public void GetReplacementCardReport(DateTime fromDate, DateTime toDate)
        {
            SqlConn conn = new SqlConn();
            List<TransactionCashRegister> transactionsCashRegister = conn.selectTransactionCashRegister("transactions_cashiermashine",
                       " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                       + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            this.replacementCards = new List<ReplacementCard>();
            foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
            {
                if ((int)transactionCashRegister.operation == 13 && (decimal)transactionCashRegister.toCardId != 0)
                {
                    replacementCards.Add( new ReplacementCard (
                        transactionCashRegister.date, 
                        (int)transactionCashRegister.cashier_register_id, 
                        (string)transactionCashRegister.cashier_name, 
                        transactionCashRegister.cardId, 
                        transactionCashRegister.toCardId));
                }
            }
        }

        public class ReplacementCard
        {
            public DateTime Date { get; set; }
            public int CashierRegisterId { get; set; }
            public string CashierName { get; set; }
            public int CardId { get; set; }
            public int ToCardId { get; set; }
            public ReplacementCard(
                  DateTime Date,
                  int CashierRegisterId,
                  string CashierName,
                  int CardId,
                  int ToCardId
                )
            {
                this.Date = Date;
                this.CashierRegisterId = CashierRegisterId;
                this.CashierName = CashierName;
                this.CardId = CardId;
                this.ToCardId = ToCardId;
            }

        }
    }
}
