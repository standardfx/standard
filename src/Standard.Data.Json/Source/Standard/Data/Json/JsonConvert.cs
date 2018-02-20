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

#if !NET35
using System.Collections.Concurrent;
using System.Dynamic;
#endif

#if !(PORTABLE || NETSTANDARD)
using System.Security.Permissions;
#endif

#if NETSTANDARD && !NETSTANDARD1_3
using Microsoft.Extensions.DependencyModel;
#endif

namespace Standard.Data.Json
{
	/// <summary>
	/// A general utility class for JSON serialization.
	/// </summary>
	public static partial class JsonConvert
	{
		#region Private fields

		private const int BUFFER_SIZE = 11;
		private const int BUFFER_SIZE_DIFF = BUFFER_SIZE - 2;

		private readonly static object _lockAsmObject = new object();
		private static object _lockObject = new object();
		private static AssemblyBuilder _assembly = null;
		private static ModuleBuilder _module = null;

		internal const string JSON_GENERATED_ASSEMBLY_NAME = "JSONGeneratedAssembly";

#if !DEBUG && !NETSTANDARD
		internal const string JSON_GENERATED_ASSEMBLY_STRONG_NAME = "JSONGeneratedAssembly, PublicKey=" + JSON_GENERATED_ASSEMBLY_PUBKEY;
		internal const string JSON_GENERATED_ASSEMBLY_KEY = "BwIAAAAkAABSU0EyAAIAAAEAAQBhNlHGhRBZhZsJXmEg3+UUVJraORzlE8BpdTp/0TXmJEiVn6/DsXVmiv+KlO3EvXjCBDe81HTSKqv5NdE1lUyvG+BLaNL+Nkl+TzkDSzjQhQPz5sjc/GYnaZqsgoOV6ckzw23YjNGtIhMBo3oy59jxfokbliPuLb10J5/eAvVB3qFNcIcej3HVydzK30lRgZUHOMS+I+z3+di3io2pZeeV4bezI4FQO0/I/yP7Z8Jw5xQn2B2/OTuO/tP7tl0aRH5TWKrSxAEB3A3Rh2z6mflCjvcKP/z86hPApY9G3G6qs4mxXI5k6imO3jJmU+LnXfxFttXm0QD8Iy6bZRwKbQxMH8wKILs4UL6n6/1fMTIhQ0/jYheYNqlaMTs7CtKaRiQ=";
		internal const string JSON_GENERATED_ASSEMBLY_PUBKEY = "0024000004800000540000000602000000240000525341310002000001000100613651c6851059859b095e6120dfe514549ada391ce513c069753a7fd135e62448959fafc3b175668aff8a94edc4bd78c20437bcd474d22aabf935d135954caf";
		internal const string JSON_GENERATED_ASSEMBLY_PUBKEY_TOKEN = "9887eccdddef65a3";
#endif

		private const int 
			Delimeter = (int)',', 
			ColonChr = (int)':',
			ArrayOpen = (int)'[', 
			ArrayClose = (int)']', 
			ObjectOpen = (int)'{', 
			ObjectClose = (int)'}';

		private const string 
			IsoFormat = "{0:yyyy-MM-ddTHH:mm:ss.fffZ}",
			TypeIdentifier = "$type",
			ClassStr = "Class", 
			_dllStr = ".dll",
			NullStr = "null",
			IListStr = "IList`1",
			IDictStr = "IDictionary`2",
			KeyValueStr = "KeyValuePair`2",
			ICollectionStr = "ICollection`1",
			IEnumerableStr = "IEnumerable`1",
			CreateListStr = "CreateList",
			CreateClassOrDictStr = "CreateClassOrDict",
			Dynamic = "Dynamic",
			ExtractStr = "Extract",
			SetStr = "Set",
			WriteStr = "Write", 
			ReadStr = "Read", 
			ReadEnumStr = "ReadEnum",
			CarrotQuoteChar = "`",
			ArrayStr = "Array", 
			AnonymousBracketStr = "<>",
			ArrayLiteral = "[]",
			Colon = ":",
			ToTupleStr = "ToTuple",
			SerializeStr = "Serialize", 
			DeserializeStr = "Deserialize", 
			SettingsFieldName = "_settingsField";

		[ThreadStatic]
		private static StringBuilder _decodeJsonStringBuilder;

		private const TypeAttributes TypeAttribute =
           TypeAttributes.Public | 
			TypeAttributes.Serializable | 
			TypeAttributes.Sealed;

		private const BindingFlags PropertyBinding = 
			BindingFlags.Instance | 
			BindingFlags.Public;

		private const BindingFlags MethodBinding = 
			BindingFlags.Static | 
			BindingFlags.NonPublic | 
			BindingFlags.Public | 
			BindingFlags.Instance;

		private const MethodAttributes MethodAttribute =
            MethodAttributes.Public |
            MethodAttributes.Virtual |
            MethodAttributes.Final |
            MethodAttributes.HideBySig |
            MethodAttributes.NewSlot |
            MethodAttributes.SpecialName;

		private const MethodAttributes StaticMethodAttribute =
            MethodAttributes.Public |
            MethodAttributes.Static |
            MethodAttributes.HideBySig |
            MethodAttributes.SpecialName;

