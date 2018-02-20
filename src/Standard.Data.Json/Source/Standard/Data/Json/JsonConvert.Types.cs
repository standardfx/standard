using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Runtime.CompilerServices;
using System.Security;
using Standard;
using System.Xml.Serialization;

#if !NET35
using System.Collections.Concurrent;
using System.Dynamic;
#endif

#if !(PORTABLE || NETSTANDARD)
using System.Security.Permissions;
#endif

namespace Standard.Data.Json
{
	partial class JsonConvert
	{
		/// <summary>
		/// Delegate to override what member can get serialized.
		/// </summary>
		public static Func<MemberInfo, bool> CanSerialize
		{
			private get;
			set;
		}

		/// <summary>
		/// Delegate to override what name to use for members when serialized.
		/// </summary>
		public static Func<MemberInfo, string> SerializeAs
		{
			private get;
			set;
		}

		private static bool IsPrimitiveType(this Type type)
		{
			return _primitiveTypes.GetOrAdd(type, key => 
			{
				lock (GetDictLockObject("IsPrimitiveType"))
				{
					if (key.GetTypeInfo().IsGenericType && key.GetGenericTypeDefinition() == _nullableType)
						key = key.GetGenericArguments()[0];

					return key == _stringType ||
						key.GetTypeInfo().IsPrimitive || 
						key == _dateTimeType ||
						key == _dateTimeOffsetType ||
						key == _decimalType || 
						key == _timeSpanType ||
						key == _guidType || 
						key == _charType ||
						key == _typeType ||
						key.GetTypeInfo().IsEnum || 
						key == _byteArrayType;
				}
			});
		}

		private static Type GetNullableType(this Type type)
		{
			return _nullableTypes.GetOrAdd(type, key => 
			{
				lock (GetDictLockObject("GetNullableType"))
				{
					return key.Name.StartsWith("Nullable`") 
						? key.GetGenericArguments()[0] 
						: null;
				}
			});
		}

		private static bool GetCanSerialize(MemberInfo memberInfo)
		{
			if (CanSerialize != null)
				return CanSerialize(memberInfo);

			return true;
		}

		private static JsonPropertyAttribute GetSerializeAs(MemberInfo memberInfo)
		{
			JsonPropertyAttribute attr = null;
			if (SerializeAs != null)
			{
				var name = SerializeAs(memberInfo);
				if (!string.IsNullOrEmpty(name))
					attr = new JsonPropertyAttribute(name);
			}

			LookupAttribute<XmlAttributeAttribute>(ref attr, memberInfo, it => it.AttributeName);
			LookupAttribute<XmlElementAttribute>(ref attr, memberInfo, it => it.ElementName);
			LookupAttribute<XmlArrayAttribute>(ref attr, memberInfo, it => it.ElementName);

			return attr;
		}

		private static void LookupAttribute<T>(ref JsonPropertyAttribute attr, MemberInfo memberInfo, Func<T, string> func) where T : Attribute
		{
			if (attr != null)
				return;
			var memberAttr = memberInfo.GetCustomAttributes(typeof(T), true).OfType<T>().FirstOrDefault();
			if (memberAttr == null)
				return;

			attr = new JsonPropertyAttribute(func(memberAttr));
		}

		internal static JsonMemberInfo[] GetTypeProperties(this Type type)
		{
			return _typeProperties.GetOrAdd(type, key => 
			{
				lock (GetDictLockObject("GetTypeProperties"))
				{
					var props = key.GetProperties(PropertyBinding)
						.Where(x => x.GetIndexParameters().Length == 0 && GetCanSerialize(x))
						.Select(x =>
						{
							var attr = x.GetCustomAttributes(_jsonPropertyType, true).OfType<JsonPropertyAttribute>().FirstOrDefault();

							if (attr == null)
								attr = GetSerializeAs(x);

							return new JsonMemberInfo
							{
								Member = x,
								Attribute = x.GetCustomAttributes(_jsonPropertyType, true).OfType<JsonPropertyAttribute>().FirstOrDefault()
							};
						});

					if (_includeFields)
						props = props
							.Union(
								key
								.GetFields(PropertyBinding)
								.Where(x => GetCanSerialize(x))
								.Select(x => new JsonMemberInfo {
									Member = x,
									Attribute = x
										.GetCustomAttributes(_jsonPropertyType, true)
										.OfType<JsonPropertyAttribute>()
										.FirstOrDefault() ?? GetSerializeAs(x)
								})
							);

					var result = props.ToArray();

					if (result.Where(x => x.Attribute != null).Any(x => StringUtility.IsNullOrWhiteSpace(x.Attribute.Name) || x.Attribute.Name.Contains(" ")))
						throw new InvalidJsonPropertyException();

					return result;
				}
			});
		}

		internal static object GetTypeIdentifierInstance(string typeName)
		{
			return _typeIdentifierFuncs.GetOrAdd(typeName, _ => 
			{
				lock (GetDictLockObject("GetTypeIdentifier"))
				{
					var type = Type.GetType(typeName, throwOnError: false);
					if (type == null)
						throw new InvalidOperationException(string.Format(RS.ResolveWithValueError, TypeIdentifier, typeName));

					var ctor = type.GetConstructor(Type.EmptyTypes);

					var meth = new DynamicMethod(Guid.NewGuid().ToString("N"), _objectType, null, restrictedSkipVisibility: true);

					var il = meth.GetILGenerator();

					if (ctor == null)
						il.Emit(OpCodes.Call, _getUninitializedInstance.MakeGenericMethod(type));
					else
						il.Emit(OpCodes.Newobj, ctor);//NewObjNoctor

					il.Emit(OpCodes.Ret);

					return meth.CreateDelegate(typeof(Func<object>)) as Func<object>;
				}
			})();
		}

