using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Standard;
using Standard.Core;

namespace Standard.Diagnostics
{
    public static partial class Assert
    {
        // NotNull

        /// <summary>
        /// Checks a string argument to ensure it is not null.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        [DebuggerStepThrough]
        public static void NotNull(string value)
            => NotNull(value, null);

        /// <summary>
        /// Checks a string argument to ensure it is not null.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void NotNull(string value, string paramName)
            => NotNull(value, paramName, null);

        /// <summary>
        /// Checks a string argument to ensure it is not null.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="paramName">The name of the argument</param>
        /// <param name="message">Custom error message thrown if assertion fails.</param>
        /// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
        [DebuggerStepThrough]
        public static void NotNull(string value, string paramName, string message, params object[] args)
        {
            if (value != null) 
                return;

            if (string.IsNullOrEmpty(message))
                message = RS.Err_StringIsNull;
            else if (args != null)            
                message = string.Format(message, args);

            throw new ArgumentException(message, paramName);
        }


        // NotNullOrEmpty

        /// <summary>
        /// Checks a string argument to ensure it is not null or empty.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        [DebuggerStepThrough]
        public static void NotNullOrEmpty(string value)
            => NotNullOrEmpty(value, null);

        /// <summary>
        /// Checks a string argument to ensure it is not null or empty.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void NotNullOrEmpty(string value, string paramName)
            => NotNullOrEmpty(value, null, null);

        /// <summary>
        /// Checks a string argument to ensure it is not null or empty.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="paramName">The name of the argument</param>
        /// <param name="message">Custom error message thrown if assertion fails.</param>
        /// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
        [DebuggerStepThrough]
        public static void NotNullOrEmpty(string value, string paramName, string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(value)) 
                return;

            if (string.IsNullOrEmpty(message))
                message = RS.Err_StringIsNullOrEmpty;
            else if (args != null)            
                message = string.Format(message, args);

            throw new ArgumentException(message, paramName);
        }


        // NotEmpty

        /// <summary>
        /// Checks a string argument to ensure it is not empty.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        [DebuggerStepThrough]
        public static void NotEmpty(string value)
            => NotEmpty(value, null);

        /// <summary>
        /// Checks a string argument to ensure it is not empty.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void NotEmpty(string value, string paramName)
            => NotEmpty(value, null, null);

        /// <summary>
        /// Checks a string argument to ensure it is not empty.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="paramName">The name of the argument</param>
        /// <param name="message">Custom error message thrown if assertion fails.</param>
        /// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
        [DebuggerStepThrough]
        public static void NotEmpty(string value, string paramName, string message, params object[] args)
        {
            if (!string.Empty.Equals(value)) 
                return;

            if (string.IsNullOrEmpty(message))
                message = RS.Err_StringIsEmpty;
            else if (args != null)            
                message = string.Format(message, args);

            throw new ArgumentException(message, paramName);
        }


        // NotNullOrWhiteSpace

        /// <summary>
        /// Checks a string argument to ensure it is not null or empty.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        [DebuggerStepThrough]
        public static void NotNullOrWhiteSpace(string value)
            => NotNullOrWhiteSpace(value, null);

        /// <summary>
        /// Checks a string argument to ensure it is not null or empty.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void NotNullOrWhiteSpace(string value, string paramName)
            => NotNullOrWhiteSpace(value, null, null);

        /// <summary>
        /// Checks a string argument to ensure it is not null or empty.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="paramName">The name of the argument</param>
        /// <param name="message">Custom error message thrown if assertion fails.</param>
        /// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
        [DebuggerStepThrough]
        public static void NotNullOrWhiteSpace(string value, string paramName, string message, params object[] args)
        {
            if (!StringUtility.IsNullOrWhiteSpace(value)) 
                return;

            if (string.IsNullOrEmpty(message))
                message = RS.Err_StringIsNullOrWhiteSpace;
            else if (args != null)            
                message = string.Format(message, args);

            throw new ArgumentException(message, paramName);
        }


        // Length

        /// <summary>
        /// Checks a string argument to ensure it satisfies a minumum and maxiumm length (both inclusive).
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="minLength">Expected minimum length of the value specified</param>
        /// <param name="maxLength">Expected maximum length of the value specified</param>
        [DebuggerStepThrough]
        public static void Length(string value, int minLength, int maxLength)
            => Length(value, minLength, maxLength, null);

