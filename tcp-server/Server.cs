using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NLog;
using System.Security.Cryptography;
using System.Management;
using System.Net.NetworkInformation;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Microsoft.Owin.Hosting;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Diagnostics;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;
using Quartz.Impl.Calendar;

namespace tcp_server
{
    // сам сервер
    
    class Server
    {
        TcpListener server;
        static Timer timer;
        Timer timerSync;
        Timer timerCheckSales;
        List<SalesInfo> salesInfos;
        //Timer timerCheckAttraction;
        long interval = 30000;
        long intervalSync = 1800000;
        DateTime lastSync = new DateTime();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        int port;
        private static string APP_PATH = "http://localhost:50375";
        string backUpPath;
        string syncInfoPath;
        bool syncBool = false;
        List<string> syncStrings  = new List<string>();
        List<Attraction> attractions = new List<Attraction>();
        public Server(int port )
        {
            DateTime dateTime2 =new DateTime(2019, 8, 29);
            
            if (checkLicense())
            {
                MakeBackUp();
                // Enter the listening loop.
                var threadAPI = new Thread(() => { this.WorkWithAPI(); });
                threadAPI.Name = "threadAPI";
                threadAPI.Start();
                CheckBDaySale();
                //CheckDayBonuses(new object());
                timer = new Timer(new TimerCallback(MakeBackUpInTime), null, 0, interval);
                //timerSync = new Timer(new TimerCallback(DoSync), null, 0, intervalSync);
                timerCheckSales = new Timer(new TimerCallback(CheckSales), null, 0, 60000);
                timerSync = new Timer(new TimerCallback(CheckDayBonuses), null, 0, 120000);

                this.port = port;
                IPAddress ip;
                if (!IPAddress.TryParse(ConfigurationManager.AppSettings.Get("serverIP"), out ip))
                {
                    ip = GetLocalIPAddress();
                }
                var endPoint = new IPEndPoint(ip, this.port);
                this.server = new TcpListener(endPoint);
                Console.WriteLine("Server starting on {0}", endPoint);
                logger.Info("Server starting on: " + endPoint);
                //sale_count(5, sql.selectAttraction("attractions", "id='1010'"));
                //Card.licenseCheck(";111=104123=3=13722819?");
               // Console.WriteLine(pay(sql.selectAttraction("attractions", "id='10'"),sql.selectCard("cards", "card_id='20888'"),(790).ToString()));
            }
        }
        public static int code;
        public static License checkLicenseFile()
        {
            byte[] bytesInFile = File.ReadAllBytes("./config.txt");
            string key = DecryptStringFromBytes_Aes(bytesInFile);
            License license = JsonConvert.DeserializeObject<License>(key);
            code = 111;
            return license;
        }
        private bool checkLicense()
        {
            try
            {
                //Card.licenseCheck(";111=27972=2=17388515?");
                License licenseLocal = new License();
                // Card.cashierCheck("111=2796=2=32635310","127.0.0.1", 111);
                APP_PATH = ConfigurationManager.AppSettings.Get("globalServerURL");
                if (File.Exists("./config.txt"))
                {
                    SqlConn conn = new SqlConn();
                    licenseLocal = checkLicenseFile();
                    string licenseKeyGenerate = license(licenseLocal.licenseAPIKey);
                    if (licenseKeyGenerate == licenseKeyGenerate)
                    {
                        code = 111;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    int companyCode = 0;
                    string APIkey = "";
                    do
                    {
                        Console.WriteLine("Введите код предприятия: ");
                        if (Int32.TryParse(Console.ReadLine(), out companyCode) && companyCode > 0)
                        {
                            Console.WriteLine("Введите APIkey: ");
                            string key = Console.ReadLine();
                            Regex regex = new Regex(@"\d");
                            if (regex.IsMatch(key))
                            {
                                APIkey = key;
                                code = companyCode;
                            }
                            else
                            {
                                Console.WriteLine("Неверный APIkey.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверный код предприятия.");
                        }
                    } while (companyCode == 0 && APIkey == "");

                    HttpClient httpClient = new HttpClient();
                    string filename = "./config.txt";
                    string path = APP_PATH + "/api/License/PostLicense";
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(path);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    string result = "";
                    ServerInfo serverInfo = new ServerInfo();
                    serverInfo = GetServerInfo();
                    serverInfo.licenseCompanyCode = 111;
                    serverInfo.licenseAPIKey = APIkey;
                    serverInfo.licenseLicenseKey = license(APIkey);
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = JsonConvert.SerializeObject(serverInfo);
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                        if (result != "null")
                        {
                            string r = result.Remove(result.Length - 1);
                            var k = JsonConvert.DeserializeObject(result);
                            LicenseInfo licenseInfo = new LicenseInfo();

                            licenseInfo = JsonConvert.DeserializeObject<LicenseInfo>(k.ToString());
                            licenseInfo.licenseAPIKey = APIkey;
                            licenseInfo.licenseCompanyCode = 111;
                            byte[] license = EncryptStringToBytes_Aes(JsonConvert.SerializeObject(licenseInfo));
                            System.IO.File.WriteAllBytes(filename, license);

                            SqlConn conn = new SqlConn();
                            if (conn.selectAdmins().Count == 0)
                            {
                                Admin.addFirstAdmin();
                            }
                            conn.close();
                            return true;
                        }
                        else
                        {
                            throw new Exception("\nОшибка лицензии.\n");
                        }
                    }

                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return false;
            }
        }
        public static  byte[] EncryptStringToBytes_Aes(string plainText)
        {
            // Check arguments.
            byte[] Key = Encoding.Default.GetBytes(GetBIOSInfo());
            if(Key.Length > 32)
            {
                Key = Key.Take(32).ToArray();
            }
            
            byte[] IV = Encoding.Default.GetBytes(GetProcessorInfo());
            if (IV.Length > 16)
            {
                IV = IV.Take(16).ToArray();
            }

            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }
        public static string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            // Check arguments.
            byte[] Key = Encoding.Default.GetBytes(GetBIOSInfo());
            if(Key.Length > 32)
            {
                Key = Key.Take(32).ToArray();
            }
            byte[] IV = Encoding.Default.GetBytes(GetProcessorInfo());
            if (IV.Length > 16)
            {
                IV = IV.Take(16).ToArray();
            }
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = "";

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return plaintext;

        }
        private static string GetBIOSInfo()
        {
            string BIOS = "";
            ManagementObjectSearcher BIOSInfo = new ManagementObjectSearcher("select * from Win32_BIOS");
            foreach (ManagementObject share in BIOSInfo.Get())
            {
                foreach (PropertyData PD in share.Properties)
                {
                    try
                    {
                        if(PD.Value != null)
                        BIOS += PD.Name.ToString() + ": " + PD.Value.ToString() + " \n";
                    }
                    catch(Exception ex)
                    {
                        continue;
                    }
                }
            }
            return BIOS;
        }
        private static string GetMAC()
        {
            string macAddresses = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return macAddresses;
        }
        private static string GetProcessorInfo()
        {
            ManagementObjectSearcher processorInfo = new ManagementObjectSearcher("select * from Win32_Processor");
            string processor = "";
            foreach (ManagementObject share in processorInfo.Get())
            {
                foreach (PropertyData PD in share.Properties)
                {
                    try
                    {
                        if (PD.Name.ToString() == "Name" || PD.Name.ToString() == "ProcessorId")
                        {
                            processor += PD.Name.ToString() + ": " + PD.Value.ToString() + " \n";
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return processor;
        }
        struct LicenseInfo
        {
            public int licenseCompanyCode { get; set; }
            public string licenseAPIKey { get; set; }
            public string licenseLicenseKey { get; set; }

        }
        struct ServerInfo
        {
            public int licenseCompanyCode { get;  set; }
            public string licenseAPIKey { get; set; }
            public string licenseMAC { get; set; }
            public string licenseProcessorInfo { get; set; }
            public string licenseSystemboardInfo { get; set; }
            public string licenseLicenseKey { get; set; }
        }
        // получаем текущий ip
        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        // функция прослушивания порта
        public void CheckAttractions(object obj)
        {
            if (attractions.Count > 0)
            {
                SqlConn sqlConn = new SqlConn();
                List<Attraction> checkAttractions = new List<Attraction>();
                checkAttractions.AddRange(attractions);
                foreach (Attraction attraction in attractions)
                {
                    Attraction selectAttraction = sqlConn.selectAttraction("attractions", "id='" + attraction.id + "'");
                    if ((DateTime.Now - ((DateTime)selectAttraction.attractionLastPing)).Seconds > 30)
                    {
                        logger.Error("Attraction IP: " + attraction.attractionIp + " Timed out waiting for a response from attraction.");
                        checkAttractions.Remove(attraction);
                    }
                }
                attractions.Clear();
                attractions.AddRange(checkAttractions);
                sqlConn.close();
            }
        }
        public void Listen()
        {
            try
            {
                //if (Process.GetProcesses().Count(x => x.ProcessName == "tcp-server") > 1)
                    //Process.GetCurrentProcess().Kill();
                // Start listening for client requests.
                this.server.Start();


                
                while (true)
                {
                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = this.server.AcceptTcpClient();
                    logger.Info("Connected: ip:  {0} ", client.Client.RemoteEndPoint);
                    //Create a new thread for new client
                    var thread = new Thread(() => { this.WorkWithSocket(client); });
                    thread.Name = "thread: " + client.Client.RemoteEndPoint; 
                    
                    thread.Start();
                    
                }
            }
            catch (SocketException e)
            {
                logger.Error(e, "Socket Exception: " + e.Message );
                
            }
            catch (Exception e)
            {
                logger.Error(e, "Unhendles exception: " + e.Message);
            }
            //finally
            //{
            //    // Stop listening for new clients.
            //    logger.Error("Stop listening for new clients.");
            //    //this.server.Stop();
            //}
        }
        private void MakeBackUpInTime(object obj)
        {
            GC.Collect();
            backUpPath = ConfigurationManager.AppSettings.Get("backUpPath");
            if (backUpPath == null || backUpPath == "")
            {
                backUpPath = Directory.GetCurrentDirectory() + "\\BackUp\\";
            }
            SqlConn conn = new SqlConn();
            bool backUpStatus = false;
            string[] files = Directory.GetFiles(@backUpPath, "*.Bak");
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    DateTime creationDate = File.GetCreationTime(file);
                    if (creationDate.Date == DateTime.Now.Date)
                    {
                        backUpStatus = true;
                    }
                }
            }
            if (backUpStatus == false)
            {
                conn.makeBackUp(backUpPath);
                CheckBDaySale();
            }
            conn.close();
        }
        private void MakeBackUp()
        {
            backUpPath = ConfigurationManager.AppSettings.Get("backUpPath");
            if (backUpPath == null || backUpPath == "")
            {
                backUpPath = Directory.GetCurrentDirectory() + "\\BackUp\\";
            }
            SqlConn conn = new SqlConn();
            Directory.CreateDirectory(backUpPath);
            string[] files = Directory.GetFiles(@backUpPath, "*.Bak");
            conn.makeBackUp(backUpPath);
            conn.close();
        }
        private void WorkWithAPI()
        {
            try
            {
                string apiPort = ConfigurationManager.AppSettings.Get("apiPort");
                IPAddress ipAddress;
                string baseAddress;
                if (IPAddress.TryParse(ConfigurationManager.AppSettings.Get("apiIP"), out ipAddress))
                {
                    baseAddress = "http://" + ipAddress.ToString();
                }
                else
                {
                    baseAddress = "http://*";
                }
                baseAddress += ":"+apiPort;
                // Start OWIN host 
                WebApp.Start<Starup>(url: baseAddress);
                Console.WriteLine("Start API on: " + baseAddress);
                logger.Info("Start API on: " + baseAddress);
            }
            catch (Exception exc)
            {
                logger.Error("API: " + exc.ToString());
            }
        }
        //Аттракцион открыл соединение. 
        private void WorkWithSocket(TcpClient client)
        {
            Byte[] bytes = new Byte[1024];
            String data = null;
            int i;
            try
            {
                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();
                stream.ReadTimeout = 30000;
                while (true)
                {
                    i = stream.Read(bytes, 0, bytes.Length);
                    if (i <= 0) break;
                    data = Encoding.UTF8.GetString(bytes, 0, i);
                    //Console.WriteLine(numberOfRepeat.ToString()+ ": " + DateTime.Now + " \t"+client.Client.RemoteEndPoint+":\n"+data); //теаст вывод 
                    var headerStr = Regex.Match(data, @"^([^<]+)").ToString();
                    //var bodyStr = Regex.Matches(data, @"<.+>$");

                    if (headerStr.Length <= 0) throw new Exception("Empty header.");
                    var header = this.GetHeader(headerStr);

                    var tm = header.Find(x => x.key.Contains("TM")).value.ToString();
                    
                    if (tm.Length <= 0) throw new Exception("TM cant bee undefined.");

                    /*
                     * tm=
                     * 555 - attraction
                     * 
                     */
                     // сравнение по TM. Если аттракцион, то работаем с ним
                    switch (int.Parse(tm))
                    {
                        case 555:
                            this.WorkWithAttraction(client, header, stream);
                            break;
                        default:
                            logger.Info(client.Client.RemoteEndPoint + ": In progress....");
                            break;
                    }
                    
                }
                //logger.Error("Disconnected: {0};", client.Client.RemoteEndPoint);
            }
            catch (SocketException e)
            {
                logger.Error("SocketException: {0}", e.Message);
            }
            catch (Exception e)
            {
                logger.Error(e, "Unholded exception: " +  e.Message);
            }
            finally
            {
                // Shutdown and end connection
                logger.Error("Disconnected: {0};", client.Client.RemoteEndPoint);
                client.Close();
                
            }
        }
        // чтиаем заголовок
        private List<Pair> GetHeader(string headerStr)
        {
            try
            {
                List<Pair> pairs = new List<Pair>();
                var matches = Regex.Matches(headerStr, @"([a-zA-Z]+)\:(.+)\b");
                if (matches.Count <= 0) throw new Exception("Header parse error.");

                foreach (Match match in matches)
                {
                    var groups = match.Groups;
                    pairs.Add(new Pair() { key = groups[1].ToString(), value = groups[2].ToString() });
                }

                return pairs;
                
            }
            catch(Exception e)
            {
                logger.Error(e, "GetHeader error:" + e.Message);
                return new List<Pair>();
            }
        }
        // работа с аттракционом
        private void WorkWithAttraction(TcpClient client, List<Pair> header, NetworkStream stream)
        {
            Byte[] respMsg;
            String data = null;
            string attractionIp = "";
            try
            {
                var ac = header.Find(x => x.key.Contains("AC")).value.ToString();
                if (ac.Length <= 0) throw new Exception("AC cant bee undefined.");
                attractionIp = Regex.Match(client.Client.RemoteEndPoint.ToString(), @"([^:]+)").ToString();
                var conn = new SqlConn(); // работа с БД. Смотреть SqlConn.cs
                var attraction = conn.selectAttraction("attractions", "ip='" + attractionIp + "'");
                if (attraction != null && (int)attraction.id > 0)
                {
                    attractions.Add(attraction);
                    // анализ "action"
                    switch (int.Parse(ac))
                    {
                        case 0:
                            byte pulse_duretion = Convert.ToByte(attraction.attractionPusleDuration);
                            byte param1 = Convert.ToByte(attraction.attractionParam1);

                            data = "<3" + Convert.ToChar(pulse_duretion) + Convert.ToChar(param1) + attraction.attractionName.ToString() + ">";
                            break;
                        case 1:
                            var cc = header.Find(x => x.key.Contains("CC")).value.ToString();
                            //cc = "111=274=2=37179121";
                            if (cc.Length <= 0) throw new Exception("!CC");
                            var groups = Regex.Matches(cc, @"([0-9]+)");

                            var cc1 = groups[1].ToString();
                            if (groups[0].ToString() == "790")
                            {

                                cc1 = groups[2].ToString();
                            }
                            else if (groups[0].ToString() == "111")
                            {
                                cc1 = groups[1].ToString();
                            }
                            int cardId = Int32.Parse(cc1);
                            if (groups.Count > 3)
                            {
                                if (Card.licenseCheck(cc))
                                {
                                    int codeCardBusiness = code;

                                    var card = conn.selectCard("cards", "card_id='" + cardId.ToString() + "'");
                                    if (card != null)
                                    {
                                        data = this.pay(attraction, card, groups[0].ToString());
                                        //logger.Info("AC:" + ac + " from {0}", attractionIp);
                                        //logger.Info("Response: {0}", data);
                                    }
                                    else
                                    {
                                        data = this.messageToAttraction(2, "КАРТА      ", "НЕ НАЙДЕНА");
                                        logger.Info("AC:" + ac + " from {0}", attractionIp + " mess: " + data +" card:" +cc);
                                        //logger.Info("Response: {0}", data);
                                    }
                                }
                                else
                                {
                                    data = this.messageToAttraction(2, "НЕВЕРНАЯ   ", "КАРТА");
                                    logger.Info("AC:" + ac + " from {0}", attractionIp + " mess: " + data + " card:" + cc);
                                    //logger.Info("Response: {0}", data);
                                }
                            }
                            else
                            {
                                data = this.messageToAttraction(2, "НЕВЕРНАЯ   ", "КАРТА");
                                logger.Info("AC:" + ac + " from {0}", attractionIp + " mess: "+ data + " card:" + cc);
                                //logger.Info("Response: {0}", data);
                            }
                            break;
                        case 2:
                            data = "<2КАКАЯ-ТО ИНФОРМАЦИЯ>";
                            break;
                        case 3:

                            data = "<2ЦЕНА: " + Decimal.ToInt32(attraction.attractionPrice) + ">";
                            List<Pair> parameters = new List<Pair>();
                            parameters.Add(new Pair
                            {
                                key = "time_last_ping",
                                value = DateTime.Now
                            });

                            conn.update("attractions", "ip='" + attractionIp + "'", parameters);
                            break;
                        default:
                            logger.Info("status default from {0}", attractionIp);
                            data = "What are you want?";
                            break;
                    }
                }
                else
                {
                    data = "АТТРАКЦИОН НЕ ЗАРЕГИСТРИРОВАН";
                }
                conn.close();
                respMsg = Encoding.GetEncoding(1251).GetBytes(data);
                stream.Write(respMsg, 0, respMsg.Length);
            }
            catch (Exception e)
            {
                data = "<2" + e.Message + ">";
                //logger.Error("Attraction Error: "+e.Message);
                //logger.Info("Response: {0}", data);
                respMsg = Encoding.GetEncoding(1251).GetBytes(data);
                stream.Write(respMsg, 0, respMsg.Length);
            }
        }
        private string messageToAttraction(int status, string first, string last)
        {
            string message = "<" + status;
            for (int i = first.Length; i < 16; i++)
            {
                first += " ";
            }
            message += first;
            message += last;
            message += ">";
           

            return message;
        }
        // проверка оплаты аттракциона
        private string pay(Attraction attraction, Card card, string companyCode)
        {
            TimeSpan timeSpanFrom = new TimeSpan(17, 59, 59);
            TimeSpan timeSpanTo = new TimeSpan(20, 00, 59);
            int adminCardRole = 1;
            string codeOfCompany = code.ToString();
            if (codeOfCompany == companyCode || (790).ToString() == companyCode)
            {
                if ((int)card.cardStatus == 0) return this.messageToAttraction(2, "КАРТА НЕ   ", "АКТИВИРОВАНА");
                if ((int)card.cardStatus == 2) return this.messageToAttraction(2, "KAРTA", "ЗАБЛОКИРОВАНА");

                if ((int)card.cardStatus == 1)
                {
                    var conn = new SqlConn();
                    var price = (decimal)attraction.attractionPrice;
                    var cardCount = (decimal)card.cardCount;
                    var sale = sale_count((int)card.cardSale, attraction);
                    if (attraction.attractionDiscountSpread == true)
                    {
                        price = Math.Ceiling(price - ((price / 100) * sale));
                    }
                    if ((int)card.cardRole == adminCardRole)
                    {
                        conn.log(card.cardId, 23, 0, 0, attraction.id, 0, card.cardCount, (decimal)card.cardBonus, (int)card.cardTicket);
                        return this.messageToAttraction(1, "СПИСАНО: " + 0, "ОСТАТОК: " + Decimal.ToInt32(card.cardCount));
                    }
                    else
                    {
                        SalesInfo saleJule= salesInfos.Find(x => x.salesName == "4sale");
                        SalesInfo saleBabyBoomSoftZone = salesInfos.Find(x => x.salesName == "5sale");
                        SalesInfo saleBabyBoomBabySister = salesInfos.Find(x => x.salesName == "6sale");


                        DateTime dateTimeJule = DateTime.Now;


                        if (saleJule != null 
                            && saleJule.active 
                            && saleJule.attractions != null 
                            && saleJule.attractions.FindIndex(x=> x.id == attraction.id) > -1
                            && (dateTimeJule.TimeOfDay > timeSpanFrom)
                            && (dateTimeJule.TimeOfDay < timeSpanTo)   
                            )
                        {
                            price = 200;
                            //Console.WriteLine(price.ToString());
                            //Console.WriteLine((dateTimeJule.TimeOfDay));
                            //Console.WriteLine((dateTimeJule.TimeOfDay));
                            //Console.WriteLine(timeSpanFrom);
                            //Console.WriteLine(timeSpanTo);
                            //Console.WriteLine(dateTimeJule.DayOfYear);
                        }
                        //if (saleBabyBoomSoftZone != null
                        //    && saleBabyBoomSoftZone.active
                        //    && saleBabyBoomSoftZone.attractions != null
                        //    && (saleBabyBoomSoftZone.attractions.FindIndex(x => x.id == attraction.id) > -1)
                        //    && dateTimeJule.DayOfWeek == DayOfWeek.Saturday
                        //    && dateTimeJule.DayOfWeek == DayOfWeek.Sunday
                        //    )
                        //{
                        //    price = 650;
                        //}
                        if (saleBabyBoomBabySister != null
                            && saleBabyBoomBabySister.active
                            && saleBabyBoomBabySister.attractions != null
                            && (saleBabyBoomBabySister.attractions.FindIndex(x => x.id == attraction.id) > -1)
                            && dateTimeJule.DayOfWeek == DayOfWeek.Saturday
                            && dateTimeJule.DayOfWeek == DayOfWeek.Sunday
                            )
                        {
                            price = 180;
                        }
                        var negative = this.messageToAttraction(2, "ЦЕНА ИГРЫ: " + Decimal.ToInt32(attraction.attractionPrice), "ОСТАТОК: " + Decimal.ToInt32(card.cardCount));             
                        if (cardCount >= price)
                        {
                            var parameters = new List<Pair>() { new Pair() { key = "card_count", value = cardCount - price } };
                            var res = conn.update("cards", "id='" + card.id + "'", parameters);
                            if (res)
                            {
                                conn.log(card.cardId, 10, price, 0, attraction.id, 0, cardCount, (decimal)card.cardBonus, (int)card.cardTicket);
                                conn.close();
                               
                                return this.messageToAttraction(1, "СПИСАНО: " + Decimal.ToInt32(price), "ОСТАТОК: " + Decimal.ToInt32(cardCount -price));
                            }
                            else
                            {
                                conn.close();
                                return negative;
                            }
                        }
                        else
                        {
                            if (attraction.attractionType != 1)
                            {
                                decimal bonus = (decimal)card.cardBonus;
                                decimal dayBonus = card.cardDayBonus;
                                if(dayBonus >= price)
                                {
                                    var parameters = new List<Pair>() { new Pair() { key = "card_day_bonus", value = dayBonus - price } };
                                    var res = conn.update("cards", "id='" + card.id + "'", parameters);

                                    if (res)
                                    {
                                        conn.log(card.cardId, 24, 0, price, attraction.id, 0, card.cardCount, dayBonus, (int)card.cardTicket);
                                        conn.close();
                                        return this.messageToAttraction(1, "СПИСАНО ВСЕГО: " + Decimal.ToInt32(price), "ДБОНУСОВ: " + Decimal.ToInt32(dayBonus - price)); ;
                                    }
                                    else
                                    {
                                        conn.close();
                                        return negative;
                                    }
                                }
                                else if (((dayBonus + cardCount) - price) >= 0)
                                {
                                    var ostatoc = dayBonus - (price - cardCount);
                                    var parameters = new List<Pair>() { new Pair() { key = "card_count", value = 0 },
                                        new Pair() { key = "card_day_bonus", value = ostatoc } };
                                    var res = conn.update("cards", "id='" + card.id + "'", parameters);

                                    if (res)
                                    {
                                        conn.log(card.cardId, 10, cardCount, 0, attraction.id, 0, card.cardCount, dayBonus, (int)card.cardTicket);
                                        conn.log(card.cardId, 24, 0, ostatoc, attraction.id, 0, card.cardCount, dayBonus, (int)card.cardTicket);
                                        conn.close();
                                        return this.messageToAttraction(1, "СПИСАНО: " + Decimal.ToInt32(price), "ДБОНУСОВ: " + Decimal.ToInt32(ostatoc)); ;
                                    }
                                    else
                                    {
                                        conn.close();
                                        return negative;
                                    }
                                }
                                else if (((dayBonus + cardCount + bonus) - price) >= 0)
                                {
                                    var ostatoc = bonus - (price - cardCount - dayBonus);
                                    var parameters = new List<Pair>() { new Pair() { key = "card_count", value = 0 },
                                        new Pair() { key = "card_day_bonus", value = 0 },
                                    new Pair() { key = "card_bonus", value = ostatoc }};
                                    var res = conn.update("cards", "id='" + card.id + "'", parameters);

                                    if (res)
                                    {
                                        conn.log(card.cardId, 10, cardCount, 0, attraction.id, 0, card.cardCount, bonus, (int)card.cardTicket);
                                        conn.log(card.cardId, 9, 0, ostatoc, attraction.id, 0, card.cardCount, bonus, (int)card.cardTicket);
                                        conn.log(card.cardId, 24, 0, dayBonus, attraction.id, 0, card.cardCount, bonus, (int)card.cardTicket);
                                        conn.close();
                                        return this.messageToAttraction(1, "СПИСАНО: " + Decimal.ToInt32(price), "ДБОНУСОВ: " + Decimal.ToInt32(ostatoc)); ;
                                    }
                                    else
                                    {
                                        conn.close();
                                        return negative;
                                    }
                                }
                                else return negative;
                            }
                            else return negative;
                        }
                    }
                }
                {
                    return this.messageToAttraction(2, "КАРТА НЕ   ", "АКТИВИРОВАНА");
                }
            }
            return this.messageToAttraction(2, "ОШИБКА     ", "ОПЛАТЫ");
        }
        private decimal sale_count(int index ,Attraction attraction)
        {
            try
            {
                Object obj = new object();
                CheckSales(obj);
                var conn = new SqlConn();
                Sale sale = conn.selectSale("sales", "sale_id=" + index);
                List<decimal> vs = new List<decimal>();
                vs.Add(sale.saleValue);
                SalesInfo salesInfo1214 = salesInfos.Find(x => x.salesName == "1sale");
                SalesInfo salesInfoWednesDay = salesInfos.Find(x => x.salesName == "2sale");
                SalesInfo salesInfoTueWedThuDay = salesInfos.Find(x => x.salesName == "3sale");
                if (salesInfo1214.active && DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 14)
                {
                    if (salesInfo1214.attractions != null && salesInfo1214.attractions.Count > 0)
                    {
                        Attraction attractionInSale = salesInfo1214.attractions.Find(x => (int)x.id == (int)attraction.id);
                        if (attractionInSale != null && (int)attractionInSale.id > 0 && attraction.attractionDiscountSpread == true && DateTime.Now.DayOfWeek != DayOfWeek.Sunday && DateTime.Now.DayOfWeek != DayOfWeek.Saturday)
                        {
                            vs.Add(salesInfo1214.salePercetage);
                        }
                    }
                }
                if (salesInfoWednesDay.active && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
                {
                    if (salesInfoWednesDay.attractions != null && salesInfoWednesDay.attractions.Count > 0)
                    {
                        Attraction attractionInSale = salesInfoWednesDay.attractions.Find(x => (int)x.id == (int)attraction.id);
                        if (attractionInSale != null && (int)attractionInSale.id > 0 && attraction.attractionDiscountSpread == true)
                        {
                            vs.Add(salesInfoWednesDay.salePercetage);
                        }
                    }
                }
                if (salesInfoTueWedThuDay.active && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday
                    || salesInfoTueWedThuDay.active && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday
                    || salesInfoTueWedThuDay.active && DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
                {
                    if (salesInfoTueWedThuDay.attractions != null && salesInfoTueWedThuDay.attractions.Count > 0)
                    {
                        Attraction attractionInSale = salesInfoTueWedThuDay.attractions.Find(x => (int)x.id == (int)attraction.id);
                        if (attractionInSale != null && (int)attractionInSale.id > 0 && attraction.attractionDiscountSpread == true)
                        {
                            vs.Add(salesInfoTueWedThuDay.salePercetage);
                        }
                    }
                }
                //if (value1 > 100)
                //{
                //    value1 = 0;
                //}
                //if (value1214 > 100)
                //{
                //    value1214 = 0;
                //}
                //if (valueWednesDay > 100)
                //{
                //    valueWednesDay = 0;
                //}
                //decimal valueMax1 = Math.Max(value1, value1214);
                //decimal valueMax2 = Math.Max(value1, valueWednesDay);
                decimal valueMax = 0;
                foreach(decimal saleValue in vs)
                {
                    if(valueMax < saleValue)
                    {
                        valueMax = saleValue;
                    }
                }
                return valueMax;
            }
            catch(Exception exc)
            {
                logger.Error("Ошибка выбора скидки: " + exc.ToString());
                return 0;
            }
        }
        private ServerInfo GetServerInfo()
        {
            ServerInfo serverInfo = new ServerInfo();
            serverInfo.licenseMAC = GetMAC();
            
            serverInfo.licenseProcessorInfo = GetProcessorInfo();
            serverInfo.licenseSystemboardInfo = GetBIOSInfo();
            return serverInfo;
        }
        private  string license(string APIKey)
        {
            ServerInfo serverInfo = GetServerInfo();
            
            byte[] generalHash;
            byte[] primalHash;
            byte[] endHash;
            string license = "";
            using (var deriveBytes = new Rfc2898DeriveBytes(serverInfo.licenseSystemboardInfo,
                Encoding.ASCII.GetBytes(Encoding.ASCII.GetBytes(serverInfo.licenseMAC).Count() < 8 ? serverInfo.licenseMAC + "E7BBF0EDA002C1BA04BFADFC73ED17C8" : serverInfo.licenseMAC)))
            {
                generalHash = deriveBytes.GetBytes(40);
            }
            using (var deriveBytes = new Rfc2898DeriveBytes(serverInfo.licenseProcessorInfo,
               Encoding.ASCII.GetBytes(Encoding.ASCII.GetBytes(APIKey).Count() < 8 ? APIKey + "2a43428bd6988ad194ed4005ff7017c8a" : APIKey)))
            {
                primalHash = deriveBytes.GetBytes(40);
            }
            using (var deriveBytes = new Rfc2898DeriveBytes(primalHash, generalHash, 50))
            {
                endHash = deriveBytes.GetBytes(20);
            }
            foreach (var element in endHash)
            {
                license += Convert.ToInt32(element).ToString();
            }
            return license;
        }
        private void DoSync(object obj)
        {
            if (lastSync.Year < DateTime.Now.Year || lastSync.Month < DateTime.Now.Month || lastSync.Day < DateTime.Now.Day )
            {
                syncStrings = getSyncInfo();
                syncBool = false;
            }
            if (syncStrings!= null && syncStrings.Count > 0 && syncBool == false)
            {
                List<string> forRemoving = new List<string>();

                lastSync = DateTime.Now;
                Sync sync = new Sync();
                MakeBackUp();
                // List<string> forRemovingChaged = new List<string>() ;
                // forRemovingChaged.AddRange(forRemoving);
                foreach (string syncString in syncStrings)
                {
                    sync.Synchronization(syncString);
                }
                foreach (string syncString in syncStrings)
                {
                    if (sync.Synchronization(syncString))
                    {
                        forRemoving.Add(syncString);
                    }
                }
                if (forRemoving.Count == syncStrings.Count)
                {
                    syncBool = true;
                }
            }
        }
        private void CheckSales(object obj)
        {
            try
            {
                SqlConn sqlConn = new SqlConn();
                salesInfos = new List<SalesInfo>();
                salesInfos = sqlConn.select12SaleSalesInfos("sales_info", "");
            }
            catch(Exception exc)
            {
                logger.Error(exc.ToString(), "Ошибка получения списка скидок");
            }

        }
        private void CheckBDaySale()
        {
            SqlConn sqlConn = new SqlConn();
            List<BDaySale> bDaySales = new List<BDaySale>();
            List<Sale> sales = new List<Sale>();
            sales = sqlConn.selectSales("sales", "");
            if (sales != null && sales.Count > 0)
            {
                bDaySales = sqlConn.selectBDaysSales("");
                if (bDaySales != null && bDaySales.Count > 0)
                {
                    foreach (BDaySale bDaySale in bDaySales)
                    {
                        if (DateTime.Now.Day != bDaySale.date.Day)
                        {
                            Sale sale = sales.Find(x => (int)x.saleId == bDaySale.previousSale);
                            if (sale != null && (int)sale.id > 0)
                            {
                                Card card = sqlConn.selectCard("cards","card_id='"+bDaySale.cardId+"'");
                                if (card != null && card.id > 0)
                                {
                                    List<Pair> pairs = new List<Pair>();
                                    pairs.Add(new Pair("card_sale", bDaySale.previousSale));
                                    if (sqlConn.update("cards", "card_id='" + bDaySale.cardId + "'", pairs))
                                    {
                                        if (sqlConn.delete("bday_sales", "card_id='" + bDaySale.cardId + "'"))
                                        {
                                            Card.upgradeCardState(bDaySale.cardId, 0, 16, "установлен пакет скидка " + sale.saleValue + "%", "", card.cardCount, (decimal)card.cardBonus, card.cardTicket);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void CheckDayBonuses(object obj)
        {
            SqlConn sqlConn = new SqlConn();
            List<Card> cards = new List<Card>();
            cards = sqlConn.selectCards("cards", "card_day_bonus_date > '"+ new DateTime(2020,10,20).ToString("yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture) + "'" + " and card_day_bonus_date < '" + DateTime.Now.ToString("yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture) + "'");
            foreach (var card in cards)
            {
                List<Pair> pairs = new List<Pair>();
                pairs.Add(new Pair("card_day_bonus", 0));
                pairs.Add(new Pair("card_day_bonus_date", new DateTime(2020, 10, 20)));
                sqlConn.update("cards", "card_id='" + card.cardId + "'", pairs);
            }
        }
        private List<string> getSyncInfo()
        {
            try
            {
                try
                {
                    //Card.licenseCheck(";111=10010=1=11045198?");
                    syncInfoPath = ConfigurationManager.AppSettings.Get("syncFilePath");
                    if (syncInfoPath == null || syncInfoPath == "")
                    {
                        syncInfoPath = Directory.GetCurrentDirectory();
                    }
                    HttpClient httpClient = new HttpClient();
                    string path = APP_PATH + "/api/Company/GetSyncInfo";
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(path);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    string result = "";
                    License license = checkLicenseFile();
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = JsonConvert.SerializeObject(license);
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                        if (result != "null")
                        {
                            string r = result.Remove(result.Length - 1);
                            var k = JsonConvert.DeserializeObject(result);
                            File.WriteAllText(@syncInfoPath + "\\sync_info", result);
                            return JsonConvert.DeserializeObject<List<string>>(k.ToString());
                        }
                        else
                        {
                            throw new Exception("\nОшибка получения данных для синхронизации.\n");
                        }
                    }
                }
                catch (Exception exc)
                {
                    if (File.Exists(@syncInfoPath + "\\sync_info"))
                    {
                        string r = File.ReadAllText(@syncInfoPath + "\\sync_info");
                        var k = JsonConvert.DeserializeObject(r);
                        return JsonConvert.DeserializeObject<List<string>>(k.ToString());
                    }
                    else
                    {
                       logger.Error(exc.ToString());
                        return null;
                    }
                }
            }
            catch(Exception exc)
            {
                logger.Error(exc.ToString());
                return null;
            }
        }
    }
}
