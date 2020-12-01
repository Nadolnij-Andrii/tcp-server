using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tcp_server
{
    public class CashierInfo
    {
        public Cashier cashier { get; set; }
        public string cardInfoString { get; set; }
        public Admin admin { get; set; }
        public CashierInfo()
        {

        }
        public CashierInfo(
            Cashier cashier,
            string cardInfoString,
            Admin admin
            )
        {
            this.cashier = cashier;
            this.cardInfoString = cardInfoString;
            this.admin = admin;
        }
        public bool addCashier()
        {
            try
            {
                License license = Server.checkLicenseFile();
                var matches = Regex.Matches(cardInfoString, @"([0-9])+");
                string cardId = matches[1].ToString();
                if (admin.check())
                {
                    if (license.licenseCompanyCode.ToString() == matches[0].ToString())
                    {
                        if (Card.licenseCheck(cardInfoString))
                        {
                            SqlConn conn = new SqlConn();
                            List<Pair> parameters = new List<Pair>();
                            Card card = conn.selectCard("cards", "card_id='" + cardId + "'");
                            if (card == null)
                            {
                                if (conn.selectCashier("cashiers", "cashier_id='" + cashier.cashierId + "'") == null)
                                {
                                    cashier.cashierCardId = Card.registerCashierCard(cardInfoString, cashier.cashierName.ToString()).cardId;
                                    parameters.Add(new Pair { key = "cashier_id", value = cashier.cashierId });
                                    parameters.Add(new Pair { key = "FIO", value = cashier.cashierName });
                                    parameters.Add(new Pair { key = "card_id", value = cashier.cashierCardId });
                                    if (conn.insert("cashiers", parameters))
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
            catch
            {
                return false;
            }
        }
        public bool changeCashierInfo()
        {
            try
            {
                SqlConn conn = new SqlConn();
                Cashier cashierOld = conn.selectCashier("cashiers", "cashier_id='" + cashier.cashierId + "'");
                if (cashierOld != null)
                {
                    if (cardInfoString != null)
                    {
                        var matches = Regex.Matches(cardInfoString, @"([0-9])+");
                        string cardId = matches[1].ToString();
                        License license = Server.checkLicenseFile();
                        
                        if (license.licenseCompanyCode.ToString() == matches[0].ToString())
                        {
                            if (Card.licenseCheck(cardInfoString))
                            {
                                List<Pair> parameters = new List<Pair>();
                                if (conn.selectCard("cards", "card_id='" + cardId + "'") == null)
                                {

                                    cashier.cashierCardId = Card.registerCashierCard(cardInfoString, cashier.cashierName.ToString()).cardId;
                                    parameters.Add(new Pair { key = "FIO", value = cashier.cashierName });
                                    parameters.Add(new Pair { key = "card_id", value = cashier.cashierCardId });
                                    if (conn.update("cashiers", "cashier_id='" + cashier.cashierId + "'", parameters))
                                    {
                                        return true;
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        List<Pair> parameters = new List<Pair>();
                        if (conn.selectCard("cards", "card_id='" + cashier.cashierCardId + "'") != null)
                        {
                            parameters.Add(new Pair { key = "FIO", value = cashier.cashierName });
                            if (conn.update("cashiers", "cashier_id='" + cashier.cashierId + "'", parameters))
                            {
                                return true;
                            }
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

    }
}
