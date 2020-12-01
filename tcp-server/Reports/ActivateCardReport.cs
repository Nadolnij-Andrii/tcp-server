using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class ActivateCardReport
    {
        public List<ActivateCard> activatedCards { get; set; }
        public void GetActivateCardReport(DateTime fromDate, DateTime toDate)
        {
            SqlConn conn = new SqlConn();

            List<CardEvent> cardEvents = conn.selectCardEvents("card_event",
                       " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                       + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            this.activatedCards = new List<ActivateCard>();
            foreach (CardEvent cardEvent in cardEvents)
            {
                if ((int)cardEvent.cardEvent == 1)
                {
                    activatedCards.Add( 
                        new ActivateCard(
                        cardEvent.date, 
                        (int)cardEvent.cashierRegisterId, 
                        (string)cardEvent.cashierName, 
                        (int)cardEvent.cardId));
                }
            }
        }
        public class ActivateCard
        {
            public DateTime Date { get; set; }
            public int CashierRegisterId { get; set; }
            public string CashierName { get; set; }
            public int CardId { get; set; }

            public ActivateCard(
                DateTime Date,
                int CashierRegisterId,
                string CashierName,
                int CardId
                )
            {
                this.Date = Date;
                this.CashierRegisterId = CashierRegisterId;
                this.CashierName = CashierName;
                this.CardId = CardId;
            }
        }
    }
}
