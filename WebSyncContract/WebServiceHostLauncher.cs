using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;

namespace WebSyncContract
{
    public class WebServiceHostLauncher
    {
        public static void Main(String[] args)
        {
            try
            {
                SqlConn sqlConn = new SqlConn();
                ServiceHost sqlHost = new ServiceHost(typeof(SqlWebSyncService));
                sqlHost.Open();

                Console.WriteLine("Press <ENTER> to terminate the service host");
                Console.ReadLine();

                sqlHost.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in opening servicehost. " + e);
                Console.ReadLine();
            }
        }
    }
}
