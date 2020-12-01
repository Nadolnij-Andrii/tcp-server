using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class AttractionType
    {
        public int id { get; set; }
        public int attractionTypeId { get; set; }
        public string attractionTypeName { get; set; }

        public AttractionType()
        {

        }
        public AttractionType(
            int id,
            int attractionTypeId,
            string attractionTypeName)
        {
            this.id = id;
            this.attractionTypeId = attractionTypeId;
            this.attractionTypeName = attractionTypeName;
        }
    }
}