        internal static readonly Type 
			_dateTimeType = typeof(DateTime),
            _dateTimeOffsetType = typeof(DateTimeOffset),
            _enumType = typeof(Enum),
            _stringType = typeof(string),
            _byteArrayType = typeof(byte[]),
            _charType = typeof(char),
            _charPtrType = typeof(char*),
            _guidType = typeof(Guid),
            _boolType = typeof(bool),
            _byteType = typeof(byte),
            _sbyteType = typeof(sbyte),
            _timeSpanType = typeof(TimeSpan),
            _stringBuilderType = typeof(StringBuilder),
            _listType = typeof(IList),
            _dictType = typeof(IDictionary),
            _dictStringObject = typeof(Dictionary<string, object>),
            _genericDictType = typeof(Dictionary<,>),
            _genericListType = typeof(List<>),
            _objectType = typeof(object),
            _nullableType = typeof(Nullable<>),
            _decimalType = typeof(decimal),
            _genericCollectionType = typeof(ICollection<>),
            _floatType = typeof(float),
            _doubleType = typeof(double),
            _enumeratorTypeNonGeneric = typeof(IEnumerator),
            _idictStringObject = typeof(IDictionary<string, object>),
            _ienumerableType = typeof(IEnumerable<>),
            _enumeratorType = typeof(IEnumerator<>),
            _genericKeyValuePairType = typeof(KeyValuePair<,>),
            _invalidJsonExceptionType = typeof(InvalidJsonException),
            _serializerType = typeof(JsonSerializer<>),
            _expandoObjectType = typeof(ExpandoObject),
            _genericDictionaryEnumerator = typeof(Dictionary<,>.Enumerator),
            _genericListEnumerator = typeof(List<>.Enumerator),
            _typeType = typeof(Type),
            _voidType = typeof(void),
            _intType = typeof(int),
            _shortType = typeof(short),
            _longType = typeof(long),
            _methodInfoType = typeof(MethodBase),
            _textWriterType = typeof(TextWriter),
            _textReaderType = typeof(TextReader),
            _stringComparison = typeof(StringComparison),
			// custom
			_tupleContainerType = typeof(TupleContainer),
			_jsonPropertyType = typeof(JsonPropertyAttribute),
			_jsonType = typeof(JsonConvert),
			_internalJsonType = typeof(JsonSerializingEngine),
			_settingsType = typeof(JsonSerializerSettings);

		internal static Type _jsonSerializerType = typeof(JsonSerializer<>);

