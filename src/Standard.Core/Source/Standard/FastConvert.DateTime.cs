using System;
using System.Text;
using System.Globalization;
using Standard.Core;

namespace Standard
{
    partial class FastConvert
    {
        [ThreadStatic]
        private static StringBuilder t_cachedDateStringBuilder;

        private static DateTime s_unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        private static long s_unixEpochTicks = 621355968000000000;

        // --- Serialize string to DateTime ---

        //<#
        //      .SYNOPSIS
        //          Converts the specified string representation of a date and time to its @DateTime equivalent.
        //
        //      .PARAMETER s
        //          A string containing a date and time to convert.
        //
        //      .PARAMETER format
        //          Any formatting that is supported by `DateTime.ParseExact(string, string)`, or one of the following:
        //
        //          - `e`: Epoch format. This is the number of ticks from Jan 1, 1970 (the epoch date). Dates prior to the 
        //            epoch date are represented by negative numbers.
        //
        //          - `E`: Microsoft JavaScript Serializer epoch format. This is the same as `e`, but supports millisecond 
        //            precision only. It also records the OS timezone at the time of serialization, although this information 
        //            is not used when deserializing.
        //
        //      .OUTPUT
        //          The @DateTime value equivalent to the date and time contained in @[s].
        //#>
        public static DateTime ToDateTime(string s, string format)
        {
            return ToDateTime(s, format, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces);
        }

        //<#
        //      .SYNOPSIS
        //          Converts the specified string representation of a date and time to its @DateTimeSet equivalent.
        //
        //      .PARAMETER s
        //          A string containing a date and time to convert.
        //
        //      .PARAMETER format
        //          Any formatting that is supported by `DateTimeOffset.ParseExact(string, string)`, or one of the following:
        //
        //          - `e`: Epoch format. This is the number of ticks from Jan 1, 1970 (the epoch date). Dates prior to the 
        //            epoch date are represented by negative numbers.
        //
        //          - `E`: Microsoft JavaScript Serializer epoch format. This is the same as `e`, but supports millisecond 
        //            precision only. It also records the OS timezone at the time of serialization, although this information 
        //            is not used when deserializing.
        //
        //          - `i`: The format defined by ISO8601. UTC is not used.
        //
        //      .OUTPUT
        //          The @DateTimeSet value equivalent to the date and time contained in @[s].
        //#>
        public static DateTimeOffset ToDateTimeOffset(string s, string format)
        {
            return ToDateTimeOffset(s, format, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces);
        }

        public static DateTime ToDateTime(string s, string format, IFormatProvider provider)
        {
            return ToDateTime(s, format, provider, DateTimeStyles.AllowWhiteSpaces);
        }

        public static DateTimeOffset ToDateTimeOffset(string s, string format, IFormatProvider provider)
        {
            return ToDateTimeOffset(s, format, provider, DateTimeStyles.AllowWhiteSpaces);
        }

        public static DateTime ToDateTime(string s, string format, DateTimeStyles style)
        {
            return ToDateTime(s, format, CultureInfo.CurrentCulture, style);
        }

        public static DateTimeOffset ToDateTimeOffset(string s, string format, DateTimeStyles style)
        {
            return ToDateTimeOffset(s, format, CultureInfo.CurrentCulture, style);
        }

        public static DateTimeOffset ToDateTimeOffset(string s, string format, IFormatProvider provider, DateTimeStyles style)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(s))
                throw new FormatException(string.Format(RS.ArgumentIsEmptyString, nameof(s)));
            if (string.IsNullOrEmpty(format))
                throw new FormatException(string.Format(RS.ArgumentIsEmptyString, nameof(format)));

            TimeSpan offset;
            DateTime dt;
            if (format.Length == 1)
            {
                if (format[0] == 'E')
                    dt = StringToDate(s, null, 'E', out offset, provider, style, true);
                else if (format[0] == 'e')
                    dt = StringToDate(s, null, 'e', out offset, provider, style);
                else if (format[0] == 'i')
                    dt = StringToDate(s, null, 'i', out offset, provider, style);
                else
                    dt = StringToDate(s, format, 'x', out offset, provider, style);
            }
            else
            {
                dt = StringToDate(s, format, 'x', out offset, provider, style);
            }