        /// <summary>
        /// Checks a string argument to ensure it is exactly of a specified length.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="length">Expected length of the value specified</param>
        [DebuggerStepThrough]
        public static void Length(string value, int length)
            => Length(value, length, null);

        /// <summary>
        /// Checks a string argument to ensure its length is at least the specified value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="length">Expected minimum length of the value specified</param>
        [DebuggerStepThrough]
        public static void MinLength(string value, int length)
            => MinLength(value, length, null);

        /// <summary>
        /// Checks a string argument to ensure its length does not exceed the specified value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="length">Expected maximum length of the value specified</param>
        [DebuggerStepThrough]
        public static void MaxLength(string value, int length)
            => MaxLength(value, length, null);

        /// <summary>
        /// Checks a string argument to ensure it satisfies a minumum and maxiumm length (both inclusive).
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="minLength">Expected maximum length of the value specified</param>
        /// <param name="maxLength">Expected maximum length of the value specified</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void Length(string value, int minLength, int maxLength, string paramName)
            => Length(value, minLength, maxLength, paramName, null);

        /// <summary>
        /// Checks a string argument to ensure it is exactly of a specified length.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="length">Expected length of the value specified</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void Length(string value, int length, string paramName)
            => Length(value, length, paramName, null);

        /// <summary>
        /// Checks a string argument to ensure its length is at least the specified value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="length">Expected minimum length of the value specified</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void MinLength(string value, int length, string paramName)
            => MinLength(value, length, paramName, null);

        /// <summary>
        /// Checks a string argument to ensure its length does not exceed the specified value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="length">Expected maximum length of the value specified</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void MaxLength(string value, int length, string paramName)
            => MaxLength(value, length, paramName, null);

        /// <summary>
        /// Checks a string argument to ensure it satisfies a minumum and maxiumm length (both inclusive).
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="minLength">Expected mimimum length of the value specified</param>
        /// <param name="maxLength">Expected maximum length of the value specified</param>
        /// <param name="paramName">The name of the argument</param>
        /// <param name="message">Custom error message thrown if assertion fails.</param>
        /// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
        [DebuggerStepThrough]
        public static void Length(string value, int minLength, int maxLength, string paramName, string message, params object[] args)
        {
            if (minLength > maxLength)
                throw new ArgumentException(string.Format(RS.Err_MinGtMax, minLength, maxLength), 
                    nameof(minLength));

            if (value != null && 
                value.Length >= minLength && 
                value.Length <= maxLength)
            {
                return;
            }

            if (message == null)
            {
                if (value == null)
                    message = string.Format(RS.Err_StringIsNullLengthRangeMismatch, 
                        minLength, maxLength);
                else
                    message = string.Format(RS.Err_StringLengthRangeMismatch, 
                        minLength, maxLength, value.Length);                
            }
            else
            {
                if (args != null)
                    message = string.Format(message, args);
            }

            throw new ArgumentException(message, paramName);
        }

        /// <summary>
        /// Checks a string argument to ensure it is exactly of a specified length.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="length">Expected length of the value specified</param>
        /// <param name="paramName">The name of the argument</param>
        /// <param name="message">Custom error message thrown if assertion fails.</param>
        /// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
        [DebuggerStepThrough]
        public static void Length(string value, int length, string paramName, string message, params object[] args)
        {
            if (value != null && value.Length == length)
                return;

            if (message == null)
            {
                if (value == null)
                    message = string.Format(RS.Err_StringIsNullLengthMismatch, length);
                else
                    message = string.Format(RS.Err_StringLengthMismatch, length, value.Length);                
            }
            else
            {
                if (args != null)
                    message = string.Format(message, args);
            }

            throw new ArgumentException(message, paramName);
        }

