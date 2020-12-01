using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{

    class TransactionsCardReport
    {
        public List<ReportedTransaction> reportedTransactions { get; set; }
        public void GetTransactionsCardReport(DateTime fromDate, DateTime toDate, int cardId)
        {
            SqlConn conn = new SqlConn();
            List<Card> card = conn.selectCards("cards", "card_id='" + cardId + "'");
            if (card != null && card.Count > 0)
            {
                List<TransactionCashRegister> transactionsCashRegister = conn.selectTransactionCashRegister("transactions_cashiermashine", " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                       + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
                transactionsCashRegister.Sort(
                    delegate (TransactionCashRegister transactionCashRegister1, TransactionCashRegister transactionCashRegister2)
                    {
                        return transactionCashRegister1.date.CompareTo(transactionCashRegister2.date);
                    }
                );
                List<TransactionAttractions> transactionsAttractions = conn.selectTransactionAttractions("transactions_attractions", " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                       + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
                transactionsAttractions.Sort(
                    delegate (TransactionAttractions transactionAttractions1, TransactionAttractions transactionAttractions2)
                    {
                        return transactionAttractions1.date.CompareTo(transactionAttractions2.date);
                    }
                );
                List<CardEvent> cardEvents = conn.selectCardEvents("card_event", " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                       + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
                cardEvents.Sort(
                    delegate (CardEvent cardEvent1, CardEvent cardEvent2)
                    {
                        return cardEvent1.date.CompareTo(cardEvent2.date);
                    }
                );
                this.reportedTransactions = new List<ReportedTransaction>();
                if (cardEvents.Count != 0)
                {
                    foreach (CardEvent cardEvent in cardEvents)
                    {
                        string[] comingValue = new string[2];
                        string[] outGo = new string[2];
                        if ((int)cardEvent.cardId == (int)card.Last().cardId)
                        {
                            comingValue = coming(null, (int)cardEvent.cardEvent);
                            outGo = coming(null, (int)cardEvent.cardEvent);
                            if ((int)cardEvent.cardEvent != 16)
                            {
                                this.reportedTransactions.Add(
                                    new ReportedTransaction(
                                    cardEvent.date,
                                    null,
                                    (string)cardEvent.cardMessage,
                                    "0",
                                    cardEvent.summBalance,
                                    "0",
                                    cardEvent.bonusesBalance,
                                    "0",
                                    cardEvent.ticketsBalance,
                                    null,
                                     comingValue[0].ToString(),
                                     comingValue[1].ToString(),
                                     outGo[0].ToString(),
                                     outGo[1].ToString()
                                    ));
                            }
                            else
                            {
                                this.reportedTransactions.Add(
                                    new ReportedTransaction(
                                    cardEvent.date,
                                    "Уровень " + cardEvent.toCardId + "%",
                                    (string)cardEvent.cardMessage,
                                    "0",
                                    cardEvent.summBalance,
                                    "0",
                                    cardEvent.bonusesBalance,
                                    "0",
                                    cardEvent.ticketsBalance,
                                    null,
                                     comingValue[0].ToString(),
                                     comingValue[1].ToString(),
                                     outGo[0].ToString(),
                                     outGo[1].ToString()
                                    ));
                            }
                        }

                    }
                }
                if (transactionsCashRegister.Count != 0)
                {
                    foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
                    {

                        if (transactionCashRegister.cardId == card.Last().cardId)
                        {
                            string[] comingValue = new string[2];
                            if ((int)transactionCashRegister.operation == 5 || (int)transactionCashRegister.operation == 10
                                ||
                                (int)transactionCashRegister.operation == 18 ||
                                    (int)transactionCashRegister.operation == 19
                                )
                            {
                                comingValue[0] = transactionCashRegister.summ.ToString();
                                comingValue[1] = "Очки";
                            }
                            else if ((int)transactionCashRegister.operation == 8 ||
                                (int)transactionCashRegister.operation == 21)
                            {
                                comingValue[0] = transactionCashRegister.bonus.ToString();
                                comingValue[1] = "Бонусы";
                            }
                            else if ((int)transactionCashRegister.operation == 9)
                            {
                                comingValue[0] = transactionCashRegister.tickets.ToString();
                                comingValue[1] = "Билетов";
                            }
                            else if ((int)transactionCashRegister.operation == 22)
                            {
                                comingValue[0] = transactionCashRegister.summ.ToString();
                                comingValue[1] = "Очки";
                                transactionCashRegister.summ = 0;
                            }
                            else if ((int)transactionCashRegister.operation == 16)
                            {
                                comingValue[0] = "1";
                                comingValue[1] = "Уровень карточки";
                            }
                            else
                            {
                                comingValue[0] = "0";
                                comingValue[1] = "-";
                            }
                            string operation = "";
                            if (((int)transactionCashRegister.operation >= 8
                                && (int)transactionCashRegister.operation <= 11)
                                || (int)transactionCashRegister.operation == 13
                                || (int)transactionCashRegister.operation == 2
                                || (int)transactionCashRegister.operation == 20
                                || (int)transactionCashRegister.operation == 21)
                            {
                                operation = "-";
                            }
                            if (((int)transactionCashRegister.operation >= 5
                                && (int)transactionCashRegister.operation <= 7)
                                || (int)transactionCashRegister.operation == 12
                                || (int)transactionCashRegister.operation == 14
                                || (int)transactionCashRegister.operation == 22
                                || (int)transactionCashRegister.operation == 18
                                || (int)transactionCashRegister.operation ==19)
                            {
                                operation = "+";
                            }
                            string[] outGo = new string[2];
                            if ((int)transactionCashRegister.operation == 2 || (int)transactionCashRegister.operation == 5 ||
                                (int)transactionCashRegister.operation == 18 ||
                                    (int)transactionCashRegister.operation == 19)
                            {
                                outGo[0] = transactionCashRegister.summ.ToString();
                                outGo[1] = "Очки";
                            }
                            else if ((int)transactionCashRegister.operation == 6 || (int)transactionCashRegister.operation == 21)
                            {
                                outGo[0] = transactionCashRegister.tickets.ToString();
                                outGo[1] = "Бонусов";
                            }
                            else if ((int)transactionCashRegister.operation == 7)
                            {
                                outGo[0] = transactionCashRegister.tickets.ToString();
                                outGo[1] = "Билетов";
                            }
                            else if ((int)transactionCashRegister.operation == 9)
                            {
                                outGo[0] = "1";
                                outGo[1] = "Услуга или приз";
                            }
                            else if ((int)transactionCashRegister.operation == 20)
                            {
                                outGo[0] = transactionCashRegister.summ.ToString();
                                outGo[1] = "Очки";
                                transactionCashRegister.summ = 0;
                            }
                            else if ((int)transactionCashRegister.operation == 10 || (int)transactionCashRegister.operation == 8)
                            {
                                outGo[0] = "1";
                                outGo[1] = "Игра";
                            }
                            else if ((int)transactionCashRegister.operation == 15)
                            {
                                outGo[0] = "1";
                                outGo[1] = "Карточка";
                            }
                            else if ((int)transactionCashRegister.operation == 16)
                            {
                                outGo[0] = "1";
                                outGo[1] = "Уровень карточки";
                            }
                            else
                            {
                                outGo[0] = "0";
                                outGo[1] = "-";
                            }
                            this.reportedTransactions.Add(
                                    new ReportedTransaction(
                                    transactionCashRegister.date,
                                    null,
                                    conn.selectCardStatus("card_state", "state_id='" + transactionCashRegister.operation + "'").status_message,
                                    operation + transactionCashRegister.summ.ToString(),
                                    transactionCashRegister.summBalance,
                                    operation + transactionCashRegister.bonus.ToString(),
                                    transactionCashRegister.bonusesBalance,
                                    operation + transactionCashRegister.tickets.ToString(),
                                    transactionCashRegister.ticketsBalance,
                                    null,
                                    comingValue[0].ToString(),
                                     comingValue[1].ToString(),
                                     outGo[0].ToString(),
                                     outGo[1].ToString()
                                    ));
                        }
                    }
                }
                if (transactionsAttractions.Count != 0)
                {
                    foreach (TransactionAttractions transactionAttractions in transactionsAttractions)
                    {
                        string[] comingValue = new string[2];
                        string[] outGo = new string[2];

                        if (transactionAttractions.cardId == card.Last().cardId)
                        {
                            List<Attraction> attraction = conn.selectAttractions("attractions", "id='" + transactionAttractions.attractionId + "'");
                            if ((decimal)transactionAttractions.summ != 0)
                            {

                                comingValue = coming(transactionAttractions.summ.ToString(), (int)transactionAttractions.operation);
                                outGo = coming(transactionAttractions.summ.ToString(), (int)transactionAttractions.operation);
                                this.reportedTransactions.Add(
                                    new ReportedTransaction(
                                    transactionAttractions.date,
                                    null,
                                    "Игра(очки)",
                                    "-" + transactionAttractions.summ.ToString(),
                                    transactionAttractions.summBalance,
                                    transactionAttractions.bonus.ToString(),
                                    transactionAttractions.bonusesBalance,
                                    transactionAttractions.tickets.ToString(),
                                    transactionAttractions.ticketsBalance,
                                    (string)attraction.Last().attractionName,
                                     comingValue[0].ToString(),
                                     comingValue[1].ToString(),
                                     outGo[0].ToString(),
                                     outGo[1].ToString()
                                    ));
                            }
                            else if ((decimal)transactionAttractions.bonus != 0)
                            {
                                comingValue = coming(transactionAttractions.summ.ToString(), (int)transactionAttractions.operation);
                                outGo = coming(transactionAttractions.summ.ToString(), (int)transactionAttractions.operation);
                                this.reportedTransactions.Add(
                                    new ReportedTransaction(
                                    transactionAttractions.date,
                                    null,
                                    "Игра(бонусы)",
                                    transactionAttractions.summ.ToString(),
                                    transactionAttractions.summBalance,
                                    "-" + transactionAttractions.bonus.ToString(),
                                    transactionAttractions.bonusesBalance,
                                    transactionAttractions.tickets.ToString(),
                                    transactionAttractions.ticketsBalance,
                                    "Игра - " + attraction.Last().attractionName,
                                    comingValue[0].ToString(),
                                     comingValue[1].ToString(),
                                     outGo[0].ToString(),
                                     outGo[1].ToString()
                                    ));
                            }
                        }
                    }
                }
            }
        }

        private string[] coming(string count, int operation)
        {
            string[] comingValue = new string[2];
            if (operation == 5 || operation == 10)
            {
                comingValue[0] = count.ToString();
                comingValue[1] = "Очки";
            }
            else if (operation == 8)
            {
                comingValue[0] = count.ToString();
                comingValue[1] = "Бонусы";
            }
            else if (operation == 9)
            {
                comingValue[0] = count.ToString();
                comingValue[1] = "Билетов";
            }
            else if (operation == 16)
            {
                comingValue[0] = "1";
                comingValue[1] = "Уровень карточки";
            }
            else
            {
                comingValue[0] = "0";
                comingValue[1] = "-";
            }
            return comingValue;

        }
        private string[] outGo(string count, int operation)
        {
            string[] outGo = new string[2];
            if (operation == 2 || operation == 5)
            {
                outGo[0] = count.ToString();
                outGo[1] = "Очки";
            }
            else if (operation == 7)
            {
                outGo[0] = count.ToString();
                outGo[1] = "Билетов";
            }
            else if (operation == 9)
            {
                outGo[0] = "1";
                outGo[1] = "Услуга или приз";
            }
            else if (operation == 10 || operation == 8)
            {
                outGo[0] = "1";
                outGo[1] = "Игра";
            }
            else if (operation == 15)
            {
                outGo[0] = "1";
                outGo[1] = "Карточка";
            }
            else if (operation == 16)
            {
                outGo[0] = "1";
                outGo[1] = "уровень карточки";
            }
            else
            {
                outGo[0] = "0";
                outGo[1] = "-";
            }
            return outGo;
        }

        public class ReportedTransaction
        {
            public DateTime Date { get; set; }
            public string Level { get; set; }
            public string Message { get; set; }
            public string Summ  { get; set; }
            public decimal SummBalance { get; set; }
            public string Bonus { get; set; }
            public decimal BonusBalance { get; set; }
            public string Tickets { get; set; }
            public decimal ticketsBalance { get; set; }
            public string attractionName { get; set; }
            public string comingValue0 { get; set; }
            public string comingValue1 { get; set; }
            public string outGo0 { get; set; }
            public string outGo1 { get; set; }
            
            public ReportedTransaction(
                 DateTime Date,
                 string Level,
                 string Message,
                 string Summ,
                 decimal SummBalance,
                 string Bonus,
                 decimal BonusBalance,
                 string Tickets,
                 decimal ticketsBalance,
                 string attractionName,
                 string comingValue0,
                 string comingValue1,
                 string outGo0,
                 string outGo1
                )
            {
                this.Date = Date;
                this.Level = Level;
                this.Message = Message;
                this.Summ = Summ;
                this.SummBalance = SummBalance;
                this.Bonus = Bonus;
                this.BonusBalance = BonusBalance;
                this.Tickets = Tickets;
                this.ticketsBalance = ticketsBalance;
                this.attractionName = attractionName;
                this.comingValue0 = comingValue0;
                this.comingValue1 = comingValue1;
                this.outGo0 = outGo0;
                this.outGo1 = outGo1;
            }

        }
    }
}
