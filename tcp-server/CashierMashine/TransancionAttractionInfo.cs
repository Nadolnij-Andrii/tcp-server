using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tcp_server
{
    class TransancionAttractionInfo
    {
        public Card card { get; set; }
        public List<TransactionAttractions> transactionsAttractions { get; set; }
        public List<Attraction> attractions { get; set; }
        public TransancionAttractionInfo()
        {

        }
        public TransancionAttractionInfo(
            Card card,
            List<TransactionAttractions> transactionsAttractions,
            List<Attraction> attractions
            )
        {
            this.card = card;
            this.transactionsAttractions = transactionsAttractions;
            this.attractions = attractions;
        }
        public void GetTransancionAttractionInfo(CardInfo cardInfo,int companyCode)
        {
            if(Card.cashierCheck(cardInfo.loginCard, cardInfo.ip, companyCode))
            {
                if (Card.licenseCheckResponse(cardInfo, companyCode))
                {
                    var matches = Regex.Matches(cardInfo.inputInfo, @"([0-9])+");
                    if (matches.Count > 3)
                    {
                        string cardId = matches[1].ToString();
                        SqlConn conn = new SqlConn();

                        this.card = conn.selectCard("cards", "card_id='" + cardId + "'");
                        if (card != null)
                        {
                            transactionsAttractions = conn.selectTransactionAttractions("transactions_attractions", "card_id='" + cardId + "'");
                            if (transactionsAttractions != null)
                            {
                                attractions = new List<Attraction>();
                                foreach (var transactionAttractions in transactionsAttractions)
                                {
                                    if (attractions.Count > 0 && attractions.Find(x => x.id.ToString() == transactionAttractions.attractionId.ToString()) == null)
                                    {
                                        attractions.Add(conn.selectAttraction("attractions", "id='" + transactionAttractions.attractionId + "'"));
                                    }
                                    else if (attractions.Count == 0)
                                    {
                                        attractions.Add(conn.selectAttraction("attractions", "id='" + transactionAttractions.attractionId + "'"));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
