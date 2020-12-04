using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebSyncContract
{
    class SqlConn
    {
        string dataSource = "";
        string initialCatalog = "";
        public string schema = "";
        string userId = "";
        string password = "";
        public string connString;
        ConnInfo connInfo;
        struct ConnInfo
        {
            public string dataSource;
            public string initialCatalog;
            public string schema;
            public string userId;
            public string password;
        }
        private SqlConnection conn;


        public SqlConn(
            )
        {
            if ((dataSource == null || dataSource == "")
                || (initialCatalog == null || initialCatalog == "")
                || (schema == null || schema == "")
                || (userId == null || userId == "") ||
                (password == null || password == ""))
            {
                
                string fileName = "./db_connection";
                if (File.Exists(fileName))
                {
                    Console.WriteLine("Проверка доступа к базе данных. ");
                    byte[] bytesInFile = File.ReadAllBytes(fileName);
                    string key = DecryptStringFromBytes_Aes(bytesInFile);
                    connInfo = JsonConvert.DeserializeObject<ConnInfo>(key);
                    dataSource = connInfo.dataSource;
                    initialCatalog = connInfo.initialCatalog;
                    schema = connInfo.schema;
                    userId = connInfo.userId;
                    password = connInfo.password;
                }
                else
                {
                    Console.WriteLine("Проверка доступа к базе данных. ");
                    if (GetConnInfo())
                    {
                        byte[] license = EncryptStringToBytes_Aes(JsonConvert.SerializeObject(connInfo));
                        System.IO.File.WriteAllBytes(fileName, license);
                        dataSource = connInfo.dataSource;
                        initialCatalog = connInfo.initialCatalog;
                        schema = connInfo.schema;
                        userId = connInfo.userId;
                        password = connInfo.password;
                    }
                }
                connString = @"Data Source=" + dataSource + ";Initial Catalog=" + initialCatalog + ";User ID=" + userId + ";Password=" + password + ";";
                this.conn = new SqlConnection(connString);
                this.conn.Open();
                this.conn.Close();
                Console.WriteLine("\nДоступ к базе данных получен.");
            }
            connString = @"Data Source=" + dataSource + ";Initial Catalog=" + initialCatalog + ";User ID=" + userId + ";Password=" + password + ";";
            this.conn = new SqlConnection(connString);
            this.conn.Open();
        }
        public void close()
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                this.conn.Close();
            }
        }
        public bool GetConnInfo()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Введите расположение сервера базы данных:");
                    connInfo.dataSource = Console.ReadLine();
                    Console.WriteLine("Введите имя базы данных:");
                    connInfo.initialCatalog = Console.ReadLine();
                    Console.WriteLine("Введите схему(schema) таблиц базы данных:");
                    connInfo.schema = Console.ReadLine();
                    Console.WriteLine("Введите имя пользователя сервера:");
                    connInfo.userId = Console.ReadLine();
                    Console.WriteLine("Введите пароль пользователя сервера:");
                    string str = string.Empty;
                    ConsoleKeyInfo key;
                    do
                    {
                        key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Enter) break;
                        if (key.Key == ConsoleKey.Backspace)
                        {
                            if (str.Length != 0)
                            {
                                str = str.Remove(str.Length - 1);
                                Console.Write("\b \b");
                            }
                        }
                        else
                        {
                            str += key.KeyChar;
                            Console.Write("*");
                        }
                    }
                    while (true);
                    connInfo.password = str;
                    try
                    {
                        connString = @"Data Source=" + connInfo.dataSource + ";Initial Catalog=" + connInfo.initialCatalog + ";User ID=" + connInfo.userId + ";Password=" + connInfo.password + ";";
                        this.conn = new SqlConnection(connString);
                        this.conn.Open();
                        this.conn.Close();
                        return true;
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("\nОшибка подключения к базе данных: " + exc.Message + "\nПовторите ввод данных базы.");
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return false;
            }
        }
        public static string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            // Check arguments.
            byte[] Key = Encoding.Default.GetBytes(GetBIOSInfo());
            if (Key.Length > 32)
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
        public static byte[] EncryptStringToBytes_Aes(string plainText)
        {
            // Check arguments.
            byte[] Key = Encoding.Default.GetBytes(GetBIOSInfo());
            if (Key.Length > 32)
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
                        BIOS += PD.Name.ToString() + ": " + PD.Value.ToString() + " \n";
                    }
                    catch
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
        public bool makeBackUp(string backUpPath)
        {
            try
            {
                string backUpName = backUpPath
                   + "BackUp_" + DateTime.Now.ToString().Replace(':', '-') + ".bak";
                if (!File.Exists(backUpName))
                {
                    var commandString = "USE " + initialCatalog + ";  BACKUP DATABASE " + initialCatalog + " TO DISK = '" + backUpName + "' ";
                    var command = new SqlCommand(commandString, this.conn);
                    command.CommandText = commandString;
                    command.Connection = this.conn;
                    var res = command.ExecuteNonQuery();
                    Console.WriteLine("Создана резервная копия базы данных");
                    return (res > 0) ? true : false;
                }
                return false;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return false;
            }
        }
    }
}
