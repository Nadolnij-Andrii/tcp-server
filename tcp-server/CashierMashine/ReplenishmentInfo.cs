using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tcp_server
{
    class ReplenishmentInfo
    {
        public string cardInfo { get; set; }
        public decimal cash { get; set; }
        public decimal cashlessPayment { get; set; }
        public decimal creditCard { get; set; }
        public string loginCard { get; set; }
        public string ip { get; set; }
        public ReplenishmentInfo()
        {

        }
        public ReplenishmentInfo(
             string cardInfo,
             decimal cash,
             decimal cashlessPayment,
             decimal creditCard,
             string loginCard,
             string ip
            )
        {
            this.cardInfo = cardInfo;
            this.cash = cash;
            this.cashlessPayment = cashlessPayment;
            this.creditCard = creditCard;
            this.loginCard = loginCard;
            this.ip = ip;
        }
        public WorkShift ContributeCash(string cardInfo, string ip, int companyCode, decimal cash)
        {
            try
            {
                WorkShiftReport workShiftReport = new WorkShiftReport();
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
                                            parameters.Add(new Pair("cash_on_hand", workShiftReport.workShift.cashOnHand + cash));
                                            parameters.Add(new Pair("contributions", cash + workShiftReport.workShift.contributions));
                                            parameters.Add(new Pair("contributions_count", ++workShiftReport.workShift.contributionsCount));

                                            if (sqlConn.update("work_shifts", "id='" + workShiftReport.workShift.id + "' AND cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'", parameters))
                                            {
                                                WorkShiftInfo workShiftInfo = workShiftReport.workShiftInfos.Find(x => x.cashierId == (int)cashier.cashierId);
                                                parameters = new List<Pair>();
                                                parameters.Add(new Pair("contributions", cash + workShiftInfo.contributions));
                                                parameters.Add(new Pair("contributions_count", ++workShiftInfo.contributionsCount));
                                                if (sqlConn.update("work_shifts_info", "shift_id='" + workShiftReport.workShift.id + "' AND cashier_id='" + cashier.cashierId + "'", parameters))
                                                {
                                                    parameters = new List<Pair>();
                                                    parameters.Add(new Pair("shift_id", workShiftReport.workShift.id));
                                                    parameters.Add(new Pair("operation", 3));
                                                    parameters.Add(new Pair("date", DateTime.Now));
                                                    parameters.Add(new Pair("cashier_mashine_id", workShiftReport.workShift.cashierMashineId));
                                                    parameters.Add(new Pair("cashier_id", cashier.cashierId));
                                                    parameters.Add(new Pair("cashier_name", cashier.cashierName));
                                                    parameters.Add(new Pair("withdrawal", workShiftReport.workShift.withdrawal));
                                                    parameters.Add(new Pair("contributions", workShiftReport.workShift.contributions + cash));
                                                    parameters.Add(new Pair("revenue", workShiftReport.workShift.revenue));
                                                    parameters.Add(new Pair("cash_on_hand", workShiftReport.workShift.cashOnHand + cash));
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
                Console.WriteLine(exc.Message);
                return null;
            }
        }
        private bool licenseCheck(int cardId, int serverNumber, string cardLicense, int companyCode)
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
        public WorkShift WithdrawaSomeCash(string cardInfo, string ip, int companyCode, decimal cash)
        {
            try
            {
                WorkShiftReport workShiftReport = new WorkShiftReport();
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
                                            parameters.Add(new Pair("cash_on_hand", workShiftReport.workShift.cashOnHand - cash));
                                            parameters.Add(new Pair("withdrawal", cash + workShiftReport.workShift.withdrawal));
                                            parameters.Add(new Pair("withdrawal_count", ++workShiftReport.workShift.withdrawalCount));

                                            if (sqlConn.update("work_shifts", "id='" + workShiftReport.workShift.id + "' AND cashier_mashine_id='" + cashierRegister.cashierRegisterId + "' AND is_closed='" + false + "'", parameters))
                                            {
                                                WorkShiftInfo workShiftInfo = workShiftReport.workShiftInfos.Find(x => x.cashierId == (int)cashier.cashierId);
                                                parameters = new List<Pair>();
                                                parameters.Add(new Pair("withdrawal", workShiftInfo.withdrawal + cash));
                                                parameters.Add(new Pair("withdrawal_count", ++workShiftInfo.withdrawalCount));
                                                if (sqlConn.update("work_shifts_info", "shift_id='" + workShiftReport.workShift.id + "' AND cashier_id='" + cashier.cashierId + "'", parameters))
                                                {
                                                    parameters = new List<Pair>();
                                                    parameters.Add(new Pair("shift_id", workShiftReport.workShift.id));
                                                    parameters.Add(new Pair("operation", 3));
                                                    parameters.Add(new Pair("date", DateTime.Now));
                                                    parameters.Add(new Pair("cashier_mashine_id", workShiftReport.workShift.cashierMashineId));
                                                    parameters.Add(new Pair("cashier_id", cashier.cashierId));
                                                    parameters.Add(new Pair("cashier_name", cashier.cashierName));
                                                    parameters.Add(new Pair("withdrawal", workShiftReport.workShift.withdrawal + cash));
                                                    parameters.Add(new Pair("contributions", workShiftReport.workShift.contributions));
                                                    parameters.Add(new Pair("revenue", workShiftReport.workShift.revenue));
                                                    parameters.Add(new Pair("cash_on_hand", workShiftReport.workShift.cashOnHand - cash));
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
                Console.WriteLine(exc.Message);
                //logger.Error(exc.Message);
                return null;
            }
        }
    }
}
