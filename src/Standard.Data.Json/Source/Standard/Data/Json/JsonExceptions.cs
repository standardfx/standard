using System;

namespace Standard.Data.Json
{
	/// <summary>
	/// The exception that is thrown when an invalid JSON syntax is encountered.
	/// </summary>
	public sealed class InvalidJsonException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidJsonException"/> class.
		/// </summary>
		public InvalidJsonException()
			: base("Input is not a valid JSON.")
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidJsonException"/> class.
		/// </summary>
		public InvalidJsonException(int index)
			: this()
		{ 
			this.Index = index;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidJsonException"/> class.
		/// </summary>
		public InvalidJsonException(string message, int index)
			: base(message)
		{ 
			this.Index = index;
		}

		/// <summary>
		/// The index position where invalid JSON syntax was detected. 
		/// </summary>
		public int Index { get; private set; } 
	}

	/// <summary>
	/// The exception that is thrown when an invalid JSON property name is encountered.
	/// </summary>
	public sealed class InvalidJsonPropertyException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidJsonPropertyException"/> class.
		/// </summary>
		public InvalidJsonPropertyException()
			: base("Class cannot contain any JSON with null or blank space character.")
		{ }
	}

	/// <summary>
	/// The exception that is thrown when an invalid assembly is generated. This usually happens when adding all assembly into a specified assembly file.
	/// </summary>
	public sealed class JsonAssemblyGeneratorException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonAssemblyGeneratorException"/> class.
		/// </summary>
		/// <param name="asmName"></param>
		public JsonAssemblyGeneratorException(string asmName)
			: base(string.Format("Could not generate assembly with name [{0}] due to empty list of types to include", asmName))
		{ }
	}
}
