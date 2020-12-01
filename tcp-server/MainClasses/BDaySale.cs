using System;

namespace tcp_server
{
    public class BDaySale
    {
        

        public int id { get; set; }
        public int cardId { get; set; }
        public int previousSale { get; set; }
        public DateTime date { get; set; }
        public BDaySale()
        {

        }
        public BDaySale(int id, 
            int cardId,
            int previousSale,
            DateTime date)
        {

        }
    }
}