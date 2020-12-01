using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;


namespace tcp_server
{
    public class CashierMashineController : ApiController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public object replenishmentInfo { get; private set; }
        public int companyCode = getCompanyCode();
        private static int getCompanyCode()
        {
            if(Server.code == 0)
            {
                return Server.checkLicenseFile().licenseCompanyCode;
            }
            else
            {
                return Server.code;
            }
        }
        [HttpPost]
        public string GetCashierInfo(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                LoginInfo loginInfo = new LoginInfo();
                loginInfo = JsonConvert.DeserializeObject<LoginInfo>(j);
                loginInfo.GetCashierInfo(loginInfo.cardInfo, loginInfo.IP, companyCode);
                if (loginInfo.loginInfoResponce != null)
                {
                    return JsonConvert.SerializeObject(loginInfo.loginInfoResponce);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetCard(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardInfo = new CardInfo();
                cardInfo = JsonConvert.DeserializeObject<CardInfo>(j);
                string card = Card.selectCard(cardInfo, companyCode);
                return card;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string PostSwap(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                SwapCardInfo swapCardInfo = new SwapCardInfo();
                swapCardInfo = JsonConvert.DeserializeObject<SwapCardInfo>(j);
                Card card = Card.swap(
                        swapCardInfo.newCardInfo,
                        swapCardInfo.oldCardInfo,
                        swapCardInfo.loginCard,
                        swapCardInfo.ip,
                        companyCode);
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(card);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetStartInfo(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                LoginInfo logininfo = new LoginInfo();
                logininfo = JsonConvert.DeserializeObject<LoginInfo>(j);
                return JsonConvert.SerializeObject(
                    Card.getStartInfo(logininfo.cardInfo, logininfo.IP, companyCode)
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public HttpResponseMessage CheckLicense(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardInfo = new CardInfo();
                cardInfo = JsonConvert.DeserializeObject<CardInfo>(j);
                var matches = Regex.Matches(cardInfo.inputInfo, @"([0-9])+");
                if (matches.Count > 3 && Card.licenseCheck(cardInfo.inputInfo))
                {
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string RegisterateNewCard(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                RegistretedCardInfo registretedCardInfo = new RegistretedCardInfo();
                registretedCardInfo = JsonConvert.DeserializeObject<RegistretedCardInfo>(j);
                Card card = Card.register(
                        registretedCardInfo.cardInfo,
                        "",
                        registretedCardInfo.loginCard,
                        registretedCardInfo.ip,
                        registretedCardInfo.swap,
                        companyCode);
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(card);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string TransferCard(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TransferCardInfo transferCardInfo = new TransferCardInfo();
                transferCardInfo = JsonConvert.DeserializeObject<TransferCardInfo>(j);
                Card card = Card.transfer(
                        transferCardInfo.fromCardInfoString,
                        transferCardInfo.toCardInfoString,
                        transferCardInfo.loginCard,
                        transferCardInfo.ip,
                        companyCode);
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(card);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public HttpResponseMessage FormClose(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                LoginInfo loginInfo = new LoginInfo();
                loginInfo = JsonConvert.DeserializeObject<LoginInfo>(j);

                if (CashierRegister.formClosing(loginInfo, companyCode))
                {
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string Replenishment(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ReplenishmentInfo replenishmentInfo = new ReplenishmentInfo();
                replenishmentInfo = JsonConvert.DeserializeObject<ReplenishmentInfo>(j);
                Card card = new Card();
                //replenishmentInfo.cardInfo = "111?274?2?37179121\0\0\0\0\0\0\0\0\0\0\0";
                decimal count = card.cardCount;
                card = Card.replenishment(
                        replenishmentInfo.cardInfo,
                        Decimal.Parse(replenishmentInfo.cash.ToString()),
                        Decimal.Parse(replenishmentInfo.cashlessPayment.ToString()),
                        Decimal.Parse(replenishmentInfo.creditCard.ToString()),
                        replenishmentInfo.loginCard,
                        replenishmentInfo.ip,
                        companyCode
                    );
                if (card != null && card.id > 0 && (card.cardCount != count || (replenishmentInfo.cash + replenishmentInfo.cashlessPayment + replenishmentInfo.creditCard) == 0))
                {
                        Card.UpdateWorkShift(Decimal.Parse(replenishmentInfo.cash.ToString()),
                           Decimal.Parse(replenishmentInfo.cashlessPayment.ToString()),
                           Decimal.Parse(replenishmentInfo.creditCard.ToString()),
                           0,
                           replenishmentInfo.loginCard,
                           replenishmentInfo.ip,
                           replenishmentInfo.cardInfo,
                           companyCode);
                }
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(
                    card
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string RemoveCard(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardInfo = new CardInfo();
                cardInfo = JsonConvert.DeserializeObject<CardInfo>(j);
                Card card = Card.removeCard(
                        cardInfo.inputInfo,
                        cardInfo.loginCard,
                        cardInfo.ip,
                        companyCode
                        );
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(
                   card
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string AddBonuses(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ReplenishmentInfo replenishmentInfo = new ReplenishmentInfo();
                replenishmentInfo = JsonConvert.DeserializeObject<ReplenishmentInfo>(j);
                Card card = Card.addBonuses(
                        replenishmentInfo.cardInfo,
                        Decimal.Parse(replenishmentInfo.cash.ToString()),
                        replenishmentInfo.loginCard,
                        replenishmentInfo.ip,
                        companyCode
                        );
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(
                    card
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }

        public string AddDayBonuses(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ReplenishmentInfo replenishmentInfo = new ReplenishmentInfo();
                replenishmentInfo = JsonConvert.DeserializeObject<ReplenishmentInfo>(j);
                Card card = Card.addDayBonuses(
                        replenishmentInfo.cardInfo,
                        Decimal.Parse(replenishmentInfo.cash.ToString()),
                        replenishmentInfo.loginCard,
                        replenishmentInfo.ip,
                        companyCode
                        );
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(
                    card
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string ActivateCard(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardinfo = new CardInfo();
                cardinfo = JsonConvert.DeserializeObject<CardInfo>(j);
                Card card = Card.activateCard(
                        cardinfo.inputInfo,
                        cardinfo.loginCard,
                        cardinfo.ip,
                        companyCode
                        );
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(
                    card
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string AddTickets(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ReplenishmentInfo replenishmentInfo = new ReplenishmentInfo();
                replenishmentInfo = JsonConvert.DeserializeObject<ReplenishmentInfo>(j);
                Card card = Card.addTickets(
                        replenishmentInfo.cardInfo,
                        Decimal.ToInt32(replenishmentInfo.cash),
                        replenishmentInfo.loginCard,
                        replenishmentInfo.ip,
                        companyCode
                        );
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(
                    card
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string RemoveTickets(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ReplenishmentInfo replenishmentInfo = new ReplenishmentInfo();
                replenishmentInfo = JsonConvert.DeserializeObject<ReplenishmentInfo>(j);
                Card card = Card.removeTicket(
                        replenishmentInfo.cardInfo,
                        Decimal.ToInt32(replenishmentInfo.cash),
                        replenishmentInfo.loginCard,
                        replenishmentInfo.ip,
                        companyCode
                        );

                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(
                    card
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string CardBlock(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardInfo = new CardInfo();
                cardInfo = JsonConvert.DeserializeObject<CardInfo>(j);
                Card card = Card.block(
                        cardInfo.inputInfo,
                        cardInfo.loginCard,
                        cardInfo.ip,
                        companyCode
                        );
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(
                    card
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string SetPacket(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ReplenishmentInfo replenishmentInfo = new ReplenishmentInfo();
                replenishmentInfo = JsonConvert.DeserializeObject<ReplenishmentInfo>(j);
                Card card = Card.setPacket(
                        replenishmentInfo.cardInfo,
                        Decimal.ToInt32(replenishmentInfo.cash),
                        replenishmentInfo.loginCard,
                        replenishmentInfo.ip,
                        companyCode
                        );
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(
                    card
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetTransancionAttractionInfo(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardInfo = new CardInfo();
                cardInfo = JsonConvert.DeserializeObject<CardInfo>(j);
                TransancionAttractionInfo transancionAttractionInfo = new TransancionAttractionInfo();
                transancionAttractionInfo.GetTransancionAttractionInfo(cardInfo, companyCode);
                return JsonConvert.SerializeObject(
                    transancionAttractionInfo
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetTransancionCashierRegister(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardInfo = new CardInfo();
                cardInfo = JsonConvert.DeserializeObject<CardInfo>(j);
                TransactionCashRegisterInfo transactionCashRegisterInfo = new TransactionCashRegisterInfo();
                transactionCashRegisterInfo.GetTransactionCashRegisterInfo(cardInfo, companyCode);
                return JsonConvert.SerializeObject(
                    transactionCashRegisterInfo
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public HttpResponseMessage ChangeClientInfo(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ClientsInfo clientsInfo = new ClientsInfo();
                clientsInfo = JsonConvert.DeserializeObject<ClientsInfo>(j);
                if (clientsInfo.changeClientInfo(companyCode))
                {
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public HttpResponseMessage AddClientInfo(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ClientsInfo clientsInfo = new ClientsInfo();
                clientsInfo = JsonConvert.DeserializeObject<ClientsInfo>(j);
                if (clientsInfo.addClientInfo(companyCode))
                {
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetClients(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardInfo = new CardInfo();
                cardInfo = JsonConvert.DeserializeObject<CardInfo>(j);
                List<Client> clients = new List<Client>();
                clients = Client.getClients(cardInfo, companyCode);
                ClientsInfo clientsInfo = new ClientsInfo();
                var matches = Regex.Matches(cardInfo.inputInfo, @"([0-9])+");
                string newCardId = matches[2].ToString();
                if (matches[0].ToString() == "790")
                {
                    newCardId = matches[2].ToString();
                }
                else if (matches[0].ToString() == "111")
                {
                    newCardId = matches[1].ToString();
                }
                clientsInfo.clients = clients;
                Card card = new Card();
                SqlConn sqlConn = new SqlConn();
                card = sqlConn.select("cards", "card_id='" + newCardId + "'");
                clientsInfo.parentName = card.cardParentName;
                clientsInfo.numberOfClients = clients.Count();
                clientsInfo.email = sqlConn.selectClientContact("card_id='" + newCardId + "'").email;
                clientsInfo.telephone = sqlConn.selectClientContact("card_id='" + newCardId + "'").telephone;
                return JsonConvert.SerializeObject(clientsInfo);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public HttpResponseMessage UpdateTimeLastPing(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CashierRegisterTimeUpdateInfo cashierRegisterTimeUpdateInfo = new CashierRegisterTimeUpdateInfo();
                cashierRegisterTimeUpdateInfo = JsonConvert.DeserializeObject<CashierRegisterTimeUpdateInfo>(j);
                if (cashierRegisterTimeUpdateInfo.UpdateTimeLastPing(companyCode))
                {
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string CheckWorkShift(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                LoginInfo loginInfo = new LoginInfo();
                loginInfo = JsonConvert.DeserializeObject<LoginInfo>(j);
                loginInfo.CheckWorkShiftInfo(loginInfo.cardInfo, loginInfo.IP, companyCode);
                if (loginInfo.loginInfoResponce != null)
                {
                    return JsonConvert.SerializeObject(loginInfo.loginInfoResponce);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string OpenWorkShift(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                LoginInfo loginInfo = new LoginInfo();
                loginInfo = JsonConvert.DeserializeObject<LoginInfo>(j);
                loginInfo.OpenWorkShift(loginInfo.cardInfo, loginInfo.IP, companyCode);

                if (loginInfo.loginInfoResponce != null)
                {
                    return JsonConvert.SerializeObject(loginInfo);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string ContinueWorkShift(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                LoginInfo loginInfo = new LoginInfo();
                loginInfo = JsonConvert.DeserializeObject<LoginInfo>(j);
                loginInfo.ContinueWorkShift(loginInfo.cardInfo, loginInfo.IP, companyCode);

                if (loginInfo.loginInfoResponce != null)
                {
                    return JsonConvert.SerializeObject(loginInfo);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetXReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                LoginInfo loginInfo = new LoginInfo();
                loginInfo = JsonConvert.DeserializeObject<LoginInfo>(j);
                WorkShiftReport workShiftReport =  loginInfo.GetXReport(loginInfo.cardInfo, loginInfo.IP, companyCode);
                if (workShiftReport.workShift != null && workShiftReport.workShift.id > 0 && workShiftReport.workShiftInfos.Count > 0)
                {
                    if (workShiftReport.workShiftInfos.FindAll(x => x.workShiftId == workShiftReport.workShift.id).Count > 0)
                    {
                        return JsonConvert.SerializeObject(workShiftReport);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetZReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                LoginInfo loginInfo = new LoginInfo();
                loginInfo = JsonConvert.DeserializeObject<LoginInfo>(j);
                WorkShiftReport workShiftReport = loginInfo.GetZReport(loginInfo.cardInfo, loginInfo.IP, companyCode);
                if (workShiftReport.workShift != null && workShiftReport.workShift.id > 0 && workShiftReport.workShiftInfos.Count > 0)
                {
                    if (workShiftReport.workShiftInfos.FindAll(x => x.workShiftId == workShiftReport.workShift.id).Count > 0)
                    {
                        return JsonConvert.SerializeObject(workShiftReport);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string WithdrawaCash(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                LoginInfo loginInfo = new LoginInfo();
                loginInfo = JsonConvert.DeserializeObject<LoginInfo>(j);
                WorkShift workShift = loginInfo.WithdrawaCash(loginInfo.cardInfo, loginInfo.IP, companyCode);
                if (workShift != null && workShift.id > 0 )
                {
                        return JsonConvert.SerializeObject(workShift);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetCardPrice(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                LoginInfo loginInfo = new LoginInfo();
                loginInfo = JsonConvert.DeserializeObject<LoginInfo>(j);
                if (Card.cashierCheck(loginInfo.cardInfo, loginInfo.IP, companyCode))
                {
                    SqlConn sqlConn = new SqlConn();
                    return sqlConn.selectCardStatus("card_state", "state_id='" + 17 + "'").status_message;
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string IsCardPaid(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardInfo = new CardInfo();
                cardInfo = JsonConvert.DeserializeObject<CardInfo>(j);
                if(Card.cashierCheck(cardInfo.loginCard, cardInfo.ip, companyCode))
                {
                    SqlConn sqlConn = new SqlConn();
                    CardPrice cardPrice = new CardPrice();
                    List<Pair> pairs = new List<Pair>();
                    MatchCollection matches = Regex.Matches(cardInfo.inputInfo, @"([0-9])+");
                    if (matches.Count > 3)
                    {
                        string cardId = matches[2].ToString();
                        if (matches[0].ToString() == "790")
                        {
                            cardId = matches[2].ToString();
                        }
                        else if (matches[0].ToString() == "111")
                        {
                            cardId = matches[1].ToString();
                        }
                        cardPrice = sqlConn.selectCardPrice("cards_price", "card_id='" + cardId + "'");
                        return JsonConvert.SerializeObject(cardPrice);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string PaidForCard(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ReplenishmentInfo replenishmentInfo = new ReplenishmentInfo();
                replenishmentInfo = JsonConvert.DeserializeObject<ReplenishmentInfo>(j);
                List<Pair> pairs = new List<Pair>();
                CardPrice cardPrice = new CardPrice();
                MatchCollection matches = Regex.Matches(replenishmentInfo.cardInfo, @"([0-9])+");
                if (Card.cashierCheck(replenishmentInfo.loginCard, replenishmentInfo.ip, companyCode) && Card.licenseCheck(replenishmentInfo.cardInfo))
                {
                    SqlConn sqlConn = new SqlConn();
                    WorkShiftReport workShiftReport = new WorkShiftReport();
                    string cardId = matches[2].ToString();
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
                        Card card = sqlConn.selectCard("cards", "card_id='" + cardId + "'");
                        if (card != null && card.id > 0)
                        {
                            MatchCollection matches1 = Regex.Matches(replenishmentInfo.loginCard, @"([0-9])+");
                            if (matches1.Count > 3)
                            {
                                Cashier cashier = sqlConn.selectCashier("cashiers", "card_id='" + cardId + "'");
                                if (cashier != null)
                                {
                                    CashierRegister cashierRegister = sqlConn.selectCashierRegister("cashierregister", "ip='" + replenishmentInfo.ip + "'");
                                    if (cashierRegister.id != null)
                                    {
                                        workShiftReport.workShift = sqlConn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");
                                        if (workShiftReport.workShift != null && workShiftReport.workShift.id > 0)
                                        {
                                            workShiftReport.workShiftInfos = sqlConn.selectWorkShiftInfos("work_shifts_info", "shift_id='" + workShiftReport.workShift.id + "'");
                                            if (workShiftReport.workShiftInfos.Count > 0 && workShiftReport.workShiftInfos.FindAll(x => x.workShiftId == workShiftReport.workShift.id).Count > 0)
                                            {
                                                cardPrice = sqlConn.selectCardPrice("cards_price", "card_id='" + cardId + "'");
                                                if (cardPrice == null || cardPrice.id == 0)
                                                {
                                                    cardPrice.cardPrice = Decimal.Parse(sqlConn.selectCardStatus("card_state", "state_id='" + 17 + "'").status_message);
                                                    List<Pair> parameters = new List<Pair>();
                                                    //parameters.Add(new Pair("cash_on_hand", workShiftReport.workShift.cashOnHand + cardPrice.cardPrice));
                                                    //parameters.Add(new Pair("revenue", cardPrice.cardPrice + workShiftReport.workShift.revenue));
                                                    if (replenishmentInfo.cash > 0)
                                                    {
                                                        sqlConn.updateWorkShift("work_shifts", "id='" + workShiftReport.workShift.id + "' AND is_closed='" + false + "'",
                                                            workShiftReport.workShift, cardPrice.cardPrice, 0, 0, 0);
                                                        sqlConn.updateWorkShiftInfo("work_shifts_info", "cashier_id='" + cashier.cashierId + "' AND shift_id='" + workShiftReport.workShift.id + "'", workShiftReport.workShift, cardPrice.cardPrice, 0, 0, 0);

                                                    }
                                                    if (replenishmentInfo.creditCard > 0)
                                                    {
                                                        sqlConn.updateWorkShift("work_shifts", "id='" + workShiftReport.workShift.id + "' AND is_closed='" + false + "'",
                                                            workShiftReport.workShift, 0, 0, cardPrice.cardPrice, 0);
                                                        sqlConn.updateWorkShiftInfo("work_shifts_info", "cashier_id='" + cashier.cashierId + "'", workShiftReport.workShift, 0, 0, cardPrice.cardPrice, 0);

                                                    }
                                                    if (replenishmentInfo.cashlessPayment > 0)
                                                    {
                                                        sqlConn.updateWorkShift("work_shifts", "id='" + workShiftReport.workShift.id + "' AND is_closed='" + false + "'",
                                                            workShiftReport.workShift, 0, cardPrice.cardPrice, 0, 0);
                                                        sqlConn.updateWorkShiftInfo("work_shifts_info", "cashier_id='" + cashier.cashierId + "'", workShiftReport.workShift, 0, cardPrice.cardPrice, 0, 0);
                                                    }
                                                    cardPrice = sqlConn.selectCardPrice("cards_price", "card_id='" + cardId + "'");

                                                    pairs.Add(new Pair("card_id", cardId));
                                                    pairs.Add(new Pair("card_price", sqlConn.selectCardStatus("card_state", "state_id='" + 17 + "'").status_message));
                                                    sqlConn.insert("cards_price", pairs);
                                                    cardPrice = sqlConn.selectCardPrice("cards_price", "card_id='" + cardId + "'");
                                                    Card.addTransaction(card.cardId, 0, 22, cardPrice.cardPrice, 0, 0, replenishmentInfo.loginCard, replenishmentInfo.ip, card.cardCount, card.cardBonus, card.cardTicket, companyCode);
                                                }
                                                else
                                                {
                                                    cardPrice.cardPrice = Decimal.Parse(sqlConn.selectCardStatus("card_state", "state_id='" + 17 + "'").status_message);

                                                    List<Pair> parameters = new List<Pair>();
                                                    //parameters.Add(new Pair("cash_on_hand", workShiftReport.workShift.cashOnHand + cardPrice.cardPrice));
                                                    //parameters.Add(new Pair("revenue", cardPrice.cardPrice + workShiftReport.workShift.revenue));
                                                    if (replenishmentInfo.cash > 0)
                                                    {
                                                        sqlConn.updateWorkShift("work_shifts", "id='" + workShiftReport.workShift.id + "' AND is_closed='" + false + "'",
                                                            workShiftReport.workShift, cardPrice.cardPrice, 0, 0, 0);
                                                        sqlConn.updateWorkShiftInfo("work_shifts_info", "cashier_id='" + cashier.cashierId + "' AND shift_id='" + workShiftReport.workShift.id + "'", workShiftReport.workShift, cardPrice.cardPrice, 0, 0, 0);

                                                    }
                                                    if (replenishmentInfo.creditCard > 0)
                                                    {
                                                        sqlConn.updateWorkShift("work_shifts", "id='" + workShiftReport.workShift.id + "' AND is_closed='" + false + "'",
                                                            workShiftReport.workShift, 0, 0, cardPrice.cardPrice, 0);
                                                        sqlConn.updateWorkShiftInfo("work_shifts_info", "cashier_id='" + cashier.cashierId + "' AND shift_id='" + workShiftReport.workShift.id + "'", workShiftReport.workShift, 0, 0, cardPrice.cardPrice, 0);

                                                    }
                                                    if (replenishmentInfo.cashlessPayment > 0)
                                                    {
                                                        sqlConn.updateWorkShift("work_shifts", "id='" + workShiftReport.workShift.id + "' AND is_closed='" + false + "'",
                                                            workShiftReport.workShift, 0, cardPrice.cardPrice, 0, 0);
                                                        sqlConn.updateWorkShiftInfo("work_shifts_info", "cashier_id='" + cashier.cashierId + "' AND shift_id='" + workShiftReport.workShift.id + "'", workShiftReport.workShift, 0, cardPrice.cardPrice, 0, 0);
                                                    }
                                                    pairs = new List<Pair>();
                                                    pairs.Add(new Pair("card_price", sqlConn.selectCardStatus("card_state", "state_id='" + 17 + "'").status_message));
                                                    sqlConn.update("cards_price", "card_id='" + cardId + "'", pairs);
                                                    cardPrice = sqlConn.selectCardPrice("cards_price", "card_id='" + cardId + "'");
                                                    Card.addTransaction(card.cardId, 0, 22, cardPrice.cardPrice, 0, 0, replenishmentInfo.loginCard, replenishmentInfo.ip, card.cardCount, card.cardBonus, card.cardTicket, companyCode);
                                                }
                                                return JsonConvert.SerializeObject(cardPrice);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string ReturnCashForCard(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardInfo = new CardInfo();
                cardInfo = JsonConvert.DeserializeObject<CardInfo>(j);

                return JsonConvert.SerializeObject(
                    Card.ReturnCashForCard(
                        cardInfo.inputInfo,
                        cardInfo.loginCard,
                        cardInfo.ip,
                        companyCode
                        )
                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string ReturnBonuses(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ReplenishmentInfo replenishmentInfo = new ReplenishmentInfo();
                replenishmentInfo = JsonConvert.DeserializeObject<ReplenishmentInfo>(j);
                Card card = Card.removeBonuses(
                        replenishmentInfo.cardInfo,
                        Decimal.ToInt32(replenishmentInfo.cash),
                        replenishmentInfo.loginCard,
                        replenishmentInfo.ip,
                        companyCode
                        );
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(card

                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string ReturnDayBonuses(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ReplenishmentInfo replenishmentInfo = new ReplenishmentInfo();
                replenishmentInfo = JsonConvert.DeserializeObject<ReplenishmentInfo>(j);
                Card card = Card.removeDayBonuses(
                        replenishmentInfo.cardInfo,
                        Decimal.ToInt32(replenishmentInfo.cash),
                        replenishmentInfo.loginCard,
                        replenishmentInfo.ip,
                        companyCode
                        );
                card.TotalAccrued = Card.selectAllIncomeOnCard(card.cardId.ToString());
                card.TotalSpend = Card.selectAllSpendOnCard(card.cardId.ToString());
                card.TotalGames = Card.selectAllGames(card.cardId.ToString());
                ClientContact clientContact = Card.selectClientContact(card.cardId.ToString());
                if (clientContact != null)
                {
                    card.Telephone = clientContact.telephone;
                    card.Email = clientContact.email;
                }
                return JsonConvert.SerializeObject(card

                    );
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string ContributeCash(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ReplenishmentInfo replenishmentInfo = new ReplenishmentInfo();
                replenishmentInfo = JsonConvert.DeserializeObject<ReplenishmentInfo>(j);
                WorkShift workShift = replenishmentInfo.ContributeCash(replenishmentInfo.loginCard, replenishmentInfo.ip, companyCode, replenishmentInfo.cash);
                if (workShift != null && workShift.id > 0)
                {
                    return JsonConvert.SerializeObject(workShift);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string WithdrawaSomeCash(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                ReplenishmentInfo replenishmentInfo = new ReplenishmentInfo();
                replenishmentInfo = JsonConvert.DeserializeObject<ReplenishmentInfo>(j);
                WorkShift workShift = replenishmentInfo.WithdrawaSomeCash(replenishmentInfo.loginCard, replenishmentInfo.ip, companyCode, replenishmentInfo.cash);
                if (workShift != null && workShift.id > 0)
                {
                    return JsonConvert.SerializeObject(workShift);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetCardByNumber(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardInfo = new CardInfo();
                cardInfo = JsonConvert.DeserializeObject<CardInfo>(j);
                int number = 0;
                if(Int32.TryParse(cardInfo.inputInfo, out number) )
                {
                    return JsonConvert.SerializeObject(Card.selectCardByNumber(number, cardInfo, companyCode));
                }
                return JsonConvert.SerializeObject(new Card());
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetCardInfoByNumber(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardInfo cardInfo = new CardInfo();
                cardInfo = JsonConvert.DeserializeObject<CardInfo>(j);
                int number = 0;
                if (Int32.TryParse(cardInfo.inputInfo, out number))
                {

                    return JsonConvert.SerializeObject(Card.getCardInfoByNumber(number, cardInfo, companyCode));
                }
                return JsonConvert.SerializeObject(new Card());
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
    }
}
