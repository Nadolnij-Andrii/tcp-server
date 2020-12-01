using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class Pair
    {
        public string key { get; set; }
        public object value { get; set; }

        public Pair() { }

        public Pair(string key, object value)
        {
            this.key = key;
            this.value = value;
        }

        public override string ToString()
        {
            return "Key: " + this.key + "; Value: " + this.value.ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Pair objAsPart = obj as Pair;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return Convert.ToInt32(key);
        }
        public bool Equals(Pair other)
        {
            if (other == null) return false;
            return (this.key.Equals(other.key));
        }
    }
}
