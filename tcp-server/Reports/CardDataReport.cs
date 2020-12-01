using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tcp_server
{
    class CardDataReport
    {

        public CardReport cardReport { get; set; }
        public List<Client> clients { get; set; }
        public CardPrice cardPrice { get; set; }
        public CardDataReport()
        {

        }
        public CardDataReport(DateTime fromDate, DateTime toDate, int cardId)
        {
            CardReport cardReport = new CardReport();
            cardReport.GetOneCardReport(fromDate, toDate, cardId);
            SqlConn conn = new  SqlConn();
            List<Client> clients = conn.selectClient("client_info", "card_id='" + cardId + "'");
            CardPrice cardPrice = conn.selectCardPrice("cards_price", "card_id='"+cardId+"'");
            conn.close();
            this.cardReport = cardReport;
            this.clients = clients;
            this.cardPrice = cardPrice;
        }
    }
}