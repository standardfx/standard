using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Standard.Collections.Graphs
{
	public class DependencyItem<T>
	{
		public T Name 
		{ 
			get; 
			private set; 
		}

	 	public IEnumerable<T> Dependencies 
	 	{ 
	 		get; 
	 		private set; 
	 	}
	 
	 	public DependencyItem(T name, params T[] dependencies)
	 	{
	 		Name = name;
	 		Dependencies = dependencies;
	 	}
	 
		public override string ToString()
	 	{
	 		return Name.ToString();
	 	}
	}	
}