        /// <summary>
        /// Checks a string argument to ensure it is at least a specified length.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="paramName">The name of the argument</param>
        /// <param name="length">Expected minimum length of the value specified</param>
        /// <param name="message">Custom error message thrown if assertion fails.</param>
        /// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
        [DebuggerStepThrough]
        public static void MinLength(string value, int length, string paramName, string message, params object[] args)
        {
            if (value != null && value.Length >= length)
                return;

            if (message == null)
            {
                if (value == null)
                    message = string.Format(RS.Err_StringIsNullRequireLength, length);
                else
                    message = string.Format(RS.Err_StringRequireLength, length, value.Length);                
            }
            else
            {
                if (args != null)
                    message = string.Format(message, args);
            }

            throw new ArgumentException(message, paramName);
        }

        /// <summary>
        /// Checks a string argument to ensure it is exactly of a specified length.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="paramName">The name of the argument</param>
        /// <param name="length">Expected length of the value specified</param>
        /// <param name="message">Custom error message thrown if assertion fails.</param>
        /// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
        [DebuggerStepThrough]
        public static void MaxLength(string value, int length, string paramName, string message, params object[] args)
        {
            if (value != null && value.Length <= length)
                return;

            if (value == null)
                return;

            if (message == null)
            {
                message = string.Format(RS.Err_StringExceedLength, length, value.Length);                
            }
            else
            {
                if (args != null)
                    message = string.Format(message, args);
            }

            throw new ArgumentException(message, paramName);
        }


		// IsLike

		/// <summary>
		/// Checks a string argument to ensure it satifies the wildcard expression specified.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="wildcard">The wildcard expression to evaluate.</param>
		[DebuggerStepThrough]
		public static void IsLike(string value, string wildcard)
			=> IsLike(value, wildcard, null, null, null);

		/// <summary>
		/// Checks a string argument to ensure it satifies the wildcard expression specified.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="wildcard">The wildcard expression to evaluate.</param>
		/// <param name="paramName">The name of the argument</param>
		[DebuggerStepThrough]
		public static void IsLike(string value, string wildcard, string paramName)
			=> IsLike(value, wildcard, paramName, null, null);

		/// <summary>
		/// Checks a string argument to ensure it satifies the wildcard expression specified.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="wildcard">The wildcard expression to evaluate.</param>
		/// <param name="paramName">The name of the argument</param>
		/// <param name="message">Custom error message thrown if assertion fails.</param>
		/// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
		[DebuggerStepThrough]
		public static void IsLike(string value, string wildcard, string paramName, string message, params object[] args)
		{
			if (value.IsLike(wildcard))
				return;

			if (message == null)
			{
				if (value == null)
					message = string.Format(RS.Err_StringIsNullWildcardMismatch, wildcard);
				else
					message = string.Format(RS.Err_WildcardMismatch, value, wildcard);
			}
			else if (args != null)
			{
				message = string.Format(message, args);
			}

			throw new ArgumentException(message, paramName);
		}


		// IsMatch

		/// <summary>
		/// Checks a string argument to ensure it satifies the RegEx expression specified.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="regex">The RegEx expression to evaluate</param>
		[DebuggerStepThrough]
        public static void IsMatch(string value, string regex)
            => IsMatch(value, new Regex(regex), null);

        /// <summary>
        /// Checks a string argument to ensure it satifies the RegEx expression specified.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="regex">The RegEx expression to evaluate</param>
        [DebuggerStepThrough]
        public static void IsMatch(string value, Regex regex)
            => IsMatch(value, regex, null);

        /// <summary>
        /// Checks a string argument to ensure it satifies the RegEx expression specified.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="regex">The RegEx expression to evaluate</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void IsMatch(string value, string regex, string paramName)
            => IsMatch(value, new Regex(regex), paramName);

        /// <summary>
        /// Checks a string argument to ensure it satifies the RegEx expression specified.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="regex">The RegEx expression to evaluate</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void IsMatch(string value, Regex regex, string paramName)
            => IsMatch(value, regex, paramName, null);

		/// <summary>
		/// Checks a string argument to ensure it satifies the RegEx expression specified.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="regex">The RegEx expression to evaluate</param>
		/// <param name="paramName">The name of the argument</param>
		/// <param name="message">Custom error message thrown if assertion fails.</param>
		/// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
		[DebuggerStepThrough]
		public static void IsMatch(string value, string regex, string paramName, string message, params object[] args)
			=> IsMatch(value, new Regex(regex), paramName, message, args);

