using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class StartInfo
    {
        public List<CardStatus> cardStatuses { get; set; }
        public List<Sale> sales { get; set; }
        public StartInfo()
        {

        }
        public StartInfo(
            List<CardStatus> cardStatuses,
            List<Sale> sales

            )
        {
            this.cardStatuses = cardStatuses;
            this.sales = sales;
        }
        
    }
}