		private static MethodInfo
			// clr core
			_stringBuilderToString = _stringBuilderType.GetMethod("ToString", Type.EmptyTypes),
			_stringBuilderAppend = _stringBuilderType.GetMethod("Append", new[] { _stringType }),
			_stringBuilderAppendObject = _stringBuilderType.GetMethod("Append", new[] { _objectType }),
			_stringBuilderAppendChar = _stringBuilderType.GetMethod("Append", new[] { _charType }),
			_stringOpEquality = _stringType.GetMethod("op_Equality", MethodBinding),
			_stringEqualCompare = _stringType.GetMethod("Equals", new[] { _stringType, _stringType, typeof(StringComparison) }),
			_stringConcat = _stringType.GetMethod("Concat", new[] { _objectType, _objectType, _objectType, _objectType }),
			_stringLength = _stringType.GetMethod("get_Length"),
			_stringFormat = _stringType.GetMethod("Format", new[] { _stringType, _objectType }),
			_getChars = _stringType.GetMethod("get_Chars"),
			_textWriterWrite = _textWriterType.GetMethod("Write", new[] { _stringType }),
			_textReaderReadToEnd = _textReaderType.GetMethod("ReadToEnd"),
			_dictSetItem = _dictType.GetMethod("set_Item"),
			_methodGetMethodFromHandle = _methodInfoType.GetMethod("GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) }),
			_iDisposableDispose = typeof(IDisposable).GetMethod("Dispose"),
			_typeopEquality = _typeType.GetMethod("op_Equality", MethodBinding),
			_assemblyQualifiedName = _typeType.GetProperty("AssemblyQualifiedName").GetGetMethod(),
			_typeGetTypeFromHandle = _typeType.GetMethod("GetTypeFromHandle", MethodBinding),
			_objectGetType = _objectType.GetMethod("GetType", MethodBinding),
			_objectEquals = _objectType.GetMethod("Equals", new[] { _objectType }),
			_objectToString = _objectType.GetMethod("ToString", Type.EmptyTypes),
			// custom
			_toExpectedType = typeof(AutomaticTypeConverter).GetMethod("ToExpectedType"),
			_tupleContainerAdd = _tupleContainerType.GetMethod("Add"),
			// serialization engine
			_generatorGetStringBuilder = _internalJsonType.GetMethod("GetStringBuilder", MethodBinding),
			_generatorInt32ToStr = _internalJsonType.GetMethod("Int32ToStr", MethodBinding),
			_generatorCharToStr = _internalJsonType.GetMethod("CharToStr", MethodBinding),
			_generatorEnumToStr = _internalJsonType.GetMethod("EnumToStr", MethodBinding),
			_generatorInt64ToStr = _internalJsonType.GetMethod("Int64ToStr", MethodBinding),
			_generatorSingleToStr = _internalJsonType.GetMethod("SingleToStr", MethodBinding),
			_generatorDoubleToStr = _internalJsonType.GetMethod("DoubleToStr", MethodBinding),
			_generatorDecimalToStr = _internalJsonType.GetMethod("DecimalToStr", MethodBinding),
			_generatorDateToStr = _internalJsonType.GetMethod("DateToStr", MethodBinding),
			_generatorDateOffsetToStr = _internalJsonType.GetMethod("DateOffsetToStr", MethodBinding),
			_generatorSByteToStr = _internalJsonType.GetMethod("SByteToStr", MethodBinding),
			_guidToStr = _internalJsonType.GetMethod("GuidToStr", MethodBinding),
			_byteArrayToStr = _internalJsonType.GetMethod("ByteArrayToStr", MethodBinding),
			_convertBase64 = _internalJsonType.GetMethod("ByteArrayToBase64Str", MethodBinding), 
			_convertFromBase64 = _internalJsonType.GetMethod("Base64StrToByteArray", MethodBinding), 
			_getStringBasedValue = _internalJsonType.GetMethod("GetStringBasedValue", MethodBinding),
			_getNonStringValue = _internalJsonType.GetMethod("GetNonStringValue", MethodBinding),
			_isDateValue = _internalJsonType.GetMethod("IsValueDate", MethodBinding),
			_strToInt32 = _internalJsonType.GetMethod("StrToInt32", MethodBinding),
			_strToUInt32 = _internalJsonType.GetMethod("StrToUInt32", MethodBinding),
			_strToUInt16 = _internalJsonType.GetMethod("StrToUInt16", MethodBinding),
			_strToInt16 = _internalJsonType.GetMethod("StrToInt16", MethodBinding),
			_strToByte = _internalJsonType.GetMethod("StrToByte", MethodBinding),
			_strToInt64 = _internalJsonType.GetMethod("StrToInt64", MethodBinding),
			_strToUInt64 = _internalJsonType.GetMethod("StrToUInt64", MethodBinding),
			_strToDecimal = _internalJsonType.GetMethod("StrToDecimal", MethodBinding),
			_strToSingle = _internalJsonType.GetMethod("StrToSingle", MethodBinding),
			_strToDate = _internalJsonType.GetMethod("StrToDate", MethodBinding),
			_strToDateTimeoffset = _internalJsonType.GetMethod("StrToDateTimeoffset", MethodBinding),
			_strToChar = _internalJsonType.GetMethod("StrToChar", MethodBinding),
			_strToDouble = _internalJsonType.GetMethod("StrToDouble", MethodBinding),
			_strToBoolean = _internalJsonType.GetMethod("StrToBoolean", MethodBinding),
			_strToGuid = _internalJsonType.GetMethod("StrToGuid", MethodBinding),
			_strToType = _internalJsonType.GetMethod("StrToType", MethodBinding),
			_moveToArrayBlock = _internalJsonType.GetMethod("MoveToArrayBlock", MethodBinding),
			_strToByteArray = _internalJsonType.GetMethod("StrToByteArray", MethodBinding),
			_listToListObject = _internalJsonType.GetMethod("ListToListObject", MethodBinding),
			_isListType = _internalJsonType.GetMethod("IsListType", MethodBinding),
			_isDictType = _internalJsonType.GetMethod("IsDictionaryType", MethodBinding),
			//_createString = _internalJsonType.GetMethod("CreateString"),
			_isCharTag = _internalJsonType.GetMethod("IsCharTag"),
			_isEndChar = _internalJsonType.GetMethod("IsEndChar", MethodBinding),
			_isArrayEndChar = _internalJsonType.GetMethod("IsArrayEndChar", MethodBinding),
			_flagEnumToStr = _internalJsonType.GetMethod("FlagEnumToStr", MethodBinding),
			_flagStrToEnum = _internalJsonType.GetMethod("FlagStrToEnum", MethodBinding),
			_toStrIfStr = _internalJsonType.GetMethod("ToStrIfStr", MethodBinding),
			_skipProperty = _internalJsonType.GetMethod("SkipProperty", MethodBinding),
			_isRawPrimitive = _internalJsonType.GetMethod("IsRawPrimitive", MethodBinding),
			_isInRange = _internalJsonType.GetMethod("IsInRange", MethodBinding),
			_dateTimeParse = _dateTimeType.GetMethod("Parse", new[] { _stringType }),
			_timeSpanParse = _timeSpanType.GetMethod("Parse", new[] { _stringType }),
			_cTypeOpEquality = _internalJsonType.GetMethod("CustomTypeEquality", MethodBinding),
			_needQuote = _internalJsonType.GetMethod("NeedQuotes", MethodBinding),
			_IsCurrentAQuotMethod = _internalJsonType.GetMethod("IsCurrentAQuot", MethodBinding),
			_getTypeIdentifierInstanceMethod = _internalJsonType.GetMethod("GetTypeIdentifierInstance", MethodBinding),
			_getUninitializedInstance = _internalJsonType.GetMethod("GetUninitializedInstance", MethodBinding),
			_setterPropertyValueMethod = _internalJsonType.GetMethod("SetterPropertyValue", MethodBinding),
			_throwIfInvalidJson = _internalJsonType.GetMethod("ThrowIfInvalidJson", MethodBinding),
			_toCamelCase = _internalJsonType.GetMethod("ToCamelCase", MethodBinding),
			// JsonConvert
			_getSerializerMethod = _jsonType.GetMethod("GetSerializer", BindingFlags.NonPublic | BindingFlags.Static),
			_encodedJsonString = _jsonType.GetMethod("EncodedJsonString", MethodBinding),
			_decodeJsonString = _jsonType.GetMethod("DecodeJsonString", MethodBinding),
			_prettifyJsonIfNeeded = _jsonType.GetMethod("PrettifyJsonIfNeeded", MethodBinding),
			// settings
			_settingsUseEnumStringProp = _settingsType.GetProperty("EnumAsString", MethodBinding).GetGetMethod(),
			_settingsUseStringOptimization = _settingsType.GetProperty("EnumAsString", MethodBinding).GetGetMethod(),
			_settingsHasOverrideQuoteChar = _settingsType.GetProperty("HasOverrideQuoteChar", MethodBinding).GetGetMethod(),
			_settingsDateFormat = _settingsType.GetProperty("DateFormat", MethodBinding).GetGetMethod(),
			_settingsSkipDefaultValue = _settingsType.GetProperty("SkipDefaultValue", MethodBinding).GetGetMethod(),
			_settingsCurrentSettings = _settingsType.GetProperty("Current", MethodBinding).GetGetMethod(),
			_settingsCamelCase = _settingsType.GetProperty("CamelCase", MethodBinding).GetGetMethod();

		private static FieldInfo 
			_guidEmptyGuid = _guidType.GetField("Empty"),
            _settingQuoteChar = _settingsType.GetField("_quoteChar", MethodBinding),
            _settingsCaseComparison = _settingsType.GetField("_caseComparison", MethodBinding),
            _settingQuoteCharString = _settingsType.GetField("_quoteCharString", MethodBinding);

		private static ConstructorInfo _strCtorWithPtr = _stringType.GetConstructor(new[] { typeof(char*), _intType, _intType });
		private static ConstructorInfo _invalidJsonCtor = _invalidJsonExceptionType.GetConstructor(Type.EmptyTypes);
		private static ConstructorInfo _settingsCtor = _settingsType.GetConstructor(Type.EmptyTypes);

		private static Dictionary<Type, MethodInfo> _defaultSerializerTypes = new Dictionary<Type, MethodInfo>
		{
			{ _enumType, null },
            { _stringType, null },
            { _charType, _generatorCharToStr },
            { _intType, _generatorInt32ToStr },
            { _shortType, _generatorInt32ToStr },
            { _longType, _generatorInt64ToStr },
            { _decimalType, _generatorDecimalToStr },
            { _boolType, null },
            { _doubleType, _generatorDoubleToStr },
            { _floatType, _generatorSingleToStr },
            { _byteArrayType, _byteArrayToStr },
            { _guidType, _guidToStr },
            { _objectType, null }
        };

		private static ConcurrentDictionary<string, object> _dictLockObjects = 
			new ConcurrentDictionary<string, object>();

		private static ConcurrentDictionary<Type, MethodInfo> _registeredSerializerMethods = 
			new ConcurrentDictionary<Type, MethodInfo>();

		private static ConcurrentDictionary<Type, Type> _types = 
			new ConcurrentDictionary<Type, Type>();

		private static ConcurrentDictionary<string, MethodInfo> _writeMethodBuilders =
            new ConcurrentDictionary<string, MethodInfo>();

		private static ConcurrentDictionary<string, MethodInfo> _setValueMethodBuilders =
            new ConcurrentDictionary<string, MethodInfo>();

		private static ConcurrentDictionary<string, MethodInfo> _readMethodBuilders =
            new ConcurrentDictionary<string, MethodInfo>();

		private static ConcurrentDictionary<string, MethodInfo> _createListMethodBuilders =
            new ConcurrentDictionary<string, MethodInfo>();

		private static ConcurrentDictionary<string, MethodInfo> _extractMethodBuilders =
            new ConcurrentDictionary<string, MethodInfo>();

		private static ConcurrentDictionary<string, MethodInfo> _readDeserializeMethodBuilders =
            new ConcurrentDictionary<string, MethodInfo>();

		private static ConcurrentDictionary<string, MethodInfo> _writeEnumToStringMethodBuilders =
            new ConcurrentDictionary<string, MethodInfo>();

		private static ConcurrentDictionary<string, MethodInfo> _readEnumToStringMethodBuilders =
            new ConcurrentDictionary<string, MethodInfo>();

		private static readonly ConcurrentDictionary<MethodInfo, Delegate> _setMemberValues = 
			new ConcurrentDictionary<MethodInfo, Delegate>();

		private static ConcurrentDictionary<string, Func<object>> _typeIdentifierFuncs = 
			new ConcurrentDictionary<string, Func<object>>();

		private static ConcurrentDictionary<Type, bool> _primitiveTypes =
            new ConcurrentDictionary<Type, bool>();

		private static ConcurrentDictionary<Type, Type> _nullableTypes =
            new ConcurrentDictionary<Type, Type>();

		private static ConcurrentDictionary<Type, List<Type>> _includedTypeTypes = 
			new ConcurrentDictionary<Type, List<Type>>();

        private static ConcurrentDictionary<Type, object> _serializers = 
			new ConcurrentDictionary<Type, object>();

		private static ConcurrentDictionary<Type, JsonMemberInfo[]> _typeProperties =
            new ConcurrentDictionary<Type, JsonMemberInfo[]>();

		private static ConcurrentDictionary<string, string> _fixes =
            new ConcurrentDictionary<string, string>();

		// Serialization and deserialization

		private static ConcurrentDictionary<string, DeserializeWithTypeDelegate> _deserializeWithTypes = 
			new ConcurrentDictionary<string, DeserializeWithTypeDelegate>();

		private static ConcurrentDictionary<Type, SerializeWithTypeDelegate> _serializeWithTypes = 
			new ConcurrentDictionary<Type, SerializeWithTypeDelegate>();

		private static ConcurrentDictionary<string, DeserializeWithTypeSettingsDelegate> _deserializeWithTypesSettings =
			new ConcurrentDictionary<string, DeserializeWithTypeSettingsDelegate>();

		private static ConcurrentDictionary<Type, SerializeWithTypeSettingsDelegate> _serializeWithTypesSettings =
			new ConcurrentDictionary<Type, SerializeWithTypeSettingsDelegate>();

		private delegate object DeserializeWithTypeDelegate(string value);
		private delegate string SerializeWithTypeDelegate(object value);

		delegate object DeserializeWithTypeSettingsDelegate(string value, JsonSerializerSettings settings);
		delegate string SerializeWithTypeSettingsDelegate(object value, JsonSerializerSettings settings);

		// Setter property

		private delegate void SetterPropertyDelegate<T>(T instance, object value, MethodInfo methodInfo);

		#endregion // Private fields

		#region Assembly generator settings

		private static bool _useSharedAssembly = true;
		private static bool _includeTypeInfo = false;
		private static bool _includeFields = true;
		private static bool _generateAssembly = false;

#if NETSTANDARD && NETSTANDARD1_3
		// NETSTANDARD1_3 will require the consumer to manually specify the entry assembly name and
		// what assemblies are loaded.
		private static Assembly _entryAssembly;
		private static Assembly[] _loadedAssemblies;
#endif

		/// <summary>
		/// Flag to determine whether to store all generated data for types in a single assembly.
		/// </summary>
		public static bool ShareAssembly
		{
			get { return _useSharedAssembly; }
			set { _useSharedAssembly = value; }
		}

		/// <summary>
		/// Enable inclusion of type information for serialization and deserialization.
		/// </summary>
		public static bool IncludeTypeInfo
		{
			get { return _includeTypeInfo; }
			set { _includeTypeInfo = value; }
		}

		/// <summary>
		/// Flag to determine whether to include fields.
		/// </summary>
		public static bool IncludeFields
		{
			get { return _includeFields; }
			set { _includeFields = value; }
		}

		/// <summary>
		/// Flag to determine whether to an assembly should be generated dynamically for serialized data.
		/// </summary>
		public static bool GenerateAssembly
		{
			get { return _generateAssembly; }
			set { _generateAssembly = value; }
		}

		/// <summary>
		/// Gets or sets the process executable in the default application domain.
		/// </summary>
		/// <remarks>
		/// <para>This property is used for compatibility purposes with NETStandard versions below 1.6.</para>
		/// <para>On NETStandard 1.4 and below, there is no API similar to <c>Assembly.GetExecutingAssembly()</c>. When targeting such frameworks, you need 
		/// to set this property manually.</para>
		/// <para>Conversely, this property is managed automatically when targeting NETStandard 1.6. You do not need to set this property 
		/// when targeting NETStandard 1.6.</para>
		/// </remarks>
		public static Assembly EntryAssembly
		{
			get 
			{ 
#if NETSTANDARD && !NETSTANDARD1_3
				return Assembly.GetEntryAssembly(); 
#elif NETSTANDARD && NETSTANDARD1_3
				return _entryAssembly; 
#else
				return Assembly.GetExecutingAssembly();
#endif
			}
			set 
			{ 
#if NETSTANDARD && NETSTANDARD1_3
				_entryAssembly = value; 
#else
				return;
#endif
			}
		}

		/// <summary>
		/// Returns assemblies that have been loaded into the execution context of this application domain.
		/// </summary>
		/// <remarks>
		/// <para>This property is used for compatibility purposes with NETStandard versions below 1.6.</para>
		/// <para>NETStandard below 1.6 cannot automatically resolve loaded assemblies. When targeting such frameworks, you need 
		/// to set this property manually.</para>
		/// <para>Conversely, this property is managed automatically when targeting .NETStandard 1.6. You do not need to set this property 
		/// when targeting NETStandard 1.6.</para>
		/// </remarks>
		public static Assembly[] LoadedAssemblies
		{
			get 
			{ 
				// Microsoft.Extensions.DependencyModel
				//
				// netstandard1.3 does not have DependencyContext.Default nor DependencyContextLoader -> user must specify manually.
				// netstandard1.6 supports DependencyContext.Default

#if NETSTANDARD && NETSTANDARD1_3
				return _loadedAssemblies;
#elif NETSTANDARD && !NETSTANDARD1_3
				return DependencyContext.Default.GetDefaultAssemblyNames().Select(x => Assembly.Load(x)).ToArray();
#else
				return AppDomain.CurrentDomain.GetAssemblies();
#endif
			}
			set
			{
#if NETSTANDARD && NETSTANDARD1_3
				_loadedAssemblies = value;
#else
				// do nothing
#endif				
			}
		}

		#endregion /Assembly generator settings

		#region Internal methods

		private static void LoadQuotChar(ILGenerator il)
		{
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Ldfld, _settingQuoteCharString);
			//il.Emit(OpCodes.Ldsfld, _threadQuoteStringField);
		}

