using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tcp_server
{
    public class ClientsInfo
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public string cardInfoString { get; set; }
        public string loginInfo { get; set; }
        public string ip { get; set; }
        public string parentName { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public List<Client> clients { get; set; }
        public int numberOfClients { get; set; }
        public ClientsInfo()
        {

        }
        public ClientsInfo(
            string cardInfoString,
            string loginInfo,
            string ip,
            string parentName,
            int numberOfClients,
            string email,
            string telephone,
            List<Client> clients
            )
        {
            this.cardInfoString = cardInfoString;
            this.loginInfo = loginInfo;
            this.ip = ip;
            this.parentName = parentName;
            this.email = email;
            this.telephone = telephone;
            this.numberOfClients = numberOfClients;
            this.clients = clients;
        }
        public bool changeClientInfo(int companyCode)
        {
            try
            {
                if (Card.cashierCheck(loginInfo, ip))
                {
                    CardInfo cardInfo = new CardInfo(cardInfoString, loginInfo, ip);
                    if (Card.licenseCheckResponse(cardInfo, companyCode))
                    {
                        var matches = Regex.Matches(cardInfo.inputInfo, @"([0-9])+");

                        string cardId = matches[1].ToString();
                        SqlConn conn = new SqlConn();
                        Card card = conn.select("cards", "card_id='" + cardId + "'");
                        updateContacts(card, email, telephone, conn);
                        if (numberOfClients != 0)
                        {
                            conn.delete("client_info", "card_id='" + cardId + "'");
                            updateParentNameCard(card, parentName, conn);
                            
                            foreach (var client in clients)
                            {
                                Client.addClient(client);
                            }
                            return true;
                        }
                        else
                        {
                            conn.delete("client_info", "card_id='" + cardId + "'");
                            updateParentNameCard( card,  parentName,  conn);
                            Client.addClient(new Client { cardId = cardId, childrenName = "", childrenDate = "", parentName = parentName, adultCard = 1 });
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public static bool updateContacts(Card card, string email, string telephone, SqlConn conn)
        {
            ClientContact clientContact = conn.selectClientContact( "card_id='" + card.cardId + "'");
            if (clientContact != null && clientContact.cardId > 0)
            {
                List<Pair> parameters = new List<Pair>();
                parameters.Add(new Pair() { key = "card_id", value = card.cardId });
                parameters.Add(new Pair() { key = "email", value = email });
                parameters.Add(new Pair() { key = "telephone", value = telephone });
                conn.update("client_contacts", "card_id='" + card.cardId + "'", parameters);
                return true;
            }
            else
            {
                List<Pair> parameters = new List<Pair>();
                parameters.Add(new Pair() { key = "card_id", value = card.cardId });
                parameters.Add(new Pair() { key = "email", value = email });
                parameters.Add(new Pair() { key = "telephone", value = telephone });
                conn.insert("client_contacts", parameters);
                return true;
            }
            return false;
        }
        public bool addClientInfo(int companyCode)
        {
            try
            {
                if (Card.cashierCheck(loginInfo, ip))
                {
                    CardInfo cardInfo = new CardInfo(cardInfoString, loginInfo, ip);
                    if (Card.licenseCheckResponse(cardInfo, companyCode))
                    {
                        var matches = Regex.Matches(cardInfo.inputInfo, @"([0-9])+");

                        string cardId = matches[1].ToString();
                        SqlConn conn = new SqlConn();
                        Card card = conn.select("cards", "card_id='" + cardId + "'");
                        if (numberOfClients != 0)
                        {
                            conn.delete("client_info", "card_id='" + cardId + "'");

                            foreach (var client in clients)
                            {
                                client.cardId = cardId;
                                Client.addClient(client);
                            }
                            return true;
                        }
                        else
                        {
                            conn.delete("client_info", "card_id='" + cardId + "'");
                            Client.addClient(new Client { cardId = cardId, childrenName = "", childrenDate = "", parentName = parentName, adultCard = 1 });
                            return true;
                        }
                    }
                }
                return false;
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Info(exc.ToString());
                return false;
            }
        }
        private void updateParentNameCard(Card card, string parentName, SqlConn conn)
        {
            List<Pair> parameters = new List<Pair>();
            parameters.Add(new Pair() { key = "card_id", value = card.cardId });
            parameters.Add(new Pair() { key = "card_count", value = card.cardCount });
            parameters.Add(new Pair() { key = "card_sale", value = card.cardSale });
            parameters.Add(new Pair() { key = "card_role", value = card.cardRole });
            parameters.Add(new Pair() { key = "card_status", value = card.cardStatus });
            parameters.Add(new Pair() { key = "card_bonus", value = card.cardBonus });
            parameters.Add(new Pair() { key = "card_ticket", value = card.cardTicket });
            parameters.Add(new Pair() { key = "parent_name", value = (object)parentName });
            parameters.Add(new Pair() { key = "card_reg_date", value = card.cardRegDate });
            conn.update("cards", "card_id='" + card.cardId + "'", parameters);
        }
    }
}
