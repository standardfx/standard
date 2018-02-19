using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using Standard;

namespace Standard.Data.Json
{
#if (PORTABLE || NETSTANDARD)
    internal enum MemberTypes
    {
        Property = 0,
        Field = 1,
        Event = 2,
        Method = 3,
        Other = 4
    }
#endif

	internal static class ReflectionPolyfillExtension
	{
		public static MemberTypes GetMemberType(this MemberInfo memberInfo)
		{
#if (PORTABLE || NETSTANDARD)
			if (memberInfo is PropertyInfo)
                return MemberTypes.Property;
            else if (memberInfo is FieldInfo)
                return MemberTypes.Field;
            else if (memberInfo is EventInfo)
                return MemberTypes.Event;
            else if (memberInfo is MethodInfo)
                return MemberTypes.Method;
            else
                return MemberTypes.Other;
#else
			return memberInfo.MemberType;
#endif
		}

#if (PORTABLE || NETSTANDARD)
		public static IEnumerable<Type> GetInterfaces(this TypeInfo typeinfo)
		{
			return typeinfo.ImplementedInterfaces;
		}

		public static Type GetInterface(this Type type, string name)
		{
			for (Type currentType = type; currentType != null; currentType = currentType.GetTypeInfo().BaseType)
			{
				IEnumerable<Type> interfaces = currentType.GetTypeInfo().GetInterfaces();
				foreach (Type i in interfaces)
				{
					if (i.Name == name) // || (i != null && i.ImplementInterface(interfaceType)))
						return i;
				}
			}

			return null;
		}

		public static IEnumerable<FieldInfo> GetFields(this Type type)
		{
			return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
		}

		public static IEnumerable<FieldInfo> GetFields(this Type type, BindingFlags bindingFlags)
		{
			IList<FieldInfo> fields = (bindingFlags.HasFlag(BindingFlags.DeclaredOnly))
				? type.GetTypeInfo().DeclaredFields.ToList()
				: type.GetTypeInfo().GetFieldsRecursive();

			return fields.Where(f => TestAccessibility(f, bindingFlags)).ToList();
		}

		private static IList<FieldInfo> GetFieldsRecursive(this TypeInfo type)
		{
			TypeInfo t = type;
			IList<FieldInfo> fields = new List<FieldInfo>();
			while (t != null)
			{
				foreach (FieldInfo member in t.DeclaredFields)
				{
					if (!fields.Any(p => p.Name == member.Name))
						fields.Add(member);
				}
				t = (t.BaseType != null) ? t.BaseType.GetTypeInfo() : null;
			}

			return fields;
		}

		private static bool TestAccessibility(PropertyInfo member, BindingFlags bindingFlags)
		{
			if (member.GetMethod != null && TestAccessibility(member.GetMethod, bindingFlags))
				return true;

			if (member.SetMethod != null && TestAccessibility(member.SetMethod, bindingFlags))
				return true;

			return false;
		}

		private static bool TestAccessibility(MemberInfo member, BindingFlags bindingFlags)
		{
			if (member is FieldInfo)
				return TestAccessibility((FieldInfo)member, bindingFlags);
			else if (member is MethodBase)
				return TestAccessibility((MethodBase)member, bindingFlags);
			else if (member is PropertyInfo)
				return TestAccessibility((PropertyInfo)member, bindingFlags);

			throw new Exception("Unexpected member type.");
		}

		private static bool TestAccessibility(FieldInfo member, BindingFlags bindingFlags)
		{
			bool visibility = (member.IsPublic && bindingFlags.HasFlag(BindingFlags.Public)) ||
				(!member.IsPublic && bindingFlags.HasFlag(BindingFlags.NonPublic));

			bool instance = (member.IsStatic && bindingFlags.HasFlag(BindingFlags.Static)) ||
				(!member.IsStatic && bindingFlags.HasFlag(BindingFlags.Instance));

			return visibility && instance;
		}

		private static bool TestAccessibility(MethodBase member, BindingFlags bindingFlags)
		{
			bool visibility = (member.IsPublic && bindingFlags.HasFlag(BindingFlags.Public)) ||
				(!member.IsPublic && bindingFlags.HasFlag(BindingFlags.NonPublic));

			bool instance = (member.IsStatic && bindingFlags.HasFlag(BindingFlags.Static)) ||
				(!member.IsStatic && bindingFlags.HasFlag(BindingFlags.Instance));

			return visibility && instance;
		}

		public static Type GetEnumUnderlyingType(this Type type)
		{
			if (!type.GetTypeInfo().IsEnum)
				throw new ArgumentException("MustBeEnum");

			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToArray();
			if (fields == null || fields.Length != 1)
				throw new ArgumentException("Argument_InvalidEnum");

			return fields[0].FieldType;
		}
#endif
	}

	internal static class EmitPolyfillExtension
	{
		public static Type CreateType(this TypeBuilder tb)
		{
			return tb.CreateTypeInfo().AsType();
		}
	}
}
