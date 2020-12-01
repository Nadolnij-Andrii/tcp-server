using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class SellPrizesReport
    {
        public List<SellPrize> sellPrizes { get; set; }
        public SellPrizesOther sellPrizesOther { get; set; }
        public SellPrizesTotal sellPrizesTotal { get; set; }
        public void GetSellPrizesReport(DateTime fromDate, DateTime toDate)
        {
            SqlConn conn = new SqlConn();
            List<TransactionCashRegister> transactionsCashRegister = conn.selectTransactionCashRegister("transactions_cashiermashine",
                       " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                       + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            List<Ticket> tickets = conn.selectTickets("tickets");
            int allTotal = 0, allCount = 0;
            this.sellPrizes = new List<SellPrize>();
            int otherTotal = 0, otherCount = 0;
            foreach (Ticket ticket in tickets)
            {
                foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
                {
                    if ((int)transactionCashRegister.operation == 9 && (int)transactionCashRegister.tickets == (int)ticket.prise)
                    {
                        ticket.count++;
                        ticket.total += (int)transactionCashRegister.tickets;
                        allCount++;
                    }
                    
                }
                allTotal += ticket.total;
                sellPrizes.Add(
                    new SellPrize(
                    "Погашение " + ticket.prise + " билетов", 
                    ticket.count, 
                    (int)ticket.prise, 
                    ticket.total));
            }
            foreach(TransactionCashRegister transactionCashRegister in transactionsCashRegister)
            {
                if ((int)transactionCashRegister.operation == 9 && tickets.Find(x => (int)x.prise == (int)transactionCashRegister.tickets) == null)
                {
                    otherTotal += (int)transactionCashRegister.tickets;
                    otherCount++;
                }
            }
            this.sellPrizesOther = new SellPrizesOther(otherCount,
                otherTotal
                );
            this.sellPrizesTotal = new SellPrizesTotal(
                allCount+otherCount, 
                allTotal+otherTotal);
        }
        public class SellPrize
        {
            public string Discribe { get; set; }
            public int Count { get; set; }
            public int Price { get; set; }
            public int Total { get; set; }

            public SellPrize(
                  string Discribe,
                  int Count,
                  int Price,
                  int Total
                )
            {
                this.Discribe = Discribe;
                this.Count = Count;
                this.Price = Price;
                this.Total = Total;
            }
        }
        public class SellPrizesOther
        {
            public int otherCount { get; set; }
            public int otherTotal { get; set; }
            public SellPrizesOther(
                int otherCount,
                int otherTotal
                )
            {
                this.otherCount = otherCount;
                this.otherTotal = otherTotal;
            }

        }
        public class SellPrizesTotal
        {
            public int AllCount { get; set; }
            public int AllTotal { get; set; }
            public SellPrizesTotal(
                int AllCount,
                int AllTotal
                )
            {
                this.AllCount = AllCount;
                this.AllTotal = AllTotal;
            }

        }
    }
}
