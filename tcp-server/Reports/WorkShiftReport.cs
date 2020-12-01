
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tcp_server
{
    public class WorkShiftReport
    {
        public List<WorkShiftInfo> workShiftInfos { get; set; }
        public WorkShift workShift { get; set; }
        public Admin admin { get; set; }
        public WorkShiftReport()
        {

        }
        public WorkShiftReport(
            List<WorkShiftInfo> workShiftInfos,
            WorkShift workShift,
            Admin admin
            )
        {
            this.workShiftInfos = workShiftInfos;
            this.workShift = workShift;
            this.admin = admin;
        }
        public WorkShiftReport GetZReport()
        {
            try
            {
                //WorkShiftReport workShiftReport = new WorkShiftReport();
               

                SqlConn sqlConn = new SqlConn();
                if (this.workShift != null && this.workShift.id > 0)
                {
                    //workShiftReport.workShiftInfos = sqlConn.selectWorkShiftInfos("work_shifts_info", "shift_id='" + workShiftReport.workShift.id + "'");
                    if (this.workShiftInfos.Count > 0 && this.workShiftInfos.FindAll(x => x.workShiftId == this.workShift.id).Count > 0)
                    {
                        List<Pair> parameters = new List<Pair>();
                        parameters.Add(new Pair("end_time", DateTime.Now));
                        parameters.Add(new Pair("is_closed", true));
                        if (sqlConn.update("work_shifts", "id='"+ workShift.id+ "'", parameters))
                        {
                            parameters = new List<Pair>();
                            parameters.Add(new Pair("shift_id", this.workShift.id));
                            parameters.Add(new Pair("operation", 2));
                            parameters.Add(new Pair("date", DateTime.Now));
                            parameters.Add(new Pair("cashier_mashine_id", this.workShift.cashierMashineId));
                            parameters.Add(new Pair("cashier_id", 0));
                            parameters.Add(new Pair("cashier_name", "Смена закрыта администраторм :"+admin.FIO));
                            parameters.Add(new Pair("withdrawal", this.workShift.withdrawal));
                            parameters.Add(new Pair("contributions", this.workShift.contributions));
                            parameters.Add(new Pair("revenue", this.workShift.revenue));
                            parameters.Add(new Pair("cash_on_hand", this.workShift.cashOnHand));
                            parameters.Add(new Pair("nonnullable_amount", this.workShift.nonNullableAmount));
                            if (sqlConn.insert("work_shift_transactions", parameters))
                            {
                                return this;
                            }
                        }
                    }
                    sqlConn.close();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
    
}