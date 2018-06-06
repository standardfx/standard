using System;
using System.Diagnostics;
using System.Reflection;
using Standard.Core;

namespace Standard.Diagnostics
{
    public static partial class Assert
    {
        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if <paramref key="val"/> is <c>null</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static void NotNull<T>(T value) where T : class
            => NotNull(value, null, null);

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if <paramref key="val"/> is <c>null</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static void NotNull<T>(T value, string paramName) where T : class
            => NotNull(value, paramName, null);

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if <paramref key="val"/> is <c>null</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static void NotNull<T>(T value, string paramName, string message, params object[] args) where T : class
        {
            if (!ReferenceEquals(value, null))
                return;

            if (message == null)
                throw new ArgumentNullException(paramName);
            else if (args != null)
                throw new ArgumentNullException(paramName, string.Format(message, args));
            else
                throw new ArgumentNullException(paramName, message);
        }

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if <paramref key="value"/> is <c>null</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static void NotNull<T>(T? value) where T : struct
            => NotNull(value, null, null);

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if <paramref key="value"/> is <c>null</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static void NotNull<T>(T? value, string paramName) where T : struct
            => NotNull(value, paramName, null);

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if <paramref key="value"/> is <c>null</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static void NotNull<T>(T? value, string paramName, string message, params object[] args) where T : struct
        {
            if (value != null)
                return;

            if (message == null)
                throw new ArgumentNullException(paramName);
            else if (args != null)
                throw new ArgumentNullException(paramName, string.Format(message, args));
            else
                throw new ArgumentNullException(paramName, message);
        }

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if an object of type <paramref key="valType"/> is not
        /// assignable to type <typeparam key="T" />.
        /// </summary>
        [DebuggerStepThrough]
        public static void IsAssignableFrom<T>(Type valType)
            => IsAssignableFrom<T>(valType, null, null);

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if an object of type <paramref key="valType"/> is not
        /// assignable to type <typeparam key="T" />.
        /// </summary>
        [DebuggerStepThrough]
        public static void IsAssignableFrom<T>(Type valType, string paramName)
            => IsAssignableFrom<T>(valType, paramName, null);

        /// <summary>
        /// Throws a <see cref="ArgumentOutOfRangeException"/> if an object of type <paramref key="valType"/> is not
        /// assignable to type <typeparam key="T" />.
        /// </summary>
        [DebuggerStepThrough]
        public static void IsAssignableFrom<T>(Type valType, string paramName, string message, params object[] args)
        {
#if NETSTANDARD
            if (typeof(T).GetTypeInfo().IsAssignableFrom(valType.GetTypeInfo()))
                return;

            if (message == null && paramName == null)
                message = string.Format(RS.Err_BadTypeAssignment, valType.GetTypeInfo().AssemblyQualifiedName, typeof(T).GetTypeInfo().AssemblyQualifiedName);
            else if (message == null && paramName != null)
                message = string.Format(RS.Err_BadTypeAssignmentParams, valType.GetTypeInfo().AssemblyQualifiedName, paramName, typeof(T).GetTypeInfo().AssemblyQualifiedName);
            else if (args != null)
                message = string.Format(message, args).Replace("{#paramName}", paramName);;

#else
			if (typeof(T).IsAssignableFrom(valType))
                return;

            if (message == null && paramName == null)
                message = string.Format(RS.Err_BadTypeAssignment, valType.AssemblyQualifiedName, typeof(T).AssemblyQualifiedName);
            else if (message == null && paramName != null)
                message = string.Format(RS.Err_BadTypeAssignmentParams, valType.AssemblyQualifiedName, paramName, typeof(T).AssemblyQualifiedName);
            else if (args != null)
                message = string.Format(message, args).Replace("{#paramName}", paramName);
#endif

            throw new InvalidCastException(message);
        }
    }
}