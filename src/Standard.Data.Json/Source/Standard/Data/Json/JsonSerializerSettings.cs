using System;

namespace Standard.Data.Json
{
	/// <summary>
	/// Settings for controlling how JSON is serialized and deserialized.
	/// </summary>
	public sealed class JsonSerializerSettings
	{
        internal bool _hasDateStringFormat = false;
		internal string _dateStringFormat;
		internal StringComparison _caseComparison = StringComparison.Ordinal;
		private bool _caseSensitive;
		private JsonQuoteHandling _quoteType;

		internal char _quoteChar;
		internal string _quoteCharString;

		[ThreadStatic]
		private static JsonSerializerSettings _current;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonSerializerSettings" /> class.
		/// </summary>
		public JsonSerializerSettings()
		{
			if (_current == null)
			{
				DateFormat = JsonDateTimeHandling.Default;
				TimeZone = JsonTimeZoneHandling.Default;
				EnumAsString = false;
				SkipDefaultValue = true;
				CaseSensitive = true;
				QuoteType = JsonQuoteHandling.Default;
				OptimizeString = true;
			}
			else
			{
				DateFormat = Current.DateFormat;
				TimeZone = Current.TimeZone;
				EnumAsString = Current.EnumAsString;
				SkipDefaultValue = Current.SkipDefaultValue;
				CaseSensitive = Current.CaseSensitive;
				QuoteType = Current.QuoteType;
				OptimizeString = Current.OptimizeString;
			}

			Indent = JsonIndentHandling.Default;
			CamelCase = false;
		}

		/// <summary>
		/// Formatting pattern for <see cref="System.DateTime"/> serialization.
		/// </summary>
		public JsonDateTimeHandling DateFormat { get; set; }

