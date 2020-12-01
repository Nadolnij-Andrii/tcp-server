using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    [Serializable]
    class TimeWindows
    {
        [JsonProperty("fromDate")]
        public DateTime fromDate { get; set; }
        [JsonProperty("toDate")]
        public DateTime toDate { get; set; }
        [JsonProperty("admin")]
        public Admin admin { get; set; }
    }
}
