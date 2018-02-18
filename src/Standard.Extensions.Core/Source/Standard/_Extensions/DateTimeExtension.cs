using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Standard
{
    public static class DateTimeExtension
    {
        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            if (dateTime == null)   
                throw new ArgumentNullException(nameof(dateTime));

            return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        }

        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            if (dateTime == null)   
                throw new ArgumentNullException(nameof(dateTime));

            return ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds();
        }
    }
}
