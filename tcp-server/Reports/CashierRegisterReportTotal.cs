using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class CashierRegisterReportTotal
    {
        public List<CahsierRegisterReportedInfo> cahsierRegisterReportedInfos { get; set; }
        public void GetCashierRegisterReportTotal(DateTime fromDate, DateTime toDate)
        {
            SqlConn conn = new SqlConn();
            List<CashierRegister> cashierRegisters = conn.selectCashierRegisters("cashierregister", "");
            List<TransactionCashRegister> transactionsCashRegisters = conn.selectTransactionCashRegister("transactions_cashiermashine", " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                       + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            //List<CashierRegistersInfo> cashierRegistersInfos = new List<CashierRegistersInfo>();
            this.cahsierRegisterReportedInfos = new List<CahsierRegisterReportedInfo>();
            decimal income = 0,
                    transfer = 0,
                    transferTickets = 0,
                    transferedTickets = 0,
                    addedTickets = 0,
                    writeoffTickets = 0,
                    addedBonuses = 0,
                    returnedSumm = 0,
                    returnedMoneyForCard = 0,
                    incomeMoneyForCard = 0;
                   
            int incomeCount = 0, 
                transferCount = 0,
                transferTicketsCount = 0,
                transferedTicketsCount = 0,
                addedTicketsCount = 0,
                writeoffTicketsCount = 0,
                addedBonusesCount = 0, 
                returnedSummCount = 0,
                returnedMoneyForCardCount = 0,
                incomeMoneyForCardCount = 0;
            foreach (CashierRegister cashierRegister in cashierRegisters)
            {
                income = 0;
                incomeCount = 0;
                transfer = 0;
                transferCount = 0;
                transferTickets = 0;
                transferTicketsCount = 0;
                transferedTickets = 0;
                transferedTicketsCount = 0;
                addedTickets = 0;
                addedTicketsCount = 0;
                writeoffTickets = 0;
                writeoffTicketsCount = 0;
                addedBonuses = 0;
                addedBonusesCount = 0;
                returnedSumm = 0;
                returnedSummCount = 0;
                returnedMoneyForCard = 0;
                returnedMoneyForCardCount = 0;
                incomeMoneyForCardCount = 0;
                incomeMoneyForCard = 0;
                foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegisters)
                {
                    if ((int)transactionCashRegister.cashier_register_id == (int)cashierRegister.cashierRegisterId)
                    {

                        switch ((int)transactionCashRegister.operation)
                        {
                            case 11:
                            case 13:
                                transfer += (decimal)transactionCashRegister.summ;
                                transferCount++;
                                if ((int)transactionCashRegister.tickets > 0)
                                {
                                    transferTicketsCount++;
                                    transferTickets += (int)transactionCashRegister.tickets;
                                }
                                break;
                            case 12:
                            case 14:
                                if ((int)transactionCashRegister.tickets > 0)
                                {
                                    transferedTicketsCount++;
                                    transferedTickets += (int)transactionCashRegister.tickets;
                                }
                                break;
                            case 5:
                                if ((decimal)transactionCashRegister.summ > 0)
                                {
                                    income += (decimal)transactionCashRegister.summ;
                                    incomeCount++;
                                }
                                break;
                            case 7:
                                if ((int)transactionCashRegister.tickets > 0)
                                {
                                    addedTickets += (int)transactionCashRegister.tickets;
                                    addedTicketsCount++;
                                }
                                break;
                            case 9:
                                if ((int)transactionCashRegister.tickets > 0)
                                {
                                    writeoffTickets += (int)transactionCashRegister.tickets;
                                    writeoffTicketsCount++;
                                }
                                break;
                            case 6:
                                if ((decimal)transactionCashRegister.bonus > 0)
                                {
                                    addedBonuses += (decimal)transactionCashRegister.bonus;
                                    addedBonusesCount++;
                                }
                                break;
                            case 2:
                                if ((decimal)transactionCashRegister.summ > 0)
                                {
                                    returnedSumm += (decimal)transactionCashRegister.summ;
                                    returnedSummCount++;
                                }
                                break;
                            case 20:
                                if ((decimal)transactionCashRegister.summ > 0)
                                {
                                    returnedMoneyForCard += (decimal)transactionCashRegister.summ;
                                    returnedMoneyForCardCount++;
                                }
                                break;
                            case 22:
                                if ((decimal)transactionCashRegister.summ > 0)
                                {
                                    incomeMoneyForCard += (decimal)transactionCashRegister.summ;
                                    incomeMoneyForCardCount++;
                                }
                                break;
                        };
                    }
                }
                cahsierRegisterReportedInfos.Add(new CahsierRegisterReportedInfo(
                    (int)cashierRegister.id,
                    income,
                    incomeCount,
                    transfer,
                    transferCount,
                    transferTickets,
                    transferTicketsCount,
                    transferedTickets,
                    transferedTicketsCount,
                    addedTickets,
                    addedTicketsCount,
                    writeoffTickets,
                    writeoffTicketsCount,
                    addedBonuses,
                    addedBonusesCount,
                    returnedSumm,returnedSummCount,
                    returnedMoneyForCard,
                    returnedMoneyForCardCount,
                    incomeMoneyForCardCount,
                    incomeMoneyForCard));
                //    cashierRegistersInfos.Add(new CashierRegistersInfo
                //    {
                //        income = income,
                //        returned = returnedSumm,
                //        points = income + transfer,
                //        bonuses = addedBonuses,
                //        ticketsIncome = transferedTickets + addedTickets,
                //        ticketsWritenOff = transferTickets + writeoffTickets,
                //        transactionsCount = (transferTicketsCount +  transferedTicketsCount + transferCount + incomeCount + addedTicketsCount + writeoffTicketsCount + addedBonusesCount + returnedSummCount)
                //    });
                //    this.cahsierRegisterReportedInfos.Add(new CahsierRegisterReportedInfo(
                //         cashierRegister.cashierRegisterId.ToString(),
                //        "Итого по кассе",
                //        income,
                //        returnedSumm,
                //        income + transfer,
                //        addedBonuses,
                //        transferedTickets + addedTickets,
                //        transferTickets + writeoffTickets,
                //        (transferTicketsCount
                //            + transferedTicketsCount
                //            + transferCount
                //            + incomeCount
                //            + addedTicketsCount
                //            + writeoffTicketsCount
                //            + addedBonusesCount
                //            + returnedSummCount)));

                //}
                //int transactionsCount = 0;
                //income = 0;
                //transfer = 0;
                //transferTickets = 0;
                //transferedTickets = 0;
                //addedTickets = 0;
                //writeoffTickets = 0;
                //addedBonuses = 0;
                //returnedSumm = 0;
                //foreach (CashierRegistersInfo cashierRegistersInfo in cashierRegistersInfos)
                //{
                //    income += cashierRegistersInfo.income;
                //    addedTickets += cashierRegistersInfo.points;
                //    returnedSumm += cashierRegistersInfo.returned;
                //    transferTickets += cashierRegistersInfo.ticketsIncome;
                //    transferedTickets += cashierRegistersInfo.ticketsWritenOff;
                //    addedBonuses += cashierRegistersInfo.bonuses;
                //    transactionsCount += cashierRegistersInfo.transactionsCount;
                //}
                //this.cahsierRegisterReportedInfos.Add(new CahsierRegisterReportedInfo("Итого", "-", income, returnedSumm, addedTickets, addedBonuses, transferTickets, transferedTickets, transactionsCount));
            }
        }
        

        public class CahsierRegisterReportedInfo
        {
            public int cahsierRegisterId { get; set; }
            public decimal income { get; set; }
            public int incomeCount { get; set; }
            public decimal transfer { get; set; }
            public int transferCount { get; set; }
            public decimal transferTickets { get; set; }
            public int transferTicketsCount { get; set; }
            public decimal transferedTickets { get; set; }
            public int transferedTicketsCount { get; set; }
            public decimal addedTickets { get; set; }
            public int addedTicketsCount { get; set; }
            public decimal writeoffTickets { get; set; }
            public int writeoffTicketsCount { get; set; }
            public decimal addedBonuses { get; set; }
            public int addedBonusesCount { get; set; }
            public decimal returnedSumm { get; set; }
            public int returnedSummCount { get; set; }
            public decimal returnedMoneyForCard { get; set; }
            public int returnedMoneyForCardCount { get; set; }
            public int incomeMoneyForCardCount { get; set; }
            public decimal incomeMoneyForCard { get; set; }
            public CahsierRegisterReportedInfo()
            {

            }
            public CahsierRegisterReportedInfo(
                int cahsierRegisterId,
                decimal income,
                int incomeCount,
                decimal transfer,
                int transferCount,
                decimal transferTickets,
                int transferTicketsCount,
                decimal transferedTickets,
                int transferedTicketsCount,
                decimal addedTickets,
                int addedTicketsCount,
                decimal writeoffTickets,
                int writeoffTicketsCount,
                decimal addedBonuses,
                int addedBonusesCount,
                decimal returnedSumm,
                int returnedSummCount,
                decimal returnedMoneyForCard,
                int returnedMoneyForCardCount,
                int incomeMoneyForCardCount,
                decimal incomeMoneyForCard
                )
            {
                this.cahsierRegisterId = cahsierRegisterId;
                this.income = income;
                this.incomeCount = incomeCount;
                this.transfer = transfer;
                this.transferCount = transferCount;
                this.transferTickets = transferTickets;
                this.transferTicketsCount = transferedTicketsCount;
                this.transferedTickets = transferedTickets;
                this.transferedTicketsCount = transferedTicketsCount;
                this.addedTickets = addedTickets;
                this.addedTicketsCount = addedTicketsCount;
                this.writeoffTickets = writeoffTickets;
                this.writeoffTicketsCount = writeoffTicketsCount;
                this.addedBonuses = addedBonuses;
                this.addedBonusesCount = addedBonusesCount;
                this.returnedSumm = returnedSumm;
                this.returnedSummCount = returnedSummCount;
                this.returnedMoneyForCard = returnedMoneyForCard;
                this.returnedMoneyForCardCount = returnedMoneyForCardCount;
                this.incomeMoneyForCardCount = incomeMoneyForCardCount;
                this.incomeMoneyForCard = incomeMoneyForCard;
            }
        }
    }
}
