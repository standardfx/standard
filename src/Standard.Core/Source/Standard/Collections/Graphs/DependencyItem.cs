using System;
using System.Collections.Generic;

namespace Standard.Collections.Graphs
{
	/// <summary>
	/// This class is designed to represent an object with a list of dependencies.
	/// </summary>
	public class DependencyItem<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyItem{T}" /> class.
		/// </summary>
		/// <param name="name">The name of the object for referencing purposes.</param>
		/// <param name="dependencies">A list of objects that this object depends on.</param>
	 	public DependencyItem(T name, params T[] dependencies)
	 	{
	 		Name = name;
	 		Dependencies = dependencies;
	 	}

		/// <summary>
		/// The name of the object for referencing purposes.
		/// </summary>
		public T Name 
		{ 
			get; 
			private set; 
		}

		/// <summary>
		/// A list of objects that is object depends on.
		/// </summary>
	 	public IEnumerable<T> Dependencies 
	 	{ 
	 		get; 
	 		private set; 
	 	}

	 	/// <summary>
	 	/// Returns a string representation of this object.
	 	/// </summary>
		public override string ToString()
	 	{
	 		return Name.ToString();
	 	}
	}	
}