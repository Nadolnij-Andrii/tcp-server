using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class Sale
    {
        public object id { get; set; }
        public object saleId { get; set; }
        public decimal saleValue { get; set; }
        public Sale()
        {
        }
        public Sale(
            object id,
            object saleId,
            decimal saleValue)
        {
            this.id = id;
            this.saleId = saleId;
            this.saleValue = saleValue;
        }

        
    }
}
