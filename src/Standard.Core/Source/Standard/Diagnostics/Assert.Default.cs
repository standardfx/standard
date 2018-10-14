using System;
using System.Diagnostics;
using Standard.Core;

namespace Standard.Diagnostics
{
    public static partial class Assert
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> exception if the object instance is assigned to its default value.
        /// </summary>
        /// <typeparam name="T">The type of object to evaluate</typeparam>
        /// <param name="value">The object to evaluate.</param>
		[DebuggerStepThrough]
        public static void NotDefault<T>(T value) where T : struct
            => NotDefault(value, null, null);

        /// <summary>
        /// Throws an exception if the object instance is assigned to its default value.
        /// </summary>
        /// <typeparam name="T">The type of object to evaluate</typeparam>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        [DebuggerStepThrough]
        public static void NotDefault<T>(T value, string paramName) where T : struct
            => NotDefault(value, paramName, null);

        /// <summary>
        /// Throws an exception if the object instance is assigned to its default value.
        /// </summary>
        /// <typeparam name="T">The type of object to evaluate</typeparam>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        /// <param name="message">When throwing an <see cref="ArgumentException"/>, set the argument message to this value.</param>
        /// <param name="args">Format the <paramref name="message"/> string with the values set by this parameter.</param>
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