        /// <summary>
        /// Use a custom formatting pattern for <see cref="System.DateTime"/> serialization. 
        /// </summary>
        public string CustomDateFormat
		{
            get
            {
                return _dateStringFormat;
            }
            set
            {
                _dateStringFormat = value;
                _hasDateStringFormat = !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// Determine time zone format. Default is system defined.
        /// </summary>
        public JsonTimeZoneHandling TimeZone { get; set; }

		/// <summary>
		/// Determine formatting for output JSON. Default is compact layout.
		/// </summary>
		public JsonIndentHandling Indent { get; set; }

		/// <summary>
		/// Determine if enums should be serialized as string or <see cref="System.Int32"/> value. Default is <c>True</c>.
		/// </summary>
		public bool EnumAsString { get; set; }

		/// <summary>
		/// Determine if default value should be skipped. Default is <c>True</c>.
		/// </summary>
		public bool SkipDefaultValue { get; set; }

		/// <summary>
		/// Determine case sensitive for property/field name. Default is <c>True</c>.
		/// </summary>
		internal bool CaseSensitive
		{
			get
			{
				return _caseSensitive;
			}
			set
			{
				_caseSensitive = value;
				_caseComparison = _caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			}
		}

		public bool IgnoreCase
		{
			get
			{
				return !CaseSensitive;
			}
			set
			{
				CaseSensitive = !value;
			}
		}

		/// <summary>
		/// Determine the quote character. Default is double quote.
		/// </summary>
		public JsonQuoteHandling QuoteType
		{
			get
			{
				return _quoteType;
			}
			set
			{
				_quoteType = value;
				_quoteChar = _quoteType == JsonQuoteHandling.Single ? '\'' : '"';
				_quoteCharString = _quoteType == JsonQuoteHandling.Single ? "'" : "\"";
			}
		}

		public bool HasOverrideQuoteChar { get; internal set; }

		public bool OptimizeString { get; set; }

		private StringComparison CaseComparison
		{
			get
			{
				return CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			}
		}

		/// <summary>
		/// Enable camelCasing for property/field names
		/// </summary>
		public bool CamelCase { get; set; }

		/// <summary>
		/// Returns current <see cref="JsonSerializerSettings"/> that correspond to old use of settings.
		/// </summary>
		public static JsonSerializerSettings Current
		{
			get
			{
				return _current ?? (_current = new JsonSerializerSettings());
			}
		}

		/// <summary>
		/// Clone current settings as a new object.
		/// </summary>
		public JsonSerializerSettings Clone()
		{
			return new JsonSerializerSettings
			{
				DateFormat = DateFormat,
				TimeZone = TimeZone,
				EnumAsString = EnumAsString,
				SkipDefaultValue = SkipDefaultValue,
				CaseSensitive = CaseSensitive,
				QuoteType = QuoteType,
				OptimizeString = OptimizeString,
				Indent = Indent
			};
		}
	}

	/// <summary>
	/// Option for determining date formatting
	/// </summary>
	public enum JsonDateTimeHandling
	{
		/// <summary>
		/// Default following the format <c>/Date(...)/</c>.
		/// </summary>
		Default = 0,

		/// <summary>
		/// Dates are written in the ISO 8601 format, e.g. `2012-03-21T05:40Z` / `2012-03-21T05:40.143+0200`.
		/// </summary>
		ISO = 2,

		/// <summary>
		/// Unix epoch milliseconds.
		/// </summary>
		EpochTime = 4,

		/// <summary>
		/// ISO 8601 format used by <c>JSON.NET</c>. This format is intended for backward compatibility.
		/// </summary>
		JsonNetISO = 6,

        /// <summary>
        /// Format used by <c>System.Web.Script.Serialization.JavaScriptSerializer</c>. This format is intended for backward compatibility.
        /// </summary>
        MicrosoftJsonDate = 8
	}

	//<#
	// 	.SYNOPSIS
	//		Option for determining timezone formatting.
	//#>
	public enum JsonTimeZoneHandling
	{
		//<#
		//	.INHERIT JsonTimeZoneHandling.UtcAsLocalStrict
		//#>
		Default = UtcAsLocalStrict,

		//<#
		//	.SYNOPSIS
		// 		When parsing JSON text, assumes that the JSON source formats time in UTC. All deserialized @DateTime/@DateTimeOffset objects are 
		// 		in UTC. 
		//
		//		When serializing to JSON, will convert to UTC formats where applicable. When serializing a @DateTime object, will assume that it 
		// 		is referring to UTC time, unless the @"DateTime.Kind" property is set to @"DateTimeKind.Local", in which case it is assumed to be 
		// 		in the time zone set by the operating system.
		//
		// 		The parser expects exact formatting, and will raise an exception if unexpected characters are encountered (such as whitespaces).
		//#>
		UtcStrict = 2,

		//<#
		//	.SYNOPSIS
		// 		When parsing JSON text, assumes that the JSON source formats time in UTC. All deserialized @DateTime/@DateTimeOffset objects are 
		// 		in UTC. 
		//
		//		When serializing to JSON, will convert to UTC formats where applicable. When serializing a @DateTime object, will assume that it 
		// 		is referring to UTC time, unless the @"DateTime.Kind" property is set to @"DateTimeKind.Local", in which case it is assumed to be 
		// 		in the time zone set by the operating system.
		//
		// 		The parser can handle minor deviations in formatting (such as whitespaces), but at the expense of efficiency.
		//#>
		Utc = 4,

		//<#
		//	.SYNOPSIS
		// 		When parsing JSON text, assumes that the JSON source formats time with time zone specifiers. All deserialized @DateTime/@DateTimeOffset 
		// 		objects are converted to local time in accordance with the time zone of the operating system.
		//
		//		When serializing to JSON, will convert to formats with time zone specifiers where applicable. When serializing a @DateTime object, 
		//		will assume that it is referring to the operating system time zone, unless the @"DateTime.Kind" property is set to @"DateTimeKind.Utc".
		//
		// 		The parser expects exact formatting, and will raise an exception if unexpected characters are encountered (such as whitespaces).
		//#>	
		LocalStrict = 6,

		//<#
		//	.SYNOPSIS
		// 		When parsing JSON text, assumes that the JSON source formats time with time zone specifiers. All deserialized @DateTime/@DateTimeOffset 
		// 		objects are converted to local time in accordance with the time zone of the operating system.
		//
		//		When serializing to JSON, will convert to formats with time zone specifiers where applicable. When serializing a @DateTime object, 
		//		will assume that it is referring to the operating system time zone, unless the @"DateTime.Kind" property is set to @"DateTimeKind.Utc".
		//
		// 		The parser can handle minor deviations in formatting (such as whitespaces), but at the expense of efficiency.
		//#>	
		Local = 8,

		//<#
		//	.SYNOPSIS
		// 		When parsing JSON text, assumes that the JSON source formats time in UTC. All deserialized @DateTime/@DateTimeOffset 
		// 		objects are converted to local time in accordance with the time zone of the operating system.
		//
		//		When serializing to JSON, will convert to UTC formats where applicable. When serializing a @DateTime object, 
		//		will assume that it is referring to the operating system time zone, unless the @"DateTime.Kind" property is set to @"DateTimeKind.Utc".
		//
		// 		The parser expects exact formatting, and will raise an exception if unexpected characters are encountered (such as whitespaces).
		//#>
		UtcAsLocalStrict = 10,

		//<#
		//	.SYNOPSIS
		// 		When parsing JSON text, assumes that the JSON source formats time in UTC. All deserialized @DateTime/@DateTimeOffset 
		// 		objects are converted to local time in accordance with the time zone of the operating system.
		//
		//		When serializing to JSON, will convert to UTC formats where applicable. When serializing a @DateTime object, 
		//		will assume that it is referring to the operating system time zone, unless the @"DateTime.Kind" property is set to @"DateTimeKind.Utc".
		//
		// 		The parser can handle minor deviations in formatting (such as whitespaces), but at the expense of efficiency.
		//#>
		UtcAsLocal = 12,

		//<#
		//	.SYNOPSIS
		// 		When parsing JSON text, assumes that the JSON source formats time with time zone specifiers. All deserialized @DateTime/@DateTimeOffset 
		// 		objects are converted to UTC time.
		//
		//		When serializing to JSON, will convert to formats with time zone specifiers where applicable. When serializing a @DateTime object, 
		//		will assume that it is referring to UTC time, unless the @"DateTime.Kind" property is set to @"DateTimeKind.Local", in which case it is 
		// 		assumed to be in the time zone of the operating system. 
		//
		// 		The parser expects exact formatting, and will raise an exception if unexpected characters are encountered (such as whitespaces).
		//#>
		LocalAsUtcStrict = 14,

		//<#
		//	.SYNOPSIS
		// 		When parsing JSON text, assumes that the JSON source formats time with time zone specifiers. All deserialized @DateTime/@DateTimeOffset 
		// 		objects are converted to UTC time.
		//
		//		When serializing to JSON, will convert to formats with time zone specifiers where applicable. When serializing a @DateTime object, 
		//		will assume that it is referring to UTC time, unless the @"DateTime.Kind" property is set to @"DateTimeKind.Local", in which case it is 
		// 		assumed to be in the time zone of the operating system. 
		//
		// 		The parser can handle minor deviations in formatting (such as whitespaces), but at the expense of efficiency.
		//#>
		LocalAsUtc = 16
	}

	/// <summary>
	/// Option for determine what type of quote to use for serialization and deserialization
	/// </summary>
	public enum JsonQuoteHandling
	{
		/// <summary>
		/// Default (double quote)
		/// </summary>
		Default = 0,

		/// <summary>
		/// Use double quote
		/// </summary>
		Double = Default,

		/// <summary>
		/// Use single quote
		/// </summary>
		Single = 2
	}

	/// <summary>
	/// Options for controlling serialize json format
	/// </summary>
	public enum JsonIndentHandling
	{
		/// <summary>
		/// Default (compact layout)
		/// </summary>
		Default = 0,

		/// <summary>
		/// Prettify string
		/// </summary>
		Prettify = 2
	}
}
