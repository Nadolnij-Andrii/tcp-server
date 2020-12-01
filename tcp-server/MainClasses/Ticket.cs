using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class Ticket
    {
        public object id { get; set; }
        public object ticket_id { get; set; }
        public object prise { get; set; }
        public int count { get; set; }
        public int total { get; set; }

        public Ticket()
        {

        }
        public Ticket(
            object id,
            object ticket_id,
            object prise,
            int count,
            int total
            )
        {
            this.id = id;
            this.ticket_id = ticket_id;
            this.prise = prise;
            this.count = count;
            this.total = total;
        }
    }
}
