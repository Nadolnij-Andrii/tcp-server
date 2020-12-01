using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tcp_server
{
    class TransactionCashRegisterInfo
    {
        public Card card { get; set; }
        public List<TransactionCashRegister> transactionsCashRegister { get; set; }
        public TransactionCashRegisterInfo()
        {

        }
        public TransactionCashRegisterInfo(
            Card card,
            List<TransactionCashRegister> transactionsCashRegister
            )
        {
            this.card = card;
            this.transactionsCashRegister = transactionsCashRegister;
        }
        public void GetTransactionCashRegisterInfo(CardInfo cardInfo, int companyCode)
        {
            if (Card.cashierCheck(cardInfo.loginCard, cardInfo.ip, companyCode))
            {
                if (Card.licenseCheckResponse(cardInfo, companyCode))
                {
                    var matches = Regex.Matches(cardInfo.inputInfo, @"([0-9])+");

                    string cardId = matches[1].ToString();
                    SqlConn conn = new SqlConn();

                    this.card = conn.selectCard("cards", "card_id='" + cardId + "'");
                    if (card != null)
                    {
                        this.transactionsCashRegister = conn.selectTransactionCashRegister("transactions_cashiermashine", "card_id='" + this.card.cardId + "'" );
                    }
                }
            }
        }
    }
}
