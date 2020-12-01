using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class Admin
    {
        public int id { get; set; }
        public string login { get; set; }
        public string FIO { get; set; }
        public string password { get; set; }
        public byte[] passwordEncript { get; set; }
        public string salt { get; set; }
        public string privateKey { get; set; }
        public string publicKey { get; set; }
        public DateTime keyTime { get; set; }
        public string cardInfo {get; set;}
        public int cardId { get; set; }
        public Admin()
        {

        }
        public Admin(
            int id,
            string login,
            string FIO,
            string password,
            string salt,
            byte[] passwordEncript,
            string privateKey,
            string publicKey,
            DateTime keyTime
            )
        {
            this.id = id;
            this.login = login;
            this.FIO = FIO;
            this.password = password;
            this.passwordEncript = passwordEncript;
            this.salt = salt;
            this.privateKey = privateKey;
            this.publicKey = privateKey;
            this.keyTime = keyTime;
        }
        RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
        public string checkLogin()
        {
            SqlConn conn = new SqlConn();
            Admin admin = conn.selectAdmin("admins", "login='" + login + "'");
            if (admin != null && admin.id > 0)
            {
                if ( (DateTime.Now.Date != admin.keyTime.Date) ||
                    ( admin.privateKey == "") ||
                    (admin.publicKey == ""))
                {
                    List<Pair> parameters = new List<Pair>();
                    parameters.Add(new Pair { key = "private_key", value = RSA.ToXmlString(true) });
                    parameters.Add(new Pair { key = "public_key", value = RSA.ToXmlString(false) });
                    parameters.Add(new Pair { key = "time_key_gen", value = DateTime.Now });
                    conn.update("admins", "login='" + login + "'", parameters);
                    return RSA.ToXmlString(false);
                }
                else
                {
                    RSA.FromXmlString(admin.privateKey);
                    RSA.FromXmlString(admin.privateKey);
                    return RSA.ToXmlString(false);
                }
            }
            return null;
        }
        public bool check()
        {
            SqlConn conn = new SqlConn();
            Admin admin = conn.selectAdmin("admins", "login='" + login + "'");
            if (admin != null)
            {
                if ((DateTime.Now.Date == admin.keyTime.Date) ||
                    (admin.privateKey == "") ||
                    (admin.publicKey == ""))
                {
                    RSA.FromXmlString(admin.privateKey);
                    byte[] paswordBytes = RSA.Decrypt(this.passwordEncript, false);
                    string password = Encoding.Unicode.GetString(paswordBytes);
                    var deriveBytes = new Rfc2898DeriveBytes(password, Convert.FromBase64String(admin.salt), 100);
                    byte[] passwordHashByte = deriveBytes.GetBytes(40);
                    if (Convert.ToBase64String(passwordHashByte) == admin.password.Trim())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static List<Admin> getAdmins(Admin admin)
        {
            if(admin.check())
            {
                SqlConn conn = new SqlConn();
                return conn.selectAdmins();
            }
            return null;
        }
        public static void addFirstAdmin()
        {
            SqlConn conn = new SqlConn();
            byte[] passwordHashByte;
            byte[] saltByte;
            string password = "admin";
            var deriveBytes = new Rfc2898DeriveBytes(password, 40, 100);
            saltByte = deriveBytes.Salt;
            passwordHashByte = deriveBytes.GetBytes(40);
            string passwordHash = "";
            string salt = "";
            passwordHash = Convert.ToBase64String(passwordHashByte);
            salt = Convert.ToBase64String(saltByte);
            List<Pair> parameters = new List<Pair>();
            parameters.Add(new Pair { key = "login", value = "admin" });
            parameters.Add(new Pair { key = "fio", value = "admin" });
            parameters.Add(new Pair { key = "pass_hash", value = passwordHash });
            parameters.Add(new Pair { key = "salt", value = salt });
            parameters.Add(new Pair { key = "private_key", value = "" });
            parameters.Add(new Pair { key = "public_key", value = "" });
            parameters.Add(new Pair { key = "time_key_gen", value = DateTime.Now });
            conn.insert("admins", parameters);
        }
        public static void addDefaultAdmin()
        {
            
        }

    }
}
