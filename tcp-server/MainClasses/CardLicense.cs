using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tcp_server
{
    public class CardLicense
    {
        public int id { get; set; }
        public int cardId { get; set; }
        public string License { get; set; }
        public CardLicense()
        {

        }
        public CardLicense(int id, int cardId, string License)
        {
            this.id = id;
            this.cardId = cardId;
            this.License = License;
        }

    }
}