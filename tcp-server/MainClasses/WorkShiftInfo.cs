
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tcp_server
{
    public class WorkShiftInfo
    {
            public int id { get; set; }
            public int workShiftId { get; set; }
            public int cashierMashineId { get; set; }
            public int cashierId { get; set; }
            public string cashierName { get; set; }
            public decimal cash { get; set; }
            public int cashCount { get; set; }
            public decimal cashlessPayment { get; set; }
            public int cashlessPaymentCount { get; set; }
            public decimal creditCard { get; set; }
            public int creditCardCount { get; set; }
            public decimal contributions { get; set; }
            public int contributionsCount { get; set; }
            public decimal refund { get; set; }
            public int refundCount { get; set; }
            public decimal withdrawal { get; set; }
            public int withdrawalCount { get; set; }
            public decimal revenue { get; set; }
        public WorkShiftInfo()
        {

        }
        public WorkShiftInfo(
            int id,
            int workShiftId,
            int cashierMashineId,
            int cashierId,
            string cashierName,
            decimal cash, 
            int cashCount,
            decimal cashlessPayment,
            int cashlessPaymentCount,
            decimal creditCard,
            int creditCardCount,
            decimal contributions,
            int contributionsCount,
            decimal refund,
            int refundCount,
            decimal withdrawal,
            int withdrawalCount,
            decimal revenue
            )
        {
            this.id = id;
            this.workShiftId = workShiftId;
            this.cashierMashineId = cashierMashineId;
            this.cashierId = cashierId;
            this.cashierName = cashierName;
            this.cash = cash;
            this.cashCount = cashCount;
            this.cashlessPayment = cashlessPayment;
            this.creditCard = creditCard;
            this.creditCardCount = creditCardCount;
            this.contributions = contributions;
            this.contributionsCount = contributionsCount;
            this.refund = refund;
            this.refundCount = refundCount;
            this.withdrawal = withdrawal;
            this.withdrawalCount = withdrawalCount;
            this.revenue = revenue;
        }
    }
}