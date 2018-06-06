using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Standard.Core;

namespace Standard
{
	public static class EnumUtility
	{
		//<#
		// 	.SYNOPSIS
		// 		Test whether a given type is an @Enum.
		// 
		// 	.PARAM enumType
		// 		The object type to test.
		//
		// 	.OUTPUT
		// 		If `true`, indicates that @[enumType] is an @Enum. Otherwise, `false`.
		//#>
		public static bool IsEnum(Type enumType)
		{
#if NETSTANDARD
            if (enumType.GetTypeInfo().BaseType == typeof(Enum))
				return true;
#elif WINRT
            if (enumType.IsEnum())
				return true;
#else
			if (enumType.BaseType == typeof(Enum))
				return true;
#endif

			return false;
		}

		//<#
		// 	.SYNOPSIS
		// 		Returns all members of an @Enum.
		// 
		// 	.PARAM <TEnum>
		// 		The object type.
		//
		// 	.OUTPUT
		// 		A list of all members of @<TEnum>.
		//#>
		public static IEnumerable<TEnum> GetMembers<TEnum>()
		{
			Type enumType = typeof(TEnum);
			bool isEnumType = IsEnum(enumType);

			// Can't use generic type constraints on value types, so have to do check like this.
			if (!isEnumType)
				throw new ArgumentException(RS.Err_ExpectEnumType, enumType.FullName);

			//Array enumValArray = Enum.GetValues(enumType);
			//List<T> enumValList = new List<T>(enumValArray.Length);
			IOrderedEnumerable<TEnum> enumValArray = Enum.GetValues(enumType).Cast<TEnum>().OrderBy(e => e.ToString());
			List<TEnum> enumValList = new List<TEnum>(enumValArray.Count());

			enumValList.AddRange(enumValArray.Select(val => (TEnum)Enum.Parse(enumType, val.ToString())));
			return enumValList;
		}

		/// <summary>
		/// Converts string to a enum value.
		/// </summary>
		/// <typeparam name="TEnum">Enum type</typeparam>
		/// <param name="enumName">ToString("F") value of the enumerator type</param>
		/// <returns>Enum value</returns>
		/// <remarks>
		/// Comparison is case-insensitive.
		/// <code lang="C#"><![CDATA[
		/// enum VehicleType
		/// {
		///     Car
		///     Bus
		/// }
		/// VehicleType val = EnumUtility.Parse<VehicleType>("Car")  // returns VehicleType.Car
		/// ]]></code>
		/// </remarks>
		public static TEnum Parse<TEnum>(string enumName) where TEnum : struct
		{
			return Parse<TEnum>(enumName, true);
		}

		/// <summary>
		/// Converts string to a enum value.
		/// </summary>
		/// <typeparam name="TEnum">Enum type</typeparam>
		/// <param name="enumName">ToString("F") value of the enumerator type</param>
		/// <param name="ignoreCase">Performs case-insensitive comparison.</param>
		/// <returns>Enum value</returns>
		public static TEnum Parse<TEnum>(string enumName, bool ignoreCase) where TEnum : struct
		{
			if (string.IsNullOrEmpty(enumName))
				throw new ArgumentNullException(nameof(enumName));

			Type enumType = typeof(TEnum);

			if (!IsEnum(enumType))
				throw new ArgumentException(string.Format(RS.Err_ExpectEnumType, enumType.FullName));

			return (TEnum)Enum.Parse(enumType, enumName, ignoreCase);
		}

		/// <summary>
		/// Converts string to a enum value.
		/// </summary>
		public static TEnum TryParse<TEnum>(string enumName, TEnum defaultValue) where TEnum : struct
		{
			return TryParse<TEnum>(enumName, defaultValue, true);
		}

		/// <summary>
		/// Converts string to a enum value.
		/// </summary>
		public static TEnum TryParse<TEnum>(string enumName, TEnum defaultValue, bool ignoreCase) where TEnum : struct
		{			
			try
			{
				return Parse<TEnum>(enumName, ignoreCase);
			}
			catch
			{ }

			return defaultValue;
		}

#if !NETSTANDARD
        /// <summary>Gets an attribute on an enum field value.</summary>
        /// <returns>The description belonging to the enum option, as a string</returns>
        /// <param name="enumValue">An enum type.</param>
        public static string GetDescription(Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
        }
#endif
    }
}
