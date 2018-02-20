using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using Standard;

namespace Standard.Data.Json
{
	internal static class JsonSerializingEngine
	{
		public const char 
			QuotDoubleChar = '"',
			QuotSingleChar = '\'';

		public const int DefaultStringBuilderCapacity = 1024 * 2;

		[ThreadStatic]
		private static StringBuilder _cachedStringBuilder;

		//private static Regex //_dateRegex = new Regex(@"\\/Date\((?<ticks>-?\d+)\)\\/", RegexOptions.Compiled),
		private static Regex _dateISORegex = new Regex(@"(\d){4}-(\d){2}-(\d){2}T(\d){2}:(\d){2}:(\d){2}.(\d){3}Z", RegexOptions.Compiled);


#if NETSTANDARD
        // Retrieved from https://github.com/dotnet/corefx/pull/10088
        private static readonly Func<Type, object> s_getUninitializedObjectDelegate = 
			(Func<Type, object>)typeof(string)
				.GetTypeInfo().Assembly.GetType("System.Runtime.Serialization.FormatterServices")
				?.GetMethod("GetUninitializedObject", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
				?.CreateDelegate(typeof(Func<Type, object>));

        internal static object GetUninitializedObject(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return s_getUninitializedObjectDelegate(type);
        }
#endif

		public static StringBuilder GetStringBuilder()
		{
			return _cachedStringBuilder ?? (_cachedStringBuilder = new StringBuilder(DefaultStringBuilderCapacity));
		}

		public static T GetUninitializedInstance<T>()
		{
#if NETSTANDARD
            return (T)GetUninitializedObject(typeof(T));
#else
			return (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
#endif
		}

		public static object GetTypeIdentifierInstance(string typeName)
		{
			return JsonConvert.GetTypeIdentifierInstance(typeName);
		}

		internal unsafe static object ToStrIfStr(object value, JsonSerializerSettings settings)
		{
			var str = value as string;

			if (str != null)
			{
				str = string.Concat('"', str, '"');

				fixed (char* p = str)
				{
					char* ptr = p;
					int index = 0;

					return JsonConvert.DecodeJsonString(ptr, ref index, settings, fromObject: true);
				}
			}

			return value;
		}

		public unsafe static void ThrowIfInvalidJson(string json, char chr)
		{
			if (StringUtility.IsNullOrWhiteSpace(json))
				throw new InvalidJsonException();

			var endChar = chr == '[' ? ']' : '}';

			if (json[0] != chr)
				throw new InvalidJsonException();

			var length = json.Length - 1;
			var lastChr = '\0';
			fixed (char* ptr = json)
			{
				do
				{
					lastChr = *(ptr + length);
					if (lastChr != '\n' && lastChr != '\r' && lastChr != '\t' && lastChr != ' ')
						break;

					length--;
				} while (lastChr != '\0');
			}

			if (lastChr != endChar)
				throw new InvalidJsonException(length);
		}

		internal unsafe static bool IsInRange(char* ptr, ref int index, int offset, string key, JsonSerializerSettings settings)
		{
			var inRangeChr = *(ptr + index + offset + 2);
			fixed (char* kPtr = key)
			{
				return (*(ptr + index) == settings._quoteChar &&
					(inRangeChr == ':' || inRangeChr == ' ' ||
					inRangeChr == '\t' || inRangeChr == '\n' || inRangeChr == '\r')) &&
					*(ptr + index + 1) == *kPtr;
			}
		}

		public static bool IsListType(Type type)
		{
			return type.IsListType();
		}

		public static bool IsDictionaryType(Type type)
		{
			return type.IsDictionaryType();
		}

		public static bool IsCollectionType(this Type type)
		{
			return type.IsListType() || type.IsDictionaryType();
		}

		public static bool IsRawPrimitive(string value)
		{
			value = value.Trim();
			return !value.StartsWith("{") && !value.StartsWith("[");
		}

		public static bool IsCurrentAQuot(char current, JsonSerializerSettings settings)
		{
			if (settings.HasOverrideQuoteChar)
				return current == settings._quoteChar;

			var quote = settings._quoteChar;
			var isQuote = current == QuotSingleChar || current == QuotDoubleChar;
			if (isQuote)
			{
				if (quote != current)
					settings._quoteCharString = (settings._quoteChar = current).ToString();

				settings.HasOverrideQuoteChar = true;
			}
			return isQuote;
		}

		public static bool NeedQuotes(Type type, JsonSerializerSettings settings)
		{
			return JsonConvert.NeedQuotes(type, settings);
		}

		public static bool IsValueDate(string value)
		{
			return value.StartsWith("\\/Date") || _dateISORegex.IsMatch(value);
		}

		internal static unsafe void SkipProperty(char* ptr, ref int index, JsonSerializerSettings settings)
		{
			var currentIndex = index;
			char current = '\0';
			char bchar = '\0';
			char echar = '\0';
			bool isStringType = false;
			bool isNonStringType = false;
			int counter = 0;
			bool hasChar = false;
			var currentQuote = settings._quoteChar;

			while (true)
			{
				current = *(ptr + index);
				if (current != ' ' && current != ':' && current != '\n' && current != '\r' && current != '\t')
				{
					if (!hasChar)
					{
						isStringType = current == currentQuote;
						if (!isStringType)
							isNonStringType = current != '{' && current != '[';

						if (isStringType || isNonStringType)
							break;

						bchar = current;
						echar = current == '{' ? '}' :
							current == '[' ? ']' : '\0';
						counter = 1;
						hasChar = true;
					}
					else
					{
						if ((current == '{' && bchar == '{') || (current == '[' && bchar == '['))
							counter++;
						else if ((current == '}' && echar == '}') || (current == ']' && echar == ']'))
							counter--;
					}
				}

				index++;
				if (hasChar && counter == 0)
					break;
			}

			if (isStringType || isNonStringType)
			{
				index = currentIndex;
				if (isStringType)
					GetStringBasedValue(ptr, ref index, settings);
				else if (isNonStringType)
					GetNonStringValue(ptr, ref index);
			}
		}

		internal unsafe static string GetStringBasedValue(char* ptr, ref int index, JsonSerializerSettings settings)
		{
			char current = '\0', prev = '\0';
			int count = 0, startIndex = 0;
			string value = string.Empty;

			while (true)
			{
				current = ptr[index];
				if (count == 0 && current == settings._quoteChar)
				{
					startIndex = index + 1;
					++count;
				}
				else if (count > 0 && current == settings._quoteChar && prev != '\\')
				{
					value = new string(ptr, startIndex, index - startIndex);
					++index;
					break;
				}
				else if (count == 0 && current == 'n')
				{
					index += 3;
					return null;
				}

				prev = current;
				++index;
			}

			return value;
		}

		internal unsafe static string GetNonStringValue(char* ptr, ref int index)
		{
			char current = '\0';
			int startIndex = -1;
			string value = string.Empty;
			int indexDiff = 0;

			while (true)
			{
				current = ptr[index];
				if (startIndex > -1)
				{
					switch (current)
					{
						case '\n':
						case '\r':
						case '\t':
						case ' ':
							++indexDiff;
							break;
					}
				}
				if (current != ' ' && current != ':')
				{
					if (startIndex == -1)
						startIndex = index;

					if (current == ',' || current == ']' || current == '}' || current == '\0')
					{
						value = new string(ptr, startIndex, index - startIndex - indexDiff);
						--index;
						break;
					}
					else if (current == 't')
					{
						index += 4;
						return "true";
					}
					else if (current == 'f')
					{
						index += 5;
						return "false";
					}
					else if (current == 'n')
					{
						index += 4;
						return null;
					}
				}
				++index;
			}

			if (value == "null")
				return null;

			return value;
		}

		internal unsafe static int MoveToArrayBlock(char* str, ref int index)
		{
			char* ptr = str + index;

			if (*ptr == '[')
			{
				index++;
			}
			else
			{
				do
				{
					index++;
					if (*(ptr = str + index) == 'n')
					{
						index += 3;
						return 0;
					}
				} while (*ptr != '[');
				index++;
			}
			return 1;
		}

		public static bool IsEndChar(char current)
		{
			return current == ':' || current == '{' || current == ' ';
		}

		public static bool IsArrayEndChar(char current)
		{
			return current == ',' || current == ']' || current == ' ';
		}

		public static bool IsCharTag(char current)
		{
			return current == '{' || current == '}';
		}

		public static bool CustomTypeEquality(Type type1, Type type2)
		{
			if (type1.GetTypeInfo().IsEnum)
			{
				if (type1.GetTypeInfo().IsEnum && type2 == typeof(Enum))
					return true;
			}

			return type1 == type2;
		}

		public static void SetterPropertyValue<T>(T instance, object value, MethodInfo methodInfo)
		{
			JsonConvert.SetterPropertyValue(instance, value, methodInfo);
		}

		public static List<object> ListToListObject(IList list)
		{
			return list.Cast<object>().ToList();
		}

		public static string ToCamelCase(string value)
		{
			return value.ToCamelCase();
		}

		#region Primitive serialization

		public static Type StrToType(string value)
		{
			return FastConvert.ToType(value);
			//return Type.GetType(value, false);
		}

		public static double StrToDouble(string value)
		{
			return FastConvert.ToDouble(value);
		}

		public static int StrToInt32(string value)
		{
			return FastConvert.ToInt32(value);
		}

		public static long StrToInt64(string value)
		{
			return FastConvert.ToInt64(value);
		}

		public static string UInt32ToStr(uint value)
		{
			return FastConvert.ToString(value);
		}

		public static string UInt64ToStr(ulong value)
		{
			return FastConvert.ToString(value);
		}

		public static uint StrToUInt32(string value)
		{
			return FastConvert.ToUInt32(value);
		}

		public static ulong StrToUInt64(string value)
		{
			return FastConvert.ToUInt64(value);
		}

		public unsafe static bool StrToBoolean(string value)
		{
			return value[0] == 't';
		}

		public static char StrToChar(string value)
		{
			return value[0];
		}

		public static string CharToStr(char chr)
		{
			return chr.ToString();
		}

		public static byte[] StrToByteArray(string value)
		{
			if (StringUtility.IsNullOrWhiteSpace(value))
				return null;

			return Base64StrToByteArray(value);
		}

		public static string ByteArrayToStr(byte[] value)
		{
			return ByteArrayToBase64Str(value);
		}

		public static byte[] Base64StrToByteArray(string value)
		{
			//TODO: Optimize
			return Convert.FromBase64String(value);
		}

		public static string ByteArrayToBase64Str(byte[] value)
		{
			//TODO: Optimize
			return Convert.ToBase64String(value);
		}

		public static string GuidToStr(Guid value)
		{
			return FastConvert.ToString(value);
		}

		public static Guid StrToGuid(string value)
		{
			return FastConvert.ToGuid(value);
		}

		public static unsafe byte StrToByte(string str)
		{
			return FastConvert.ToByte(str);
		}

		public static unsafe short StrToInt16(string str)
		{
			return FastConvert.ToInt16(str);
		}

		public static unsafe ushort StrToUInt16(string str)
		{
			return FastConvert.ToUInt16(str);
		}

		public static float StrToSingle(string numStr)
		{
			return FastConvert.ToSingle(numStr);
		}

		public static decimal StrToDecimal(string numStr)
		{
			return FastConvert.ToDecimal(numStr);
		}

		public static string SingleToStr(float value)
		{
			return FastConvert.ToString(value);
		}

		public static string DoubleToStr(double value)
		{
			return FastConvert.ToString(value);
		}

		public static string SByteToStr(sbyte value)
		{
			return FastConvert.ToString(value);
		}

		public static string DecimalToStr(decimal value)
		{
			return FastConvert.ToString(value);
		}

		public static string Int32ToStr(int snum)
		{
			return FastConvert.ToString(snum);
		}

		public static string Int64ToStr(long snum)
		{
			return FastConvert.ToString(snum);
		}

		public static string EnumToStr(Enum e, JsonSerializerSettings settings)
		{
			if (settings.EnumAsString)
				return e.ToString();
			return Int32ToStr((int)((object)e));
		}

		public static string FlagEnumToStr(object value, JsonSerializerSettings settings)
		{
			if ((int)value == 0)
				return "0";

			if (settings.EnumAsString)
				return ((Enum)value).ToString();

			var eType = value.GetType().GetEnumUnderlyingType();

			if (eType == JsonConvert._intType)
				return Int32ToStr((int)value);
			else if (eType == JsonConvert._longType)
				return Int64ToStr((long)value);
			else if (eType == typeof(ulong))
				return Int64ToStr((long)((ulong)value));
			else if (eType == typeof(uint))
				return UInt32ToStr((uint)value);
			else if (eType == typeof(byte))
				return Int32ToStr((int)((byte)value));
			else if (eType == typeof(ushort))
				return Int32ToStr((int)((ushort)value));
			else if (eType == typeof(short))
				return Int32ToStr((int)((short)value));

			return Int32ToStr((int)value);
		}

		internal static T FlagStrToEnum<T>(string value)
		{
			return (T)Enum.Parse(typeof(T), value);
		}

		private static DateTimeStyles JsonTimeZoneToDateTimeStyles(JsonTimeZoneHandling tzHandling)
		{
			if (tzHandling == JsonTimeZoneHandling.Default)
				return DateTimeStyles.AssumeUniversal;

			switch (tzHandling)
			{
				case JsonTimeZoneHandling.UtcAsLocalStrict:
					return DateTimeStyles.AssumeUniversal;

				case JsonTimeZoneHandling.UtcAsLocal:
					return DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces;

				case JsonTimeZoneHandling.LocalAsUtcStrict:
					return DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal;

				case JsonTimeZoneHandling.LocalAsUtc:
					return DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces;

				case JsonTimeZoneHandling.UtcStrict:
					return DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;

				case JsonTimeZoneHandling.Utc:
					return DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces;

				case JsonTimeZoneHandling.LocalStrict:
					return DateTimeStyles.AssumeLocal;

				case JsonTimeZoneHandling.Local:
					return DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces;

				default:
					return DateTimeStyles.None;
			}
		}

		public static DateTimeOffset StrToDateTimeoffset(string value, JsonSerializerSettings settings)
		{
			DateTimeStyles style = JsonTimeZoneToDateTimeStyles(settings.TimeZone);

			if (value[0] == '\\')
				value = value.Substring(7, value.IndexOf(')', 7) - 7); 

			return settings._hasDateStringFormat ? FastConvert.ToDateTimeOffset(value, settings.CustomDateFormat, style)
				: settings.DateFormat == JsonDateTimeHandling.Default ? FastConvert.ToDateTimeOffset(value, "e", style)
				: settings.DateFormat == JsonDateTimeHandling.ISO ? FastConvert.ToDateTimeOffset(value, "i", style)
				: settings.DateFormat == JsonDateTimeHandling.JsonNetISO ? FastConvert.ToDateTimeOffset(value, "i", style)
				: settings.DateFormat == JsonDateTimeHandling.MicrosoftJsonDate ? FastConvert.ToDateTimeOffset(value, "E", style)
				: settings.DateFormat == JsonDateTimeHandling.EpochTime ? FastConvert.ToDateTimeOffset(value, "e", style)
				: FastConvert.ToDateTime(value, "i", style);
		}

		public static DateTime StrToDate(string value, JsonSerializerSettings settings)
		{
			DateTimeStyles style = JsonTimeZoneToDateTimeStyles(settings.TimeZone);

			if (value[0] == '\\')
				value = value.Substring(7, value.IndexOf(')', 7) - 7); 

			return settings._hasDateStringFormat ? FastConvert.ToDateTime(value, settings.CustomDateFormat, style)
				: settings.DateFormat == JsonDateTimeHandling.Default ? FastConvert.ToDateTime(value, "e", style)
				: settings.DateFormat == JsonDateTimeHandling.ISO ? FastConvert.ToDateTime(value, "i", style)
				: settings.DateFormat == JsonDateTimeHandling.JsonNetISO ? FastConvert.ToDateTime(value, "i", style)
				: settings.DateFormat == JsonDateTimeHandling.MicrosoftJsonDate ? FastConvert.ToDateTime(value, "E", style)
				: settings.DateFormat == JsonDateTimeHandling.EpochTime ? FastConvert.ToDateTime(value, "e", style)
				: FastConvert.ToDateTime(value, "i", style);
		}

		public static string DateToStr(DateTime date, JsonSerializerSettings settings)
		{
			DateTimeStyles style = JsonTimeZoneToDateTimeStyles(settings.TimeZone);

			return settings._hasDateStringFormat ? FastConvert.ToString(date, settings.CustomDateFormat, style)
				: settings.DateFormat == JsonDateTimeHandling.Default ? string.Concat("\\/Date(", FastConvert.ToString(date, "e", style), ")\\/")
				: settings.DateFormat == JsonDateTimeHandling.EpochTime ? string.Concat("\\/Date(", FastConvert.ToString(date, "e", style), ")\\/")
				: settings.DateFormat == JsonDateTimeHandling.MicrosoftJsonDate ? string.Concat("\\/Date(", FastConvert.ToString(date, "E", style), ")\\/")
				: settings.DateFormat == JsonDateTimeHandling.ISO ? FastConvert.ToString(date, "i", style)
				: settings.DateFormat == JsonDateTimeHandling.JsonNetISO ? FastConvert.ToString(date, "i", style)
				: FastConvert.ToString(date, "i", style);
		}

		public static string DateOffsetToStr(DateTimeOffset offset, JsonSerializerSettings settings)
		{
			DateTimeStyles style = JsonTimeZoneToDateTimeStyles(settings.TimeZone);

			return settings._hasDateStringFormat ? FastConvert.ToString(offset, settings.CustomDateFormat, style)
				: settings.DateFormat == JsonDateTimeHandling.Default ? string.Concat("\\/Date(", FastConvert.ToString(offset, "e", style), ")\\/")
				: settings.DateFormat == JsonDateTimeHandling.EpochTime ? string.Concat("\\/Date(", FastConvert.ToString(offset, "e", style), ")\\/")
				: settings.DateFormat == JsonDateTimeHandling.MicrosoftJsonDate ? string.Concat("\\/Date(", FastConvert.ToString(offset, "E", style), ")\\/")
				: settings.DateFormat == JsonDateTimeHandling.ISO ? FastConvert.ToString(offset, "i", style)
				: settings.DateFormat == JsonDateTimeHandling.JsonNetISO ? FastConvert.ToString(offset, "i", style)
				: FastConvert.ToString(offset, "i", style);
		}

		#endregion // Primitive serialization
	}
}
