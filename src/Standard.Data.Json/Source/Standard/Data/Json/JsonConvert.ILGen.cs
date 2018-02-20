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

namespace Standard.Data.Json
{
	partial class JsonConvert
	{
		/// <summary>
		/// current = *(ptr + index);
		/// </summary>
		/// <param name="il"></param>
		/// <param name="current"></param>
		/// <param name="ptr"></param>
		private static void GenerateUpdateCurrent(ILGenerator il, LocalBuilder current, LocalBuilder ptr)
		{
			//current = *(ptr + index);
			il.Emit(OpCodes.Ldloc, ptr);
			il.Emit(OpCodes.Ldarg_1);
			//Extract direct value of ref value
			il.Emit(OpCodes.Ldind_I4);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Ldc_I4_2);
			il.Emit(OpCodes.Mul);
			il.Emit(OpCodes.Add);
			il.Emit(OpCodes.Ldind_U2);
			il.Emit(OpCodes.Stloc, current);
		}

		private static ModuleBuilder GenerateModuleBuilderNoShare(AssemblyBuilder assembly)
		{
			var module = assembly.DefineDynamicModule(string.Concat(assembly.GetName().Name, _dllStr));
			return module;
		}

		private static ModuleBuilder GenerateModuleBuilder(AssemblyBuilder assembly)
		{
			if (_module == null)
			{
				lock (_lockAsmObject)
				{
					if (_module == null)
						_module = assembly.DefineDynamicModule(string.Concat(assembly.GetName().Name, _dllStr));
				}
			}
			return _module;
		}

		private static AssemblyBuilder GenerateAssemblyBuilder()
		{
			if (_assembly == null)
			{
				lock (_lockAsmObject)
				{
					if (_assembly == null)
					{
						AssemblyName assemName = new AssemblyName(JSON_GENERATED_ASSEMBLY_NAME) 
						{ 
							Version = new Version(1, 0, 0, 0)
							//VersionCompatibility = AssemblyVersionCompatibility.SameProcess
						};
#if !DEBUG && !NETSTANDARD
						//assemName.HashAlgorithm = AssemblyHashAlgorithm.SHA1;
						assemName.Flags = AssemblyNameFlags.PublicKey;
						assemName.SetPublicKey(Convert.FromBase64String(JSON_GENERATED_ASSEMBLY_PUBKEY));
						assemName.SetPublicKeyToken(Convert.FromBase64String(JSON_GENERATED_ASSEMBLY_PUBKEY_TOKEN));
						assemName.KeyPair = new StrongNameKeyPair(Convert.FromBase64String(JSON_GENERATED_ASSEMBLY_KEY));
#endif

#if NETSTANDARD || NETSTANDARD2
						_assembly = AssemblyBuilder.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.Run);
#else
						assemName.CultureInfo = new CultureInfo("en-US");
						_assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.RunAndSave);
#endif

						//[assembly: CompilationRelaxations(8)]
						_assembly.SetCustomAttribute(new CustomAttributeBuilder(
							typeof(CompilationRelaxationsAttribute).GetConstructor(new[] { _intType }), 
							new object[] { 8 }));

						//[assembly: RuntimeCompatibility(WrapNonExceptionThrows=true)]
						_assembly.SetCustomAttribute(new CustomAttributeBuilder(
							typeof(RuntimeCompatibilityAttribute).GetConstructor(Type.EmptyTypes),
							new object[] { },
							new[] { typeof(RuntimeCompatibilityAttribute).GetProperty("WrapNonExceptionThrows") },
							new object[] { true }));

#if !NETSTANDARD
						//[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification=true)]
						_assembly.SetCustomAttribute(new CustomAttributeBuilder(
							typeof(SecurityPermissionAttribute).GetConstructor(new[] { typeof(SecurityAction) }),
							new object[] { SecurityAction.RequestMinimum },
							new[] { typeof(SecurityPermissionAttribute).GetProperty("SkipVerification") },
							new object[] { true }));
#endif

#if !NETSTANDARD && !NET35
						//[assembly: SecurityRules(SecurityRuleSet.Level2,SkipVerificationInFullTrust=true)]
						_assembly.SetCustomAttribute(new CustomAttributeBuilder(typeof(SecurityRulesAttribute).GetConstructor(new[] { typeof(SecurityRuleSet) }),
							new object[] { SecurityRuleSet.Level2 },
							new[] { typeof(SecurityRulesAttribute).GetProperty("SkipVerificationInFullTrust") }, new object[] { true }));
#endif
					}
				}
			}
			return _assembly;
		}

		private static AssemblyBuilder GenerateAssemblyBuilderNoShare(string asmName)
		{
#if NETSTANDARD || NETSTANDARD2
            var assembly = AssemblyBuilder.DefineDynamicAssembly(
				new AssemblyName(asmName) { Version = new Version(1, 0, 0, 0) },
				AssemblyBuilderAccess.Run);
#else
			var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
				new AssemblyName(asmName) { Version = new Version(1, 0, 0, 0) }, 
				AssemblyBuilderAccess.RunAndSave);
#endif

			//[assembly: CompilationRelaxations(8)]
			assembly.SetCustomAttribute(new CustomAttributeBuilder(
				typeof(CompilationRelaxationsAttribute).GetConstructor(new[] { _intType }), 
				new object[] { 8 }));

			//[assembly: RuntimeCompatibility(WrapNonExceptionThrows=true)]
			assembly.SetCustomAttribute(new CustomAttributeBuilder(
				typeof(RuntimeCompatibilityAttribute).GetConstructor(Type.EmptyTypes),
				new object[] { },
				new[] { typeof(RuntimeCompatibilityAttribute).GetProperty("WrapNonExceptionThrows") },
				new object[] { true }));

#if !NETSTANDARD
			//[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification=true)]
			assembly.SetCustomAttribute(new CustomAttributeBuilder(
				typeof(SecurityPermissionAttribute).GetConstructor(new[] { typeof(SecurityAction) }),
				new object[] { SecurityAction.RequestMinimum },
				new[] { typeof(SecurityPermissionAttribute).GetProperty("SkipVerification") },
				new object[] { true }));
