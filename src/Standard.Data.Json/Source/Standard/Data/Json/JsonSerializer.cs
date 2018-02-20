using System;
using System.IO;

namespace Standard.Data.Json
{
	/// <summary>
	/// Base class for JSON serialization.
	/// </summary>
	public abstract class JsonSerializer<T>
	{
		/// <summary>
		/// Converts an object of type <c>T</c> into JSON string.
		/// </summary>
		public abstract string Serialize(T value);

		/// <summary>
		/// Converts a JSON string into an object of type <c>T</c>.
		/// </summary>
		public abstract T Deserialize(string value);

		/// <summary>
		/// Converts an object of type <c>T</c> into JSON, and output the result to a <c>TextWriter</c>.
		/// </summary>
		public abstract void Serialize(T value, TextWriter writer);

		/// <summary>
		/// Converts a JSON string inside a <c>TextReader</c> object into an object of type <c>T</c>.
		/// </summary>
		public abstract T Deserialize(TextReader reader);

		// With settings

		/// <summary>
		/// Converts an object of type <c>T</c> into JSON string, using the specified settings.
		/// </summary>
		public abstract string Serialize(T value, JsonSerializerSettings settings);

		/// <summary>
		/// Converts a JSON string into an object of type <c>T</c>, using the specified settings.
		/// </summary>
		public abstract T Deserialize(string value, JsonSerializerSettings settings);

		/// <summary>
		/// Converts an object of type <c>T</c> into JSON, and output the result to a <c>TextWriter</c>, using the specified settings.
		/// </summary>
		public abstract void Serialize(T value, TextWriter writer, JsonSerializerSettings settings);

		/// <summary>
		/// Converts a JSON string inside a <c>TextReader</c> object into an object of type <c>T</c>, using the specifed settings.
		/// </summary>
		public abstract T Deserialize(TextReader reader, JsonSerializerSettings settings);
	}
}
