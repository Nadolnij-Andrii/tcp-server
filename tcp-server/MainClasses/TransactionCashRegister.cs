using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class TransactionCashRegister
    {
        public int id { get; set; }
        public int cardId { get; set; }
        public int toCardId { get; set; }
        public int operation { get; set; }
        public decimal summ { get; set; }
        public decimal bonus { get; set; }
        public int tickets { get; set; }
        public int cashier_register_id { get; set; }
        public int cashier_id { get; set; }
        public string cashier_name { get; set; }
        public DateTime date { get; set; }
        public decimal summBalance { get; set; }
        public decimal bonusesBalance { get; set; }
        public int ticketsBalance { get; set; }


        public TransactionCashRegister()
        {

        }
        public TransactionCashRegister(
            int id,
            int cardId,
            int toCardId,
            int operation,
            decimal summ,
            decimal bonus,
            DateTime date,
            int tickets,
            int cashier_id,
            string cashier_name,
            int cashier_register_id,
            decimal summBalance,
            decimal bonusesBalance,
            int ticketsBalance
            )
        {
            this.id = id;
            this.cardId = cardId;
            this.toCardId = toCardId;
            this.operation = operation;
            this.summ = summ;
            this.bonus = bonus;
            this.date = date;
            this.tickets = tickets;
            this.cashier_id = cashier_id;
            this.cashier_name = cashier_name;
            this.cashier_register_id = cashier_register_id;
            this.summBalance = summBalance;
            this.bonusesBalance = bonusesBalance;
            this.ticketsBalance = ticketsBalance;
        }
    }
}
