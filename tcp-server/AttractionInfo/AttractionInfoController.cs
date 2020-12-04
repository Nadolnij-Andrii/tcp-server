using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace tcp_server.AttractionInfo
{
    public class AttractionInfoController : ApiController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        [HttpPost]
        public string GetKey(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                Admin admin = new Admin();
                admin = JsonConvert.DeserializeObject<Admin>(j);

                string publicKey = admin.checkLogin();
                if (publicKey != null)
                {
                    return publicKey;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public HttpResponseMessage CheckAdmin(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                Admin admin = new Admin();
                admin = JsonConvert.DeserializeObject<Admin>(j);
                if (admin.check())
                {
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
                return null;
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetAttractionInfo(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                Admin admin = new Admin();
                admin = JsonConvert.DeserializeObject<Admin>(j);
                if (admin.check())
                {
                    SqlConn conn = new SqlConn();
                    AttractionsInfo attractionsInfo = new AttractionsInfo();
                    attractionsInfo.GetAttractionsInfo();
                    if (attractionsInfo.Attractions != null &&
                        attractionsInfo.AttractionTypes != null)
                    {
                        return JsonConvert.SerializeObject(attractionsInfo);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string GetCheckedAllAttractionSale(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                Admin admin = new Admin();
                admin = JsonConvert.DeserializeObject<Admin>(j);
                if (admin.check())
                {
                    SqlConn conn = new SqlConn();
                    AttractionsInfoResponce attractionsInfo = new AttractionsInfoResponce();
                    attractionsInfo.attractions = conn.select12SaleSalesInfo("sales_info", "sales_name='1sale'").attractions;
                    attractionsInfo.active = conn.select12SaleSalesInfo("sales_info", "sales_name='1sale'").active;

                        return JsonConvert.SerializeObject(attractionsInfo);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string GetCheckedSoftZoneSale(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                Admin admin = new Admin();
                admin = JsonConvert.DeserializeObject<Admin>(j);
                if (admin.check())
                {
                    SqlConn conn = new SqlConn();
                    AttractionsInfoResponce attractionsInfo = new AttractionsInfoResponce();
                    attractionsInfo.attractions = conn.select12SaleSalesInfo("sales_info", "sales_name='2sale'").attractions;
                    attractionsInfo.active = conn.select12SaleSalesInfo("sales_info", "sales_name='2sale'").active;
                    return JsonConvert.SerializeObject(attractionsInfo);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string GetCheckedSoftZoneTueWedThuSale(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                Admin admin = new Admin();
                admin = JsonConvert.DeserializeObject<Admin>(j);
                if (admin.check())
                {
                    SqlConn conn = new SqlConn();
                    AttractionsInfoResponce attractionsInfo = new AttractionsInfoResponce();
                    attractionsInfo.attractions = conn.select12SaleSalesInfo("sales_info", "sales_name='3sale'").attractions;
                    attractionsInfo.active = conn.select12SaleSalesInfo("sales_info", "sales_name='3sale'").active;
                    return JsonConvert.SerializeObject(attractionsInfo);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string GetCheckedSoftZoneJule(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                Admin admin = new Admin();
                admin = JsonConvert.DeserializeObject<Admin>(j);
                if (admin.check())
                {
                    SqlConn conn = new SqlConn();
                    AttractionsInfoResponce attractionsInfo = new AttractionsInfoResponce();
                    attractionsInfo.attractions = conn.select12SaleSalesInfo("sales_info", "sales_name='4sale'").attractions;
                    attractionsInfo.active = conn.select12SaleSalesInfo("sales_info", "sales_name='4sale'").active;
                    return JsonConvert.SerializeObject(attractionsInfo);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string UpdateAllAttractionSale(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                AttractionInfoRequest attractionInfoRequest = new AttractionInfoRequest();
                attractionInfoRequest = JsonConvert.DeserializeObject<AttractionInfoRequest>(j);
                if (attractionInfoRequest.admin.check())
                {
                    SqlConn conn = new SqlConn();
                    AttractionsInfoResponce attractionsInfoResponce = new AttractionsInfoResponce();
                    List<Pair> pairs = new List<Pair>();
                    pairs.Add(new  Pair( "attractions", JsonConvert.SerializeObject(attractionInfoRequest.attractions)));
                    pairs.Add(new Pair("active", attractionInfoRequest.active));
                    conn.update("sales_info", "sales_name='1sale'", pairs);
                    attractionsInfoResponce.attractions = conn.select12SaleSalesInfo("sales_info", "sales_name='1sale'").attractions;
                    attractionsInfoResponce.active = conn.select12SaleSalesInfo("sales_info", "sales_name='1sale'").active;

                    return JsonConvert.SerializeObject(attractionsInfoResponce);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string UpdateSoftZoneSale(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                AttractionInfoRequest attractionInfoRequest = new AttractionInfoRequest();
                attractionInfoRequest = JsonConvert.DeserializeObject<AttractionInfoRequest>(j);
                if (attractionInfoRequest.admin.check())
                {
                    SqlConn conn = new SqlConn();
                    AttractionsInfoResponce attractionsInfo = new AttractionsInfoResponce();
                    List<Pair> pairs = new List<Pair>();
                    pairs.Add(new Pair("attractions", JsonConvert.SerializeObject(attractionInfoRequest.attractions)));
                    pairs.Add(new Pair("active", attractionInfoRequest.active));
                    conn.update("sales_info", "sales_name='2sale'", pairs);
                    
                    attractionsInfo.attractions = conn.select12SaleSalesInfo("sales_info", "sales_name='2sale'").attractions;
                    attractionsInfo.active = conn.select12SaleSalesInfo("sales_info", "sales_name='2sale'").active;
                    return JsonConvert.SerializeObject(attractionsInfo);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string UpdateSoftZoneTueWedThuSale(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                AttractionInfoRequest attractionInfoRequest = new AttractionInfoRequest();
                attractionInfoRequest = JsonConvert.DeserializeObject<AttractionInfoRequest>(j);
                if (attractionInfoRequest.admin.check())
                {
                    SqlConn conn = new SqlConn();
                    AttractionsInfoResponce attractionsInfo = new AttractionsInfoResponce();
                    List<Pair> pairs = new List<Pair>();
                    pairs.Add(new Pair("attractions", JsonConvert.SerializeObject(attractionInfoRequest.attractions)));
                    pairs.Add(new Pair("active", attractionInfoRequest.active));
                    conn.update("sales_info", "sales_name='3sale'", pairs);

                    attractionsInfo.attractions = conn.select12SaleSalesInfo("sales_info", "sales_name='3sale'").attractions;
                    attractionsInfo.active = conn.select12SaleSalesInfo("sales_info", "sales_name='3sale'").active;
                    return JsonConvert.SerializeObject(attractionsInfo);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string UpdateSoftZoneJule(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                AttractionInfoRequest attractionInfoRequest = new AttractionInfoRequest();
                attractionInfoRequest = JsonConvert.DeserializeObject<AttractionInfoRequest>(j);
                if (attractionInfoRequest.admin.check())
                {
                    SqlConn conn = new SqlConn();
                    AttractionsInfoResponce attractionsInfo = new AttractionsInfoResponce();
                    List<Pair> pairs = new List<Pair>();
                    pairs.Add(new Pair("attractions", JsonConvert.SerializeObject(attractionInfoRequest.attractions)));
                    pairs.Add(new Pair("active", attractionInfoRequest.active));
                    conn.update("sales_info", "sales_name='4sale'", pairs);

                    attractionsInfo.attractions = conn.select12SaleSalesInfo("sales_info", "sales_name='4sale'").attractions;
                    attractionsInfo.active = conn.select12SaleSalesInfo("sales_info", "sales_name='4sale'").active;
                    return JsonConvert.SerializeObject(attractionsInfo);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string GetCashierRegisters(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                Admin admin = new Admin();
                admin = JsonConvert.DeserializeObject<Admin>(j);
                if (admin.check())
                {
                    SqlConn conn = new SqlConn();
                    List<CashierRegister> cashierRegisters = new List<CashierRegister>();
                    cashierRegisters = CashierRegister.getCashierRegister();
                    if (cashierRegisters != null)
                    {
                        return JsonConvert.SerializeObject(cashierRegisters);
                    }
                }
                return null;
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string GetCashiers(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                Admin admin = new Admin();
                admin = JsonConvert.DeserializeObject<Admin>(j);
                if (admin.check())
                {
                    SqlConn conn = new SqlConn();
                    List<Cashier> cashiers = new List<Cashier>();
                    cashiers = Cashier.getCashiers();
                    if (cashiers != null)
                    {
                        return JsonConvert.SerializeObject(cashiers);
                    }
                }
                return null;
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string GetAdmins(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;

                Admin admin = new Admin();
                admin = JsonConvert.DeserializeObject<Admin>(j);
                if (admin.check())
                {
                    SqlConn conn = new SqlConn();
                    List<Admin> admins = new List<Admin>();
                    admins = Admin.getAdmins(admin);
                    if (admins != null)
                    {
                        return JsonConvert.SerializeObject(admins);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public HttpResponseMessage PostNewAttraction(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                AttractionInfo attractionInfo = new AttractionInfo();
                attractionInfo = JsonConvert.DeserializeObject<AttractionInfo>(j);
                if (attractionInfo != null)
                {
                    attractionInfo.attraction.addAttraction();
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public HttpResponseMessage AddCashierMashine(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;

                CashierRegisterInfo cashierRegisterInfo = new CashierRegisterInfo();
                cashierRegisterInfo = JsonConvert.DeserializeObject<CashierRegisterInfo>(j);
                if (cashierRegisterInfo != null)
                {
                    if (cashierRegisterInfo.admin.check())
                    {
                        if (cashierRegisterInfo.cashierRegister.addCashierRegister())
                        {
                            return new HttpResponseMessage(HttpStatusCode.Accepted);
                        }
                        else
                        {
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                    }
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public HttpResponseMessage AddCashier(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CashierInfo cashierInfo = new CashierInfo();
                cashierInfo = JsonConvert.DeserializeObject<CashierInfo>(j);
                if (cashierInfo != null)
                {
                    if (cashierInfo.addCashier())
                    {
                        return new HttpResponseMessage(HttpStatusCode.Accepted);
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                    }

                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public HttpResponseMessage AddAdmin(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;

                AdminInfo adminInfo = new AdminInfo();
                adminInfo = JsonConvert.DeserializeObject<AdminInfo>(j);
                if (adminInfo != null)
                {

                    if (adminInfo.addAdmin(adminInfo.loginedAdmin, adminInfo.admin))
                    {
                        return new HttpResponseMessage(HttpStatusCode.Accepted);
                    }
                    else
                    {
                        HttpError newAdminError = new HttpError("") { { "AdminErrorCode", 1 } };
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, newAdminError);
                    }

                }
                else
                {
                    HttpError newAdminError = new HttpError("") { { "AdminErrorCode", 2 } };
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, newAdminError);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public HttpResponseMessage PostChangesInAttractionInfo(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;

                AttractionInfo attractionInfo = new AttractionInfo();
                attractionInfo = JsonConvert.DeserializeObject<AttractionInfo>(j);
                if (attractionInfo.admin.check() && attractionInfo.attraction != null)
                {
                    attractionInfo.attraction.saveChanges();
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public HttpResponseMessage PostChangesInCashierRegister(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;

                CashierRegisterInfo cashierRegisterInfo = new CashierRegisterInfo();
                cashierRegisterInfo = JsonConvert.DeserializeObject<CashierRegisterInfo>(j);
                if (cashierRegisterInfo.cashierRegister != null)
                {
                    if (cashierRegisterInfo.admin.check() && cashierRegisterInfo.cashierRegister.changeCashierRegister())
                    {
                        return new HttpResponseMessage(HttpStatusCode.Accepted);
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                    }

                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public HttpResponseMessage PostChangesInCashier(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;

                CashierInfo cashierInfo = new CashierInfo();
                cashierInfo = JsonConvert.DeserializeObject<CashierInfo>(j);
                if (cashierInfo.cashier != null)
                {
                    if (cashierInfo.changeCashierInfo())
                    {
                        return new HttpResponseMessage(HttpStatusCode.Accepted);
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                    }
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public HttpResponseMessage ChangeAdmin(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;

                AdminInfo adminInfo = new AdminInfo();
                adminInfo = JsonConvert.DeserializeObject<AdminInfo>(j);
                if (adminInfo != null)
                {
                    if (adminInfo.changeAdmin(adminInfo.loginedAdmin, adminInfo.admin))
                    {
                        return new HttpResponseMessage(HttpStatusCode.Accepted);
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                    }
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetCardInfoFromAdminPanel(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                Admin admin = new Admin();
                admin = JsonConvert.DeserializeObject<Admin>(j);
                if (admin.check())
                {
                    SqlConn sqlConn = new SqlConn();
                    return JsonConvert.SerializeObject(new CardPriceInfo( sqlConn.selectCardStatus("card_state", "state_id='" + 17 + "'"), admin ));
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public HttpResponseMessage ChangeCardPrice(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                CardPriceInfo cardPriceInfo = JsonConvert.DeserializeObject<CardPriceInfo>(j);

                if (cardPriceInfo.ChangeCardPrice())
                {
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetWorkShifts(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                Admin admin = new Admin();
                SqlConn sqlConn = new SqlConn();
                List<WorkShift> workShifts = new List<WorkShift>();
                workShifts = sqlConn.selectWorkShifts();
                if (workShifts.Count() > 0)
                {
                    return JsonConvert.SerializeObject(workShifts);
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
        }
        
    }
}