#endif

			return assembly;
		}

		private static MethodInfo GenerateExtractObject(TypeBuilder type)
		{
			MethodInfo method;
			var key = "ExtractObjectValue";
			if (_readMethodBuilders.TryGetValue(key, out method))
				return method;

			method = type.DefineMethodEx("ExtractObjectValue", StaticMethodAttribute, _objectType,
					new[] { _charPtrType, _intType.MakeByRefType(), _settingsType });

			_readMethodBuilders[key] = method;

			var il = method.GetILGenerator();

			var obj = il.DeclareLocal(_objectType);
			var returnLabel = il.DefineLabel();

			ILFixedWhile(il, 
				whileAction: (msil, current, ptr, startLoop, bLabel) => 
				{
					var valueLocal = il.DeclareLocal(_stringType);

					var tokenLabel = il.DefineLabel();
					var quoteLabel = il.DefineLabel();
					var bracketLabel = il.DefineLabel();
					var curlyLabel = il.DefineLabel();
					var dateLabel = il.DefineLabel();

					il.Emit(OpCodes.Ldc_I4, (int)' ');
					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Beq, tokenLabel);

					il.Emit(OpCodes.Ldc_I4, (int)':');
					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Beq, tokenLabel);

					il.Emit(OpCodes.Ldc_I4, (int)',');
					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Beq, tokenLabel);

					il.Emit(OpCodes.Ldc_I4, (int)'\n');
					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Beq, tokenLabel);

					il.Emit(OpCodes.Ldc_I4, (int)'\t');
					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Beq, tokenLabel);

					il.Emit(OpCodes.Ldc_I4, (int)'\r');
					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Beq, tokenLabel);

					//if(current == _ThreadQuoteChar) {
					//il.Emit(OpCodes.Ldc_I4, (int)_ThreadQuoteChar);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Ldfld, _settingQuoteChar);

					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Bne_Un, quoteLabel);

					//value = GetStringBasedValue(json, ref index)
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Call, _getStringBasedValue);
					il.Emit(OpCodes.Stloc, valueLocal);

					//if(IsDateValue(value)){
					il.Emit(OpCodes.Ldloc, valueLocal);
					il.Emit(OpCodes.Call, _isDateValue);
					il.Emit(OpCodes.Brfalse, dateLabel);

					il.Emit(OpCodes.Ldloc, valueLocal);
					il.Emit(OpCodes.Call, _toExpectedType);
					il.Emit(OpCodes.Stloc, obj);

					il.Emit(OpCodes.Leave, returnLabel);

					il.MarkLabel(dateLabel);
					//}

					il.Emit(OpCodes.Ldloc, valueLocal);
					il.Emit(OpCodes.Stloc, obj);

					il.Emit(OpCodes.Leave, returnLabel);

					il.MarkLabel(quoteLabel);
					//}

					//if(current == '[')
					il.Emit(OpCodes.Ldc_I4, (int)'[');
					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Bne_Un, bracketLabel);

					//CreateList(json, typeof(List<object>), ref index)
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Call, GenerateCreateListFor(type, typeof(List<object>)));

					il.Emit(OpCodes.Stloc, obj);

					il.Emit(OpCodes.Leave, returnLabel);

					il.MarkLabel(bracketLabel);
					//}

					//if(current == '{')
					il.Emit(OpCodes.Ldc_I4, (int)'{');
					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Bne_Un, curlyLabel);

					//GetClassOrDict(json, typeof(Dictionary<string, object>), ref index)
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Call, GenerateGetClassOrDictFor(type, typeof(Dictionary<string, object>)));

					il.Emit(OpCodes.Stloc, obj);

					il.Emit(OpCodes.Leave, returnLabel);

					il.MarkLabel(curlyLabel);
					//}

					//value = GetNonStringValue(json, ref index)
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Call, _getNonStringValue);
					il.Emit(OpCodes.Stloc, valueLocal);

					il.Emit(OpCodes.Ldloc, valueLocal);
					il.Emit(OpCodes.Call, _toExpectedType);

					il.Emit(OpCodes.Stloc, obj);

					il.Emit(OpCodes.Leave, returnLabel);

					il.MarkLabel(tokenLabel);
				},
				returnAction: msil => 
				{
					il.MarkLabel(returnLabel);
					il.Emit(OpCodes.Ldloc, obj);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Call, _toStrIfStr);
				});

			return method;
		}

		private static MethodInfo GenerateExtractValueFor(TypeBuilder typeBuilder, Type type)
		{
			MethodInfo method;
			var key = string.Concat(type.FullName, typeBuilder == null ? Dynamic : string.Empty);
			var typeName = type.GetName().Fix();
			if (_extractMethodBuilders.TryGetValue(key, out method))
				return method;
			var methodName = String.Concat(ExtractStr, typeName);
			var isObjectType = type == _objectType;
			method = typeBuilder.DefineMethodEx(methodName, StaticMethodAttribute,
				type, new[] { _charPtrType, _intType.MakeByRefType(), _settingsType });
			_extractMethodBuilders[key] = method;

			var il = method.GetILGenerator();
			var value = il.DeclareLocal(_stringType);

			var settings = il.DeclareLocal(_settingsType);
			var nullableType = type.GetNullableType() ?? type;

			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Stloc, settings);

			var isStringBasedLocal = il.DeclareLocal(_boolType);

			il.Emit(OpCodes.Ldc_I4, type.IsStringBasedType() ? 1 : 0);
			il.Emit(OpCodes.Stloc, isStringBasedLocal);

			if (type.GetTypeInfo().IsEnum)
			{
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Callvirt, _settingsUseEnumStringProp);
				il.Emit(OpCodes.Stloc, isStringBasedLocal);
			}

			if (nullableType == _dateTimeType || nullableType == _dateTimeOffsetType)
			{
				var dateCheckLabel = il.DefineLabel();
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Callvirt, _settingsDateFormat);
				il.Emit(OpCodes.Ldc_I4, (int)JsonDateTimeHandling.EpochTime);
				il.Emit(OpCodes.Ceq);
				il.Emit(OpCodes.Brtrue, dateCheckLabel);
				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Stloc, isStringBasedLocal);
				il.MarkLabel(dateCheckLabel);
			}

			if (type.IsPrimitiveType())
			{

				var isStringBasedLabel1 = il.DefineLabel();
				var isStringBasedLabel2 = il.DefineLabel();

				il.Emit(OpCodes.Ldloc, isStringBasedLocal);
				il.Emit(OpCodes.Brfalse, isStringBasedLabel1);

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldarg_2);
				if (type == _stringType)
				{
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Call, _decodeJsonString);
				}
				else
				{
					il.Emit(OpCodes.Call, _getStringBasedValue);
				}

				il.Emit(OpCodes.Stloc, value);

				il.MarkLabel(isStringBasedLabel1);


				il.Emit(OpCodes.Ldloc, isStringBasedLocal);
				il.Emit(OpCodes.Brtrue, isStringBasedLabel2);

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldarg_1);

				il.Emit(OpCodes.Call, _getNonStringValue);

				il.Emit(OpCodes.Stloc, value);

				il.MarkLabel(isStringBasedLabel2);

				GenerateChangeTypeFor(typeBuilder, type, il, value, settings);
				il.Emit(OpCodes.Ret);
			}
			else
			{
				if (isObjectType)
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Call, GenerateExtractObject(typeBuilder));
				}
				else if (!(type.IsListType() || type.IsArray))
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Call, GenerateGetClassOrDictFor(typeBuilder, type));
				}
				else
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Call, GenerateCreateListFor(typeBuilder, type));
				}
				il.Emit(OpCodes.Ret);
			}

			return method;
		}

		private static void GenerateChangeTypeFor(TypeBuilder typeBuilder, Type type, ILGenerator il, LocalBuilder value, LocalBuilder settings, Type originalType = null)
		{
			var nullableType = type;
			type = nullableType.GetNullableType();

			var isNullable = type != null;
			if (type == null)
				type = nullableType;

			var local = il.DeclareLocal(originalType ?? nullableType);

			var defaultLabel = default(Label);
			var nullLabelCheck = isNullable ? il.DefineLabel() : defaultLabel;
			var notNullLabel = isNullable ? il.DefineLabel() : defaultLabel;

			//Check for null
			if (isNullable)
			{
				il.Emit(OpCodes.Ldloc, value);
				il.Emit(OpCodes.Ldnull);
				il.Emit(OpCodes.Call, _stringOpEquality);
				il.Emit(OpCodes.Brfalse, nullLabelCheck);

				il.Emit(OpCodes.Ldloca, local);
				il.Emit(OpCodes.Initobj, _nullableType.MakeGenericType(type));

				il.Emit(OpCodes.Br, notNullLabel);

				il.MarkLabel(nullLabelCheck);
			}


			il.Emit(OpCodes.Ldloc, value);

			if (type == _intType)
			{
				il.Emit(OpCodes.Call, _strToInt32);
			}
			else if (type == typeof(short))
			{
				il.Emit(OpCodes.Call, _strToInt16);
			}
			else if (type == typeof(ushort))
			{
				il.Emit(OpCodes.Call, _strToUInt16);
			}
			else if (type == typeof(byte))
			{
				il.Emit(OpCodes.Call, _strToByte);
			}
			else if (type == typeof(sbyte))
			{
				il.Emit(OpCodes.Call, _strToInt16);
				il.Emit(OpCodes.Conv_I1);
			}
			else if (type == typeof(uint))
			{
				il.Emit(OpCodes.Call, _strToUInt32);
			}
			else if (type == _decimalType)
			{
				il.Emit(OpCodes.Call, _strToDecimal);
			}
			else if (type == typeof(long))
			{
				il.Emit(OpCodes.Call, _strToInt64);
			}
			else if (type == typeof(ulong))
			{
				il.Emit(OpCodes.Call, _strToUInt64);
			}
			else if (type == typeof(double))
			{
				il.Emit(OpCodes.Call, _strToDouble);
			}
			else if (type == typeof(float))
			{
				il.Emit(OpCodes.Call, _strToSingle);
			}
			else if (type == _dateTimeType)
			{
				il.Emit(OpCodes.Ldloc, settings);
				il.Emit(OpCodes.Call, _strToDate);
			}
			else if (type == _dateTimeOffsetType)
			{
				il.Emit(OpCodes.Ldloc, settings);
				il.Emit(OpCodes.Call, _strToDateTimeoffset);
			}
			else if (type == _charType)
			{
				il.Emit(OpCodes.Call, _strToChar);
			}
			else if (type == _timeSpanType)
			{
				il.Emit(OpCodes.Call, _timeSpanParse);
			}
			else if (type == _byteArrayType)
			{
				il.Emit(OpCodes.Call, _strToByteArray);
			}
			else if (type == _boolType)
			{
				il.Emit(OpCodes.Call, _strToBoolean);
			}
			else if (type == _guidType)
			{
				il.Emit(OpCodes.Call, _strToGuid);
			}
			else if (type.GetTypeInfo().IsEnum)
			{
				il.Emit(OpCodes.Call, ReadStringToEnumFor(typeBuilder, type));
			}
			else if (type == _typeType)
			{
				il.Emit(OpCodes.Call, _strToType);
			}

			if (isNullable)
			{
				il.Emit(OpCodes.Newobj, _nullableType.MakeGenericType(type).GetConstructor(new[] { type }));
				il.Emit(OpCodes.Stloc, local);

				il.MarkLabel(notNullLabel);

				il.Emit(OpCodes.Ldloc, local);
			}
		}

		private static MethodInfo GenerateSetValueFor(TypeBuilder typeBuilder, Type type)
		{
			MethodInfo method;
			var key = string.Concat(type.FullName, typeBuilder == null ? Dynamic : string.Empty);
			var typeName = type.GetName().Fix();
			if (_setValueMethodBuilders.TryGetValue(key, out method))
				return method;

			var isTypeValueType = type.GetTypeInfo().IsValueType;
			var methodName = String.Concat(SetStr, typeName);
			var isObjectType = type == _objectType;
			method = typeBuilder.DefineMethodEx(methodName, StaticMethodAttribute, _voidType, 
				new[] 
				{
					_charPtrType,
					_intType.MakeByRefType(),
					isTypeValueType ? type.MakeByRefType() : type,
					_stringType,
					_settingsType
				});

			_setValueMethodBuilders[key] = method;

			const bool Optimized = true;

			var il = method.GetILGenerator();

			//if (!_includeTypeInformation)
			if (!IncludeTypeInfo)
			{
				GenerateTypeSetValueFor(typeBuilder, type, isTypeValueType, Optimized, il);
			}
			else
			{
				var pTypes = GetIncludedTypeTypes(type);

				if (pTypes.Count == 1)
				{
					GenerateTypeSetValueFor(typeBuilder, type, isTypeValueType, Optimized, il);
				}
				else
				{
					var typeLocal = il.DeclareLocal(typeof(Type));

					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Callvirt, _objectGetType);
					il.Emit(OpCodes.Stloc, typeLocal);

					foreach (var pType in pTypes)
					{
						var compareLabel = il.DefineLabel();

						il.Emit(OpCodes.Ldloc, typeLocal);

						il.Emit(OpCodes.Ldtoken, pType);
						il.Emit(OpCodes.Call, _typeGetTypeFromHandle);

						il.Emit(OpCodes.Call, _cTypeOpEquality);

						il.Emit(OpCodes.Brfalse, compareLabel);

						GenerateTypeSetValueFor(typeBuilder, pType, pType.GetTypeInfo().IsValueType, Optimized, il);

						il.MarkLabel(compareLabel);
					}
				}
			}

			il.Emit(OpCodes.Ret);

			return method;
		}

		private static MethodInfo GenerateFastObjectToString(TypeBuilder type)
		{
			return _readMethodBuilders.GetOrAdd("FastObjectToString", _ => 
			{
				lock (GetDictLockObject("GenerateFastObjectToString"))
				{
					var method = type.DefineMethodEx("FastObjectToString", StaticMethodAttribute, _voidType,
						new[] { _objectType, _stringBuilderType, _settingsType });

					var il = method.GetILGenerator();

					var typeLocal = il.DeclareLocal(_typeType);
					var needQuoteLocal = il.DeclareLocal(_boolType);
					var needQuoteStartLabel = il.DefineLabel();
					var needQuoteEndLabel = il.DefineLabel();

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Callvirt, _objectGetType);
					il.Emit(OpCodes.Stloc, typeLocal);

					var isListTypeLabel = il.DefineLabel();

					il.Emit(OpCodes.Ldloc, typeLocal);
					il.Emit(OpCodes.Call, _isListType);
					il.Emit(OpCodes.Brfalse, isListTypeLabel);

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Castclass, _listType);
					il.Emit(OpCodes.Call, _listToListObject);
					il.Emit(OpCodes.Starg, 0);

					WriteCollection(type, typeof(List<object>), il);
					il.Emit(OpCodes.Ret);

					il.MarkLabel(isListTypeLabel);

					var isDictTypeLabel = il.DefineLabel();

					il.Emit(OpCodes.Ldloc, typeLocal);
					il.Emit(OpCodes.Call, _isDictType);
					il.Emit(OpCodes.Brfalse, isDictTypeLabel);

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Castclass, _dictStringObject);
					il.Emit(OpCodes.Starg, 0);

					WriteCollection(type, _dictStringObject, il);
					il.Emit(OpCodes.Ret);

					il.MarkLabel(isDictTypeLabel);


					il.Emit(OpCodes.Ldloc, typeLocal);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Call, _needQuote);
					il.Emit(OpCodes.Stloc, needQuoteLocal);


					il.Emit(OpCodes.Ldloc, needQuoteLocal);
					il.Emit(OpCodes.Brfalse, needQuoteStartLabel);

					il.Emit(OpCodes.Ldarg_1);
					LoadQuotChar(il);
					il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
					il.Emit(OpCodes.Pop);

					il.MarkLabel(needQuoteStartLabel);

					_defaultSerializerTypes[_dateTimeType] = _generatorDateToStr;
					_defaultSerializerTypes[_dateTimeOffsetType] = _generatorDateOffsetToStr;

					var serializerTypeMethods = new Dictionary<Type, MethodInfo>();

					foreach (var kv in _defaultSerializerTypes)
					{
						serializerTypeMethods[kv.Key] = kv.Value;
					}

					foreach (var kv in _registeredSerializerMethods)
					{
						serializerTypeMethods[kv.Key] = kv.Value;
					}

					foreach (var kv in serializerTypeMethods)
					{
						var objType = kv.Key;
						var compareLabel = il.DefineLabel();

						il.Emit(OpCodes.Ldloc, typeLocal);

						il.Emit(OpCodes.Ldtoken, objType);
						il.Emit(OpCodes.Call, _typeGetTypeFromHandle);

						il.Emit(OpCodes.Call, _cTypeOpEquality);

						il.Emit(OpCodes.Brfalse, compareLabel);

						if (objType == _stringType)
						{
							il.Emit(OpCodes.Ldarg_1);
							il.Emit(OpCodes.Ldarg_0);
							il.Emit(OpCodes.Castclass, _stringType);
							il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
							il.Emit(OpCodes.Pop);
						}
						else if (objType == _enumType)
						{
							il.Emit(OpCodes.Ldarg_1);
							il.Emit(OpCodes.Ldarg_0);
							il.Emit(OpCodes.Unbox_Any, objType);
							il.Emit(OpCodes.Ldarg_2);
							il.Emit(OpCodes.Call, _generatorEnumToStr);
							il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
							il.Emit(OpCodes.Pop);
						}
						else if (objType == _boolType)
						{
							var boolLocal = il.DeclareLocal(_stringType);
							var boolLabel = il.DefineLabel();
							il.Emit(OpCodes.Ldstr, "true");
							il.Emit(OpCodes.Stloc, boolLocal);

							il.Emit(OpCodes.Ldarg_0);
							il.Emit(OpCodes.Unbox_Any, _boolType);
							il.Emit(OpCodes.Brtrue, boolLabel);
							il.Emit(OpCodes.Ldstr, "false");
							il.Emit(OpCodes.Stloc, boolLocal);
							il.MarkLabel(boolLabel);

							il.Emit(OpCodes.Ldarg_1);
							il.Emit(OpCodes.Ldloc, boolLocal);
							il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
							il.Emit(OpCodes.Pop);
						}
						else if (objType == _objectType)
						{
							il.Emit(OpCodes.Ldarg_1);
							il.Emit(OpCodes.Ldarg_0);
							il.Emit(OpCodes.Callvirt, _stringBuilderAppendObject);
							il.Emit(OpCodes.Pop);
						}
						else
						{
							il.Emit(OpCodes.Ldarg_1);
							il.Emit(OpCodes.Ldarg_0);
							if (objType.GetTypeInfo().IsValueType)
								il.Emit(OpCodes.Unbox_Any, objType);
							else il.Emit(OpCodes.Castclass, objType);
							if (objType == _dateTimeType || objType == _dateTimeOffsetType)
								il.Emit(OpCodes.Ldarg_2);
							il.Emit(OpCodes.Call, kv.Value);
							il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
							il.Emit(OpCodes.Pop);
						}

						il.MarkLabel(compareLabel);
					}

					il.Emit(OpCodes.Ldloc, needQuoteLocal);
					il.Emit(OpCodes.Brfalse, needQuoteEndLabel);

					il.Emit(OpCodes.Ldarg_1);
					LoadQuotChar(il);
					il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
					il.Emit(OpCodes.Pop);

					il.MarkLabel(needQuoteEndLabel);

					il.Emit(OpCodes.Ret);

					return method;
				}
			});
		}

		internal static Type Generate(Type objType)
		{
			var returnType = default(Type);
			if (_types.TryGetValue(objType, out returnType))
				return returnType;

			var asmName = string.Concat(objType.GetName(), ClassStr);

			var assembly = _useSharedAssembly ? GenerateAssemblyBuilder() : GenerateAssemblyBuilderNoShare(asmName);

			var module = _useSharedAssembly ? GenerateModuleBuilder(assembly) : GenerateModuleBuilderNoShare(assembly);

			var type = GenerateTypeBuilder(objType, module);

			returnType = type.CreateType();

			_types[objType] = returnType;

#if !NETSTANDARD && !NETSTANDARD2
			if (_generateAssembly)
				assembly.Save(string.Concat(assembly.GetName().Name, _dllStr));
#endif
			return returnType;
		}

		private static TypeBuilder GenerateTypeBuilder(Type objType, ModuleBuilder module)
		{
			var genericType = _serializerType.MakeGenericType(objType);

			var type = module.DefineType(String.Concat(objType.GetName(), ClassStr), TypeAttribute, genericType);

			var isPrimitive = objType.IsPrimitiveType();

			var writeMethod = WriteSerializeMethodFor(type, objType, needQuote: !isPrimitive || objType == _stringType);

			var readMethod = WriteDeserializeMethodFor(type, objType);

			var serializeMethod = type.DefineMethod(SerializeStr, MethodAttribute,
				_stringType, new[] { objType });

			var serializeWithTextWriterMethod = type.DefineMethod(SerializeStr, MethodAttribute,
				_voidType, new[] { objType, _textWriterType });

			var deserializeMethod = type.DefineMethod(DeserializeStr, MethodAttribute,
				objType, new[] { _stringType });

			var deserializeWithReaderMethod = type.DefineMethod(DeserializeStr, MethodAttribute,
				objType, new[] { _textReaderType });

			var serializeMethodWithSettings = type.DefineMethod(SerializeStr, MethodAttribute,
			   _stringType, new[] { objType, _settingsType });

			var serializeWithTextWriterMethodWithSettings = type.DefineMethod(SerializeStr, MethodAttribute,
				_voidType, new[] { objType, _textWriterType, _settingsType });

			var deserializeMethodWithSettings = type.DefineMethod(DeserializeStr, MethodAttribute,
				objType, new[] { _stringType, _settingsType });

			var deserializeWithReaderMethodWithSettings = type.DefineMethod(DeserializeStr, MethodAttribute,
				objType, new[] { _textReaderType, _settingsType });

			var il = serializeMethod.GetILGenerator();

			var sbLocal = il.DeclareLocal(_stringBuilderType);
			il.Emit(OpCodes.Call, _generatorGetStringBuilder);

			il.EmitClearStringBuilder();

			il.Emit(OpCodes.Stloc, sbLocal);

			//il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldloc, sbLocal);
			il.Emit(OpCodes.Call, _settingsCurrentSettings);
			il.Emit(OpCodes.Call, writeMethod);

			il.Emit(OpCodes.Ldloc, sbLocal);
			il.Emit(OpCodes.Callvirt, _stringBuilderToString);
			il.Emit(OpCodes.Ret);

			var wil = serializeWithTextWriterMethod.GetILGenerator();

			var wsbLocal = wil.DeclareLocal(_stringBuilderType);
			wil.Emit(OpCodes.Call, _generatorGetStringBuilder);
			wil.EmitClearStringBuilder();
			wil.Emit(OpCodes.Stloc, wsbLocal);

			//il.Emit(OpCodes.Ldarg_0);
			wil.Emit(OpCodes.Ldarg_1);
			wil.Emit(OpCodes.Ldloc, wsbLocal);
			wil.Emit(OpCodes.Call, _settingsCurrentSettings);
			wil.Emit(OpCodes.Call, writeMethod);

			wil.Emit(OpCodes.Ldarg_2);
			wil.Emit(OpCodes.Ldloc, wsbLocal);
			wil.Emit(OpCodes.Callvirt, _stringBuilderToString);
			wil.Emit(OpCodes.Callvirt, _textWriterWrite);
			wil.Emit(OpCodes.Ret);

			var dil = deserializeMethod.GetILGenerator();

			dil.Emit(OpCodes.Ldarg_1);
			dil.Emit(OpCodes.Call, _settingsCurrentSettings);
			dil.Emit(OpCodes.Call, readMethod);
			dil.Emit(OpCodes.Ret);

			var rdil = deserializeWithReaderMethod.GetILGenerator();

			rdil.Emit(OpCodes.Ldarg_1);
			rdil.Emit(OpCodes.Callvirt, _textReaderReadToEnd);
			rdil.Emit(OpCodes.Call, _settingsCurrentSettings);
			rdil.Emit(OpCodes.Call, readMethod);
			rdil.Emit(OpCodes.Ret);

			//With Settings
			var isValueType = objType.GetTypeInfo().IsValueType;
			var ilWithSettings = serializeMethodWithSettings.GetILGenerator();

			var sbLocalWithSettings = ilWithSettings.DeclareLocal(_stringBuilderType);
			ilWithSettings.Emit(OpCodes.Call, _generatorGetStringBuilder);

			ilWithSettings.EmitClearStringBuilder();

			ilWithSettings.Emit(OpCodes.Stloc, sbLocalWithSettings);

			//il.Emit(OpCodes.Ldarg_0);
			ilWithSettings.Emit(OpCodes.Ldarg_1);
			ilWithSettings.Emit(OpCodes.Ldloc, sbLocalWithSettings);
			ilWithSettings.Emit(OpCodes.Ldarg_2);
			ilWithSettings.Emit(OpCodes.Call, writeMethod);

			ilWithSettings.Emit(OpCodes.Ldloc, sbLocalWithSettings);
			ilWithSettings.Emit(OpCodes.Callvirt, _stringBuilderToString);

			ilWithSettings.Emit(OpCodes.Ldarg_2);

			ilWithSettings.Emit(OpCodes.Call, _prettifyJsonIfNeeded);

			ilWithSettings.Emit(OpCodes.Ret);

			var wilWithSettings = serializeWithTextWriterMethodWithSettings.GetILGenerator();

			var wsbLocalWithSettings = wilWithSettings.DeclareLocal(_stringBuilderType);
			wilWithSettings.Emit(OpCodes.Call, _generatorGetStringBuilder);
			wilWithSettings.EmitClearStringBuilder();
			wilWithSettings.Emit(OpCodes.Stloc, wsbLocalWithSettings);

			//il.Emit(OpCodes.Ldarg_0);
			wilWithSettings.Emit(OpCodes.Ldarg_1);
			wilWithSettings.Emit(OpCodes.Ldloc, wsbLocalWithSettings);
			wilWithSettings.Emit(OpCodes.Ldarg_3);
			wilWithSettings.Emit(OpCodes.Call, writeMethod);

			wilWithSettings.Emit(OpCodes.Ldarg_2);
			wilWithSettings.Emit(OpCodes.Ldloc, wsbLocalWithSettings);
			wilWithSettings.Emit(OpCodes.Callvirt, _stringBuilderToString);

			wilWithSettings.Emit(OpCodes.Ldarg_3);

			wilWithSettings.Emit(OpCodes.Call, _prettifyJsonIfNeeded);

			wilWithSettings.Emit(OpCodes.Callvirt, _textWriterWrite);
			wilWithSettings.Emit(OpCodes.Ret);

			var dilWithSettings = deserializeMethodWithSettings.GetILGenerator();

			dilWithSettings.Emit(OpCodes.Ldarg_1);
			dilWithSettings.Emit(OpCodes.Ldarg_2);
			dilWithSettings.Emit(OpCodes.Call, readMethod);
			dilWithSettings.Emit(OpCodes.Ret);

			var rdilWithSettings = deserializeWithReaderMethodWithSettings.GetILGenerator();

			rdilWithSettings.Emit(OpCodes.Ldarg_1);
			rdilWithSettings.Emit(OpCodes.Callvirt, _textReaderReadToEnd);
			rdilWithSettings.Emit(OpCodes.Ldarg_2);
			rdilWithSettings.Emit(OpCodes.Call, readMethod);
			rdilWithSettings.Emit(OpCodes.Ret);

			//With Settings End

			type.DefineMethodOverride(serializeMethod,
				genericType.GetMethod(SerializeStr, new[] { objType }));

			type.DefineMethodOverride(serializeWithTextWriterMethod,
				genericType.GetMethod(SerializeStr, new[] { objType, _textWriterType }));

			type.DefineMethodOverride(deserializeMethod,
				genericType.GetMethod(DeserializeStr, new[] { _stringType }));

			type.DefineMethodOverride(deserializeWithReaderMethod,
				genericType.GetMethod(DeserializeStr, new[] { _textReaderType }));

			//With Settings
			type.DefineMethodOverride(serializeMethodWithSettings,
			   genericType.GetMethod(SerializeStr, new Type[] { objType, _settingsType }));

			type.DefineMethodOverride(serializeWithTextWriterMethodWithSettings,
				genericType.GetMethod(SerializeStr, new Type[] { objType, _textWriterType, _settingsType }));

			type.DefineMethodOverride(deserializeMethodWithSettings,
				genericType.GetMethod(DeserializeStr, new Type[] { _stringType, _settingsType }));

			type.DefineMethodOverride(deserializeWithReaderMethodWithSettings,
				genericType.GetMethod(DeserializeStr, new[] { _textReaderType, _settingsType }));

			//With Settings End
			return type;
		}

		private static void GenerateTypeSetValueFor(TypeBuilder typeBuilder, Type type, bool isTypeValueType, bool Optimized, ILGenerator il)
		{
			var props = type.GetTypeProperties();
			var caseLocal = il.DeclareLocal(_stringComparison);

			il.Emit(OpCodes.Ldarg, 4);
			il.Emit(OpCodes.Ldfld, _settingsCaseComparison);
			il.Emit(OpCodes.Stloc, caseLocal);

			for (var i = 0; i < props.Length; i++)
			{
				var mem = props[i];
				var member = mem.Member;
				var prop = member.GetMemberType() == MemberTypes.Property ? member as PropertyInfo : null;
				var field = member.GetMemberType() == MemberTypes.Field ? member as FieldInfo : null;
				var attr = mem.Attribute;
				MethodInfo setter = null;
				var isProp = prop != null;

				var canWrite = isProp ? prop.CanWrite : false;
				var propName = member.Name;
				var conditionLabel = il.DefineLabel();
				var propType = isProp ? prop.PropertyType : field.FieldType;
				var originPropType = propType;
				var nullableType = propType.GetNullableType();
				var isNullable = nullableType != null;
				propType = isNullable ? nullableType : propType;

				if (canWrite)
				{
					setter = prop.GetSetMethod();
					if (setter == null)
						setter = type.GetMethod(string.Concat("set_", propName), MethodBinding);
				}

				var isPublicSetter = canWrite && setter.IsPublic;

				il.Emit(OpCodes.Ldarg_3);
				il.Emit(OpCodes.Ldstr, attr != null ? (attr.Name ?? propName) : propName);

				il.Emit(OpCodes.Ldloc, caseLocal);
				il.Emit(OpCodes.Call, _stringEqualCompare);

				il.Emit(OpCodes.Brfalse, conditionLabel);

				if (!Optimized)
				{
					//il.Emit(OpCodes.Ldarg_0);
					//il.Emit(OpCodes.Ldarg_1);
					//il.Emit(OpCodes.Ldarg, 4);
					//il.Emit(OpCodes.Call, GenerateExtractValueFor(typeBuilder, propType));
					//if (isProp) 
					//{
					//    if (setter != null) 
					//    {
					//        if (!isPublicSetter) 
					//        {
					//            if (propType.IsValueType)
					//                il.Emit(OpCodes.Box, propType);
					//            il.Emit(OpCodes.Ldtoken, setter);
					//            il.Emit(OpCodes.Call, _methodGetMethodFromHandle);
					//            il.Emit(OpCodes.Call, _setterPropertyValueMethod.MakeGenericMethod(type));
					//        } 
					//        else
					//        {
					//            il.Emit(isTypeValueType ? OpCodes.Call : OpCodes.Callvirt, setter);
					//        }
					//    } 
					//    else 
					//    {
					//        il.Emit(OpCodes.Pop);
					//        il.Emit(OpCodes.Pop);
					//    }
					//} else il.Emit(OpCodes.Stfld, field);
				}
				else
				{
					var propValue = il.DeclareLocal(originPropType);
					var isValueType = propType.GetTypeInfo().IsValueType;
					var isPrimitiveType = propType.IsPrimitiveType();
					var isStruct = isValueType && !isPrimitiveType;
					var propNullLabel = !isNullable ? il.DefineLabel() : default(Label);
					var nullablePropValue = isNullable ? il.DeclareLocal(originPropType) : null;
					var equalityMethod = propType.GetMethod("op_Equality");


					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg, 4);
					il.Emit(OpCodes.Call, GenerateExtractValueFor(typeBuilder, originPropType));

					il.Emit(OpCodes.Stloc, propValue);

					if (!isNullable)
					{
						if (isStruct)
							il.Emit(OpCodes.Ldloca, propValue);
						else
							il.Emit(OpCodes.Ldloc, propValue);

						if (isValueType && isPrimitiveType)
						{
							LoadDefaultValueByType(il, propType);
						}
						else
						{
							if (!isValueType)
								il.Emit(OpCodes.Ldnull);
						}

						if (equalityMethod != null)
						{
							il.Emit(OpCodes.Call, equalityMethod);
							il.Emit(OpCodes.Brtrue, propNullLabel);
						}
						else
						{
							if (isStruct)
							{

								var tempValue = il.DeclareLocal(propType);

								il.Emit(OpCodes.Ldloca, tempValue);
								il.Emit(OpCodes.Initobj, propType);
								il.Emit(OpCodes.Ldloc, tempValue);
								il.Emit(OpCodes.Box, propType);
								il.Emit(OpCodes.Constrained, propType);

								il.Emit(OpCodes.Callvirt, _objectEquals);

								il.Emit(OpCodes.Brtrue, propNullLabel);

							}
							else
							{
								il.Emit(OpCodes.Beq, propNullLabel);
							}
						}
					}

					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Ldloc, propValue);
					//if (isNullable) 
					//{
					//    il.Emit(OpCodes.Newobj, _nullableType.MakeGenericType(propType).GetConstructor(new[] { propType }));
					//}

					if (isProp)
					{
						if (setter != null)
						{
							if (!setter.IsPublic)
							{
								if (propType.GetTypeInfo().IsValueType)
									il.Emit(OpCodes.Box, isNullable ? prop.PropertyType : propType);
								il.Emit(OpCodes.Ldtoken, setter);
								il.Emit(OpCodes.Call, _methodGetMethodFromHandle);
								il.Emit(OpCodes.Call, _setterPropertyValueMethod.MakeGenericMethod(type));
							}
							else
							{
								il.Emit(isTypeValueType ? OpCodes.Call : OpCodes.Callvirt, setter);
							}
						}
						else
						{
							il.Emit(OpCodes.Pop);
							il.Emit(OpCodes.Pop);
						}

					}
					else
					{
						il.Emit(OpCodes.Stfld, field);
					}

					il.Emit(OpCodes.Ret);

					if (!isNullable)
						il.MarkLabel(propNullLabel);
				}

				il.Emit(OpCodes.Ret);

				il.MarkLabel(conditionLabel);
			}

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg, 4);
			il.Emit(OpCodes.Call, _skipProperty);
		}

		private static MethodInfo GenerateCreateListFor(TypeBuilder typeBuilder, Type type)
		{
			MethodInfo method;
			var key = string.Concat(type.FullName, typeBuilder == null ? Dynamic : string.Empty);
			var typeName = type.GetName().Fix();
			if (_createListMethodBuilders.TryGetValue(key, out method))
				return method;
			var methodName = String.Concat(CreateListStr, typeName);
			var isObjectType = type == _objectType;
			method = typeBuilder.DefineMethodEx(methodName, StaticMethodAttribute, type, 
				new[] { _charPtrType, _intType.MakeByRefType(), _settingsType });
			_createListMethodBuilders[key] = method;

			var il = method.GetILGenerator();

			var isArray = type.IsArray;
			var elementType = isArray ? type.GetElementType() : type.GetGenericArguments()[0];
			var nullableType = elementType.GetNullableType();
			nullableType = nullableType != null ? nullableType : elementType;

			var isPrimitive = elementType.IsPrimitiveType();
			var isStringType = elementType == _stringType;
			var isByteArray = elementType == _byteArrayType;
			var isStringBased = isStringType || nullableType == _timeSpanType || isByteArray;
			var isCollectionType = !isArray && 
				!_listType.IsAssignableFrom(type) && 
				!(type.Name == IEnumerableStr) && 
				!(type.Name == IListStr) && 
				!(type.Name == ICollectionStr);

			var isStringBasedLocal = il.DeclareLocal(_boolType);

			var settings = il.DeclareLocal(_settingsType);
			var obj = isCollectionType ? il.DeclareLocal(type) : il.DeclareLocal(typeof(List<>).MakeGenericType(elementType));
			var objArray = isArray ? il.DeclareLocal(elementType.MakeArrayType()) : null;
			var count = il.DeclareLocal(_intType);
			var startIndex = il.DeclareLocal(_intType);
			var endIndex = il.DeclareLocal(_intType);
			var prev = il.DeclareLocal(_charType);
			var addMethod = _genericCollectionType.MakeGenericType(elementType).GetMethod("Add");

			var prevLabel = il.DefineLabel();

			il.Emit(OpCodes.Ldc_I4, isStringBased ? 1 : 0);
			il.Emit(OpCodes.Stloc, isStringBasedLocal);

			if (nullableType.GetTypeInfo().IsEnum)
			{
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Callvirt, _settingsUseEnumStringProp);
				il.Emit(OpCodes.Stloc, isStringBasedLocal);
			}

			if (nullableType == _dateTimeType || nullableType == _dateTimeOffsetType)
			{
				var dateCheckLabel = il.DefineLabel();
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Callvirt, _settingsDateFormat);
				il.Emit(OpCodes.Ldc_I4, (int)JsonDateTimeHandling.EpochTime);
				il.Emit(OpCodes.Ceq);
				il.Emit(OpCodes.Brtrue, dateCheckLabel);
				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Stloc, isStringBasedLocal);
				il.MarkLabel(dateCheckLabel);
			}

			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Stloc, settings);

			il.Emit(OpCodes.Newobj, obj.LocalType.GetConstructor(Type.EmptyTypes));
			il.Emit(OpCodes.Stloc, obj);

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, count);

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, startIndex);

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, endIndex);


			il.Emit(OpCodes.Ldc_I4, (int)'\0');
			il.Emit(OpCodes.Stloc, prev);


			ILFixedWhile(il, whileAction: (msil, current, ptr, startLoop, bLabel) => 
			{
				//if prev == ']'
				il.Emit(OpCodes.Ldloc, prev);
				il.Emit(OpCodes.Ldc_I4, (int)']');
				il.Emit(OpCodes.Bne_Un, prevLabel);

				//break
				il.Emit(OpCodes.Br, bLabel);

				il.MarkLabel(prevLabel);

				if (isPrimitive)
				{
					var isStringBasedLabel1 = il.DefineLabel();
					var isStringBasedLabel2 = il.DefineLabel();

					il.Emit(OpCodes.Ldloc, isStringBasedLocal);
					il.Emit(OpCodes.Brfalse, isStringBasedLabel1);
					GenerateCreateListForStringBased(typeBuilder, il, elementType, isStringType, settings, obj, addMethod, current, ptr, bLabel);
					il.MarkLabel(isStringBasedLabel1);

					il.Emit(OpCodes.Ldloc, isStringBasedLocal);
					il.Emit(OpCodes.Brtrue, isStringBasedLabel2);
					GenerateCreateListForNonStringBased(typeBuilder, il, elementType, settings, obj, addMethod, current);
					il.MarkLabel(isStringBasedLabel2);
				}
				else
				{
					var currentBlank = il.DefineLabel();
					var currentBlockEnd = il.DefineLabel();

					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Ldc_I4, (int)' ');
					il.Emit(OpCodes.Beq, currentBlank);

					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Ldc_I4, (int)'\n');
					il.Emit(OpCodes.Beq, currentBlank);

					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Ldc_I4, (int)'\r');
					il.Emit(OpCodes.Beq, currentBlank);

					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Ldc_I4, (int)'\t');
					il.Emit(OpCodes.Beq, currentBlank);

					il.Emit(OpCodes.Ldloc, current);
					il.Emit(OpCodes.Ldc_I4, (int)']');
					il.Emit(OpCodes.Bne_Un, currentBlockEnd);

					IncrementIndexRef(il);
					il.Emit(OpCodes.Br, bLabel);

					il.MarkLabel(currentBlockEnd);

					il.Emit(OpCodes.Ldloc, obj);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Call, GenerateExtractValueFor(typeBuilder, elementType));
					il.Emit(OpCodes.Callvirt, addMethod);

					GenerateUpdateCurrent(il, current, ptr);

					il.MarkLabel(currentBlank);
				}

				il.Emit(OpCodes.Ldloc, current);
				il.Emit(OpCodes.Stloc, prev);
			}, 
			beforeAction: (msil, ptr) => 
			{
				var isNullArrayLabel = il.DefineLabel();

				il.Emit(OpCodes.Ldloc, ptr);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Call, _moveToArrayBlock);
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Bne_Un, isNullArrayLabel);

				il.Emit(OpCodes.Ldnull);
				il.Emit(OpCodes.Ret);

				il.MarkLabel(isNullArrayLabel);
			},
			needBreak: true,
			returnAction: msil => 
			{
				if (isArray)
				{
					il.Emit(OpCodes.Ldloc, obj);
					il.Emit(OpCodes.Callvirt, obj.LocalType.GetMethod("get_Count"));
					il.Emit(OpCodes.Newarr, elementType);
					il.Emit(OpCodes.Stloc, objArray);

					il.Emit(OpCodes.Ldloc, obj);
					il.Emit(OpCodes.Ldloc, objArray);
					il.Emit(OpCodes.Ldc_I4_0);
					il.Emit(OpCodes.Callvirt, obj.LocalType.GetMethod("CopyTo", new[] { objArray.LocalType, _intType }));

					il.Emit(OpCodes.Ldloc, objArray);
				}
				else
				{
					il.Emit(OpCodes.Ldloc, obj);
				}
			});

			return method;
		}

		private static void GenerateCreateListForNonStringBased(TypeBuilder typeBuilder, ILGenerator il, Type elementType, LocalBuilder settings, LocalBuilder obj, MethodInfo addMethod, LocalBuilder current)
		{
			var text = il.DeclareLocal(_stringType);

			var blankNewLineLabel = il.DefineLabel();

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)' ');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)',');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)']');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)'\n');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)'\r');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)'\t');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Call, _getNonStringValue);

			il.Emit(OpCodes.Stloc, text);

			il.Emit(OpCodes.Ldloc, obj);
			GenerateChangeTypeFor(typeBuilder, elementType, il, text, settings);
			il.Emit(OpCodes.Callvirt, addMethod);

			il.MarkLabel(blankNewLineLabel);
		}

		private static void GenerateCreateListForStringBased(TypeBuilder typeBuilder, ILGenerator il, Type elementType, bool isStringType, LocalBuilder settings, LocalBuilder obj, MethodInfo addMethod, LocalBuilder current, LocalBuilder ptr, Label bLabel)
		{
			var text = il.DeclareLocal(_stringType);

			var blankNewLineLabel = il.DefineLabel();

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)' ');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)',');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)']');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)'\n');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)'\r');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)'\t');
			il.Emit(OpCodes.Beq, blankNewLineLabel);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg_2);
			if (isStringType)
			{
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Call, _decodeJsonString);
			}
			else
			{
				il.Emit(OpCodes.Call, _getStringBasedValue);
			}

			il.Emit(OpCodes.Stloc, text);

			il.Emit(OpCodes.Ldloc, obj);

			if (!isStringType)
				GenerateChangeTypeFor(typeBuilder, elementType, il, text, settings);
			else
				il.Emit(OpCodes.Ldloc, text);

			il.Emit(OpCodes.Callvirt, addMethod);

			GenerateUpdateCurrent(il, current, ptr);

			var currentLabel = il.DefineLabel();
			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)']');
			il.Emit(OpCodes.Bne_Un, currentLabel);
			//break
			il.Emit(OpCodes.Br, bLabel);
			il.MarkLabel(currentLabel);

			il.MarkLabel(blankNewLineLabel);
		}

		private static MethodInfo GenerateGetClassOrDictFor(TypeBuilder typeBuilder, Type type)
		{
			MethodInfo method;
			var key = string.Concat(type.FullName, typeBuilder == null ? Dynamic : string.Empty);
			var typeName = type.GetName().Fix();
			if (_readMethodBuilders.TryGetValue(key, out method))
				return method;

			var methodName = string.Concat(CreateClassOrDictStr, typeName);
			var isObjectType = type == _objectType;
			method = typeBuilder.DefineMethodEx(methodName, StaticMethodAttribute,
				type, new[] { _charPtrType, _intType.MakeByRefType(), _settingsType });
			_readMethodBuilders[key] = method;


			var il = method.GetILGenerator();

			var settings = il.DeclareLocal(_settingsType);
			var foundQuote = il.DeclareLocal(_boolType);
			var dict = il.DeclareLocal(_dictType);
			var prev = il.DeclareLocal(_charType);
			var count = il.DeclareLocal(_intType);
			var startIndex = il.DeclareLocal(_intType);
			var quotes = il.DeclareLocal(_intType);
			var isTag = il.DeclareLocal(_boolType);

			var incLabel = il.DefineLabel();
			var openCloseBraceLabel = il.DefineLabel();
			var isTagLabel = il.DefineLabel();

			var countLabel = il.DefineLabel();
			var isNullObjectLabel = il.DefineLabel();


			var isDict = type.IsDictionaryType();
			var arguments = isDict ? type.GetGenericArguments() : null;
			var hasArgument = arguments != null;
			var keyType = hasArgument ? (arguments.Length > 0 ? arguments[0] : null) : _objectType;
			var valueType = hasArgument && arguments.Length > 1 ? arguments[1] : _objectType;
			var isKeyValuePair = false;
			var isExpandoObject = type == _expandoObjectType;
			ConstructorInfo selectedCtor = null;

			if (isDict && keyType == null)
			{
				var baseType = type.GetTypeInfo().BaseType;
				if (baseType == _objectType)
				{
					baseType = type.GetInterface(IEnumerableStr);
					if (baseType == null)
						throw new InvalidOperationException(string.Format("Type {0} must be a validate dictionary type such as IDictionary<Key,Value>", type.FullName));
				}
				arguments = baseType.GetGenericArguments();
				keyType = arguments[0];
				valueType = arguments.Length > 1 ? arguments[1] : null;
			}

			if (keyType.Name == KeyValueStr)
			{
				arguments = keyType.GetGenericArguments();
				keyType = arguments[0];
				valueType = arguments[1];
				isKeyValuePair = true;
			}


			var isTuple = type.GetTypeInfo().IsGenericType && type.Name.StartsWith("Tuple");
			var tupleType = isTuple ? type : null;
			var tupleArguments = tupleType != null ? tupleType.GetGenericArguments() : null;
			var tupleCount = tupleType != null ? tupleArguments.Length : 0;

			if (isTuple)
				type = _tupleContainerType;

			var obj = il.DeclareLocal(type);
			var isStringType = isTuple || isDict || keyType == _stringType || keyType == _objectType;
			var isTypeValueType = type.GetTypeInfo().IsValueType;
			var tupleCountLocal = isTuple ? il.DeclareLocal(_intType) : null;
			var isStringTypeLocal = il.DeclareLocal(_boolType);

			MethodInfo addMethod = null;

			var isNotTagLabel = il.DefineLabel();


			var dictSetItem = isDict ? (isKeyValuePair ?
				((addMethod = type.GetMethod("Add")) != null ? addMethod :
				(addMethod = type.GetMethod("Enqueue")) != null ? addMethod :
				(addMethod = type.GetMethod("Push")) != null ? addMethod : null)
				: type.GetMethod("set_Item")) : null;

			if (isExpandoObject)
				dictSetItem = _idictStringObject.GetMethod("Add");

			if (isDict)
			{
				if (type.Name == IDictStr)
					type = _genericDictType.MakeGenericType(keyType, valueType);
			}

			il.Emit(OpCodes.Ldc_I4, isStringType ? 1 : 0);
			il.Emit(OpCodes.Stloc, isStringTypeLocal);


			if (keyType.GetTypeInfo().IsEnum)
			{
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Callvirt, _settingsUseEnumStringProp);
				il.Emit(OpCodes.Stloc, isStringTypeLocal);
			}

			if (tupleCountLocal != null)
			{
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Stloc, tupleCountLocal);
			}


			if (isTypeValueType)
			{
				il.Emit(OpCodes.Ldloca, obj);
				il.Emit(OpCodes.Initobj, type);
			}
			else
			{
				if (isTuple)
				{
					il.Emit(OpCodes.Ldc_I4, tupleCount);
					il.Emit(OpCodes.Newobj, type.GetConstructor(new[] { _intType }));
					il.Emit(OpCodes.Stloc, obj);
				}
				else
				{
					var ctor = type.GetConstructor(Type.EmptyTypes);
					if (ctor == null)
					{
						if (type.GetTypeInfo().IsInterface)
						{
							il.Emit(OpCodes.Ldnull);
						}
						else
						{
							selectedCtor = type.GetConstructors().OrderBy(x => x.GetParameters().Length).LastOrDefault();
							il.Emit(OpCodes.Call, _getUninitializedInstance.MakeGenericMethod(type));
						}
					}
					else
					{
						il.Emit(OpCodes.Newobj, ctor);//NewObjNoctor
					}
					il.Emit(OpCodes.Stloc, obj);
				}
			}

			if (isDict)
			{
				il.Emit(OpCodes.Ldloc, obj);
				il.Emit(OpCodes.Isinst, _dictType);
				il.Emit(OpCodes.Stloc, dict);
			}

			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Stloc, settings);

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, count);

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, quotes);

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, startIndex);

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, isTag);

			il.Emit(OpCodes.Ldc_I4, (int)'\0');
			il.Emit(OpCodes.Stloc, prev);

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, foundQuote);

			ILFixedWhile(il, whileAction: (msil, current, ptr, startLoop, bLabel) => {
				//il.Emit(OpCodes.Ldloc, current);
				//il.Emit(OpCodes.Ldc_I4, (int)' ');
				//il.Emit(OpCodes.Beq, countLabel);

				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Stloc, isTag);

				//if (count == 0 && current == 'n') 
				//{
				//    index += 3;
				//    return null;
				//}
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Ldloc, count);
				il.Emit(OpCodes.Bne_Un, isNullObjectLabel);

				il.Emit(OpCodes.Ldc_I4, (int)'n');
				il.Emit(OpCodes.Ldloc, current);
				il.Emit(OpCodes.Bne_Un, isNullObjectLabel);

				IncrementIndexRef(il, count: 3);

				if (isTypeValueType)
				{
					var nullLocal = il.DeclareLocal(type);

					il.Emit(OpCodes.Ldloca, nullLocal);
					il.Emit(OpCodes.Initobj, type);

					il.Emit(OpCodes.Ldloc, nullLocal);
				}
				else
				{
					il.Emit(OpCodes.Ldnull);
				}

				il.Emit(OpCodes.Ret);

				il.MarkLabel(isNullObjectLabel);


				//current == '{' || current == '}'
				//il.Emit(OpCodes.Ldloc, current);
				//il.Emit(OpCodes.Call, _isCharTag);
				//il.Emit(OpCodes.Brfalse, openCloseBraceLabel);


				var currentisCharTagLabel = il.DefineLabel();
				var countCheckLabel = il.DefineLabel();

				//current == '{' || current == '}';

				il.Emit(OpCodes.Ldloc, current);
				il.Emit(OpCodes.Ldc_I4, (int)'{');
				il.Emit(OpCodes.Beq, currentisCharTagLabel);

				il.Emit(OpCodes.Ldloc, current);
				il.Emit(OpCodes.Ldc_I4, (int)'}');
				il.Emit(OpCodes.Bne_Un, openCloseBraceLabel);
				il.MarkLabel(currentisCharTagLabel);

				//quotes == 0
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Ldloc, quotes);
				il.Emit(OpCodes.Bne_Un, openCloseBraceLabel);

				//isTag = true
				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Stloc, isTag);

				il.MarkLabel(openCloseBraceLabel);

				//if(isTag == true)
				il.Emit(OpCodes.Ldloc, isTag);
				il.Emit(OpCodes.Brfalse, isTagLabel);

				//count++
				il.Emit(OpCodes.Ldloc, count);
				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Add);
				il.Emit(OpCodes.Stloc, count);

				il.MarkLabel(isTagLabel);


				//if(count > 0 && flag == false && quoteCount == 0 && char == ':')
				//Err, No quotes was found
				il.Emit(OpCodes.Ldloc, count);
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Ble, countCheckLabel);
				il.Emit(OpCodes.Ldloc, isTag);
				il.Emit(OpCodes.Brtrue, countCheckLabel);
				il.Emit(OpCodes.Ldloc, current);
				il.Emit(OpCodes.Ldc_I4, (int)':');
				il.Emit(OpCodes.Bne_Un, countCheckLabel);
				il.Emit(OpCodes.Ldloc, foundQuote);
				il.Emit(OpCodes.Brtrue, countCheckLabel);

				il.Emit(OpCodes.Newobj, _invalidJsonCtor);
				il.Emit(OpCodes.Throw);

				il.MarkLabel(countCheckLabel);

				//count == 2
				il.Emit(OpCodes.Ldloc, count);
				il.Emit(OpCodes.Ldc_I4_2);
				il.Emit(OpCodes.Bne_Un, countLabel);

				//index += 1;
				IncrementIndexRef(msil);

				il.Emit(OpCodes.Br, bLabel);

				il.MarkLabel(countLabel);


				//!isTag
				il.Emit(OpCodes.Ldloc, isTag);
				il.Emit(OpCodes.Brtrue, isNotTagLabel);

				var isStringTypeLabel1 = il.DefineLabel();

				il.Emit(OpCodes.Ldloc, isStringTypeLocal);
				il.Emit(OpCodes.Brfalse, isStringTypeLabel1);
				GenerateGetClassOrDictStringType(typeBuilder, type, il, settings, foundQuote, prev, startIndex, quotes, isDict, keyType, valueType, isKeyValuePair, isExpandoObject, isTuple, tupleArguments, tupleCount, obj, isTypeValueType, tupleCountLocal, dictSetItem, current, ptr, startLoop);
				il.MarkLabel(isStringTypeLabel1);

				if (dictSetItem != null)
				{
					var isStringTypeLabel2 = il.DefineLabel();
					il.Emit(OpCodes.Ldloc, isStringTypeLocal);
					il.Emit(OpCodes.Brtrue, isStringTypeLabel2);
					GenerateGetClassOrDictNonStringType(typeBuilder, il, settings, startIndex, keyType, valueType, isKeyValuePair, isExpandoObject, obj, dictSetItem, current, ptr);
					il.MarkLabel(isStringTypeLabel2);
				}

				il.MarkLabel(isNotTagLabel);

				il.Emit(OpCodes.Ldloc, current);
				il.Emit(OpCodes.Stloc, prev);

			}, 
			needBreak: true,
			returnAction: msil => 
			{
				if (isTuple)
				{
					var toTupleMethod = _tupleContainerType.GetMethods().FirstOrDefault(x => x.Name == ToTupleStr && x.GetGenericArguments().Length == tupleCount);
					if (toTupleMethod != null)
					{
						toTupleMethod = toTupleMethod.MakeGenericMethod(tupleType.GetGenericArguments());
						il.Emit(OpCodes.Ldloc, obj);
						il.Emit(OpCodes.Callvirt, toTupleMethod);
					}
				}
				else
				{
					if (selectedCtor != null)
					{
						var sObj = il.DeclareLocal(type);
						var parameters = selectedCtor.GetParameters();
						var props = type.GetTypeProperties();
						var paramProps = props.Where(x => parameters.Any(y => y.Name.Equals(x.Member.Name, StringComparison.OrdinalIgnoreCase)));
						var excludedParams = props.Where(x => !parameters.Any(y => y.Name.Equals(x.Member.Name, StringComparison.OrdinalIgnoreCase)));

						if (paramProps.Any())
						{
							foreach (var parameter in paramProps)
							{
								il.Emit(OpCodes.Ldloc, obj);
								GetMemberInfoValue(il, parameter);
							}

							il.Emit(OpCodes.Newobj, selectedCtor);
							il.Emit(OpCodes.Stloc, sObj);

							//Set field/prop not accounted for in constructor parameters
							foreach (var param in excludedParams)
							{
								il.Emit(OpCodes.Ldloc, sObj);
								il.Emit(OpCodes.Ldloc, obj);
								GetMemberInfoValue(il, param);
								var prop = param.Member.GetMemberType() == MemberTypes.Property ? param.Member as PropertyInfo : null;
								if (prop != null)
								{
									var setter = prop.GetSetMethod();
									if (setter == null)
										setter = type.GetMethod(string.Concat("set_", prop.Name), MethodBinding);

									var propType = prop.PropertyType;

									if (!setter.IsPublic)
									{
										if (propType.GetTypeInfo().IsValueType)
											il.Emit(OpCodes.Box, propType);
										il.Emit(OpCodes.Ldtoken, setter);
										il.Emit(OpCodes.Call, _methodGetMethodFromHandle);
										il.Emit(OpCodes.Call, _setterPropertyValueMethod.MakeGenericMethod(type));
									}
									else
									{
										il.Emit(isTypeValueType ? OpCodes.Call : OpCodes.Callvirt, setter);
									}
								}
								else
								{
									il.Emit(OpCodes.Stfld, (FieldInfo)param.Member);
								}
							}

							il.Emit(OpCodes.Ldloc, sObj);
						}
						else
						{
							il.Emit(OpCodes.Ldloc, obj);
						}
					}
					else
					{
						il.Emit(OpCodes.Ldloc, obj);
					}
				}
			},
			beginIndexIf: (msil, current) => 
			{
				il.Emit(OpCodes.Ldloc, current);
				il.Emit(OpCodes.Ldc_I4, (int)'}');
				il.Emit(OpCodes.Beq, incLabel);
			},
			endIndexIf: (msil, current) => 
			{
				il.MarkLabel(incLabel);
			});


			return method;
		}

		private static void GenerateGetClassOrDictNonStringType(TypeBuilder typeBuilder, ILGenerator il, LocalBuilder settings, LocalBuilder startIndex, Type keyType, Type valueType, bool isKeyValuePair, bool isExpandoObject, LocalBuilder obj, MethodInfo dictSetItem, LocalBuilder current, LocalBuilder ptr)
		{
			var isEndOfChar = il.DeclareLocal(_boolType);
			var text = il.DeclareLocal(_stringType);
			var keyLocal = il.DeclareLocal(keyType);
			var startIndexIsEndCharLabel = il.DefineLabel();
			var startIndexGreaterIsEndOfCharLabel = il.DefineLabel();

			var currentEndCharLabel = il.DefineLabel();
			var currentEndCharLabel2 = il.DefineLabel();

			//current == ':' || current == '{' || current == ' ';

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)':');
			il.Emit(OpCodes.Beq, currentEndCharLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)',');
			il.Emit(OpCodes.Beq, currentEndCharLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)'\n');
			il.Emit(OpCodes.Beq, currentEndCharLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)'\r');
			il.Emit(OpCodes.Beq, currentEndCharLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)'\t');
			il.Emit(OpCodes.Beq, currentEndCharLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)'{');
			il.Emit(OpCodes.Beq, currentEndCharLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldc_I4, (int)' ');
			il.Emit(OpCodes.Ceq);
			il.Emit(OpCodes.Br, currentEndCharLabel2);

			il.MarkLabel(currentEndCharLabel);
			il.Emit(OpCodes.Ldc_I4_1);
			il.MarkLabel(currentEndCharLabel2);

			il.Emit(OpCodes.Stloc, isEndOfChar);

			il.Emit(OpCodes.Ldloc, startIndex);
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Bne_Un, startIndexIsEndCharLabel);
			il.Emit(OpCodes.Ldloc, isEndOfChar);
			il.Emit(OpCodes.Brtrue, startIndexIsEndCharLabel);

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldind_I4);
			il.Emit(OpCodes.Stloc, startIndex);

			il.MarkLabel(startIndexIsEndCharLabel);

			il.Emit(OpCodes.Ldloc, startIndex);
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Ble, startIndexGreaterIsEndOfCharLabel);
			il.Emit(OpCodes.Ldloc, isEndOfChar);
			il.Emit(OpCodes.Brfalse, startIndexGreaterIsEndOfCharLabel);

			il.Emit(OpCodes.Ldloc, ptr);
			il.Emit(OpCodes.Ldloc, startIndex);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldind_I4);
			il.Emit(OpCodes.Ldloc, startIndex);
			il.Emit(OpCodes.Sub);
			il.Emit(OpCodes.Newobj, _strCtorWithPtr);


			il.Emit(OpCodes.Stloc, text);

			GenerateChangeTypeFor(typeBuilder, keyType, il, text, settings);

			il.Emit(OpCodes.Stloc, keyLocal);

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, startIndex);

			IncrementIndexRef(il);

			il.Emit(OpCodes.Ldloc, obj);
			if (isExpandoObject)
				il.Emit(OpCodes.Isinst, _idictStringObject);
			il.Emit(OpCodes.Ldloc, keyLocal);
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Call, GenerateExtractValueFor(typeBuilder, valueType));

			if (isKeyValuePair && !isExpandoObject)
			{
				il.Emit(OpCodes.Newobj, _genericKeyValuePairType.MakeGenericType(keyType, valueType).GetConstructor(new[] { keyType, valueType }));
				il.Emit(OpCodes.Callvirt, dictSetItem);
			}
			else
			{
				il.Emit(OpCodes.Callvirt, dictSetItem);
			}

			GenerateUpdateCurrent(il, current, ptr);


			il.MarkLabel(startIndexGreaterIsEndOfCharLabel);
		}

		private static void GenerateGetClassOrDictStringType(TypeBuilder typeBuilder, Type type, ILGenerator il, LocalBuilder settings, LocalBuilder foundQuote, LocalBuilder prev, LocalBuilder startIndex, LocalBuilder quotes, bool isDict, Type keyType, Type valueType, bool isKeyValuePair, bool isExpandoObject, bool isTuple, Type[] tupleArguments, int tupleCount, LocalBuilder obj, bool isTypeValueType, LocalBuilder tupleCountLocal, MethodInfo dictSetItem, LocalBuilder current, LocalBuilder ptr, Label startLoop)
		{
			var currentQuoteLabel = il.DefineLabel();
			var currentQuotePrevNotLabel = il.DefineLabel();
			var keyLocal = il.DeclareLocal(_stringType);

			var isCurrentLocal = il.DeclareLocal(_boolType);
			var hasOverrideLabel = il.DefineLabel();
			var hasOverrideLabel2 = il.DefineLabel();
			var notHasOverrideLabel = il.DefineLabel();

			var isStringBasedLocal = il.DeclareLocal(_boolType);

			il.Emit(OpCodes.Ldc_I4, keyType.IsStringBasedType() ? 1 : 0);
			il.Emit(OpCodes.Stloc, isStringBasedLocal);

			if (keyType.GetTypeInfo().IsEnum)
			{
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Callvirt, _settingsUseEnumStringProp);
				il.Emit(OpCodes.Stloc, isStringBasedLocal);
			}

			if (keyType == _dateTimeType || keyType == _dateTimeOffsetType)
			{
				var dateCheckLabel = il.DefineLabel();
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Callvirt, _settingsDateFormat);
				il.Emit(OpCodes.Ldc_I4, (int)JsonDateTimeHandling.EpochTime);
				il.Emit(OpCodes.Ceq);
				il.Emit(OpCodes.Brtrue, dateCheckLabel);
				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Stloc, isStringBasedLocal);
				il.MarkLabel(dateCheckLabel);
			}

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, isCurrentLocal);

			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Callvirt, _settingsHasOverrideQuoteChar);
			il.Emit(OpCodes.Brfalse, hasOverrideLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Ldfld, _settingQuoteChar);
			il.Emit(OpCodes.Bne_Un, hasOverrideLabel2);

			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Stloc, isCurrentLocal);

			il.MarkLabel(hasOverrideLabel2);

			il.MarkLabel(hasOverrideLabel);

			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Callvirt, _settingsHasOverrideQuoteChar);
			il.Emit(OpCodes.Brtrue, notHasOverrideLabel);

			il.Emit(OpCodes.Ldloc, current);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Call, _IsCurrentAQuotMethod);
			il.Emit(OpCodes.Stloc, isCurrentLocal);

			il.MarkLabel(notHasOverrideLabel);

			il.Emit(OpCodes.Ldloc, isCurrentLocal);

			//if(current == _ThreadQuoteChar && quotes == 0)

			//il.Emit(OpCodes.Ldloc, current);
			//il.Emit(OpCodes.Call, _IsCurrentAQuotMethod);


			il.Emit(OpCodes.Brfalse, currentQuoteLabel);

			il.Emit(OpCodes.Ldloc, quotes);
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Bne_Un, currentQuoteLabel);

			//foundQuote = true
			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Stloc, foundQuote);

			//quotes++
			il.Emit(OpCodes.Ldloc, quotes);
			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Add);
			il.Emit(OpCodes.Stloc, quotes);

			//startIndex = index + 1;
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldind_I4);
			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Add);
			il.Emit(OpCodes.Stloc, startIndex);


			// --- string skipping optimization ---

			var skipOptimizeLabel = il.DefineLabel();
			var skipOptimizeLocal = il.DeclareLocal(_boolType);

			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Callvirt, _settingsUseStringOptimization);
			il.Emit(OpCodes.Brfalse, skipOptimizeLabel);

			if (!isDict)
			{
				var typeProps = type.GetTypeProperties();

				var nextLabel = il.DefineLabel();

				foreach (var prop in typeProps.OrderBy(x => x.Member.Name.Length))
				{
					var propName = prop.Member.Name;
					var attr = prop.Attribute;
					if (attr != null)
						propName = attr.Name ?? propName;
					var set = propName.Length;
					var checkCharByIndexLabel = il.DefineLabel();

					il.Emit(OpCodes.Ldloc, ptr);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldc_I4, set);
					il.Emit(OpCodes.Ldstr, propName);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Call, _isInRange);
					il.Emit(OpCodes.Brfalse, checkCharByIndexLabel);

					IncrementIndexRef(il, count: set - 1);
					il.Emit(OpCodes.Ldc_I4_1);
					il.Emit(OpCodes.Stloc, foundQuote);

					il.Emit(OpCodes.Br, nextLabel);

					il.MarkLabel(checkCharByIndexLabel);

				}

				il.MarkLabel(nextLabel);
			}

			il.MarkLabel(skipOptimizeLabel);

			// --- /string skipping optimization ---

			il.Emit(OpCodes.Br, currentQuotePrevNotLabel);
			il.MarkLabel(currentQuoteLabel);
			//else if(current == _ThreadQuoteChar && quotes > 0 && prev != '\\')
			il.Emit(OpCodes.Ldloc, current);
			//il.Emit(OpCodes.Ldc_I4, (int)_ThreadQuoteChar);
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Ldfld, _settingQuoteChar);

			il.Emit(OpCodes.Bne_Un, currentQuotePrevNotLabel);
			il.Emit(OpCodes.Ldloc, quotes);
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Ble, currentQuotePrevNotLabel);
			il.Emit(OpCodes.Ldloc, prev);
			il.Emit(OpCodes.Ldc_I4, (int)'\\');
			il.Emit(OpCodes.Beq, currentQuotePrevNotLabel);

			//var key = new string(ptr, startIndex, index - startIndex)
			il.Emit(OpCodes.Ldloc, ptr);
			il.Emit(OpCodes.Ldloc, startIndex);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldind_I4);
			il.Emit(OpCodes.Ldloc, startIndex);
			il.Emit(OpCodes.Sub);
			il.Emit(OpCodes.Newobj, _strCtorWithPtr);
			//il.Emit(OpCodes.Call, _createString);
			il.Emit(OpCodes.Stloc, keyLocal);

			//index++
			IncrementIndexRef(il);

			if (isDict)
			{
				var isStringBasedLabel1 = il.DefineLabel();
				var isStringBasedLabel2 = il.DefineLabel();

				il.Emit(OpCodes.Ldloc, isStringBasedLocal);
				il.Emit(OpCodes.Brfalse, isStringBasedLabel1);

				// --- true ---

				il.Emit(OpCodes.Ldloc, obj);
				if (isExpandoObject)
					il.Emit(OpCodes.Isinst, _idictStringObject);

				GenerateChangeTypeFor(typeBuilder, keyType, il, keyLocal, settings);

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Call, GenerateExtractValueFor(typeBuilder, valueType));
				if (isKeyValuePair && !isExpandoObject)
				{
					il.Emit(OpCodes.Newobj, _genericKeyValuePairType.MakeGenericType(keyType, valueType).GetConstructor(new[] { keyType, valueType }));
				}
				il.Emit(OpCodes.Callvirt, dictSetItem);

				// --- /true ---

				il.MarkLabel(isStringBasedLabel1);


				il.Emit(OpCodes.Ldloc, isStringBasedLocal);
				il.Emit(OpCodes.Brtrue, isStringBasedLabel2);


				// --- false ---

				il.Emit(OpCodes.Ldloc, obj);
				if (isExpandoObject)
					il.Emit(OpCodes.Isinst, _idictStringObject);

				il.Emit(OpCodes.Ldloc, keyLocal);

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Call, GenerateExtractValueFor(typeBuilder, valueType));
				if (isKeyValuePair && !isExpandoObject)
				{
					il.Emit(OpCodes.Newobj, _genericKeyValuePairType.MakeGenericType(keyType, valueType).GetConstructor(new[] { keyType, valueType }));
				}
				il.Emit(OpCodes.Callvirt, dictSetItem);
				
				// --- /false ---

				il.MarkLabel(isStringBasedLabel2);
			}
			else
			{
				if (!isTuple)
				{
					//Set property based on key
					if (IncludeTypeInfo)
					{
						var typeIdentifierLabel = il.DefineLabel();
						var notTypeIdentifierLabel = il.DefineLabel();

						il.Emit(OpCodes.Ldloc, keyLocal);
						il.Emit(OpCodes.Ldstr, TypeIdentifier);
						il.Emit(OpCodes.Call, _stringOpEquality);
						il.Emit(OpCodes.Brfalse, typeIdentifierLabel);

						il.Emit(OpCodes.Ldarg_0);
						il.Emit(OpCodes.Ldarg_1);
						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Call, _getStringBasedValue);
						il.Emit(OpCodes.Call, _getTypeIdentifierInstanceMethod);
						il.Emit(OpCodes.Isinst, type);
						il.Emit(OpCodes.Stloc, obj);

						il.Emit(OpCodes.Br, notTypeIdentifierLabel);
						il.MarkLabel(typeIdentifierLabel);

						il.Emit(OpCodes.Ldarg_0);
						il.Emit(OpCodes.Ldarg_1);
						il.Emit(isTypeValueType ? OpCodes.Ldloca : OpCodes.Ldloc, obj);
						il.Emit(OpCodes.Ldloc, keyLocal);
						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Call, GenerateSetValueFor(typeBuilder, type));

						il.MarkLabel(notTypeIdentifierLabel);
					}
					else
					{
						il.Emit(OpCodes.Ldarg_0);
						il.Emit(OpCodes.Ldarg_1);
						il.Emit(isTypeValueType ? OpCodes.Ldloca : OpCodes.Ldloc, obj);
						il.Emit(OpCodes.Ldloc, keyLocal);
						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Call, GenerateSetValueFor(typeBuilder, type));
					}
				}
				else
				{

					for (var i = 0; i < tupleCount; i++)
					{
						GenerateTupleConvert(typeBuilder, i, il, tupleArguments, obj, tupleCountLocal, settings);
					}

					il.Emit(OpCodes.Ldloc, tupleCountLocal);
					il.Emit(OpCodes.Ldc_I4_1);
					il.Emit(OpCodes.Add);
					il.Emit(OpCodes.Stloc, tupleCountLocal);
				}
			}

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, quotes);

			il.Emit(OpCodes.Br, startLoop);

			il.MarkLabel(currentQuotePrevNotLabel);
		}

		private static void GenerateTupleConvert(TypeBuilder typeBuilder, int tupleIndex, ILGenerator il, Type[] tupleArguments, LocalBuilder obj, LocalBuilder tupleCountLocal, LocalBuilder settings)
		{
			var compareTupleIndexLabel = il.DefineLabel();
			var tupleItemType = tupleArguments[tupleIndex];

			il.Emit(OpCodes.Ldloc, tupleCountLocal);
			il.Emit(OpCodes.Ldc_I4, tupleIndex);
			il.Emit(OpCodes.Bne_Un, compareTupleIndexLabel);

			il.Emit(OpCodes.Ldloc, obj);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldloc, settings);
			il.Emit(OpCodes.Call, GenerateExtractValueFor(typeBuilder, tupleItemType));
			if (tupleItemType.GetTypeInfo().IsValueType)
				il.Emit(OpCodes.Box, tupleItemType);

			il.Emit(OpCodes.Callvirt, _tupleContainerAdd);

			il.MarkLabel(compareTupleIndexLabel);
		}

		/// <summary>
		/// index++
		/// </summary>
		private static void IncrementIndexRef(ILGenerator il, int count = 1)
		{
			//index += 1;
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Dup);
			il.Emit(OpCodes.Ldind_I4);
			il.Emit(OpCodes.Ldc_I4, count);
			il.Emit(OpCodes.Add);
			//Store updated index at index address
			il.Emit(OpCodes.Stind_I4);
		}

		private static void GetMemberInfoValue(ILGenerator il, JsonMemberInfo parameter)
		{
			var prop = parameter.Member.GetMemberType() == MemberTypes.Property 
				? parameter.Member as PropertyInfo 
				: null;

			if (prop != null)
				il.Emit(OpCodes.Callvirt, prop.GetGetMethod());
			else
				il.Emit(OpCodes.Ldfld, (FieldInfo)parameter.Member);
		}

		private static void ILFixedWhile(ILGenerator il, Action<ILGenerator, LocalBuilder, LocalBuilder, Label, Label> whileAction,
			bool needBreak = false, Action<ILGenerator> returnAction = null,
			Action<ILGenerator, LocalBuilder> beforeAction = null,
			Action<ILGenerator, LocalBuilder> beginIndexIf = null,
			Action<ILGenerator, LocalBuilder> endIndexIf = null)
		{

			var current = il.DeclareLocal(_charType);
			var ptr = il.DeclareLocal(_charPtrType);

			var startLoop = il.DefineLabel();
			var br = needBreak ? il.DefineLabel() : default(Label);

			//Logic before loop

			//current = '\0';
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, current);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Stloc, ptr);

			if (beforeAction != null)
				beforeAction(il, ptr);

			//Begin while loop
			il.MarkLabel(startLoop);

			GenerateUpdateCurrent(il, current, ptr);

			//Logic within loop
			if (whileAction != null)
				whileAction(il, current, ptr, startLoop, br);

			if (beginIndexIf != null)
				beginIndexIf(il, current);

			IncrementIndexRef(il);

			if (endIndexIf != null)
				endIndexIf(il, current);


			il.Emit(OpCodes.Br, startLoop);

			if (needBreak)
				il.MarkLabel(br);

			if (returnAction != null)
				returnAction(il);

			il.Emit(OpCodes.Ret);
		}

		internal static void EmitClearStringBuilder(this ILGenerator il)
		{
#if NET35
			il.Emit(OpCodes.Call, typeof(StringBuilderExtension).GetMethod("Clear"));
#else
			il.Emit(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Clear"));
#endif
		}
	}
}
