using System;
using System.Diagnostics;
using Standard.Core;

namespace Standard.Diagnostics
{
    public static partial class Assert
    {
        // Equals

       /// <summary>
       /// If an object is equal to a specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
       /// </summary>
       /// <param name="value">The object to evaluate.</param>
       /// <param name="compareTo">The expected value.</param>
        [DebuggerStepThrough]
        public static void Equals(int value, int compareTo)
            => Equals(value, compareTo, null, null);

        /// <summary>
        /// If an object is equal to a specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        [DebuggerStepThrough]
        public static void Equals(int value, int compareTo, string paramName)
            => Equals(value, compareTo, paramName, null);

        /// <summary>
        /// If an object is equal to a specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        /// <param name="message">When throwing an <see cref="ArgumentException"/>, set the argument message to this value.</param>
        /// <param name="args">Format the <paramref name="message"/> string with the values set by this parameter.</param>
        [DebuggerStepThrough]
        public static void Equals(int value, int compareTo, string paramName, string message, params object[] args)
        {
            if (value == compareTo)
                return;

            if (string.IsNullOrEmpty(message))
                message = string.Format(RS.Err_NumberNotEqual, compareTo, value);
            else if (args != null)            
                message = string.Format(message, args);

            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException(message);
            else
                throw new ArgumentException(message, paramName);            
        }


        // NotEquals

        /// <summary>
        /// If an object is not equal to a specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        [DebuggerStepThrough]
        public static void NotEquals(int value, int compareTo)
            => NotEquals(value, compareTo, null, null);

        /// <summary>
        /// If an object is not equal to a specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        [DebuggerStepThrough]
        public static void NotEquals(int value, int compareTo, string paramName)
            => NotEquals(value, compareTo, paramName, null);

        /// <summary>
        /// If an object is not equal to a specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        /// <param name="message">When throwing an <see cref="ArgumentException"/>, set the argument message to this value.</param>
        /// <param name="args">Format the <paramref name="message"/> string with the values set by this parameter.</param>
        [DebuggerStepThrough]
        public static void NotEquals(int value, int compareTo, string paramName, string message, params object[] args)
        {
            if (value != compareTo)
                return;

            if (string.IsNullOrEmpty(message))
                message = string.Format(RS.Err_NumberEqual, compareTo);
            else if (args != null)            
                message = string.Format(message, args);

            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException(message);
            else
                throw new ArgumentException(message, paramName);            
        }


        // Greater Than

        /// <summary>
        /// If an object is greater than or equal to the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        [DebuggerStepThrough]
        public static void GreaterThanOrEqualsTo(int value, int compareTo)
            => GreaterThanOrEqualsTo(value, compareTo, null, null);

        /// <summary>
        /// If an object is greater than or equal to the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        [DebuggerStepThrough]
        public static void GreaterThanOrEqualsTo(int value, int compareTo, string paramName)
            => GreaterThanOrEqualsTo(value, compareTo, paramName, null);

        /// <summary>
        /// If an object is greater than the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        [DebuggerStepThrough]
        public static void GreaterThan(int value, int compareTo)
            => GreaterThan(value, compareTo, null, null);

        /// <summary>
        /// If an object is greater than the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        [DebuggerStepThrough]
        public static void GreaterThan(int value, int compareTo, string paramName)
            => GreaterThan(value, compareTo, paramName, null);

        /// <summary>
        /// If an object is greater than or equal to the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        /// <param name="message">When throwing an <see cref="ArgumentException"/>, set the argument message to this value.</param>
        /// <param name="args">Format the <paramref name="message"/> string with the values set by this parameter.</param>
        [DebuggerStepThrough]
        public static void GreaterThanOrEqualsTo(int value, int compareTo, string paramName, string message, params object[] args)
        {
            if (value >= compareTo)
                return;

            if (string.IsNullOrEmpty(message))
                message = string.Format(RS.Err_NumberNotGe, compareTo, value);
            else if (args != null)            
                message = string.Format(message, args);

            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException(message);
            else
                throw new ArgumentException(message, paramName);            
        }

        /// <summary>
        /// If an object is greater than the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        /// <param name="message">When throwing an <see cref="ArgumentException"/>, set the argument message to this value.</param>
        /// <param name="args">Format the <paramref name="message"/> string with the values set by this parameter.</param>
        [DebuggerStepThrough]
        public static void GreaterThan(int value, int compareTo, string paramName, string message, params object[] args)
        {
            if (value > compareTo)
                return;

            if (string.IsNullOrEmpty(message))
                message = string.Format(RS.Err_NumberNotGt, compareTo, value);
            else if (args != null)            
                message = string.Format(message, args);

            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException(message);
            else
                throw new ArgumentException(message, paramName);            
        }


