using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class WorkShiftEvent
    {
        public int id { get; set; }
        public int event_id { get; set; }
        public int event_message { get; set; }
        public WorkShiftEvent()
        {

        }
        public WorkShiftEvent(
            int id, 
            int event_id,
            int event_message)
        {
            this.id = id;
            this.event_id = event_id;
            this.event_message = event_message;
        }
    }
}
