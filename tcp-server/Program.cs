using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using System.Timers;
using System.Xml.Serialization;
using System.Xml;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Http;
using System.Reflection;
using System.Diagnostics;
using NLog;

namespace tcp_server
{
   
    class Program
    {
        
        static void Main(string[] args)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            try
            {
                
                int port = Int32.Parse(ConfigurationManager.AppSettings.Get("attractionPort"));
                var server = new Server(port);
                server.Listen();
                Console.ReadKey();
            }
            catch(Exception e)
            {
                Console.WriteLine("Last barricade. {0}", e.Message);
                logger.Error(e.Message, "Last barricade." );
                Console.WriteLine("Неудалось создать соединение");
                logger.Error(e.Message, "Last barricade.");
                Console.ReadKey();
                Process.Start(Assembly.GetExecutingAssembly().Location);
                Environment.Exit(0);
            }
        }

    }
}
