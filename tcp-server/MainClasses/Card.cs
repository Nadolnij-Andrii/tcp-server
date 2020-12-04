using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tcp_server
{
    // класс с информацией о карточке
    public class Card
    {
        public int id { get; set; }
        public int cardId { get; set; }
        public decimal cardCount { get; set; }
        public int cardSale { get; set; }
        public int cardRole { get; set; }
        public int cardStatus { get; set; }
        public decimal cardBonus { get; set; }
        public int cardTicket { get; set; }
        public string cardParentName { get; set; }
        public DateTime cardRegDate { get; set; }
        public decimal cardDayBonus { get; set; }
        public DateTime cardDayBonusDateTime { get; set; }
        public decimal TotalAccrued { get; set; }
        public decimal TotalSpend { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public int TotalGames { get; set; }


        static Logger logger = LogManager.GetCurrentClassLogger();
        public Card() { }
        public Card(
            int id,
            int cardId,
            decimal cardCount,
            int cardSale,
            int cardRole,
            int cardStatus,
            decimal cardBonus,
            int cardTicket,
            string cardParentName,
            DateTime cardRegDate,
            decimal cardDayBonus,
            DateTime cardDayBonusDateTime,
            decimal TotalAccrued,
            decimal TotalSpend,
            string Telephone,
            string Email,
            int TotalGames)
        {
            this.id = id;
            this.cardId = cardId;
            this.cardCount = cardCount;
            this.cardSale = cardSale;
            this.cardRole = cardRole;
            this.cardStatus = cardStatus;
            this.cardBonus = cardBonus;
            this.cardTicket = cardTicket;
            this.cardParentName = cardParentName;
            this.cardRegDate = cardRegDate;
            this.cardDayBonus = cardDayBonus;
            this.cardDayBonusDateTime = cardDayBonusDateTime;
            this.TotalAccrued = TotalAccrued;
            this.TotalSpend = TotalSpend;
            this.Telephone = Telephone;
            this.Email = Email;
            this.TotalGames = TotalGames;
        }
        public static string selectCard(CardInfo cardInfo)
        {
            try
            {
                if (cashierCheck(cardInfo.loginCard, cardInfo.ip))
                {
                    SqlConn conn = new SqlConn();

                    var matches = Regex.Matches(cardInfo.inputInfo, @"([0-9])+");
                    var codeCardBusiness = matches[0].ToString();
                    string cardId = matches[1].ToString();
                    if (matches[0].ToString() == "790")
                    {
                        cardId = matches[2].ToString();
                    }
                    else if (matches[0].ToString() == "111")
                    {
                        cardId = matches[1].ToString();
                    }
                    if (matches[0].ToString() == codeCardBusiness.ToString())
                    {
                        if (matches.Count > 3)
                        {
                            Card inputCard = conn.select("cards", "card_id='" + cardId + "'");

                            ClientContact clientContact = selectClientContact(cardId);
                            
                            if (inputCard != null)
                            {
                                if (clientContact != null)
                                {
                                    inputCard.Telephone = clientContact.telephone;
                                    inputCard.Email = clientContact.email;
                                }
                                inputCard.TotalAccrued = selectAllIncomeOnCard(cardId);
                                inputCard.TotalSpend = Card.selectAllSpendOnCard(cardId);
                                inputCard.TotalGames = selectAllGames(cardId);
                                if (licenseCheck(cardInfo.inputInfo))
                                {
                                    return JsonConvert.SerializeObject(inputCard);
                                }
                                else
                                {
                                    return "errorbissness";
                                }
                            }
                        }
                        return null;
                    }
                    else
                    {
                        return "null";
                    }
                }
                else
                {
                    return "errorcashier";
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return null;
            }

        }
        public static Card swap(string newCardInfoString, string oldCardInfoString, string loginCardInfo, string ip)
        {
            if (cashierCheck(loginCardInfo, ip))
            {
                CardInfo newCardInfo = new CardInfo(newCardInfoString, loginCardInfo, ip);
                CardInfo oldCardInfo = new CardInfo(oldCardInfoString, loginCardInfo, ip);

                if (licenseCheck(newCardInfo.inputInfo) && licenseCheck(oldCardInfo.inputInfo))
                {
                    var matches = Regex.Matches(newCardInfoString, @"([0-9])+");
                    if (matches.Count > 3)
                    {
                        string newCardId = matches[2].ToString();
                        if (matches[0].ToString() == "790")
                        {
                            newCardId = matches[2].ToString();
                        }
                        else if (matches[0].ToString() == "111")
                        {
                            newCardId = matches[1].ToString();
                        }
                        matches = Regex.Matches(oldCardInfoString, @"([0-9])+");
                        if (matches.Count > 3)
                        {
                            string oldCardId = matches[2].ToString();
                            if (matches[0].ToString() == "790")
                            {
                                oldCardId = matches[2].ToString();
                            }
                            else if (matches[0].ToString() == "111")
                            {
                                oldCardId = matches[1].ToString();
                            }
                            SqlConn conn = new SqlConn();

                            Card oldCard = conn.selectCard("cards", "card_id='" + oldCardId + "'");
                            var parameters = new List<Pair>();
                            parameters.Add(new Pair("card_id", newCardId));
                            parameters.Add(new Pair("card_count", oldCard.cardCount));
                            parameters.Add(new Pair("card_sale", oldCard.cardSale));
                            parameters.Add(new Pair("card_role", oldCard.cardRole));
                            parameters.Add(new Pair("card_status", oldCard.cardStatus));
                            parameters.Add(new Pair("card_bonus", oldCard.cardBonus));
                            parameters.Add(new Pair("card_ticket", oldCard.cardTicket));
                            parameters.Add(new Pair("parent_name", oldCard.cardParentName));
                            parameters.Add(new Pair("card_reg_date", oldCard.cardRegDate));
                            parameters.Add(new Pair("card_day_bonus", oldCard.cardDayBonus));
                            parameters.Add(new Pair("card_day_bonus_date", oldCard.cardDayBonusDateTime));
                            var res = conn.insert("cards", parameters);
                            //Если пренос карты удался устанавливаем статус старой карты как не активна
                            if (res != false)
                            {
                                parameters = new List<Pair>();
                                parameters.Add(new Pair("card_count", 0));
                                parameters.Add(new Pair("card_sale", 0));
                                parameters.Add(new Pair("card_role", oldCard.cardRole));
                                parameters.Add(new Pair("card_status", 3));
                                parameters.Add(new Pair("card_bonus", 0));
                                parameters.Add(new Pair("card_ticket", 0));
                                parameters.Add(new Pair("card_day_bonus", 0));


                                res = conn.update("cards", "id='" + oldCard.id + "'", parameters);
                                if (res != false)
                                {
                                    upgradeCardState(oldCard.cardId, newCardId, 3, "Карта " + oldCard.cardId + " заменена на карту " + newCardId, loginCardInfo, ip, 0, 0, 0);
                                    addTransaction(
                                        oldCard.cardId,
                                        int.Parse(newCardId),
                                        13, oldCard.cardCount,
                                        (decimal)oldCard.cardBonus,
                                        oldCard.cardTicket,
                                        loginCardInfo,
                                        ip,
                                        0,
                                        0,
                                        0);
                                    //Возвращаем новую карту
                                    parameters = new List<Pair>();
                                    parameters.Add(new Pair("card_id", newCardId));
                                    parameters.Add(new Pair("parent_name", oldCard.cardParentName));
                                    conn.update("client_info", "card_id='" + oldCard.cardId + "'", parameters);

                                    parameters = new List<Pair>();
                                    parameters.Add(new Pair("card_id", oldCard.cardId));
                                    parameters.Add(new Pair("children_name", ""));
                                    parameters.Add(new Pair("children_date", ""));
                                    parameters.Add(new Pair("parent_name", oldCard.cardParentName));
                                    parameters.Add(new Pair("adult_card", 1));
                                    conn.insert("client_info", parameters);
                                    addTransaction(int.Parse(newCardId), 0, 14, (decimal)oldCard.cardCount, (decimal)oldCard.cardBonus, oldCard.cardTicket, loginCardInfo, ip, (decimal)oldCard.cardCount, (decimal)oldCard.cardBonus, (int)oldCard.cardTicket);
                                    upgradeCardState(newCardId, 0, 1, "Карта " + newCardId + " активированна", loginCardInfo, ip, (decimal)oldCard.cardCount, (decimal)oldCard.cardBonus, oldCard.cardTicket);
                                    return conn.select("cards", "card_id='" + newCardId + "'");
                                }
                            }
                            else
                            {
                                throw new Exception("Card swap error");
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Card swap error");
                }
            }
            return null;
        }
        public static bool  cashierCheck(string loginCard, string ip)
        {
            MatchCollection matches = Regex.Matches(loginCard, @"([0-9])+");
            var cardLoginId = matches[2].ToString();
            if (matches[0].ToString() == "790")
            {
                cardLoginId = matches[2].ToString();
            }
            else if (matches[0].ToString() == "111")
            {
                cardLoginId = matches[1].ToString();
            }
            if (matches.Count > 3)
            {
                if (licenseCheck(loginCard))
                {
                    SqlConn conn = new SqlConn();
                    Card currentCard = conn.select("cards", "card_id='" + cardLoginId + "'");
                    if (currentCard != null)
                    {
                        Cashier cashier = conn.selectCashier("cashiers", "card_id='" + currentCard.cardId + "'");
                        if (cashier != null)
                        {
                            CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + ip + "'");
                            if (cashierRegister != null)
                            {
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("Неверный IP адрес кассы");
                                logger.Error("Cashier mashine IP: " + ip + " - Неверный IP адрес кассы");
                                return false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверная карта кассира");
                            logger.Error("Card № " + cardLoginId + " - Неверная карта кассира");
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверная карта кассира");
                        logger.Error("Card № "+ cardLoginId+ " - Неверная карта кассира");
                        return false;
                    }
                }
            }
            return false;
        }
        public static bool licenseCheck(string cardInfoString)
        {
            var matches = Regex.Matches(cardInfoString, @"([0-9])+");
            CardLicense cardLicenseClass = new CardLicense();
            cardLicenseClass = Card.selectCardLicense(matches[1].ToString());
            if (matches.Count > 3)
            {
                if (Int32.Parse(matches[0].ToString()) == 790)
                {
                    return true;
                }
                if (Int32.Parse(matches[1].ToString()) >= 13000)
                if (Int32.Parse(matches[0].ToString()) == 111 && Int32.Parse(matches[1].ToString()) <= 10000 && Int32.Parse(matches[2].ToString()) == 1 && Int32.Parse(matches[3].ToString()) == 20181)
                {
                    return true;
                }
                else if(cardLicenseClass != null 
                    && cardLicenseClass.cardId.ToString() == matches[1].ToString()
                    && cardLicenseClass.License == matches[3].ToString())
                {
                        return true;
                }
                else
                {
                    matches = Regex.Matches(cardInfoString, @"([0-9])+");
                    int cardId = Int32.Parse(matches[1].ToString());
                    int serverNumber = Int32.Parse(matches[2].ToString());

                    string cardLicense = matches[3].ToString();
                    //License license = Server.checkLicenseFile();
                    int codeCardBusiness = 111;
                    if (codeCardBusiness.ToString() == matches[0].ToString())
                    {
                        string[] soleMass = new string[] {
                                    "?K ??\u007f(\u0006?? 7 ? (? wS\u001d?h\u0012?\u001e_I ?\u000eWS[; Q? bDH?S ???\u001a",
                                    ";\u001c?twQ ???\u001b????\u000e*6 ?? 94be861f636938ad0a06d917d1f5b1be614678a638ae8",
                                    "´GQá]8n±\u008agâr@\u0012²óæS\a¸daa47aaa2c1f0d660e89fccfc3adc70ca51fc6c1f337c",
                                    "캁倴㑍諾닪瑨쨁쭻\"&m ?? t ?? v ??? l ?\u0019?f ^ M ?\u0014982f0f08d336e1c9041"
                                 };
                        int j = 0;
                        if (cardId % 2 > 0)
                        {
                            j = 0;
                        }
                        else
                        {
                            j = 1;
                        }
                        byte[] cardCodeHash;
                        byte[] cardIdHash;
                        byte[] endHash;
                        using (var deriveBytes = new Rfc2898DeriveBytes(codeCardBusiness.ToString(), Encoding.ASCII.GetBytes(soleMass[Math.Abs(0 + j - serverNumber)] + Math.Sin(Math.Log10((int)cardId)).ToString()), 20))
                        {
                            cardCodeHash = deriveBytes.GetBytes(40);
                        }
                        using (var deriveBytes = new Rfc2898DeriveBytes((cardId + serverNumber).ToString(), Encoding.ASCII.GetBytes(Math.Sin((int)cardId).ToString() + soleMass[Math.Abs(2 + j)]), 20))
                        {
                            cardIdHash = deriveBytes.GetBytes(40);
                        }
                        using (var deriveBytes = new Rfc2898DeriveBytes(cardIdHash, cardCodeHash, 50))
                        {
                            endHash = deriveBytes.GetBytes(10);
                        }

                        string newCardLicence = "";
                        foreach (var endElement in endHash)
                        {
                            newCardLicence += endElement.ToString();
                        }
                        if (cardLicense == newCardLicence.Substring(0, 8).ToString())
                        {
                            insertCardLicense(cardId.ToString(), newCardLicence.Substring(0, 8).ToString());
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("card = " + cardId);
                            insertCardLicense(cardId.ToString(), newCardLicence.Substring(0, 8).ToString());
                            logger.Error("Карта " + cardId + " не прошла проверку на подлинность") ;
                        }
                    }
                }
            }
            return false;
        }
        public static bool licenseCheckResponse(CardInfo cardInfo)
        {
            try
            {
                SqlConn conn = new SqlConn();
                var matches = Regex.Matches(cardInfo.inputInfo, @"([0-9])+");
                string cardId = "";
                if (matches[0].ToString() == "790")
                {
                    cardId = matches[2].ToString();
                }
                else if (matches[0].ToString() == "111")
                {
                    cardId = matches[1].ToString();
                }
                if (matches.Count > 3)
                {
                    Card inputCard = conn.select("cards", "card_id='" + cardId + "'");
                    if (inputCard != null)
                    {

                        if (licenseCheck(cardInfo.inputInfo))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return false;
            }
        }
        public static Card register(
            string cardInfoString,
            string parentName,
            string loginCard,
            string ip,
            bool swap,
            int companyCode
            )
        {

            if(cashierCheck(loginCard, ip))
            {
                MatchCollection matches = Regex.Matches(cardInfoString, @"([0-9])+");
                if (matches.Count > 3)
                {
                    string cardId1 = "";
                    if (matches[0].ToString() == "790")
                    {
                        cardId1 = matches[2].ToString();
                    }
                    else if (matches[0].ToString() == "111")
                    {
                        cardId1 = matches[1].ToString();
                    }
                    int cardId = Int32.Parse(cardId1.ToString());

                    CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                    if (licenseCheck(cardInfo.inputInfo))
                    {
                        Card card = null;
                        var parameters = new List<Pair>();
                        parameters.Add(new Pair() { key = "card_id", value = cardId });
                        parameters.Add(new Pair() { key = "card_count", value = 0 });
                        parameters.Add(new Pair() { key = "card_sale", value = 0 });
                        parameters.Add(new Pair() { key = "card_role", value = 0 });
                        parameters.Add(new Pair() { key = "card_status", value = 1 });
                        parameters.Add(new Pair() { key = "card_bonus", value = 0 });
                        parameters.Add(new Pair() { key = "card_ticket", value = 0 });
                        parameters.Add(new Pair() { key = "parent_name", value = (object)parentName });
                        parameters.Add(new Pair() { key = "card_reg_date", value = DateTime.Now });
                        parameters.Add(new Pair() { key = "card_day_bonus", value = 0 });
                        parameters.Add(new Pair() { key = "card_day_bonus_date", value = new DateTime(2020,10,20) });
                        SqlConn conn = new SqlConn();
                        var res = conn.insert("cards", parameters);


                        if (res != false)
                        {
                            parameters = new List<Pair>();

                            if (swap == false)
                            {
                                upgradeCardState(cardId, 0, 15, "зарегистрированна новая карта №" + cardId.ToString(), loginCard, ip, 0, 0, 0);
                                upgradeCardState(cardId, 0, 16, "установлен пакет скидка " + 0 + "%", loginCard, ip, 0, 0, 0);
                            }
                            upgradeCardState(cardId, 0, 1, "карта " + cardId.ToString() + " активированна", loginCard, ip, 0, 0, 0);

                            card = conn.select("cards", "card_id='" + cardId.ToString() + "'");
                            conn.close();
                            return card;
                        }
                        else
                        {
                            conn.close();
                            throw new Exception("Cant create new card");
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;    
        }
        public static Card registerCashierCard(
            string cardInfoString,
            string cashierName
            )
        {
            MatchCollection matches = Regex.Matches(cardInfoString, @"([0-9])+");
            if (matches.Count > 3)
            {
                string cardId1 = "";
                if (matches[0].ToString() == "790")
                {
                    cardId1 = matches[2].ToString();
                }
                else if (matches[0].ToString() == "111")
                {
                    cardId1 = matches[1].ToString();
                }
                int cardId = Int32.Parse(cardId1.ToString());
                Card card = null;
                var parameters = new List<Pair>();
                parameters.Add(new Pair() { key = "card_id", value = cardId });
                parameters.Add(new Pair() { key = "card_count", value = 0 });
                parameters.Add(new Pair() { key = "card_sale", value = 0 });
                parameters.Add(new Pair() { key = "card_role", value = 0 });
                parameters.Add(new Pair() { key = "card_status", value = 1 });
                parameters.Add(new Pair() { key = "card_bonus", value = 0 });
                parameters.Add(new Pair() { key = "card_ticket", value = 0 });
                parameters.Add(new Pair() { key = "parent_name", value = (object)cashierName });
                parameters.Add(new Pair() { key = "card_reg_date", value = DateTime.Now });
                parameters.Add(new Pair() { key = "card_day_bonus", value = 0 });
                parameters.Add(new Pair() { key = "card_day_bonus_date", value = new DateTime(2020, 10, 20) });
                SqlConn conn = new SqlConn();
                var res = conn.insert("cards", parameters);
                if (res != false)
                {
                    card = conn.select("cards", "card_id='" + cardId.ToString() + "'");
                    conn.close();
                    return card;
                }
                else
                {
                    conn.close();
                    
                }
            }
            throw new Exception("Cant create new card");
        }
        public static Card registerAdminCard(
            string cardInfoString,
            string adminName
            )
        {
            MatchCollection matches = Regex.Matches(cardInfoString, @"([0-9])+");
            if (matches.Count > 3)
            {
                string cardId1 = "";
                if (matches[0].ToString() == "790")
                {
                    cardId1 = matches[2].ToString();
                }
                else if (matches[0].ToString() == "111")
                {
                    cardId1 = matches[1].ToString();
                }
                int cardId = Int32.Parse(cardId1.ToString());
                Card card = null;
                var parameters = new List<Pair>();
                parameters.Add(new Pair() { key = "card_id", value = cardId });
                parameters.Add(new Pair() { key = "card_count", value = 0 });
                parameters.Add(new Pair() { key = "card_sale", value = 0 });
                parameters.Add(new Pair() { key = "card_role", value = 1 });
                parameters.Add(new Pair() { key = "card_status", value = 1 });
                parameters.Add(new Pair() { key = "card_bonus", value = 0 });
                parameters.Add(new Pair() { key = "card_ticket", value = 0 });
                parameters.Add(new Pair() { key = "parent_name", value = (object)adminName });
                parameters.Add(new Pair() { key = "card_reg_date", value = DateTime.Now });
                parameters.Add(new Pair() { key = "card_day_bonus", value = 0 });
                parameters.Add(new Pair() { key = "card_day_bonus_date", value = new DateTime(2020, 10, 20) });
                SqlConn conn = new SqlConn();
                var res = conn.insert("cards", parameters);
                if (res != false)
                {
                    card = conn.select("cards", "card_id='" + cardId.ToString() + "'");
                    conn.close();
                    return card;
                }
                else
                {
                    conn.close();
                }
            }
            throw new Exception("Cant create new card");
        }
        public static bool addTransaction(
            int transactionCardId,
            int transactionToCardId,
            int operation,
            decimal summ,
            decimal bonuses,
            int tikets,
            string loginCardInfo,
            string ip,
            decimal cardSumm,
            decimal cardBonuses,
            int cardTickets)
        {
            SqlConn conn = new SqlConn();
            MatchCollection matches = Regex.Matches(loginCardInfo, @"([0-9])+");
            if (matches.Count > 3)
            {
                var cardLoginId = matches[1].ToString();
                if (matches[0].ToString() == "790")
                {
                    cardLoginId = matches[2].ToString();
                }
                else if (matches[0].ToString() == "111")
                {
                    cardLoginId = matches[1].ToString();
                }
                var parameters = new List<Pair>();
                Card currentCard = conn.select("cards", "card_id='" + cardLoginId + "'");
                if (currentCard != null)
                {
                    Cashier cashier = conn.selectCashier("cashiers", "card_id='" + currentCard.cardId + "'");
                    if (cashier != null)
                    {
                        CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + ip + "'");
                        if (cashierRegister != null)
                        {
                            parameters = new List<Pair>();
                            parameters.Add(new Pair("card_id", transactionCardId));
                            parameters.Add(new Pair("tocard_id", transactionToCardId));
                            parameters.Add(new Pair("operation", operation));
                            parameters.Add(new Pair("summ", summ));
                            parameters.Add(new Pair("bonus", bonuses));
                            parameters.Add(new Pair("tickets", tikets));
                            parameters.Add(new Pair("cashierregister_id", cashierRegister.cashierRegisterId));
                            parameters.Add(new Pair("cashier_id", cashier.cashierId));
                            parameters.Add(new Pair("cashier_name", cashier.cashierName));
                            parameters.Add(new Pair("date", DateTime.Now));
                            parameters.Add(new Pair("summ_balance", cardSumm));
                            parameters.Add(new Pair("bonuses_balance", cardBonuses));
                            parameters.Add(new Pair("tickets_balance", cardTickets));
                            var res = conn.insert("transactions_cashiermashine", parameters);
                            return res;
                        }
                    }
                }
            }
            return false;
        }
        public static void upgradeCardState(
            object cardId,
            object tocardId,
            int eventId,
            string message,
            string loginCardInfo,
            string ip,
            decimal cardSumm,
            decimal cardBonuses,
            int cardTickets
            )
        {
            SqlConn conn = new SqlConn();
            MatchCollection matches = Regex.Matches(loginCardInfo, @"([0-9])+");
            var cardLoginId = matches[1].ToString();
            if (matches[0].ToString() == "790")
            {
                cardLoginId = matches[2].ToString();
            }
            else if (matches[0].ToString() == "111")
            {
                cardLoginId = matches[1].ToString();
            }
            if (matches.Count > 3)
            { 
            Card currentCard = conn.select("cards", "card_id='" + cardLoginId + "'");
                if (currentCard != null)
                {
                    Cashier cashier = conn.selectCashier("cashiers", "card_id='" + currentCard.cardId + "'");
                    if (cashier != null)
                    {
                        CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + ip + "'");
                        if (cashierRegister != null)
                        {

                            var parameters = new List<Pair>();

                            parameters.Add(new Pair("card_id", cardId));
                            parameters.Add(new Pair("tocard_id", tocardId));
                            parameters.Add(new Pair("event", eventId));
                            parameters.Add(new Pair("message", message));
                            parameters.Add(new Pair("cashierregister_id", cashierRegister.cashierRegisterId));
                            parameters.Add(new Pair("cashierregister_ip", cashierRegister.cashierRegisterIP));
                            parameters.Add(new Pair("cashier_id", cashier.cashierId));
                            parameters.Add(new Pair("cashier_name", cashier.cashierName));
                            parameters.Add(new Pair("date", DateTime.Now));
                            parameters.Add(new Pair("summ_balance", cardSumm));
                            parameters.Add(new Pair("bonuses_balance", cardBonuses));
                            parameters.Add(new Pair("tickets_balance", cardTickets));
                            conn.insert("card_event", parameters);
                            conn.close();
                        }
                    }
                }
            }
        }
        public static void upgradeCardState(
            object cardId,
            object tocardId,
            int eventId,
            string message,
            string ip,
            decimal cardSumm,
            decimal cardBonuses,
            int cardTickets
            )
        {
            SqlConn conn = new SqlConn();
            var parameters = new List<Pair>();
            parameters.Add(new Pair("card_id", cardId));
            parameters.Add(new Pair("tocard_id", tocardId));
            parameters.Add(new Pair("event", eventId));
            parameters.Add(new Pair("message", message));
            parameters.Add(new Pair("cashierregister_id", 0));
            parameters.Add(new Pair("cashierregister_ip", ip));
            parameters.Add(new Pair("cashier_id", 0));
            parameters.Add(new Pair("cashier_name", "смена пакета осуществленна автоматически"));

            //parameters.Add(new Pair("cashier_name", "смена пакета 'День рождения' осуществленна автоматически"));
            parameters.Add(new Pair("date", DateTime.Now));
            parameters.Add(new Pair("summ_balance", cardSumm));
            parameters.Add(new Pair("bonuses_balance", cardBonuses));
            parameters.Add(new Pair("tickets_balance", cardTickets));
            conn.insert("card_event", parameters);
            conn.close();
        }
        public static Card replenishment(string cardInfoString, decimal cash, decimal cashlessPayment, decimal creditCard, string loginCard, string ip)
        {
            if(cashierCheck(loginCard,ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if(licenseCheckResponse(cardInfo))
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

                    Card card  = conn.selectCard("cards", "card_id='" + cardId + "'");
                    var balance = (decimal)card.cardCount;
                    var parameters = new List<Pair>();
                    int cardBonus = 0;
                    //if((cash < 2000 && cash >= 1500)  || (cashlessPayment < 2000 &&  cashlessPayment >= 1500 ) || (  creditCard < 2000 &&   creditCard >= 1500 ))
                    //{
                    //    cardBonus = 300;
                    //}
                    //else if(cash >= 2000 || cashlessPayment >= 2000 || creditCard >= 2000)
                    //{
                    //    cardBonus = 500;
                    //}
                    parameters.Add(new Pair("card_count", balance + cash + cashlessPayment + creditCard));
                    //parameters.Add(new Pair("card_bonus", card.cardBonus + cardBonus));
                    var res = conn.update("cards", "id='" + card.id + "'", parameters);
                    if (res != false)
                    {
                        card.cardCount = balance + cash + cashlessPayment + creditCard;
                        if( cash > 0)
                        {
                            addTransaction(card.cardId, 0, 5, cash + cashlessPayment + creditCard, cardBonus, 0, loginCard, ip, balance + cash, (decimal)card.cardBonus, (int)card.cardTicket);
                        }
                        if (cashlessPayment > 0)
                        {
                            addTransaction(card.cardId, 0, 18, cash + cashlessPayment + creditCard, cardBonus, 0, loginCard, ip, balance + cash + cashlessPayment, (decimal)card.cardBonus, (int)card.cardTicket);
                        }
                        if (creditCard > 0)
                        {
                            addTransaction(card.cardId, 0, 19, cash + cashlessPayment + creditCard, cardBonus, 0, loginCard, ip, balance + cash + cashlessPayment + creditCard, (decimal)card.cardBonus, (int)card.cardTicket);
                        }
                        return card;
                    }
                }
                
            }
            return null;
        }
        public static StartInfo getStartInfo(string loginCardInfo, string ip)
        {
            if (cashierCheck(loginCardInfo, ip))
            {
                StartInfo startInfo = new StartInfo();
                SqlConn conn = new SqlConn();
                startInfo.sales = conn.selectSales();
                startInfo.cardStatuses = conn.selectCardStatuses();
                return startInfo;
            }
            else
            {
                return null;
            }
        }
        public static Card transfer(string fromCardInfoString, string toCardInfoString, string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo fromCardInfo = new CardInfo(fromCardInfoString, loginCard, ip);
                CardInfo toCardInfo = new CardInfo(toCardInfoString, loginCard, ip);

                if (licenseCheckResponse(fromCardInfo) && licenseCheckResponse(toCardInfo))
                {
                    var matches = Regex.Matches(toCardInfoString, @"([0-9])+");
                    string toCardId = matches[1].ToString();
                    if (matches[0].ToString() == "790")
                    {
                        toCardId = matches[2].ToString();
                    }
                    else if (matches[0].ToString() == "111")
                    {
                        toCardId = matches[1].ToString();
                    }
                    matches = Regex.Matches(fromCardInfoString, @"([0-9])+");
                    string fromCardId = matches[1].ToString();
                    if (matches[0].ToString() == "790")
                    {
                        fromCardId = matches[2].ToString();
                    }
                    else if (matches[0].ToString() == "111")
                    {
                        fromCardId = matches[1].ToString();
                    }
                    SqlConn conn = new SqlConn();

                    Card toCard = conn.selectCard("cards", "card_id='" + toCardId + "'");
                    Card fromCard = conn.selectCard("cards", "card_id='" + fromCardId + "'");


                    Sale fromSale = conn.selectSale("sales", "sale_id='" + fromCard.cardSale+"'");
                    Sale toSale = conn.selectSale("sales", "sale_id='" + toCard.cardSale + "'");
                    object newSale;
                    if ((decimal)toSale.saleValue <= (decimal)fromSale.saleValue)
                    {
                        newSale = (object)fromSale.saleId;
                    }
                    else
                    {
                        newSale = (object)toSale.saleId;
                    }
                    object newCount = (object)((decimal)fromCard.cardCount + (decimal)toCard.cardCount);
                    object newBonus = (object)((decimal)fromCard.cardBonus + (decimal)toCard.cardBonus);
                    object newTicket = (object)((decimal)fromCard.cardTicket + (decimal)toCard.cardTicket);
                    var parameters = new List<Pair>();
                    parameters.Add(new Pair("card_id", toCard.cardId.ToString()));
                    parameters.Add(new Pair("card_count", newCount));
                    parameters.Add(new Pair("card_sale", newSale));
                    parameters.Add(new Pair("card_role", fromCard.cardRole));
                    parameters.Add(new Pair("card_status", 1));
                    parameters.Add(new Pair("card_bonus", newBonus));
                    parameters.Add(new Pair("card_ticket", newTicket));
                    parameters.Add(new Pair("parent_name", fromCard.cardParentName));
                    parameters.Add(new Pair("card_reg_date", fromCard.cardRegDate));
                    parameters.Add(new Pair("card_day_bonus", fromCard.cardDayBonus));
                    parameters.Add(new Pair("card_day_bonus_date", fromCard.cardDayBonusDateTime));

                    var res = conn.update("cards", "card_id='" + toCard.cardId.ToString() + "'", parameters);
                    //Если пренос карты удался устанавливаем статус старой карты как не активна
                    if (res != false)
                    {
                        parameters = new List<Pair>();
                        parameters.Add(new Pair("card_count", 0));
                        parameters.Add(new Pair("card_sale", 0));
                        parameters.Add(new Pair("card_role", fromCard.cardRole));
                        parameters.Add(new Pair("card_status", 4));
                        parameters.Add(new Pair("card_bonus", 0));
                        parameters.Add(new Pair("card_ticket", 0));
                        parameters.Add(new Pair("card_day_bonus", 0));
                        res = conn.update("cards", "id='" + fromCard.id + "'", parameters);



                        if (res != false)
                        {
                            List<Client> clientsFromCard = conn.selectClient("client_info", "card_id='" + fromCard.cardId + "'");
                            List<Client> clientsToCard = conn.selectClient("client_info", "card_id='" + toCard.cardId + "'");
                            foreach (Client client in clientsFromCard)
                            {
                                if ((bool)client.adultCard != true)
                                {
                                    parameters = new List<Pair>();
                                    parameters.Add(new Pair("card_id", toCard.cardId));
                                    parameters.Add(new Pair("parent_name", toCard.cardParentName));
                                    conn.update("client_info", "id='" + client.id + "'", parameters);
                                }
                            }
                            foreach (Client client in clientsToCard)
                            {
                                if ((bool)client.adultCard == true)
                                {
                                    conn.delete("client_info", "id='" + client.id + "'");
                                }
                            }
                            conn.delete("client_info", "card_id='" + fromCard.cardId + "'");
                            //Возвращаем новую карту


                            parameters = new List<Pair>();
                            parameters.Add(new Pair("card_id", fromCard.cardId));
                            parameters.Add(new Pair("children_name", ""));
                            parameters.Add(new Pair("children_date", ""));
                            parameters.Add(new Pair("parent_name", toCard.cardParentName));
                            parameters.Add(new Pair("adult_card", 1));
                            conn.insert("client_info", parameters);
                            addTransaction(fromCard.cardId, toCard.cardId, 11, (decimal)fromCard.cardCount, (decimal)fromCard.cardBonus, fromCard.cardTicket, loginCard, ip, 0, 0, 0);
                            addTransaction(toCard.cardId, 0, 12, (decimal)fromCard.cardCount, (decimal)fromCard.cardBonus, fromCard.cardTicket, loginCard, ip, (decimal)fromCard.cardCount, (decimal)fromCard.cardBonus, (int)fromCard.cardTicket);
                            upgradeCardState(fromCard.cardId, toCard.cardId, 4, "данные карты " + fromCard.cardId + " перенесены на карту " + toCard.cardId, loginCard, ip, 0, 0, 0);
                            return conn.select("cards", "card_id='" + toCard.cardId.ToString() + "'");
                        }
                    }
                }
            }
            return null;
        }
        public static Card removeCard(string cardInfoString, string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if (licenseCheckResponse(cardInfo))
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

                    Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    if (card != null && card.id > 0)
                    {
                        var parameters = new List<Pair>();
                        parameters.Add(new Pair("card_count", 0));
                        parameters.Add(new Pair("card_bonus", 0));
                        parameters.Add(new Pair("card_ticket", 0));
                        parameters.Add(new Pair("card_status", 2));
                        parameters.Add(new Pair("card_day_bonus", 0));
                        var res = conn.update("cards", "card_id='" + card.cardId + "'", parameters);
                        if (res != false)
                        {
                            //this.conn.delete("[dbo].[client_info]", "card_id='" + this.cardId.ToString() + "'");
                            upgradeCardState(card.cardId, 0, 2, "карта " + card.cardId + " возвращена", loginCard, ip, 0, 0, 0);
                            addTransaction(card.cardId, 0, 2, (decimal)card.cardCount, (decimal)card.cardBonus, card.cardTicket, loginCard, ip, 0, 0, 0);
                            UpdateWorkShift(0, 0, 0, card.cardCount, loginCard, ip);
                            card.cardStatus = 2;
                            card.cardCount = 0;
                            card.cardBonus = 0;
                            card.cardTicket = 0;
                            card.cardDayBonus = 0;
                            return card;
                        }
                    }
                }
            } 
            return null;
        }
        public static Card addBonuses(string cardInfoString, decimal bonuses, string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if (licenseCheckResponse(cardInfo))
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

                    Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    var balance = (decimal)card.cardBonus;
                    var parameters = new List<Pair>();
                    parameters.Add(new Pair("card_bonus", balance + bonuses));
                    var res = conn.update("cards", "id='" + card.id + "'", parameters);
                    if (res != false)
                    {
                        addTransaction(card.cardId, 0, 6, 0, bonuses, 0, loginCard, ip, (decimal)card.cardCount, balance + bonuses, (int)card.cardTicket);
                        card.cardBonus = balance + bonuses;
                        return card;
                    }

                }
            }
            return null;
        }
        public static Card addDayBonuses(string cardInfoString, decimal dayBonuses, string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if (licenseCheckResponse(cardInfo))
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

                    Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    var balance = (decimal)card.cardDayBonus;
                    var parameters = new List<Pair>();
                    parameters.Add(new Pair("card_day_bonus", balance + dayBonuses));
                    parameters.Add(new Pair("card_day_bonus_date", DateTime.Now));
                    var res = conn.update("cards", "id='" + card.id + "'", parameters);
                    if (res != false)
                    {
                        addTransaction(card.cardId, 0, 25, 0, dayBonuses, 0, loginCard, ip, (decimal)card.cardCount, balance + dayBonuses, (int)card.cardTicket);
                        card.cardDayBonus = balance + dayBonuses;
                        return card;
                    }

                }
            }
            return null;
        }
        public static Card activateCard(string cardInfoString, string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if (licenseCheckResponse(cardInfo))
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

                    Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    if ((int)card.cardStatus != 1)
                    {
                        
                        var parameters = new List<Pair>();
                        parameters.Add(new Pair("card_status", 1));
                        var res = conn.update("cards", "card_id='" + card.cardId + "'", parameters);
                        if (res != false)
                        {
                            upgradeCardState(card.cardId, 0, 1, "карта " + card.cardId + " активированна", loginCard, ip, 0, card.cardBonus, card.cardTicket);
                            card.cardStatus = 1;
                            return card;
                        }
                    }
                }
            }
            return null;
        }
        public static Card addTickets(string cardInfoString, int tickets, string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if (licenseCheckResponse(cardInfo))
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

                    Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    var balance = (int)card.cardTicket;
                    var parameters = new List<Pair>();
                    parameters.Add(new Pair("card_ticket", balance + tickets));
                    var res = conn.update("cards", "id='" + card.id + "'", parameters);
                    if (res != false)
                    {
                        addTransaction(card.cardId, 0, 7, 0, 0, tickets, loginCard, ip, (decimal)card.cardCount, (decimal)card.cardBonus, balance + tickets);
                        card.cardTicket = balance + tickets;
                        return card;
                    }
                }
            }
            return null;
        }
        public static Card removeTicket(string cardInfoString,  int tickets, string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if (licenseCheckResponse(cardInfo))
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

                    Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    var balance = card.cardTicket;
                    var parameters = new List<Pair>();
                    if (balance >= tickets)
                    {
                        var newBalance = ((balance - tickets) >= 0) ? (balance - tickets) : 0;
                        parameters.Add(new Pair("card_ticket", newBalance));
                        var res = conn.update("cards", "id='" + card.id + "'", parameters);
                        if (res != false)
                        {
                            addTransaction(card.cardId, 0, 9, 0, 0, tickets, loginCard, ip, (decimal)card.cardCount, (decimal)card.cardBonus, (int)newBalance);
                            card.cardTicket = newBalance;
                            return card;
                        }
                    }
                }
            }
            return null;
        }
        public static Card removeBonuses(string cardInfoString, int bonuses, string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if (licenseCheckResponse(cardInfo))
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

                    Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    var balance = card.cardBonus;
                    var parameters = new List<Pair>();
                    if (balance >= bonuses)
                    {
                        var newBalance = ((balance - bonuses) >= 0) ? (balance - bonuses) : 0;
                        parameters.Add(new Pair("card_bonus", newBalance));
                        var res = conn.update("cards", "id='" + card.id + "'", parameters);
                        if (res != false)
                        {
                            addTransaction(card.cardId, 0, 21, 0, bonuses, 0, loginCard, ip, (decimal)card.cardCount, (decimal)card.cardBonus, (int)newBalance);
                            card.cardBonus = newBalance;
                            return card;
                        }
                    }
                }
            }
            return null;
        }
        public static Card removeDayBonuses(string cardInfoString, int dayBonuses, string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if (licenseCheckResponse(cardInfo))
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

                    Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    var balance = card.cardDayBonus;
                    var parameters = new List<Pair>();
                    if (balance >= dayBonuses)
                    {
                        var newBalance = ((balance - dayBonuses) >= 0) ? (balance - dayBonuses) : 0;
                        parameters.Add(new Pair("card_day_bonus", newBalance));
                        var res = conn.update("cards", "id='" + card.id + "'", parameters);
                        if (res != false)
                        {
                            addTransaction(card.cardId, 0, 26, 0, dayBonuses, 0, loginCard, ip, (decimal)card.cardCount, (decimal)card.cardBonus, (int)newBalance);
                            card.cardDayBonus = newBalance;
                            return card;
                        }
                    }
                }
            }
            return null;
        }
        public static Card block(string cardInfoString, string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if (licenseCheckResponse(cardInfo))
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

                    Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    var parameters = new List<Pair>();
                    parameters.Add(new Pair("card_status", 0));
                    var res = conn.update("cards", "id='" + card.id + "'", parameters);
                    if (res != false)
                    {
                        upgradeCardState(card.cardId, 0, 0, "карта " + card.cardId + " заблокированна", loginCard, ip, (decimal)card.cardCount, (decimal)card.cardBonus, card.cardTicket);
                        card.cardStatus = 0;
                        return card;
                    }
                }
            }
            return null;
        }
        public static Card setPacket(string cardInfoString, int discount,  string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if (licenseCheckResponse(cardInfo))
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

                    Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    List<Sale> sales = new List<Sale>();
                    sales = conn.selectSales("sales", "");
                    if (sales != null && sales.Count > 0 && sales.Find(x => (int)x.saleId == discount) != null)
                    {
                        var parameters = new List<Pair>();
                        if (discount != 7)
                        {
                            parameters = new List<Pair>();
                            parameters.Add(new Pair("card_sale", discount));
                            var res = conn.update("cards", "id='" + card.id + "'", parameters);
                            if (res != false)
                            {
                               
                                upgradeCardState(card.cardId, discount, 16, "установлен пакет скидка " + sales.Find(x => (int)x.saleId == discount).saleValue + "%", loginCard, ip, (decimal)card.cardCount, (decimal)card.cardBonus, card.cardTicket);
                                card.cardSale = discount;
                                return card;
                            }
                        }
                        else
                        {
                            parameters = new List<Pair>();
                            parameters.Add(new Pair("card_sale", discount));
                            var res = conn.update("cards", "id='" + card.id + "'", parameters);
                            if (res != false)
                            {
                                BDaySale bDaySale = conn.selectBDaysSale("card_id='" + card.cardId + "'");
                                if (bDaySale == null || bDaySale.id == 0)
                                {
                                    parameters = new List<Pair>();
                                    parameters.Add(new Pair("card_id", card.cardId));
                                    parameters.Add(new Pair("previous_sale", card.cardSale));
                                    parameters.Add(new Pair("date", DateTime.Now));
                                    if (conn.insert("bday_sales", parameters))
                                    {
                                        upgradeCardState(card.cardId, discount, 16, "установлен пакет - день рождения - скидка " + sales.Find(x => (int)x.saleId == discount).saleValue + "%", loginCard, ip, (decimal)card.cardCount, (decimal)card.cardBonus, card.cardTicket);
                                        card.cardSale = discount;
                                        return card;
                                    }
                                }
                                else
                                {
                                    return card;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        public static bool UpdateWorkShift(
            decimal cash,
            decimal cashlessPayment,
            decimal creditCard,
            decimal refund,
            string loginCardInfo,
            string ip)
        {
            SqlConn conn = new SqlConn();
            MatchCollection matches = Regex.Matches(loginCardInfo, @"([0-9])+");
            if (matches.Count > 3)
            {
                var cardLoginId = matches[1].ToString();
                if (matches[0].ToString() == "790")
                {
                    cardLoginId = matches[2].ToString();
                }
                else if (matches[0].ToString() == "111")
                {
                    cardLoginId = matches[1].ToString();
                }
                var parameters = new List<Pair>();

                Card currentCard = conn.select("cards", "card_id='" + cardLoginId + "'");
                if (currentCard != null)
                {
                    Cashier cashier = conn.selectCashier("cashiers", "card_id='" + currentCard.cardId + "'");
                    if (cashier != null)
                    {
                        CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + ip + "'");
                        if (cashierRegister != null)
                        {
                            WorkShift workShift = conn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");
                            if (conn.updateWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'", workShift, cash, cashlessPayment, creditCard, refund))
                            {
                                WorkShiftInfo workShiftInfo = conn.selectWorkShiftInfo("work_shifts_info", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND cashier_id='" + cashier.cashierId + "' AND shift_id='" + workShift.id + "'");
                                if (conn.updateWorkShiftInfo("work_shifts_info", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND cashier_id='" + cashier.cashierId + "' AND shift_id='" + workShift.id + "'", workShift, cash, cashlessPayment, creditCard, refund))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public override string ToString()
        {
            return this.id + "; " + this.cardId + "; " + this.cardCount + "; " + this.cardSale + "; " + this.cardRole + "; " + this.cardStatus + "; " + this.cardBonus + "; " + this.cardTicket + "; " + this.cardParentName + "; " + this.cardRegDate;
        }
        public static CardPrice ReturnCashForCard(string cardInfoString, string loginCard, string ip)
        {
            if (cashierCheck(loginCard, ip))
            {
                CardInfo cardInfo = new CardInfo(cardInfoString, loginCard, ip);
                if (licenseCheckResponse(cardInfo))
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
                    CardPrice cardPrice = new CardPrice();

                    Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    if (card != null && card.id > 0)
                    {


                        cardPrice = conn.selectCardPrice("cards_price", "card_id='" + card.cardId + "'");
                        if (cardPrice.cardPrice > 0)
                        {
                            var parameters = new List<Pair>();
                            parameters.Add(new Pair("card_status", 2));
                            var res = conn.update("cards", "card_id='" + card.cardId + "'", parameters);
                            if (res != false)
                            {
                                //this.conn.delete("[dbo].[client_info]", "card_id='" + this.cardId.ToString() + "'");
                                upgradeCardState(card.cardId, 0, 20, "средства за карту " + card.cardId + " возвращены", loginCard, ip, 0, 0, 0);
                                addTransaction(card.cardId, 0, 20, cardPrice.cardPrice, 0, 0, loginCard, ip, 0, 0, 0);
                                UpdateWorkShift(0, 0, 0, cardPrice.cardPrice, loginCard, ip);
                                card.cardStatus = 2;
                                parameters = new List<Pair>();
                                parameters.Add(new Pair("card_price", 0));
                                if (conn.update("cards_price", "card_id='" + card.cardId + "'", parameters))
                                {
                                    return conn.selectCardPrice("cards_price", "card_id='" + card.cardId + "'");
                                }
                            }
                        }
                        else
                        {
                            return cardPrice;
                        }
                    }
                }
            }
            return null;
        }
        public static Card selectCardByNumber(int number, CardInfo cardInfo)
        {
            try
            {
                if (cashierCheck(cardInfo.loginCard, cardInfo.ip))
                {
                    SqlConn conn = new SqlConn();
                    Card inputCard = conn.select("cards", "card_id='" + number + "'"); 
                    if (inputCard != null)
                    {
                        return inputCard;
                    }   
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return null;
            }

        }
        public static CardLicense selectCardLicense(string cardId)
        {
            SqlConn conn = new SqlConn();
            CardLicense cardLicense = new CardLicense();
            cardLicense = conn.selectCardLicense("card_id='" + cardId + "'");
            conn.close();
            return cardLicense;
            
        }
        public static void insertCardLicense(string cardId, string cardLicense)
        {
            SqlConn sqlConn = new SqlConn();
            var parameters = new List<Pair>();
            parameters.Add(new Pair("card_id", cardId));
            parameters.Add(new Pair("license", cardLicense));
            sqlConn.insert("cards_license", parameters);
            sqlConn.close();
        }
        public static string getCardInfoByNumber(int number, CardInfo cardInfo, int companyCode)
        {
            try
            {
                var matches = Regex.Matches(cardInfo.loginCard, @"([0-9])+");
                int serverNumber = Int32.Parse(matches[2].ToString()); 
                if (cashierCheck(cardInfo.loginCard, cardInfo.ip))
                {
                    SqlConn conn = new SqlConn();
                    Card inputCard = conn.select("cards", "card_id='" + number + "'");
                    if (inputCard != null)
                    {
                        CardLicense cardLicense = selectCardLicense(number.ToString());
                        return ";" + 790 + "=" + 31217 + "=" + number.ToString() + "=" + 1198704 + "=" + 88 + "?";
                        //if (matches[0].ToString() == "790")
                        //{
                        //    return ";" + 790 + "=" + 31217 + "=" + number.ToString() + "=" + 1198704 + "=" + 88 + "?";
                        //}
                        //if (cardLicense != null && cardLicense.id > 0)
                        //{
                        //    return ";"+companyCode+"="+number.ToString()+"="+serverNumber+"="+ cardLicense.License +"?";
                        //}
                        //else
                        //{
                        //    string[] soleMass = new string[] {
                        //            "?K ??\u007f(\u0006?? 7 ? (? wS\u001d?h\u0012?\u001e_I ?\u000eWS[; Q? bDH?S ???\u001a",
                        //            ";\u001c?twQ ???\u001b????\u000e*6 ?? 94be861f636938ad0a06d917d1f5b1be614678a638ae8",
                        //            "´GQá]8n±\u008agâr@\u0012²óæS\a¸daa47aaa2c1f0d660e89fccfc3adc70ca51fc6c1f337c",
                        //            "캁倴㑍諾닪瑨쨁쭻\"&m ?? t ?? v ??? l ?\u0019?f ^ M ?\u0014982f0f08d336e1c9041"
                        //         };
                        //    int j = 0;
                        //    if (number % 2 > 0)
                        //    {
                        //        j = 0;
                        //    }
                        //    else
                        //    {
                        //        j = 1;
                        //    }
                        //    byte[] cardCodeHash;
                        //    byte[] cardIdHash;
                        //    byte[] endHash;
                        //    using (var deriveBytes = new Rfc2898DeriveBytes(companyCode.ToString(), Encoding.ASCII.GetBytes(soleMass[Math.Abs(0 + j - serverNumber)] + Math.Sin(Math.Log10((int)number)).ToString()), 20))
                        //    {
                        //        cardCodeHash = deriveBytes.GetBytes(40);
                        //    }
                        //    using (var deriveBytes = new Rfc2898DeriveBytes((number + serverNumber).ToString(), Encoding.ASCII.GetBytes(Math.Sin((int)number).ToString() + soleMass[Math.Abs(2 + j)]), 20))
                        //    {
                        //        cardIdHash = deriveBytes.GetBytes(40);
                        //    }
                        //    using (var deriveBytes = new Rfc2898DeriveBytes(cardIdHash, cardCodeHash, 50))
                        //    {
                        //        endHash = deriveBytes.GetBytes(10);
                        //    }

                        //    string newCardLicence = "";
                        //    foreach (var endElement in endHash)
                        //    {
                        //        newCardLicence += endElement.ToString();
                        //    }
                        //    return ";" + companyCode + "=" + number.ToString() + "=" + serverNumber + "=" + newCardLicence.Substring(0, 8).ToString() + "?";
                        //}
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return null;
            }
        }
        public static decimal selectAllIncomeOnCard(string cardId)
        {
            SqlConn sqlConn = new SqlConn();
            List<TransactionCashRegister> transactionCashRegisters = new List<TransactionCashRegister>();
            transactionCashRegisters = sqlConn.selectTransactionCashRegister("transactions_cashiermashine", "card_id=" + cardId);
            decimal AllIncome = 0;
            foreach (var transactionCashRegister in transactionCashRegisters)
            {
                if (transactionCashRegister.operation == 5 ||
                    transactionCashRegister.operation == 12 ||
                    transactionCashRegister.operation == 14 ||
                    transactionCashRegister.operation == 18 ||
                    transactionCashRegister.operation == 19
                    )
                {
                    AllIncome += transactionCashRegister.summ;
                }
                if(transactionCashRegister.operation == 11 ||
                    transactionCashRegister.operation == 13||
                    transactionCashRegister.operation == 2 ||
                    transactionCashRegister.operation == 3 )
                {
                    AllIncome -= transactionCashRegister.summ;
                }
            }
            return AllIncome;
        }
        public static decimal selectAllSpendOnCard(string cardId)
        {
            SqlConn sqlConn = new SqlConn();
            List<TransactionAttractions> transactionAttractions = new List<TransactionAttractions>();
            transactionAttractions = sqlConn.selectTransactionAttractions("transactions_attractions", "card_id='"+ cardId + "'");
            decimal AllSpend = 0;
            foreach (var transactionAttraction in transactionAttractions)
            {
                if(transactionAttraction.operation == 10)
                {
                    AllSpend += transactionAttraction.summ;
                }
            }
            return AllSpend;
        }
        public static int selectAllGames(string cardId)
        {
            SqlConn sqlConn = new SqlConn();
            List<TransactionAttractions> transactionAttractions = new List<TransactionAttractions>();
            transactionAttractions = sqlConn.selectTransactionAttractions("transactions_attractions", "card_id='" + cardId + "'");
            if(transactionAttractions != null)
            {
                return transactionAttractions.Count;
            }
            return 0;
        }
        public static ClientContact selectClientContact(string cardId)
        {
            SqlConn sqlConn = new SqlConn();
            ClientContact clientContact = sqlConn.selectClientContact("card_id='" + cardId + "'");
            sqlConn.close();
            return clientContact;
        }
    }
}
