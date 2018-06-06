using System;

namespace Standard
{
    partial class FastConvert
    {
    	/// <summary>
    	/// Converts a string to a <see cref="Type"/> instance.
    	/// </summary>
    	/// <param name="typeFullName">Full qualifying name of the type.</param>
        public static Type ToType(string typeFullName)
        {
            return Type.GetType(typeFullName, false);
        }
    }
}
