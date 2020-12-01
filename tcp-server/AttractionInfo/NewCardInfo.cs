
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tcp_server
{
    class NewCardInfo
    {
        public Admin admin { get; set; }
        public int cardId { get; set; }
        public string cardCode { get; set; }
        public NewCardInfo()
        {

        }
        public NewCardInfo(Admin admin, int cardId, string cardCode)
        {
            this.admin = admin;
            this.cardId = cardId;
            this.cardCode = cardCode;
        }
    }
}