		internal static bool NeedQuotes(Type type, JsonSerializerSettings settings)
		{
            return type == _stringType || 
				type == _charType || 
				type == _guidType || 
				type == _timeSpanType || 
				((type == _dateTimeType || type == _dateTimeOffsetType) && settings.DateFormat != JsonDateTimeHandling.EpochTime) || 
				type == _byteArrayType || 
				(settings.EnumAsString && type.GetTypeInfo().IsEnum);
        }

        internal static string PrettifyJsonIfNeeded(string str, JsonSerializerSettings settings)
		{
            if (settings.Indent == JsonIndentHandling.Prettify)
                return PrettifyJson(str);

			return str;
        }

        internal static unsafe string PrettifyJson(string str)
		{
            var sb = new StringBuilder();
            
            var horizontal = 0;
            var horizontals = new int[10000];
            var hrIndex = -1;
            var returnFlag = false;
            var quote = 0;

            char c;

            fixed (char* chr = str) {
                char* ptr = chr;
                while ((c = *(ptr++)) != '\0')
				{
                    switch (c)
					{
                        case '{':
                        case '[':
                            sb.Append(c);
                            if (quote == 0)
                            {
                                hrIndex++;
                                horizontals[hrIndex] = horizontal;
								returnFlag = true;
                            }
                            break;

                        case '}':
                        case ']':
                            if (quote == 0)
                            {
								returnFlag = false;
                                sb.Append('\n');
                                horizontal = horizontals[hrIndex];
                                hrIndex--;
                                for (var i = 0; i < horizontal; i++)
                                {
                                    sb.Append(' ');
                                }
                            }
                            sb.Append(c);
                            break;

						case ',':
                            sb.Append(c);
                            if (quote == 0)
								returnFlag = true;
                            break;

						default:
                            if (returnFlag)
							{
								returnFlag = false;
                                sb.Append('\n');
                                horizontal = horizontals[hrIndex] + 1;
                                for (var i = 0; i < horizontal; i++)
								{
                                    sb.Append(' ');
                                }
                            }
                            var escaped = *(ptr - 2) == '\\';
                            if (c == '"' && !escaped)
                            {
                                quote++;
                                quote %= 2;
                            }
                            sb.Append(c);
                            break;
                    }

                    horizontal++;
                }
            }

            return sb.ToString();
        }

