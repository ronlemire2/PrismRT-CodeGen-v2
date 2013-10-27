using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBrother.DalEF4.Repositories
{
    public static class Extensions
    {
        public static string ToString(this DateTime? dateTime, string format)
        {
            return dateTime.ToString(format, "");
        }

        public static string ToString(this DateTime? dateTime, string format, string returnIfNull)
        {
            if (dateTime.HasValue)
                return dateTime.Value.ToString(format);
            else
                return returnIfNull;
        }

        public static string ToShortDateString(this DateTime? dateTime)
        {
            return dateTime.ToShortDateString("");
        }

        public static string ToShortDateString(this DateTime? dateTime, string returnIfNull)
        {
            if (dateTime.HasValue)
                return dateTime.Value.ToShortDateString();
            else
                return returnIfNull;
        }
    }
}