		/// <summary>
		/// Checks a string argument to ensure it satifies the RegEx expression specified.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="regex">The RegEx expression to evaluate</param>
		/// <param name="paramName">The name of the argument</param>
		/// <param name="message">Custom error message thrown if assertion fails.</param>
		/// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
		[DebuggerStepThrough]
        public static void IsMatch(string value, Regex regex, string paramName, string message, params object[] args)
        {
            if (regex.IsMatch(value))
                return;

            if (message == null)
            {
                if (value == null)
                    message = string.Format(RS.Err_StringIsNullRegexMismatch, regex);
                else
                    message = string.Format(RS.Err_StringIsNullRegexMismatch, value, regex);                
            }
            else if (args != null)
            {
                message = string.Format(message, args);
            }

            throw new ArgumentException(message, paramName);
        }


        // Equality

        /// <summary>
        /// Checks a string argument to ensure it is equal to the compared value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="compareTo">The value to compare against.</param>
        [DebuggerStepThrough]
        public static void Equals(string value, string compareTo)
            => Equals(value, compareTo, null, null, null);

        /// <summary>
        /// Checks a string argument to ensure it is equal to the compared value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="compareTo">The value to compare against.</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void Equals(string value, string compareTo, string paramName)
            => Equals(value, compareTo, paramName, null, null);

		/// <summary>
		/// Checks a string argument to ensure it is equal to the compared value.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="compareTo">The value to compare against.</param>
		/// <param name="paramName">The name of the argument</param>
		/// <param name="message">Custom error message thrown if assertion fails.</param>
		/// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
		[DebuggerStepThrough]
        public static void Equals(string value, string compareTo, string paramName, string message, params object[] args)
        {
            if (string.Equals(value, compareTo))
                return;

            if (message == null)
                message = string.Format(RS.Err_StringNotEqual, compareTo, value);
            else if (args != null)
                message = string.Format(message, args);

            throw new ArgumentException(message, paramName);
        }

		/// <summary>
		/// Checks a string argument to ensure it is equal to the compared value.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="compareTo">The value to compare against.</param>
		[DebuggerStepThrough]
		public static void EqualsIgnoreCase(string value, string compareTo)
			=> EqualsIgnoreCase(value, compareTo, null, null, null);

		/// <summary>
		/// Checks a string argument to ensure it is equal to the compared value.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="compareTo">The value to compare against.</param>
		/// <param name="paramName">The name of the argument</param>
		[DebuggerStepThrough]
		public static void EqualsIgnoreCase(string value, string compareTo, string paramName)
			=> EqualsIgnoreCase(value, compareTo, paramName, null, null);

		/// <summary>
		/// Checks a string argument to ensure it is equal to the compared value.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="compareTo">The value to compare against.</param>
		/// <param name="paramName">The name of the argument</param>
		/// <param name="message">Custom error message thrown if assertion fails.</param>
		/// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
		[DebuggerStepThrough]
		public static void EqualsIgnoreCase(string value, string compareTo, string paramName, string message, params object[] args)
			=> Equals(value, compareTo, StringComparison.OrdinalIgnoreCase, paramName, message, args);

		/// <summary>
		/// Checks a string argument to ensure it is equal to the compared value.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="compareTo">The value to compare against.</param>
		/// <param name="comparer">Comparison options.</param>
		[DebuggerStepThrough]
        public static void Equals(string value, string compareTo, StringComparison comparer)
            => Equals(value, compareTo, comparer, null);

        /// <summary>
        /// Checks a string argument to ensure it is equal to the compared value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="compareTo">The value to compare against.</param>
        /// <param name="comparer">Comparison options.</param>        
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void Equals(string value, string compareTo, StringComparison comparer, string paramName)
            => Equals(value, compareTo, comparer, null, null);

        /// <summary>
        /// Checks a string argument to ensure it is equal to the compared value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="compareTo">The value to compare against.</param>
        /// <param name="comparer">Comparison options.</param>        
        /// <param name="paramName">The name of the argument</param>
        /// <param name="message">Custom error message thrown if assertion fails.</param>
        /// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
        [DebuggerStepThrough]
        public static void Equals(string value, string compareTo, StringComparison comparer, string paramName, string message, params object[] args)
        {
            if (string.Equals(value, compareTo, comparer))
                return;

            if (message == null)
                message = string.Format(RS.Err_StringNotEqual, compareTo, value);
            else if (args != null)
                message = string.Format(message, args);

            throw new ArgumentException(message, paramName);
        }


