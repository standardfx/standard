using System;
using System.Diagnostics;
using Standard.Diagnostics.Core;

namespace Standard.Diagnostics
{
    public static partial class Assert
    {
        // Equals

        [DebuggerStepThrough]
        public static void Equals(long value, long compareTo)
            => Equals(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void Equals(long value, long compareTo, string paramName)
            => Equals(value, compareTo, paramName, null);

        [DebuggerStepThrough]
        public static void Equals(long value, long compareTo, string paramName, string message, params object[] args)
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
        public static void NotEquals(long value, long compareTo)
            => NotEquals(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void NotEquals(long value, long compareTo, string paramName)
            => NotEquals(value, compareTo, paramName, null);

        [DebuggerStepThrough]
        public static void NotEquals(long value, long compareTo, string paramName, string message, params object[] args)
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
        public static void GreaterThanOrEqualsTo(long value, long compareTo)
            => GreaterThanOrEqualsTo(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void GreaterThanOrEqualsTo(long value, long compareTo, string paramName)
            => GreaterThanOrEqualsTo(value, compareTo, paramName, null);

        [DebuggerStepThrough]
        public static void GreaterThan(long value, long compareTo)
            => GreaterThan(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void GreaterThan(long value, long compareTo, string paramName)
            => GreaterThan(value, compareTo, paramName, null);

        [DebuggerStepThrough]
        public static void GreaterThanOrEqualsTo(long value, long compareTo, string paramName, string message, params object[] args)
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
        public static void GreaterThan(long value, long compareTo, string paramName, string message, params object[] args)
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
        public static void LessThanOrEqualsTo(long value, long compareTo)
            => LessThanOrEqualsTo(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void LessThanOrEqualsTo(long value, long compareTo, string paramName)
            => LessThanOrEqualsTo(value, compareTo, paramName, null);

        [DebuggerStepThrough]
        public static void LessThan(long value, long compareTo)
            => LessThan(value, compareTo, null, null);

        [DebuggerStepThrough]
        public static void LessThan(long value, long compareTo, string paramName)
            => LessThan(value, compareTo, paramName, null);

        [DebuggerStepThrough]
        public static void LessThanOrEqualsTo(long value, long compareTo, string paramName, string message, params object[] args)
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
        public static void LessThan(long value, long compareTo, string paramName, string message, params object[] args)
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
        public static void Between(long value, long min, long max)
            => Between(value, min, max, null, null);

        [DebuggerStepThrough]
        public static void Between(long value, long min, long max, string paramName)
            => Between(value, min, max, paramName, null);

        [DebuggerStepThrough]
        public static void Between(long value, long min, long max, string paramName, string message, params object[] args)
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
