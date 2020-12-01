using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class CardEvent
    {
        public object id { get; set; }
        public object cardId { get; set; }
        public object toCardId { get; set; }
        public object cardEvent { get; set; }
        public object cardMessage { get; set; }
        public object cashierRegisterId { get; set; }
        public object cashierRegisterIP { get; set; }
        public object cashierId { get; set; }
        public object cashierName { get; set; }
        public DateTime date { get; set; }
        public decimal summBalance { get; set; }
        public decimal bonusesBalance { get; set; }
        public int ticketsBalance { get; set; }


        public CardEvent()
        {

        }
        public CardEvent(
            object id,
            object cardId,
            object toCardId,
            object cardEvent,
            object cardMessage,
            object cashierRegisterId,
            object cashierRegisterIP,
            object cashierId,
            object cashierName,
            DateTime date
            )
        {
            this.id = id;
            this.cardId = cardId;
            this.toCardId = toCardId;
            this.cardEvent = cardEvent;
            this.cardMessage = cardMessage;
            this.cashierRegisterId = cashierRegisterId;
            this.cashierRegisterId = cashierRegisterIP;
            this.cashierId = cashierId;
            this.cashierName = cashierName;
            this.date = date;
            this.summBalance = summBalance;
            this.bonusesBalance = bonusesBalance;
            this.ticketsBalance = ticketsBalance;
        }
    }
}
