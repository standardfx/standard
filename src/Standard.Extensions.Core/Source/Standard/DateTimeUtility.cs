using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Standard
{
    public static class DateTimeUtility
    {
        private static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        //<#
        //  .SYNOPSIS
        //      Converts a Unix epoch time to @DateTime.
        //
        // .PARAM unixTime
        //      The Unix epoch time, represented by an @Int64.
        //
        // .PARAM msPrecision
        //      If `true`, indicates that @[unixTime] is in milliseconds, otherwise, that @[unixTime] is in seconds.
        //
        // .OUTPUT
        //      The @DateTime representation of @[unixTime].
        //#>
        public static DateTime ToDateTime(long unixTime, bool msPrecision = false)
        {
            return ToDateTimeOffset(unixTime, msPrecision).DateTime;
        }

        //<#
        //  .SYNOPSIS
        //      Converts a Unix epoch time to @DateTimeOffset.
        //
        // .PARAM unixTime
        //      The Unix epoch time, represented by an @Int64.
        //
        // .PARAM msPrecision
        //      If `true`, indicates that @[unixTime] is in milliseconds, otherwise, that @[unixTime] is in seconds.
        //
        // .OUTPUT
        //      The @DateTimeOffset representation of @[unixTime].
        //#>
        public static DateTimeOffset ToDateTimeOffset(long unixTime, bool msPrecision = false)
        {
            if (!msPrecision)
                return UnixEpoch.AddSeconds((long)unixTime);
            else
                return UnixEpoch.AddMilliseconds((long)unixTime);
        }
    }
}