            return new DateTimeOffset(dt, offset);
        }

        public static DateTime ToDateTime(string s, string format, IFormatProvider provider, DateTimeStyles style)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(s))
                throw new FormatException(string.Format(RS.ArgumentIsEmptyString, nameof(s)));
            if (string.IsNullOrEmpty(format))
                throw new FormatException(string.Format(RS.ArgumentIsEmptyString, nameof(format)));

            TimeSpan offset;
            if (format.Length == 1)
            {
                if (format[0] == 'E')
                    return StringToDate(s, null, 'E', out offset, provider, style, true);
                else if (format[0] == 'e')
                    return StringToDate(s, null, 'e', out offset, provider, style);
                else if (format[0] == 'i')
                    return StringToDate(s, null, 'i', out offset, provider, style);
            }

            return StringToDate(s, format, 'x', out offset, provider, style);
        }

        private static DateTime StringToDate(string value, string dateFormatString, char dateFormat, out TimeSpan offset, IFormatProvider provider, DateTimeStyles style, bool msPrecision = false)
        {
            offset = TimeSpan.Zero;
            DateTime dt;

            if (!string.IsNullOrEmpty(dateFormatString))
                return DateTime.ParseExact(value, dateFormatString, provider, style);

            bool negativeOffset = false;
            int signIndex = -1;

            if (style.HasFlag(DateTimeStyles.AllowWhiteSpaces))
                value = value.Trim();
            else if (style.HasFlag(DateTimeStyles.AllowLeadingWhite))
                value = value.TrimStart();
            else if (style.HasFlag(DateTimeStyles.AllowTrailingWhite))
                value = value.TrimEnd();

            if (style.HasFlag(DateTimeStyles.AssumeLocal) && style.HasFlag(DateTimeStyles.AssumeUniversal))
                throw new ArgumentException(string.Format(RS.InvalidDateTimeStylesCombo, "AssumeLocal", "AssumeUniversal"), nameof(style));
            if (style.HasFlag(DateTimeStyles.AdjustToUniversal) && style.HasFlag(DateTimeStyles.RoundtripKind))
                throw new ArgumentException(string.Format(RS.InvalidDateTimeStylesCombo, "AdjustToUniversal", "RoundtripKind"), nameof(style));

            if (dateFormat == 'e' || dateFormat == 'E')
            {
                if (value == "-62135596800")
                    return DateTime.MinValue;
                else if (value == "253402300800")
                    return DateTime.MaxValue;

                if (value[0] == '-')
                    signIndex = value.Substring(1).LastIndexOf('-');
                else if (value[0] == '+')
                    throw new FormatException(RS.EpochTimeCannotStartWithPlus);
                else
                    signIndex = value.LastIndexOf('-');

                if (signIndex >= 0)
                    negativeOffset = true;
                else
                    signIndex = value.LastIndexOf('+');

                long unixTimestamp = signIndex < 0 ? ToInt64(value) : ToInt64(value.Substring(0, signIndex));
                if (unixTimestamp == -62135596800)
                    return DateTime.MinValue;
                else if (unixTimestamp == 253402300800)
                    return DateTime.MaxValue;

                dt = new DateTime(s_unixEpochTicks, System.DateTimeKind.Utc);

                if (dateFormat == 'E' && msPrecision)
                    dt = dt.AddTicks(unixTimestamp * TimeSpan.TicksPerMillisecond);
                else
                    dt = dt.AddTicks(unixTimestamp);

                // AdjustToUniversal || RoundtripKind => offset = 0; time as utc
                // otherwise                          => offset = oslocal; time as utc convert to oslocal

                if (style.HasFlag(DateTimeStyles.AdjustToUniversal) || 
                    style.HasFlag(DateTimeStyles.RoundtripKind))
                {
                    return dt;
                }
                else
                {
                    offset = GetUtcOffset(DateTime.Now);
                    return dt.ToLocalTime();
                }
            }
            
            if (dateFormat == 'i')
            {
                string dateText = value.Substring(0, 19);

                int diff = value.Length - dateText.Length;
                bool hasOffset = diff > 0;

                if (!hasOffset)
                {
                    // unspecified
                    dt = DateTime.ParseExact(dateText, "s", provider, DateTimeStyles.None);

                    if (style.HasFlag(DateTimeStyles.AdjustToUniversal))
                    {
                        // AdjustToUniversal && AssumeLocal     => offset = 0; time as oslocal convert to utc
                        // AdjustToUniversal && AssumeUniversal => offset = 0; time kind -> utc
                        // AdjustToUniversal only               => offset = oslocal; time as unspecified

                        if (style.HasFlag(DateTimeStyles.AssumeLocal))
                        {
                            return dt.ToUniversalTime();
                        }
                        else if (style.HasFlag(DateTimeStyles.AssumeUniversal))
                        {
                            return new DateTime(dt.Ticks, DateTimeKind.Utc);
                        }
                        else
                        {
                            offset = GetUtcOffset(DateTime.Now);
                            return dt;
                        }
                    }
                    else
                    {
                        // AssumeLocal     => offset = oslocal; time kind -> local
                        // AssumeUniversal => offset = 0; time as utc convert to local
                        // otherwise       => offset = oslocal; time kind unspecified
                        if (style.HasFlag(DateTimeStyles.AssumeLocal))
                        {
                            offset = GetUtcOffset(DateTime.Now);
                            return new DateTime(dt.Ticks, DateTimeKind.Local);
                        }
                        else if (style.HasFlag(DateTimeStyles.AssumeUniversal))
                        {
                            offset = GetUtcOffset(DateTime.Now);
                            return dt.ToLocalTime();
                        }
                        else
                        {
                            offset = GetUtcOffset(DateTime.Now);
                            return dt;
                        }
                    }
                }

                string utcOffsetText;
                if (value[dateText.Length] == '.')
                {
                    if (diff < 2)
                        throw new FormatException(string.Format(RS.IsoDateExpectNumberAfterDot, value));

                    utcOffsetText = value.Substring(dateText.Length + 1, diff - 1);

                    if (utcOffsetText[0] < 48 || utcOffsetText[0] > 57)
                        throw new FormatException(string.Format(RS.IsoDateExpectNumberAfterDot, value));
                }
                else
                {
                    utcOffsetText = value.Substring(dateText.Length, diff);
                    if (diff == 1 && utcOffsetText[0] != 'Z')
                        throw new FormatException(string.Format(RS.IsoDateExpectEndWithZ, value));
                    else if (utcOffsetText[0] != '+' && utcOffsetText[0] != '-')
                        throw new FormatException(string.Format(RS.IsoDateExpectContainPlusMinus, value));
                }

                signIndex = utcOffsetText.IndexOf('-');
                if (signIndex >= 0)
                    negativeOffset = true;
                else
                    signIndex = utcOffsetText.IndexOf('+');

                // Ends with Z
                if (utcOffsetText[utcOffsetText.Length - 1] == 'Z')
                {
                    if (signIndex >= 0)
                        throw new FormatException(string.Format(RS.IsoDateExpectNotBothPlusMinus, value));

                    dt = new DateTime(DateTime.ParseExact(dateText, "s", provider, DateTimeStyles.AdjustToUniversal).Ticks, DateTimeKind.Utc);

                    // 2015-05-31T00:00Z
                    // 2015-05-31T00:00.3123Z
                    if (utcOffsetText.Length > 1)
                        dt = dt.AddTicks(ToInt32(utcOffsetText.Substring(0, utcOffsetText.Length - 1)));

                    if (style.HasFlag(DateTimeStyles.AdjustToUniversal) || style.HasFlag(DateTimeStyles.RoundtripKind))
                    {
                        return dt;
                    }
                    else
                    {
                        offset = GetUtcOffset(DateTime.Now);
                        return dt.ToLocalTime();
                    }
                }

                // Doesn't end with Z and no +/-
                if (signIndex < 0)
                {
                    dt = new DateTime(DateTime.ParseExact(dateText, "s", provider, DateTimeStyles.AdjustToUniversal).Ticks, DateTimeKind.Unspecified);
                    dt = dt.AddTicks(ToInt32(utcOffsetText));

                    if (style.HasFlag(DateTimeStyles.AdjustToUniversal))
                    {
                        if (style.HasFlag(DateTimeStyles.AssumeUniversal))
                        {
                            return new DateTime(dt.Ticks, DateTimeKind.Utc);
                        }
                        else if (style.HasFlag(DateTimeStyles.AssumeLocal))
                        {
                            return dt.ToUniversalTime();
                        }
                        else
                        {
                            offset = GetUtcOffset(DateTime.Now);
                            return dt;
                        }
                    }
                    else
                    {
                        if (style.HasFlag(DateTimeStyles.AssumeUniversal))
                        {
                            offset = GetUtcOffset(DateTime.Now);
                            return dt.ToLocalTime();
                        }
                        else if (style.HasFlag(DateTimeStyles.AssumeLocal))
                        {
                            offset = GetUtcOffset(DateTime.Now);
                            return new DateTime(dt.Ticks, DateTimeKind.Local);
                        }
                        else
                        {
                            offset = GetUtcOffset(DateTime.Now);
                            return dt;
                        }
                    }
                }

                // From here we are sure text has +/- sign and doesn't end with Z

                dt = new DateTime(DateTime.ParseExact(dateText, "s", provider, DateTimeStyles.AssumeLocal).Ticks, DateTimeKind.Local);
                dt = dt.AddTicks(ToInt32(utcOffsetText.Substring(0, signIndex)));

                // hrs and minutes are always positive numbers.
                // we use `negativeOffset` flag to determine whether they should be + or -.
                int hours = utcOffsetText.Length >= (signIndex + 3) ? ToInt32(utcOffsetText.Substring(signIndex + 1, 2)) : 0;
                int minutes = utcOffsetText.Length >= (signIndex + 5) ? ToInt32(utcOffsetText.Substring(signIndex + 3, 2)) : 0;

                // say we are at +0800. ISO8601 time part is the time at +0800.
                // so to get utc, we need to minus 8 hours from that time part.
                // conversely, at -0800, we need to add 8 hours to get to utc.
                // since `hours` and `minutes` are always positive, we only need to flip it
                // when dealing with + offsets.
                if (!negativeOffset)
                {
                    hours *= -1;
                    minutes *= -1;
                }

                // adjust to utc => convert to utc
                // roundtrip     => update offset and return date as local
                // otherwise     => convert to utc and then convert to oslocal
                if (style.HasFlag(DateTimeStyles.AdjustToUniversal))
                {
                    offset = TimeSpan.Zero;
                    return new DateTime(dt.AddHours(hours).AddMinutes(minutes).Ticks, DateTimeKind.Utc);
                }
                else if (style.HasFlag(DateTimeStyles.RoundtripKind))
                {
                    offset = negativeOffset ? new TimeSpan(hours * -1, minutes * -1, 0) : new TimeSpan(hours, minutes, 0);
                    return dt;
                }
                else
                {
                    offset = GetUtcOffset(DateTime.Now);
                    return new DateTime(dt.AddHours(hours).AddMinutes(minutes).Ticks, DateTimeKind.Utc).ToLocalTime();
                }
            }

            throw new FormatException(string.Format(RS.UnsupportedFormatSpecifier, dateFormat));
        }

        // --- /Serialize string to DateTime ---

        // --- Deserialize DateTime to string ---

        public static string ToString(DateTime date, string format)
        {
            return ToString(date, format, DateTimeStyles.AssumeLocal);
        }

        public static string ToString(DateTimeOffset offset, string format)
        {
            return ToString(offset, format, DateTimeStyles.AssumeLocal);
        }

        public static string ToString(DateTime date, string format, DateTimeStyles style)
        {
            if (date == null)
                throw new ArgumentNullException(nameof(date));
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(format))
                throw new FormatException(string.Format(RS.ArgumentIsEmptyString, nameof(format)));

            TimeSpan offset;
            if (date.Kind == DateTimeKind.Utc)
                offset = TimeSpan.Zero;
            else if (date.Kind == DateTimeKind.Unspecified && style.HasFlag(DateTimeStyles.AssumeUniversal))
                offset = TimeSpan.Zero;
            else
                offset = GetUtcOffset(DateTime.Now);

            // E = Microsoft .NET JSON serializer
            // ref: http://www.abstractpath.com/2013/the-ridiculousness-of-microsoft-net-json-dates
            // example: /Date(700000+0500)/
            //
            // The 700000 part is epoch time, but in ms instead of ticks. You need to * TimeSpan.TicksPerMilliseconds before 
            // feeding it to the epoch calculation.
            // +0500 is just the tz of the machine that did the serialization. do not use it when deserializing 
            // bcos the time is already in utc.

            if (format.Length == 1)
            {
                if (format[0] == 'E')
                    return DateToString(date, offset, true, true);
                else if (format[0] == 'e')
                    return DateToEpochTime(date, offset);
                else if (format[0] == 'i')
                    return DateToISOFormat(date, offset, style.HasFlag(DateTimeStyles.AdjustToUniversal));
            }
            
            return date.ToString(format);
        }

        public static string ToString(DateTimeOffset offset, string format, DateTimeStyles style)
        {
            if (offset == null)
                throw new ArgumentNullException(nameof(offset));
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (string.IsNullOrEmpty(format))
                throw new FormatException(string.Format(RS.ArgumentIsEmptyString, nameof(format)));

            if (format.Length == 1)
            {
                if (format[0] == 'E')
                    return DateToString(offset.DateTime, offset.Offset, true, true);
                else if (format[0] == 'e')
                    return DateToEpochTime(offset.DateTime, offset.Offset);
                else if (format[0] == 'i')
                    return DateToISOFormat(offset.DateTime, offset.Offset, style.HasFlag(DateTimeStyles.AdjustToUniversal));
            }
            
            return offset.ToString(format);
        }

        private static string DateToString(DateTime date, TimeSpan offset, bool includeLocalTz, bool msPrecision = false)
        {
            if (date == DateTime.MinValue)
                return "-62135596800";
            else if (date == DateTime.MaxValue)
                return "253402300800";

            int hours = includeLocalTz ? Math.Abs(offset.Hours) : 0;
            int minutes = includeLocalTz ? Math.Abs(offset.Minutes) : 0;
            string offsetText = includeLocalTz 
                ? (string.Concat(
                    offset.Ticks >= 0 ? "+" : "-",
                    hours < 10 ? "0" : string.Empty,
                    hours,
                    minutes < 10 ? "0" : string.Empty,
                    minutes))
                : string.Empty;

            // [utc epoch]
            // -or-
            // [utc epoch][+-hhmm]
            return string.Concat(DateToEpochTime(date, offset, msPrecision), offsetText);
        }

        private static string DateToEpochTime(DateTime date, TimeSpan offset, bool msPrecision = false)
        {
            if (!offset.Equals(TimeSpan.Zero))
                date = date.AddHours(offset.Hours * -1).AddMinutes(offset.Minutes * -1);

            long epochTime = (long)(date - s_unixEpoch).Ticks;
            if (msPrecision)
                epochTime = (long)(epochTime / TimeSpan.TicksPerMillisecond);

            return ToString(epochTime);
        }

        private static string DateToISOFormat(DateTime date, TimeSpan offset, bool toUtcFormat)
        {
            // unlike epoch, we need to change the date.
            if (toUtcFormat && !offset.Equals(TimeSpan.Zero))
                date = date.AddHours(offset.Hours * -1).AddMinutes(offset.Minutes * -1);

            int minute = date.Minute;
            int hour = date.Hour;
            int second = date.Second;
            int totalSeconds = (int)(date.Ticks - (Math.Floor((decimal)date.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond));
            int day = date.Day;
            int month = date.Month;
            int year = date.Year;

            // YYYY-MM-DDTHH:MM:SS.ttttt
            StringBuilder value = (t_cachedDateStringBuilder ?? (t_cachedDateStringBuilder = new StringBuilder(25)))
                .Clear()
                .Append(ToString(year)).Append('-')
                .Append(month < 10 ? "0" : string.Empty).Append(ToString(month)).Append('-')
                .Append(day < 10 ? "0" : string.Empty).Append(ToString(day)).Append('T')
                .Append(hour < 10 ? "0" : string.Empty).Append(ToString(hour)).Append(':')
                .Append(minute < 10 ? "0" : string.Empty).Append(ToString(minute)).Append(':')
                .Append(second < 10 ? "0" : string.Empty).Append(ToString(second)).Append('.')
                .Append(ToString(totalSeconds));

            if (toUtcFormat)
            {
                value.Append('Z');
            }
            else
            {
                int hours = Math.Abs(offset.Hours);
                int minutes = Math.Abs(offset.Minutes);
                value.Append(offset.Ticks >= 0 ? '+' : '-')
                    .Append(hours < 10 ? "0" : string.Empty).Append(ToString(hours))
                    .Append(minutes < 10 ? "0" : string.Empty).Append(ToString(minutes));
            }

            return value.ToString();
        }

        // --- /Deserialize DateTime to string ---

        // --- Helper ---

        private static TimeSpan GetUtcOffset(DateTime date)
        {
#if NETSTANDARD
            return TimeZoneInfo.Local.GetUtcOffset(date);
#else
            return TimeZone.CurrentTimeZone.GetUtcOffset(date);
#endif
        }

        // --- /Helper ---
    }
}