        internal static unsafe void EncodedJsonString(StringBuilder sb, string str, JsonSerializerSettings settings)
		{
            var quote = settings._quoteChar;
            char c;
            fixed (char* chr = str)
			{
                char* ptr = chr;
                while ((c = *(ptr++)) != '\0')
				{
                    switch (c)
					{
                        //case '"': sb.Append("\\\""); break;
                        case '\\': sb.Append(@"\\"); break;
                        case '\u0000': sb.Append(@"\u0000"); break;
                        case '\u0001': sb.Append(@"\u0001"); break;
                        case '\u0002': sb.Append(@"\u0002"); break;
                        case '\u0003': sb.Append(@"\u0003"); break;
                        case '\u0004': sb.Append(@"\u0004"); break;
                        case '\u0005': sb.Append(@"\u0005"); break;
                        case '\u0006': sb.Append(@"\u0006"); break;
                        case '\u0007': sb.Append(@"\u0007"); break;
                        case '\u0008': sb.Append(@"\b"); break;
                        case '\u0009': sb.Append(@"\t"); break;
                        case '\u000A': sb.Append(@"\n"); break;
                        case '\u000B': sb.Append(@"\u000B"); break;
                        case '\u000C': sb.Append(@"\f"); break;
                        case '\u000D': sb.Append(@"\r"); break;
                        case '\u000E': sb.Append(@"\u000E"); break;
                        case '\u000F': sb.Append(@"\u000F"); break;
                        case '\u0010': sb.Append(@"\u0010"); break;
                        case '\u0011': sb.Append(@"\u0011"); break;
                        case '\u0012': sb.Append(@"\u0012"); break;
                        case '\u0013': sb.Append(@"\u0013"); break;
                        case '\u0014': sb.Append(@"\u0014"); break;
                        case '\u0015': sb.Append(@"\u0015"); break;
                        case '\u0016': sb.Append(@"\u0016"); break;
                        case '\u0017': sb.Append(@"\u0017"); break;
                        case '\u0018': sb.Append(@"\u0018"); break;
                        case '\u0019': sb.Append(@"\u0019"); break;
                        case '\u001A': sb.Append(@"\u001A"); break;
                        case '\u001B': sb.Append(@"\u001B"); break;
                        case '\u001C': sb.Append(@"\u001C"); break;
                        case '\u001D': sb.Append(@"\u001D"); break;
                        case '\u001E': sb.Append(@"\u001E"); break;
                        case '\u001F': sb.Append(@"\u001F"); break;
                        default:
							if (quote == c)
							{
								if (quote == '"')
									sb.Append("\\\"");
								else if (quote == '\'')
									sb.Append("\\\'");
							}
							else
							{
								sb.Append(c);
							}
                            break;
                    }
                }
            }
        }

