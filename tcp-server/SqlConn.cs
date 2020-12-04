using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    // клфсс для работы с БД
    public class SqlConn
    {
        static string dataSource = "";
        static string initialCatalog = "";
        static public string schema = "";
        static string userId = "";
        static string password = "";
        public string connString;
        ConnInfo connInfo;
        Logger logger = LogManager.GetCurrentClassLogger();
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
                    string key = Server.DecryptStringFromBytes_Aes(bytesInFile);
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
                        byte[] license = Server.EncryptStringToBytes_Aes(JsonConvert.SerializeObject(connInfo));
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
        public List<TransactionAttractions> selectTransactionAttractions(string table = "transactions_attractions", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");

            var commandString = "SELECT * FROM [" + schema + "].[" + table + "] WHERE " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();


            List<TransactionAttractions> transactions = new List<TransactionAttractions>();
            while (reader.Read())
            {

                transactions.Add(new TransactionAttractions
                {
                    id = reader.GetInt32(0),
                    cardId = reader.GetInt32(1),
                    attractionId = reader.GetValue(2),
                    operation = reader.GetInt32(3),
                    summ = reader.GetDecimal(4),
                    bonus = reader.GetValue(5),
                    tickets = reader.GetValue(6),
                    date = reader.GetDateTime(7),
                    summBalance = reader.GetDecimal(8),
                    bonusesBalance = reader.GetDecimal(9),
                    ticketsBalance = reader.GetInt32(10)
                }
            );

            }
            reader.Close();
            return transactions;
        }
        public Attraction selectAttraction(string table = "attractions", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");

            var commandString = "SELECT * FROM [" + schema + "].[" + table + "] WHERE " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            Attraction attraction = null;

            while (reader.Read())
            {
                attraction = new Attraction()
                {
                    id = reader.GetInt32(0),
                    attractionIp = reader.GetValue(1),
                    attractionPrice = reader.GetDecimal(2),
                    attractionName = reader.GetValue(3),
                    attractionType = reader.GetInt32(4),
                    attractionIsRental = reader.GetValue(5),
                    attractionLastPing = reader.GetValue(6),
                    attractionPusleDuration = reader.GetValue(7),
                    attractionParam1 = reader.GetValue(8),
                    attractionDiscountSpread = reader.GetBoolean(9)
                };
            }
            reader.Close();
            if (attraction == null) throw new Exception("Attraction is no register");
            return attraction;
        }
        public List<Attraction> selectAttractions(string table = "attractions", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length > 0) where = "WHERE " + where;

            var commandString = "SELECT * FROM [" + schema + "].[" + table + "] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            List<Attraction> attractions = new List<Attraction>();
            while (reader.Read())
            {

                attractions.Add(new Attraction
                {
                    id = reader.GetInt32(0),
                    attractionIp = reader.GetValue(1),
                    attractionPrice = reader.GetDecimal(2),
                    attractionName = reader.GetValue(3),
                    attractionType = reader.GetInt32(4),
                    attractionIsRental = reader.GetValue(5),
                    attractionLastPing = reader.GetValue(6),
                    attractionPusleDuration = reader.GetValue(7),
                    attractionParam1 = reader.GetValue(8),
                    attractionDiscountSpread = reader.GetBoolean(9)
                }
                );
            }
            reader.Close();
            return attractions;
        }
        public Card selectCard(string table = "cards", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");

            var commandString = "SELECT * FROM [" + schema + "].[" + table + "] WHERE " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            Card card = null;

            while (reader.Read())
            {
                card = new Card()
                {
                    id = reader.GetInt32(0),
                    cardId = reader.GetInt32(1),
                    cardCount = reader.GetDecimal(2),
                    cardSale = reader.GetInt32(3),
                    cardRole = reader.GetInt32(4),
                    cardStatus = reader.GetInt32(5),
                    cardBonus = reader.GetDecimal(6),
                    cardTicket = reader.GetInt32(7),
                    cardParentName = reader.GetString(8),
                    cardRegDate = reader.GetDateTime(9),
                    cardDayBonus = (reader.IsDBNull(10) == false) ? reader.GetDecimal(10) : 0,
                    cardDayBonusDateTime = (reader.IsDBNull(10) == false) ? reader.GetDateTime(11) : new DateTime(20, 10, 2020)
                };
            }
            reader.Close();

            return card;
        }
        public List<Card> selectCards(string from = "cards", string where = "")
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + from + "] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            List<Card> cards = new List<Card>();
            while (reader.Read())
            {
                cards.Add(new Card()
                {
                    id = reader.GetInt32(0),
                    cardId = reader.GetInt32(1),
                    cardCount = reader.GetDecimal(2),
                    cardSale = reader.GetInt32(3),
                    cardRole = reader.GetInt32(4),
                    cardStatus = reader.GetInt32(5),
                    cardBonus = reader.GetDecimal(6),
                    cardTicket = reader.GetInt32(7),
                    cardParentName = reader.GetString(8),
                    cardRegDate = reader.GetDateTime(9),
                    cardDayBonus = (reader.IsDBNull(10) == false) ? reader.GetDecimal(10) : 0,
                    cardDayBonusDateTime = (reader.IsDBNull(10) == false) ? reader.GetDateTime(11) : new DateTime(20, 10, 2020)
                });

            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return cards;
        }
        public List<TransactionCashRegister> selectTransactionCashRegister(string table = "transactions_cashiermashine", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            List<TransactionCashRegister> transactions = new List<TransactionCashRegister>();
            while (reader.Read())
            {
                transactions.Add(new TransactionCashRegister
                {
                    id = reader.GetInt32(0),
                    cardId = reader.GetInt32(1),
                    toCardId = reader.GetInt32(2),
                    operation = reader.GetInt32(3),
                    summ = reader.GetDecimal(4),
                    bonus = reader.GetDecimal(5),
                    tickets = reader.GetInt32(6),
                    cashier_register_id = reader.GetInt32(7),
                    cashier_id = reader.GetInt32(8),
                    cashier_name = reader.GetString(9),
                    date = reader.GetDateTime(10),
                    summBalance = reader.GetDecimal(11),
                    bonusesBalance = reader.GetDecimal(12),
                    ticketsBalance = reader.GetInt32(13)
                }
            );
            }
            reader.Close();
            return transactions;
        }
        public Sale selectSale(string table = "sales", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");


            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();

            Sale sale = null;

            while (reader.Read())
            {
                sale = new Sale()
                {
                    id = reader.GetValue(0),
                    saleId = reader.GetValue(1),
                    saleValue = reader.GetDecimal(2)
                };
            }
            reader.Close();
            if (sale == null) throw new Exception("Скидка не        зарегистрирована");
            return sale;
        }
        public List<Sale> selectSales(string table = "sales", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");


            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();

            List<Sale> sales = new List<Sale>();

            while (reader.Read())
            {
                sales.Add(new Sale()
                {
                    id = reader.GetValue(0),
                    saleId = reader.GetValue(1),
                    saleValue = reader.GetDecimal(2)
                });
            }
            reader.Close();
            if (sales == null) throw new Exception("Скидки не найдены");
            return sales;
        }
        public List<CashierRegister> selectCashierRegisters(string from = "cashierregister", string where = "")
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + from + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            List<CashierRegister> cashierRegisters = new List<CashierRegister>();
            while (reader.Read())
            {
                cashierRegisters.Add(new CashierRegister()
                {
                    id = reader.GetValue(0),
                    cashierRegisterId = reader.GetValue(1),
                    cashierRegisterIP = reader.GetValue(2),
                    timeLastPing = reader.GetDateTime(3)
                });

            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return cashierRegisters;
        }
        public CashierRegister selectCashierRegister(string from = "cashierregister", string where = "")
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + from + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            CashierRegister cashierRegister = new CashierRegister();
            while (reader.Read())
            {
                cashierRegister = new CashierRegister(
                    reader.GetValue(0),
                    reader.GetValue(1),
                    reader.GetValue(2),
                    reader.GetDateTime(3)
                );

            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return cashierRegister;
        }
        public List<Ticket> selectTickets(string table = "tickets", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();


            List<Ticket> tickets = new List<Ticket>();
            while (reader.Read())
            {

                tickets.Add(new Ticket
                {
                    id = reader.GetValue(0),
                    ticket_id = reader.GetInt32(1),
                    prise = reader.GetInt32(2),
                    count = 0,
                    total = 0
                }
            );

            }
            reader.Close();
            return tickets;
        }
        public bool update(string table, string where, List<Pair> parameters)
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");
            if (parameters.Count <= 0) throw new Exception("SQL Update 'parameters' count < 0;");

            var command = new SqlCommand();
            string parametersString = "";
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue("@" + param.key, param.value);
                parametersString += param.key + " = @" + param.key + ",";
            }
            parametersString = parametersString.Trim(',');
            var commandString = "UPDATE [" + schema + "].[" + table + "]" + " SET " + parametersString + " WHERE " + where + ";";
            command.CommandText = commandString;
            command.Connection = this.conn;
            var res = command.ExecuteNonQuery();
            return (res > 0) ? true : false;
        }
        public bool insert(string table, List<Pair> parameters)
        {
            if (table.Length <= 0) throw new Exception("SQL Insert 'table' cant bee undefined.");
            if (parameters.Count <= 0) throw new Exception("SQL Insert 'parameters' count < 0;");

            var command = new SqlCommand();
            string parametersString = "";
            string valuesString = "";
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue("@" + param.key, param.value);
                parametersString += param.key + ",";
                valuesString += "@" + param.key + ",";
            }
            parametersString = parametersString.Trim(',');
            valuesString = valuesString.Substring(0, valuesString.Length - 1);
            var commandString = "INSERT [" + schema + "].[" + table + "]" + " (" + parametersString + ") VALUES (" + valuesString + ");";
            command.CommandText = commandString;
            command.Connection = this.conn;
            var res = command.ExecuteNonQuery();
            return (res > 0) ? true : false;
        }
        public bool delete(string table, string where)
        {
            if (table.Length <= 0) throw new Exception("sql delete table cant bee undefined;");
            if (where.Length <= 0) throw new Exception("sql where cant bee undefined;");

            var command = new SqlCommand("DELETE  [" + schema + "].[" + table + "]" + " WHERE " + where + ";", this.conn);
            var res = command.ExecuteNonQuery();
            return (res > 0) ? true : false;
        }
        public int log(
            object card_id,
            int operation,
            decimal summ,
            decimal bonus,
            object attraction_id,
            decimal tickets,
            decimal cardSumm,
            decimal cardBonuses,
            int cardTickets
            )
        {
            var command = new SqlCommand();
            //string format = "yyyy-MM-dd HH:mm:ss";
            command.Parameters.AddWithValue("@card_id", (int)card_id);
            command.Parameters.AddWithValue("@attraction_id", (int)attraction_id);
            command.Parameters.AddWithValue("@operation", operation);
            command.Parameters.AddWithValue("@summ", summ);
            command.Parameters.AddWithValue("@bonus", bonus);
            command.Parameters.AddWithValue("@tickets", tickets);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            command.Parameters.AddWithValue("@summ_balance", cardSumm);
            command.Parameters.AddWithValue("@bonuses_balance", cardBonuses);
            command.Parameters.AddWithValue("@tickets_balance", cardTickets);

            command.CommandText = "INSERT INTO [" + schema + "].[transactions_attractions] (card_id, attraction_id, operation, summ, bonus, tickets, date, summ_balance, bonuses_balance, tickets_balance) VALUES (@card_id, @attraction_id, @operation, @summ, @bonus, @tickets, @date, @summ_balance, @bonuses_balance, @tickets_balance);";
            command.Connection = this.conn;

            var res = command.ExecuteNonQuery();
            return res;
        }
        public List<AttractionType> selectAttractionTypes(string table = "attraction_types")
        {


            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            //if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");

            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            List<AttractionType> attractionType = new List<AttractionType>();
            while (reader.Read())
            {
                attractionType.Add(new AttractionType
                {
                    id = reader.GetInt32(0),
                    attractionTypeId = reader.GetInt32(1),
                    attractionTypeName = reader.GetString(2)
                });
            }
            reader.Close();
            return attractionType;
        }
        public List<CardEvent> selectCardEvents(string table = "card_event", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");

            var commandString = "SELECT * FROM [" + schema + "].[" + table + "] WHERE " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();


            List<CardEvent> events = new List<CardEvent>();
            while (reader.Read())
            {

                events.Add(new CardEvent
                {
                    id = reader.GetValue(0),
                    cardId = reader.GetInt32(1),
                    toCardId = reader.GetValue(2),
                    cardEvent = reader.GetValue(3),
                    cardMessage = reader.GetValue(4),
                    cashierRegisterId = reader.GetValue(5),
                    cashierRegisterIP = reader.GetValue(6),
                    cashierId = reader.GetValue(7),
                    cashierName = reader.GetValue(8),
                    date = reader.GetDateTime(9),
                    summBalance = reader.GetDecimal(10),
                    bonusesBalance = reader.GetDecimal(11),
                    ticketsBalance = reader.GetInt32(12)
                }
            );

            }
            reader.Close();
            return events;
        }
        public Cashier selectCashier(string table = "cashiers", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");

            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();


            Cashier cashier = null;
            while (reader.Read())
            {

                cashier = new Cashier
                {
                    id = reader.GetValue(0),
                    cashierId = reader.GetValue(1),
                    cashierName = reader.GetValue(2),
                    cashierCardId = reader.GetValue(3)
                };

            }
            reader.Close();
            return cashier;
        }
        public Admin selectAdmin(string table = "admins", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");

            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            Admin admin = null;
            while (reader.Read())
            {
                admin = new Admin
                {
                    id = reader.GetInt32(0),
                    login = reader.GetString(1),
                    FIO = reader.GetString(2),
                    password = reader.GetString(3),
                    salt = reader.GetString(4),
                    privateKey = reader.GetString(5),
                    publicKey = reader.GetString(6),
                    keyTime = reader.GetDateTime(7)
                };

            }
            reader.Close();
            return admin;
        }
        public List<Admin> selectAdmins(string table = "admins", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");

            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            List<Admin> admins = new List<Admin>();
            while (reader.Read())
            {
                admins.Add(new Admin
                {
                    id = reader.GetInt32(0),
                    login = reader.GetString(1),
                    FIO = reader.GetString(2)
                });

            }           
            reader.Close();
            List<AdminCard> adminCards = getAdminCards("");
            foreach (Admin admin in admins)
            {
                AdminCard adminCard = adminCards.Find(x => x.adminId == admin.id);
                if (adminCard != null && adminCard.id > 0)
                {
                    admin.cardId = adminCard.cardId;
                }
            }
            return admins;
        }
        public List<AdminCard> getAdminCards(string where)
        {
            //if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");

            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[admin_cards]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            List<AdminCard> admins = new List<AdminCard>();
            while (reader.Read())
            {
                admins.Add(new AdminCard
                {
                    id = reader.GetInt32(0),
                    adminId = reader.GetInt32(1),
                    cardId = reader.GetInt32(2)
                });

            }
            reader.Close();
            return admins;
        }
        public AdminCard getAdminCard(string where)
        {
            //if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");

            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[admin_cards]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            AdminCard adminCard = new AdminCard();
            while (reader.Read())
            {
                adminCard = new AdminCard
                {
                    id = reader.GetInt32(0),
                    adminId = reader.GetInt32(1),
                    cardId = reader.GetInt32(2)
                };

            }
            reader.Close();
            return adminCard;
        }
        public List<Cashier> selectCashiers(string table = "cashiers", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");

            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();


            List<Cashier> cashiers = new List<Cashier>();
            while (reader.Read())
            {

                cashiers.Add(new Cashier
                {
                    id = reader.GetValue(0),
                    cashierId = reader.GetValue(1),
                    cashierName = reader.GetValue(2),
                    cashierCardId = reader.GetValue(3)
                });

            }
            reader.Close();
            return cashiers;
        }
        public List<Client> selectClient(string table = "client_info", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");

            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();


            List<Client> clients = new List<Client>();
            while (reader.Read())
            {

                clients.Add(new Client
                {
                    id = reader.GetValue(0),
                    cardId = reader.GetValue(1),
                    childrenName = reader.GetValue(2),
                    childrenDate = reader.GetValue(3),
                    parentName = reader.GetValue(4),
                    adultCard = reader.GetValue(5)
                }
            );

            }
            reader.Close();
            return clients;
        }
        public CardStatus selectCardStatus(string table = "card_state", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");


            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();


            CardStatus cardStatus = null;
            while (reader.Read())
            {

                cardStatus = new CardStatus
                {
                    id = reader.GetInt32(0),
                    status_id = reader.GetInt32(1),
                    status_message = reader.GetString(2),
                };

            }
            reader.Close();
            return cardStatus;
        }
        public List<CardStatus> selectCardStatuses(string table = "card_state", string where = "")
        {
            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");


            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "]" + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();


            List<CardStatus> cardStatuses = new List<CardStatus>();
            while (reader.Read())
            {
                cardStatuses.Add(new CardStatus
                {
                    id = reader.GetInt32(0),
                    status_id = reader.GetInt32(1),
                    status_message = reader.GetString(2),
                });
            }
            reader.Close();
            return cardStatuses;
        }
        public Card select(string from = "cards", string where = "")
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + from + "] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            Card card = null;
            while (reader.Read())
            {
                card = new Card()
                {
                    id = reader.GetInt32(0),
                    cardId = reader.GetInt32(1),
                    cardCount = reader.GetDecimal(2),
                    cardSale = reader.GetInt32(3),
                    cardRole = reader.GetInt32(4),
                    cardStatus = reader.GetInt32(5),
                    cardBonus = reader.GetDecimal(6),
                    cardTicket = reader.GetInt32(7),
                    cardParentName = reader.GetString(8),
                    cardRegDate = reader.GetDateTime(9),
                    cardDayBonus = (reader.IsDBNull(10) == false) ? reader.GetDecimal(10) : 0,
                    cardDayBonusDateTime = (reader.IsDBNull(10) == false) ? reader.GetDateTime(11) : new DateTime(20,10,2020)
                };

            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return card;
        }
        public bool makeBackUp(string backUpPath)
        {
            try
            {
                string backUpName = backUpPath
                   + "BackUp_" + DateTime.Now.ToString().Replace(':','-') + ".bak";
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
                logger.Info(exc);
                return false;
            }
        }
        public CardPrice selectCardPrice(string from = "cards_price", string where = "")
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + from + "] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            CardPrice cardPrice = new CardPrice();
            while (reader.Read())
            {
                cardPrice = new CardPrice()
                {
                    id = reader.GetInt32(0),
                    cardId = reader.GetInt32(1),
                    cardPrice = reader.GetDecimal(2)
                };
            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return cardPrice;
        }
        public WorkShift selectWorkShift(string from = "work_shifts", string where = "")
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + from + "] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            WorkShift workShift = new WorkShift();
            while (reader.Read())
            {
                workShift = new WorkShift()
                {
                    id = reader.GetInt32(0),
                    cashierMashineId = reader.GetInt32(1),
                    startTime = reader.GetDateTime(2),
                    endTime = (reader.IsDBNull(3) == false) ? reader.GetDateTime(3) : DateTime.MinValue,
                    cash = reader.GetDecimal(4),
                    cashCount = reader.GetInt32(5),
                    cashlessPayment = reader.GetDecimal(6),
                    cashlessPaymentCount = reader.GetInt32(7),
                    creditCard = reader.GetDecimal(8),
                    creditCardCount = reader.GetInt32(9),
                    contributions = reader.GetDecimal(10),
                    contributionsCount = reader.GetInt32(11),
                    refund = reader.GetDecimal(12),
                    refundCount = reader.GetInt32(13),
                    withdrawal = reader.GetDecimal(14),
                    withdrawalCount = reader.GetInt32(15),
                    revenue = reader.GetDecimal(16),
                    cashOnHand = reader.GetDecimal(17),
                    nonNullableAmount = reader.GetDecimal(18),
                    closedShifts = reader.GetInt32(19),
                    isClosed =reader.GetBoolean(20)
                };
            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return workShift;
        }
        public WorkShiftInfo selectWorkShiftInfo(string from = "work_shifts_info", string where = "")
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + from + "] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            WorkShiftInfo workShiftInfo = new WorkShiftInfo();
            while (reader.Read())
            {
                workShiftInfo = new WorkShiftInfo()
                {
                    id = reader.GetInt32(0),
                    workShiftId = reader.GetInt32(1),
                    cashierMashineId = reader.GetInt32(2),
                    cashierId = reader.GetInt32(3),
                    cashierName = reader.GetString(4),
                    cash = reader.GetDecimal(5),
                    cashCount = reader.GetInt32(6),
                    cashlessPayment = reader.GetDecimal(7),
                    cashlessPaymentCount = reader.GetInt32(8),
                    creditCard = reader.GetDecimal(9),
                    creditCardCount = reader.GetInt32(10),
                    contributions = reader.GetDecimal(11),
                    contributionsCount = reader.GetInt32(12),
                    refund = reader.GetDecimal(13),
                    refundCount = reader.GetInt32(14),
                    withdrawal = reader.GetDecimal(15),
                    withdrawalCount = reader.GetInt32(16),
                    revenue = reader.GetDecimal(17)
                };
            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return workShiftInfo;
        }
        public List<WorkShiftInfo> selectWorkShiftInfos(string from = "work_shifts_info", string where = "")
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + from + "] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            List<WorkShiftInfo> workShiftInfos = new List<WorkShiftInfo>();
            while (reader.Read())
            {
                workShiftInfos.Add( new  WorkShiftInfo
                {
                    id = reader.GetInt32(0),
                    workShiftId = reader.GetInt32(1),
                    cashierMashineId = reader.GetInt32(2),
                    cashierId = reader.GetInt32(3),
                    cashierName = reader.GetString(4),
                    cash = reader.GetDecimal(5),
                    cashCount = reader.GetInt32(6),
                    cashlessPayment = reader.GetDecimal(7),
                    cashlessPaymentCount = reader.GetInt32(8),
                    creditCard = reader.GetDecimal(9),
                    creditCardCount = reader.GetInt32(10),
                    contributions = reader.GetDecimal(11),
                    contributionsCount = reader.GetInt32(12),
                    refund = reader.GetDecimal(13),
                    refundCount = reader.GetInt32(14),
                    withdrawal = reader.GetDecimal(15),
                    withdrawalCount = reader.GetInt32(16),
                    revenue = reader.GetDecimal(17)
                });
            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return workShiftInfos;
        }
        public List<WorkShift> selectWorkShifts(string from = "work_shifts", string where = "")
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + from + "] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            List<WorkShift> workShift = new List<WorkShift>();
            while (reader.Read())
            {
                workShift.Add( new WorkShift()
                {
                    id = reader.GetInt32(0),
                    cashierMashineId = reader.GetInt32(1),
                    startTime = reader.GetDateTime(2),
                    endTime = (reader.IsDBNull(3) == false) ? reader.GetDateTime(3) : DateTime.MinValue,
                    cash = reader.GetDecimal(4),
                    cashCount = reader.GetInt32(5),
                    cashlessPayment = reader.GetDecimal(6),
                    cashlessPaymentCount = reader.GetInt32(7),
                    creditCard = reader.GetDecimal(8),
                    creditCardCount = reader.GetInt32(9),
                    contributions = reader.GetDecimal(10),
                    contributionsCount = reader.GetInt32(11),
                    refund = reader.GetDecimal(12),
                    refundCount = reader.GetInt32(13),
                    withdrawal = reader.GetDecimal(14),
                    withdrawalCount = reader.GetInt32(15),
                    revenue = reader.GetDecimal(16),
                    cashOnHand = reader.GetDecimal(17),
                    nonNullableAmount = reader.GetDecimal(18),
                    closedShifts = reader.GetInt32(19),
                    isClosed = reader.GetBoolean(20)
                });
            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return workShift;
        }
        public bool updateWorkShift(string table , string where, WorkShift workShift, decimal cash, decimal cashlessPayment, decimal creditCard, decimal refund)
        {
            List<Pair> parameters = new List<Pair>();

            //parameters.Add(new Pair("end_time", ));
            if (cash > 0)
            {
                parameters.Add(new Pair("cash", workShift.cash + cash));
                parameters.Add(new Pair("cash_count", ++workShift.cashCount));
            }
            if(cashlessPayment > 0)
            {
                parameters.Add(new Pair("cashless_payment", workShift.cashlessPayment + cashlessPayment));
                parameters.Add(new Pair("cashless_payment_count", ++workShift.cashlessPaymentCount));
            }
            if(creditCard > 0)
            {
                parameters.Add(new Pair("credit_card", workShift.creditCard + creditCard));
                parameters.Add(new Pair("credit_card_count", ++workShift.creditCardCount));
            }

            if (refund > 0)
            {
                parameters.Add(new Pair("refund", workShift.refund + refund));
                parameters.Add(new Pair("refund_count", ++workShift.refundCount));
            }
            parameters.Add(new Pair("revenue", workShift.revenue + cash + cashlessPayment + creditCard));
            parameters.Add(new Pair("cash_on_hand", workShift.cashOnHand + cash + cashlessPayment + creditCard - refund));
            parameters.Add(new Pair("nonnullable_amount", workShift.nonNullableAmount + cash + cashlessPayment + creditCard));

            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");
            if (parameters.Count <= 0) throw new Exception("SQL Update 'parameters' count < 0;");

            var command = new SqlCommand();
            string parametersString = "";
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue("@" + param.key, param.value);
                parametersString += param.key + " = @" + param.key + ",";
            }
            parametersString = parametersString.Trim(',');
            var commandString = "UPDATE [" + schema + "].[" + table + "]" + " SET " + parametersString + " WHERE " + where + ";";
            command.CommandText = commandString;
            command.Connection = this.conn;
            var res = command.ExecuteNonQuery();
            return (res > 0) ? true : false;
        }
        public bool updateWorkShiftInfo(string table, string where, WorkShift workShift, decimal cash, decimal cashlessPayment, decimal creditCard, decimal refund)
        {
            List<Pair> parameters = new List<Pair>();
            WorkShiftInfo workShiftInfo = new WorkShiftInfo();
            workShiftInfo = selectWorkShiftInfo("work_shifts_info", where);
            //parameters.Add(new Pair("end_time", ));
            if (cash > 0)
            {
                parameters.Add(new Pair("cash", workShiftInfo.cash + cash));
                parameters.Add(new Pair("cash_count", ++workShiftInfo.cashCount));
            }
            if (cashlessPayment > 0)
            {
                parameters.Add(new Pair("cashless_payment", workShiftInfo.cashlessPayment + cashlessPayment));
                parameters.Add(new Pair("cashless_payment_count", ++workShiftInfo.cashlessPaymentCount));
            }
            if (creditCard > 0)
            {
                parameters.Add(new Pair("credit_card", workShiftInfo.creditCard + creditCard));
                parameters.Add(new Pair("credit_card_count", ++workShiftInfo.creditCardCount));
            }

            if (refund > 0)
            {
                parameters.Add(new Pair("refund", workShiftInfo.refund + refund));
                parameters.Add(new Pair("refund_count", ++workShiftInfo.refundCount));
            }
            parameters.Add(new Pair("revenue", workShiftInfo.revenue + cash + cashlessPayment + creditCard));

            if (table.Length <= 0) throw new Exception("SQL Update 'table' cant bee undefined.");
            if (where.Length <= 0) throw new Exception("SQL Update 'where' cant bee undefined.");
            if (parameters.Count <= 0) throw new Exception("SQL Update 'parameters' count < 0;");

            var command = new SqlCommand();
            string parametersString = "";
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue("@" + param.key, param.value);
                parametersString += param.key + " = @" + param.key + ",";
            }
            parametersString = parametersString.Trim(',');
            var commandString = "UPDATE [" + schema + "].[" + table + "]" + " SET " + parametersString + " WHERE " + where + ";";
            command.CommandText = commandString;
            command.Connection = this.conn;
            var res = command.ExecuteNonQuery();
            return (res > 0) ? true : false;
        }
        public SalesInfo select12SaleSalesInfo(string table, string where)
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            SalesInfo salesInfo = new SalesInfo();
            while (reader.Read())
            {
                salesInfo = new SalesInfo()
                {
                    id = reader.GetInt32(0),
                    salesName = reader.GetString(1),
                    attractions = JsonConvert.DeserializeObject<List<Attraction>>(reader.GetString(2)),
                    active = reader.GetBoolean(9)
                    
                };
            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return salesInfo;
        }
        public List<SalesInfo> select12SaleSalesInfos(string table, string where)
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[" + table + "] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            List<SalesInfo> salesInfos = new List<SalesInfo>();
            while (reader.Read())
            {
                salesInfos.Add(new SalesInfo()
                {
                    id = reader.GetInt32(0),
                    salesName = reader.GetString(1),
                    attractions = JsonConvert.DeserializeObject<List<Attraction>>(reader.GetString(2)),
                    active = reader.GetBoolean(9),
                    salePercetage = reader.GetDecimal(10)
                });
            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return salesInfos;
        }
        public List<BDaySale> selectBDaysSales(string where)
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[bday_sales] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            List<BDaySale> bDaySales = new List<BDaySale>();
            while (reader.Read())
            {
                bDaySales.Add(new BDaySale()
                {
                    id = reader.GetInt32(0),
                    cardId = reader.GetInt32(1),
                    previousSale = reader.GetInt32(2),
                    date = reader.GetDateTime(3)
                });
            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return bDaySales;
        }
        public BDaySale selectBDaysSale(string where)
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[bday_sales] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            BDaySale bDaySale = new BDaySale();
            while (reader.Read())
            {
                bDaySale = new BDaySale()
                {
                    id = reader.GetInt32(0),
                    cardId = reader.GetInt32(1),
                    previousSale = reader.GetInt32(2),
                    date = reader.GetDateTime(3)
                };
            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return bDaySale;
        }
        public CardLicense selectCardLicense(string where)
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[cards_license] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            CardLicense cardLicense = new CardLicense();
            while (reader.Read())
            {
                cardLicense = new CardLicense()
                {
                    id = reader.GetInt32(0),
                    cardId = reader.GetInt32(1),
                    License = reader.GetString(2)
                };
            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return cardLicense;
        }
        public ClientContact selectClientContact(string where)
        {
            if (where.Length > 0) where = " WHERE " + where;
            var commandString = "SELECT * FROM [" + schema + "].[client_contacts] " + where + ";";
            var command = new SqlCommand(commandString, this.conn);
            var reader = command.ExecuteReader();
            ClientContact clientContact = new ClientContact();
            while (reader.Read())
            {
                clientContact = new ClientContact()
                {
                    id = reader.GetInt32(0),
                    cardId = reader.GetInt32(1),
                    email = reader.GetString(2),
                    telephone = reader.GetString(3)
                };
            }
            //if (card == null) throw new Exception("Card is undefined");
            reader.Close();
            return clientContact;
        }

        public void close()
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                this.conn.Close();
            }
        }
    }
}


   