
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tcp_server
{
    public class AdminCard
    {
        public int id { get; set; }
        public int adminId { get; set; }
        public int cardId { get; set; }
        public AdminCard()
        {

        }
        public AdminCard(
            int id,
            int adminId,
            int cardId
            )
        {
            this.id = id;
            this.adminId = adminId;
            this.cardId = cardId;
        }
    }
}