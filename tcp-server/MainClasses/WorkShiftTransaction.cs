
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tcp_server
{
    public class WorkShiftTransaction
    {
        public int id { get; set; }
        public int workShiftId { get; set; }
        public int operation { get; set; }
        public DateTime Time { get; set; }
        public int cashierMashineId { get; set; }
        public int cashierId { get; set; }
        public string cashierName { get; set; }
        public decimal withdrawal { get; set; }
        public decimal contributions { get; set; }
        public decimal revenue { get; set; }
        public decimal cashOnHand { get; set; }
        public decimal nonNullableAmount { get; set; }
        public WorkShiftTransaction()
        {

        }
        public WorkShiftTransaction(
            int id,
            int workShiftId,
            int operation,
            DateTime Time,
            int cashierMashineId,
            int cashierId,
            string cashierName,
            decimal withdrawal,
            decimal contributions,
            decimal revenue,
            decimal cashOnHand,
            decimal nonNullableAmount
            )
        {
            this.id = id;
            this.workShiftId = workShiftId;
            this.operation = operation;
            this.Time = Time;
            this.cashierMashineId = cashierMashineId;
            this.cashierId = cashierId;
            this.cashierName = cashierName;
            this.withdrawal = withdrawal;
            this.contributions = contributions;
            this.revenue = revenue;
            this.cashOnHand = cashOnHand;
            this.nonNullableAmount = nonNullableAmount;

        }
    }
}