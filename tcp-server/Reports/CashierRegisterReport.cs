using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class CashierRegisterReport
    {
        public List<ReportedCashierRegister> reportedCashierRegisters { get; set; }
        public CashierRegisterTotal cashierRegisterTotal { get; set; }
        public void GetCashierRegisterReport(DateTime fromDate, DateTime toDate)
        {
            SqlConn conn = new SqlConn();
            List<TransactionCashRegister> transactionsCashRegister = conn.selectTransactionCashRegister("transactions_cashiermashine",
                       " date BETWEEN '" + fromDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 0:00:00' AND '"
                       + toDate.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture) + " 23:59:59'");
            List<CashierRegister> cashierRegisters = conn.selectCashierRegisters("cashierregister", "");
            this.reportedCashierRegisters = new List<ReportedCashierRegister>();
            decimal allreceivedMoney = 0,
                    allreceivedBonus = 0,
                    allrecievedTicketsFromCashierRegister = 0,
                    allwritenOffMoney = 0,
                    allwritenOffBonuses = 0,
                    allspendTickets = 0,
                    alltotalReceived = 0,
                    allreceivedMoneyForCard = 0,
                    allwritenOffMoneyForCard = 0,
                    allwritenOffTickets = 0;
            foreach (CashierRegister cashierRegister in cashierRegisters)
            {
                decimal receivedMoney = 0,
                    receivedBonus = 0,
                    recievedTicketsFromCashierRegister = 0,
                    writenOffMoney = 0,
                    writenOffBonuses = 0,
                    spendTickets = 0,
                    totalReceived = 0,
                    recievedMoneyForCard = 0,
                    writenOffMoneyForCard = 0,
                    writenOffTickets = 0;
                    
                
                foreach (TransactionCashRegister transactionCashRegister in transactionsCashRegister)
                {
                    if ((int)transactionCashRegister.cashier_register_id == (int)cashierRegister.cashierRegisterId)
                    {

                        if (((int)transactionCashRegister.operation >= 5
                            && (int)transactionCashRegister.operation <= 7)
                            || (int)transactionCashRegister.operation == 18
                            || (int)transactionCashRegister.operation == 19)
                        {
                            receivedMoney += (decimal)transactionCashRegister.summ;
                            receivedBonus += (decimal)transactionCashRegister.bonus;
                            recievedTicketsFromCashierRegister += (int)transactionCashRegister.tickets;
                        }
                        if((int)transactionCashRegister.operation == 22)
                        {
                            recievedMoneyForCard += (decimal)transactionCashRegister.summ;
                        }
                        if ((int)transactionCashRegister.operation == 20)
                        {
                            writenOffMoneyForCard += (decimal)transactionCashRegister.summ;
                        }
                        if ((int)transactionCashRegister.operation == 2)
                        {
                            writenOffMoney += (decimal)transactionCashRegister.summ;
                            writenOffBonuses += (decimal)transactionCashRegister.bonus;
                            writenOffTickets += (decimal)transactionCashRegister.tickets;
                        }
                        if ((int)transactionCashRegister.operation == 9)
                        {
                            spendTickets += (int)transactionCashRegister.tickets;
                        }
                    }
                    
                }
                totalReceived += receivedMoney + recievedMoneyForCard;
                allreceivedMoneyForCard += recievedMoneyForCard;
                alltotalReceived += totalReceived;
                allreceivedMoney += receivedMoney;
                allreceivedBonus += receivedBonus;
                allrecievedTicketsFromCashierRegister += recievedTicketsFromCashierRegister;
                allwritenOffMoney += writenOffMoney;
                allwritenOffBonuses += writenOffBonuses;
                allspendTickets += spendTickets;
                allwritenOffMoneyForCard += writenOffMoneyForCard;
                allwritenOffTickets += writenOffTickets;
                reportedCashierRegisters.Add( new ReportedCashierRegister(
                    (int)cashierRegister.cashierRegisterId,
                    (string)cashierRegister.cashierRegisterIP,
                    totalReceived,
                    receivedMoney,
                    receivedBonus,
                    recievedTicketsFromCashierRegister,
                    writenOffMoney,
                    writenOffBonuses,
                    writenOffTickets,
                    spendTickets,
                    recievedMoneyForCard,
                    writenOffMoneyForCard
                    ));
            }
            this.cashierRegisterTotal = new CashierRegisterTotal(
                    alltotalReceived,
                    allreceivedMoney,
                    allreceivedBonus,
                    allrecievedTicketsFromCashierRegister,
                    allwritenOffMoney,
                    allwritenOffBonuses,
                    allwritenOffTickets,
                    allspendTickets,
                    allreceivedMoneyForCard,
                    allwritenOffMoneyForCard
                    );
        }
        public class ReportedCashierRegister
        {
            public int CashierRegisterId { get; set; }
            public string CashierRegisterIP { get; set; }
            public decimal TotalReceived { get; set; }
            public decimal ReceivedMoney { get; set; }
            public decimal ReceivedBonus { get; set; }
            public decimal RecievedTicketsFromCashierRegister { get; set; }
            public decimal WritenOffMoney { get; set; }
            public decimal WritenOffBonuses { get; set; }
            public decimal WritenOffTickets { get; set; }
            public decimal SpendTickets { get; set; }
            public decimal RecievedMoneyForCard { get; set; }
            public decimal WritenOffMoneyForCard { get; set; }


            public ReportedCashierRegister(
                 int CashierRegisterId,
                 string CashierRegisterIP,
                 decimal TotalReceived,
                 decimal ReceivedMoney,
                 decimal ReceivedBonus,
                 decimal RecievedTicketsFromCashierRegister,
                 decimal WritenOffMoney,
                 decimal WritenOffBonuses,
                 decimal WritenOffTickets,
                 decimal SpendTickets,
                 decimal RecievedMoneyForCard,
                 decimal WritenOffMoneyForCard
                )
            {
                this.CashierRegisterId = CashierRegisterId;
                this.CashierRegisterIP = CashierRegisterIP;
                this.TotalReceived = TotalReceived;
                this.ReceivedMoney = ReceivedMoney;
                this.ReceivedBonus = ReceivedBonus;
                this.RecievedTicketsFromCashierRegister = RecievedTicketsFromCashierRegister;
                this.WritenOffMoney = WritenOffBonuses;
                this.WritenOffBonuses = WritenOffMoney;
                this.WritenOffTickets = WritenOffTickets;
                this.SpendTickets = SpendTickets;
                this.RecievedMoneyForCard = RecievedMoneyForCard;
                this.WritenOffMoneyForCard = WritenOffMoneyForCard;
            }
        }
        public class CashierRegisterTotal
        {
            public decimal AlltotalReceived { get; set; }
            public decimal AllreceivedMoney { get; set; }
            public decimal AllreceivedBonus { get; set; }
            public decimal AllrecievedTicketsFromCashierRegister { get; set; }
            public decimal AllwritenOffMoney { get; set; }
            public decimal AllwritenOffBonuses { get; set; }
            public decimal AllwritenOffTickets { get; set; }
            public decimal AllspendTickets { get; set; }
            public decimal AllreceivedMoneyForCard { get; set; }
            public decimal AllwritenOffMoneyForCard { get; set; }

            public CashierRegisterTotal(
                  decimal AlltotalReceived,
                  decimal AllreceivedMoney,
                  decimal AllreceivedBonus,
                  decimal AllrecievedTicketsFromCashierRegister,
                  decimal AllwritenOffMoney,
                  decimal AllwritenOffBonuses,
                  decimal AllwritenOffTickets,
                  decimal AllspendTickets,
                  decimal AllreceivedMoneyForCard,
                  decimal AllwritenOffMoneyForCard
                )
            {
                this.AlltotalReceived = AlltotalReceived;
                this.AllreceivedMoney = AllreceivedMoney;
                this.AllreceivedBonus = AllreceivedBonus;
                this.AllrecievedTicketsFromCashierRegister = AllrecievedTicketsFromCashierRegister;
                this.AllwritenOffMoney = AllwritenOffMoney;
                this.AllwritenOffBonuses = AllwritenOffBonuses;
                this.AllspendTickets = AllspendTickets;
                this.AllreceivedMoneyForCard = AllreceivedMoneyForCard;
                this.AllwritenOffMoneyForCard = AllwritenOffMoneyForCard;
                this.AllwritenOffTickets = AllwritenOffTickets;
            }
        }
    }
}
