using System;

namespace Standard.Data.Json
{
	/// <summary>
	/// Attribute for renaming field and property names when serializing and deserializing.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum)]
	public sealed class JsonPropertyAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonPropertyAttribute"/> class.
		/// </summary>
		/// <param name="name">The name to use for serialization.</param>
		public JsonPropertyAttribute(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Name of the property or field to use for serialization.
		/// </summary>
		public string Name { get; private set; }
	}
}
