using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class CardPrice
    {
        public int id { get; set; }
        public int cardId { get; set; }
        public decimal cardPrice { get; set; }

        public CardPrice ()
        {

        }
        public CardPrice(int id, int cardId, decimal cardPrice)
        {
            this.id = id;
            this.cardId = cardId;
            this.cardPrice = cardPrice;
        }
    }
}
