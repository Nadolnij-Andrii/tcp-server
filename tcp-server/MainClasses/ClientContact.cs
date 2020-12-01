using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class ClientContact
    {
        public int id { get; set; }
        public int cardId { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public ClientContact()
        {

        }
        public ClientContact(int id, int cardId, string email, string telephone)
        {
            this.id = id;
            this.cardId = cardId;
            this.email = email;
            this.telephone = telephone;
        }
    }
}
