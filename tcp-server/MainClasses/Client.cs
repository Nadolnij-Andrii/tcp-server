using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tcp_server
{
    public class Client
    {
        public object id { get; set; }
        public object cardId { get; set; }
        public object childrenName { get; set; }
        public object childrenDate { get; set; }
        public object parentName { get; set; }
        public object adultCard { get; set; }
        public Client()
        {

        }
        public Client(
            object id,
            object cardId,
            object childrenName,
            object childrenDate,
            object parentName,
            object adultCard
            )
        {
            this.id = id;
            this.cardId = cardId;
            this.childrenName = childrenName;
            this.childrenDate = childrenDate;
            this.parentName = parentName;
            this.adultCard = adultCard;
        }
        public static bool addClient(Client client)
        {
            var parameters = new List<Pair>();
            parameters.Add(new Pair() { key = "card_id", value = client.cardId });
            parameters.Add(new Pair() { key = "children_name", value = client.childrenName });
            parameters.Add(new Pair() { key = "children_date", value = client.childrenDate });
            parameters.Add(new Pair() { key = "parent_name", value = client.parentName });
            parameters.Add(new Pair() { key = "adult_card", value = client.adultCard });
            SqlConn conn = new SqlConn();
            var res = conn.insert("client_info", parameters);
            if (res != false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static List<Client> getClients(CardInfo cardInfo, int companyCode)
        {
            try
            {
                if (Card.cashierCheck(cardInfo.loginCard, cardInfo.ip, companyCode))
                {
                    if (Card.licenseCheckResponse(cardInfo, companyCode))
                    {
                        var matches = Regex.Matches(cardInfo.inputInfo, @"([0-9])+");

                        string cardId = matches[1].ToString();
                        if (matches[0].ToString() == "790")
                        {
                            cardId = matches[2].ToString();
                        }
                        else if (matches[0].ToString() == "111")
                        {
                            cardId = matches[1].ToString();
                        }
                        SqlConn conn = new SqlConn();
                        Card card = conn.select("cards", "card_id='" + cardId + "'");
                        return conn.selectClient("client_info", "card_id='" + cardId + "'");
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
