using System;
using System.Collections.Generic;
using System.Linq;

namespace Standard.StringParsing
{
    /// <summary>
    /// Helper functions to create <see cref="IResult{T}"/> instances.
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <typeparam name="T">The type of the result (value).</typeparam>
        /// <param name="value">The sucessfully parsed value.</param>
        /// <param name="remainder">The remainder of the input.</param>
        /// <returns>
        /// The new <see cref="IResult{T}"/> representing a successful result.
        /// </returns>
        public static IResult<T> Success<T>(T value, IInput remainder)
        {
            return new Result<T>(value, remainder);
        }

        /// <summary>
        /// Creates a failure result.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="remainder">The remainder of the input.</param>
        /// <param name="message">The error message.</param>
        /// <param name="expectations">The parser expectations containing error messages.</param>
        /// <returns>
        /// The new <see cref="IResult{T}"/> representing a failed result.
        /// </returns>
        public static IResult<T> Failure<T>(IInput remainder, string message, IEnumerable<string> expectations)
        {
            return new Result<T>(remainder, message, expectations);
        }
    }

    internal class Result<T> : IResult<T>
    {
        private readonly T _value;
        private readonly IInput _remainder;
        private readonly bool _wasSuccessful;
        private readonly string _message;
        private readonly IEnumerable<string> _expectations;

        public Result(T value, IInput remainder)
        {
            _value = value;
            _remainder = remainder;
            _wasSuccessful = true;
            _message = null;
            _expectations = Enumerable.Empty<string>();
        }

        public Result(IInput remainder, string message, IEnumerable<string> expectations)
        {
            _value = default(T);
            _remainder = remainder;
            _wasSuccessful = false;
            _message = message;
            _expectations = expectations;
        }

        public T Value
        {
            get
            {
                if (!WasSuccessful)
                    throw new InvalidOperationException(RS.NoComputableValue);

                return _value;
            }
        }

        public bool WasSuccessful 
        { 
            get { return _wasSuccessful; } 
        }

        public string Message 
        { 
            get { return _message; } 
        }

        public IEnumerable<string> Expectations 
        { 
            get { return _expectations; } 
        }

        public IInput Remainder 
        { 
            get { return _remainder; } 
        }

        public override string ToString()
        {
            if (WasSuccessful)
                return string.Format(RS.ParseSuccessful, Value);

            string expMsg = string.Empty;

            if (Expectations.Any())
                expMsg = RS.Expected + RS.Space + Expectations.Aggregate((e1, e2) => e1 + RS.Space + RS.Or + RS.Space + e2);

            string recentlyConsumed = CalculateRecentlyConsumed();

            return string.Format(RS.ParseFailureInfo, Message, expMsg, Remainder, recentlyConsumed);
        }

        private string CalculateRecentlyConsumed()
        {
            const int windowSize = 10;

            int totalConsumedChars = Remainder.Position;
            int windowStart = totalConsumedChars - windowSize;
            windowStart = windowStart < 0 ? 0 : windowStart;

            int numberOfRecentlyConsumedChars = totalConsumedChars - windowStart;

            return Remainder.Source.Substring(windowStart, numberOfRecentlyConsumedChars);
        }
    }
}
