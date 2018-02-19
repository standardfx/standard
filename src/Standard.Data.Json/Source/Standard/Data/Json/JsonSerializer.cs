using System;
using System.IO;

namespace Standard.Data.Json
{
	public abstract class JsonSerializer<T>
	{
		public abstract string Serialize(T value);
		public abstract T Deserialize(string value);

		public abstract void Serialize(T value, TextWriter writer);
		public abstract T Deserialize(TextReader reader);

		// With settings
		public abstract string Serialize(T value, JsonSerializerSettings settings);
		public abstract T Deserialize(string value, JsonSerializerSettings settings);
		public abstract void Serialize(T value, TextWriter writer, JsonSerializerSettings settings);
		public abstract T Deserialize(TextReader reader, JsonSerializerSettings settings);
	}
}
