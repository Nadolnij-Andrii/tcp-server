using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    public class WorkShiftsReport
    {
        public List<WorkShift> workShifts { get; set; }
        public List<WorkShiftInfo> workShiftInfos { get; set; }
        public WorkShiftsReport()
        {

        }
        public WorkShiftsReport(List<WorkShift> workShifts,
            List<WorkShiftInfo> workShiftInfos)
        {

        }
    }
}
