using System;
using System.Diagnostics;
using Standard.Diagnostics.Core;

namespace Standard.Diagnostics
{
    public static partial class Assert
    {
		[DebuggerStepThrough]
        public static void NotDefault<T>(T value) where T : struct
            => NotDefault(value, null, null);

        [DebuggerStepThrough]
        public static void NotDefault<T>(T value, string paramName) where T : struct
            => NotDefault(value, paramName, null);

        [DebuggerStepThrough]
        public static void NotDefault<T>(T value, string paramName, string message, params object[] args) where T : struct
        {
            if (!default(T).Equals(value))
                return;

            if (message == null)
                throw new ArgumentException(RS.Err_IsDefaultValue, paramName);
            else if (args != null)
                throw new ArgumentException(string.Format(message, args), paramName);
            else
                throw new ArgumentException(message, paramName);
        }
    }
}
