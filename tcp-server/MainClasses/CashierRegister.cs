using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tcp_server
{
    public class CashierRegister
    {
        public object id { get; set; }
        public object cashierRegisterId { get; set; }
        public object cashierRegisterIP { get; set; }
        public DateTime timeLastPing { get; set; }
        public CashierRegister()
        {

        }
        public CashierRegister(
            object id,
            object cashierRegisterId,
            object cashierRegisterIP,
            DateTime timeLastPing
            )
        {
            this.id = id;
            this.cashierRegisterId = cashierRegisterId;
            this.cashierRegisterIP = cashierRegisterIP;
            this.timeLastPing = timeLastPing;
        }
        public static bool formClosing(LoginInfo loginInfo)
        {
            if(Card.cashierCheck(loginInfo.cardInfo, loginInfo.IP))
            {
                SqlConn conn = new SqlConn();
                CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + loginInfo.IP + "'");
                if (cashierRegister != null)
                {
                    MatchCollection matches = Regex.Matches(loginInfo.cardInfo, @"([0-9])+");
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
                                List<Pair> parameters = new List<Pair>();
                                parameters.Add(new Pair { key = "cashierregister_id", value = cashierRegister.cashierRegisterId });
                                parameters.Add(new Pair { key = "ip", value = loginInfo.IP });
                                parameters.Add(new Pair { key = "cashier_id", value = cashier.cashierCardId });
                                parameters.Add(new Pair { key = "cashier_name", value = cashier.cashierName });
                                parameters.Add(new Pair { key = "event", value = "exit" });
                                parameters.Add(new Pair { key = "time", value = DateTime.Now });
                                return conn.insert("cashierregister_event", parameters);
                            }
                        }
                    }
                }
            }
            return false;
        }
        public static List<CashierRegister> getCashierRegister()
        {
            SqlConn conn = new SqlConn();
            List<CashierRegister> cashierRegister = conn.selectCashierRegisters();
            if(cashierRegister != null)
            {
                return cashierRegister;
            }
            else
            {
                return null;
            }
        }
        public  bool addCashierRegister()
        {
            SqlConn conn = new SqlConn();
            IPAddress ipAddress;
            int id;
            if (IPAddress.TryParse(this.cashierRegisterIP.ToString(), out ipAddress) && 
                Int32.TryParse(this.cashierRegisterId.ToString(), out id))
            {
                if (conn.selectCashierRegister("cashierregister", "cashierregister_id='" + id+"'").cashierRegisterId == null &&
                    conn.selectCashierRegister("cashierregister", "ip='" + this.cashierRegisterIP.ToString() + "'").cashierRegisterIP == null)
                {
                    List<Pair> parameters = new List<Pair>();
                    parameters.Add(new Pair { key = "cashierregister_id", value = this.cashierRegisterId });
                    parameters.Add(new Pair { key = "ip", value = this.cashierRegisterIP.ToString() });
                    parameters.Add(new Pair { key = "time_last_ping", value = DateTime.Now });
                    var res = conn.insert("cashierregister", parameters);
                    if (res)
                    {
                        return true;
                    }
                } 
            }
            return false;
        }
        public bool changeCashierRegister()
        {
            SqlConn conn = new SqlConn();
            IPAddress ipAddress;
            if (IPAddress.TryParse(this.cashierRegisterIP.ToString(), out ipAddress))
            {
                if (conn.selectCashierRegister("cashierregister", "id='" + id + "'").id != null
                     && conn.selectCashierRegister("cashierregister", "ip='" + this.cashierRegisterIP.ToString() + "' AND cashierregister_id='" + id + "'").cashierRegisterIP == null)
                {
                    List<Pair> parameters = new List<Pair>();
                    parameters.Add(new Pair { key = "ip", value = this.cashierRegisterIP.ToString() });
                    var res = conn.update("cashierregister", "id='"+id+"'",parameters);
                    if (res)
                    {
                        return true;
                    }
                }

            }
            return false;
        }
    }
}
