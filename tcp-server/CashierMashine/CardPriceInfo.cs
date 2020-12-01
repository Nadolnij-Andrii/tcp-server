using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tcp_server
{
    public class CardPriceInfo
    {
        public CardStatus cardStatus { get; set; }
        public Admin admin { get; set; }

        public CardPriceInfo()
        {

        }
        public CardPriceInfo(CardStatus cardStatus, Admin admin)
        {
           
            this.cardStatus = cardStatus;
            this.admin = admin;
        }
        public bool ChangeCardPrice()
        {

            if (admin.check())
            {
                SqlConn sqlConn = new SqlConn();
                List<Pair> pairs = new List<Pair>();
                pairs.Add(new Pair("state_message", cardStatus.status_message));
                sqlConn.update("card_state", "state_id='" + 17 + "'", pairs);
                sqlConn.close();
                return true;
            }
            return false;

        }
    }
}