		internal unsafe static string DecodeJsonString(char* ptr, ref int index, JsonSerializerSettings settings, bool fromObject = false)
		{
			char current = '\0', next = '\0', prev = '\0';
			bool hasQuote = false;
			bool isJustString = index == 0 && !fromObject;
			var sb = (_decodeJsonStringBuilder ?? (_decodeJsonStringBuilder = new StringBuilder())).Clear();

			// Don't process null string
            if ((IntPtr)ptr == IntPtr.Zero)
                return null;

            ptr += index; 
            var startPtr = ptr; 

			while (*ptr != '\0')
			{
				current = *ptr; 

				if (isJustString || hasQuote)
				{
					if (!isJustString && current == settings._quoteChar)
					{
						next = *(++ptr);
						if (next != ',' && 
							next != ' ' && 
							next != ':' && 
							next != '\n' && 
							next != '\r' && 
							next != '\t' && 
							next != ']' && 
							next != '}' && 
							next != '\0')
						{
							throw new InvalidJsonException();
						}

						break;
					}

					if (current != '\\')
					{
						sb.Append(current);
						prev = current; 
						++ptr; 
						continue;
					}

					next = *(++ptr); 

					switch (next)
					{
						case '\0': 
							// string ends with '\' 
							goto EXIT_DECODE_JSON_STRING_LOOP; 

						case 'r':
							sb.Append('\r');
							break;

						case 'n':
							sb.Append('\n');
							break;

						case 't':
							sb.Append('\t');
							break;

						case 'f':
							sb.Append('\f');
							break;

						case '\\':
							sb.Append('\\');
							break;

						case '/':
							sb.Append('/');
							break;

						case 'b':
							sb.Append('\b');
							break;

						case 'u':
							int unicode = 0; 
							for (int i = 0; i < 4; i++) 
							{
								var c = *(++ptr); 
								if (c >= '0' && c <= '9')
									unicode = (unicode <<= 4) + (c - '0'); 
								else if (c >= 'a' && c <= 'f')
									unicode = (unicode <<= 4) + c - ('a' - 10); 
								else if (c >= 'A' && c <= 'F')
									unicode = (unicode <<= 4) + c - ('A' - 10); 
								else
									throw new InvalidJsonException("Invalid Unicode escape sequence", (int)(ptr - startPtr)); 
							}

							sb.Append((char)unicode); 
							break;

						default:
							if (next == settings._quoteChar)
								sb.Append(next);
							break;
					}
				}
				else
				{
					if (current == settings._quoteChar)
					{
						hasQuote = true;
					}
					else if (current == 'n')
					{
						ptr += 3;
						index += (int)(ptr - startPtr); 
						return null;
					}
				}

				prev = current;
				++ptr;
			}

			// return
EXIT_DECODE_JSON_STRING_LOOP:
			index += (int)(ptr - startPtr); 
			return sb.ToString();
		}

		private static string Fix(this string name)
		{
            return _fixes.GetOrAdd(name, key => 
			{
                lock (GetDictLockObject("FixName"))
				{
                    var index = key.IndexOf(CarrotQuoteChar, StringComparison.OrdinalIgnoreCase);
                    var quoteText = index > -1 ? key.Substring(index, 2) : CarrotQuoteChar;
                    var value = key.Replace(quoteText, string.Empty).Replace(ArrayLiteral, ArrayStr).Replace(AnonymousBracketStr, string.Empty);
                    if (value.Contains(CarrotQuoteChar))
                        value = Fix(value);
                    return value;
                }
            });
        }

        internal static MethodInfo DefineMethodEx(this TypeBuilder builder, string methodName, MethodAttributes methodAttribute, Type returnType, Type[] parameterTypes)
        {
			if (builder == null)
				return new DynamicMethod(methodName, returnType, parameterTypes, EntryAssembly.ManifestModule, true);

            return builder.DefineMethod(methodName, methodAttribute, returnType, parameterTypes);
        }

        internal static ILGenerator GetILGenerator(this MethodInfo methodInfo)
        {
            var dynamicMethod = methodInfo as DynamicMethod;
            return dynamicMethod != null ? 
				dynamicMethod.GetILGenerator() 
				: (methodInfo as MethodBuilder).GetILGenerator();
        }

        private static object GetDictLockObject(params string[] keys)
		{
            return _dictLockObjects.GetOrAdd(string.Concat(keys), new object());
        }

        internal static JsonSerializer<T> GetSerializer<T>()
		{
            return CachedJsonSerializer<T>.Serializer;
        }

		internal static void SetterPropertyValue<T>(T instance, object value, MethodInfo methodInfo)
		{
			(_setMemberValues.GetOrAdd(methodInfo, key =>
			{
				lock (GetDictLockObject("SetDynamicMemberValue"))
				{
					var propType = key.GetParameters()[0].ParameterType;

					var type = key.DeclaringType;

					var name = String.Concat(type.Name, "_", key.Name);
					var meth = new DynamicMethod(name + "_setPropertyValue", _voidType, 
						new[] { type, _objectType, _methodInfoType },
						restrictedSkipVisibility: true);

					var il = meth.GetILGenerator();

					il.Emit(OpCodes.Ldarg_0);

					il.Emit(OpCodes.Ldarg_1);

					if (propType.GetTypeInfo().IsValueType)
						il.Emit(OpCodes.Unbox_Any, propType);
					else
						il.Emit(OpCodes.Isinst, propType);

					il.Emit(OpCodes.Callvirt, key);

					il.Emit(OpCodes.Ret);

					return meth.CreateDelegate(typeof(SetterPropertyDelegate<T>));
				}
			}) as SetterPropertyDelegate<T>)(instance, value, methodInfo);
		}

		private unsafe static void MoveToNextKey(char* str, ref int index)
		{
			var current = *(str + index);
			while (current != ':')
			{
				index++;
				current = *(str + index);
			}
			index++;
		}

