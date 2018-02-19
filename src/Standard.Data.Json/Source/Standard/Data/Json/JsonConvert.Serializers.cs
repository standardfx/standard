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
		private sealed class DynamicJsonSerializer<T> : JsonSerializer<T>
		{
			private readonly Func<TextReader, T> _DeserializeTextReader;
			private readonly Func<string, T> _Deserialize;
			private readonly Func<TextReader, JsonSerializerSettings, T> _DeserializeTextReaderWithSettings;
			private readonly Func<string, JsonSerializerSettings, T> _DeserializeWithSettings;
			private readonly Func<T, string> _Serialize;
			private readonly Func<T, JsonSerializerSettings, string> _SerializeWithSettings;
			private readonly Action<T, TextWriter> _SerializeTextWriter;
			private readonly Action<T, TextWriter, JsonSerializerSettings> _SerializeTextWriterWithSettings;

			private Type _objType;

			public DynamicJsonSerializer()
			{
				_DeserializeTextReader = CreateDeserializerWithTextReader();
				_Deserialize = CreateDeserializer();
				_DeserializeTextReaderWithSettings = CreateDeserializerWithTextReaderSettings();
				_DeserializeWithSettings = CreateDeserializerWithSettings();
				_Serialize = CreateSerializer();
				_SerializeTextWriter = CreateSerializerWithTextWriter();
				_SerializeTextWriterWithSettings = CreateSerializerWithTextWriterSettings();
				_SerializeWithSettings = CreateSerializerWithSettings();
			}

			public Type ObjType
			{
				get
				{
					return _objType ?? (_objType = typeof(T));
				}
			}

			public bool IsPrimitive
			{
				get
				{
					return ObjType.IsPrimitiveType();
				}
			}

			private Module ManifestModule
			{
				get
				{
#if NETSTANDARD
					return ObjType.GetTypeInfo().Assembly.ManifestModule;
#else
					return Assembly.GetExecutingAssembly().ManifestModule;
#endif
				}
			}

			private Func<TextReader, T> CreateDeserializerWithTextReader()
			{
				var meth = new DynamicMethod("DeserializeValueTextReader", ObjType, new[] { _textReaderType }, ManifestModule, true);

				var rdil = meth.GetILGenerator();

				var readMethod = WriteDeserializeMethodFor(null, ObjType);

				rdil.Emit(OpCodes.Ldarg_0);
				rdil.Emit(OpCodes.Callvirt, _textReaderReadToEnd);
				rdil.Emit(OpCodes.Call, _settingsCurrentSettings);
				rdil.Emit(OpCodes.Call, readMethod);
				rdil.Emit(OpCodes.Ret);

				return meth.CreateDelegate(typeof(Func<TextReader, T>)) as Func<TextReader, T>;
			}

			private Func<string, T> CreateDeserializer()
			{
				var meth = new DynamicMethod("DeserializeValue", ObjType, new[] { _stringType }, ManifestModule, true);

				var dil = meth.GetILGenerator();

				var readMethod = WriteDeserializeMethodFor(null, ObjType);

				dil.Emit(OpCodes.Ldarg_0);
				dil.Emit(OpCodes.Call, _settingsCurrentSettings);
				dil.Emit(OpCodes.Call, readMethod);
				dil.Emit(OpCodes.Ret);

				return meth.CreateDelegate(typeof(Func<string, T>)) as Func<string, T>;
			}

			private Func<TextReader, JsonSerializerSettings, T> CreateDeserializerWithTextReaderSettings()
			{
				var meth = new DynamicMethod("DeserializeValueTextReaderSettings", ObjType, new[] { _textReaderType, _settingsType }, ManifestModule, true);

				var rdilWithSettings = meth.GetILGenerator();

				var readMethod = WriteDeserializeMethodFor(null, ObjType);

				rdilWithSettings.Emit(OpCodes.Ldarg_1);
				rdilWithSettings.Emit(OpCodes.Callvirt, _textReaderReadToEnd);
				rdilWithSettings.Emit(OpCodes.Ldarg_2);
				rdilWithSettings.Emit(OpCodes.Call, readMethod);
				rdilWithSettings.Emit(OpCodes.Ret);

				return meth.CreateDelegate(typeof(Func<TextReader, JsonSerializerSettings, T>)) as Func<TextReader, JsonSerializerSettings, T>;
			}

			private Func<string, JsonSerializerSettings, T> CreateDeserializerWithSettings()
			{
				var meth = new DynamicMethod("DeserializeValueSettings", ObjType, new[] { _stringType, _settingsType }, ManifestModule, true);

				var dilWithSettings = meth.GetILGenerator();

				var readMethod = WriteDeserializeMethodFor(null, ObjType);

				dilWithSettings.Emit(OpCodes.Ldarg_0);
				dilWithSettings.Emit(OpCodes.Ldarg_1);
				dilWithSettings.Emit(OpCodes.Call, readMethod);
				dilWithSettings.Emit(OpCodes.Ret);

				return meth.CreateDelegate(typeof(Func<string, JsonSerializerSettings, T>)) as Func<string, JsonSerializerSettings, T>;
			}

			private Func<T, string> CreateSerializer()
			{
				var meth = new DynamicMethod("SerializeValue", _stringType, new[] { ObjType }, ManifestModule, true);

				var il = meth.GetILGenerator();

				var writeMethod = WriteSerializeMethodFor(null, ObjType, needQuote: !IsPrimitive || ObjType == _stringType);

				var sbLocal = il.DeclareLocal(_stringBuilderType);
				il.Emit(OpCodes.Call, _generatorGetStringBuilder);

				il.EmitClearStringBuilder();

				il.Emit(OpCodes.Stloc, sbLocal);

				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldloc, sbLocal);
				il.Emit(OpCodes.Call, _settingsCurrentSettings);
				il.Emit(OpCodes.Call, writeMethod);

				il.Emit(OpCodes.Ldloc, sbLocal);
				il.Emit(OpCodes.Callvirt, _stringBuilderToString);
				il.Emit(OpCodes.Ret);

				return meth.CreateDelegate(typeof(Func<T, string>)) as Func<T, string>;
			}

			private Func<T, JsonSerializerSettings, string> CreateSerializerWithSettings()
			{
				DynamicMethod meth = new DynamicMethod("SerializeValueSettings", _stringType, new[] { ObjType, _settingsType }, ManifestModule, true);

				ILGenerator ilWithSettings = meth.GetILGenerator();

				MethodInfo writeMethod = WriteSerializeMethodFor(null, ObjType, needQuote: !IsPrimitive || ObjType == _stringType);

				bool isValueType = ObjType.GetTypeInfo().IsValueType;

				LocalBuilder sbLocalWithSettings = ilWithSettings.DeclareLocal(_stringBuilderType);
				ilWithSettings.Emit(OpCodes.Call, _generatorGetStringBuilder);

				ilWithSettings.EmitClearStringBuilder();

				ilWithSettings.Emit(OpCodes.Stloc, sbLocalWithSettings);

				ilWithSettings.Emit(OpCodes.Ldarg_0);
				ilWithSettings.Emit(OpCodes.Ldloc, sbLocalWithSettings);
				ilWithSettings.Emit(OpCodes.Ldarg_1);
				ilWithSettings.Emit(OpCodes.Call, writeMethod);

				ilWithSettings.Emit(OpCodes.Ldloc, sbLocalWithSettings);
				ilWithSettings.Emit(OpCodes.Callvirt, _stringBuilderToString);

				ilWithSettings.Emit(OpCodes.Ldarg_1);

				ilWithSettings.Emit(OpCodes.Call, _prettifyJsonIfNeeded);

				ilWithSettings.Emit(OpCodes.Ret);

				return meth.CreateDelegate(typeof(Func<T, JsonSerializerSettings, string>)) as Func<T, JsonSerializerSettings, string>;
			}

			private Action<T, TextWriter> CreateSerializerWithTextWriter()
			{
				DynamicMethod meth = new DynamicMethod("SerializeValueTextWriter", _voidType, new[] { typeof(T), _textWriterType }, ManifestModule, true);

				ILGenerator wil = meth.GetILGenerator();

				MethodInfo writeMethod = WriteSerializeMethodFor(null, ObjType, needQuote: !IsPrimitive || ObjType == _stringType);

				LocalBuilder wsbLocal = wil.DeclareLocal(_stringBuilderType);
				wil.Emit(OpCodes.Call, _generatorGetStringBuilder);
				wil.EmitClearStringBuilder();
				wil.Emit(OpCodes.Stloc, wsbLocal);

				wil.Emit(OpCodes.Ldarg_0);
				wil.Emit(OpCodes.Ldloc, wsbLocal);
				wil.Emit(OpCodes.Call, _settingsCurrentSettings);
				wil.Emit(OpCodes.Call, writeMethod);

				wil.Emit(OpCodes.Ldarg_1);
				wil.Emit(OpCodes.Ldloc, wsbLocal);
				wil.Emit(OpCodes.Callvirt, _stringBuilderToString);
				wil.Emit(OpCodes.Callvirt, _textWriterWrite);
				wil.Emit(OpCodes.Ret);

				return meth.CreateDelegate(typeof(Action<T, TextWriter>)) as Action<T, TextWriter>;
			}

			private Action<T, TextWriter, JsonSerializerSettings> CreateSerializerWithTextWriterSettings()
			{
				DynamicMethod meth = new DynamicMethod("SerializeValueTextWriterSettings", _voidType, new[] {
					ObjType,
					_textWriterType,
					_settingsType
				}, ManifestModule, true);

				ILGenerator wilWithSettings = meth.GetILGenerator();

				MethodInfo writeMethod = WriteSerializeMethodFor(null, ObjType, needQuote: !IsPrimitive || ObjType == _stringType);

				LocalBuilder wsbLocalWithSettings = wilWithSettings.DeclareLocal(_stringBuilderType);
				wilWithSettings.Emit(OpCodes.Call, _generatorGetStringBuilder);
				wilWithSettings.EmitClearStringBuilder();
				wilWithSettings.Emit(OpCodes.Stloc, wsbLocalWithSettings);

				wilWithSettings.Emit(OpCodes.Ldarg_0);
				wilWithSettings.Emit(OpCodes.Ldloc, wsbLocalWithSettings);
				wilWithSettings.Emit(OpCodes.Ldarg_2);
				wilWithSettings.Emit(OpCodes.Call, writeMethod);

				wilWithSettings.Emit(OpCodes.Ldarg_1);
				wilWithSettings.Emit(OpCodes.Ldloc, wsbLocalWithSettings);
				wilWithSettings.Emit(OpCodes.Callvirt, _stringBuilderToString);

				wilWithSettings.Emit(OpCodes.Ldarg_2);

				wilWithSettings.Emit(OpCodes.Call, _prettifyJsonIfNeeded);

				wilWithSettings.Emit(OpCodes.Callvirt, _textWriterWrite);
				wilWithSettings.Emit(OpCodes.Ret);

				return meth.CreateDelegate(typeof(Action<T, TextWriter, JsonSerializerSettings>)) as Action<T, TextWriter, JsonSerializerSettings>;
			}

			public override T Deserialize(TextReader reader)
			{
				return _DeserializeTextReader(reader);
			}

			public override T Deserialize(string value)
			{
				return _Deserialize(value);
			}

			public override T Deserialize(TextReader reader, JsonSerializerSettings settings)
			{
				return _DeserializeTextReaderWithSettings(reader, settings);
			}

			public override T Deserialize(string value, JsonSerializerSettings settings)
			{
				return _DeserializeWithSettings(value, settings);
			}

			public override string Serialize(T value)
			{
				return _Serialize(value);
			}

			public override string Serialize(T value, JsonSerializerSettings settings)
			{
				return _SerializeWithSettings(value, settings);
			}

			public override void Serialize(T value, TextWriter writer)
			{
				_SerializeTextWriter(value, writer);
			}

			public override void Serialize(T value, TextWriter writer, JsonSerializerSettings settings)
			{
				_SerializeTextWriterWithSettings(value, writer, settings);
			}
		}

		private static class CachedJsonSerializer<T>
		{
			public static readonly JsonSerializer<T> Serializer = GetSerializer();

			private static JsonSerializer<T> GetSerializer()
			{
				JsonSerializer<T> serializer = null;
				Type type = typeof(T);
				if (type.GetTypeInfo().IsGenericType)
				{
					foreach (Type item in type.GetGenericArguments())
					{
						if (IsPrivate(item))
						{
							type = item;
							break;
						}
					}
				}

				if (IsPrivate(type))
					serializer = new DynamicJsonSerializer<T>();
				else
					serializer = (JsonSerializer<T>)Activator.CreateInstance(Generate(typeof(T)));

				return serializer;
			}

			private static bool IsPrivate(Type type)
			{
				return type.GetTypeInfo().IsNotPublic ||
					type.GetTypeInfo().IsNestedPrivate ||
					!type.GetTypeInfo().IsVisible;
			}
		}
	}
}
