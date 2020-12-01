using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace RemoteProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost serviceHost = new ServiceHost(typeof(RemotePeerSyncService)))
            {
                serviceHost.Open();
                Console.WriteLine(String.Format("The Sync server is ready."));
                Console.ReadLine();
            }
        }
    }
}
