using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class CardStatus
    {
        public int id { get; set; }
        public int status_id { get; set; }
        public string status_message { get; set; }

        public CardStatus()
        {

        }
        public CardStatus(
            int id,
            int status_id,
            string status_massage
            )
        {
            this.id = id;
            this.status_id = status_id;
            this.status_message = status_message;
        }
    }
}
