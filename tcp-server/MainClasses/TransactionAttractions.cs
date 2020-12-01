using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class TransactionAttractions
    {
        public int id { get; set; }
        public int cardId { get; set; }
        public object attractionId { get; set; }
        public int operation { get; set; }
        public decimal summ { get; set; }
        public object bonus { get; set; }
        public object tickets { get; set; }
        public DateTime date { get; set; }
        public decimal summBalance { get; set; }
        public decimal bonusesBalance { get; set; }
        public int ticketsBalance { get; set; }



        public TransactionAttractions()
        {

        }
        public TransactionAttractions(
            int id,
            int cardId,
            object attractionId,
            int operation,
            decimal summ,
            object bonus,
            object tikets,
            DateTime date,
            decimal summBalance,
            decimal bonusesBalance,
            int ticketsBalance
            )
        {
            this.id = id;
            this.attractionId = attractionId;
            this.operation = operation;
            this.summ = summ;
            this.bonus = bonus;
            this.tickets = tikets;
            this.date = date;
            this.summBalance = summBalance;
            this.bonusesBalance = bonusesBalance;
            this.ticketsBalance = ticketsBalance;
        }
    }
}
