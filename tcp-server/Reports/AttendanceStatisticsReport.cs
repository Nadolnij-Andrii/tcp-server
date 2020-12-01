using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class AttendanceStatisticsReport
    {
        public RegistretedCards registretedCards { get; set; }
        public UsedCards usedCards { get; set; }
        public TotalIncome totalIncome { get; set; }
        public ReturnedNewCards returnedNewCards { get; set; }
        public ReturnedOldCards returnedOldCards { get; set; }
        public TotalReturn totalReturn { get; set; }
        public void GetAttendanceStatisticsReport(DateTime fromDate, DateTime toDate)
        {
            SqlConn conn = new SqlConn();
            List<TransactionCashRegister> transactionsCashRegister = conn.selectTransactionCashRegister("transactions_cashiermashine",
                      " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                      + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            List<TransactionAttractions> trasactionsAttractions = conn.selectTransactionAttractions("transactions_attractions",
                      " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                      + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            List<Card> cards = conn.selectCards("cards");
            List<Card> oldCards = new List<Card>();
            List<Card> newCards = new List<Card>();
            List<Card> returnedOldCards = new List<Card>();
            List<Card> returnedNewCards = new List<Card>();
              
            foreach (Card card in cards) 
            {
                if (transactionsCashRegister.FindAll(x => x.cardId == card.cardId).Count > 0
                    || trasactionsAttractions.FindAll(x => x.cardId == card.cardId).Count > 0)
                {
                    if ((DateTime)card.cardRegDate <= fromDate.Date)
                    {
                        oldCards.Add(card);
                    }
                    else if ((DateTime)card.cardRegDate >= fromDate.Date && (DateTime)card.cardRegDate <= ((toDate).Date.AddDays(1)))
                    {
                        newCards.Add(card);
                    }
                }
            }
            decimal addedPointsNewCards = 0, spendedPointsNewCards = 0, returnedPointsNewCards = 0, addedMoneyForNewCard = 0, returnedMoneyForNewCards = 0;
            foreach (Card newCard in newCards)
            {
                foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
                {
                    if ((int)transactionCashRegister.cardId == (int)newCard.cardId)
                    {
                        if ((int)transactionCashRegister.operation == 5 || (int)transactionCashRegister.operation == 18 || (int)transactionCashRegister.operation == 19)
                        {
                            addedPointsNewCards += (decimal)transactionCashRegister.summ;
                        }
                        if ((int)transactionCashRegister.operation == 22 )
                        {
                            addedMoneyForNewCard += (decimal)transactionCashRegister.summ;
                        }
                        if ((int)transactionCashRegister.operation == 2)
                        {
                            returnedNewCards.Add(newCard);
                            returnedPointsNewCards += (decimal)transactionCashRegister.summ;
                        }
                        if ((int)transactionCashRegister.operation == 20)
                        {
                            returnedNewCards.Add(newCard);
                            returnedMoneyForNewCards += (decimal)transactionCashRegister.summ;
                        }
                    }
                }
                foreach (TransactionAttractions trasactionAttractions in trasactionsAttractions)
                {
                    if ((int)trasactionAttractions.cardId == (int)newCard.cardId)
                    {
                        spendedPointsNewCards += (decimal)trasactionAttractions.summ;
                    }
                }
            }
            this.registretedCards = new RegistretedCards( 
                newCards.Count, 
                addedPointsNewCards+ addedMoneyForNewCard, 
                addedPointsNewCards,
                addedMoneyForNewCard,
                spendedPointsNewCards);
            decimal addedPointsOldCards = 0, spendedPointsOldCards = 0, returnedPointsOldCards = 0, addedMoneyForOldCard = 0, returnedMoneyForOldCards = 0;
            foreach (Card oldCard in oldCards)
            {

                foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
                {
                    if ((int)transactionCashRegister.cardId == (int)oldCard.cardId)
                    {
                        if ((int)transactionCashRegister.operation == 5)
                        {
                            addedPointsOldCards += (decimal)transactionCashRegister.summ;
                        }
                        if ((int)transactionCashRegister.operation == 22)
                        {
                            addedMoneyForOldCard += (decimal)transactionCashRegister.summ;
                        }
                        if ((int)transactionCashRegister.operation == 2)
                        {
                            returnedOldCards.Add(oldCard);
                            returnedPointsOldCards += (decimal)transactionCashRegister.summ;
                        }
                        if ((int)transactionCashRegister.operation == 20)
                        {
                            returnedOldCards.Add(oldCard);
                            returnedMoneyForOldCards += (decimal)transactionCashRegister.summ;
                        }
                    }
                }

                foreach (TransactionAttractions trasactionAttractions in trasactionsAttractions)
                {
                    if ((int)trasactionAttractions.cardId == (int)oldCard.cardId)
                    {
                        spendedPointsOldCards += (decimal)trasactionAttractions.summ;
                    }
                }
            }
            this.usedCards = new UsedCards(
                oldCards.Count, 
                addedPointsOldCards + addedMoneyForOldCard, 
                addedPointsOldCards,
                addedMoneyForOldCard,
                spendedPointsOldCards);
            this.totalIncome = new TotalIncome(
                oldCards.Count + newCards.Count,
                addedPointsOldCards + addedMoneyForOldCard + addedPointsNewCards + addedMoneyForNewCard, 
                addedPointsOldCards + addedPointsNewCards,
                addedMoneyForOldCard + addedMoneyForNewCard,
                spendedPointsOldCards + spendedPointsNewCards);
            addedPointsNewCards = 0; spendedPointsNewCards = 0;
            if (returnedNewCards.Count != 0)
            {
                foreach (Card returnedNewCard in returnedNewCards)
                {
                    foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
                    {
                        if ((int)transactionCashRegister.cardId == (int)returnedNewCard.cardId)
                        {
                            if ((int)transactionCashRegister.operation == 5)
                            {
                                addedPointsNewCards += (decimal)transactionCashRegister.summ;
                            }
                        }
                    }
                    foreach (TransactionAttractions trasactionAttractions in trasactionsAttractions)
                    {
                        if ((int)trasactionAttractions.cardId == (int)returnedNewCard.cardId)
                        {
                            spendedPointsNewCards += (decimal)trasactionAttractions.summ;
                        }
                    }
                }
            }
            this.returnedNewCards = new ReturnedNewCards(
                returnedNewCards.Count, 
                returnedPointsNewCards+returnedMoneyForNewCards,
                returnedPointsNewCards,
                returnedMoneyForNewCards, 
                spendedPointsNewCards);
            addedPointsOldCards = 0; spendedPointsOldCards = 0;
            if (returnedOldCards.Count != 0)
            {
                foreach (Card returnedOldCard in returnedOldCards)
                {
                    foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
                    {
                        if ((int)transactionCashRegister.cardId == (int)returnedOldCard.cardId)
                        {
                            if ((int)transactionCashRegister.operation == 5)
                            {
                                addedPointsOldCards += (decimal)transactionCashRegister.summ;
                            }
                        }
                    }
                    foreach (TransactionAttractions trasactionAttractions in trasactionsAttractions)
                    {
                        if ((int)trasactionAttractions.cardId == (int)returnedOldCard.cardId)
                        {
                            spendedPointsOldCards += (decimal)trasactionAttractions.summ;
                        }
                    }
                }
            }
            this.returnedOldCards = new ReturnedOldCards(
                returnedOldCards.Count, 
                returnedPointsOldCards+returnedMoneyForOldCards,
                returnedPointsOldCards,
                returnedMoneyForOldCards, 
                spendedPointsOldCards);
            this.totalReturn = new TotalReturn( 
                returnedOldCards.Count + returnedNewCards.Count,
                 returnedPointsOldCards + returnedMoneyForOldCards + returnedPointsNewCards + returnedMoneyForNewCards,
                returnedPointsOldCards + returnedPointsNewCards,
                returnedMoneyForOldCards + returnedMoneyForNewCards,
                spendedPointsOldCards + spendedPointsNewCards);

        }
        public class RegistretedCards
        {
            public int Count { get; set; }
            public decimal AddedMoneyNewCards { get; set; }
            public decimal AddedPointsNewCards { get; set; }
            public decimal AddedCashForNewCards { get; set; }
            public decimal SpendedPointsNewCards { get; set; }


            public RegistretedCards(
                int Count,
                decimal AddedMoneyNewCards,
                decimal AddedPointsNewCards,
                decimal AddedCashForNewCards,
                decimal SpendedPointsNewCards
                )
            {
                this.Count = Count;
                this.AddedMoneyNewCards = AddedMoneyNewCards;
                this.AddedPointsNewCards = AddedPointsNewCards;
                this.AddedCashForNewCards = AddedCashForNewCards;
                this.SpendedPointsNewCards = SpendedPointsNewCards;
            }
        }
        public class UsedCards
        {
            public int Count { get; set; }
            public decimal AddedMoneyOldCards { get; set; }
            public decimal AddedPointsOldCards { get; set; }
            public decimal AddedMoneyForOldCard { get; set; }
            public decimal SpendedPointsCards { get; set; }

            public UsedCards(
                int Count,
                decimal AddedMoneyOldCards,
                decimal AddedPointsOldCards,
                decimal AddedMoneyForOldCard,
                decimal SpendedPointsOldCards
                )
            {
                this.Count = Count;
                this.AddedMoneyOldCards = AddedMoneyOldCards;
                this.AddedPointsOldCards = AddedPointsOldCards;
                this.AddedMoneyForOldCard = AddedMoneyForOldCard;
                this.SpendedPointsCards = SpendedPointsOldCards;
            }
        }
        public class TotalIncome
        {
            public int AllCardsCount { get; set; }
            public decimal AllAddedMoney { get; set; }
            public decimal AllAddedPointsCards { get; set; }
            public decimal AllAddedMoneyForCards { get; set; }
            public decimal AllSpendedPointsCards { get; set; }

            public TotalIncome(
                int AllCardsCount,
                decimal AllAddedMoney,
                decimal AllAddedPointsCards,
                decimal AllAddedMoneyForCards,
                decimal AllSpendedPointsCards
                )
            {
                this.AllCardsCount = AllCardsCount;
                this.AllAddedMoney = AllAddedMoney;
                this.AllAddedPointsCards = AllAddedPointsCards;
                this.AllAddedMoneyForCards = AllAddedMoneyForCards;
                this.AllSpendedPointsCards = AllSpendedPointsCards;
            }
        }
        public class ReturnedNewCards
        {
            public int Count { get; set; }
            public decimal ReturnedPointsNewCards { get; set; }
            public decimal AddedPointsNewCards { get; set; }
            public decimal ReturnedMoneyForNewCards { get; set; }
            public decimal SpendedPointsNewCards { get; set; }
            public ReturnedNewCards(
                  int Count,
                  decimal ReturnedPointsNewCards,
                  decimal AddedPointsNewCards,
                  decimal ReturnedMoneyForNewCards,
                  decimal SpendedPointsNewCards
                )
            {
                this.Count = Count;
                this.ReturnedPointsNewCards = ReturnedPointsNewCards;
                this.AddedPointsNewCards = AddedPointsNewCards;
                this.ReturnedMoneyForNewCards = ReturnedMoneyForNewCards;
                this.SpendedPointsNewCards = SpendedPointsNewCards;
            }
        }
        public class ReturnedOldCards
        {
            public int Count { get; set; }
            public decimal ReturnedPointsOldCards { get; set; }
            public decimal AddedPointsOldCards { get; set; }
            public decimal ReturnedMoneyForOldCards { get; set; }
            public decimal SpendedPointsOldCards { get; set; }
            public ReturnedOldCards(
                  int Count,
                  decimal ReturnedPointsOldCards,
                  decimal AddedPointsOldCards,
                  decimal ReturnedMoneyForOldCards,
                  decimal SpendedPointsOldCards
                )
            {
                this.Count = Count;
                this.ReturnedPointsOldCards = ReturnedPointsOldCards;
                this.AddedPointsOldCards = AddedPointsOldCards;
                this.ReturnedMoneyForOldCards = ReturnedMoneyForOldCards;
                this.SpendedPointsOldCards = SpendedPointsOldCards;
            }
        }
        public class TotalReturn
        {
            public int AllCardsCount { get; set; }
            public decimal AllReturnedMoney { get; set; }
            public decimal AllAddedPointsCards { get; set; }
            public decimal AllReturnedMoneyForCards { get; set; }
            public decimal AllSpendedPointsCards { get; set; }

            public TotalReturn(
                int AllCardsCount,
                decimal AllReturnedMoney,
                decimal AllAddedPointsCards,
                decimal AllReturnedMoneyForCards,
                decimal AllSpendedPointsCards
                )
            {
                this.AllCardsCount = AllCardsCount;
                this.AllReturnedMoney = AllReturnedMoney;
                this.AllAddedPointsCards = AllAddedPointsCards;
                this.AllReturnedMoneyForCards = AllReturnedMoneyForCards;
                this.AllSpendedPointsCards = AllSpendedPointsCards;
            }
        }
    }
}
