using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Standard;

namespace Standard.Data.Json
{
	internal static class AutomaticTypeConverter
	{
		private static readonly long _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
		private static Regex _dateRegex = new Regex(@"\\/Date\((?<ticks>-?\d+)\)\\/", RegexOptions.Compiled);

		public static object ToExpectedType(string value)
		{
			if (StringUtility.IsNullOrWhiteSpace(value))
				return value;

			var typeRegExs = TypeRegExs;
			var typeRuleFuncs = TypeRuleFuncs;

			foreach (var regex in typeRegExs)
			{
				if (Regex.IsMatch(value, regex.Value))
					return typeRuleFuncs[regex.Key](value);
			}
			return value;
		}

		private static Dictionary<string, string> TypeRegExs
		{
			get
			{
				Dictionary<string, string> regexs = new Dictionary<string, string>()
				{
					// #MANUAL_FORMAT
					{ "bool",   @"^(false)$|^(true)$" },
					{ "int",    @"^-?\d{1,10}$" },
					{ "long",   @"^-?\d{19}$" },
					{ "double", @"^-?[0-9]{0,15}(\.[0-9]{1,15})?$|^-?(100)(\.[0]{1,15})?$" },
					{ "date",   @"^\d{1,2}/\d{1,2}/\d{4}" },
					{ "date2",  @"\\/Date\((?<ticks>-?\d+)\)\\/" },
					{ "date3",  @"^(\d){4}-(\d){2}-(\d){2}T(\d){2}:(\d){2}:(\d){2}.(\d){2,3}Z$" },
					{ "date4",  @"^(\d){4}-(\d){2}-(\d){2}T(\d){2}:(\d){2}:(\d){2}.(\d){2,3}$" },
					{ "date5",  @"^(\d){4}-(\d){2}-(\d){2}T(\d){2}:(\d){2}:(\d){2}Z$" },
					{ "date6",  @"^(\d){4}-(\d){2}-(\d){2}T(\d){2}:(\d){2}:(\d){2}$" }
					// #/MANUAL_FORMAT
				};

				return regexs;
			}
		}

		private static Dictionary<string, Func<string, object>> TypeRuleFuncs
		{
			get
			{
				Dictionary<string, Func<string, object>> rules = new Dictionary<string, Func<string, object>>()
				{
					{ "int", new Func<string, object>(str => { return JsonSerializingEngine.StrToInt32(str); }) },
					{ "long", new Func<string, object>(str => { return JsonSerializingEngine.StrToInt64(str); }) },
					{ "double", new Func<string, object>(str => { return JsonSerializingEngine.StrToDouble(str); }) },
					{ "bool", new Func<string, object>(str => { return CastTo<bool>(str); }) },
					{ "date", new Func<string, object>(str => { return CastTo<DateTime>(str); }) },
					{ 
						"date2", 
						new Func<string, object>(str => { 
							var ticks = JsonSerializingEngine.StrToInt64(_dateRegex.Match(str).Groups["ticks"].Value);
							return new DateTime(ticks + _epoch).ToLocalTime();
						}) 
					},
					{ "date3", new Func<string, object>(str => { return DateTime.Parse(str); }) },
					{ "date4", new Func<string, object>(str => { return DateTime.Parse(str); }) },
					{ "date5", new Func<string, object>(str => { return DateTime.Parse(str); }) },
					{ "date6", new Func<string, object>(str => { return DateTime.Parse(str); }) }
				};

				return rules;
			}
		}

		private static T CastTo<T>(string str)
		{
			return (T)Convert.ChangeType(str, typeof(T));
		}
	}
}