        // Inequality

        /// <summary>
        /// Checks a string value to ensure it is not equal to the compared value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="compareTo">The value to compare against.</param>
        [DebuggerStepThrough]
        public static void NotEquals(string value, string compareTo)
            => NotEquals(value, compareTo, null);

        /// <summary>
        /// Checks a string value to ensure it is not equal to the compared value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="compareTo">The value to compare against.</param>
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void NotEquals(string value, string compareTo, string paramName)
            => NotEquals(value, compareTo, null, null);

        /// <summary>
        /// Checks a string value to ensure it is not equal to the compared value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="compareTo">The value to compare against.</param>
        /// <param name="paramName">The name of the argument</param>
        /// <param name="message">Custom error message thrown if assertion fails.</param>
        /// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
        [DebuggerStepThrough]
        public static void NotEquals(string value, string compareTo, string paramName, string message, params object[] args)
        {
            if (!string.Equals(value, compareTo))
                return;

            if (message == null)
                message = string.Format(RS.Err_StringEqual,  compareTo, value);
            else if (args != null)
                message = string.Format(message, args);

            throw new ArgumentException(message, paramName);
        }

		/// <summary>
		/// Checks a string argument to ensure it is not equal to the compared value.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="compareTo">The value to compare against.</param>
		[DebuggerStepThrough]
		public static void NotEqualsIgnoreCase(string value, string compareTo)
			=> NotEqualsIgnoreCase(value, compareTo, null, null, null);

		/// <summary>
		/// Checks a string argument to ensure it is not equal to the compared value.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="compareTo">The value to compare against.</param>
		/// <param name="paramName">The name of the argument</param>
		[DebuggerStepThrough]
		public static void NotEqualsIgnoreCase(string value, string compareTo, string paramName)
			=> NotEqualsIgnoreCase(value, compareTo, paramName, null, null);

		/// <summary>
		/// Checks a string argument to ensure it is not equal to the compared value.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="compareTo">The value to compare against.</param>
		/// <param name="paramName">The name of the argument</param>
		/// <param name="message">Custom error message thrown if assertion fails.</param>
		/// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
		[DebuggerStepThrough]
		public static void NotEqualsIgnoreCase(string value, string compareTo, string paramName, string message, params object[] args)
			=> NotEquals(value, compareTo, StringComparison.OrdinalIgnoreCase, paramName, message, args);

		/// <summary>
		/// Checks a string value to ensure it is not equal to the compared value.
		/// </summary>
		/// <param name="value">The argument value to check</param>
		/// <param name="compareTo">The value to compare against.</param>
		/// <param name="comparer">Comparison options.</param>
		[DebuggerStepThrough]
        public static void NotEquals(string value, string compareTo, StringComparison comparer)
            => NotEquals(value, compareTo, comparer, null);

        /// <summary>
        /// Checks a string value to ensure it is not equal to the compared value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="compareTo">The value to compare against.</param>
        /// <param name="comparer">Comparison options.</param>        
        /// <param name="paramName">The name of the argument</param>
        [DebuggerStepThrough]
        public static void NotEquals(string value, string compareTo, StringComparison comparer, string paramName)
            => NotEquals(value, compareTo, comparer, null, null);

        /// <summary>
        /// Checks a string value to ensure it is not equal to the compared value.
        /// </summary>
        /// <param name="value">The argument value to check</param>
        /// <param name="compareTo">The value to compare against.</param>
        /// <param name="comparer">Comparison options.</param>        
        /// <param name="paramName">The name of the argument</param>
        /// <param name="message">Custom error message thrown if assertion fails.</param>
        /// <param name="args">Additional parameters to be passed for formatting the customized failure message.</param>
        [DebuggerStepThrough]
        public static void NotEquals(string value, string compareTo, StringComparison comparer, string paramName, string message, params object[] args)
        {
            if (!string.Equals(value, compareTo, comparer))
                return;

            if (message == null)
                message = string.Format(RS.Err_StringEqual, compareTo, value);
            else if (args != null)
                message = string.Format(message, args);

            throw new ArgumentException(message, paramName);
        }
    }
}