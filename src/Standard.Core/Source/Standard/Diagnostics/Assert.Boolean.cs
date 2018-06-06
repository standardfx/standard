using System;
using System.Diagnostics;
using Standard.Core;

namespace Standard.Diagnostics
{
    public static partial class Assert
    {
		/// <summary>
		/// Verifies that the condition is <c>true</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>false</c>.</exception>
		[DebuggerStepThrough]
		public static void True(bool condition)
			=> True((bool?)condition, null, null, null);

		/// <summary>
		/// Verifies that the condition is <c>true</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>false</c>.</exception>
		[DebuggerStepThrough]
		public static void True(bool? condition)
			=> True(condition, null, null, null);

		/// <summary>
		/// Verifies that the condition is <c>true</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <param name="paramName">The parameter name being tested.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>false</c>.</exception>
		[DebuggerStepThrough]
        public static void True(bool condition, string paramName)
			=> True((bool?)condition, paramName, RS.Err_BooleanNotTrueParams, null);

		/// <summary>
		/// Verifies that the condition is <c>true</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <param name="paramName">The parameter name being tested.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>false</c>.</exception>
		[DebuggerStepThrough]
		public static void True(bool? condition, string paramName)
			=> True(condition, paramName, RS.Err_BooleanNotTrueParams, null);

		/// <summary>
		/// Verifies that the condition is <c>true</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <param name="paramName">The parameter name being tested.</param>
		/// <param name="message">The message to be shown if an exception occurs.</param>
		/// <param name="args">Format arguments for the <paramref name="message"/> argument.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>false</c>.</exception>
		[DebuggerStepThrough]
		public static void True(bool condition, string paramName, string message, params object[] args)
			=> True((bool?)condition, paramName, message, args);

		/// <summary>
		/// Verifies that the condition is <c>true</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <param name="paramName">The parameter name being tested.</param>
		/// <param name="message">The message to be shown if an exception occurs.</param>
		/// <param name="args">Format arguments for the <paramref name="message"/> argument.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>false</c>.</exception>
		[DebuggerStepThrough]
        public static void True(bool? condition, string paramName, string message, params object[] args)
        {
            if (!condition.HasValue || !condition.GetValueOrDefault())
            {
                if (message == null)
                    message = RS.Err_BooleanNotTrue;
                else if (args != null)
                    message = string.Format(message, args);

                throw new ArgumentException(message, paramName);
            }
        }

		// False

		/// <summary>
		/// Verifies that the condition is <c>false</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>true</c>.</exception>
		[DebuggerStepThrough]
        public static void False(bool condition)
			=> False((bool?)condition, null, null, null);

		/// <summary>
		/// Verifies that the condition is <c>false</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>true</c>.</exception>
		[DebuggerStepThrough]
		public static void False(bool? condition)
			=> False(condition, null, null, null);

		/// <summary>
		/// Verifies that the condition is <c>false</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <param name="paramName">The parameter name being tested.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>true</c>.</exception>
		[DebuggerStepThrough]
		public static void False(bool condition, string paramName)
			=> False((bool?)condition, paramName, RS.Err_BooleanNotFalseParams);

		/// <summary>
		/// Verifies that the condition is <c>false</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <param name="paramName">The parameter name being tested.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>true</c>.</exception>
		[DebuggerStepThrough]
		public static void False(bool? condition, string paramName)
			=> False(condition, paramName, RS.Err_BooleanNotFalseParams);

		/// <summary>
		/// Verifies that the condition is <c>false</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <param name="paramName">The parameter name being tested.</param>
		/// <param name="message">The message to be shown if an exception occurs.</param>
		/// <param name="args">Format arguments for the <paramref name="message"/> argument.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>true</c>.</exception>
		[DebuggerStepThrough]
		public static void False(bool condition, string paramName, string message, params object[] args)
			=> False((bool?)condition, paramName, message, args);

		/// <summary>
		/// Verifies that the condition is <c>false</c>.
		/// </summary>
		/// <param name="condition">The condition to be tested.</param>
		/// <param name="paramName">The parameter name being tested.</param>
		/// <param name="message">The message to be shown if an exception occurs.</param>
		/// <param name="args">Format arguments for the <paramref name="message"/> argument.</param>
		/// <exception cref="ArgumentException">Thrown if the condition is <c>true</c>.</exception>
		[DebuggerStepThrough]
        public static void False(bool? condition, string paramName, string message, params object[] args)
        {
            if (!condition.HasValue || condition.GetValueOrDefault())
            {
                if (message == null)
                    message = RS.Err_BooleanNotFalse;
                else if (args != null)
                    message = string.Format(message, args);

                throw new ArgumentException(message, paramName);
            }
        }
    }
}