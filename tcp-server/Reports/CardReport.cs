using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class CardReport
    {
        public List<ReportedCard> reportedCards { get; set; }
        public CardReport()
        {

        }
        public  void GetCardReport(DateTime fromDate, DateTime toDate)
        {
            SqlConn conn = new SqlConn();
            List<Card> cards = conn.selectCards("cards", "");
            
            List<TransactionAttractions> transactionsAttraction = conn.selectTransactionAttractions("transactions_attractions",
                       " date >= '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND date <= '" + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            List<TransactionCashRegister> transactionsCashRegister = conn.selectTransactionCashRegister("transactions_cashiermashine",
                       " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '" + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            this.reportedCards = new List<ReportedCard>();
            foreach (Card card in cards)
            {
                int gamesForMoney = 0, gamesForBonuses = 0, gamesTotal = 0;
                decimal spendMoney = 0, spendBonuses = 0, recievedTicketsFromAttraction = 0, receivedMoney = 0, receivedBonus = 0, recievedTicketsFromCashierRegister = 0, spendTickets = 0, writenOffBonuses = 0, writenOffMoney = 0, writenOffCardPrice = 0;
                bool cardUsed = false;
                foreach (TransactionAttractions transactionAttraction in transactionsAttraction)
                {
                    if (((int)transactionAttraction.operation) != 23)
                    {
                        if ((int)transactionAttraction.cardId == (int)card.cardId)
                        {
                            if ((decimal)transactionAttraction.summ != 0)
                            {
                                gamesForMoney++;
                                spendMoney += (decimal)transactionAttraction.summ;
                                cardUsed = true;
                            }
                            else if ((decimal)transactionAttraction.bonus != 0)
                            {
                                gamesForBonuses++;
                                spendBonuses += (decimal)transactionAttraction.bonus;
                                cardUsed = true;
                            }
                            recievedTicketsFromAttraction += (int)transactionAttraction.tickets;
                        }
                    }
                }
                gamesTotal = gamesForMoney + gamesForBonuses;
                foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
                {
                    if ((int)transactionCashRegister.cardId == (int)card.cardId)
                    {
                        if (((int)transactionCashRegister.operation >= 5
                                    && (int)transactionCashRegister.operation <= 7)
                                    || (int)transactionCashRegister.operation == 12
                                    || (int)transactionCashRegister.operation == 14
                                    || (int)transactionCashRegister.operation == 18
                                    || (int)transactionCashRegister.operation == 19)
                        {
                            receivedMoney += (decimal)transactionCashRegister.summ;
                            receivedBonus += (decimal)transactionCashRegister.bonus;
                            recievedTicketsFromCashierRegister += (int)transactionCashRegister.tickets;
                            cardUsed = true;
                        }
                        else if (((int)transactionCashRegister.operation >= 8
                                && (int)transactionCashRegister.operation <= 11)
                                || (int)transactionCashRegister.operation == 13
                                || (int)transactionCashRegister.operation == 21)
                        {
                            writenOffMoney += (decimal)transactionCashRegister.summ;
                            writenOffBonuses += (decimal)transactionCashRegister.bonus;
                            spendTickets += (int)transactionCashRegister.tickets;
                            cardUsed = true;

                        }
                        else if ((int)transactionCashRegister.operation == 20)
                        {
                            writenOffCardPrice += (decimal)transactionCashRegister.summ;
                        }
                    }
                }

                Sale cardSale = conn.selectSale("sales", "sale_id='" + card.cardSale + "'");

                CardPrice cardPrice = new CardPrice();
                cardPrice = conn.selectCardPrice("cards_price", "card_id='"+card.cardId+"'");
                if (cardUsed == true)
                {
                    CardStatus cardStatus = conn.selectCardStatus("card_state", "state_id='" + card.cardStatus + "'");
                    reportedCards.Add(
                        new ReportedCard(
                        (int)card.cardId,
                        (string)card.cardParentName,
                        cardStatus.status_message,
                        card.cardCount,
                        (decimal)card.cardBonus,
                        (int)card.cardTicket,
                        cardPrice.cardPrice,
                        cardSale.saleValue + "%",
                        (DateTime)card.cardRegDate,
                        gamesForMoney,
                        gamesForBonuses,
                        gamesTotal,
                        spendMoney,
                        spendBonuses,
                        spendTickets,
                        recievedTicketsFromAttraction,
                        recievedTicketsFromCashierRegister,
                        receivedMoney,
                        receivedBonus,
                        writenOffMoney,
                        writenOffCardPrice,
                        writenOffBonuses
                        ));
                }

            }
        }
        public void GetOneCardReport(DateTime fromDate, DateTime toDate, int cardId)
        {
            try
            {
                SqlConn conn = new SqlConn();
                Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                if (card != null && card.id > 0)
                {
                    List<TransactionAttractions> transactionsAttraction = conn.selectTransactionAttractions("transactions_attractions",
                               " date >= '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND date <= '" + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
                    List<TransactionCashRegister> transactionsCashRegister = conn.selectTransactionCashRegister("transactions_cashiermashine",
                               " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '" + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
                    this.reportedCards = new List<ReportedCard>();
                    int gamesForMoney = 0, gamesForBonuses = 0, gamesTotal = 0;
                    decimal spendMoney = 0, spendBonuses = 0, recievedTicketsFromAttraction = 0, receivedMoney = 0, receivedBonus = 0, recievedTicketsFromCashierRegister = 0, spendTickets = 0, writenOffBonuses = 0, writenOffMoney = 0, writenOffCardPrice = 0;
                    bool cardUsed = false;
                    foreach (TransactionAttractions transactionAttraction in transactionsAttraction)
                    {
                        if (((int)transactionAttraction.operation) != 23)
                        {
                            if ((int)transactionAttraction.cardId == (int)card.cardId)
                            {
                                if ((decimal)transactionAttraction.summ != 0)
                                {
                                    gamesForMoney++;
                                    spendMoney += (decimal)transactionAttraction.summ;
                                    cardUsed = true;
                                }
                                else if ((decimal)transactionAttraction.bonus != 0)
                                {
                                    gamesForBonuses++;
                                    spendBonuses += (decimal)transactionAttraction.bonus;
                                    cardUsed = true;
                                }
                                recievedTicketsFromAttraction += (int)transactionAttraction.tickets;
                            }
                        }
                    }
                    gamesTotal = gamesForMoney + gamesForBonuses;
                    foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
                    {
                        if ((int)transactionCashRegister.cardId == (int)card.cardId)
                        {
                            if (((int)transactionCashRegister.operation >= 5
                                    && (int)transactionCashRegister.operation <= 7)
                                    || (int)transactionCashRegister.operation == 12
                                    || (int)transactionCashRegister.operation == 14
                                    || (int)transactionCashRegister.operation == 18
                                    || (int)transactionCashRegister.operation == 19)
                            {
                                receivedMoney += (decimal)transactionCashRegister.summ;
                                receivedBonus += (decimal)transactionCashRegister.bonus;
                                recievedTicketsFromCashierRegister += (int)transactionCashRegister.tickets;
                                cardUsed = true;
                            }
                            else if (((int)transactionCashRegister.operation >= 8
                                    && (int)transactionCashRegister.operation <= 11)
                                    || (int)transactionCashRegister.operation == 13
                                    || (int)transactionCashRegister.operation == 21)
                            {
                                writenOffMoney += (decimal)transactionCashRegister.summ;
                                writenOffBonuses += (decimal)transactionCashRegister.bonus;
                                spendTickets += (int)transactionCashRegister.tickets;
                                cardUsed = true;

                            }
                            else if((int)transactionCashRegister.operation == 20)
                            {
                                writenOffCardPrice += (decimal)transactionCashRegister.summ;
                            }
                        }
                    }

                    Sale cardSale = conn.selectSale("sales", "sale_id='" + card.cardSale + "'");
                    CardStatus cardStatus = conn.selectCardStatus("card_state", "state_id='" + card.cardStatus + "'");
                    reportedCards.Add(
                        new ReportedCard(
                        (int)card.cardId,
                        (string)card.cardParentName,
                        cardStatus.status_message,
                        card.cardCount,
                        (decimal)card.cardBonus,
                        (int)card.cardTicket,
                        0,
                        cardSale.saleValue + "%",
                        (DateTime)card.cardRegDate,
                        gamesForMoney,
                        gamesForBonuses,
                        gamesTotal,
                        spendMoney,
                        spendBonuses,
                        spendTickets,
                        recievedTicketsFromAttraction,
                        recievedTicketsFromCashierRegister,
                        receivedMoney,
                        receivedBonus,
                        writenOffMoney,
                        writenOffCardPrice,
                        writenOffBonuses
                        ));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public class ReportedCard
        {
            public int CardId { get; set; }
            public string CardParentName { get; set; }
            public string CardStatusMessage { get; set; }
            public decimal CardCount { get; set; }
            public decimal CardBonus { get; set; }
            public int CardTicket { get; set; }
            public decimal CardPrice { get; set; }

            public string CardSale { get; set; }
            public DateTime CardRegDate { get; set; }
            public int GamesForMoney { get; set; }
            public int GamesForBonuses { get; set; }
            public int GamesTotal { get; set; }
            public decimal SpendMoney { get; set; }
            public decimal SpendBonuses { get; set; }
            public decimal SpendTickets { get; set; }
            public decimal RecievedTicketsFromAttraction { get; set; }
            public decimal RecievedTicketsFromCashierRegister { get; set; }
            public decimal ReceivedMoney { get; set; }
            public decimal ReceivedBonus { get; set; }
            public decimal WritenOffMoney { get; set; }
            public decimal WritenOffCardPrice { get; set; }
            public decimal WritenOffBonuses { get; set; }

            public ReportedCard()
            {

            }

            public ReportedCard(
              int CardId,
              string CardParentName,
              string CardStatusMessage,
              decimal CardCount,
              decimal CardBonus,
              int CardTicket,
              decimal CardPrice,
              string CardSale,
              DateTime CardRegDate,
              int GamesForMoney,
              int GamesForBonuses,
              int GamesTotal,
              decimal SpendMoney,
              decimal SpendBonuses,
              decimal SpendTickets,
              decimal RecievedTicketsFromAttraction,
              decimal RecievedTicketsFromCashierRegister,
              decimal ReceivedMoney,
              decimal ReceivedBonus,
              decimal WritenOffMoney,
              decimal WritenOffCardPrice,
              decimal WritenOffBonuses
                )
            {
                this.CardId = CardId;
                this.CardParentName = CardParentName;
                this.CardStatusMessage = CardStatusMessage;
                this.CardCount = CardCount;
                this.CardBonus = CardBonus;
                this.CardTicket = CardTicket;
                this.CardPrice = CardPrice;
                this.CardSale = CardSale;
                this.CardRegDate = CardRegDate;
                this.GamesForMoney = GamesForMoney;
                this.GamesForBonuses = GamesForBonuses;
                this.GamesTotal = GamesTotal;
                this.SpendMoney = SpendMoney;
                this.SpendBonuses = SpendBonuses;
                this.SpendTickets = SpendTickets;
                this.RecievedTicketsFromAttraction = RecievedTicketsFromAttraction;
                this.RecievedTicketsFromCashierRegister = RecievedTicketsFromCashierRegister;
                this.ReceivedMoney = ReceivedMoney;
                this.ReceivedBonus = ReceivedBonus;
                this.WritenOffMoney = WritenOffMoney;
                this.WritenOffCardPrice = WritenOffCardPrice;
                this.WritenOffBonuses = WritenOffBonuses;
            }
        }
    }
}
