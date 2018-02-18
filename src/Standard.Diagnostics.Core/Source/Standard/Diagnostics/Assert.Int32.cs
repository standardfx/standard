using System;
using System.Diagnostics;
using Standard.Diagnostics.Core;

namespace Standard.Diagnostics
{
    public static partial class Assert
    {
        // Equals

        [DebuggerStepThrough]
        public static void Equals(int value, int compareTo)
            => Equals(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void Equals(int value, int compareTo, string paramName)
            => Equals(value, compareTo, paramName, null);

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

        [DebuggerStepThrough]
        public static void NotEquals(int value, int compareTo)
            => NotEquals(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void NotEquals(int value, int compareTo, string paramName)
            => NotEquals(value, compareTo, paramName, null);

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

        [DebuggerStepThrough]
        public static void GreaterThanOrEqualsTo(int value, int compareTo)
            => GreaterThanOrEqualsTo(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void GreaterThanOrEqualsTo(int value, int compareTo, string paramName)
            => GreaterThanOrEqualsTo(value, compareTo, paramName, null);

        [DebuggerStepThrough]
        public static void GreaterThan(int value, int compareTo)
            => GreaterThan(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void GreaterThan(int value, int compareTo, string paramName)
            => GreaterThan(value, compareTo, paramName, null);

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

        [DebuggerStepThrough]
        public static void LessThanOrEqualsTo(int value, int compareTo)
            => LessThanOrEqualsTo(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void LessThanOrEqualsTo(int value, int compareTo, string paramName)
            => LessThanOrEqualsTo(value, compareTo, paramName, null);

        [DebuggerStepThrough]
        public static void LessThan(int value, int compareTo)
            => LessThan(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void LessThan(int value, int compareTo, string paramName)
            => LessThan(value, compareTo, paramName, null);

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

        [DebuggerStepThrough]
        public static void Between(int value, int min, int max)
            => Between(value, min, max, null, null);

        [DebuggerStepThrough]
        public static void Between(int value, int min, int max, string paramName)
            => Between(value, min, max, paramName, null);

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
