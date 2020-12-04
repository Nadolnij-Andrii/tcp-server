using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tcp_server
{
    public class LoginInfo
    {
        public string cardInfo { get; set; }
        public string IP { get; set; }
        Logger logger = LogManager.GetCurrentClassLogger();
        public LoginInfoResponce loginInfoResponce { get; set; }
        public LoginInfo()
        {

        }
        public LoginInfo(
            string cardInfo,
            string IP,
            LoginInfoResponce loginInfoResponce
            )
        {
            this.cardInfo = cardInfo;
            this.IP = IP;
            this.loginInfoResponce = loginInfoResponce;
        }

        public LoginInfoResponce GetCashierInfo(string cardInfo, string ip, int companyCode)
        {
            try
            {
                
                MatchCollection matches = Regex.Matches(cardInfo, @"([0-9])+");
                var cardId = matches[1].ToString();
                if (matches.Count > 3)
                {
                    if (Card.licenseCheck(cardInfo))
                    {
                        SqlConn conn = new SqlConn();
                        Card currentCard = conn.select("cards", "card_id='" + cardId + "'");
                        if (currentCard != null)
                        {
                            Cashier cashier = conn.selectCashier("cashiers", "card_id='" + currentCard.cardId + "'");
                            if (cashier != null)
                            {
                                CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + ip + "'");
                                if (cashierRegister.id != null)
                                {
                                    List<Pair> parameters = new List<Pair>();
                                    parameters.Add(new Pair { key = "cashierregister_id", value = cashierRegister.cashierRegisterId });
                                    parameters.Add(new Pair { key = "ip", value = cashierRegister.cashierRegisterIP });
                                    parameters.Add(new Pair { key = "cashier_id", value = cashier.cashierCardId });
                                    parameters.Add(new Pair { key = "cashier_name", value = cashier.cashierName });
                                    parameters.Add(new Pair { key = "event", value = "enter" });
                                    parameters.Add(new Pair { key = "time", value = DateTime.Now });
                                    if(conn.insert("cashierregister_event", parameters))
                                    {
                                        WorkShift workShift = conn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");
                                        if(workShift == null)
                                        {

                                        }
                                        else
                                        {

                                        }
                                        this.loginInfoResponce = new LoginInfoResponce(cashier, cashierRegister, null, false);

                                        return this.loginInfoResponce;
                                    }
                                    

                                }
                                else
                                {
                                    throw new Exception("Неверный IP адрес кассы :" + ip);
                                }
                            }
                            else
                            {

                                throw new Exception("Неверная карта кассира :" + cardInfo + " полученная из IP :" + ip);
                            }
                        }
                        else
                        {
                            throw new Exception("Неверная карта кассира :" + cardInfo + " полученная из IP :" + ip);
                        }
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return null;
            }

        }
        private bool licenseCheck(int cardId, int serverNumber,  string cardLicense, int companyCode)
        {
            int codeCardBusiness = companyCode;
            string[] soleMass = new string[] {
                                    "?K ??\u007f(\u0006?? 7 ? (? wS\u001d?h\u0012?\u001e_I ?\u000eWS[; Q? bDH?S ???\u001a",
                                    ";\u001c?twQ ???\u001b????\u000e*6 ?? 94be861f636938ad0a06d917d1f5b1be614678a638ae8",
                                    "´GQá]8n±\u008agâr@\u0012²óæS\a¸daa47aaa2c1f0d660e89fccfc3adc70ca51fc6c1f337c",
                                    "캁倴㑍諾닪瑨쨁쭻\"&m ?? t ?? v ??? l ?\u0019?f ^ M ?\u0014982f0f08d336e1c9041"
                                 };
            int j = 0;
            if (cardId % 2 > 0)
            {
                j = 0;
            }
            else
            {
                j = 1;
            }
            byte[] cardCodeHash;
            byte[] cardIdHash;
            byte[] endHash;
            using (var deriveBytes = new Rfc2898DeriveBytes(codeCardBusiness.ToString(), Encoding.ASCII.GetBytes(soleMass[Math.Abs(0 + j - serverNumber)] + Math.Sin(Math.Log10((int)cardId)).ToString()), 20))
            {
                cardCodeHash = deriveBytes.GetBytes(40);
            }
            using (var deriveBytes = new Rfc2898DeriveBytes((cardId + serverNumber).ToString(), Encoding.ASCII.GetBytes(Math.Sin((int)cardId).ToString() + soleMass[Math.Abs(2 + j)]), 20))
            {
                cardIdHash = deriveBytes.GetBytes(40);
            }
            using (var deriveBytes = new Rfc2898DeriveBytes(cardIdHash, cardCodeHash, 50))
            {
                endHash = deriveBytes.GetBytes(10);
            }

            string newCardLicence = "";
            foreach (var endElement in endHash)
            {
                newCardLicence += endElement.ToString();
            }
            if (cardLicense == newCardLicence.Substring(0, 8).ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public class LoginInfoResponce
        {
            public Cashier cashier { get; set; }
            public CashierRegister cashierRegister { get; set; }
            public WorkShift workShift { get; set; }
            public bool isWokrShiftStarts { get; set; }
            public LoginInfoResponce()
            {

            }
            public LoginInfoResponce(
                Cashier cashier,
                CashierRegister cashierRegister,
                WorkShift workShift,
                bool isWokrShiftStarts
                )
            {
                this.cashier = cashier;
                this.cashierRegister = cashierRegister;
                this.workShift = workShift;
                this.isWokrShiftStarts = isWokrShiftStarts;
            }

        }
        public LoginInfoResponce CheckWorkShiftInfo(string cardInfo, string ip, int companyCode)
        {
            try
            {
                loginInfoResponce = new LoginInfoResponce();
                MatchCollection matches = Regex.Matches(cardInfo, @"([0-9])+");
                var cardId = matches[1].ToString();
                if (matches.Count > 3)
                {
                    if (Card.licenseCheck(cardInfo))
                    {
                        SqlConn conn = new SqlConn();
                        Card currentCard = conn.select("cards", "card_id='" + cardId + "'");
                        if (currentCard != null)
                        {
                            Cashier cashier = conn.selectCashier("cashiers", "card_id='" + currentCard.cardId + "'");
                            if (cashier != null)
                            {
                                CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + ip + "'");
                                if (cashierRegister.id != null)
                                {
                                    this.loginInfoResponce.cashier = cashier;
                                    this.loginInfoResponce.cashierRegister = cashierRegister;
                                    WorkShift workShift = conn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='"+false+"'");
                                    if(workShift.id >0 && workShift.isClosed == false)
                                    {
                                        this.loginInfoResponce.workShift = workShift;
                                        this.loginInfoResponce.isWokrShiftStarts = true;
                                    }
                                    else if (workShift.id > 0 && workShift.isClosed == true)
                                    {
                                        this.loginInfoResponce.workShift = workShift;
                                        this.loginInfoResponce.isWokrShiftStarts = false;
                                    }
                                    else
                                    {
                                        this.loginInfoResponce.workShift = null;
                                        this.loginInfoResponce.isWokrShiftStarts = false;
                                    }
                                    return this.loginInfoResponce;
                                }
                                else
                                {
                                    throw new Exception("Неверный IP адрес кассы :" + ip);
                                }
                            }
                            else
                            {

                                throw new Exception("Неверная карта кассира :" + cardInfo + " полученная из IP :" + ip);
                            }
                        }
                        else
                        {
                            throw new Exception("Неверная карта кассира :" + cardInfo + " полученная из IP :" + ip);
                        }
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return null;
            }

        }
        public LoginInfoResponce OpenWorkShift(string cardInfo, string ip, int companyCode)
        {
            try
            {
                loginInfoResponce = new LoginInfoResponce();
                MatchCollection matches = Regex.Matches(cardInfo, @"([0-9])+");
                var cardId = matches[1].ToString();
                if (matches.Count > 3)
                {
                    if (Card.licenseCheck(cardInfo))
                    {
                        SqlConn conn = new SqlConn();
                        Card currentCard = conn.select("cards", "card_id='" + cardId + "'");
                        if (currentCard != null)
                        {
                            Cashier cashier = conn.selectCashier("cashiers", "card_id='" + currentCard.cardId + "'");
                            if (cashier != null)
                            {
                                CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + ip + "'");
                                if (cashierRegister.id != null)
                                {
                                    this.loginInfoResponce.cashier = cashier;
                                    this.loginInfoResponce.cashierRegister = cashierRegister;
                                    List<WorkShift> workShifts = new List<WorkShift>();
                                    WorkShiftCountInfo workShiftInfo = new WorkShiftCountInfo();
                                    workShifts = conn.selectWorkShifts("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "'");
                                    if (workShifts.Count > 0)
                                    {
                                        foreach (WorkShift workShift in workShifts)
                                        {
                                            workShiftInfo.nonNullableAmount += workShift.revenue;
                                            if (workShift.isClosed)
                                            {
                                                workShiftInfo.closedWorkShift++;
                                            }
                                            else if (workShift.isClosed != true)
                                            {
                                                workShiftInfo.notClosedWorkShift++;
                                            }

                                        }
                                    }
                                    workShifts = conn.selectWorkShifts("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");
                                    if (workShifts.Count == 0)
                                    {
                                        List<Pair> pairs = new List<Pair>();
                                        pairs.Add(new Pair("cashier_mashine_id", loginInfoResponce.cashierRegister.cashierRegisterId));
                                        pairs.Add(new Pair("start_time", DateTime.Now));
                                        //pairs.Add(new Pair("end_time", ));
                                        pairs.Add(new Pair("cash", 0));
                                        pairs.Add(new Pair("cash_count", 0));
                                        pairs.Add(new Pair("cashless_payment", 0));
                                        pairs.Add(new Pair("cashless_payment_count",0));
                                        pairs.Add(new Pair("credit_card", 0));
                                        pairs.Add(new Pair("credit_card_count", 0));
                                        pairs.Add(new Pair("contributions", 0));
                                        pairs.Add(new Pair("contributions_count", 0));
                                        pairs.Add(new Pair("refund", 0));
                                        pairs.Add(new Pair("refund_count", 0));
                                        pairs.Add(new Pair("withdrawal", 0));
                                        pairs.Add(new Pair("withdrawal_count", 0));
                                        pairs.Add(new Pair("revenue", 0));
                                        pairs.Add(new Pair("cash_on_hand", 0));
                                        pairs.Add(new Pair("nonnullable_amount", workShiftInfo.nonNullableAmount));
                                        pairs.Add(new Pair("closed_shifts", workShiftInfo.closedWorkShift));
                                        pairs.Add(new Pair("is_closed", false));
                                        if (conn.insert("work_shifts", pairs))
                                        {
                                            loginInfoResponce.workShift = conn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");
                                            pairs = new List<Pair>();
                                            pairs.Add(new Pair("shift_id", loginInfoResponce.workShift.id));
                                            pairs.Add(new Pair("cashier_mashine_id", loginInfoResponce.cashierRegister.cashierRegisterId));
                                            pairs.Add(new Pair("cashier_id", loginInfoResponce.cashier.cashierId));
                                            pairs.Add(new Pair("cashier_name", loginInfoResponce.cashier.cashierName));
                                            pairs.Add(new Pair("cash", 0));
                                            pairs.Add(new Pair("cash_count", 0));
                                            pairs.Add(new Pair("cashless_payment", 0));
                                            pairs.Add(new Pair("cashless_payment_count", 0));
                                            pairs.Add(new Pair("credit_card", 0));
                                            pairs.Add(new Pair("credit_card_count", 0));
                                            pairs.Add(new Pair("contributions", 0));
                                            pairs.Add(new Pair("contributions_count", 0));
                                            pairs.Add(new Pair("refund", 0));
                                            pairs.Add(new Pair("refund_count", 0));
                                            pairs.Add(new Pair("withdrawal", 0));
                                            pairs.Add(new Pair("withdrawal_count", 0));
                                            pairs.Add(new Pair("revenue", 0));
                                            if(conn.insert("work_shifts_info", pairs))
                                            {
                                                //WorkShift workShift = conn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");
                                                List<Pair> parameters = new List<Pair>();
                                                parameters = new List<Pair>();
                                                parameters.Add(new Pair("shift_id", loginInfoResponce.workShift.id));
                                                parameters.Add(new Pair("operation", 1));
                                                parameters.Add(new Pair("date", DateTime.Now));
                                                parameters.Add(new Pair("cashier_mashine_id", loginInfoResponce.workShift.cashierMashineId));
                                                parameters.Add(new Pair("cashier_id", cashier.cashierId));
                                                parameters.Add(new Pair("cashier_name", cashier.cashierName));
                                                parameters.Add(new Pair("withdrawal", loginInfoResponce.workShift.withdrawal));
                                                parameters.Add(new Pair("contributions", loginInfoResponce.workShift.contributions));
                                                parameters.Add(new Pair("revenue", loginInfoResponce.workShift.revenue));
                                                parameters.Add(new Pair("cash_on_hand", loginInfoResponce.workShift.cashOnHand));
                                                parameters.Add(new Pair("nonnullable_amount", loginInfoResponce.workShift.nonNullableAmount));
                                                if (conn.insert("work_shift_transactions", parameters))
                                                {
                                                    conn.close();
                                                    return this.loginInfoResponce;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Неверный IP адрес кассы :" + ip);
                                }
                            }
                            else
                            {

                                throw new Exception("Неверная карта кассира :" + cardInfo + " полученная из IP :" + ip);
                            }
                        }
                        else
                        {
                            throw new Exception("Неверная карта кассира :" + cardInfo + " полученная из IP :" + ip);
                        }
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return null;
            }

        }
        public LoginInfoResponce ContinueWorkShift(string cardInfo, string ip, int companyCode)
        {
            try
            {
                loginInfoResponce = new LoginInfoResponce();
                MatchCollection matches = Regex.Matches(cardInfo, @"([0-9])+");
                var cardId = matches[1].ToString();
                if (matches.Count > 3)
                {
                    if (Card.licenseCheck(cardInfo))
                    {
                        SqlConn conn = new SqlConn();
                        Card currentCard = conn.select("cards", "card_id='" + cardId + "'");
                        if (currentCard != null)
                        {
                            Cashier cashier = conn.selectCashier("cashiers", "card_id='" + currentCard.cardId + "'");
                            if (cashier != null)
                            {
                                CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + ip + "'");
                                if (cashierRegister.id != null)
                                {
                                    this.loginInfoResponce.cashier = cashier;
                                    this.loginInfoResponce.cashierRegister = cashierRegister;
                                    List<WorkShift> workShifts = new List<WorkShift>();
                                    WorkShiftCountInfo workShiftCountInfo = new WorkShiftCountInfo();
                                    workShifts = conn.selectWorkShifts("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "'");
                                    if (workShifts.Count > 0)
                                    {
                                        foreach (WorkShift workShift in workShifts)
                                        {
                                            workShiftCountInfo.nonNullableAmount += workShift.revenue;
                                            if (workShift.isClosed)
                                            {
                                                workShiftCountInfo.closedWorkShift++;
                                            }
                                            else if (workShift.isClosed != true && workShift.cashierMashineId == (int)cashierRegister.cashierRegisterId)
                                            {
                                                workShiftCountInfo.notClosedWorkShift++;
                                            }

                                        }
                                    }
                                    if (workShiftCountInfo.notClosedWorkShift  == 1)
                                    {
                                        loginInfoResponce.workShift = conn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");
                                        if (loginInfoResponce.workShift != null && loginInfoResponce.workShift.id > 0 && loginInfoResponce.workShift.isClosed != true)
                                        {
                                            List<Pair> parameters = new List<Pair>();
                                            parameters = new List<Pair>();
                                            parameters.Add(new Pair("shift_id", loginInfoResponce.workShift.id));
                                            parameters.Add(new Pair("operation", 7));
                                            parameters.Add(new Pair("date", DateTime.Now));
                                            parameters.Add(new Pair("cashier_mashine_id", loginInfoResponce.workShift.cashierMashineId));
                                            parameters.Add(new Pair("cashier_id", cashier.cashierId));
                                            parameters.Add(new Pair("cashier_name", cashier.cashierName));
                                            parameters.Add(new Pair("withdrawal", loginInfoResponce.workShift.withdrawal));
                                            parameters.Add(new Pair("contributions", loginInfoResponce.workShift.contributions));
                                            parameters.Add(new Pair("revenue", loginInfoResponce.workShift.revenue));
                                            parameters.Add(new Pair("cash_on_hand", loginInfoResponce.workShift.cashOnHand));
                                            parameters.Add(new Pair("nonnullable_amount", loginInfoResponce.workShift.nonNullableAmount));
                                            if (conn.insert("work_shift_transactions", parameters))
                                            {
                                                List<WorkShiftInfo> workShiftInfos = conn.selectWorkShiftInfos("work_shifts_info", "shift_id='" + loginInfoResponce.workShift.id + "'");
                                                if (workShiftInfos.FindAll(x => x.cashierId == (int)loginInfoResponce.cashier.cashierId).ToList().Count == 0)
                                                {
                                                    List<Pair> pairs = new List<Pair>();
                                                    pairs.Add(new Pair("shift_id", loginInfoResponce.workShift.id));
                                                    pairs.Add(new Pair("cashier_mashine_id", loginInfoResponce.cashierRegister.cashierRegisterId));
                                                    pairs.Add(new Pair("cashier_id", loginInfoResponce.cashier.cashierId));
                                                    pairs.Add(new Pair("cashier_name", loginInfoResponce.cashier.cashierName));
                                                    pairs.Add(new Pair("cash", 0));
                                                    pairs.Add(new Pair("cash_count", 0));
                                                    pairs.Add(new Pair("cashless_payment", 0));
                                                    pairs.Add(new Pair("cashless_payment_count", 0));
                                                    pairs.Add(new Pair("credit_card", 0));
                                                    pairs.Add(new Pair("credit_card_count", 0));
                                                    pairs.Add(new Pair("contributions", 0));
                                                    pairs.Add(new Pair("contributions_count", 0));
                                                    pairs.Add(new Pair("refund", 0));
                                                    pairs.Add(new Pair("refund_count", 0));
                                                    pairs.Add(new Pair("withdrawal", 0));
                                                    pairs.Add(new Pair("withdrawal_count", 0));
                                                    pairs.Add(new Pair("revenue", 0));
                                                    if (conn.insert("work_shifts_info", pairs))
                                                    {
                                                        //WorkShift workShift = conn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");

                                                        conn.close();
                                                        return this.loginInfoResponce;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Неверный IP адрес кассы :" + ip);
                                }
                            }
                            else
                            {

                                throw new Exception("Неверная карта кассира :" + cardInfo + " полученная из IP :" + ip);
                            }
                        }
                        else
                        {
                            throw new Exception("Неверная карта кассира :" + cardInfo + " полученная из IP :" + ip);
                        }
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return null;
            }

        }
        public WorkShiftReport GetXReport(string cardInfo, string ip, int companyCode)
        {
            try
            {
                WorkShiftReport workShiftReport = new WorkShiftReport();
                loginInfoResponce = new LoginInfoResponce();
                MatchCollection matches = Regex.Matches(cardInfo, @"([0-9])+");
                var cardId = matches[1].ToString();
                if (matches.Count > 3)
                {
                    if (Card.licenseCheck(cardInfo))
                    {
                        SqlConn conn = new SqlConn();
                        Card currentCard = conn.select("cards", "card_id='" + cardId + "'");
                        if (currentCard != null)
                        {
                            Cashier cashier = conn.selectCashier("cashiers", "card_id='" + currentCard.cardId + "'");
                            if (cashier != null)
                            {
                                CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + ip + "'");
                                if (cashierRegister.id != null)
                                {
                                    SqlConn sqlConn = new SqlConn();

                                    workShiftReport.workShift = sqlConn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");
                                    if (workShiftReport.workShift != null && workShiftReport.workShift.id > 0)
                                    {
                                        workShiftReport.workShiftInfos = sqlConn.selectWorkShiftInfos("work_shifts_info", "shift_id='" + workShiftReport.workShift.id + "'");
                                        if (workShiftReport.workShiftInfos.Count >0 && workShiftReport.workShiftInfos.FindAll(x => x.workShiftId == workShiftReport.workShift.id).Count > 0)
                                        {
                                            return workShiftReport;
                                        }
                                    }
                                    sqlConn.close();
                                }
                            }
                        }
                    }
                }
                return null;
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return null;
            }
        }
        public WorkShiftReport GetZReport(string cardInfo, string ip, int companyCode)
        {
            try
            {
                WorkShiftReport workShiftReport = new WorkShiftReport();
                loginInfoResponce = new LoginInfoResponce();
                MatchCollection matches = Regex.Matches(cardInfo, @"([0-9])+");
                var cardId = matches[1].ToString();
                if (matches.Count > 3)
                {
                    if (Card.licenseCheck(cardInfo))
                    {
                        SqlConn conn = new SqlConn();
                        Card currentCard = conn.select("cards", "card_id='" + cardId + "'");
                        if (currentCard != null)
                        {
                            Cashier cashier = conn.selectCashier("cashiers", "card_id='" + currentCard.cardId + "'");
                            if (cashier != null)
                            {
                                CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + ip + "'");
                                if (cashierRegister.id != null)
                                {
                                    SqlConn sqlConn = new SqlConn();

                                    workShiftReport.workShift = sqlConn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");
                                    if (workShiftReport.workShift != null && workShiftReport.workShift.id > 0)
                                    {
                                        workShiftReport.workShiftInfos = sqlConn.selectWorkShiftInfos("work_shifts_info", "shift_id='" + workShiftReport.workShift.id + "'");
                                        if (workShiftReport.workShiftInfos.Count > 0 && workShiftReport.workShiftInfos.FindAll(x => x.workShiftId == workShiftReport.workShift.id).Count > 0)
                                        {
                                            List<Pair> parameters = new List<Pair>();
                                            parameters.Add(new Pair("end_time", DateTime.Now));
                                            parameters.Add(new Pair("is_closed", true));
                                            if(sqlConn.update("work_shifts", "id='"+ workShiftReport.workShift.id + "' AND cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'", parameters))
                                            {
                                                parameters = new List<Pair>();
                                                parameters.Add(new Pair("shift_id", workShiftReport.workShift.id));
                                                parameters.Add(new Pair("operation", 2));
                                                parameters.Add(new Pair("date", DateTime.Now));
                                                parameters.Add(new Pair("cashier_mashine_id", workShiftReport.workShift.cashierMashineId));
                                                parameters.Add(new Pair("cashier_id", cashier.cashierId));
                                                parameters.Add(new Pair("cashier_name", cashier.cashierName));
                                                parameters.Add(new Pair("withdrawal", workShiftReport.workShift.withdrawal));
                                                parameters.Add(new Pair("contributions", workShiftReport.workShift.contributions));
                                                parameters.Add(new Pair("revenue", workShiftReport.workShift.revenue));
                                                parameters.Add(new Pair("cash_on_hand", workShiftReport.workShift.cashOnHand));
                                                parameters.Add(new Pair("nonnullable_amount", workShiftReport.workShift.nonNullableAmount));
                                                if (sqlConn.insert("work_shift_transactions", parameters))
                                                {
                                                    return workShiftReport; 
                                                }
                                            }
                                        }
                                    }
                                    sqlConn.close();
                                }
                            }
                        }
                    }
                }
                return null; 
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return null;
            }
        }
        public WorkShift WithdrawaCash(string cardInfo, string ip, int companyCode)
        {
            try
            {
                WorkShiftReport workShiftReport = new WorkShiftReport();
                loginInfoResponce = new LoginInfoResponce();
                MatchCollection matches = Regex.Matches(cardInfo, @"([0-9])+");
                var cardId = matches[1].ToString();
                if (matches.Count > 3)
                {
                    if (Card.licenseCheck(cardInfo))
                    {
                        SqlConn conn = new SqlConn();
                        Card currentCard = conn.select("cards", "card_id='" + cardId + "'");
                        if (currentCard != null)
                        {
                            Cashier cashier = conn.selectCashier("cashiers", "card_id='" + currentCard.cardId + "'");
                            if (cashier != null)
                            {
                                CashierRegister cashierRegister = conn.selectCashierRegister("cashierregister", "ip='" + ip + "'");
                                if (cashierRegister.id != null)
                                {
                                    SqlConn sqlConn = new SqlConn();

                                    workShiftReport.workShift = sqlConn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");
                                    if (workShiftReport.workShift != null && workShiftReport.workShift.id > 0)
                                    {
                                        workShiftReport.workShiftInfos = sqlConn.selectWorkShiftInfos("work_shifts_info", "shift_id='" + workShiftReport.workShift.id + "'");
                                        if (workShiftReport.workShiftInfos.Count > 0 && workShiftReport.workShiftInfos.FindAll(x => x.workShiftId == workShiftReport.workShift.id).Count > 0)
                                        {
                                            List<Pair> parameters = new List<Pair>();
                                            parameters.Add(new Pair("cash_on_hand", 0));
                                            parameters.Add(new Pair("withdrawal", workShiftReport.workShift.cashOnHand+ workShiftReport.workShift.withdrawal));
                                            parameters.Add(new Pair("withdrawal_count", ++workShiftReport.workShift.withdrawalCount));

                                            if (sqlConn.update("work_shifts", "id='" + workShiftReport.workShift.id + "' AND cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'", parameters))
                                            {
                                                WorkShiftInfo workShiftInfo = workShiftReport.workShiftInfos.Find(x => x.cashierId ==(int)cashier.cashierId);
                                                parameters = new List<Pair>();
                                                parameters.Add(new Pair("withdrawal", workShiftInfo.withdrawal + workShiftReport.workShift.cashOnHand));
                                                parameters.Add(new Pair("withdrawal_count", ++workShiftInfo.withdrawalCount));
                                                if(sqlConn.update("work_shifts_info", "shift_id='" + workShiftReport.workShift.id + "' AND cashier_id='"+cashier.cashierId+"'", parameters))
                                                {
                                                    parameters = new List<Pair>();
                                                    parameters.Add(new Pair("shift_id", workShiftReport.workShift.id));
                                                    parameters.Add(new Pair("operation", 3));
                                                    parameters.Add(new Pair("date", DateTime.Now));
                                                    parameters.Add(new Pair("cashier_mashine_id", workShiftReport.workShift.cashierMashineId));
                                                    parameters.Add(new Pair("cashier_id", cashier.cashierId));
                                                    parameters.Add(new Pair("cashier_name", cashier.cashierName));
                                                    parameters.Add(new Pair("withdrawal", workShiftReport.workShift.withdrawal + workShiftReport.workShift.cashOnHand ));
                                                    parameters.Add(new Pair("contributions", workShiftReport.workShift.contributions));
                                                    parameters.Add(new Pair("revenue", workShiftReport.workShift.revenue));
                                                    parameters.Add(new Pair("cash_on_hand", 0));
                                                    parameters.Add(new Pair("nonnullable_amount", workShiftReport.workShift.nonNullableAmount));
                                                    if (conn.insert("work_shift_transactions", parameters))
                                                    {
                                                        return sqlConn.selectWorkShift("work_shifts", "cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'");
                                                        
                                                    }
                                                } 
                                            }
                                        }
                                    }
                                    sqlConn.close();
                                }
                            }
                        }
                    }
                }
                return null;
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.ToString());
                return null;
            }
        }
        public class WorkShiftCountInfo
        {
            public decimal revenue { get; set; }
            public decimal cashOnHand { get; set; }
            public decimal nonNullableAmount { get; set; }
            public int closedWorkShift { get; set; }
            public int notClosedWorkShift { get; set; }

            public WorkShiftCountInfo()
            {

            }
            public WorkShiftCountInfo(decimal revenue, 
                 decimal cashOnHand,
                 decimal nonNullableAmount,
                 int closedWorkShift, 
                 int notClosedWorkShift)
            {
                this.revenue = revenue;
                this.cashOnHand = cashOnHand;
                this.nonNullableAmount = nonNullableAmount;
                this.closedWorkShift = closedWorkShift;
                this.notClosedWorkShift = notClosedWorkShift;
            }
        }

    }
}
