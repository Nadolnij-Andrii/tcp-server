using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tcp_server
{
    public class WorkShift
    {
        public int id { get; set; }
        public int cashierMashineId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
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
        public decimal cashOnHand { get; set; }
        public decimal nonNullableAmount { get; set; }
        public int closedShifts { get; set; }
        public bool isClosed { get; set; }
        public WorkShift()
        {

        }
            public WorkShift
                (int id ,
                int cashierMashineId ,
                DateTime startTime ,
                DateTime endTime ,
                decimal cash ,
                int cashCount ,
                decimal cashlessPayment ,
                int cashlessPaymentCount ,
                decimal creditCard ,
                int creditCardCount ,
                decimal contributions ,
                int contributionsCount ,
                decimal refund ,
                int refundCount ,
                decimal withdrawal ,
                int withdrawalCount ,
                decimal revenue ,
                decimal cashOnHand ,
                decimal nonNullableAmount ,
                int closedShifts ,
                bool isClosed)
        {
            this.id = id;
            this.cashierMashineId = cashierMashineId;
            this.startTime = startTime;
            this.endTime = endTime;
            this.cash = cash;
            this.cashCount = cashCount;
            this.cashlessPayment = cashlessPayment;
            this.cashlessPaymentCount = cashlessPaymentCount;
            this.creditCard = creditCard;
            this.creditCardCount = creditCardCount;
            this.contributions = contributions;
            this.contributionsCount = contributionsCount;
            this.refund = refund;
            this.refundCount = refundCount;
            this.withdrawal = withdrawal;
            this.withdrawalCount = withdrawalCount;
            this.revenue = revenue;
            this.cashOnHand = cashOnHand;
            this.nonNullableAmount = nonNullableAmount;
            this.closedShifts = closedShifts;
            this.isClosed = isClosed;
        }
    }
}