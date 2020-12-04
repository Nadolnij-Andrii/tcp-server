using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    // класс с информацией о аттракционе
    public class Attraction
    {
        public int id { get; set; }
        public object attractionIp { get; set; }
        public decimal attractionPrice { get; set; }
        public object attractionName { get; set; }
        public int attractionType { get; set; }
        public object attractionIsRental { get; set; }
        public object attractionLastPing { get; set; }
        public object attractionPusleDuration { get; set; }
        public object attractionParam1 { get; set; }
        public bool attractionDiscountSpread { get; set; }
        Logger logger = LogManager.GetCurrentClassLogger();

        public Attraction()
        {

        }
        public Attraction(
            int id,
            object attractionIp,
            decimal attractionPrice,
            object attractionName,
            int attractionType,
            object attractionIsRental,
            object attractionLastPing,
            object attractionPusleDuration,
            object attractionParam1,
            bool attractionDiscountSpread)
        {
            this.id = id;
            this.attractionIp = attractionIp;
            this.attractionPrice = attractionPrice;
            this.attractionName = attractionName;
            this.attractionType = attractionType;
            this.attractionIsRental = attractionIsRental;
            this.attractionLastPing = attractionLastPing;
            this.attractionPusleDuration = attractionPusleDuration;
            this.attractionParam1 = attractionParam1;
            this.attractionDiscountSpread = attractionDiscountSpread;
        }
        public void saveChanges()
        {
            try
            {
                SqlConn conn = new SqlConn();
                List<Pair> parameters = new List<Pair>();
                parameters.Add(new Pair { key = "ip", value = this.attractionIp});
                parameters.Add(new Pair { key = "price", value = this.attractionPrice });
                parameters.Add(new Pair { key = "name", value = this.attractionName });
                parameters.Add(new Pair { key = "type", value = this.attractionType });
                parameters.Add(new Pair { key = "isrental", value = this.attractionIsRental });
                parameters.Add(new Pair { key = "pulse_duration", value = this.attractionPusleDuration });
                parameters.Add(new Pair { key = "param1", value = this.attractionParam1 });
                parameters.Add(new Pair { key = "discount_spread", value = this.attractionDiscountSpread });
                conn.update("attractions", " id='" + this.id + "'", parameters);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Info(exc);
            }
        }
        public void addAttraction()
        {
            try
            {
                SqlConn conn = new SqlConn();
                List<Pair> parameters = new List<Pair>();
                parameters.Add(new Pair { key = "ip", value = this.attractionIp });
                parameters.Add(new Pair { key = "price", value = this.attractionPrice });
                parameters.Add(new Pair { key = "name", value = this.attractionName });
                parameters.Add(new Pair { key = "type", value = this.attractionType });
                parameters.Add(new Pair { key = "isrental", value = this.attractionIsRental });
                parameters.Add(new Pair { key = "time_last_ping", value = this.attractionLastPing });
                parameters.Add(new Pair { key = "pulse_duration", value = this.attractionPusleDuration });
                parameters.Add(new Pair { key = "param1", value = this.attractionParam1 });
                parameters.Add(new Pair { key = "discount_spread", value = this.attractionDiscountSpread });
                conn.insert("attractions",  parameters);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Info(exc);
            }
        }
    }
}
