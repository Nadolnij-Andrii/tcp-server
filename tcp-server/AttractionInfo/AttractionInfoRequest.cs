using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class AttractionInfoRequest
    {
        public List<Attraction> attractions { get; set; }
        public Admin admin { get; set; }
        public bool active { get; set; }

        public void GetAttractionsInfo(List<Attraction> attractions, Admin admin, bool active)
        {
            this.attractions = attractions;
            this.admin = admin;
            this.active = active;
        }
    }
}