        // Less Than

        /// <summary>
        /// If an object is less than or equal to the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        [DebuggerStepThrough]
        public static void LessThanOrEqualsTo(int value, int compareTo)
            => LessThanOrEqualsTo(value, compareTo, null, null);

        /// <summary>
        /// If an object is less than or equal to the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        [DebuggerStepThrough]
        public static void LessThanOrEqualsTo(int value, int compareTo, string paramName)
            => LessThanOrEqualsTo(value, compareTo, paramName, null);

        /// <summary>
        /// If an object is less than the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        [DebuggerStepThrough]
        public static void LessThan(int value, int compareTo)
            => LessThan(value, compareTo, null, null);

        /// <summary>
        /// If an object is less than the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        [DebuggerStepThrough]
        public static void LessThan(int value, int compareTo, string paramName)
            => LessThan(value, compareTo, paramName, null);

        /// <summary>
        /// If an object is less than or equal to the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        /// <param name="message">When throwing an <see cref="ArgumentException"/>, set the argument message to this value.</param>
        /// <param name="args">Format the <paramref name="message"/> string with the values set by this parameter.</param>
        [DebuggerStepThrough]
        public static void LessThanOrEqualsTo(int value, int compareTo, string paramName, string message, params object[] args)
        {
            if (value <= compareTo)
                return;

            if (string.IsNullOrEmpty(message))
                message = string.Format(RS.Err_NumberNotLe, compareTo, value);
            else if (args != null)            
                message = string.Format(message, args);

            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException(message);
            else
                throw new ArgumentException(message, paramName);            
        }

        /// <summary>
        /// If an object is less than the specified value, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="compareTo">The expected value.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        /// <param name="message">When throwing an <see cref="ArgumentException"/>, set the argument message to this value.</param>
        /// <param name="args">Format the <paramref name="message"/> string with the values set by this parameter.</param>
        [DebuggerStepThrough]
        public static void LessThan(int value, int compareTo, string paramName, string message, params object[] args)
        {
            if (value < compareTo)
                return;

            if (string.IsNullOrEmpty(message))
                message = string.Format(RS.Err_NumberNotLt, compareTo, value);
            else if (args != null)            
                message = string.Format(message, args);

            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException(message);
            else
                throw new ArgumentException(message, paramName);            
        }


        // Between

        /// <summary>
        /// If an object is between the specified range, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="min">The lower bound of the specified range. If the value is less than this value, an <see cref="ArgumentException"/> exception is thrown.</param>
        /// <param name="max">The upper bound of the specified range. If the value is greater than this value, an <see cref="ArgumentException"/> exception is thrown.</param>
        [DebuggerStepThrough]
        public static void Between(int value, int min, int max)
            => Between(value, min, max, null, null);

        /// <summary>
        /// If an object is between the specified range, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="min">The lower bound of the specified range. If the value is less than this value, an <see cref="ArgumentException"/> exception is thrown.</param>
        /// <param name="max">The upper bound of the specified range. If the value is greater than this value, an <see cref="ArgumentException"/> exception is thrown.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        [DebuggerStepThrough]
        public static void Between(int value, int min, int max, string paramName)
            => Between(value, min, max, paramName, null);

        /// <summary>
        /// If an object is between the specified range, nothing happens. Otherwise, an <see cref="ArgumentException"/> exception is thrown.
        /// </summary>
        /// <param name="value">The object to evaluate.</param>
        /// <param name="min">The lower bound of the specified range. If the value is less than this value, an <see cref="ArgumentException"/> exception is thrown.</param>
        /// <param name="max">The upper bound of the specified range. If the value is greater than this value, an <see cref="ArgumentException"/> exception is thrown.</param>
        /// <param name="paramName">When throwing an <see cref="ArgumentException"/>, set the argument parameter to this value.</param>
        /// <param name="message">When throwing an <see cref="ArgumentException"/>, set the argument message to this value.</param>
        /// <param name="args">Format the <paramref name="message"/> string with the values set by this parameter.</param>
        [DebuggerStepThrough]
        public static void Between(int value, int min, int max, string paramName, string message, params object[] args)
        {
            if (min > max)
                throw new ArgumentException(string.Format(RS.Err_MinGtMax, min, max), 
                    nameof(min));

            if (value >= min && value <= max)
                return;

            if (string.IsNullOrEmpty(message))
                message = string.Format(RS.Err_NumberNotBetween, min, max, value);
            else if (args != null)            
                message = string.Format(message, args);

            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException(message);
            else
                throw new ArgumentException(message, paramName);            
        }
    }
}
