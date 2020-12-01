using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class AttractionsInfo
    {
        public List<Attraction> Attractions { get; set; }
        public List<AttractionType> AttractionTypes { get; set; }
      
        public void GetAttractionsInfo()
        {
            SqlConn conn = new SqlConn();
            this.Attractions = conn.selectAttractions();
            this.AttractionTypes = conn.selectAttractionTypes();
        }
    }
}
