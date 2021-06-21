using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Service
{
    public interface ITimeVN
    {
        public static DateTime Now { get; }
    }

    public class TimeVN : ITimeVN
    {
        static TimeZoneInfo timeVNZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        public static DateTime Now()
        {
            return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, timeVNZone); ;
        }
    }
}