		#endregion /Internal methods

		#region Public methods

		/// <summary>
		/// Serialize an object to JSON string, using the specified type and current settings.
		/// </summary>
		/// <param name="type">The type information of <paramref name="value"/>.</param>
		/// <param name="value">An object to serialize.</param>
		/// <returns>The serialized result as an instance of <paramref name="value"/>.</returns>
		public static string Serialize(Type type, object value)
		{
            return _serializeWithTypes.GetOrAdd(type, _ => 
			{
                lock (GetDictLockObject("SerializeType", type.Name))
				{
                    var name = string.Concat(SerializeStr, type.FullName);
                    var method = new DynamicMethod(name, _stringType, new[] { _objectType }, restrictedSkipVisibility: true);

                    var il = method.GetILGenerator();
                    var genericMethod = _getSerializerMethod.MakeGenericMethod(type);
                    var genericType = _jsonSerializerType.MakeGenericType(type);

                    var genericSerialize = genericType.GetMethod(SerializeStr, new[] { type });

                    il.Emit(OpCodes.Call, genericMethod);

                    il.Emit(OpCodes.Ldarg_0);
                    if (type.GetTypeInfo().IsClass)
                        il.Emit(OpCodes.Isinst, type);
                    else
						il.Emit(OpCodes.Unbox_Any, type);

                    il.Emit(OpCodes.Callvirt, genericSerialize);

                    il.Emit(OpCodes.Ret);

                    return method.CreateDelegate(typeof(SerializeWithTypeDelegate)) as SerializeWithTypeDelegate;
                }
            })(value);
        }

		/// <summary>
		/// Serialize an object to JSON string, using the specified type and settings.
		/// </summary>
		/// <param name="type">The type information of <paramref name="value"/>.</param>
		/// <param name="value">An object to serialize.</param>
		/// <param name="settings">The settings to use for serialization.</param>
		/// <returns>The serialized result as an instance of <paramref name="value"/>.</returns>
		public static string Serialize(Type type, object value, JsonSerializerSettings settings)
		{
			return _serializeWithTypesSettings.GetOrAdd(type, _ => 
			{
				lock (GetDictLockObject("SerializeTypeSetting", type.Name))
				{
					var name = string.Concat(SerializeStr + "Settings", type.FullName);
					var method = new DynamicMethod(name, _stringType, 
						new[] { _objectType, _settingsType }, 
						restrictedSkipVisibility: true);

					var il = method.GetILGenerator();
					var genericMethod = _getSerializerMethod.MakeGenericMethod(type);
					var genericType = _jsonSerializerType.MakeGenericType(type);

					var genericSerialize = genericType.GetMethod(SerializeStr, new[] { type, _settingsType });

					il.Emit(OpCodes.Call, genericMethod);

					il.Emit(OpCodes.Ldarg_0);

					if (type.GetTypeInfo().IsClass)
						il.Emit(OpCodes.Isinst, type);
					else
						il.Emit(OpCodes.Unbox_Any, type);

					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Callvirt, genericSerialize);

					il.Emit(OpCodes.Ret);

					return method.CreateDelegate(typeof(SerializeWithTypeSettingsDelegate)) as SerializeWithTypeSettingsDelegate;
				}
			})(value, settings);
		}

		/// <summary>
		/// Serialize specified object to JSON string, using current settings.
		/// </summary>
		/// <param name="value">An instance of an object to serialize.</param>
		public static string Serialize(object value)
		{
            return Serialize(value.GetType(), value);
        }

		/// <summary>
		/// Serialize a value of type <typeparamref name="T"/> to JSON string, using current settings.
		/// </summary>
        /// <typeparam name="T">The type of object to serialize into.</typeparam>
		/// <param name="value">An instance of <typeparamref name="T"/> to serialize.</param>
        public static string Serialize<T>(T value)
		{
           return GetSerializer<T>().Serialize(value);
        }

		/// <summary>
		/// Serialize specified <typeparamref name="T"/> to JSON string, using specified settings.
		/// </summary>
        /// <typeparam name="T">The type of object to serialize into.</typeparam>
		/// <param name="value">An instance of <typeparamref name="T"/> to serialize.</param>
        /// <param name="settings">A <see cref="JsonSerializerSettings"/> instance containing serialization settings.</param>
        public static string Serialize<T>(T value, JsonSerializerSettings settings)
		{
            return GetSerializer<T>().Serialize(value, settings);
        }

		/// <summary>
		/// Serialize specified <typeparamref name="T"/> to writer, using current settings.
		/// </summary>
        /// <typeparam name="T">The type of object to serialize into.</typeparam>
		/// <param name="value">An instance of <typeparamref name="T"/> to serialize.</param>
		/// <param name="writer">The writer to use for serialization.</param>
		public static void Serialize<T>(T value, TextWriter writer)
		{
			GetSerializer<T>().Serialize(value, writer);
		}

		/// <summary>
		/// Serialize specified <typeparamref name="T"/> to writer, using specified settings.
		/// </summary>
        /// <typeparam name="T">The type of object to serialize into.</typeparam>
		/// <param name="value">An instance of <typeparamref name="T"/> to serialize.</param>
		/// <param name="writer">The writer to use for serialization.</param>
        /// <param name="settings">A <see cref="JsonSerializerSettings"/> instance containing serialization settings.</param>
		public static void Serialize<T>(T value, TextWriter writer, JsonSerializerSettings settings)
		{
            GetSerializer<T>().Serialize(value, writer, settings);
        }

