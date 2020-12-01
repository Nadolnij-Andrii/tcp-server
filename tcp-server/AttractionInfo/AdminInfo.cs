using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tcp_server
{
    class AdminInfo
    {
        public Admin admin { get; set; }
        public Admin loginedAdmin { get; set; }
        public AdminInfo()
        {
        }
        public AdminInfo(
            Admin admin,
            Admin loginedAdmin
            )
        {
            this.admin = admin;
            this.loginedAdmin = loginedAdmin;
        }
        public bool addAdmin(Admin loginedAdmin, Admin changedAdmin)
        {
            SqlConn conn = new SqlConn();
            if (loginedAdmin.check() && conn.selectAdmin("admins", "login='" + changedAdmin.login + "'") == null)
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

                loginedAdmin.privateKey = conn.selectAdmin("admins", "login='" + loginedAdmin.login + "'").privateKey;
                loginedAdmin.keyTime = conn.selectAdmin("admins", "login='" + loginedAdmin.login + "'").keyTime;
                if ((DateTime.Now - loginedAdmin.keyTime).Days <= 1)
                {
                    RSA.FromXmlString(loginedAdmin.privateKey);
                    byte[] paswordBytes = RSA.Decrypt(admin.passwordEncript, false);
                    byte[] passwordHashByte;
                    byte[] saltByte;
                    string password = Encoding.Unicode.GetString(paswordBytes);
                    var deriveBytes = new Rfc2898DeriveBytes(password, 40, 100);
                    saltByte = deriveBytes.Salt;
                    passwordHashByte = deriveBytes.GetBytes(40);
                    string passwordHash = "";
                    string salt = "";
                    passwordHash = Convert.ToBase64String(passwordHashByte);
                    salt = Convert.ToBase64String(saltByte);
                    List<Pair> parameters = new List<Pair>();
                    parameters.Add(new Pair { key = "login", value = admin.login });
                    parameters.Add(new Pair { key = "fio", value = admin.FIO });
                    parameters.Add(new Pair { key = "pass_hash", value = passwordHash });
                    parameters.Add(new Pair { key = "salt", value = salt });
                    parameters.Add(new Pair { key = "private_key", value = "" });
                    parameters.Add(new Pair { key = "public_key", value = "" });
                    parameters.Add(new Pair { key = "time_key_gen", value = DateTime.Now });
                    if (conn.insert("admins", parameters))
                    {
                        Admin addedAdmin = conn.selectAdmin("admins", "login='" + changedAdmin.login + "'");
                        if (changedAdmin.cardInfo != null && changedAdmin.cardInfo != "" && Card.licenseCheck(changedAdmin.cardInfo))
                        {
                            var matches = Regex.Matches(changedAdmin.cardInfo, @"([0-9])+");
                            string cardId = matches[1].ToString();
                            if (conn.selectCard("cards", "card_id='" + cardId + "'") == null)
                            {
                                Card card = new Card();
                                card = Card.registerAdminCard(changedAdmin.cardInfo, changedAdmin.FIO);
                                if (card != null && card.id > 0)
                                {
                                    parameters = new List<Pair>();
                                    parameters.Add(new Pair { key = "admin_id", value = addedAdmin.id });
                                    parameters.Add(new Pair { key = "card_id", value = cardId });
                                    if (conn.insert("admin_cards", parameters))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool changeAdmin(Admin loginedAdmin, Admin changedAdmin)
        {
            SqlConn conn = new SqlConn();
            if (loginedAdmin.check() && conn.selectAdmin("admins", "id='" + changedAdmin.id + "'") != null)
            {
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                loginedAdmin.privateKey = conn.selectAdmin("admins", "login='" + loginedAdmin.login + "'").privateKey;
                Admin admin = conn.selectAdmin("admins", "id='" + changedAdmin.id + "'");
                RSA.FromXmlString(loginedAdmin.privateKey);
                List<Pair> parameters = new List<Pair>();
                if (changedAdmin.login == admin.login)
                {
                    parameters.Add(new Pair { key = "login", value = admin.login });
                }
                else
                {
                    parameters.Add(new Pair { key = "login", value = changedAdmin.login });
                }
                parameters.Add(new Pair { key = "fio", value = changedAdmin.FIO });
                if (changedAdmin.password != null && changedAdmin.password != "")
                {
                    byte[] paswordBytes = RSA.Decrypt(changedAdmin.passwordEncript, false);
                    byte[] passwordHashByte;

                    string password = Encoding.Unicode.GetString(paswordBytes);
                    var deriveBytes = new Rfc2898DeriveBytes(password, Convert.FromBase64String(admin.salt), 100);
                    passwordHashByte = deriveBytes.GetBytes(40);
                    string passwordHash = "";
                    passwordHash = Convert.ToBase64String(passwordHashByte);

                    if (passwordHash != admin.password)
                    {
                        var newDeriveBytes = new Rfc2898DeriveBytes(password, 40, 100);
                        byte[] newSaltByte = newDeriveBytes.Salt;
                        byte[] newPasswordHashByte = newDeriveBytes.GetBytes(40);
                        string newPasswordHash = "";
                        string newSalt = "";
                        newPasswordHash = Convert.ToBase64String(newPasswordHashByte);
                        newSalt = Convert.ToBase64String(newSaltByte);
                        parameters.Add(new Pair { key = "pass_hash", value = newPasswordHash });
                        parameters.Add(new Pair { key = "salt", value = newSalt });
                    }
                    else
                    {
                        parameters.Add(new Pair { key = "pass_hash", value = passwordHash });
                        parameters.Add(new Pair { key = "salt", value = admin.salt });
                    }
                }
                if(conn.update("admins", "id='" + changedAdmin.id + "'", parameters))
                {
                    if (changedAdmin.cardInfo != null && changedAdmin.cardInfo != "" && Card.licenseCheck(changedAdmin.cardInfo))
                    {
                        var matches = Regex.Matches(changedAdmin.cardInfo, @"([0-9])+");
                        string cardId = matches[1].ToString();
                        if (conn.selectCard("cards", "card_id='" + cardId + "'") == null)
                        {
                            Card card = new Card();
                            card = Card.registerAdminCard(changedAdmin.cardInfo, changedAdmin.FIO);
                            
                            if (card != null && card.id > 0)
                            {
                                AdminCard adminCard = new AdminCard();
                                adminCard = conn.getAdminCard("admin_id='"+ changedAdmin.id+"'");
                                if (adminCard != null && adminCard.id > 0)
                                {
                                    parameters = new List<Pair>();
                                    parameters.Add(new Pair { key = "card_id", value = cardId });
                                    if (conn.update("admin_cards", "admin_id='" + changedAdmin.id + "'", parameters))
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    parameters = new List<Pair>();
                                    parameters.Add(new Pair { key = "admin_id", value = changedAdmin.id });
                                    parameters.Add(new Pair { key = "card_id", value = cardId });
                                    if (conn.insert("admin_cards", parameters))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
