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
		internal static MethodInfo WriteDeserializeMethodFor(TypeBuilder typeBuilder, Type type)
		{
			MethodInfo method;
			var key = string.Concat(type.FullName, typeBuilder == null ? Dynamic : string.Empty);
			var typeName = type.GetName().Fix();
			if (_readDeserializeMethodBuilders.TryGetValue(key, out method))
				return method;
			var methodName = String.Concat(ReadStr, typeName);
			method = typeBuilder.DefineMethodEx(methodName, StaticMethodAttribute,
				type, new[] { _stringType, _settingsType });
			_readDeserializeMethodBuilders[key] = method;
			var il = method.GetILGenerator();

			var index = il.DeclareLocal(_intType);

			var ptr = il.DeclareLocal(typeof(char*));
			var pinned = il.DeclareLocal(typeof(string), true);

			var @fixed = il.DefineLabel();

			//fixed
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Stloc, pinned);

			il.Emit(OpCodes.Ldloc, pinned);
			il.Emit(OpCodes.Conv_I);
			il.Emit(OpCodes.Dup);

			il.Emit(OpCodes.Brfalse, @fixed);
			il.Emit(OpCodes.Call, typeof(RuntimeHelpers).GetMethod("get_OffsetToStringData"));
			il.Emit(OpCodes.Add);
			il.MarkLabel(@fixed);

			//char* ptr = str;
			il.Emit(OpCodes.Stloc, ptr);

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, index);

			if (type == _objectType)
			{
				var startsWithLabel = il.DefineLabel();
				var notStartsWithLabel = il.DefineLabel();
				var startsWith = il.DeclareLocal(_boolType);
				var notDictOrArrayLabel = il.DefineLabel();
				var notDictOrArray = il.DeclareLocal(_boolType);

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Call, _isRawPrimitive);
				il.Emit(OpCodes.Stloc, notDictOrArray);

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldstr, "[");
				il.Emit(OpCodes.Callvirt, _stringType.GetMethod("StartsWith", new[] { _stringType }));
				il.Emit(OpCodes.Stloc, startsWith);

				il.Emit(OpCodes.Ldloc, notDictOrArray);
				il.Emit(OpCodes.Brfalse, notDictOrArrayLabel);

				//IsPrimitive
				il.Emit(OpCodes.Ldloc, ptr);
				il.Emit(OpCodes.Ldloca, index);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Call, GenerateExtractValueFor(typeBuilder, type));
				il.Emit(OpCodes.Ret);

				il.MarkLabel(notDictOrArrayLabel);

				il.Emit(OpCodes.Ldloc, startsWith);
				il.Emit(OpCodes.Brfalse, startsWithLabel);

				//Fast fail when invalid json exists
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldc_I4, (int)'[');
				il.Emit(OpCodes.Call, _throwIfInvalidJson);

				//IsArray
				il.Emit(OpCodes.Ldloc, ptr);
				il.Emit(OpCodes.Ldloca, index);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Call, GenerateCreateListFor(typeBuilder, typeof(List<object>)));
				il.Emit(OpCodes.Ret);

				il.MarkLabel(startsWithLabel);

				il.Emit(OpCodes.Ldloc, startsWith);
				il.Emit(OpCodes.Brtrue, notStartsWithLabel);

				//Fast fail when invalid json exists
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldc_I4, (int)'{');
				il.Emit(OpCodes.Call, _throwIfInvalidJson);

				//IsDictionary
				il.Emit(OpCodes.Ldloc, ptr);
				il.Emit(OpCodes.Ldloca, index);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Call, GenerateGetClassOrDictFor(typeBuilder,
					typeof(Dictionary<string, object>)));
				il.Emit(OpCodes.Ret);

				il.MarkLabel(notStartsWithLabel);

				il.Emit(OpCodes.Ldnull);
			}
			else
			{
				var isPrimitive = type.IsPrimitiveType();
				var isArray = (type.IsListType() || type.IsArray) && !isPrimitive;
				var isComplex = isArray || type.IsDictionaryType() || !isPrimitive;

				if (isComplex)
				{
					//Fast fail when invalid json exists
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldc_I4, (int)(isArray ? '[' : '{'));
					il.Emit(OpCodes.Call, _throwIfInvalidJson);
				}

				il.Emit(OpCodes.Ldloc, ptr);
				il.Emit(OpCodes.Ldloca, index);
				il.Emit(OpCodes.Ldarg_1);
				if (isArray)
				{
					il.Emit(OpCodes.Call, GenerateCreateListFor(typeBuilder, type));
				}
				else
				{
					if (isPrimitive)
						il.Emit(OpCodes.Call, GenerateExtractValueFor(typeBuilder, type));
					else
						il.Emit(OpCodes.Call, GenerateGetClassOrDictFor(typeBuilder, type));
				}
			}

			il.Emit(OpCodes.Ret);

			return method;
		}

		internal static MethodInfo WriteSerializeMethodFor(TypeBuilder typeBuilder, Type type, bool needQuote = true)
		{
			MethodInfo method;
			var key = string.Concat(type.FullName, typeBuilder == null ? Dynamic : string.Empty);
			var typeName = type.GetName().Fix();
			if (_writeMethodBuilders.TryGetValue(key, out method))
				return method;
			var methodName = string.Concat(WriteStr, typeName);

			method = typeBuilder.DefineMethodEx(methodName, StaticMethodAttribute,
				_voidType, new[] { type, _stringBuilderType, _settingsType });
			_writeMethodBuilders[key] = method;
			var il = method.GetILGenerator();
			var isTypeObject = type == _objectType;
			var originalType = type;
			var nullableType = type.GetNullableType();
			var isNullable = nullableType != null && !originalType.IsArray;
			type = isNullable ? nullableType : type;

			if (type.IsPrimitiveType() || isTypeObject)
			{
				var nullLabel = il.DefineLabel();
				var valueLocal = isNullable ? il.DeclareLocal(type) : null;

				if (isNullable)
				{
					var nullableValue = il.DeclareLocal(originalType);
					var nullableLabel = il.DefineLabel();

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Stloc, nullableValue);

					var hasValueMethod = originalType.GetMethod("get_HasValue");

					if (hasValueMethod != null)
					{
						il.Emit(OpCodes.Ldloca, nullableValue);
						il.Emit(OpCodes.Call, hasValueMethod);
					}
					else
					{
						il.Emit(OpCodes.Ldloc, nullableValue);
					}

					il.Emit(OpCodes.Brtrue, nullableLabel);

					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldstr, NullStr);
					il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
					il.Emit(OpCodes.Pop);

					il.Emit(OpCodes.Ret);

					il.MarkLabel(nullableLabel);

					il.Emit(OpCodes.Ldloca, nullableValue);
					il.Emit(OpCodes.Call, originalType.GetMethod("GetValueOrDefault", Type.EmptyTypes));

					il.Emit(OpCodes.Stloc, valueLocal);
				}

				needQuote = needQuote && (type == _stringType || type == _charType || type == _guidType || type == _timeSpanType || type == _byteArrayType);

				if (type == _stringType || isTypeObject)
				{
					if (isNullable)
						il.Emit(OpCodes.Ldloc, valueLocal);
					else
						il.Emit(OpCodes.Ldarg_0);

					if (type == _stringType)
					{
						il.Emit(OpCodes.Ldnull);
						il.Emit(OpCodes.Call, _stringOpEquality);
						il.Emit(OpCodes.Brfalse, nullLabel);
					}
					else
					{
						il.Emit(OpCodes.Brtrue, nullLabel);
					}

					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldstr, NullStr);
					il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
					il.Emit(OpCodes.Pop);

					il.Emit(OpCodes.Ret);

					il.MarkLabel(nullLabel);

					if (needQuote)
					{
						il.Emit(OpCodes.Ldarg_1);
						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						//il.Emit(OpCodes.Pop);
					}

					//il.Emit(OpCodes.Ldarg_2);

					if (type == _objectType)
					{
						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Ldarg_1);
						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Call, GenerateFastObjectToString(typeBuilder));
						il.Emit(OpCodes.Ldarg_1);
						//il.Emit(OpCodes.Pop);
					}
					else if (type == _stringType)
					{
						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						//il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Call, _encodedJsonString);
						il.Emit(OpCodes.Ldarg_1);
					}

					if (needQuote)
					{
						//il.Emit(OpCodes.Ldarg_2);
						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);
					}
					else
					{
						il.Emit(OpCodes.Pop);
					}
				}
				else
				{
					if (type == _dateTimeType || type == _dateTimeOffsetType)
					{
						var needDateQuoteLocal = il.DeclareLocal(_boolType);
						var needDateCheck = il.DefineLabel();
						var needDateQuoteLabel1 = il.DefineLabel();
						var needDateQuoteLabel2 = il.DefineLabel();

						il.Emit(OpCodes.Ldc_I4_0);
						il.Emit(OpCodes.Stloc, needDateQuoteLocal);

						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Callvirt, _settingsDateFormat);
						il.Emit(OpCodes.Ldc_I4, (int)JsonDateTimeHandling.EpochTime);
						il.Emit(OpCodes.Ceq);
						il.Emit(OpCodes.Brfalse, needDateCheck);

						il.Emit(OpCodes.Ldc_I4_1);
						il.Emit(OpCodes.Stloc, needDateQuoteLocal);

						il.MarkLabel(needDateCheck);

						il.Emit(OpCodes.Ldloc, needDateQuoteLocal);
						il.Emit(OpCodes.Brtrue, needDateQuoteLabel1);

						il.Emit(OpCodes.Ldarg_1);
						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);

						il.MarkLabel(needDateQuoteLabel1);

						//if (needDateQuote) {
						//    il.Emit(OpCodes.Ldarg_1);
						//    LoadQuotChar(il);
						//    il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						//    il.Emit(OpCodes.Pop);
						//}

						il.Emit(OpCodes.Ldarg_1);
						//il.Emit(OpCodes.Ldstr, IsoFormat);

						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						//il.Emit(OpCodes.Box, _dateTimeType);
						//il.Emit(OpCodes.Call, _stringFormat);
						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Call, type == _dateTimeType ? _generatorDateToStr : _generatorDateOffsetToStr);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);

						//if (needDateQuote) {
						//    il.Emit(OpCodes.Ldarg_1);
						//    LoadQuotChar(il);
						//    il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						//    il.Emit(OpCodes.Pop);
						//}

						il.Emit(OpCodes.Ldloc, needDateQuoteLocal);
						il.Emit(OpCodes.Brtrue, needDateQuoteLabel2);

						il.Emit(OpCodes.Ldarg_1);
						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);

						il.MarkLabel(needDateQuoteLabel2);
					}
					else if (type == _byteArrayType)
					{
						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Brtrue, nullLabel);

						il.Emit(OpCodes.Ldarg_1);
						il.Emit(OpCodes.Ldstr, NullStr);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);

						il.Emit(OpCodes.Ret);
						il.MarkLabel(nullLabel);

						il.Emit(OpCodes.Ldarg_1);
						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);

						il.Emit(OpCodes.Ldarg_1);

						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Call, _convertBase64);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);
						il.Emit(OpCodes.Ldarg_1);
						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);
					}
					else if (type == _boolType)
					{
						var boolLocal = il.DeclareLocal(_stringType);
						var boolLabel = il.DefineLabel();
						il.Emit(OpCodes.Ldstr, "true");
						il.Emit(OpCodes.Stloc, boolLocal);

						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Brtrue, boolLabel);
						il.Emit(OpCodes.Ldstr, "false");
						il.Emit(OpCodes.Stloc, boolLocal);
						il.MarkLabel(boolLabel);

						il.Emit(OpCodes.Ldarg_1);
						il.Emit(OpCodes.Ldloc, boolLocal);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);
					}
					else if (type.GetTypeInfo().IsEnum)
					{
						var useEnumStringLocal = il.DeclareLocal(_boolType);
						var useEnumLabel1 = il.DefineLabel();
						var useEnumLabel2 = il.DefineLabel();

						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Callvirt, _settingsUseEnumStringProp);
						il.Emit(OpCodes.Stloc, useEnumStringLocal);

						il.Emit(OpCodes.Ldloc, useEnumStringLocal);
						il.Emit(OpCodes.Brfalse, useEnumLabel1);
						il.Emit(OpCodes.Ldarg_1);
						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);

						il.MarkLabel(useEnumLabel1);

						il.Emit(OpCodes.Ldarg_1);

						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Call, WriteEnumToStringFor(typeBuilder, type));
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);

						il.Emit(OpCodes.Ldloc, useEnumStringLocal);
						il.Emit(OpCodes.Brfalse, useEnumLabel2);
						il.Emit(OpCodes.Ldarg_1);
						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);
						il.MarkLabel(useEnumLabel2);
					}
					else if (type == _floatType)
					{
						il.Emit(OpCodes.Ldarg_1);

						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Call, _generatorSingleToStr);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);
					}
					else if (type == _doubleType)
					{
						il.Emit(OpCodes.Ldarg_1);

						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Call, _generatorDoubleToStr);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);
					}
					else if (type == _sbyteType)
					{
						il.Emit(OpCodes.Ldarg_1);

						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Call, _generatorSByteToStr);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);
					}
					else if (type == _decimalType)
					{
						il.Emit(OpCodes.Ldarg_1);

						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Call, _generatorDecimalToStr);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);
					}
					else if (type == _typeType)
					{
						il.Emit(OpCodes.Ldarg_1);
						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);

						il.Emit(OpCodes.Ldarg_0);
						il.Emit(OpCodes.Callvirt, _assemblyQualifiedName);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);

						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);
					}
					else if (type == _guidType)
					{
						il.Emit(OpCodes.Ldarg_1);
						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);

						il.Emit(OpCodes.Ldarg_1);

						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Call, _guidToStr);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);

						il.Emit(OpCodes.Ldarg_1);
						LoadQuotChar(il);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);
					}
					else
					{
						if (needQuote)
						{
							il.Emit(OpCodes.Ldarg_1);
							LoadQuotChar(il);
							il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
							il.Emit(OpCodes.Pop);
						}

						il.Emit(OpCodes.Ldarg_1);

						if (isNullable)
							il.Emit(OpCodes.Ldloc, valueLocal);
						else
							il.Emit(OpCodes.Ldarg_0);

						il.Emit(OpCodes.Box, type);
						il.Emit(OpCodes.Callvirt, _objectToString);
						il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
						il.Emit(OpCodes.Pop);

						if (needQuote)
						{
							il.Emit(OpCodes.Ldarg_1);
							LoadQuotChar(il);
							il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
							il.Emit(OpCodes.Pop);
						}
					}
				}
			}
			else
			{
				WriteSerializeFor(typeBuilder, type, il);
			}
			il.Emit(OpCodes.Ret);
			return method;
		}

		internal static void WriteSerializeFor(TypeBuilder typeBuilder, Type type, ILGenerator methodIL)
		{
			var conditionLabel = methodIL.DefineLabel();

			if (type.GetTypeInfo().IsValueType)
			{
				var defaultValue = methodIL.DeclareLocal(type);

				methodIL.Emit(OpCodes.Ldarga, 0);

				methodIL.Emit(OpCodes.Ldloca, defaultValue);
				methodIL.Emit(OpCodes.Initobj, type);
				methodIL.Emit(OpCodes.Ldloc, defaultValue);
				methodIL.Emit(OpCodes.Box, type);
				methodIL.Emit(OpCodes.Constrained, type);

				methodIL.Emit(OpCodes.Callvirt, _objectEquals);

				methodIL.Emit(OpCodes.Brfalse, conditionLabel);
			}
			else
			{
				methodIL.Emit(OpCodes.Ldarg_0);
				methodIL.Emit(OpCodes.Brtrue, conditionLabel);
			}

			methodIL.Emit(OpCodes.Ldarg_1);
			methodIL.Emit(OpCodes.Ldstr, NullStr);
			methodIL.Emit(OpCodes.Callvirt, _stringBuilderAppend);
			methodIL.Emit(OpCodes.Pop);
			methodIL.Emit(OpCodes.Ret);
			methodIL.MarkLabel(conditionLabel);

			if (type.IsCollectionType())
			{
				WriteCollection(typeBuilder, type, methodIL);
			}
			else
			{
				if (!IncludeTypeInfo)
				{
					//if (!_includeTypeInformation) {
					WritePropertiesFor(typeBuilder, type, methodIL);
				}
				else
				{
					var pTypes = GetIncludedTypeTypes(type);
					if (pTypes.Count == 1)
					{
						WritePropertiesFor(typeBuilder, type, methodIL);
					}
					else
					{
						var typeLocal = methodIL.DeclareLocal(typeof(Type));
						methodIL.Emit(OpCodes.Ldarg_0);
						methodIL.Emit(OpCodes.Callvirt, _objectGetType);
						methodIL.Emit(OpCodes.Stloc, typeLocal);

						foreach (var pType in pTypes)
						{
							var compareLabel = methodIL.DefineLabel();

							methodIL.Emit(OpCodes.Ldloc, typeLocal);

							methodIL.Emit(OpCodes.Ldtoken, pType);
							methodIL.Emit(OpCodes.Call, _typeGetTypeFromHandle);

							methodIL.Emit(OpCodes.Call, _cTypeOpEquality);

							methodIL.Emit(OpCodes.Brfalse, compareLabel);

							WritePropertiesFor(typeBuilder, pType, methodIL, isPoly: true);

							methodIL.MarkLabel(compareLabel);
						}
					}
				}
			}
		}

		internal static void WriteCollection(TypeBuilder typeBuilder, Type type, ILGenerator il)
		{
			var isDict = type.IsDictionaryType();

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldc_I4_S, isDict ? ObjectOpen : ArrayOpen);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppendChar);
			il.Emit(OpCodes.Pop);

			if (isDict)
				WriteDictionary(typeBuilder, type, il);
			else
				WriteListArray(typeBuilder, type, il);

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldc_I4_S, isDict ? ObjectClose : ArrayClose);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppendChar);
			il.Emit(OpCodes.Pop);
		}

		internal static void WriteDictionary(TypeBuilder typeBuilder, Type type, ILGenerator il)
		{
			var arguments = type.GetGenericArguments();
			var keyType = arguments != null && arguments.Length > 0 ? arguments[0] : null;
			var valueType = arguments != null && arguments.Length > 1 ? arguments[1] : null;

			if (keyType == null || valueType == null)
			{
				var baseType = type.GetTypeInfo().BaseType;
				if (baseType == _objectType)
				{
					baseType = type.GetInterface(IEnumerableStr);
					if (baseType == null)
					{
						throw new InvalidOperationException(string.Format(RS.ExpectGenericIDictionaryType, type.FullName));
					}
				}

				if (baseType.Name != IEnumerableStr && !baseType.IsDictionaryType())
					throw new InvalidOperationException(string.Format(RS.ExpectGenericIDictionaryType, type.FullName));

				arguments = baseType.GetGenericArguments();
				keyType = arguments[0];
				valueType = arguments.Length > 1 ? arguments[1] : null;
			}

			if (keyType.Name == KeyValueStr)
			{
				arguments = keyType.GetGenericArguments();
				keyType = arguments[0];
				valueType = arguments[1];
			}

			var isKeyPrimitive = keyType.IsPrimitiveType();
			var isValuePrimitive = valueType.IsPrimitiveType();
			var keyValuePairType = _genericKeyValuePairType.MakeGenericType(keyType, valueType);
			var enumerableType = _ienumerableType.MakeGenericType(keyValuePairType);
			var enumeratorType = _enumeratorType.MakeGenericType(keyValuePairType);//_genericDictionaryEnumerator.MakeGenericType(keyType, valueType);
			var enumeratorLocal = il.DeclareLocal(enumeratorType);
			var entryLocal = il.DeclareLocal(keyValuePairType);
			var startEnumeratorLabel = il.DefineLabel();
			var moveNextLabel = il.DefineLabel();
			var endEnumeratorLabel = il.DefineLabel();
			var hasItem = il.DeclareLocal(_boolType);
			var hasItemLabel = il.DefineLabel();

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, hasItem);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Callvirt,
				enumerableType.GetMethod("GetEnumerator"));
			il.Emit(OpCodes.Stloc, enumeratorLocal);
			il.BeginExceptionBlock();
			il.Emit(OpCodes.Br, startEnumeratorLabel);
			il.MarkLabel(moveNextLabel);
			il.Emit(OpCodes.Ldloc, enumeratorLocal);
			il.Emit(OpCodes.Callvirt,
				enumeratorLocal.LocalType.GetProperty("Current").GetGetMethod());
			il.Emit(OpCodes.Stloc, entryLocal);

			il.Emit(OpCodes.Ldloc, hasItem);
			il.Emit(OpCodes.Brfalse, hasItemLabel);

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldc_I4_S, Delimeter);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppendChar);
			il.Emit(OpCodes.Pop);

			il.MarkLabel(hasItemLabel);

			il.Emit(OpCodes.Ldarg_1);

			LoadQuotChar(il);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppend);

			il.Emit(OpCodes.Ldloca, entryLocal);
			il.Emit(OpCodes.Call, keyValuePairType.GetProperty("Key").GetGetMethod());

			if (keyType == _intType || keyType == _longType)
			{
				il.Emit(OpCodes.Call, keyType == _intType ? _generatorInt32ToStr : _generatorInt64ToStr);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
			}
			else
			{
				if (keyType.GetTypeInfo().IsValueType)
					il.Emit(OpCodes.Box, keyType);

				il.Emit(OpCodes.Callvirt, _stringBuilderAppendObject);
			}

			LoadQuotChar(il);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppend);

			il.Emit(OpCodes.Ldstr, Colon);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppend);

			il.Emit(OpCodes.Pop);

			//il.Emit(OpCodes.Ldarg_0);
			if (valueType == _intType || valueType == _longType)
			{
				il.Emit(OpCodes.Ldarg_1);

				il.Emit(OpCodes.Ldloca, entryLocal);
				il.Emit(OpCodes.Call, keyValuePairType.GetProperty("Value").GetGetMethod());

				il.Emit(OpCodes.Call, valueType == _intType ? _generatorInt32ToStr : _generatorInt64ToStr);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
				il.Emit(OpCodes.Pop);
			}
			else
			{
				il.Emit(OpCodes.Ldloca, entryLocal);
				il.Emit(OpCodes.Call, keyValuePairType.GetProperty("Value").GetGetMethod());

				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Call, WriteSerializeMethodFor(typeBuilder, valueType));
			}

			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Stloc, hasItem);

			il.MarkLabel(startEnumeratorLabel);
			il.Emit(OpCodes.Ldloc, enumeratorLocal);
			il.Emit(OpCodes.Callvirt, _enumeratorTypeNonGeneric.GetMethod("MoveNext", MethodBinding));
			il.Emit(OpCodes.Brtrue, moveNextLabel);
			il.Emit(OpCodes.Leave, endEnumeratorLabel);
			il.BeginFinallyBlock();
			il.Emit(OpCodes.Ldloc, enumeratorLocal);
			il.Emit(OpCodes.Callvirt, _iDisposableDispose);
			il.EndExceptionBlock();
			il.MarkLabel(endEnumeratorLabel);
		}

		internal static void WriteICollectionArray(TypeBuilder typeBuilder, Type type, ILGenerator il)
		{
			var arguments = type.GetGenericArguments();
			var itemType = arguments[0];

			var isItemPrimitive = itemType.IsPrimitiveType();

			var enumerableType = _ienumerableType.MakeGenericType(itemType);
			var enumeratorType = _enumeratorType.MakeGenericType(itemType);
			var enumeratorLocal = il.DeclareLocal(enumeratorType);
			var entryLocal = il.DeclareLocal(itemType);
			var startEnumeratorLabel = il.DefineLabel();
			var moveNextLabel = il.DefineLabel();
			var endEnumeratorLabel = il.DefineLabel();
			var hasItem = il.DeclareLocal(_boolType);
			var hasItemLabel = il.DefineLabel();

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, hasItem);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Callvirt,
				enumerableType.GetMethod("GetEnumerator"));
			il.Emit(OpCodes.Stloc, enumeratorLocal);
			il.BeginExceptionBlock();
			il.Emit(OpCodes.Br, startEnumeratorLabel);
			il.MarkLabel(moveNextLabel);
			il.Emit(OpCodes.Ldloc, enumeratorLocal);
			il.Emit(OpCodes.Callvirt,
				enumeratorLocal.LocalType.GetProperty("Current").GetGetMethod());
			il.Emit(OpCodes.Stloc, entryLocal);

			il.Emit(OpCodes.Ldloc, hasItem);
			il.Emit(OpCodes.Brfalse, hasItemLabel);

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldc_I4_S, Delimeter);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppendChar);
			il.Emit(OpCodes.Pop);

			il.MarkLabel(hasItemLabel);

			//il.Emit(OpCodes.Ldarg_0);
			if (itemType == _intType || itemType == _longType)
			{
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldloc, entryLocal);
				il.Emit(OpCodes.Call, itemType == _intType ? _generatorInt32ToStr : _generatorInt64ToStr);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
				il.Emit(OpCodes.Pop);
			}
			else
			{
				il.Emit(OpCodes.Ldloc, entryLocal);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Call, WriteSerializeMethodFor(typeBuilder, itemType));
			}

			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Stloc, hasItem);

			il.MarkLabel(startEnumeratorLabel);
			il.Emit(OpCodes.Ldloc, enumeratorLocal);
			il.Emit(OpCodes.Callvirt, _enumeratorTypeNonGeneric.GetMethod("MoveNext", MethodBinding));
			il.Emit(OpCodes.Brtrue, moveNextLabel);
			il.Emit(OpCodes.Leave, endEnumeratorLabel);
			il.BeginFinallyBlock();
			il.Emit(OpCodes.Ldloc, enumeratorLocal);
			il.Emit(OpCodes.Callvirt, _iDisposableDispose);
			il.EndExceptionBlock();
			il.MarkLabel(endEnumeratorLabel);
		}

		internal static void WriteListArray(TypeBuilder typeBuilder, Type type, ILGenerator il)
		{
			var isArray = type.IsArray;
			var isCollectionList = !isArray && !_listType.IsAssignableFrom(type);

			if (isCollectionList)
			{
				WriteICollectionArray(typeBuilder, type, il);
				return;
			}

			var itemType = isArray ? type.GetElementType() : type.GetGenericArguments()[0];
			var isPrimitive = itemType.IsPrimitiveType();
			var itemLocal = il.DeclareLocal(itemType);
			var indexLocal = il.DeclareLocal(_intType);
			var startLabel = il.DefineLabel();
			var endLabel = il.DefineLabel();
			var countLocal = il.DeclareLocal(typeof(int));
			var diffLocal = il.DeclareLocal(typeof(int));
			var checkCountLabel = il.DefineLabel();
			var listLocal = isArray ? default(LocalBuilder) : il.DeclareLocal(type);

			if (listLocal != null)
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Stloc, listLocal);
			}

			if (isArray)
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldlen);
				il.Emit(OpCodes.Conv_I4);
				il.Emit(OpCodes.Stloc, countLocal);
			}
			else
			{
				//il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldloc, listLocal);
				il.Emit(OpCodes.Callvirt, type.GetMethod("get_Count"));
				il.Emit(OpCodes.Stloc, countLocal);
			}

			il.Emit(OpCodes.Ldloc, countLocal);
			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Sub);
			il.Emit(OpCodes.Stloc, diffLocal);

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, indexLocal);
			il.Emit(OpCodes.Br, startLabel);
			il.MarkLabel(endLabel);
			if (isArray)
				il.Emit(OpCodes.Ldarg_0);
			else
				il.Emit(OpCodes.Ldloc, listLocal);

			il.Emit(OpCodes.Ldloc, indexLocal);

			if (isArray)
				il.Emit(OpCodes.Ldelem, itemType);
			else
				il.Emit(OpCodes.Callvirt, type.GetMethod("get_Item"));
			il.Emit(OpCodes.Stloc, itemLocal);

			//il.Emit(OpCodes.Ldarg_0);

			if (itemType == _intType || itemType == _longType)
			{
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldloc, itemLocal);
				il.Emit(OpCodes.Call, itemType == _intType ? _generatorInt32ToStr : _generatorInt64ToStr);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
				il.Emit(OpCodes.Pop);
			}
			else
			{
				il.Emit(OpCodes.Ldloc, itemLocal);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Call, WriteSerializeMethodFor(typeBuilder, itemType));
			}

			il.Emit(OpCodes.Ldloc, indexLocal);
			il.Emit(OpCodes.Ldloc, diffLocal);
			il.Emit(OpCodes.Beq, checkCountLabel);

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldc_I4_S, Delimeter);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppendChar);
			il.Emit(OpCodes.Pop);

			il.MarkLabel(checkCountLabel);

			il.Emit(OpCodes.Ldloc, indexLocal);
			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Add);
			il.Emit(OpCodes.Stloc, indexLocal);
			il.MarkLabel(startLabel);
			il.Emit(OpCodes.Ldloc, indexLocal);
			il.Emit(OpCodes.Ldloc, countLocal);
			il.Emit(OpCodes.Blt, endLabel);
		}

		internal static void WritePropertiesFor(TypeBuilder typeBuilder, Type type, ILGenerator il, bool isPoly = false)
		{
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldc_I4_S, ObjectOpen);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppendChar);
			il.Emit(OpCodes.Pop);

			var skipDefaultValue = il.DeclareLocal(_boolType);
			var camelCasing = il.DeclareLocal(_boolType);
			var hasValue = il.DeclareLocal(_boolType);
			var props = type.GetTypeProperties();
			var count = props.Length - 1;
			var counter = 0;
			var isClass = type.GetTypeInfo().IsClass;

			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, hasValue);

			//Get skip default value setting
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Callvirt, _settingsSkipDefaultValue);
			il.Emit(OpCodes.Stloc, skipDefaultValue);

			//Get camel case setting
			il.Emit(OpCodes.Ldarg_2);
			il.Emit(OpCodes.Callvirt, _settingsCamelCase);
			il.Emit(OpCodes.Stloc, camelCasing);

			if (isPoly)
			{
				il.Emit(OpCodes.Ldarg_1);
				LoadQuotChar(il);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
				il.Emit(OpCodes.Ldstr, TypeIdentifier);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
				LoadQuotChar(il);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
				il.Emit(OpCodes.Ldc_I4, ColonChr);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppendChar);
				LoadQuotChar(il);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppend);

				il.Emit(OpCodes.Ldstr, string.Format("{0}, {1}", type.FullName, type.GetTypeInfo().Assembly.GetName().Name));
				il.Emit(OpCodes.Callvirt, _stringBuilderAppend);

				LoadQuotChar(il);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
				il.Emit(OpCodes.Pop);
				counter = 1;

				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Stloc, hasValue);
			}

			foreach (var mem in props)
			{
				var member = mem.Member;
				var name = member.Name;
				var prop = member.GetMemberType() == MemberTypes.Property ? member as PropertyInfo : null;
				var field = member.GetMemberType() == MemberTypes.Field ? member as FieldInfo : null;
				var attr = mem.Attribute;
				var isProp = prop != null;
				var getMethod = isProp ? prop.GetGetMethod() : null;
				if (!isProp || getMethod != null)
				{
					if (attr != null)
						name = attr.Name ?? name;

					var memberType = isProp ? prop.PropertyType : field.FieldType;
					var propType = memberType;
					var originPropType = memberType;
					var isPrimitive = propType.IsPrimitiveType();
					var nullableType = propType.GetNullableType();
					var isNullable = nullableType != null && !originPropType.IsArray;

					propType = isNullable ? nullableType : propType;
					var isValueType = propType.GetTypeInfo().IsValueType;
					//var propNullLabel = _skipDefaultValue ? il.DefineLabel() : default(Label);
					var equalityMethod = propType.GetMethod("op_Equality");
					var propValue = il.DeclareLocal(propType);
					var isStruct = isValueType && !isPrimitive;
					var nullablePropValue = isNullable ? il.DeclareLocal(originPropType) : null;
					var nameLocal = il.DeclareLocal(_stringType);
					var camelCaseLabel = il.DefineLabel();

					il.Emit(OpCodes.Ldstr, name);
					il.Emit(OpCodes.Stloc, nameLocal);

					il.Emit(OpCodes.Ldloc, camelCasing);
					il.Emit(OpCodes.Brfalse, camelCaseLabel);

					il.Emit(OpCodes.Ldloc, nameLocal);
					il.Emit(OpCodes.Call, _toCamelCase);
					il.Emit(OpCodes.Stloc, nameLocal);

					il.MarkLabel(camelCaseLabel);

					if (isClass)
					{
						il.Emit(OpCodes.Ldarg_0);
						if (isProp)
							il.Emit(OpCodes.Callvirt, getMethod);
						else
							il.Emit(OpCodes.Ldfld, field);
					}
					else
					{
						il.Emit(OpCodes.Ldarga, 0);
						if (isProp)
							il.Emit(OpCodes.Call, getMethod);
						else il.Emit(OpCodes.Ldfld, field);
					}

					if (isNullable)
					{
						il.Emit(OpCodes.Stloc, nullablePropValue);

						il.Emit(OpCodes.Ldloca, nullablePropValue);
						il.Emit(OpCodes.Call, originPropType.GetMethod("GetValueOrDefault", Type.EmptyTypes));

						il.Emit(OpCodes.Stloc, propValue);
					}
					else
					{
						il.Emit(OpCodes.Stloc, propValue);
					}

					var propNullLabel = il.DefineLabel();
					var skipDefaultValueTrueLabel = il.DefineLabel();
					var skipDefaultValueFalseLabel = il.DefineLabel();
					var skipDefaultValueTrueAndHasValueLabel = il.DefineLabel();
					var successLocal = il.DeclareLocal(_boolType);

					var hasValueMethod = isNullable ? originPropType.GetMethod("get_HasValue") : null;

					il.Emit(OpCodes.Ldc_I4, 0);
					il.Emit(OpCodes.Stloc, successLocal);

					il.Emit(OpCodes.Ldloc, skipDefaultValue);
					il.Emit(OpCodes.Brfalse, skipDefaultValueTrueLabel);

					if (isNullable)
					{
						il.Emit(OpCodes.Ldloca, nullablePropValue);
						il.Emit(OpCodes.Call, hasValueMethod);
						il.Emit(OpCodes.Brfalse, propNullLabel);
					}

					if (isStruct)
						il.Emit(OpCodes.Ldloca, propValue);
					else
						il.Emit(OpCodes.Ldloc, propValue);

					if (isValueType && isPrimitive)
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

					WritePropertyForType(typeBuilder, il, hasValue, counter, nameLocal, propType, propValue);

					il.Emit(OpCodes.Ldc_I4, 1);
					il.Emit(OpCodes.Stloc, successLocal);

					il.MarkLabel(propNullLabel);

					il.MarkLabel(skipDefaultValueTrueLabel);


					il.Emit(OpCodes.Ldloc, skipDefaultValue);
					il.Emit(OpCodes.Brtrue, skipDefaultValueFalseLabel);
					il.Emit(OpCodes.Ldloc, successLocal);
					il.Emit(OpCodes.Brtrue, skipDefaultValueFalseLabel);

					WritePropertyForType(typeBuilder, il, hasValue, counter, nameLocal, propType, propValue);

					il.Emit(OpCodes.Ldc_I4, 1);
					il.Emit(OpCodes.Stloc, successLocal);

					il.MarkLabel(skipDefaultValueFalseLabel);

					if (isNullable)
					{
						il.Emit(OpCodes.Ldloc, skipDefaultValue);
						il.Emit(OpCodes.Brfalse, skipDefaultValueTrueAndHasValueLabel);
						il.Emit(OpCodes.Ldloca, nullablePropValue);
						il.Emit(OpCodes.Call, hasValueMethod);
						il.Emit(OpCodes.Brfalse, skipDefaultValueTrueAndHasValueLabel);
						il.Emit(OpCodes.Ldloc, successLocal);
						il.Emit(OpCodes.Brtrue, skipDefaultValueTrueAndHasValueLabel);

						WritePropertyForType(typeBuilder, il, hasValue, counter, nameLocal, propType, propValue);

						il.MarkLabel(skipDefaultValueTrueAndHasValueLabel);
					}

					#region

					//if (_skipDefaultValue) {

					//    if (isNullable) {
					//        var hasValueMethod = originPropType.GetMethod("get_HasValue");
					//        il.Emit(OpCodes.Ldloca, nullablePropValue);
					//        il.Emit(OpCodes.Call, hasValueMethod);
					//        il.Emit(OpCodes.Brfalse, propNullLabel);
					//    }

					//    if (isStruct)
					//        il.Emit(OpCodes.Ldloca, propValue);
					//    else
					//        il.Emit(OpCodes.Ldloc, propValue);
					//    if (isValueType && isPrimitive) {
					//        LoadDefaultValueByType(il, propType);
					//    } else {
					//        if (!isValueType)
					//            il.Emit(OpCodes.Ldnull);
					//    }

					//    if (equalityMethod != null) {
					//        il.Emit(OpCodes.Call, equalityMethod);
					//        il.Emit(OpCodes.Brtrue, propNullLabel);
					//    } else {
					//        if (isStruct) {

					//            var tempValue = il.DeclareLocal(propType);

					//            il.Emit(OpCodes.Ldloca, tempValue);
					//            il.Emit(OpCodes.Initobj, propType);
					//            il.Emit(OpCodes.Ldloc, tempValue);
					//            il.Emit(OpCodes.Box, propType);
					//            il.Emit(OpCodes.Constrained, propType);

					//            il.Emit(OpCodes.Callvirt, _objectEquals);

					//            il.Emit(OpCodes.Brtrue, propNullLabel);

					//        } else
					//            il.Emit(OpCodes.Beq, propNullLabel);
					//    }
					//}

					#endregion

					//if (_skipDefaultValue) {
					//    il.MarkLabel(propNullLabel);
					//}
				}
				counter++;
			}

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldc_I4_S, ObjectClose);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppendChar);
			il.Emit(OpCodes.Pop);
		}

		private static void WritePropertyForType(TypeBuilder typeBuilder, ILGenerator il, LocalBuilder hasValue, int counter, LocalBuilder name, Type propType, LocalBuilder propValue)
		{
			if (counter > 0)
			{
				var hasValueDelimeterLabel = il.DefineLabel();

				il.Emit(OpCodes.Ldloc, hasValue);
				il.Emit(OpCodes.Brfalse, hasValueDelimeterLabel);

				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldc_I4, Delimeter);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppendChar);
				il.Emit(OpCodes.Pop);

				il.MarkLabel(hasValueDelimeterLabel);
			}

			il.Emit(OpCodes.Ldarg_1);
			LoadQuotChar(il);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
			il.Emit(OpCodes.Ldloc, name);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
			LoadQuotChar(il);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
			il.Emit(OpCodes.Ldc_I4, ColonChr);
			il.Emit(OpCodes.Callvirt, _stringBuilderAppendChar);
			il.Emit(OpCodes.Pop);

			if (propType == _intType || propType == _longType)
			{
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldloc, propValue);

				il.Emit(OpCodes.Call, propType == _longType ? _generatorInt64ToStr : _generatorInt32ToStr);
				il.Emit(OpCodes.Callvirt, _stringBuilderAppend);
				il.Emit(OpCodes.Pop);
			}
			else
			{
				il.Emit(OpCodes.Ldloc, propValue);

				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldarg_2);
				il.Emit(OpCodes.Call, WriteSerializeMethodFor(typeBuilder, propType));
			}

			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Stloc, hasValue);
		}

		internal static MethodInfo ReadStringToEnumFor(TypeBuilder typeBuilder, Type type)
		{
			MethodInfo method;
			var key = string.Concat(type.FullName, typeBuilder == null ? Dynamic : string.Empty);
			var typeName = type.GetName().Fix();
			if (_readEnumToStringMethodBuilders.TryGetValue(key, out method))
				return method;
			var methodName = String.Concat(ReadEnumStr, typeName);
			method = typeBuilder.DefineMethodEx(methodName, StaticMethodAttribute,
				type, new[] { _stringType });
			_readEnumToStringMethodBuilders[key] = method;

			var eType = type.GetEnumUnderlyingType();
			var il = method.GetILGenerator();

			var values = Enum.GetValues(type).Cast<object>()
				.Select(x => new {
					Value = x,
					Attr = type.GetMember(x.ToString()).FirstOrDefault()
				})
				.Select(x => new {
					Value = x.Value,
					Attr = x.Attr != null 
						? (x.Attr.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault() as JsonPropertyAttribute) 
						: null
				}).ToArray();

			var keys = Enum.GetNames(type);

			for (var i = 0; i < values.Length; i++)
			{
				var valueInfo = values[i];
				var value = valueInfo.Value;
				var attr = valueInfo.Attr;
				var k = keys[i];

				if (attr != null)
					k = attr.Name;

				var label = il.DefineLabel();
				var label2 = il.DefineLabel();

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldstr, k);
				il.Emit(OpCodes.Call, _stringOpEquality);
				il.Emit(OpCodes.Brfalse, label);

				if (eType == _intType)
				{
					il.Emit(OpCodes.Ldc_I4, (int)value);
				}
				else if (eType == _longType)
				{
					il.Emit(OpCodes.Ldc_I8, (long)value);
				}
				else if (eType == typeof(ulong))
				{
					il.Emit(OpCodes.Ldc_I8, (long)((ulong)value));
				}
				else if (eType == typeof(uint))
				{
					il.Emit(OpCodes.Ldc_I4, (uint)value);
				}
				else if (eType == typeof(byte))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((byte)value));
					il.Emit(OpCodes.Conv_U1);
				}
				else if (eType == typeof(ushort))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((ushort)value));
					il.Emit(OpCodes.Conv_U2);
				}
				else if (eType == typeof(short))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((short)value));
					il.Emit(OpCodes.Conv_I2);
				}

				il.Emit(OpCodes.Ret);

				il.MarkLabel(label);

				il.Emit(OpCodes.Ldarg_0);

				if (eType == _intType)
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.Int32ToStr((int)value));
				else if (eType == _longType)
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.Int64ToStr((long)value));
				else if (eType == typeof(ulong))
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.UInt64ToStr((ulong)value));
				else if (eType == typeof(uint))
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.UInt32ToStr((uint)value));
				else if (eType == typeof(byte))
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.Int32ToStr((int)((byte)value)));
				else if (eType == typeof(ushort))
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.Int32ToStr((int)((ushort)value)));
				else if (eType == typeof(short))
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.Int32ToStr((int)((short)value)));

				il.Emit(OpCodes.Call, _stringOpEquality);
				il.Emit(OpCodes.Brfalse, label2);

				if (eType == _intType)
				{
					il.Emit(OpCodes.Ldc_I4, (int)value);
				}
				else if (eType == _longType)
				{
					il.Emit(OpCodes.Ldc_I8, (long)value);
				}
				else if (eType == typeof(ulong))
				{
					il.Emit(OpCodes.Ldc_I8, (long)((ulong)value));
				}
				else if (eType == typeof(uint))
				{
					il.Emit(OpCodes.Ldc_I4, (uint)value);
				}
				else if (eType == typeof(byte))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((byte)value));
					il.Emit(OpCodes.Conv_U1);
				}
				else if (eType == typeof(ushort))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((ushort)value));
					il.Emit(OpCodes.Conv_U2);
				}
				else if (eType == typeof(short))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((short)value));
					il.Emit(OpCodes.Conv_I2);
				}

				il.Emit(OpCodes.Ret);

				il.MarkLabel(label2);
			}

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Call, _flagStrToEnum.MakeGenericMethod(type));
			il.Emit(OpCodes.Ret);

			return method;
		}

		internal static MethodInfo WriteEnumToStringFor(TypeBuilder typeBuilder, Type type)
		{
			MethodInfo method;
			var key = String.Concat(type.FullName, typeBuilder == null ? Dynamic : string.Empty);
			var typeName = type.GetName().Fix();
			if (_writeEnumToStringMethodBuilders.TryGetValue(key, out method))
				return method;
			var methodName = String.Concat(WriteStr, typeName);
			method = typeBuilder.DefineMethodEx(methodName, StaticMethodAttribute,
				_stringType, new[] { type, _settingsType });
			_writeEnumToStringMethodBuilders[key] = method;

			var eType = type.GetEnumUnderlyingType();

			var il = method.GetILGenerator();
			var useEnumLabel = il.DefineLabel();

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Callvirt, _settingsUseEnumStringProp);
			il.Emit(OpCodes.Brfalse, useEnumLabel);

			WriteEnumToStringForWithString(type, eType, il);

			il.MarkLabel(useEnumLabel);

			WriteEnumToStringForWithInt(type, eType, il);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Box, type);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Call, _flagEnumToStr);

			il.Emit(OpCodes.Ret);

			return method;
		}

		private static void WriteEnumToStringForWithInt(Type type, Type eType, ILGenerator il)
		{
			var values = Enum.GetValues(type).Cast<object>().ToArray();

			var count = values.Length;

			for (var i = 0; i < count; i++)
			{
				var value = values[i];

				var label = il.DefineLabel();

				il.Emit(OpCodes.Ldarg_0);

				if (eType == _intType)
				{
					il.Emit(OpCodes.Ldc_I4, (int)value);
				}
				else if (eType == _longType)
				{
					il.Emit(OpCodes.Ldc_I8, (long)value);
				}
				else if (eType == typeof(ulong))
				{
					il.Emit(OpCodes.Ldc_I8, (long)((ulong)value));
				}
				else if (eType == typeof(uint))
				{
					il.Emit(OpCodes.Ldc_I4, (uint)value);
				}
				else if (eType == typeof(byte))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((byte)value));
					il.Emit(OpCodes.Conv_U1);
				}
				else if (eType == typeof(ushort))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((ushort)value));
					il.Emit(OpCodes.Conv_U2);
				}
				else if (eType == typeof(short))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((short)value));
					il.Emit(OpCodes.Conv_I2);
				}

				il.Emit(OpCodes.Bne_Un, label);

				if (eType == _intType)
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.Int32ToStr((int)value));
				else if (eType == _longType)
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.Int64ToStr((long)value));
				else if (eType == typeof(ulong))
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.UInt64ToStr((ulong)value));
				else if (eType == typeof(uint))
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.UInt32ToStr((uint)value));
				else if (eType == typeof(byte))
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.Int32ToStr((int)((byte)value)));
				else if (eType == typeof(ushort))
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.Int32ToStr((int)((ushort)value)));
				else if (eType == typeof(short))
					il.Emit(OpCodes.Ldstr, JsonSerializingEngine.Int32ToStr((int)((short)value)));

				il.Emit(OpCodes.Ret);

				il.MarkLabel(label);
			}
		}

		private static void WriteEnumToStringForWithString(Type type, Type eType, ILGenerator il)
		{
			var values = Enum.GetValues(type).Cast<object>()
				.Select(x => new {
					Value = x,
					Attr = type.GetMember(x.ToString()).FirstOrDefault()
				})
				.Select(x => new {
					Value = x.Value,
					Attr = x.Attr != null 
						? (x.Attr.GetCustomAttributes(typeof(JsonPropertyAttribute), true).FirstOrDefault() as JsonPropertyAttribute) 
						: null
				}).ToArray();

			var names = Enum.GetNames(type);

			var count = values.Length;

			for (var i = 0; i < count; i++)
			{
				var valueInfo = values[i];
				var attr = valueInfo.Attr;
				var value = valueInfo.Value;

				var name = names[i];
				var label = il.DefineLabel();

				il.Emit(OpCodes.Ldarg_0);

				if (eType == _intType)
				{
					il.Emit(OpCodes.Ldc_I4, (int)value);
				}
				else if (eType == _longType)
				{
					il.Emit(OpCodes.Ldc_I8, (long)value);
				}
				else if (eType == typeof(ulong))
				{
					il.Emit(OpCodes.Ldc_I8, (long)((ulong)value));
				}
				else if (eType == typeof(uint))
				{
					il.Emit(OpCodes.Ldc_I4, (uint)value);
				}
				else if (eType == typeof(byte))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((byte)value));
					il.Emit(OpCodes.Conv_U1);
				}
				else if (eType == typeof(ushort))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((ushort)value));
					il.Emit(OpCodes.Conv_U2);
				}
				else if (eType == typeof(short))
				{
					il.Emit(OpCodes.Ldc_I4, (int)((short)value));
					il.Emit(OpCodes.Conv_I2);
				}

				il.Emit(OpCodes.Bne_Un, label);

				if (attr != null)
					name = attr.Name;

				il.Emit(OpCodes.Ldstr, name);
				il.Emit(OpCodes.Ret);

				il.MarkLabel(label);
			}
		}
	}
}