		/// <summary>
		/// Deserialize a JSON string to an object, using the specified type and current settings.
		/// </summary>
		/// <param name="type">The type of object to deserialize into.</param>
		/// <param name="value">The JSON string to parse.</param>
        /// <returns>The deserialized result as an instance of <paramref name="type"/>.</returns>
		public static object Deserialize(Type type, string value)
		{
			return _deserializeWithTypes.GetOrAdd(type.FullName, _ =>
			{
				lock (GetDictLockObject("DeserializeType", type.Name))
				{
					var name = string.Concat(DeserializeStr, type.FullName);
					var method = new DynamicMethod(name, _objectType, new[] { _stringType }, restrictedSkipVisibility: true);

					var il = method.GetILGenerator();
					var genericMethod = _getSerializerMethod.MakeGenericMethod(type);
					var genericType = _jsonSerializerType.MakeGenericType(type);

					var genericDeserialize = genericType.GetMethod(DeserializeStr, new[] { _stringType });

					il.Emit(OpCodes.Call, genericMethod);

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Callvirt, genericDeserialize);

					if (type.GetTypeInfo().IsClass)
						il.Emit(OpCodes.Isinst, type);
					else
						il.Emit(OpCodes.Box, type);

					il.Emit(OpCodes.Ret);

					return method.CreateDelegate(typeof(DeserializeWithTypeDelegate)) as DeserializeWithTypeDelegate;
				}
			})(value);
		}

		/// <summary>
		/// Deserialize a JSON string to an object, using the specified type and settings.
		/// </summary>
		/// <param name="type">The type of object to deserialize into.</param>
		/// <param name="value">The JSON string to parse.</param>
		/// <param name="settings">The settings to use for deserialization.</param>
		/// <returns>The deserialized result as an instance of <paramref name="type"/>.</returns>
		public static object Deserialize(Type type, string value, JsonSerializerSettings settings)
		{
			return _deserializeWithTypesSettings.GetOrAdd(type.FullName, _ => 
			{
				lock (GetDictLockObject("DeserializeTypeSettings", type.Name))
				{
					var name = string.Concat(DeserializeStr + "Settings", type.FullName);
					var method = new DynamicMethod(name, _objectType, 
						new[] { _stringType, _settingsType }, 
						restrictedSkipVisibility: true);

					var il = method.GetILGenerator();
					var genericMethod = _getSerializerMethod.MakeGenericMethod(type);
					var genericType = _jsonSerializerType.MakeGenericType(type);

					var genericDeserialize = genericType.GetMethod(DeserializeStr, new[] { _stringType, _settingsType });

					il.Emit(OpCodes.Call, genericMethod);

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Callvirt, genericDeserialize);

					if (type.GetTypeInfo().IsClass)
						il.Emit(OpCodes.Isinst, type);
					else
						il.Emit(OpCodes.Box, type);

					il.Emit(OpCodes.Ret);

					return method.CreateDelegate(typeof(DeserializeWithTypeSettingsDelegate)) as DeserializeWithTypeSettingsDelegate;
				}
			})(value, settings);
		}

		/// <summary>
		/// Deserialize JSON to <typeparamref name="T"/>, using current settings.
		/// </summary>
		/// <typeparam name="T">The type of object to deserialize into.</typeparam>
		/// <param name="json">The JSON string to parse.</param>
		/// <returns>The deserialized result as an instance of <typeparamref name="T"/>.</returns>
		public static T Deserialize<T>(string json)
		{
			return GetSerializer<T>().Deserialize(json);
		}

		/// <summary>
		/// Deserialize reader content to <typeparamref name="T"/>, using current settings.
		/// </summary>
        /// <typeparam name="T">The type of object to deserialize into.</typeparam>
        /// <param name="reader">A reader that holds the JSON source.</param>
        /// <returns>The deserialized result as an instance of <typeparamref name="T"/>.</returns>
		public static T Deserialize<T>(TextReader reader)
		{
			return GetSerializer<T>().Deserialize(reader);
		}

		/// <summary>
		/// Deserialize JSON to <typeparamref name="T"/>, using specified settings.
		/// </summary>
        /// <typeparam name="T">The type of object to deserialize into.</typeparam>
		/// <param name="json">The JSON string to parse.</param>
        /// <param name="settings">A <see cref="JsonSerializerSettings"/> instance containing serialization settings.</param>
        /// <returns>The deserialized result as an instance of <typeparamref name="T"/>.</returns>
		public static T Deserialize<T>(string json, JsonSerializerSettings settings)
		{
            return GetSerializer<T>().Deserialize(json, settings);
        }

        /// <summary>
        /// Deserialize the content of a reader to <typeparamref name="T"/>, using specified settings.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize into.</typeparam>
        /// <param name="reader">A reader that holds the JSON source.</param>
        /// <param name="settings">A <see cref="JsonSerializerSettings"/> instance containing serialization settings.</param>
        /// <returns>The deserialized result as an instance of <typeparamref name="T"/>.</returns>
        public static T Deserialize<T>(TextReader reader, JsonSerializerSettings settings)
		{
            return GetSerializer<T>().Deserialize(reader, settings);
        }

        /// <summary>
        /// Deserialize json into Dictionary[string, object]
        /// </summary>
        /// <param name="json">The JSON string to parse.</param>
        /// <returns>A <c>Dictionary[string, object]</c> representation of the parsed JSON result.</returns>
        public static object DeserializeObject(string json)
		{
            return GetSerializer<object>().Deserialize(json);
        }

		/// <summary>
		/// Register serializer primitive method for <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The serializer type.</typeparam>
		/// <param name="serializeFunc">The serialization logic.</param>
		public static void RegisterTypeSerializer<T>(Func<T, string> serializeFunc)
		{
			var type = typeof(T);

			if (serializeFunc == null)
				throw new InvalidOperationException("serializeFunc cannot be null");

#if !NETSTANDARD
			var method = serializeFunc.Method;
#else
			 var method = serializeFunc.GetMethodInfo();
#endif

			if (!(method.IsPublic && method.IsStatic))
				throw new InvalidOperationException("serializeFun must be a public and static method");

			_registeredSerializerMethods[type] = method;
		}

		/// <summary>
		/// Emit an assembly containing the specified types at runtime.
		/// </summary>
		public static void GenerateTypesInto(string asmName, params Type[] types)
		{
			if (!types.Any())
				throw new JsonAssemblyGeneratorException(asmName);

			var assembly = GenerateAssemblyBuilderNoShare(asmName);
			var module = GenerateModuleBuilder(assembly);

			foreach (var type in types)
			{
#if NETSTANDARD
				GenerateTypeBuilder(type, module).CreateTypeInfo().AsType();
#else
				GenerateTypeBuilder(type, module).CreateType();
#endif
			}

#if !NETSTANDARD && !NETSTANDARD2
			assembly.Save(String.Concat(assembly.GetName().Name, _dllStr));
#endif
		}

		#endregion /Public methods
	}
}
