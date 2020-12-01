using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class SalesInfo
    {
        public int id{ get ; set;}
        public string salesName{ get ; set;}
        public List<Attraction> attractions{ get ; set;}
        public DateTime dateStart{ get ; set;}
        public DateTime dateEnd{ get ; set;}
        public DateTime timeStart{ get ; set;}
        public DateTime timeEnd{ get ; set;}
        public List<DateTime> listOfDates{ get ; set;}
        public string listOfDateOfWeek { get; set; }
        public bool active { get; set; }
        public decimal salePercetage { get; set; }

        public SalesInfo(
            int id,
            string salesName,
            List<Attraction> attractions,
            DateTime dateStart,
            DateTime dateEnd,
            DateTime timeStart,
            DateTime timeEnd,
            List<DateTime> listOfDates,
            string listOfDateOfWeek,
            bool active,
            decimal salePercetage
            )
        {
            this.id = id;
            this.salesName = salesName;
            this.attractions = attractions;
            this.dateStart = dateStart;
            this.dateEnd = dateEnd;
            this.timeEnd = timeEnd;
            this.timeStart = timeStart;
            this.listOfDates = listOfDates;
            this.listOfDateOfWeek = listOfDateOfWeek;
            this.active = active;
            this.salePercetage = salePercetage;
        }
        public SalesInfo()
        {

        }
    }
}
