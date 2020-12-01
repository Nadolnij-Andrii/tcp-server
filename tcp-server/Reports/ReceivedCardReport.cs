using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    
    class ReceivedCardReport
    {
        public List<ReceivedCard> receivedCards { get; set; }
        public void GetReceivedCardReport(DateTime fromDate, DateTime toDate)
        {
            SqlConn conn = new SqlConn();
            List<TransactionCashRegister> transactionsCashRegister = conn.selectTransactionCashRegister("transactions_cashiermashine",
                       " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                       + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            this.receivedCards = new List<ReceivedCard>();
            foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
            {
                if (((int)transactionCashRegister.operation >= 5 && (int)transactionCashRegister.operation <= 7) || (int)transactionCashRegister.operation == 22)
                {
                    CardStatus cardStatus = conn.selectCardStatus("card_state", "state_id='" + transactionCashRegister.operation + "'");
                    receivedCards.Add(
                        new ReceivedCard(
                        transactionCashRegister.date,
                        (int)transactionCashRegister.cashier_register_id,
                        (string)transactionCashRegister.cashier_name,
                        transactionCashRegister.cardId,
                        cardStatus.status_message,
                        (decimal)transactionCashRegister.summ,
                        (decimal)transactionCashRegister.bonus,
                        (int)transactionCashRegister.tickets
                        ));
                }
            }
        }
        public class ReceivedCard
        {
            public DateTime Date { get; set; }
            public int CashierRegisterId { get; set; }
            public string CashierName { get; set; }
            public int CardId { get; set; }
            public string StatusMessage { get; set; }
            public decimal Summ { get; set; }
            public decimal Bonus { get; set; }
            public int Tickets { get; set; }

            public ReceivedCard(
                  DateTime Date,
                  int CashierRegisterId,
                  string CashierName,
                  int CardId,
                  string StatusMessage,
                  decimal Summ,
                  decimal Bonus,
                  int Tickets
                )
            {
                this.Date = Date;
                this.CashierRegisterId = CashierRegisterId;
                this.CashierName = CashierName;
                this.CardId = CardId;
                this.StatusMessage = StatusMessage;
                this.Summ = Summ;
                this.Bonus = Bonus;
                this.Tickets = Tickets;
            }

        }
    }
}
