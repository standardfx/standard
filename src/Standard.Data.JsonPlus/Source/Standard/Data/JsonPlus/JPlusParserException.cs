using System;

namespace Standard.Data.JsonPlus
{
    /// <summary>
    /// Represents an error when parsing Json+ text.
    /// </summary>
    public class JPlusParserException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusParserException"/> class.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        public JPlusParserException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlusParserException"/> class.
        /// </summary>
        /// <param name="message">The message associated with the exception.</param>
        /// <param name="innerException">The inner exception associated with this exception.</param>
        public JPlusParserException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
