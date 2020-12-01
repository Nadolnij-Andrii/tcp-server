using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class CashierRegisterTimeUpdateInfo
    {
        public DateTime timeLastPing { get; set; }
        public string loginCard { get; set; }
        public string ip { get; set; }
        public CashierRegisterTimeUpdateInfo()
        {

        }
        public CashierRegisterTimeUpdateInfo(
             DateTime timeLastPing,
             string loginCard,
             string ip
            )
        {
            this.timeLastPing = timeLastPing;
            this.loginCard = loginCard;
            this.ip = ip;
        }
        public bool UpdateTimeLastPing(int companyCode)
        {
            if (Card.cashierCheck(loginCard, ip, companyCode))
            {
                var parameters = new List<Pair>();
                parameters.Add(new Pair() { key = "time_last_ping", value = timeLastPing });
                SqlConn conn = new SqlConn();
                var res = conn.update("cashierregister", "ip='"+ip+"'", parameters);
                if(res)
                {
                    return true;
                }
            }
            return false;
       }
    }
}
