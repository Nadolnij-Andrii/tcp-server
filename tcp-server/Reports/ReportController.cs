using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace tcp_server.Controllers
{
    public class ReportController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        Logger logger = LogManager.GetCurrentClassLogger();
        [HttpPost]
        public  string GetAttractionReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string  j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if(timeWindow.admin.check())
                {
                    AttractionReport attractionReports = new AttractionReport();
                    attractionReports.GetAttractionReport(timeWindow.fromDate, timeWindow.toDate);
                    if (attractionReports.reportedAttractions != null &&
                        attractionReports.attractionTotal != null)
                    {
                        return JsonConvert.SerializeObject(attractionReports);
                    }
                }
                return null;
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
                logger.Error(exc.Message);
                return null;
            }
            
        }
        [HttpPost]
        public string GetCardReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if(timeWindow.admin.check())
                {
                    CardReport cardReport = new CardReport();
                    cardReport.GetCardReport(timeWindow.fromDate, timeWindow.toDate);
                    if (cardReport != null)
                    {
                        return JsonConvert.SerializeObject(cardReport);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string GetOneCardReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TransactionInfo transactionInfo = new TransactionInfo();
                transactionInfo = JsonConvert.DeserializeObject<TransactionInfo>(j);
                if (transactionInfo.timeWindow.admin.check())
                {
                    CardDataReport cardDataReport = new CardDataReport(transactionInfo.timeWindow.fromDate, transactionInfo.timeWindow.toDate, transactionInfo.cardId);
                    if (cardDataReport.cardReport.reportedCards != null && cardDataReport.cardReport.reportedCards.Count > 0)
                    {
                        return JsonConvert.SerializeObject(cardDataReport);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.Message);
                return null;
            }

        }
        [HttpPost]
        public string GetCashierregisterReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if(timeWindow.admin.check())
                {
                    CashierRegisterReport cashierRegisterReport = new CashierRegisterReport();
                    cashierRegisterReport.GetCashierRegisterReport(timeWindow.fromDate, timeWindow.toDate);
                    if (cashierRegisterReport.reportedCashierRegisters != null &&
                        cashierRegisterReport.cashierRegisterTotal != null)
                    {
                        return JsonConvert.SerializeObject(cashierRegisterReport);
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
        public string GetReturnedCardReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if(timeWindow.admin.check())
                {
                    ReturnCardReport returnedCardReport = new ReturnCardReport();
                    returnedCardReport.GetRetunCardReport(timeWindow.fromDate, timeWindow.toDate);
                    if (returnedCardReport.returnedCards != null)
                    {
                        return JsonConvert.SerializeObject(returnedCardReport);
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
        public string GetReplacementCardReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if(timeWindow.admin.check())
                {
                    ReplacementCardReport replacementCardReport = new ReplacementCardReport();
                    replacementCardReport.GetReplacementCardReport(timeWindow.fromDate, timeWindow.toDate);
                    if (replacementCardReport.replacementCards != null)
                    {
                        return JsonConvert.SerializeObject(replacementCardReport);
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
        public string GetTransferCardReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if(timeWindow.admin.check())
                {
                    TransferCardReport transferCardReport = new TransferCardReport();
                    transferCardReport.GetTransferCardReport(timeWindow.fromDate, timeWindow.toDate);
                    if (transferCardReport.transferCards != null)
                    {
                        return JsonConvert.SerializeObject(transferCardReport);
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
        public string GetActivateCardReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if(timeWindow.admin.check())
                {
                    ActivateCardReport activateCardReport = new ActivateCardReport();
                    activateCardReport.GetActivateCardReport(timeWindow.fromDate, timeWindow.toDate);
                    if (activateCardReport.activatedCards != null)
                    {
                        return JsonConvert.SerializeObject(activateCardReport);
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
        public string GetReceivedCardReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if(timeWindow.admin.check())
                {
                    ReceivedCardReport receivedCardReport = new ReceivedCardReport();
                    receivedCardReport.GetReceivedCardReport(timeWindow.fromDate, timeWindow.toDate);
                    if (receivedCardReport.receivedCards != null)
                    {
                        return JsonConvert.SerializeObject(receivedCardReport);
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
        public string GetSellPrizesReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if(timeWindow.admin.check())
                {
                    SellPrizesReport sellPrizesReport = new SellPrizesReport();
                    sellPrizesReport.GetSellPrizesReport(timeWindow.fromDate, timeWindow.toDate);
                    if (sellPrizesReport.sellPrizes != null &&
                        sellPrizesReport.sellPrizesTotal != null)
                    {
                        return JsonConvert.SerializeObject(sellPrizesReport);
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
        public string GetAttendanceStatisticsReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if(timeWindow.admin.check())
                {
                    AttendanceStatisticsReport attendanceStatisticsReport = new AttendanceStatisticsReport();
                    attendanceStatisticsReport.GetAttendanceStatisticsReport(timeWindow.fromDate, timeWindow.toDate);
                    if (attendanceStatisticsReport.registretedCards != null &&
                        attendanceStatisticsReport.usedCards != null &&
                        attendanceStatisticsReport.totalReturn != null &&
                        attendanceStatisticsReport.returnedNewCards != null &&
                        attendanceStatisticsReport.returnedOldCards != null &&
                        attendanceStatisticsReport.totalReturn != null)
                    {
                        return JsonConvert.SerializeObject(attendanceStatisticsReport);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetTransactionsCardReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TransactionInfo transactionInfo = new TransactionInfo();
                transactionInfo = JsonConvert.DeserializeObject<TransactionInfo>(j);
                if(transactionInfo.timeWindow.admin.check())
                {
                    TransactionsCardReport transactionsCardReport = new TransactionsCardReport();
                    transactionsCardReport.GetTransactionsCardReport(
                    transactionInfo.timeWindow.fromDate,
                    transactionInfo.timeWindow.toDate,
                    transactionInfo.cardId);
                    if (transactionsCardReport.reportedTransactions != null)
                    {
                        return JsonConvert.SerializeObject(transactionsCardReport);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetCashierRegisterReportTotal(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if(timeWindow.admin.check())
                {
                    CashierRegisterReportTotal cashierRegisterReportTotal = new CashierRegisterReportTotal();
                    cashierRegisterReportTotal.GetCashierRegisterReportTotal(
                        timeWindow.fromDate,
                        timeWindow.toDate);
                    if (cashierRegisterReportTotal.cahsierRegisterReportedInfos != null)
                    {
                        return JsonConvert.SerializeObject(cashierRegisterReportTotal);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetCashForCardsReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                if (timeWindow.admin.check())
                {
                    CashForCardsReport cashForCardsReport = new CashForCardsReport();
                    cashForCardsReport.GetCashforCardsReport(timeWindow.fromDate, timeWindow.toDate);
                    if (cashForCardsReport.cashForCardReports != null)
                    {
                        return JsonConvert.SerializeObject(cashForCardsReport);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                logger.Error(exc.Message);
                return null;
            }
        }
        [HttpPost]
        public string GetSelectAttractionReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                TransactionInfo transactionInfo = new TransactionInfo();
                transactionInfo = JsonConvert.DeserializeObject<TransactionInfo>(j);
                if (transactionInfo.timeWindow.admin.check())
                {
                    AttractionReport attractionReports = new AttractionReport();
                    attractionReports.GetAttractionReport(transactionInfo.timeWindow.fromDate, transactionInfo.timeWindow.toDate);
                    if (attractionReports.reportedAttractions != null &&
                        attractionReports.attractionTotal != null)
                    {
                        attractionReports.reportedAttractions = attractionReports.reportedAttractions.FindAll(x => x.id == transactionInfo.cardId);
                        return JsonConvert.SerializeObject(attractionReports);
                    }
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
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
                TimeWindows timeWindow = new TimeWindows();
                timeWindow = JsonConvert.DeserializeObject<TimeWindows>(j);
                SqlConn sqlConn = new SqlConn();
                WorkShiftsReport workShiftsReport = new WorkShiftsReport();
                List<WorkShift> wokrShifts = sqlConn.selectWorkShifts("work_shifts", "");
                workShiftsReport.workShifts = wokrShifts.FindAll(x => x.startTime.Date >= timeWindow.fromDate.Date).FindAll(x => x.startTime.Date <= timeWindow.toDate.Date);
                List<WorkShiftInfo> workShiftInfos = sqlConn.selectWorkShiftInfos("work_shifts_info","");
                workShiftsReport.workShiftInfos = new List<WorkShiftInfo>();
                foreach (WorkShift workShift in workShiftsReport.workShifts)
                {
                    foreach(WorkShiftInfo workShiftInfo in workShiftInfos)
                    {
                        if(workShiftInfo.workShiftId == workShift.id)
                        {
                            workShiftsReport.workShiftInfos.Add(workShiftInfo);
                        }
                    }
                }
                if (timeWindow.admin.check())
                {
                        return JsonConvert.SerializeObject(workShiftsReport);
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
        public string GetZReport(HttpRequestMessage request)
        {
            try
            {
                var content = request.Content;
                string j = content.ReadAsStringAsync().Result;
                WorkShiftReport workShiftReportRequest = new WorkShiftReport();
                workShiftReportRequest = JsonConvert.DeserializeObject<WorkShiftReport>(j);
                WorkShiftReport workShiftReportResponce = workShiftReportRequest.GetZReport();
                if (workShiftReportRequest.admin.check())
                {
                    if (workShiftReportResponce.workShift != null && workShiftReportResponce.workShift.id > 0 && workShiftReportResponce.workShiftInfos.Count > 0)
                    {
                        if (workShiftReportResponce.workShiftInfos.FindAll(x => x.workShiftId == workShiftReportResponce.workShift.id).Count > 0)
                        {
                            return JsonConvert.SerializeObject(workShiftReportResponce);
                        }
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
        struct TransactionInfo
        {
            public TimeWindows timeWindow { get; set; }
            public int cardId { get; set; }

            public TransactionInfo (
                TimeWindows timeWindow,
                int cardId
                )
            {
                this.timeWindow = timeWindow;
                this.cardId = cardId;
            }
        }
    }
}
