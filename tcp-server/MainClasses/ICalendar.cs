using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quartz
{
    interface ICalendar
    {
        string Description { get; set; }

        ICalendar CalendarBase { set; get; }

        bool IsTimeIncluded(DateTimeOffset timeUtc);

        DateTime GetNextIncludedTimeUtc(DateTimeOffset timeUtc);

        ICalendar Clone();
    }
}
