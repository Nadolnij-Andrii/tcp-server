using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class SwapCardInfo
    {
        public string oldCardInfo { get; set; }
        public string newCardInfo { get; set; }
        public string loginCard { get; set; }
        public string ip { get; set; }
        public SwapCardInfo()
        {

        }
        public SwapCardInfo(
            string oldCardInfo,
            string newCardInfo,
            string loginCard,
            string ip
            )
        {
            this.oldCardInfo = oldCardInfo;
            this.newCardInfo = newCardInfo;
            this.loginCard = loginCard;
            this.ip = ip;
        }
    }
}
