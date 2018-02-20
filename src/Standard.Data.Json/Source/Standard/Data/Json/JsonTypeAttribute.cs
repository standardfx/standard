using System;

namespace Standard.Data.Json
{
	/// <summary>
	/// Attribute for configuring class objects that requires type information for serialization and deserialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class JsonTypeAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonTypeAttribute"/> class.
		/// </summary>
		/// <param name="type"></param>
		public JsonTypeAttribute(Type type)
		{
			Type = type;
		}

		/// <summary>
		/// The class object type.
		/// </summary>
		public Type Type { private set; get; }
	}
}