		internal static bool IsListType(this Type type)
		{
			Type interfaceType = null;
			//Skip type == typeof(String) since String is same as IEnumerable<Char>
			return type != _stringType && (
				_listType.IsAssignableFrom(type) || 
				type.Name == IListStr ||
				(type.Name == ICollectionStr && type.GetGenericArguments()[0].Name != KeyValueStr) ||
				(type.Name == IEnumerableStr && type.GetGenericArguments()[0].Name != KeyValueStr) ||
				((interfaceType = type.GetInterface(ICollectionStr)) != null && interfaceType.GetGenericArguments()[0].Name != KeyValueStr) ||
				((interfaceType = type.GetInterface(IEnumerableStr)) != null && interfaceType.GetGenericArguments()[0].Name != KeyValueStr));
		}

		internal static bool IsDictionaryType(this Type type)
		{
			Type interfaceType = null;
			return _dictType.IsAssignableFrom(type) || 
				type.Name == IDictStr || 
				((interfaceType = type.GetInterface(IEnumerableStr)) != null && interfaceType.GetGenericArguments()[0].Name == KeyValueStr);
		}

		private static bool IsStringBasedType(this Type type)
		{
			var nullableType = type.GetNullableType() ?? type;
			type = nullableType;
			return type == _stringType || 
				type == _charType || 
				type == _typeType || 
				type == _timeSpanType || 
				type == _byteArrayType || 
				type == _guidType;
		}

		private static void LoadDefaultValueByType(ILGenerator il, Type type)
		{
			if (type == _intType)
			{
				il.Emit(OpCodes.Ldc_I4_0);
			}
			else if (type == _sbyteType || type == _byteType || type == typeof(short) || type == typeof(ushort))
			{
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(_byteType == type ? OpCodes.Conv_U1 :
					_sbyteType == type ? OpCodes.Conv_I1 :
					typeof(short) == type ? OpCodes.Conv_I2 : OpCodes.Conv_U2);
			}
			else if (type == typeof(uint))
			{
				il.Emit(OpCodes.Ldc_I4_0);
			}
			else if (type == _charType)
			{
				il.Emit(OpCodes.Ldc_I4_0);
			}
			else if (type == typeof(long))
			{
				il.Emit(OpCodes.Ldc_I8, 0L);
			}
			else if (type == typeof(ulong))
			{
				il.Emit(OpCodes.Ldc_I8, 0L);
			}
			else if (type == typeof(double))
			{
				il.Emit(OpCodes.Ldc_R8, 0d);
			}
			else if (type == typeof(float))
			{
				il.Emit(OpCodes.Ldc_R4, 0f);
			}
			else if (type == _dateTimeType)
			{
				il.Emit(OpCodes.Ldsfld, _dateTimeType.GetField("MinValue"));
			}
			else if (type == _dateTimeOffsetType)
			{
				il.Emit(OpCodes.Ldsfld, _dateTimeOffsetType.GetField("MinValue"));
			}
			else if (type == _timeSpanType)
			{
				il.Emit(OpCodes.Ldsfld, _timeSpanType.GetField("MinValue"));
			}
			else if (type == _boolType)
			{
				il.Emit(OpCodes.Ldc_I4_0);
			}
			else if (type == _guidType)
			{
				il.Emit(OpCodes.Ldsfld, _guidEmptyGuid);
			}
			else if (type == _decimalType)
			{
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Newobj, _decimalType.GetConstructor(new[] { _intType }));
			}
			else if (type.GetTypeInfo().IsEnum)
			{
				il.Emit(OpCodes.Ldc_I4_0);
			}
		}

		private static string GetName(this Type type)
		{
			var sb = new StringBuilder();
			var arguments = !type.GetTypeInfo().IsGenericType 
				? Type.EmptyTypes 
				: type.GetGenericArguments();

			if (!type.GetTypeInfo().IsGenericType)
			{
				sb.Append(type.Name);
			}
			else
			{
				sb.Append(type.Name);
				foreach (var argument in arguments)
				{
					sb.Append(GetName(argument));
				}
			}
			return sb.ToString();
		}

		private static List<Type> GetIncludedTypeTypes(Type type)
		{
			var pTypes = _includedTypeTypes.GetOrAdd(type, _ => 
			{
				lock (GetDictLockObject("GetIncludeTypeTypes"))
				{
					var attrs = type.GetTypeInfo().GetCustomAttributes(typeof(JsonTypeAttribute), true).OfType<JsonTypeAttribute>();
					var types = attrs.Any() 
						? attrs.Where(x => !x.Type.GetTypeInfo().IsAbstract).Select(x => x.Type).ToList() 
						: null;

					// Expense call to auto-magically figure all subclass of current type
					if (types == null)
					{
						types = new List<Type>();

						// LoadedAssemblies:
						//
						// Querying this property will cause loaded assemblies to be automatically resolved. See code for
						// this property.
						//
						// For netstandard1.3, you need to specify loaded assemblies and entry assembly names manually

						foreach (var asm in LoadedAssemblies)
						{
							try
							{
								types.AddRange(asm.GetTypes().Where(x => x.GetTypeInfo().IsSubclassOf(type) || x.GetTypeInfo().GetInterfaces().Any(i => i == type)));
							}
							catch (ReflectionTypeLoadException ex)
							{
								var exTypes = ex.Types != null 
									? ex.Types.Where(x => x != null && x.GetTypeInfo().IsSubclassOf(type)) 
									: null;
								if (exTypes != null)
									types.AddRange(exTypes);
							}
						}
					}

					if (!types.Contains(type) && !type.GetTypeInfo().IsAbstract)
						types.Insert(0, type);

					return types;
				}
			});
			return pTypes;
		}
	}
}
