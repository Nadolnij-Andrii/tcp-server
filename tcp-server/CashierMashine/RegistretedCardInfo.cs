using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class RegistretedCardInfo
    {
        public string cardInfo { get; set; }
        public string loginCard { get; set; }
        public string ip { get; set; }
        public bool swap { get; set; }
        public RegistretedCardInfo()
        {

        }
        public RegistretedCardInfo(
            string cardInfo,
            string loginCard,
            string ip,
            bool swap
            )
        {
            this.cardInfo = cardInfo;
            this.loginCard = loginCard;
            this.ip = ip;
            this.swap = swap;
        }
    }
}
