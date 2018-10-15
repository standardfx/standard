using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// Basic data type for a Json+ element. All elements in Json+ inherits from this class.
    /// </summary>
    public class JPlusValue : IJPlusObjectCandidate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusValue"/> class.
        /// </summary>
        public JPlusValue()
        {
            Values = new List<IJPlusElement>();
        }

        /// <summary>
        /// Returns true if this element does not contain any value.
        /// </summary>
        public bool IsEmpty
        {
            get { return Values.Count == 0; }
        }

        /// <summary>
        /// The list of children elements contained inside this <see cref="JPlusValue"/>.
        /// </summary>
        public List<IJPlusElement> Values { get; private set; }

        /// <summary>
        /// Wraps this <see cref="JPlusValue"/> into a new <see cref="JPlusContext"/> object at the specified key.
        /// </summary>
        /// <param name="key">The key designated to be the new root element.</param>
        /// <returns>A <see cref="JPlusContext"/> with the given key as the root element.</returns>
        public JPlusContext AtKey(string key)
        {
            JPlusObject o = new JPlusObject();
            o.GetOrCreateKey(key);
            o.Items[key] = this;
            JPlusValue r = new JPlusValue();
            r.Values.Add(o);
            return new JPlusContext(new JPlusRoot(r));
        }

        /// <summary>
        /// The list of properties for a <see cref="JPlusValue"/> that is also a <see cref="JPlusObject"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">This <see cref="JPlusValue" /> is not a <see cref="JPlusObject"/>.</exception>
        public List<string> GetKeys()
        {
            JPlusObject jObject = GetObject();
            if (jObject == null)
                throw new InvalidOperationException(RS.Err_GetKeysFromNonObject);

            return jObject.Items.Keys.ToList();
        }

        /// <see cref="IJPlusObjectCandidate.GetObject()"/>
        public JPlusObject GetObject()
        {
            //TODO: merge objects?
            IJPlusElement raw = Values.FirstOrDefault();
            JPlusObject o = raw as JPlusObject;
            IJPlusObjectCandidate sub = raw as IJPlusObjectCandidate;

            if (o != null) 
                return o;

            if (sub != null && sub.IsObject()) 
                return sub.GetObject();

            return null;
        }

        /// <see cref="IJPlusObjectCandidate.IsObject()"/>
        public bool IsObject()
        {
            return GetObject() != null;
        }

        /// <summary>
        /// Adds the specified element to  this <see cref="JPlusValue"/> as a child item.
        /// </summary>
        /// <param name="value">The element to add.</param>
        public void AppendValue(IJPlusElement value)
        {
            Values.Add(value);
        }

        /// <summary>
        /// Clears the children elements contained by this <see cref="JPlusValue"/>.
        /// </summary>
        public void Clear()
        {
            Values.Clear();
        }

        /// <summary>
        /// Removes all children elements contained by this <see cref="JPlusValue"/>, and then add the specified value as a child element.
        /// </summary>
        /// <param name="value">The value to add to this <see cref="JPlusValue"/> as a child element.</param>
        public void NewValue(IJPlusElement value)
        {
            Values.Clear();
            Values.Add(value);
        }

        /// <summary>
        /// Determines whether all the children elements contained by this <see cref="JPlusValue"/> are <see cref="string"/>.
        /// </summary>
        /// <returns>`true` if all children elements contained by this <see cref="JPlusValue"/> are <see cref="string"/>. Otherwise, `false`.</returns>
        public bool IsString()
        {
            return Values.Any() && Values.All(v => v.IsString());
        }

        private string ConcatString()
        {
            string concat = string.Join(string.Empty, Values.Select(l => l.GetString())).Trim();

            if (concat == "null")
                return null;

            return concat;
        }

        /// <summary>
        /// Returns a child element of this <see cref="JPlusValue"/>, located at the specified key.
        /// </summary>
        /// <param name="key">The key associated with the child element.</param>
        /// <returns>The child element at the <paramref name="key"/> specified.</returns>
        public JPlusValue GetChildObject(string key)
        {
            return GetObject().GetKey(key);
        }

        /// <summary>
        /// Return this element as a <see cref="bool"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">The value is not one of the following keywords: true, false, yes, no</exception>
        /// <returns>The <see cref="bool"/> value of this element.</returns>
        public bool GetBoolean()
        {
            string v = GetString();
            switch (v)
            {
                case "true":
                    return true;
                case "false":
                    return false;
                case "yes":
                    return true;
                case "no":
                    return false;
                default:
                    throw new NotSupportedException(string.Format(RS.BadBooleanName, v));
            }
        }

        /// <see cref="IJPlusElement.GetString()"/>
        public string GetString()
        {
            if (IsString())
                return ConcatString();

            return null; //TODO: throw exception?
        }

        /// <summary>
        /// Return this element as a <see cref="decimal"/>.
        /// </summary>
        /// <returns>The <see cref="decimal"/> value of this element.</returns>
        public decimal GetDecimal()
        {
            return decimal.Parse(GetString(), NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Return this element as a <see cref="float"/>.
        /// </summary>
        /// <returns>The <see cref="float"/> value of this element.</returns>
        public float GetSingle()
        {
            return float.Parse(GetString(), NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Return this element as a <see cref="double"/>.
        /// </summary>
        /// <returns>The <see cref="double"/> value of this element.</returns>
        public double GetDouble()
        {
            return double.Parse(GetString(), NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Return this element as a <see cref="long"/>.
        /// </summary>
        /// <returns>The <see cref="long"/> value of this element.</returns>
        public long GetInt64()
        {
            return long.Parse(GetString(), NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Return this element as an <see cref="int"/>.
        /// </summary>
        /// <returns>The <see cref="int"/> value of this element.</returns>
        public int GetInt32()
        {
            return int.Parse(GetString(), NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Return this element as a <see cref="byte"/>.
        /// </summary>
        /// <returns>The <see cref="byte"/> value of this element.</returns>
        public byte GetByte()
        {
            return byte.Parse(GetString(), NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Return this element as an enumerable collection <see cref="byte"/> objects.
        /// </summary>
        /// <returns>The <see cref="IList{Byte}"/> value of this element.</returns>
        public IList<byte> GetByteList()
        {
            return GetArray().Select(v => v.GetByte()).ToList();
        }

        /// <summary>
        /// Return this element as an enumerable collection <see cref="int"/> objects.
        /// </summary>
        /// <returns>The <see cref="IList{Int32}"/> value of this element.</returns>
        public IList<int> GetInt32List()
        {
            return GetArray().Select(v => v.GetInt32()).ToList();
        }

        /// <summary>
        /// Return this element as an enumerable collection <see cref="long"/> objects.
        /// </summary>
        /// <returns>The <see cref="IList{Int64}"/> value of this element.</returns>
        public IList<long> GetInt64List()
        {
            return GetArray().Select(v => v.GetInt64()).ToList();
        }

        /// <summary>
        /// Return this element as an enumerable collection <see cref="bool"/> objects.
        /// </summary>
        /// <returns>The <see cref="IList{Boolean}"/> value of this element.</returns>
        public IList<bool> GetBooleanList()
        {
            return GetArray().Select(v => v.GetBoolean()).ToList();
        }

        /// <summary>
        /// Return this element as an enumerable collection <see cref="float"/> objects.
        /// </summary>
        /// <returns>The <see cref="IList{Single}"/> value of this element.</returns>
        public IList<float> GetSingleList()
        {
            return GetArray().Select(v => v.GetSingle()).ToList();
        }

        /// <summary>
        /// Return this element as an enumerable collection <see cref="double"/> objects.
        /// </summary>
        /// <returns>The <see cref="IList{Double}"/> value of this element.</returns>
        public IList<double> GetDoubleList()
        {
            return GetArray().Select(v => v.GetDouble()).ToList();
        }

        /// <summary>
        /// Return this element as an enumerable collection <see cref="decimal"/> objects.
        /// </summary>
        /// <returns>The <see cref="IList{Decimal}"/> value of this element.</returns>
        public IList<decimal> GetDecimalList()
        {
            return GetArray().Select(v => v.GetDecimal()).ToList();
        }

        /// <summary>
        /// Return this element as an enumerable collection <see cref="string"/> objects.
        /// </summary>
        /// <returns>The <see cref="IList{String}"/> value of this element.</returns>
        public IList<string> GetStringList()
        {
            return GetArray().Select(v => v.GetString()).ToList();
        }

        /// <see cref="IJPlusElement.GetArray()"/>
        public IList<JPlusValue> GetArray()
        {
            IEnumerable<JPlusValue> x = from arr in Values
                where arr.IsArray()
                from e in arr.GetArray()
                select e;

            return x.ToList();
        }

        /// <see cref="IJPlusElement.IsArray()"/>
        public bool IsArray()
        {
            IList<JPlusValue> array = GetArray();
            return (array != null) && (array.Count != 0);
        }
        
        /// <summary>
        /// Returns this element as a <see cref="TimeSpan"/> object.
        /// </summary>
        /// <param name="allowInfinite">Set to `true` to allow the keyword `infinite`, which will return <see cref="Timeout.InfiniteTimeSpan"/>. Otherwise, `false`.</param>
        /// <returns>The <see cref="TimeSpan"/> value of this <see cref="JPlusValue"/>.</returns>
        public TimeSpan GetTimeSpan(bool allowInfinite = true)
        {
            string res = GetString();

            if (res.EndsWith("milliseconds") || res.EndsWith("millisecond") || res.EndsWith("millis") || res.EndsWith("milli"))
                res = "ms";
            else if (res.EndsWith("seconds") || res.EndsWith("second"))
                res = "s";
            else if (res.EndsWith("minutes") || res.EndsWith("minute"))
                res = "m";
            else if (res.EndsWith("hours") || res.EndsWith("hour"))
                res = "h";
            else if (res.EndsWith("days") || res.EndsWith("day"))
                res = "d";

            // #TODO: Add support for ns, us 
            // see https://github.com/typesafehub/config/blob/master/HOCON.md#duration-format

            if (res.EndsWith("ms"))
            {
                string v = res.Substring(0, res.Length - 2);
                return TimeSpan.FromMilliseconds(ParsePositiveValue(v));
            }

            if (res.EndsWith("s"))
            {
                string v = res.Substring(0, res.Length - 1);
                return TimeSpan.FromSeconds(ParsePositiveValue(v));
            }
            
            if (res.EndsWith("m"))
            {
                string v = res.Substring(0, res.Length - 1);
                return TimeSpan.FromMinutes(ParsePositiveValue(v));
            }
            
            if (res.EndsWith("h"))
            {
                string v = res.Substring(0, res.Length - 1);
                return TimeSpan.FromHours(ParsePositiveValue(v));
            }
            
            if (res.EndsWith("d"))
            {
                string v = res.Substring(0, res.Length - 1);
                return TimeSpan.FromDays(ParsePositiveValue(v));
            }
            
            // not in spec
            if (allowInfinite && res.Equals("infinite", StringComparison.OrdinalIgnoreCase))  
                return Timeout.InfiniteTimeSpan;

            return TimeSpan.FromMilliseconds(ParsePositiveValue(res));
        }

        private static double ParsePositiveValue(string v)
        {
            double parsed = double.Parse(v, NumberFormatInfo.InvariantInfo);

            if (parsed < 0)
                throw new FormatException(string.Format(RS.ExpectPositiveNumber, parsed));

            return parsed;
        }

        /// <summary>
        /// Returns this element as a <see cref="Nullable{Int64}"/> object by parsing the value as a number with data size unit.
        /// </summary>
        /// <exception cref="System.OverflowException">Maxiumum supported size is 7 exbibytes (e), or 9 exabytes (eb).</exception>
        /// <returns>The <see cref="Nullable{Int64}"/> value represented by this <see cref="JPlusValue"/>.</returns>
        /// <remarks>
        /// This method returns a value of type <see cref="Int64"/>. Therefore, the maximum supported size is 7e (or 9eb).
        /// 
        /// To specify a byte size, append any of the following keywords to a number:
        /// 
        /// | Unit                     | Meaning                    |
        /// |--------------------------|----------------------------|
        /// | b, byte, bytes           | This unit will be ignored. |
        /// | kb, kilobyte, kilobytes  | x1000                      |
        /// | kib, kibibyte, kibibytes | x1024                      |
        /// | mb, megabyte, megabytes  | x1000^2                    |
        /// | mib, mebibyte, mebibytes | x1024^2                    |
        /// | gb, gigabyte, gigabytes  | x1000^3                    |
        /// | gib, gibibyte, gibibytes | x1024^3                    |
        /// | tb, terabyte, terabytes  | x1000^4                    |
        /// | tib, tebibyte, tebibyte  | x1024^4                    |
        /// | pb, petabyte, petabytes  | x1000^5                    |
        /// | pib, pebibyte, pebibytes | x1024^5                    |
        /// | eb, exabyte, exabytes    | x1000^6                    |
        /// | eib, exbibyte, exbibytes | x1024^6                    |
        /// </remarks>
        public long? GetByteSize()
        {
            // #todo support for zb/yb

            string res = GetString();

            if (res.EndsWith("byte") || res.EndsWith("bytes"))
                res = "b";
            else if (res.EndsWith("kilobytes") || res.EndsWith("kilobyte"))
                res = "kb";
            else if (res.EndsWith("kib") || res.EndsWith("kibibytes") || res.EndsWith("kibibyte"))
                res = "k";
            else if (res.EndsWith("megabytes") || res.EndsWith("megabyte"))
                res = "mb";
            else if (res.EndsWith("mib") || res.EndsWith("mebibytes") || res.EndsWith("mebibyte"))
                res = "m";
            else if (res.EndsWith("gigabytes") || res.EndsWith("gigabyte"))
                res = "gb";
            else if (res.EndsWith("gib") || res.EndsWith("gibibytes") || res.EndsWith("gibibyte"))
                res = "g";
            else if (res.EndsWith("terabytes") || res.EndsWith("terabyte"))
                res = "tb";
            else if (res.EndsWith("tib") || res.EndsWith("tebibytes") || res.EndsWith("tebibyte"))
                res = "t";
            else if (res.EndsWith("petabytes") || res.EndsWith("petabyte"))
                res = "pb";
            else if (res.EndsWith("pib") || res.EndsWith("pebibytes") || res.EndsWith("pebibyte"))
                res = "p";
            else if (res.EndsWith("exabytes") || res.EndsWith("exabyte"))
                res = "eb";
            else if (res.EndsWith("eib") || res.EndsWith("exbibytes") || res.EndsWith("exbibyte"))
                res = "e";

            if (res.EndsWith("b"))
            {
                string v = res.Substring(0, res.Length - 1);
                return long.Parse(v);
            }

            if (res.EndsWith("kb"))
            {
                string v = res.Substring(0, res.Length - 2);
                return (long.Parse(v) * 1000);
            }
            if (res.EndsWith("k"))
            {
                string v = res.Substring(0, res.Length - 1);
                return (long.Parse(v) * 1024);
            }
            if (res.EndsWith("mb"))
            {
                string v = res.Substring(0, res.Length - 2);
                return (long.Parse(v) * 1000 * 1000);
            }
            if (res.EndsWith("m"))
            {
                string v = res.Substring(0, res.Length - 1);
                return (long.Parse(v) * 1024 * 1024);
            }
            if (res.EndsWith("gb"))
            {
                string v = res.Substring(0, res.Length - 2);
                return (long.Parse(v) * 1000 * 1000 * 1000);
            }
            if (res.EndsWith("g"))
            {
                string v = res.Substring(0, res.Length - 1);
                return (long.Parse(v) * 1024 * 1024 * 1024);
            }
            if (res.EndsWith("tb"))
            {
                string v = res.Substring(0, res.Length - 2);
                return (long.Parse(v) * 1000 * 1000 * 1000 * 1000);
            }
            if (res.EndsWith("t"))
            {
                string v = res.Substring(0, res.Length - 1);
                return (long.Parse(v) * 1024 * 1024 * 1024 * 1024);
            }
            if (res.EndsWith("pb"))
            {
                string v = res.Substring(0, res.Length - 2);
                return (long.Parse(v) * 1000 * 1000 * 1000 * 1000 * 1000);
            }
            if (res.EndsWith("p"))
            {
                string v = res.Substring(0, res.Length - 1);
                return (long.Parse(v) * 1024 * 1024 * 1024 * 1024 * 1024);
            }
            if (res.EndsWith("eb"))
            {
                string v = res.Substring(0, res.Length - 2);
                return (long.Parse(v) * 1000 * 1000 * 1000 * 1000 * 1000 * 1000);
            }
            if (res.EndsWith("e"))
            {
                string v = res.Substring(0, res.Length - 1);
                return (long.Parse(v) * 1024 * 1024 * 1024 * 1024 * 1024 * 1024);
            }

            return long.Parse(res);
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="JPlusValue"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="JPlusValue"/>.</returns>
        public override string ToString()
        {
            return ToString(0);
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="JPlusValue"/>.
        /// </summary>
        /// <param name="indent">The number of spaces to indent the string.</param>
        /// <returns>A <see cref="string"/> representation of this <see cref="JPlusValue"/>.</returns>
        public virtual string ToString(int indent)
        {
            if (IsString())
            {
                string text = QuoteIfNeeded(GetString());
                return text;
            }
            if (IsObject())
            {
                string i = new string(' ', indent * 2);
                return string.Format("{{\r\n{1}{0}}}", i, GetObject().ToString(indent + 1));
            }
 
            if (IsArray())
                return string.Format("[{0}]", string.Join(",", GetArray().Select(e => e.ToString(indent + 1))));
 
            return "<<unknown value>>";
        }

        private string QuoteIfNeeded(string text)
        {
            if (text == null) 
                return string.Empty;
            
            if (text.ToCharArray().Intersect(" \t".ToCharArray()).Any())
                return "\"" + text + "\"";

            return text;
        }
    }
}
