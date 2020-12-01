using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class Cashier
    {
        public object id { get; set; }
        public object cashierId { get; set; }
        public object cashierName { get; set; }
        public object cashierCardId { get; set; }

        public Cashier()
        {

        }

        public Cashier(
            object id,
            object cashierId,
            object cashierName,
            object cashierCardId
            )
        {
            this.id = id;
            this.cashierId = cashierCardId;
            this.cashierName = cashierName;
            this.cashierCardId = cashierCardId;
        }

        public static List<Cashier> getCashiers()
        {
            SqlConn conn = new SqlConn();
            List<Cashier> cashiers = conn.selectCashiers();
            if (cashiers != null)
            {
                return cashiers;
            }
            else
            {
                return null;
            }
        }
        
    }
}
